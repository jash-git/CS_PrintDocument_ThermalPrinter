using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.ComponentModel;

namespace CS_PrintDocument_ThermalPrinter
{
    /// <summary>
    /// 列印功能
    /// </summary>
    public class PrinterHelper
    {
        private PrinterHelper() { }
        #region API聲明
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structPrinterDefaults
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pDatatype;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.I4)]
            public int DesiredAccess;
        };

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPTStr)] string printerName, out IntPtr phPrinter, ref structPrinterDefaults pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool ClosePrinter(IntPtr phPrinter);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structSize
        {
            public Int32 width;
            public Int32 height;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structRect
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        internal struct FormInfo1
        {
            [FieldOffset(0), MarshalAs(UnmanagedType.I4)]
            public uint Flags;
            [FieldOffset(4), MarshalAs(UnmanagedType.LPWStr)]
            public String pName;
            [FieldOffset(8)]
            public structSize Size;
            [FieldOffset(16)]
            public structRect ImageableArea;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct structDevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String
             dmDeviceName;
            [MarshalAs(UnmanagedType.U2)]
            public short dmSpecVersion;
            [MarshalAs(UnmanagedType.U2)]
            public short dmDriverVersion;
            [MarshalAs(UnmanagedType.U2)]
            public short dmSize;
            [MarshalAs(UnmanagedType.U2)]
            public short dmDriverExtra;
            [MarshalAs(UnmanagedType.U4)]
            public int dmFields;
            [MarshalAs(UnmanagedType.I2)]
            public short dmOrientation;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPaperSize;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPaperLength;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPaperWidth;
            [MarshalAs(UnmanagedType.I2)]
            public short dmScale;
            [MarshalAs(UnmanagedType.I2)]
            public short dmCopies;
            [MarshalAs(UnmanagedType.I2)]
            public short dmDefaultSource;
            [MarshalAs(UnmanagedType.I2)]
            public short dmPrintQuality;
            [MarshalAs(UnmanagedType.I2)]
            public short dmColor;
            [MarshalAs(UnmanagedType.I2)]
            public short dmDuplex;
            [MarshalAs(UnmanagedType.I2)]
            public short dmYResolution;
            [MarshalAs(UnmanagedType.I2)]
            public short dmTTOption;
            [MarshalAs(UnmanagedType.I2)]
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String dmFormName;
            [MarshalAs(UnmanagedType.U2)]
            public short dmLogPixels;
            [MarshalAs(UnmanagedType.U4)]
            public int dmBitsPerPel;
            [MarshalAs(UnmanagedType.U4)]
            public int dmPelsWidth;
            [MarshalAs(UnmanagedType.U4)]
            public int dmPelsHeight;
            [MarshalAs(UnmanagedType.U4)]
            public int dmNup;
            [MarshalAs(UnmanagedType.U4)]
            public int dmDisplayFrequency;
            [MarshalAs(UnmanagedType.U4)]
            public int dmICMMethod;
            [MarshalAs(UnmanagedType.U4)]
            public int dmICMIntent;
            [MarshalAs(UnmanagedType.U4)]
            public int dmMediaType;
            [MarshalAs(UnmanagedType.U4)]
            public int dmDitherType;
            [MarshalAs(UnmanagedType.U4)]
            public int dmReserved1;
            [MarshalAs(UnmanagedType.U4)]
            public int dmReserved2;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct PRINTER_INFO_9
        {
            public IntPtr pDevMode;
        }

        [DllImport("winspool.Drv", EntryPoint = "AddFormW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true,
                    CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool AddForm(IntPtr phPrinter, [MarshalAs(UnmanagedType.I4)] int level, ref FormInfo1 form);

        [DllImport("winspool.Drv", EntryPoint = "DeleteForm", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
                  SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool DeleteForm(IntPtr phPrinter, [MarshalAs(UnmanagedType.LPTStr)] string pName);

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false, ExactSpelling = true, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern Int32 GetLastError();

        [DllImport("GDI32.dll", EntryPoint = "CreateDC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)] string pDrive, [MarshalAs(UnmanagedType.LPTStr)] string pName, [MarshalAs(UnmanagedType.LPTStr)] string pOutput, ref structDevMode pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "ResetDC", SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern IntPtr ResetDC(IntPtr hDC, ref structDevMode pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "DeleteDC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false,
                    CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool DeleteDC(IntPtr hDC);

        [DllImport("winspool.Drv", EntryPoint = "SetPrinterA", SetLastError = true,
          CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool SetPrinter(IntPtr hPrinter, [MarshalAs(UnmanagedType.I4)] int level, IntPtr pPrinter, [MarshalAs(UnmanagedType.I4)] int command);

        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesA", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter, [MarshalAs(UnmanagedType.LPStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool GetPrinter(IntPtr hPrinter, int dwLevel, IntPtr pPrinter, int dwBuf, out int dwNeeded);

        [Flags]
        internal enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }
        const int WM_SETTINGCHANGE = 0x001A;
        const int HWND_BROADCAST = 0xffff;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessageTimeout(IntPtr windowHandle, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flags, uint timeout, out IntPtr result);

        //EnumPrinters用到的函數和結構體
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumPrinters(PrinterEnumFlags Flags, string Name, uint Level, IntPtr pPrinterEnum, uint cbBuf, ref uint pcbNeeded, ref uint pcReturned);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PRINTER_INFO_2
        {
            public string pServerName;
            public string pPrinterName;
            public string pShareName;
            public string pPortName;
            public string pDriverName;
            public string pComment;
            public string pLocation;
            public IntPtr pDevMode;
            public string pSepFile;
            public string pPrintProcessor;
            public string pDatatype;
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }

        [FlagsAttribute]
        internal enum PrinterEnumFlags
        {
            PRINTER_ENUM_DEFAULT = 0x00000001,
            PRINTER_ENUM_LOCAL = 0x00000002,
            PRINTER_ENUM_CONNECTIONS = 0x00000004,
            PRINTER_ENUM_FAVORITE = 0x00000004,
            PRINTER_ENUM_NAME = 0x00000008,
            PRINTER_ENUM_REMOTE = 0x00000010,
            PRINTER_ENUM_SHARED = 0x00000020,
            PRINTER_ENUM_NETWORK = 0x00000040,
            PRINTER_ENUM_EXPAND = 0x00004000,
            PRINTER_ENUM_CONTAINER = 0x00008000,
            PRINTER_ENUM_ICONMASK = 0x00ff0000,
            PRINTER_ENUM_ICON1 = 0x00010000,
            PRINTER_ENUM_ICON2 = 0x00020000,
            PRINTER_ENUM_ICON3 = 0x00040000,
            PRINTER_ENUM_ICON4 = 0x00080000,
            PRINTER_ENUM_ICON5 = 0x00100000,
            PRINTER_ENUM_ICON6 = 0x00200000,
            PRINTER_ENUM_ICON7 = 0x00400000,
            PRINTER_ENUM_ICON8 = 0x00800000,
            PRINTER_ENUM_HIDE = 0x01000000
        }

        //印表機狀態
        [FlagsAttribute]
        internal enum PrinterStatus
        {
            PRINTER_STATUS_BUSY = 0x00000200,
            PRINTER_STATUS_DOOR_OPEN = 0x00400000,
            PRINTER_STATUS_ERROR = 0x00000002,
            PRINTER_STATUS_INITIALIZING = 0x00008000,
            PRINTER_STATUS_IO_ACTIVE = 0x00000100,
            PRINTER_STATUS_MANUAL_FEED = 0x00000020,
            PRINTER_STATUS_NO_TONER = 0x00040000,
            PRINTER_STATUS_NOT_AVAILABLE = 0x00001000,
            PRINTER_STATUS_OFFLINE = 0x00000080,
            PRINTER_STATUS_OUT_OF_MEMORY = 0x00200000,
            PRINTER_STATUS_OUTPUT_BIN_FULL = 0x00000800,
            PRINTER_STATUS_PAGE_PUNT = 0x00080000,
            PRINTER_STATUS_PAPER_JAM = 0x00000008,
            PRINTER_STATUS_PAPER_OUT = 0x00000010,
            PRINTER_STATUS_PAPER_PROBLEM = 0x00000040,
            PRINTER_STATUS_PAUSED = 0x00000001,
            PRINTER_STATUS_PENDING_DELETION = 0x00000004,
            PRINTER_STATUS_PRINTING = 0x00000400,
            PRINTER_STATUS_PROCESSING = 0x00004000,
            PRINTER_STATUS_TONER_LOW = 0x00020000,
            PRINTER_STATUS_USER_INTERVENTION = 0x00100000,
            PRINTER_STATUS_WAITING = 0x20000000,
            PRINTER_STATUS_WARMING_UP = 0x00010000
        }

        //GetDefaultPrinter用到的API函數說明
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int size);

        //SetDefaultPrinter用到的API函式宣告
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetDefaultPrinter(string Name);

        //EnumFormsA用到的函式宣告，應該和EnumPrinters類似
        [DllImport("winspool.drv", EntryPoint = "EnumForms")]
        internal static extern int EnumFormsA(IntPtr hPrinter, int Level, ref byte pForm, int cbBuf, ref int pcbNeeded, ref int pcReturned);

        #endregion   API聲明
        internal static int GetPrinterStatusInt(string PrinterName)
        {
            int intRet = 0;
            IntPtr hPrinter;
            structPrinterDefaults defaults = new structPrinterDefaults();
            if (OpenPrinter(PrinterName, out hPrinter, ref defaults))
            {
                int cbNeeded = 0;
                bool bolRet = GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out cbNeeded);
                if (cbNeeded > 0)
                {
                    IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
                    bolRet = GetPrinter(hPrinter, 2, pAddr, cbNeeded, out cbNeeded);
                    if (bolRet)
                    {
                        PRINTER_INFO_2 Info2 = new PRINTER_INFO_2();

                        Info2 = (PRINTER_INFO_2)Marshal.PtrToStructure(pAddr, typeof(PRINTER_INFO_2));

                        intRet = System.Convert.ToInt32(Info2.Status);
                    }
                    Marshal.FreeHGlobal(pAddr);
                }
                ClosePrinter(hPrinter);
            }
            return intRet;
        }
        internal static PRINTER_INFO_2[] EnumPrintersByFlag(PrinterEnumFlags Flags)
        {
            uint cbNeeded = 0;
            uint cReturned = 0;
            bool ret = EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL, null, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned);
            IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
            ret = EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL, null, 2, pAddr, cbNeeded, ref cbNeeded, ref cReturned);
            if (ret)
            {
                PRINTER_INFO_2[] Info2 = new PRINTER_INFO_2[cReturned];
                int offset = pAddr.ToInt32();
                for (int i = 0; i < cReturned; i++)
                {
                    Info2[i].pServerName = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pPrinterName = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pShareName = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pPortName = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pDriverName = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pComment = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pLocation = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pDevMode = Marshal.ReadIntPtr(new IntPtr(offset));
                    offset += 4;
                    Info2[i].pSepFile = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pPrintProcessor = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pDatatype = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pParameters = Marshal.PtrToStringAuto(Marshal.ReadIntPtr(new IntPtr(offset)));
                    offset += 4;
                    Info2[i].pSecurityDescriptor = Marshal.ReadIntPtr(new IntPtr(offset));
                    offset += 4;
                    Info2[i].Attributes = (uint)Marshal.ReadIntPtr(new IntPtr(offset));
                    offset += 4;
                    Info2[i].Priority = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                    Info2[i].DefaultPriority = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                    Info2[i].StartTime = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                    Info2[i].UntilTime = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                    Info2[i].Status = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                    Info2[i].cJobs = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                    Info2[i].AveragePPM = (uint)Marshal.ReadInt32(new IntPtr(offset));
                    offset += 4;
                }
                Marshal.FreeHGlobal(pAddr);
                return Info2;
            }
            else
            {
                return new PRINTER_INFO_2[0];
            }
        }
        #region 獲取當前指定印表機的狀態
        /// </summary>
        /// 獲取當前指定印表機的狀態
        /// </summary>
        /// <param name="PrinterName">印表機名稱</param>
        /// <returns>印表機狀態描述</returns>

        public static string GetPrinterStatus(string PrinterName)
        {
            int intValue = GetPrinterStatusInt(PrinterName);
            string strRet = string.Empty;
            switch (intValue)
            {
                case 0:
                    strRet = "準備就緒（Ready）";
                    break;
                case 0x00000200:
                    strRet = "忙(Busy）";
                    break;
                case 0x00400000:
                    strRet = "門被打開（Printer Door Open）";
                    break;
                case 0x00000002:
                    strRet = "錯誤(Printer Error）";
                    break;
                case 0x0008000:
                    strRet = "正在初始化(Initializing）";
                    break;
                case 0x00000100:
                    strRet = "正在輸入或輸出（I/O Active）";
                    break;
                case 0x00000020:
                    strRet = "手工送紙（Manual Feed）";
                    break;
                case 0x00040000:
                    strRet = "無墨粉（No Toner）";
                    break;
                case 0x00001000:
                    strRet = "不可用（Not Available）";
                    break;
                case 0x00000080:
                    strRet = "離線（Off Line）";
                    break;
                case 0x00200000:
                    strRet = "記憶體溢出（Out of Memory）";
                    break;
                case 0x00000800:
                    strRet = "輸出口已滿（Output Bin Full）";
                    break;
                case 0x00080000:
                    strRet = "當前頁無法列印（Page Punt）";
                    break;
                case 0x00000008:
                    strRet = "塞紙（Paper Jam）";
                    break;
                case 0x00000010:
                    strRet = "打印紙用完（Paper Out）";
                    break;
                case 0x00000040:
                    strRet = "紙張問題（Page Problem）";
                    break;
                case 0x00000001:
                    strRet = "暫停（Paused）";
                    break;
                case 0x00000004:
                    strRet = "正在刪除（Pending Deletion）";
                    break;
                case 0x00000400:
                    strRet = "正在列印（Printing）";
                    break;
                case 0x00004000:
                    strRet = "正在處理（Processing）";
                    break;
                case 0x00020000:
                    strRet = "墨粉不足（Toner Low）";
                    break;
                case 0x00100000:
                    strRet = "需要使用者干預（User Intervention）";
                    break;
                case 0x20000000:
                    strRet = "等待（Waiting）";
                    break;
                case 0x00010000:
                    strRet = "正在準備（Warming Up）";
                    break;
                default:
                    strRet = "未知狀態（Unknown Status）";
                    break;
            }
            return strRet;
        }
        #endregion 獲取當前指定印表機的狀態

        #region 刪除已經存在的自訂紙張
        /**/
        /// <summary>
        /// 刪除已經存在的自訂紙張
        /// </summary>
        /// <param name="PrinterName">印表機名稱</param>
        /// <param name="PaperName">紙張名稱</param>
        public static void DeleteCustomPaperSize(string PrinterName, string PaperName)
        {
            const int PRINTER_ACCESS_USE = 0x00000008;
            const int PRINTER_ACCESS_ADMINISTER = 0x00000004;

            structPrinterDefaults defaults = new structPrinterDefaults();
            defaults.pDatatype = null;
            defaults.pDevMode = IntPtr.Zero;
            defaults.DesiredAccess = PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE;

            IntPtr hPrinter = IntPtr.Zero;

            //打開印表機
            if (OpenPrinter(PrinterName, out hPrinter, ref defaults))
            {
                try
                {
                    DeleteForm(hPrinter, PaperName);
                    ClosePrinter(hPrinter);
                }
                catch
                {

                }
            }
        }
        #endregion 刪除已經存在的自訂紙張

        #region 指定的印表機設置以mm為單位的自訂紙張(Form)
        /**/
        /// <summary>
        /// 指定的印表機設置以mm為單位的自訂紙張(Form)
        /// </summary>
        /// <param name="PrinterName">印表機名稱</param>
        /// <param name="PaperName">Form名稱</param>
        /// <param name="WidthInMm">以mm為單位的寬度</param>
        /// <param name="HeightInMm">以mm為單位的高度</param>
        public static void AddCustomPaperSize(string PrinterName, string PaperName, float WidthInMm, float HeightInMm)
        {
            if (PlatformID.Win32NT == Environment.OSVersion.Platform)
            {
                const int PRINTER_ACCESS_USE = 0x00000008;
                const int PRINTER_ACCESS_ADMINISTER = 0x00000004;
                structPrinterDefaults defaults = new structPrinterDefaults();
                defaults.pDatatype = null;
                defaults.pDevMode = IntPtr.Zero;
                defaults.DesiredAccess = PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE;
                IntPtr hPrinter = IntPtr.Zero;
                //打開印表機
                if (OpenPrinter(PrinterName, out hPrinter, ref defaults))
                {
                    try
                    {
                        //如果Form存在刪除之
                        DeleteForm(hPrinter, PaperName);
                        //創建並初始化FORM_INFO_1
                        FormInfo1 formInfo = new FormInfo1();
                        formInfo.Flags = 0;
                        formInfo.pName = PaperName;
                        formInfo.Size.width = (int)(WidthInMm * 1000.0);
                        formInfo.Size.height = (int)(HeightInMm * 1000.0);
                        formInfo.ImageableArea.left = 0;
                        formInfo.ImageableArea.right = formInfo.Size.width;
                        formInfo.ImageableArea.top = 0;
                        formInfo.ImageableArea.bottom = formInfo.Size.height;
                        if (!AddForm(hPrinter, 1, ref formInfo))
                        {
                            StringBuilder strBuilder = new StringBuilder();
                            strBuilder.AppendFormat("向印表機 {1} 添加自訂紙張 {0} 失敗！錯誤代號：{2}",
                             PaperName, PrinterName, GetLastError());
                            throw new ApplicationException(strBuilder.ToString());
                        }

                        //初始化
                        const int DM_OUT_BUFFER = 2;
                        const int DM_IN_BUFFER = 8;
                        structDevMode devMode = new structDevMode();
                        IntPtr hPrinterInfo, hDummy;
                        PRINTER_INFO_9 printerInfo;
                        printerInfo.pDevMode = IntPtr.Zero;
                        int iPrinterInfoSize, iDummyInt;


                        int iDevModeSize = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName, IntPtr.Zero, IntPtr.Zero, 0);

                        if (iDevModeSize < 0)
                            throw new ApplicationException("無法取得DEVMODE結構的大小！");

                        //分配緩衝
                        IntPtr hDevMode = Marshal.AllocCoTaskMem(iDevModeSize + 100);

                        //獲取DEV_MODE指針
                        int iRet = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName, hDevMode, IntPtr.Zero, DM_OUT_BUFFER);

                        if (iRet < 0)
                            throw new ApplicationException("無法獲得DEVMODE結構！");

                        //填充DEV_MODE
                        devMode = (structDevMode)Marshal.PtrToStructure(hDevMode, devMode.GetType());


                        devMode.dmFields = 0x10000;

                        //FORM名稱
                        devMode.dmFormName = PaperName;

                        Marshal.StructureToPtr(devMode, hDevMode, true);

                        iRet = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName,
                         printerInfo.pDevMode, printerInfo.pDevMode, DM_IN_BUFFER | DM_OUT_BUFFER);

                        if (iRet < 0)
                            throw new ApplicationException("無法為印表機設定列印方向！");

                        GetPrinter(hPrinter, 9, IntPtr.Zero, 0, out iPrinterInfoSize);
                        if (iPrinterInfoSize == 0)
                            throw new ApplicationException("調用GetPrinter方法失敗！");

                        hPrinterInfo = Marshal.AllocCoTaskMem(iPrinterInfoSize + 100);

                        bool bSuccess = GetPrinter(hPrinter, 9, hPrinterInfo, iPrinterInfoSize, out iDummyInt);

                        if (!bSuccess)
                            throw new ApplicationException("調用GetPrinter方法失敗！");

                        printerInfo = (PRINTER_INFO_9)Marshal.PtrToStructure(hPrinterInfo, printerInfo.GetType());
                        printerInfo.pDevMode = hDevMode;

                        Marshal.StructureToPtr(printerInfo, hPrinterInfo, true);

                        bSuccess = SetPrinter(hPrinter, 9, hPrinterInfo, 0);

                        if (!bSuccess)
                            throw new Win32Exception(Marshal.GetLastWin32Error(), "調用SetPrinter方法失敗，無法進行印表機設置！");

                        SendMessageTimeout(
                         new IntPtr(HWND_BROADCAST),
                         WM_SETTINGCHANGE,
                         IntPtr.Zero,
                         IntPtr.Zero,
                         PrinterHelper.SendMessageTimeoutFlags.SMTO_NORMAL,
                         1000,
                         out hDummy);
                    }
                    finally
                    {
                        ClosePrinter(hPrinter);
                    }
                }
                else
                {
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.AppendFormat("無法打開印表機{0}, 錯誤代號: {1}",
                     PrinterName, GetLastError());
                    throw new ApplicationException(strBuilder.ToString());
                }
            }
            else
            {
                structDevMode pDevMode = new structDevMode();
                IntPtr hDC = CreateDC(null, PrinterName, null, ref pDevMode);
                if (hDC != IntPtr.Zero)
                {
                    const long DM_PAPERSIZE = 0x00000002L;
                    const long DM_PAPERLENGTH = 0x00000004L;
                    const long DM_PAPERWIDTH = 0x00000008L;
                    pDevMode.dmFields = (int)(DM_PAPERSIZE | DM_PAPERWIDTH | DM_PAPERLENGTH);
                    pDevMode.dmPaperSize = 256;
                    pDevMode.dmPaperWidth = (short)(WidthInMm * 1000.0);
                    pDevMode.dmPaperLength = (short)(HeightInMm * 1000.0);
                    ResetDC(hDC, ref pDevMode);
                    DeleteDC(hDC);
                }
            }
        }
        #endregion 指定的印表機設置以mm為單位的自訂紙張(Form)

        #region 獲取本地印表機列表
        /**/
        /// <summary>
        /// 獲取本地印表機列表
        /// 可以通過制定參數獲取網路印表機
        /// </summary>
        /// <returns>印表機列表</returns>
        public static System.Collections.ArrayList GetPrinterList()
        {
            System.Collections.ArrayList alRet = new System.Collections.ArrayList();
            PRINTER_INFO_2[] Info2 = EnumPrintersByFlag(PrinterEnumFlags.PRINTER_ENUM_LOCAL);
            for (int i = 0; i < Info2.Length; i++)
            {
                alRet.Add(Info2[i].pPrinterName);
            }
            return alRet;
        }
        #endregion 獲取本地印表機列表

        #region 獲取本機的預設印表機名稱
        /**/
        /// <summary>
        /// 獲取本機的預設印表機名稱
        /// </summary>
        /// <returns>預設印表機名稱</returns>
        public static string GetDeaultPrinterName()
        {
            StringBuilder dp = new StringBuilder(256);
            int size = dp.Capacity;
            if (GetDefaultPrinter(dp, ref size))
            {
                return dp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion 獲取本機的預設印表機名稱

        #region 設置預設印表機
        /**/
        /// <summary>
        /// 設置預設印表機
        /// </summary>
        /// <param name="PrinterName">可用的印表機名稱</param>
        public static void SetPrinterToDefault(string PrinterName)
        {
            SetDefaultPrinter(PrinterName);
        }
        #endregion 設置預設印表機

        #region 判斷印表機是否在系統可用的印表機清單中
        /**/
        ///// <summary>
        ///// 判斷印表機是否在系統可用的印表機清單中
        ///// </summary>
        ///// <param name="PrinterName">印表機名稱</param>
        ///// <returns>是：在；否：不在</returns>
        public static bool PrinterInList(string PrinterName)
        {
            bool bolRet = false;

            System.Collections.ArrayList alPrinters = GetPrinterList();

            for (int i = 0; i < alPrinters.Count; i++)
            {
                if (PrinterName == alPrinters[i].ToString())
                {
                    bolRet = true;
                    break;
                }
            }

            alPrinters.Clear();
            alPrinters = null;

            return bolRet;
        }
        #endregion 判斷印表機是否在系統可用的印表機清單中

        #region 判斷表單是否在指定的印表機所支援的紙張列表中
        /**/
        ///// <summary>
        ///// 判斷表單是否在指定的印表機所支持的紙張列表中,表單就是我們平常所說的紙張
        ///// </summary>
        ///// <param name="PrinterName">印表機名稱</param>
        ///// <param name="PaperName">紙張名稱</param>
        ///// <returns>是：在；否：不在</returns>
        public static bool FormInPrinter(string PrinterName, string PaperName)
        {
            bool bolRet = false;

            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

            pd.PrinterSettings.PrinterName = PrinterName;

            foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
            {
                if (ps.PaperName == PaperName)
                {
                    bolRet = true;
                    break;
                }
            }

            pd.Dispose();

            return bolRet;
        }
        #endregion 判斷表單是否在指定的印表機所支援的紙張列表中

        #region 判斷指定紙張的寬度和高度和與列印內容指定的寬度和高度是否匹配
        /**/
        /// <summary>
        /// 判斷指定紙張的寬度和高度和與列印內容指定的寬度和高度是否匹配
        /// </summary>
        /// <param name="PrinterName">印表機名稱</param>
        /// <param name="FormName">表單名稱</param>
        /// <param name="Width">寬度</param>
        /// <param name="Height">高度</param>
        /// <returns></returns>
        public static bool FormSameSize(string PrinterName, string FormName, decimal Width, decimal Height)
        {
            bool bolRet = false;

            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

            pd.PrinterSettings.PrinterName = PrinterName;

            foreach (System.Drawing.Printing.PaperSize ps in pd.PrinterSettings.PaperSizes)
            {
                if (ps.PaperName == FormName)
                {
                    decimal decWidth = FromInchToCM(System.Convert.ToDecimal(ps.Width));
                    decimal decHeight = FromInchToCM(System.Convert.ToDecimal(ps.Height));
                    //只要整數位元相同即認為是同一紙張，畢竟inch到cm的轉換並不能整除
                    if (Math.Round(decWidth, 0) == Math.Round(Width, 0) && Math.Round(decHeight, 0) == Math.Round(Height, 0))
                        bolRet = true;
                    break;
                }
            }

            pd.Dispose();

            return bolRet;
        }
        #endregion 判斷指定紙張的寬度和高度和與列印內容指定的寬度和高度是否匹配

        #region 英寸到釐米的轉換
        /**/
        /// <summary>
        /// 英寸到釐米的轉換
        /// /* = = = = = = = = = = = = = = = = *\
        /// | 換算一下計量單位，將其換算成釐米  |
        /// |    釐米     圖元     英寸         |
        /// |     1        38     0.395         |
        /// |   0.026       1      0.01         |
        /// |    2.54      96        1          |
        /// \* = = = = = = = = = = = = = = = = */
        /// </summary>
        /// <param name="inch">英寸數</param>
        /// <returns>釐米數，兩位小數</returns>
        ///
        public static decimal FromInchToCM(decimal inch)
        {
            return Math.Round((System.Convert.ToDecimal((inch / 100)) * System.Convert.ToDecimal(2.5400)), 2);
        }
        #endregion 英寸到釐米的轉換

        #region 列印操作
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);
        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);
        /// <summary>        
        /// 該方法把非託管記憶體中的位元組陣列發送到印表機的列印佇列        
        /// </summary>        
        /// <param name="szPrinterName">印表機名稱</param>
        /// <param name="pBytes">非託管記憶體指標</param>
        /// <param name="dwCount">位元組數</param>
        /// <returns>成功返回true，失敗時為false</returns>        
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";
            try
            {
                // 打開印表機                
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // 啟動文檔列印                    
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // 開始列印                        
                        if (StartPagePrinter(hPrinter))
                        {
                            // 向印表機輸出位元組                              
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
            }
            catch (Win32Exception ex)
            {
                bSuccess = false;
            }
            return bSuccess;
        }
        /// <summary>        
        /// 發送檔到印表機方法        
        /// </summary>        
        /// <param name="szPrinterName">印表機名稱</param>        
        /// <param name="szFileName">列印檔案的路徑</param>        
        /// <returns></returns>        
        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            bool bSuccess = false;
            try
            {
                // 打開文件              
                FileStream fs = new FileStream(szFileName, FileMode.Open);
                // 將檔內容讀作二進位               
                BinaryReader br = new BinaryReader(fs);
                // 定義位元組陣列              
                Byte[] bytes = new Byte[fs.Length];
                // 非託管指針              
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;
                nLength = Convert.ToInt32(fs.Length);
                // 讀取檔內容到位元組陣列               
                bytes = br.ReadBytes(nLength);
                // 為這些位元組分配一些非託管記憶體        
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // 將託管位元組陣列複製到非託管記憶體指標             
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
                // 將非託管位元組發送到印表機             
                bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
                // 釋放先前分配的非託管記憶體          
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                fs.Close();
                fs.Dispose();
            }
            catch (Win32Exception ex)
            {
                bSuccess = false;
            }
            return bSuccess;
        }
        /// <summary>  
        /// 將字串發送到印表機方法       
        /*
            //列印示例：
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("--------------列印測試--------------");
            string PrintName = PrinterHelper.GetDeaultPrinterName();
            PrinterHelper.SendStringToPrinter(PrintName, builder.ToString());
        */
        /// </summary>      
        /// <param name="szPrinterName">印表機名稱</param>        
        /// <param name="szString">列印的字串</param>       
        /// <returns></returns>       
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            bool flag = false;
            try
            {
                // 讀取檔內容到位元組陣列
                byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(szString.ToString());
                Int32 dwCount = bytes.Length;
                // 非託管指針              
                IntPtr pBytes = Marshal.AllocHGlobal(dwCount);
                // 將託管位元組陣列複製到非託管記憶體指標          
                Marshal.Copy(bytes, 0, pBytes, dwCount);
                // 將非託管位元組發送到印表機            
                flag = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                if (flag)
                {
                    StartQianXiang(szPrinterName);  //開錢箱操作
                }
                // 釋放先前分配的非託管記憶體              
                Marshal.FreeCoTaskMem(pBytes);
            }
            catch (Win32Exception ex)
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 開始彈出錢箱
        /// </summary>
        public static void StartQianXiang(string szPrinterName)
        {
            //不同的印表機需要不同的參數，這個參數應該可以在印表機的程式設計文檔中找到
            string str = ((char)27).ToString() + ((char)112).ToString() + ((char)0).ToString() + ((char)0).ToString() + ((char)0).ToString();
            byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            Int32 dwCount = bytes.Length;
            IntPtr pBytes = Marshal.AllocHGlobal(dwCount);
            Marshal.Copy(bytes, 0, pBytes, dwCount);
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDataType;
    }
    #endregion
}

