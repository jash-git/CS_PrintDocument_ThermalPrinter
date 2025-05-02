C# .net8 的console 專案 指定對應熱敏印表機驅動 並使用 PrintDocument 設定紙張大小撰寫一個完整的列印文字和繪製方形的範例 

資料來源: chatgpt

01.安裝元件:
dotnet add package System.Drawing.Common

============================

C# .net8 Qrcode/Barcode 產生

資料來源: 
NET5之後CS3050(Error CS0305 Using generic type 'BarcodeWriter ' requires type 1) 解決方法 : https://github.com/micjahn/ZXing.Net/issues/458

01.安裝元件:
dotnet add package ZXing.Net
dotnet add package ZXing.Net.Bindings.Windows.Compatibility

============================

C# 印表機(PrintDocument的解析度 設定) ~  像素(Pixels)/英吋(Inches)/毫米(Millimeters) 轉換函數

資料來源:
https://blog.csdn.net/wangnaisheng/article/details/139059374
https://learn.microsoft.com/zh-tw/dotnet/api/system.drawing.graphicsunit?view=windowsdesktop-9.0&viewFallbackFrom=dotnet-plat-ext-8.0
https://radio-idea.blogspot.com/2016/09/c-printdocument.html#google_vignette


e.Graphics.PageUnit = GraphicsUnit.Pixel;
"Arial", 12, FontStyle.Bold -> Width = Height = 4mm
"Arial", 10                 -> Width = Height = 3nm
y+=X						-> Height(X) = 字高+1 (mm)

============================

