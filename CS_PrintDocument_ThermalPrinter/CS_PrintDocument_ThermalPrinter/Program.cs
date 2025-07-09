using System;
using System.Drawing;
using System.Drawing.Printing;
using ZXing;
using ZXing.Windows.Compatibility;// NET5之後CS3050(Error CS0305 Using generic type 'BarcodeWriter ' requires type 1) 解決方法 : https://github.com/micjahn/ZXing.Net/issues/458
using ZXing.Common;
using ZXing.QrCode;
using System.Text.Json;
using CS_PrintDocument_ThermalPrinter;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.Intrinsics.X86;
using System.Text;
public class TimeConvert
{
    //https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp, bool blnUTC = true)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        try
        {
            if (blnUTC)
            {
                dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();//from php vteam api to C# 使用
            }
            else
            {
                dateTime = dateTime.AddSeconds(unixTimeStamp);//from SQLite STRFTIME('%s',max(report_time)) to C# 使用
            }
        }
        catch (Exception ex)
        {
            String StrLog = String.Format("{0}: {1}", "TimeConvert_UnixTimeStampToDateTime() Error", ex.ToString());
        }

        return dateTime;
    }

    //https://ourcodeworld.com/articles/read/865/how-to-convert-an-unixtime-to-datetime-class-and-viceversa-in-c-sharp
    public static long DateTimeToUnixTimeStamp(DateTime MyDateTime)
    {
        /*
        TimeSpan timeSpan = MyDateTime - new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)(timeSpan.TotalSeconds-8*60*60);//8*60*60 來源: GMT+08:00 ~ https://www.epochconverter.com/
        */

        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = MyDateTime.ToUniversalTime() - origin;
        return (long)(diff.TotalSeconds);//https://stackoverflow.com/questions/3354893/how-can-i-convert-a-datetime-to-the-number-of-seconds-since-1970
    }

    public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
    {
        // Java timestamp is milliseconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
        return dateTime;
    }
}//TimeConvert

public class BitmapBase64_Funs//圖片和Base64戶轉
{
    //---
    //相關線上測試工具
    // https://www.base64-image.de/
    // https://base64.guru/converter/encode/image
    // https://base64.guru/converter/decode/file
    //---相關線上測試工具

    //圖片轉換base64字串
    public static string Image2Base64String(Bitmap bmp)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    //base64字串轉換圖片
    public static Bitmap Base64String2Image(string strbase64)
    {
        try
        {
            byte[] arr = Convert.FromBase64String(strbase64);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();
            return bmp;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}

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

    public static Bitmap QrCode(String StrData,String ErrorCorrection="H")//文字轉QrCode
    {
        // Create a BarcodeWriter instance
        var barcodeWriter = new BarcodeWriter();//ZXing.Windows.Compatibility
        barcodeWriter.Format = BarcodeFormat.QR_CODE;
        barcodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");  //編碼字元utf-8
        switch(ErrorCorrection)
        {
            case "H"://30%
                barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H); //錯誤校正等級
                break;
            case "Q"://25%
                barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.Q); //錯誤校正等級
                break;
            case "M"://15%
                barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.M); //錯誤校正等級
                break;
            case "L"://7%
                barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.L); //錯誤校正等級
                break;
        }
        barcodeWriter.Options.Height = 300;
        barcodeWriter.Options.Width = 300;
        barcodeWriter.Options.Margin = 0; //外邊距

        // Generate the barcode as a Bitmap
        Bitmap barcodeBitmap = barcodeWriter.Write(StrData);

        // Save the barcode as a BMP file
        barcodeBitmap.Save("qrcode.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        return barcodeBitmap;
    }
    public static Bitmap BarCode(String StrData,int Height=40,int Width=320)//文字轉BarCode
    {
        // Create a BarcodeWriter instance
        var barcodeWriter = new BarcodeWriter();//ZXing.Windows.Compatibility
        barcodeWriter.Format = BarcodeFormat.CODE_39;//電子發票規定
        barcodeWriter.Options.Height = Height;//>=5mm(203dpi->40pixel)電子發票規定
        barcodeWriter.Options.Width = Width;//>=4cm(40mm)(203dpi->320pixel)電子發票
        barcodeWriter.Options.PureBarcode = true; //不顯示條碼文字[false 為顯示 true為不顯示]
        barcodeWriter.Options.Margin = 0;//左右填充

        // Generate the barcode as a Bitmap
        Bitmap barcodeBitmap = barcodeWriter.Write(StrData);

        // Save the barcode as a BMP file
        barcodeBitmap.Save("barcode.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

        return barcodeBitmap;
    }

}

//資料元件結構
public class PT_ChildElement
{
    public string ElementType { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string X_Alignment { get; set; }
    public string Y_Alignment { get; set; }
    public int Index { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Content { get; set; }
    public string RootName { get; set; }//Block,Rows
    public List<PT_ChildElement> ChildElements { get; set; }//Block,Rows
    public int Rotation { get; set; }//Text,QrCode,BarCode,Image
    public string VerticalContentAlig { get; set; }//Text
    public string HorizontalContentAlig { get; set; }//Text
    public int? ContentSize { get; set; }//Text
    public string ContentBold { get; set; }//Text
    public int? VerticalSpacing { get; set; }//Text
    public string IntervalSymbols { get; set; }//Text
    public string AutoWrap { get; set; }//Text
    public int? RowSpacing { get; set; }//Table
    public string AlwaysPrint { get; set; }//Table
    public string DisplayMode { get; set; }//Rows
    public string ErrorCorrection { get; set; }//QrCode
    public string Format { get; set; }//BarCode
}
public class ContainerElement
{//存放在堆疊內的原件
   public PT_ChildElement m_Element;
    public int m_index;
    public ContainerElement(PT_ChildElement element, int index)
    {
        m_Element = element;
        m_index = index;
    }
}
public class PT_Page
{
    public string Content { get; set; }
    public string ElementType { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string X_Alignment { get; set; }
    public string Y_Alignment { get; set; }
    public int Index { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Spacing { get; set; }
    public int Rotation { get; set; }
    public int ZoomRatio { get; set; }
    public string FontName { get; set; }
    public string StartBuzzer { get; set; }
    public string ExternalBuzzer { get; set; }
    public string BuzzerCmd { get; set; }
    public string ExtBuzzerCmd { get; set; }
    public string PrintMode { get; set; }
    public List<PT_ChildElement> ChildElements { get; set; }
}
public class CS_PrintTemplate_Fun
{
    //---
    //JsonDocument搜尋函數 

    // 主搜尋邏輯
    public static void FindInElement(JsonElement element, string searchKey, Dictionary<string, string> results, bool partialMatch, string currentPath, bool findAllMatches)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (JsonProperty prop in element.EnumerateObject())
                {
                    string newPath = $"{currentPath}.{prop.Name}";
                    bool isMatch = partialMatch
                        ? prop.Name.Contains(searchKey, StringComparison.OrdinalIgnoreCase)
                        : prop.NameEquals(searchKey);

                    if (isMatch && !results.ContainsKey(newPath))
                        results.Add(newPath, prop.Value.ToString());

                    if (!findAllMatches && results.Count > 0)
                    {
                        return; // Stop searching once we find the first match
                    }

                    FindInElement(prop.Value, searchKey, results, partialMatch, newPath, findAllMatches);
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (JsonElement item in element.EnumerateArray())
                {
                    string newPath = $"{currentPath}[{index}]";
                    FindInElement(item, searchKey, results, partialMatch, newPath, findAllMatches);
                    index++;

                    if (!findAllMatches && results.Count > 0)
                    {
                        return; // Stop searching once we find the first match
                    }
                }
                break;
        }
    }

    // 根據 JSON 路徑取出對應 JsonElement
    public static bool TryGetElementAtPath(JsonElement root, string path, out JsonElement result)
    {
        result = root;

        if (string.IsNullOrEmpty(path))
            return true;

        string[] parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            if (result.ValueKind == JsonValueKind.Object)
            {
                string key = part;
                int bracketIndex = part.IndexOf('[');
                if (bracketIndex != -1)
                {
                    key = part[..bracketIndex];
                }

                if (!result.TryGetProperty(key, out result))
                    return false;

                if (bracketIndex != -1)
                {
                    if (!TryParseArrayAccess(ref result, part[bracketIndex..]))
                        return false;
                }
            }
            else if (result.ValueKind == JsonValueKind.Array)
            {
                if (!TryParseArrayAccess(ref result, part))
                    return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static bool TryParseArrayAccess(ref JsonElement element, string bracketPart)
    {
        while (bracketPart.StartsWith("["))
        {
            int endBracket = bracketPart.IndexOf(']');
            if (endBracket == -1)
                return false;

            string indexStr = bracketPart[1..endBracket];
            if (!int.TryParse(indexStr, out int index))
                return false;

            if (element.ValueKind != JsonValueKind.Array || element.GetArrayLength() <= index)
                return false;

            element = element[index];

            bracketPart = bracketPart[(endBracket + 1)..];
        }

        return true;
    }

    //---JsonDocument搜尋函數 
}

//資料元件結構
public class ForLoopVar
{
    public string m_strName;
    public int m_intCount;
    public int m_intIndex;
    public ForLoopVar(string strName,int intCount,int intIndex=-1) 
    {
        m_strName = strName;//資料及名稱
        m_intCount = intCount;//資料及總比數
        m_intIndex = intIndex;//目前存取在第幾筆的旗標，-1:表示intCount還未初始化
    }
}
public class CS_PrintTemplate
{
    protected PrintDocument m_PrintDocument;
    //protected JsonDocument m_JsonDocument;
    protected PT_Page m_PT_Page = null;
    protected orders_new m_OrderDataAll = null;
    protected orders_new m_OrderData = null;//實際運算參與運算用

    //---
    //繪圖系統變數
    private Font m_NormalFont;//一般字 Height =3mm ~[0]
    private Font m_BigFont;//單倍字 Height =5mm ~[1]
    private Font m_InvoiceFont;//發票 Height =6mm ~[2]
    private Font m_DoubleFont;//雙倍字 Height =11mm ~[3]
    private Font m_FourFont;//四倍字 Height =13mm ~[4]
    private float [] m_fltFontHeight =new float[5] {3,5,6,11,13};//由小到大,單位為 mm
    private float m_fltSysDpi = 0;//印表DPI
    private float m_fltLast_X = 0;//最後列印定位點X座標
    private float m_fltLast_Y = 0;//最後列印定位點Y座標
    private float m_fltMax_Height = 0;//同列最大列印高度
    private float m_fltLast_Height = 0;//最後列印最大高度
    //---繪圖系統變數

    //---
    //運算系統變數
    private Stack<ContainerElement> m_ContainerElements = new Stack<ContainerElement>();//存放容器物件
    //private Stack<ContainerElement> m_RecycleElements = new Stack<ContainerElement>();//回收
    private List<ForLoopVar> m_ForLoopVars = new List<ForLoopVar>();//存放迴圈索引變數(資料集名稱列表，不再裡面的都是變數名稱)
    private string m_strDataPath = "";//目前資料集路徑階層
    private int m_intDataPath = -1;
    private string m_strRealData = "";//目前真實資料
    private string m_strDisplayMode = "Single";//顯示模式: Single/Merge ~ 元件預先處理被更動
    private int m_intPages = 1;//此範本一次要列印次數(一菜一切)
    //---運算系統變數

    public bool m_blnResult;
    public string m_strResult;
    public CS_PrintTemplate(string strPrinterDriverName,string strPrintTemplate,string strOrderData)//建構子 
    {
        try
        {
            bool blnPrinterFound = false;
            bool blnPrintTemplateCreated = false;
            //---
            //驅動程式名稱比對
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                if (printer.Equals(strPrinterDriverName, StringComparison.OrdinalIgnoreCase))
                {
                    blnPrinterFound = true;
                    break;
                }
            }
            //---驅動程式名稱比對

            //---
            //json2object
            // m_JsonDocument = JsonDocument.Parse(strPrintTemplate);
            m_PT_Page = JsonSerializer.Deserialize<PT_Page>(strPrintTemplate);
            if((m_PT_Page!=null) && (m_PT_Page.ChildElements!=null) && (m_PT_Page.ChildElements.Count>0))
            {
                // 就地修改 ChildElements 的順序，依照 Index 遞增排序 //如果你要遞減排序，改成 b.Index.CompareTo(a.Index)
                m_PT_Page.ChildElements.Sort((a, b) => a.Index.CompareTo(b.Index));
            }

            m_OrderDataAll = JsonSerializer.Deserialize<orders_new>(strOrderData);
            blnPrintTemplateCreated = (m_PT_Page != null) ? true : false;//((m_JsonDocument!=null) && (m_PT_Page!=null))?true:false;
            //---json2object

            if (!(blnPrinterFound & blnPrintTemplateCreated))
            {
                m_blnResult = false;
                if ((!blnPrinterFound) & (blnPrintTemplateCreated))
                {
                    m_strResult = $"建構子運行失敗;找不到對應驅動";
                }
                else if((blnPrinterFound) & (!blnPrintTemplateCreated))
                {
                    m_strResult = $"建構子運行失敗;模板資料錯誤";
                }
                else
                {
                    m_strResult = $"建構子運行失敗;找不到對應驅動且模板資料錯誤";
                }              
            }
            else
            {
                //---
                //補上時間變數字串資料 因為JSON只能存取變數
                DateTime DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(m_OrderDataAll.order_time);
                if(DateTimeBuf!=null)
                {
                    m_OrderDataAll.order_time_year = DateTimeBuf.ToString("yyyy");
                    m_OrderDataAll.order_time_month = DateTimeBuf.ToString("MM");
                    m_OrderDataAll.order_time_day = DateTimeBuf.ToString("dd");
                    m_OrderDataAll.order_time_hours = DateTimeBuf.ToString("HH");
                    m_OrderDataAll.order_time_minutes = DateTimeBuf.ToString("mm");
                }
                //---補上時間變數字串資料 因為JSON只能存取變數

                m_fltSysDpi = 203 * m_PT_Page.ZoomRatio;//設定印表機DPI
                //---
                //字型變數定義
                m_NormalFont = new Font(m_PT_Page.FontName, 9);//一般字 Height =3mm
                m_BigFont = new Font(m_PT_Page.FontName, 13);//單倍字 Height =5mm
                m_InvoiceFont = new Font(m_PT_Page.FontName, 16);//發票 Height =6mm
                m_DoubleFont = new Font(m_PT_Page.FontName, 28);//雙倍字 Height =11mm
                m_FourFont = new Font(m_PT_Page.FontName, 40);//四倍字 Height =13mm
                for (int i = 0; i<m_fltFontHeight.Length ; i++)//計算出每種字型的高度Pixels
                {
                    m_fltFontHeight[i] = DPI_Funs.MillimetersToPixels(m_fltFontHeight[i], m_fltSysDpi);
                }
                //---字型變數定義
                if((m_PT_Page.PrintMode!=null) && (m_PT_Page.PrintMode== "SingleProduct"))
                {
                    if ((m_OrderDataAll != null) && (m_OrderDataAll.order_items.Count > 0))
                    {
                        m_intPages = m_OrderDataAll.order_items.Count;
                    }
                    else
                    {
                        m_intPages = 0;
                    }
                }
                else
                {
                    m_OrderData = m_OrderDataAll;
                    ForLoopVarsInit();// m_ForLoopVars變數初始化
                    m_intPages = 1;//一般模式
                }

                if(m_intPages==1)
                {
                    NormalPrint(strPrinterDriverName);//一般列印模式
                }
                else
                {//一菜一切
                    if(m_intPages>0)
                    {
                        SingleProductPrint(strPrinterDriverName);//一菜一切
                    }            
                }

            }//if (!(blnPrinterFound & blnPrintTemplateCreated))-else

        }
        catch(Exception ex)
        {
            m_blnResult = false;
            m_strResult = $"建構子運行失敗;{ex.Message}";
        }

    }

    //---
    //共用函數
    private bool Element_Preprocess(PT_ChildElement PT_ChildElementBuf)//元件預先處理
    {
        bool blnResult= true;
        string strDataPathBuf = "";
        //int intDataPathBuf = -1;
        int intIndex = -1;
        int intNum = -1;
        switch (PT_ChildElementBuf.ElementType)
        {
            case "Table":
                blnResult = TableShow(PT_ChildElementBuf);
                break;
            case "Rows":
                m_strDisplayMode = (PT_ChildElementBuf.DisplayMode.Length==0)? "Single": PT_ChildElementBuf.DisplayMode;//指定顯示模式
                
                m_strDataPath = GetStackPath(ref m_intDataPath);//strDataPathBuf = GetStackPath(ref intDataPathBuf);
                if (PT_ChildElementBuf.RootName.Length > 0)
                {
                    strDataPathBuf = m_strDataPath;
                    if (strDataPathBuf.Length > 0)
                    {
                        strDataPathBuf += "." + PT_ChildElementBuf.RootName;
                    }
                    else
                    {
                        strDataPathBuf = PT_ChildElementBuf.RootName;
                    }

                    if (ForLoopVarsSet(strDataPathBuf, ref intIndex, ref intNum) <= 0)
                    {
                        blnResult = false;
                    }
                }
                break;
            case "Block":
                m_strDataPath = GetStackPath(ref m_intDataPath);//strDataPathBuf = GetStackPath(ref intDataPathBuf);
                if (PT_ChildElementBuf.RootName.Length > 0)
                {
                    strDataPathBuf = m_strDataPath;
                    if (strDataPathBuf.Length > 0)
                    {
                        strDataPathBuf += "." + PT_ChildElementBuf.RootName;
                    }
                    else
                    {
                        strDataPathBuf = PT_ChildElementBuf.RootName;
                    }

                    if (ForLoopVarsSet(strDataPathBuf, ref intIndex, ref intNum) <= 0)
                    {
                        blnResult = false;
                    }
                }
                break;
        }
        return blnResult;
    }
    private bool TableShow(PT_ChildElement PT_ChildElementBuf)//判斷資料表是否要顯示
    {//透過程式手段 把Table下面低一層的元素RootName全部合併放到Table中
        bool blnResult = false;
        int intCount = 0;
        if(PT_ChildElementBuf.AlwaysPrint=="Y")
        {
            blnResult = true;
            return blnResult;
        }

        for (int i = 0; i < PT_ChildElementBuf.ChildElements.Count; i++)
        {
            if (PT_ChildElementBuf.ChildElements[i].RootName.Length > 0)//判斷是否有要判斷參數
            {
                bool blnfind=false;
                for(int j = 0; j<m_ForLoopVars.Count;j++)//陣列變數判斷條件 : ArrayName.Count>0
                {
                    if(m_ForLoopVars[j].m_strName == PT_ChildElementBuf.ChildElements[i].RootName)
                    {
                        intCount += m_ForLoopVars[j].m_intCount;
                        blnfind = true;
                        break;
                    }
                }

                if ((!blnfind) & (intCount == 0)) //非陣列變數判斷條件: 變數.length > 0
                {
                    m_strDataPath = GetStackPath(ref m_intDataPath);
                    string strDataBuf = GetOrderData(m_strDataPath, PT_ChildElementBuf.ChildElements[i].RootName);
                    if( (strDataBuf != "0") & (strDataBuf != "0.0")) //判斷取出值不等0
                    {
                        intCount = (GetOrderData(m_strDataPath, PT_ChildElementBuf.ChildElements[i].RootName)).Length;
                    }
                    else
                    {
                        intCount = 0;
                    }
                }
                
                if (intCount>0)
                {
                    blnResult = true;
                }
            }
        }

        return blnResult;
    }
    private object? GetFieldValueByName(object obj, string fieldName)//從JSON記憶體給定元素名稱字串取回對應員素質
    {//C# .net8 ~ 傳入字串 轉換成 物件成員名稱並取回對應數值 (你可以使用 Reflection 或 Expression Trees 來根據字串取出對應的物件成員（欄位或屬性）的值)

        if (obj == null || string.IsNullOrWhiteSpace(fieldName))
            return null;


        //JSON屬性變數
        var field00 = obj.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);

        // BindingFlags to access public instance fields[一般成員變數]
        var field01 = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);

        if ((field00 == null) && (field01 == null))
            return null;

        return (field00==null)? field01.GetValue(obj): field00.GetValue(obj);
    }
    private string GetOrderData(string strDataPath,string strVarName,bool blnFindUpperLayer=false)
    {
        string strResult = "";
        bool blnfindRoot = false;//已經找過根節點旗標
        switch(strDataPath)
        {
            case "":
            case ".":
                blnfindRoot = true;//已經找過根節點旗標
                try
                {
                    strResult = GetFieldValueByName(m_OrderData, strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }            
                break;
            case "order_items":
                try
                {
                    if((m_ForLoopVars[0].m_intIndex>=0) && (m_ForLoopVars[0].m_intIndex< m_ForLoopVars[0].m_intCount))
                    {
                        strResult = GetFieldValueByName(m_OrderData.order_items[m_ForLoopVars[0].m_intIndex], strVarName).ToString();
                    }
                    else
                    {
                        strResult = "";
                    }
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "order_items.condiments":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].condiments[m_ForLoopVars[1].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "order_items.set_meals":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].set_meals[m_ForLoopVars[2].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "order_items.set_meals.product":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].set_meals[m_ForLoopVars[2].m_intIndex].product[m_ForLoopVars[3].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "order_items.set_meals.product.condiments":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].set_meals[m_ForLoopVars[2].m_intIndex].product[m_ForLoopVars[3].m_intIndex].condiments[m_ForLoopVars[4].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "packages":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.packages[m_ForLoopVars[5].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "coupons":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.coupons[m_ForLoopVars[6].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "tablewares":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.tablewares[m_ForLoopVars[7].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "payments":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.payments[m_ForLoopVars[8].m_intIndex], strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            case "invoice_data":
                try
                {
                    strResult = GetFieldValueByName(m_OrderData.invoice_data, strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
            default:
                try
                {
                    strResult = GetFieldValueByName(m_OrderData, strVarName).ToString();
                }
                catch
                {
                    strResult = "";
                }
                break;
        }

        if( (blnFindUpperLayer) && (strResult == "") && (!blnfindRoot))
        {
            string strNewDataPath = "";
            if(strDataPath.Contains("."))
            {
                strNewDataPath = strDataPath.Split(".")[0];
            }

            return GetOrderData(strNewDataPath, strVarName);//只找上一層，沒有一直往上找
        }
        else
        {
            return strResult;
        }       
    }

    private string TemplateContent2Data(string strDataPath,string strContent)//模板Content轉實際顯示資料
    {
        string strResult = "";

        string pattern = @"\{([^}]+)}(\}*)|[^{}]+";
        var matches = Regex.Matches(strContent, pattern);
        var result = new List<string>();

        foreach (Match match in matches)
        {
            if (match.Groups[1].Success) // 匹配了 {xxx}
            {
                string value = match.Groups[1].Value + match.Groups[2].Value;
                result.Add(value);
            }
            else
            {
                result.Add(match.Value);
            }
        }

        foreach (var part in result)
        {
            string strVarName = part;
            if ((strVarName.Length>1) && (strVarName.Substring(0,1)=="$"))
            {
                strResult += GetOrderData(strDataPath, strVarName.Substring(1, strVarName.Length-1),true);
            }
            else
            {
                strResult += strVarName;
            }
        }
        return strResult;
    }

    private string GetStackPath(ref int intDataPath)//取得堆疊物件的資料集路徑
    {
        string strResult = "";
        ContainerElement[] ContainerElements = m_ContainerElements.ToArray();
        if((ContainerElements!=null) && (ContainerElements.Length>0))
        {
            for (int i = (ContainerElements.Length - 1); i >= 0; i--)
            {
                if((ContainerElements[i].m_Element.RootName!=null) && ContainerElements[i].m_Element.RootName.Length>0)
                {
                    string strBuf="";
                    bool blnfind = false;
                    for(int j = 0; j<m_ForLoopVars.Count;j++)
                    {
                        if(strResult.Length > 0)
                        {
                            strBuf = strResult + "." + ContainerElements[i].m_Element.RootName;
                        }
                        else
                        {
                            strBuf = ContainerElements[i].m_Element.RootName;
                        }
                        if (strBuf == m_ForLoopVars[j].m_strName)
                        {
                            blnfind = true;
                            break;
                        }
                    }

                    if(blnfind)
                    {
                        if (strResult.Length > 0)
                        {
                            strResult += "." + ContainerElements[i].m_Element.RootName;
                        }
                        else
                        {
                            strResult = ContainerElements[i].m_Element.RootName;
                        }
                    }
                }            
            }
        }

        switch (strResult)
        {
            case "order_items"://0
                intDataPath = 0;
                break;
            case "order_items.condiments"://1
                intDataPath = 1;
                break;
            case "order_items.set_meals"://2
                intDataPath = 2;
                break;
            case "order_items.set_meals.product"://3
                intDataPath = 3;
                break;
            case "order_items.set_meals.product.condiments"://4
                intDataPath = 4;
                break;
            case "packages"://5
                intDataPath = 5;
                break;
            case "coupons"://6
                intDataPath = 6;
                break;
            case "tablewares"://7
                intDataPath = 7;
                break;
            case "payments"://8
                intDataPath = 8;
                break;
            case "invoice_data"://9
                intDataPath = 9;
                break;
            default://以上都不符合走這個
                intDataPath = -1;
                break;
        }

        return strResult;
    }
    private int ForLoopVarsSet(string strPath,ref int intIndex,ref int intNum)// m_ForLoopVars變數設定
    {
        int intResult = 0;
        if(m_ForLoopVars.Count==0)
        {
            return intResult;
        }

        switch (strPath)
        {
            case "order_items"://0
                intIndex = m_ForLoopVars[0].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                m_ForLoopVars[0].m_intIndex = ((m_ForLoopVars[0].m_intIndex + 1) >= m_ForLoopVars[0].m_intCount) ? (m_ForLoopVars[0].m_intCount - 1) : (m_ForLoopVars[0].m_intIndex + 1);
                intResult = m_ForLoopVars[0].m_intCount;
                intNum = 0;
                break;
            case "order_items.condiments"://1
                if(m_ForLoopVars[1].m_intIndex==-1)
                {
                    if(m_ForLoopVars[0].m_intCount>0)
                    {
                        m_ForLoopVars[1].m_intCount = m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].condiments.Count;
                        m_ForLoopVars[1].m_intIndex = (m_ForLoopVars[1].m_intCount > 0) ? 0 : -1;
                    }
                }
                else
                {
                    intIndex = m_ForLoopVars[1].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                    m_ForLoopVars[1].m_intIndex = ((m_ForLoopVars[1].m_intIndex + 1) >= m_ForLoopVars[1].m_intCount) ? (m_ForLoopVars[1].m_intCount - 1) : (m_ForLoopVars[1].m_intIndex + 1);
                }
                intResult = m_ForLoopVars[1].m_intCount;
                intNum = 1;
                break;
            case "order_items.set_meals"://2
                if (m_ForLoopVars[2].m_intIndex == -1)
                {
                    if (m_ForLoopVars[0].m_intCount > 0)
                    {
                        m_ForLoopVars[2].m_intCount = m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].set_meals.Count;
                        m_ForLoopVars[2].m_intIndex = (m_ForLoopVars[2].m_intCount > 0) ? 0 : -1;
                    }
                }
                else
                {
                    intIndex = m_ForLoopVars[2].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                    m_ForLoopVars[2].m_intIndex = ((m_ForLoopVars[2].m_intIndex + 1) >= m_ForLoopVars[2].m_intCount) ? (m_ForLoopVars[2].m_intCount - 1) : (m_ForLoopVars[2].m_intIndex + 1);
                }
                intResult = m_ForLoopVars[2].m_intCount;
                intNum = 2;
                break;
            case "order_items.set_meals.product"://3
                if (m_ForLoopVars[3].m_intIndex == -1)
                {
                    if ((m_ForLoopVars[0].m_intCount > 0) && (m_ForLoopVars[2].m_intCount > 0))
                    {
                        m_ForLoopVars[3].m_intCount = m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].set_meals[m_ForLoopVars[2].m_intIndex].product.Count;
                        m_ForLoopVars[3].m_intIndex = (m_ForLoopVars[3].m_intCount > 0) ? 0 : -1;
                    }
                }
                else
                {
                    intIndex = m_ForLoopVars[3].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                    m_ForLoopVars[3].m_intIndex = ((m_ForLoopVars[3].m_intIndex + 1) >= m_ForLoopVars[3].m_intCount) ? (m_ForLoopVars[3].m_intCount - 1) : (m_ForLoopVars[3].m_intIndex + 1);
                }
                intResult = m_ForLoopVars[3].m_intCount;
                intNum = 3;
                break;
            case "order_items.set_meals.product.condiments"://4
                if (m_ForLoopVars[4].m_intIndex == -1)
                {
                    if ((m_ForLoopVars[0].m_intCount > 0) && (m_ForLoopVars[2].m_intCount > 0) && (m_ForLoopVars[3].m_intCount > 0))
                    {
                        m_ForLoopVars[4].m_intCount = m_OrderData.order_items[m_ForLoopVars[0].m_intIndex].set_meals[m_ForLoopVars[2].m_intIndex].product[m_ForLoopVars[3].m_intIndex].condiments.Count;
                        m_ForLoopVars[4].m_intIndex = (m_ForLoopVars[4].m_intCount > 0) ? 0 : -1;
                    }
                }
                else
                {
                    intIndex = m_ForLoopVars[4].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                    m_ForLoopVars[4].m_intIndex = ((m_ForLoopVars[4].m_intIndex + 1) >= m_ForLoopVars[4].m_intCount) ? (m_ForLoopVars[4].m_intCount - 1) : (m_ForLoopVars[4].m_intIndex + 1);
                }
                intResult = m_ForLoopVars[4].m_intCount;
                intNum = 4;
                break;
            case "packages"://5
                intIndex = m_ForLoopVars[5].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                m_ForLoopVars[5].m_intIndex = ((m_ForLoopVars[5].m_intIndex + 1) >= m_ForLoopVars[5].m_intCount) ? (m_ForLoopVars[5].m_intCount - 1) : (m_ForLoopVars[5].m_intIndex + 1);
                intResult = m_ForLoopVars[5].m_intCount;
                intNum = 5;
                break;
            case "coupons"://6
                intIndex = m_ForLoopVars[6].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                m_ForLoopVars[6].m_intIndex = ((m_ForLoopVars[6].m_intIndex + 1) >= m_ForLoopVars[6].m_intCount) ? (m_ForLoopVars[6].m_intCount - 1) : (m_ForLoopVars[6].m_intIndex + 1);
                intResult = m_ForLoopVars[6].m_intCount;
                intNum = 6;
                break;
            case "tablewares"://7
                intIndex = m_ForLoopVars[7].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                m_ForLoopVars[7].m_intIndex = ((m_ForLoopVars[7].m_intIndex + 1) >= m_ForLoopVars[7].m_intCount) ? (m_ForLoopVars[7].m_intCount - 1) : (m_ForLoopVars[7].m_intIndex + 1);
                intResult = m_ForLoopVars[7].m_intCount;
                intNum = 7;
                break;
            case "payments"://8
                intIndex = m_ForLoopVars[8].m_intIndex + 1;//判斷用不用防呆害怕超過陣列範圍
                m_ForLoopVars[8].m_intIndex = ((m_ForLoopVars[8].m_intIndex + 1) >= m_ForLoopVars[8].m_intCount) ? (m_ForLoopVars[8].m_intCount - 1) : (m_ForLoopVars[8].m_intIndex + 1);
                intResult = m_ForLoopVars[8].m_intCount;
                intNum = 8;
                break;
            case "invoice_data"://9
                intIndex = m_ForLoopVars[9].m_intIndex;
                intResult = m_ForLoopVars[9].m_intCount;
                intNum = 9;
                break;
            default://以上都不符合走這個
                intResult = 0;
                string[]strsBuf=strPath.Split('.');
                if(strsBuf.Length > 0 )
                {
                    intResult = (GetOrderData(m_strDataPath, strsBuf[strsBuf.Length - 1])).Length;//不符合資料集就查是否為目前結點下的變數，並將變數長度回傳承回判斷依據
                }             
                intIndex = -1;
                intNum = -1;
                break;
        }

        return intResult;
    }
    private void ForLoopVarsInit()// m_ForLoopVars變數初始化
    {
        m_ForLoopVars.Clear();

        m_ForLoopVars.Add(new ForLoopVar("order_items", 0));
        m_ForLoopVars.Add(new ForLoopVar("order_items.condiments", 0));
        m_ForLoopVars.Add(new ForLoopVar("order_items.set_meals", 0));
        m_ForLoopVars.Add(new ForLoopVar("order_items.set_meals.product", 0));
        m_ForLoopVars.Add(new ForLoopVar("order_items.set_meals.product.condiments", 0));
        m_ForLoopVars.Add(new ForLoopVar("packages", 0));
        m_ForLoopVars.Add(new ForLoopVar("coupons", 0));
        m_ForLoopVars.Add(new ForLoopVar("tablewares", 0));
        m_ForLoopVars.Add(new ForLoopVar("payments", 0));
        m_ForLoopVars.Add(new ForLoopVar("invoice_data", 0));

        if (m_OrderData!=null)
        {
            m_ForLoopVars[0].m_intCount = (m_OrderData.order_items!=null)? m_OrderData.order_items.Count : 0;
            m_ForLoopVars[0].m_intIndex = -1;//(m_ForLoopVars[0].m_intCount > 0) ? 0 : -1;
            //m_ForLoopVars.Add(new ForLoopVar("order_items.condiments", 0));
            //m_ForLoopVars.Add(new ForLoopVar("order_items.set_meals", 0));
            //m_ForLoopVars.Add(new ForLoopVar("order_items.set_meals.product", 0));
            //m_ForLoopVars.Add(new ForLoopVar("order_items.set_meals.product.condiments", 0));
            m_ForLoopVars[5].m_intCount = (m_OrderData.packages != null) ? m_OrderData.packages.Count : 0;
            m_ForLoopVars[5].m_intIndex = -1;//(m_ForLoopVars[5].m_intCount > 0) ? 0 : -1;
            m_ForLoopVars[6].m_intCount = (m_OrderData.coupons != null) ? m_OrderData.coupons.Count : 0;
            m_ForLoopVars[6].m_intIndex = -1;//(m_ForLoopVars[6].m_intCount > 0) ? 0 : -1;
            m_ForLoopVars[7].m_intCount = (m_OrderData.tablewares != null) ? m_OrderData.tablewares.Count : 0;
            m_ForLoopVars[7].m_intIndex = -1;//(m_ForLoopVars[7].m_intCount > 0) ? 0 : -1;
            m_ForLoopVars[8].m_intCount = (m_OrderData.payments != null) ? m_OrderData.payments.Count : 0;
            m_ForLoopVars[8].m_intIndex = -1;//(m_ForLoopVars[8].m_intCount > 0) ? 0 : -1;
            m_ForLoopVars[9].m_intCount = 1;
            m_ForLoopVars[9].m_intIndex = 0;
        }
    }

    private bool m_blnGetDataElement = false;
    private PT_ChildElement GetDataElement(PT_ChildElement root)//取得資料物件
    {
        PT_ChildElement ElementResult = null;
        if((root.ChildElements!=null) && (root.ChildElements.Count>0))
        {
            // 就地修改 ChildElements 的順序，依照 Index 遞增排序 //如果你要遞減排序，改成 b.Index.CompareTo(a.Index)
            root.ChildElements.Sort((a, b) => a.Index.CompareTo(b.Index));
            if(!Element_Preprocess(root))
            {
                return ElementResult;
            }

            ContainerElement ContainerElementBuf = new ContainerElement(root, 1);
            m_ContainerElements.Push(ContainerElementBuf);//放入堆疊

            m_strDataPath = GetStackPath(ref m_intDataPath);
            m_blnGetDataElement = false;
            return GetDataElement(root.ChildElements[0]);//遞迴呼叫
        }
        else
        {
            m_blnGetDataElement = true;
            ElementResult = root;
        }
        return ElementResult;
    }

    private string m_strElement2DataLog = "";
    private string Element2Data(PT_ChildElement PT_ChildElementBuf)//元件轉資料
    {
        string strResult = "";
        if (PT_ChildElementBuf==null)
        {
            return strResult;
        }

        if (PT_ChildElementBuf.Index==0)
        {// Block/Rows下第一個資料元件要進行的處理程序

            //上層元件標註
            ContainerElement[] ContainerElements = m_ContainerElements.ToArray();
            for (int i = (ContainerElements.Length - 1); i >= 0; i--)
            {
                m_strElement2DataLog += ContainerElements[i].m_Element.ElementType + "/";
            }
            m_strElement2DataLog += "~";



            //資料集標註
            m_strElement2DataLog += ((m_strDataPath.Length == 0) ? "." : m_strDataPath) + ";\t";
        }

        string strContentDatrBuf = "";
        if(m_strDisplayMode== "Single")
        {
            strContentDatrBuf = TemplateContent2Data(m_strDataPath, PT_ChildElementBuf.Content);
        }
        else
        {
            int intIndex = -1;
            int intNum = -1;
            do
            {
                if (strContentDatrBuf.Length == 0)
                {
                    strContentDatrBuf = TemplateContent2Data(m_strDataPath, PT_ChildElementBuf.Content);
                }
                else
                {
                    strContentDatrBuf += PT_ChildElementBuf.IntervalSymbols + TemplateContent2Data(m_strDataPath, PT_ChildElementBuf.Content);
                }

                ForLoopVarsSet(m_strDataPath, ref intIndex, ref intNum);
            } while (intIndex<m_ForLoopVars[m_intDataPath].m_intCount);
        }
        m_strElement2DataLog += strContentDatrBuf + ";";//PT_ChildElementBuf.Content + ";";

        strResult = strContentDatrBuf;
        return strResult;
    }

    private void Data2Image(PT_ChildElement PT_ChildElementBuf, Graphics g)//資料轉圖
    {

        if (m_strRealData.Length == 0)
        {
            return;
        }

        float fltWidth = PT_ChildElementBuf.Width;
        float fltHeight = (PT_ChildElementBuf.Height>0) ? PT_ChildElementBuf.Height : 0;
        float fltStartX = -1;
        float fltStartY = -1;
        float fltXBuf = 0;
        float fltYBuf = 0;

        //---
        //計算元件定位點
        switch(PT_ChildElementBuf.X_Alignment)
        {
            case "X":
                fltStartX = PT_ChildElementBuf.X;
                break;
            case "Right":
                fltStartX = -1;
                fltXBuf = m_PT_Page.Width - DPI_Funs.MillimetersToPixels(6, m_fltSysDpi);//6(機器滾輪大小)            
                break;
            case "Center":
                fltStartX = -2;
                if(m_PT_Page.Height>0)
                {
                    fltXBuf = (m_PT_Page.Width - DPI_Funs.MillimetersToPixels(1 * 2, m_fltSysDpi)) / 2;//標籤機
                }
                else
                {
                    if (m_PT_Page.X == 0)
                    {
                        fltXBuf = (m_PT_Page.Width - DPI_Funs.MillimetersToPixels(4 * 2, m_fltSysDpi)) / 2;//8mm是兩側留白(機器滾輪大小)
                    }
                    else
                    {
                        fltXBuf = m_PT_Page.Width - m_PT_Page.X;//(12mm+57mm)-
                        fltXBuf = fltXBuf - DPI_Funs.MillimetersToPixels(7, m_fltSysDpi);
                        fltXBuf = fltXBuf / 2;
                    }
                }  
                break;
        }

        switch (PT_ChildElementBuf.Y_Alignment)
        {
            case "Y":
                fltStartY = PT_ChildElementBuf.Y;
                break;
            case "Increment":
                fltStartY = m_fltLast_Y + m_fltLast_Height + 1;//[垂直排列 ~ 目前Y = 前一元件的Y + 前一元件的Height + 1]
                break;
            case "Element":
                fltStartY = m_fltLast_Y;//[橫向排列 ~ 目前Y = 前一個元件Y]
                break;
        }

        //移動到第一次計算出元件定位點，因為文字定位點≠元件定位點
        if (fltStartX < 0)
        {
            switch (fltStartX)
            {
                case -1://"Right"
                    fltStartX = fltXBuf - fltWidth;
                    break;
                case -2://"Center"
                    fltStartX = fltXBuf - (fltWidth / 2);
                    break;
            }
        }
        fltStartX += m_PT_Page.X;//計算出X座標進行最後紙張平移運算(57mm)
        m_fltLast_X = fltStartX;
        m_fltLast_Y = fltStartY;
        //---計算元件定位點

        Bitmap BitmapBuf = null;
        switch (PT_ChildElementBuf.ElementType)
        {
            case "Image":
                if(m_strRealData.Contains(","))
                {//data:image/png;base64,..... 把實際資料抓出來
                    m_strRealData = m_strRealData.Split(",")[1];
                }
                BitmapBuf = BitmapBase64_Funs.Base64String2Image(m_strRealData);
                break;
            case "QrCode":
                BitmapBuf = Barcode_Funs.QrCode(m_strRealData, PT_ChildElementBuf.ErrorCorrection);
                break;
            case "BarCode":
                BitmapBuf = Barcode_Funs.BarCode(m_strRealData, PT_ChildElementBuf.Height, PT_ChildElementBuf.Width);
                break;
        }

        if(BitmapBuf!=null)
        {//圖片模式

            g.DrawImage( BitmapBuf, new Rectangle((int)(fltStartX), (int)(fltStartY),(int)(fltWidth),(int)(fltHeight)) );

        }//if(BitmapBuf!=null)
        else
        {//文字模式
            /*
                m_NormalFont = new Font(m_PT_Page.FontName, 9);//一般字 Height =3mm
                m_BigFont = new Font(m_PT_Page.FontName, 13);//單倍字 Height =5mm
                m_InvoiceFont = new Font(m_PT_Page.FontName, 16); ;//發票 Height =6mm
                m_DoubleFont = new Font(m_PT_Page.FontName, 28);//雙倍字 Height =11mm
                m_FourFont = new Font(m_PT_Page.FontName, 40);//四倍字 Height =13mm              
            */
            bool blnAutoWrap =(PT_ChildElementBuf.AutoWrap=="Y") ? true : false;//自動換行旗標
            float fltVerticalSpacing = (float)PT_ChildElementBuf.VerticalSpacing;//自動換行間隙
            string strIntervalSymbols = PT_ChildElementBuf.IntervalSymbols;//合併資料間隔符號
            string strHorizontalContentAlig = PT_ChildElementBuf.HorizontalContentAlig;//文字Y軸對其方式

            Font NowFont = null;
            float fltNowFontHeight = 0;
            Brush brush = Brushes.Black;

            NowFont = m_NormalFont;
            fltNowFontHeight = m_fltFontHeight[0];
            switch (PT_ChildElementBuf.ContentSize)
            {
                case 40:
                    NowFont = m_FourFont;
                    fltNowFontHeight = m_fltFontHeight[4];
                    break;
                case 28:
                    NowFont = m_DoubleFont;
                    fltNowFontHeight = m_fltFontHeight[3];
                    break;
                case 16:
                    NowFont = m_InvoiceFont;
                    fltNowFontHeight = m_fltFontHeight[2];
                    break;
                case 13:
                    NowFont = m_BigFont;
                    fltNowFontHeight = m_fltFontHeight[1];
                    break;
                case 9:
                    NowFont = m_NormalFont;
                    fltNowFontHeight = m_fltFontHeight[0];
                    break;
            }
            
            if(PT_ChildElementBuf.ContentBold=="Y")
            {
                NowFont = new Font(NowFont, FontStyle.Bold);
            }

            string strShowData = "";
            string[] strShowArrayData = null;
            int intWlen = Wlen(m_strRealData);//資料總字數(將中文字換成2個英文字長度)
            //float fltDataAllLength = intWlen * fltNowFontHeight;//資料總長度
            int intOneRowWords = (int)(fltWidth / fltNowFontHeight)*2;//單列最大中文字數*2 = (英文字數量)
            
            int intMaxRows = 1;//最大列數
            if((intOneRowWords < intWlen) && blnAutoWrap)
            {
                strShowArrayData = SplitStringByByteLength(m_strRealData, intOneRowWords);//顯示多行資料集
                intMaxRows = (strShowArrayData!=null) ? strShowArrayData.Length : intMaxRows;//紀錄列數

                if (fltHeight > 0)//元件有限高
                {
                    int i = 0;
                    float fltBuf = fltHeight;
                    int intLimitRows = 0;//限制下最多列數
                    bool blnbreak = false;
                    do
                    {
                        if(fltBuf >= (fltNowFontHeight + i*fltVerticalSpacing))
                        {
                            intLimitRows++;
                            fltBuf = fltBuf - (fltNowFontHeight + i * fltVerticalSpacing);
                            i = 1;
                        }
                        else
                        {
                            blnbreak = true;
                        }
                    } while (!blnbreak);
                    intMaxRows = (intLimitRows < intMaxRows) ? intLimitRows : intMaxRows;//最後列數
                }
                else
                {//元件沒限高
                    fltHeight = (intMaxRows * fltNowFontHeight) + ((intMaxRows - 1) * fltVerticalSpacing);
                }
            }
            else
            {
                //strShowData = GetSubstringByByteLength(m_strRealData, (intMaxRows * intOneRowWords));//單行資料
                strShowData = m_strRealData;
                if(fltHeight<=0)
                {//元件沒限高
                    fltHeight = (intMaxRows * fltNowFontHeight) + ((intMaxRows - 1) * fltVerticalSpacing);
                }
            }



            switch (PT_ChildElementBuf.VerticalContentAlig)//文字Y軸對其方式
            {
                case "Top":
                    fltStartY = fltStartY;
                    break;
                case "Down":
                    fltStartY = fltStartY + (fltHeight - (fltNowFontHeight + (intMaxRows - 1) * (fltNowFontHeight + fltVerticalSpacing)));
                    break;
                case "Center":
                    fltStartY = fltStartY +( (fltHeight/2) - ( (fltNowFontHeight + (intMaxRows - 1) * (fltNowFontHeight + fltVerticalSpacing)) / 2) );
                    break;
            }

            switch (PT_ChildElementBuf.HorizontalContentAlig)//文字Y軸對其方式
            {
                case "Left":
                    fltStartX = fltStartX;
                    break;
                case "Right":
                    if(intMaxRows==1)
                    {
                        fltStartX = fltStartX + ( fltWidth - (0.65f * fltNowFontHeight * Wlen(strShowData)) );
                    }
                    else
                    {
                        fltStartX = fltStartX;//多行一律靠左
                    }
                    break;
                case "Center":
                    if (intMaxRows == 1)
                    {
                        fltStartX = fltStartX + (fltWidth/2 - (0.65f * fltNowFontHeight * Wlen(strShowData))/2 );//0.65是實驗出來的數值: 字寬 = 0.65*字高
                    }
                    else
                    {
                        fltStartX = fltStartX;//多行一律靠左
                    }
                    break;
            }

            if(strShowArrayData == null)
            {
                g.DrawString(strShowData, NowFont, brush, fltStartX, fltStartY);
            }
            else
            {
                for (int i = 0; i < intMaxRows; i++)
                {
                    g.DrawString(strShowArrayData[i], NowFont, brush, fltStartX, fltStartY);
                    fltStartY += (fltNowFontHeight + fltVerticalSpacing);            
                }
            }
        }

        if (m_fltMax_Height < fltHeight)
        {//同列最大列印高度
            m_fltMax_Height = fltHeight + ((PT_ChildElementBuf.VerticalSpacing!=null)?(float)PT_ChildElementBuf.VerticalSpacing : 0.0f);//fltHeight
        }
        /*
        //移動到第一次計算出元件定位點，因為文字定位點≠元件定位點
        m_fltLast_X = fltStartX;
        m_fltLast_Y = fltStartY;
        */
    }

    private int Wlen(string val)//計算資料字元數
    {
        int length = 0;
        foreach (char c in val)
        {
            length += c <= 0xFF ? 1 : 2; // ASCII ≤ 0xFF 為 1 byte，其餘為 2 byte
        }
        return length;
    }

    private string[] SplitStringByByteLength(string val, int maxByteLength)//將一字串按照最大字元數分成數段存成陣列
    {
        List<string> segments = new List<string>();  // 用來儲存切割後的字串片段
        int length = 0;  // 當前位元組長度
        StringBuilder currentSegment = new StringBuilder();  // 當前的字串片段

        foreach (char c in val)
        {
            int byteCount = c <= 0xFF ? 1 : 2;  // 根據字元的位元組數計算

            // 如果加上這個字元後超過最大位元組長度，則將當前片段儲存並重置
            if (length + byteCount > maxByteLength)
            {
                segments.Add(currentSegment.ToString());
                currentSegment.Clear();  // 清空當前片段
                length = 0;  // 重置位元組長度
            }

            currentSegment.Append(c);  // 將字元加入當前片段
            length += byteCount;  // 增加位元組長度
        }

        // 最後一段字串需要加到結果中
        if (currentSegment.Length > 0)
        {
            segments.Add(currentSegment.ToString());
        }

        return segments.ToArray();  // 返回字串陣列
    }

    private string GetSubstringByByteLength(string val, int byteLength)//取出一段指定字元數的資料
    {
        int length = 0;  // 計算目前的位元組長度
        StringBuilder result = new StringBuilder(); // 用來存放結果字串

        foreach (char c in val)
        {
            int byteCount = c <= 0xFF ? 1 : 2;  // 根據字元的位元組數計算
            if (length + byteCount > byteLength)
            {
                break; // 超過指定的位元組長度時退出
            }

            result.Append(c);  // 加入字元到結果字串
            length += byteCount;  // 累加目前的位元組長度
        }

        return result.ToString();  // 返回結果字串
    }
    //---共用函數
    
    //---
    //一般列印
    private void NormalPrint(string strPrinterDriverName)//一般列印模式
    {
        m_PrintDocument = null;
        m_PrintDocument = new PrintDocument();//印表畫布
        m_PrintDocument.PrinterSettings.PrinterName = strPrinterDriverName;
        m_PrintDocument.PrintPage += new PrintPageEventHandler(PrintPage);
        m_PrintDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

        int width = (int)DPI_Funs.PixelsToMillimeters(m_PT_Page.Width, m_fltSysDpi);  // 約 315
        int height = 50000;//500cm


        PaperSize paperSize = new PaperSize("NormalPrint", (int)(width / 25.4 * 100), (int)(height / 25.4 * 100));//以百分之一英吋為單位
        m_PrintDocument.DefaultPageSettings.PaperSize = paperSize;
        m_PrintDocument.Print();//驅動PrintPage
    }

    private void DrawingPage(Graphics g)//畫布實際建立函數
    {
        //---
        //測試多頁列印+裁紙
        /*
        Brush brush = Brushes.Black;
        g.DrawString($"050 ~ {m_intPageNumbers}", m_DoubleFont, brush, 0, 50);
        //*/
        //---測試多頁列印+裁紙
        m_strDataPath = "";
        for (int i = 0;i< m_PT_Page.ChildElements.Count;i++)//依序處理Page的內容
        {
            //---
            //Debug code
            if(i==22)
            {
                bool blncheckpoint = true;
            }
            //---Debug code

            PT_ChildElement PT_ChildElementBuf = GetDataElement(m_PT_Page.ChildElements[i]);
            if (PT_ChildElementBuf != null)
            {
                m_strRealData = Element2Data(PT_ChildElementBuf);//元件轉資料
                Data2Image(PT_ChildElementBuf, g);
            }

            //清空堆疊迴圈
            while (m_ContainerElements.Count > 0)
            {
                ContainerElement ContainerElementBuf= (ContainerElement)m_ContainerElements.Peek();//讀取堆疊資料但不刪除
                if (ContainerElementBuf.m_index < ContainerElementBuf.m_Element.ChildElements.Count) 
                {
                    PT_ChildElementBuf = GetDataElement(ContainerElementBuf.m_Element.ChildElements[ContainerElementBuf.m_index]);
                    ContainerElementBuf.m_index++;//改變已處理的子元件編號(旗標)
                    if(PT_ChildElementBuf!=null)
                    {
                        m_strRealData = Element2Data(PT_ChildElementBuf);//元件轉資料
                        Data2Image(PT_ChildElementBuf, g);
                    }
                }
                else
                {
                    if ((ContainerElementBuf.m_Element.RootName != null) && (ContainerElementBuf.m_Element.RootName.Length > 0))
                    {
                        m_strDataPath = GetStackPath(ref m_intDataPath);
                    }

                    if(m_blnGetDataElement)
                    {
                        m_strElement2DataLog += "\n";
                        m_blnGetDataElement = false;
                        m_fltLast_Height = m_fltMax_Height;//(不同列時，運算用)
                        m_fltMax_Height = 0;//初始化變數
                    }

                    if ((ContainerElementBuf.m_Element.ElementType=="Rows"))// || (ContainerElementBuf.m_Element.ElementType == "Block")
                    {
                        int intIndex = -1; 
                        int intNum = -1;
                        int intCount = -1;
                        //string strDataPathBuf = "";
                        //int intDataPathBuf = -1;
                        //strDataPathBuf = GetStackPath(ref intDataPathBuf);
                        m_strDataPath = GetStackPath(ref m_intDataPath);
                        intCount = ForLoopVarsSet(m_strDataPath, ref intIndex, ref intNum);//intCount = ForLoopVarsSet(strDataPathBuf, ref intIndex, ref intNum);
                        if ( (intCount!=0) && (intIndex < intCount) )
                        {
                            ContainerElementBuf.m_index = 0;//改變已處理的子元件編號(旗標)~ 從同計算

                            //----
                            //相依變數全部也要觸發重置機制，當下次呼叫到ForLoopVarsSet就會執行
                            if (intNum == 0)//order_items
                            {
                                m_ForLoopVars[1].m_intIndex = -1;
                                m_ForLoopVars[1].m_intCount = -1;
                                m_ForLoopVars[2].m_intIndex = -1;
                                m_ForLoopVars[2].m_intCount = -1;
                                m_ForLoopVars[3].m_intIndex = -1;
                                m_ForLoopVars[3].m_intCount = -1;
                                m_ForLoopVars[4].m_intIndex = -1;
                                m_ForLoopVars[4].m_intCount = -1;
                            }
                            if (intNum == 1)//order_items.condiments
                            {

                            }
                            if (intNum == 2)//order_items.set_meals
                            {//驗證可以
                                /*
                                if (m_RecycleElements.Count > 0)
                                {
                                    m_ContainerElements.Push((ContainerElement)m_RecycleElements.Peek());
                                    m_RecycleElements.Pop();
                                }
                                //*/
                                m_ForLoopVars[3].m_intIndex = -1;
                                m_ForLoopVars[3].m_intCount = -1;
                                m_ForLoopVars[4].m_intIndex = -1;
                                m_ForLoopVars[4].m_intCount = -1;
                            }
                            if (intNum == 3)//order_items.set_meals.product
                            {
                                m_ForLoopVars[4].m_intIndex = -1;
                                m_ForLoopVars[4].m_intCount = -1;
                            }
                            if (intNum == 4)//order_items.set_meals.product.condiments
                            {

                            }
                            //---相依變數全部也要觸發重置機制，當下次呼叫到ForLoopVarsSet就會執行
                        }
                        else
                        {
                            /*
                            bool blnTableLoop = false;
                            ContainerElement[] ContainerElements = m_ContainerElements.ToArray();
                            for(int j= (ContainerElements.Length-1);j>=0;j--)
                            {
                                if (ContainerElements[j].m_Element.ElementType== "Table")
                                {
                                    blnTableLoop = true;
                                    break;
                                }
                            }
                            if (blnTableLoop && (ContainerElementBuf.m_Element.ChildElements.Count>0))
                            {
                                ContainerElementBuf.m_index = 1;
                                m_RecycleElements.Push(ContainerElementBuf);
                            }
                            */

                            m_ContainerElements.Pop();//移除堆疊最上面元件
                        }
                    }
                    else
                    {//Table 直接移除 ~ 堆疊只放Rows
                        m_ContainerElements.Pop();//移除堆疊最上面元件
                    }

                }
            }//while (m_ContainerElements.Count > 0)           
        }

        Console.WriteLine(m_strElement2DataLog);
    }

    private void PrintPage(object sender, PrintPageEventArgs e)//實際產生列印內容觸發函數
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
        try
        {

            DrawingPage(g);
            m_blnResult = true;
            m_strResult = $"已經將列印頁面產生並傳送到對應的印表機柱列中";
            e.HasMorePages = false;//驅動切紙
        }
        catch (Exception ex)
        {
            m_blnResult = false;
            m_strResult = $"PrintPage運行失敗;{ex.Message}";
        }

    }
    //---一般列印

    //---
    //一菜一切
    private void SingleProductPrint(string strPrinterDriverName)
    {
        int intCS_Count = 0;//C# 紀錄第幾項商品變數
        for (int i = 0; i < m_OrderDataAll.order_items.Count; i++)
        {
            for(int j = 0;j< m_OrderDataAll.order_items[i].count;j++)//每一個相同商品都要印一張
            {
                intCS_Count++;

                m_OrderData = null;//把運算記憶體清空
                m_OrderData = m_OrderDataAll.order_itemsDeepClone(i);//每次只拷貝一筆資料進行運算
                m_OrderData.order_items[0].item_no = intCS_Count;//C#重新賦予編號

                //---
                //產生QrCodeBlankingMachine變數內容
                string strBuf = "";
                if (m_OrderData.order_items[0].condiments!=null)
                {
                    
                    for (int k = 0;  k< m_OrderData.order_items[0].condiments.Count; k++)
                    {
                        if(k==0)
                        {
                            strBuf = m_OrderData.order_items[0].condiments[k].condiment_code;
                        }
                        else
                        {
                            strBuf +="," + m_OrderData.order_items[0].condiments[k].condiment_code;
                        }
                    }
                }
                if (m_PT_Page.Content.Contains("陸柒零落料機"))
                {
                    m_OrderData.order_items[0].QrCodeBlankingMachine = m_OrderData.order_items[0].product_code;
                    if(strBuf.Length>0)
                    {
                        m_OrderData.order_items[0].QrCodeBlankingMachine += "-" + strBuf;
                    }
                }

                if (m_PT_Page.Content.Contains("提點落料機"))
                {
                    m_OrderData.order_items[0].QrCodeBlankingMachine = m_OrderData.order_no.Replace("-", "") + "|" + m_OrderData.order_items[0].product_code;
                    if (strBuf.Length > 0)
                    {
                        m_OrderData.order_items[0].QrCodeBlankingMachine += "|" + strBuf;
                    }
                }
                //---產生QrCodeBlankingMachine變數內容

                ForLoopVarsInit();// m_ForLoopVars變數初始化

                m_PrintDocument = null;
                m_PrintDocument = new PrintDocument();//印表畫布
                m_PrintDocument.PrinterSettings.PrinterName = strPrinterDriverName;
                m_PrintDocument.PrintPage += new PrintPageEventHandler(SingleProductPrintPage);
                m_PrintDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                int width = (int)DPI_Funs.PixelsToMillimeters(m_PT_Page.Width, m_fltSysDpi);  // 約 315
                int height = (m_PT_Page.Height > 0) ? (int)DPI_Funs.PixelsToMillimeters(m_PT_Page.Height, m_fltSysDpi) : 50000;//500cm


                PaperSize paperSize = new PaperSize("SingleProductPrint", (int)(width / 25.4 * 100), (int)(height / 25.4 * 100));//以百分之一英吋為單位
                m_PrintDocument.DefaultPageSettings.PaperSize = paperSize;
                m_PrintDocument.Print();//驅動PrintPage
            }

            //除錯用 只執行一次就跳離迴圈
            //break;
        }
    }
    private void SingleProductDrawingPage(Graphics g)//畫布實際建立函數
    {
        /*產生BMP檔案畫布 除錯用
        Bitmap BitmapBuf = new Bitmap(DPI_Funs.MillimetersToPixels(40, 203), DPI_Funs.MillimetersToPixels(25, 203));//建立BMP記憶體空間
        Graphics g_bmp = Graphics.FromImage(BitmapBuf);//從BMP記憶體自建畫布 ~ https://stackoverflow.com/questions/10868623/converting-print-page-graphics-to-bitmap-c-sharp
        g_bmp.Clear(Color.White);//畫布指定底色
        //產生BMP檔案畫布 除錯用*/

        for (int i = 0; i < m_PT_Page.ChildElements.Count; i++)//依序處理Page的內容
        {
            //---
            //Debug code
            if (i == 22)
            {
                bool blncheckpoint = true;
            }
            //---Debug code

            PT_ChildElement PT_ChildElementBuf = GetDataElement(m_PT_Page.ChildElements[i]);
            if (PT_ChildElementBuf != null)
            {
                m_strRealData = Element2Data(PT_ChildElementBuf);//元件轉資料
                Data2Image(PT_ChildElementBuf, g);
            }

            //清空堆疊迴圈
            while (m_ContainerElements.Count > 0)
            {
                ContainerElement ContainerElementBuf = (ContainerElement)m_ContainerElements.Peek();//讀取堆疊資料但不刪除
                if (ContainerElementBuf.m_index < ContainerElementBuf.m_Element.ChildElements.Count)
                {
                    PT_ChildElementBuf = GetDataElement(ContainerElementBuf.m_Element.ChildElements[ContainerElementBuf.m_index]);
                    ContainerElementBuf.m_index++;//改變已處理的子元件編號(旗標)
                    if (PT_ChildElementBuf != null)
                    {
                        m_strRealData = Element2Data(PT_ChildElementBuf);//元件轉資料
                        Data2Image(PT_ChildElementBuf, g);
                    }
                }
                else
                {
                    if ((ContainerElementBuf.m_Element.RootName != null) && (ContainerElementBuf.m_Element.RootName.Length > 0))
                    {
                        m_strDataPath = GetStackPath(ref m_intDataPath);
                    }

                    if (m_blnGetDataElement)
                    {
                        m_strElement2DataLog += "\n";
                        m_blnGetDataElement = false;
                        m_fltLast_Height = m_fltMax_Height;//(不同列時，運算用)
                        m_fltMax_Height = 0;//初始化變數
                    }

                    if ((ContainerElementBuf.m_Element.ElementType == "Rows"))// || (ContainerElementBuf.m_Element.ElementType == "Block")
                    {
                        int intIndex = -1;
                        int intNum = -1;
                        int intCount = -1;
                        //string strDataPathBuf = "";
                        //int intDataPathBuf = -1;
                        //strDataPathBuf = GetStackPath(ref intDataPathBuf);
                        m_strDataPath = GetStackPath(ref m_intDataPath);
                        intCount = ForLoopVarsSet(m_strDataPath, ref intIndex, ref intNum);//intCount = ForLoopVarsSet(strDataPathBuf, ref intIndex, ref intNum);
                        if ((intCount != 0) && (intIndex < intCount))
                        {
                            ContainerElementBuf.m_index = 0;//改變已處理的子元件編號(旗標)~ 從同計算

                            //----
                            //相依變數全部也要觸發重置機制，當下次呼叫到ForLoopVarsSet就會執行
                            if (intNum == 0)//order_items
                            {
                                m_ForLoopVars[1].m_intIndex = -1;
                                m_ForLoopVars[1].m_intCount = -1;
                                m_ForLoopVars[2].m_intIndex = -1;
                                m_ForLoopVars[2].m_intCount = -1;
                                m_ForLoopVars[3].m_intIndex = -1;
                                m_ForLoopVars[3].m_intCount = -1;
                                m_ForLoopVars[4].m_intIndex = -1;
                                m_ForLoopVars[4].m_intCount = -1;
                            }
                            if (intNum == 1)//order_items.condiments
                            {

                            }
                            if (intNum == 2)//order_items.set_meals
                            {//驗證可以
                                /*
                                if (m_RecycleElements.Count > 0)
                                {
                                    m_ContainerElements.Push((ContainerElement)m_RecycleElements.Peek());
                                    m_RecycleElements.Pop();
                                }
                                //*/
                                m_ForLoopVars[3].m_intIndex = -1;
                                m_ForLoopVars[3].m_intCount = -1;
                                m_ForLoopVars[4].m_intIndex = -1;
                                m_ForLoopVars[4].m_intCount = -1;
                            }
                            if (intNum == 3)//order_items.set_meals.product
                            {
                                m_ForLoopVars[4].m_intIndex = -1;
                                m_ForLoopVars[4].m_intCount = -1;
                            }
                            if (intNum == 4)//order_items.set_meals.product.condiments
                            {

                            }
                            //---相依變數全部也要觸發重置機制，當下次呼叫到ForLoopVarsSet就會執行
                        }
                        else
                        {
                            /*
                            bool blnTableLoop = false;
                            ContainerElement[] ContainerElements = m_ContainerElements.ToArray();
                            for(int j= (ContainerElements.Length-1);j>=0;j--)
                            {
                                if (ContainerElements[j].m_Element.ElementType== "Table")
                                {
                                    blnTableLoop = true;
                                    break;
                                }
                            }
                            if (blnTableLoop && (ContainerElementBuf.m_Element.ChildElements.Count>0))
                            {
                                ContainerElementBuf.m_index = 1;
                                m_RecycleElements.Push(ContainerElementBuf);
                            }
                            */

                            m_ContainerElements.Pop();//移除堆疊最上面元件
                        }
                    }
                    else
                    {//Table 直接移除 ~ 堆疊只放Rows
                        m_ContainerElements.Pop();//移除堆疊最上面元件
                    }

                }
            }//while (m_ContainerElements.Count > 0)           
        }

        //BitmapBuf.Save("printer.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        Console.WriteLine(m_strElement2DataLog);
    }

    private void SingleProductPrintPage(object sender, PrintPageEventArgs e)//實際產生列印內容觸發函數
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

        try
        {

            SingleProductDrawingPage(g);
            m_blnResult = true;
            m_strResult = $"已經將列印頁面產生並傳送到對應的印表機柱列中";
            e.HasMorePages = false;//驅動切紙
        }
        catch (Exception ex)
        {
            m_blnResult = false;
            m_strResult = $"PrintPage運行失敗;{ex.Message}";
        }

    }
    //---一菜一切
}
class Program
{
    static void Pause()
    {
        Console.Write("Press any key to continue...");
        Console.ReadKey(true);
    }
    static void Main()
    {
        //報表印表機~
        string strPrinterDriverName = "80mm Series Printer";//"POS-80C";//"POS80D";//"80mm_TCPMode"; // 替換成你實際的熱感印表機名稱
        //標籤機~ string strPrinterDriverName = "DT-2205";

        StreamReader sr00 = new StreamReader(@"C:\Users\jashv\OneDrive\桌面\Input.json");
        string strOrderData = sr00.ReadToEnd();
        
        //報表~
        StreamReader sr01 = new StreamReader(@"C:\Users\jashv\OneDrive\桌面\GITHUB\CS_PrintDocument_ThermalPrinter\doc\Vteam印表模板規劃\印表模板\Number_57.json");
        //一菜一切~ StreamReader sr01 = new StreamReader(@"C:\Users\jashv\OneDrive\桌面\GITHUB\CS_PrintDocument_ThermalPrinter\doc\Vteam印表模板規劃\印表模板\SingleProduct_57.json");
        //標籤~StreamReader sr01 = new StreamReader(@"C:\Users\jashv\OneDrive\桌面\GITHUB\CS_PrintDocument_ThermalPrinter\doc\Vteam印表模板規劃\印表模板\提點落料機_40mm_50mm.json");
        string strPrintTemplate = sr01.ReadToEnd();
        
        CS_PrintTemplate CPT = new CS_PrintTemplate(strPrinterDriverName, strPrintTemplate, strOrderData);
        
        Pause();
    }
    static void Main_V0()
    {
        float SysDpiX, SysDpiY;
        DPI_Funs.GetScreenDpi(out SysDpiX,out SysDpiY);
        Bitmap bmp1 = Barcode_Funs.QrCode("相關網站: https://github.com/micjahn/ZXing.Net/issues/458");
        Bitmap bmp2 = Barcode_Funs.BarCode("1234567890",100,300);
        string targetPrinterName = "80mm Series Printer";//"POS80D";//"POS-80C";//"80mm_TCPMode"; // 替換成你實際的熱感印表機名稱

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
            Bitmap BitmapBuf = new Bitmap(DPI_Funs.MillimetersToPixels(79, 203), 5000);//建立BMP記憶體空間
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