using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Security.Cryptography;

namespace tslinkcn.tools
{
    public static class PublicTools
    {

        public static int GetCellIntFromChar(char x)
        {
            int offsetLower = ((int)'a') - 1;
            int offsetUpper = ((int)'A') - 1;

            if (Char.IsLower(x))
            {
                return ((int)x) - offsetLower;
            }

            if (Char.IsUpper(x))
            {
                return ((int)x) - offsetUpper;
            }

            return int.MinValue;
        }

        public static double GetCellNumic(IRow row, int y)
        {
            double _r = double.MinValue;

            ICell cell = row.Cells[y - 1];
            if (cell != null)
                switch (cell.CellType)
                {
                    case CellType.String:
                        {

                            _r = double.Parse(cell.StringCellValue.Trim());

                        }
                        break;
                    case CellType.Numeric:
                        {
                            _r = cell.NumericCellValue;
                        }
                        break;
                    default:
                        {
                            _r = 0;
                        }
                        break;
                }

            return _r;

        }

        public static double GetCellNumic(ISheet sheet, char x, int y)
        {
            double _r = double.MinValue;

            ICell cell = sheet.GetRow(y - 1).GetCell(GetCellIntFromChar(x) - 1);
            if (cell != null)
                switch (cell.CellType)
                {
                    case CellType.String:
                        {
                            _r = double.Parse(cell.StringCellValue.Trim());

                        }
                        break;
                    case CellType.Numeric:
                        {
                            _r = cell.NumericCellValue;
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }

            return _r;

        }
        public static void SetFitCharFont(Font MaxFont, int TotolSpace, string StringCon, out Font FitFont, out int StartOffset)
        {
            FitFont = MaxFont;
            StartOffset = 1;
            while (true)
            {
                int StringWidth = TextRenderer.MeasureText(StringCon, FitFont).Width;
                if (StringWidth < TotolSpace - 2)
                {
                    break;
                }
                else
                {
                    FitFont = new Font(FitFont.FontFamily, FitFont.Size - 2, FitFont.Unit);
                }
            }

            StartOffset = (TotolSpace - TextRenderer.MeasureText(StringCon, FitFont).Width) / (int)(2 * 3.8);
        }
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;

        }

        public static Point local(Control c)
        {
            Point p = c.Location;

            if (c.Parent != null)
            {
                Point pp = local(c.Parent);
                p.X += pp.X + c.Padding.Left + c.Margin.Left;
                p.Y += pp.Y + c.Padding.Top + c.Margin.Left;
            }
            else
            {
                p.Y += 26;
            }

            return p;
        }
        public static void RecountRowsNum(DataGridView dgv)
        {
            foreach (DataGridViewRow r in dgv.Rows)
            {
                r.Cells[0].Value = r.Index + 1;
            }
            dgv.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Refresh();
        }

        public static void IniDatagridview(DataGridView dgv)
        {
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowDrop = false;
            dgv.ReadOnly = true;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.BackgroundColor = Color.FromKnownColor(KnownColor.Control);
            dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            dgv.DefaultCellStyle.Padding = new Padding(2);
            DataGridViewCellStyle dvcs = new DataGridViewCellStyle();
            dvcs.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle = dvcs;

        }

        public static void SetColumsAutoModeNone(DataGridView dgv)
        {
            foreach (DataGridViewColumn dgvc in dgv.Columns)
            {
                dgvc.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        public static void SaveColumnWidth(DataGridView dgv, string path)
        {
            using (StreamWriter w = new StreamWriter(path))
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.Visible)
                        w.WriteLine(string.Format("{0},{1}", col.HeaderText, col.Width));
                }
            }
        }

        public static void RecoverColumnWidth(DataGridView dgv, string path)
        {
            if (File.Exists(path) == false) return;
            using (StreamReader w = new StreamReader(path))
            {
                while (!w.EndOfStream)
                {
                    string[] readlinetmp = w.ReadLine().Split(',');
                    int colWidth = 0;
                    if (readlinetmp.Length == 2 && int.TryParse(readlinetmp[1], out colWidth))
                    {
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (dgv.Columns[i].HeaderText.Equals(readlinetmp[0]))
                            {
                                dgv.Columns[i].Width = colWidth;
                                dgv.Columns[i].MinimumWidth = 2;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void ReSizeTextbox(Control ctl)
        {

            float charCount = (float)ctl.Text.Length;
            float fontsize = ctl.Font.Size;
            float charlength = fontsize * charCount;
            float tbWidth = (float)ctl.Width;
            if (charlength > tbWidth)
            {
                ctl.Font = new Font(ctl.Font.FontFamily, ctl.Font.Size - 2);
                Point p = ctl.Location;
                ctl.Location = new Point(p.X, p.Y + 2);
                ReSizeTextbox(ctl);

            }
        }

        //public static Bitmap CreateQRCode(string content)
        //{
        //    Gma.QrCodeNet.Encoding.QrEncoder qrEncoder = new Gma.QrCodeNet.Encoding.QrEncoder();
        //    //Gma.QrCodeNet.Encoding.
        //    qrEncoder.ErrorCorrectionLevel = Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M;
        //    qrEncoder.Encode()
        //  //  QRCodeEncoder qrEncoder = new QRCodeEncoder();
        //    qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
        //    qrEncoder.QRCodeScale = Convert.ToInt32(4);
        //    qrEncoder.QRCodeVersion = Convert.ToInt32(3);
        //    qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
        //    try
        //    {
        //        Bitmap qrcode = qrEncoder.Encode(content, Encoding.UTF8);
        //        return qrcode;
        //    }
        //    catch (IndexOutOfRangeException ex)
        //    {
        //        return new Bitmap(100, 100);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Bitmap(100, 100);
        //    }
        //}

        public static void WritePrintername(string printerName)
        {
            INIClass iniclass = new INIClass("config.ini");
            iniclass.IniWriteValue("Other", "printer", printerName);
        }

        public static string ReadPrinterName()
        {
            INIClass iniclass = new INIClass("config.ini");
            return iniclass.IniReadValue("Other", "printer");

        }

        /// </summary>    

        private static PrintDocument fPrintDocument = new PrintDocument();
        /// <summary>    
        /// 获取本机默认打印机名称    
        /// </summary>    
        public static String DefaultPrinter
        {
            get { return fPrintDocument.PrinterSettings.PrinterName; }
        }

        /// <summary>    
        /// 获取本机的打印机列表。列表中的第一项就是默认打印机。    
        /// </summary>    
        public static List<String> GetLocalPrinters()
        {
            List<String> fPrinters = new List<string>();
            fPrinters.Add(DefaultPrinter); // 默认打印机始终出现在列表的第一项    
            foreach (String fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            return fPrinters;

        }

    }



}
