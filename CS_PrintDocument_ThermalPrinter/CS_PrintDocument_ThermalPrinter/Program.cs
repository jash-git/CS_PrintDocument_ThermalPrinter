using System;
using System.Drawing;
using System.Drawing.Printing;
using ZXing;
using ZXing.Windows.Compatibility;// NET5之後CS3050(Error CS0305 Using generic type 'BarcodeWriter ' requires type 1) 解決方法 : https://github.com/micjahn/ZXing.Net/issues/458
using ZXing.Common;
using ZXing.QrCode;
using System.Runtime.Intrinsics.Arm;

public class DPI_Funs
{
    //---
    //DPI 相關函數
    //https://blog.csdn.net/wangnaisheng/article/details/139059374
    public static void GetScreenDpi(out float DpiX, out float DpiY)//系統DPI
    {
        //Dots Per Inch(每英寸點數): 意思是指每一英吋長度中，取樣或可顯示或輸出點的數目
        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
        {
            DpiX = g.DpiX;
            DpiY = g.DpiY;
        }
    }
    public static float PixelsToInches(int pixels, float dpi)//像素 -> 英吋
    {
        return pixels / dpi;
    }
    public static float PixelsToMillimeters(int pixels, float dpi)//像素 -> 毫米
    {
        float inches = PixelsToInches(pixels, dpi);
        return inches * 25.4f;
    }
    public static int MillimetersToPixels(float millimeters, float dpi)//毫米 -> 像素
    {
        float inches = millimeters / 25.4f;
        return (int)(inches * dpi + 0.5f); // 使用0.5f进行四舍五入处理
    }
    //---DPI 相關函數
}

public class Barcode_Funs
{
    //1D條碼 和 2D條碼 產生器
    /*
    using ZXing;
    using ZXing.Windows.Compatibility;// NET5之後CS3050(Error CS0305 Using generic type 'BarcodeWriter ' requires type 1) 解決方法 : https://github.com/micjahn/ZXing.Net/issues/458
    using ZXing.Common;
    using ZXing.QrCode;  
    */

    public static Bitmap QrCode(String StrData)
    {
        // Create a BarcodeWriter instance
        var barcodeWriter = new BarcodeWriter();//ZXing.Windows.Compatibility
        barcodeWriter.Format = BarcodeFormat.QR_CODE;
        barcodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");  //編碼字元utf-8
        barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H); //錯誤校正等級
        barcodeWriter.Options.Height = 300;
        barcodeWriter.Options.Width = 300;
        barcodeWriter.Options.Margin = 0; //外邊距

        // Generate the barcode as a Bitmap
        Bitmap barcodeBitmap = barcodeWriter.Write(StrData);

        // Save the barcode as a BMP file
        barcodeBitmap.Save("qrcode.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        return barcodeBitmap;
    }
    public static Bitmap BarCode(String StrData)
    {
        // Create a BarcodeWriter instance
        var barcodeWriter = new BarcodeWriter();//ZXing.Windows.Compatibility
        barcodeWriter.Format = BarcodeFormat.CODE_128;
        barcodeWriter.Options.Height = 100;
        barcodeWriter.Options.Width = 300;

        // Generate the barcode as a Bitmap
        Bitmap barcodeBitmap = barcodeWriter.Write(StrData);

        // Save the barcode as a BMP file
        barcodeBitmap.Save("barcode.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        return barcodeBitmap;
    }

}

public class PrintTemplate_Fun
{

}
public class PrintTemplate
{
    protected string m_strPrinterName;
    protected PrintDocument m_PrintDocument;
    public PrintTemplate() 
    {
        m_PrintDocument = new PrintDocument();
        m_PrintDocument.PrintPage += new PrintPageEventHandler(PrintPage);
    }
    private void PrintPage(object sender, PrintPageEventArgs e)
    {
    }
}
class Program
{
    static void Main()
    {
        string targetPrinterName = "POS80D";//"POS-80C";//"80mm Series Printer";//"80mm_TCPMode"; // 替換成你實際的熱感印表機名稱
        PrintDocument printDoc = new PrintDocument();
        // 指定印表機
        bool found = false;
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            if (printer.Equals(targetPrinterName, StringComparison.OrdinalIgnoreCase))
            {
                printDoc.PrinterSettings.PrinterName = printer;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Console.WriteLine($"找不到印表機：{targetPrinterName}");
            return;
        }
    }
    static void Main_V0()
    {
        float SysDpiX, SysDpiY;
        DPI_Funs.GetScreenDpi(out SysDpiX,out SysDpiY);
        Bitmap bmp1 = Barcode_Funs.QrCode("相關網站: https://github.com/micjahn/ZXing.Net/issues/458");
        Bitmap bmp2 = Barcode_Funs.BarCode("1234567890");
        string targetPrinterName = "POS80D";//"POS-80C";//"80mm Series Printer";//"80mm_TCPMode"; // 替換成你實際的熱感印表機名稱

        PrintDocument printDoc = new PrintDocument();

        // 指定印表機
        bool found = false;
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            if (printer.Equals(targetPrinterName, StringComparison.OrdinalIgnoreCase))
            {
                printDoc.PrinterSettings.PrinterName = printer;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Console.WriteLine($"找不到印表機：{targetPrinterName}");
            return;
        }

        // 設定邊界為 0
        printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

        // 加入列印事件
        printDoc.PrintPage += (sender, e) =>
        {
            /*
            //https://learn.microsoft.com/zh-tw/dotnet/api/system.drawing.graphicsunit?view=windowsdesktop-9.0&viewFallbackFrom=dotnet-plat-ext-8.0
            Display	1	
            指定顯示裝置的測量單位。 一般來說，視訊顯示會使用像素，而印表機會使用 1/100 英吋。

            Document	5	
            指定文件單位 (1/300 英吋) 做為測量單位。

            Inch	4	
            指定英吋做為測量單位。

            Millimeter	6	
            指定公釐做為測量單位。

            Pixel	2	
            指定裝置像素做為測量單位。

            Point	3	
            指定印表機的點 (1/72 英吋) 做為測量單位。

            World	0	
            指定全局座標系統的單位做為測量單位。            
            */
            //e.Graphics.PageUnit = GraphicsUnit.Document;//300DPI ~ https://radio-idea.blogspot.com/2016/09/c-printdocument.html#google_vignette
            e.Graphics.PageUnit = GraphicsUnit.Pixel;//解析度 ~ https://radio-idea.blogspot.com/2016/09/c-printdocument.html#google_vignette

            Graphics g = e.Graphics;//抓取印表機畫布
            /*
            Bitmap BitmapBuf = new Bitmap((int)(80 * 3.937),5000);//建立BMP記憶體空間
            Graphics g = Graphics.FromImage(BitmapBuf);//從BMP記憶體自建畫布 ~ https://stackoverflow.com/questions/10868623/converting-print-page-graphics-to-bitmap-c-sharp
            g.Clear(Color.White);//畫布指定底色
            //*/

            Font font = new Font("新細明體", 9);
            Brush brush = Brushes.Black;

            int y = 10;

            /*
             熱敏印表機實驗取得數據:
                78mm : 78-3*2(設備邊界) = 72mm
                56mm : 56-3*2(設備邊界) = 50mm
                大小紙張可列印之差: (72-50)=22mm
            */
            g.DrawString("050", new Font("新細明體", 40, FontStyle.Bold), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(14, 203);//14=字高+1

            g.DrawString("外帶", new Font("新細明體", 28, FontStyle.Bold), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(12, 203);//12=字高+1

            g.DrawString("電子發票證明聯", new Font("新細明體", 16), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(7, 203);//7=字高+1

            g.DrawString("114年5-6月", new Font("新細明體", 16), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(7, 203);//7=字高+1

            g.DrawString("AA-67241100(測)", new Font("新細明體", 16), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(7, 203);//7=字高+1

            g.DrawString("2025-05-05 09:27:15", font, brush, 10, y);
            y += DPI_Funs.MillimetersToPixels(4, 203);//4=字高+1

            g.DrawString("隨機碼:7207        總計:160", font, brush, 10, y);
            y += DPI_Funs.MillimetersToPixels(4, 203);//4=字高+1

            g.DrawString("文字置中測試", new Font("新細明體", 13, FontStyle.Bold), brush, DPI_Funs.MillimetersToPixels(((78-12) / 2 - (5*3)), 203), y);//紙張寬度((78-12)/2)-3個字寬(4*3)
            y += DPI_Funs.MillimetersToPixels(6, 203);//6=字高+1

            g.DrawString("文字置中測試", new Font("新細明體", 13), brush, DPI_Funs.MillimetersToPixels(((78 - 12) / 2 - (5*3)), 203), y);//紙張寬度((78-12)/2)-3個字寬(3*3)
            y += DPI_Funs.MillimetersToPixels(6, 203);//5=字高+1

            // 列印標題
            g.DrawString("收據列印示範(粗體)", new Font("新細明體", 12, FontStyle.Bold), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(5, 203);//5=字高+1

            g.DrawString("收據列印示範", new Font("新細明體", 12), brush, 0, y);
            y += DPI_Funs.MillimetersToPixels(5, 203);//5=字高+1

            // 繪製文字
            g.DrawString("商品：測試產品", font, brush, 10, y);
            y += DPI_Funs.MillimetersToPixels(4, 203);//4=字高+1
            g.DrawString("數量：2", font, brush, 10, y);
            y += DPI_Funs.MillimetersToPixels(4, 203);//4=字高+1
            g.DrawString("總價：NT$200", font, brush, 10, y);
            y += DPI_Funs.MillimetersToPixels(10, 203);

            // 繪製方形區塊（模擬框）
            Pen pen = new Pen(Color.Black, 1);
            Rectangle rect = new Rectangle(0, y, DPI_Funs.MillimetersToPixels(68, 203), DPI_Funs.MillimetersToPixels(68, 203));
            g.DrawRectangle(pen, rect);
            g.DrawString("感謝您的購買！", font, brush, 10, y + 15);

            Rectangle printArea2 = new Rectangle(5, y+DPI_Funs.MillimetersToPixels(70, 203), DPI_Funs.MillimetersToPixels(30 * 1.1f, 203), DPI_Funs.MillimetersToPixels(15 * 1.1f, 203));//new RectangleF(0, 0, e.PageBounds.Width, e.PageBounds.Height);//
            g.DrawImage(bmp2, printArea2);

            Rectangle printArea1 = new Rectangle(5, y+DPI_Funs.MillimetersToPixels(70, 203) + DPI_Funs.MillimetersToPixels(17 * 1.1f, 203), DPI_Funs.MillimetersToPixels(40*1.1f, 203), DPI_Funs.MillimetersToPixels(40 * 1.1f, 203));//new RectangleF(0, 0, e.PageBounds.Width, e.PageBounds.Height);//
            g.DrawImage(bmp1, printArea1);

            //---
            //測試大圖
            /*
            y = 410 + 100 + 200 + 20;
            for (int i=0;i<100;i++)
            {
                g.DrawString(i+":", font, brush, 10, y);
                y += 20;
            }
            */
            //---測試大圖

            Font font01 = new Font("新細明體", 1);
            g.DrawString("               .", font01, brush, 10, 1500);//故意拉長紙張 (500-300-y)/3.937=25mm

            /*
            BitmapBuf.Save("printer.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            e.Graphics.DrawImage(BitmapBuf, 0, 0);//由於從BMP記憶體自建畫布，所以列印時就要從BMP記憶體進行
            //*/

            e.HasMorePages = false;
        };

        // 自訂紙張大小：寬 80mm，高 100mm（1mm ≈ 3.937 尺寸單位）
        int width = (int)(80 * 3.937);  // 約 315
        int height = 5000;//(int)(100 * 3.937); // 約 394

        PaperSize paperSize = new PaperSize("Custom_80mm", width, height);
        printDoc.DefaultPageSettings.PaperSize = paperSize;

        try
        {
            printDoc.Print();
            Console.WriteLine("列印工作已送出！");
        }
        catch (Exception ex)
        {
            Console.WriteLine("列印失敗：" + ex.Message);
        }
    }
}