using System;
using System.Drawing;
using System.Drawing.Printing;

class Program
{
    static void Main()
    {
        string targetPrinterName = "POS-80C"; // 替換成你實際的熱感印表機名稱

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

        // 自訂紙張大小：寬 80mm，高 100mm（1mm ≈ 3.937 尺寸單位）
        int width = (int)(80 * 3.937);  // 約 315
        int height = 1000;//(int)(100 * 3.937); // 約 394

        PaperSize paperSize = new PaperSize("Custom_80mm", width, height);
        printDoc.DefaultPageSettings.PaperSize = paperSize;

        // 加入列印事件
        printDoc.PrintPage += (sender, e) =>
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 10);
            Brush brush = Brushes.Black;

            int y = 10;

            // 列印標題
            g.DrawString("收據列印示範", new Font("Arial", 12, FontStyle.Bold), brush, 0, y);
            y += 30;

            // 繪製文字
            g.DrawString("商品：測試產品", font, brush, 10, y);
            y += 20;
            g.DrawString("數量：2", font, brush, 10, y);
            y += 20;
            g.DrawString("總價：NT$200", font, brush, 10, y);
            y += 30;

            // 繪製方形區塊（模擬框）
            Pen pen = new Pen(Color.Black, 1);
            Rectangle rect = new Rectangle(5, y, 270, 300);//3.937pixel≒1mm (200=50nn,300=76mm)
            g.DrawRectangle(pen, rect);
            g.DrawString("感謝您的購買！", font, brush, 10, y + 15);

            Font font01 = new Font("Arial", 1);
            g.DrawString("               .", font01, brush, 10, 500);//故意拉長紙張 (500-300-y)/3.937=25mm

            e.HasMorePages = false;
        };

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