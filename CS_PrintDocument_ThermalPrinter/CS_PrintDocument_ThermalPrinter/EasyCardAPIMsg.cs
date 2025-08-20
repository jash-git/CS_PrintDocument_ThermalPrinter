using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_PrintDocument_ThermalPrinter
{

    /*
    {
	    "SID": "6D03CBCC5F234669887675F6D85D663C",
	    "Message_Type": "0200",
	    "Trans_Code": "DEDUCT",
	    "Trans_Date": "20230421",
	    "Trans_Time": 1682058565,
	    "Trans_Amount": 50,
	    "Auto_Add_Value": 0,
	    "TMLocationID": "0000000001",
	    "Store_ID": "00010907",
	    "Pos_ID": "01",
	    "Employee_ID": "0001",
	    "Pos_Trans_Num": 213,
	    "Host_Serial_Num": 220013,
	    "Batch_No": "23042104",
	    "Dongle_Device_ID": "06100D9B2A00",
	    "New_Device_ID": "0001090701304102",
	    "Precessing_Code": "811599",
	    "Precessing_Name": "購貨",
	    "RRN": "23042122001261",
	    "Card_Info": {
		    "Physical_ID": "1719511104",
		    "Purse_ID": "",
		    "Receipt_Card_ID": "1719511104",
		    "Effective_Date": "20250810",
		    "Card_Type": "00",
		    "Balance_Amount": 103,
		    "Befer_Amount": 153,
		    "Serial_Num": ""
	    },
	    "Checkout_ID": "",
	    "Retry_Nex_Flag": "N",
	    "Trans_Success": "Y",
	    "Trans_Msg": "OK"
    }
    */
    public class ECAM_CardInfo
    {
        /*
        Physical_ID	T0200 悠遊卡號(內碼)
        Purse_ID	T0211 遊遊卡(外碼)
        Receipt_Card_ID	T0215 收據列印卡號
        Effective_Date	T1402 悠遊卡有效日期
        Card_Type	T0213 Card Type
        Balance_Amount	卡片餘額
        Befer_Amount	交易前餘額
        Serial_Num	T4808 卡片序號 
        */
        public string Physical_ID { get; set; }
        public string Purse_ID { get; set; }
        public string Receipt_Card_ID { get; set; }
        public string Effective_Date { get; set; }
        public string Card_Type { get; set; }
        public int Balance_Amount { get; set; }
        public int Befer_Amount { get; set; }
        public string Serial_Num { get; set; }
    }

    public class easycard_print_bill_data // [EasyCardAPIMsg] //遊遊卡 交易當下回饋 交易資訊
    {
        public string SID { get; set; }
        public string Message_Type { get; set; }
        public string Trans_Code { get; set; }
        public string Trans_Date { get; set; }
        public string trans_time_year { get; set; }
        public string trans_time_month { get; set; }
        public string trans_time_day { get; set; }
        public string trans_time_hours { get; set; }
        public string trans_time_minutes { get; set; }
        public long Trans_Time { get; set; }
        public int Trans_Amount { get; set; }
        public int Auto_Add_Value { get; set; }
        public string TMLocationID { get; set; }
        public string Store_ID { get; set; }
        public string Pos_ID { get; set; }
        public string Employee_ID { get; set; }
        public int Pos_Trans_Num { get; set; }
        public int Host_Serial_Num { get; set; }
        public string Batch_No { get; set; }
        public string Dongle_Device_ID { get; set; }
        public string New_Device_ID { get; set; }
        public string Precessing_Code { get; set; }
        public string Precessing_Name { get; set; }
        public string RRN { get; set; }
        public ECAM_CardInfo Card_Info { get; set; }
        public string Checkout_ID { get; set; }
        public string Retry_Nex_Flag { get; set; }
        public string Trans_Success { get; set; }
        public string Trans_Msg { get; set; }

        public void SetVariable()
        {
            DateTime DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(Trans_Time);
            if (DateTimeBuf != null)
            {
                trans_time_year = DateTimeBuf.ToString("yyyy");
                trans_time_month = DateTimeBuf.ToString("MM");
                trans_time_day = DateTimeBuf.ToString("dd");
                trans_time_hours = DateTimeBuf.ToString("HH");
                trans_time_minutes = DateTimeBuf.ToString("mm");
            }
        }
    }

    //----
    /*
    {
	    "SID": "491D5E93241D4C1D990FF33D66D94EA3",
	    "Message_Type": "0200",
	    "Trans_Code": "CASH_ADD_VALUE",
	    "Trans_Date": "20230420",
	    "Trans_Time": 1681957931,
	    "Trans_Amount": 100,
	    "Auto_Add_Value": 0,
	    "TMLocationID": "0000000001",
	    "Store_ID": "00010907",
	    "Pos_ID": "01",
	    "Employee_ID": "0001",
	    "Pos_Trans_Num": 175,
	    "Host_Serial_Num": 219918,
	    "Batch_No": "23041201",
	    "Dongle_Device_ID": "06100D9B2A00",
	    "New_Device_ID": "0001090701304102",
	    "Card_Info": {
		    "Physical_ID": "1719277600",
		    "Purse_ID": "",
		    "Receipt_Card_ID": "1719277600",
		    "Effective_Date": "20250810",
		    "Card_Type": "08",
		    "Balance_Amount": 551,
		    "Befer_Amount": 451,
		    "Serial_Num": ""
	    },
	    "Checkout_ID": "",
	    "Retry_Nex_Flag": "N",
	    "Trans_Success": "Y",
	    "Trans_Msg": "OK"
    }
    */
    public class ECAMGetCardBillInfoMsg//悠遊卡 事後查詢 交易資訊
    {
        public string SID { get; set; }
        public string Message_Type { get; set; }
        public string Trans_Code { get; set; }
        public string Trans_Date { get; set; }
        public int Trans_Time { get; set; }
        public int Trans_Amount { get; set; }
        public int Auto_Add_Value { get; set; }
        public string TMLocationID { get; set; }
        public string Store_ID { get; set; }
        public string Pos_ID { get; set; }
        public string Employee_ID { get; set; }
        public int Pos_Trans_Num { get; set; }
        public int Host_Serial_Num { get; set; }
        public string Batch_No { get; set; }
        public string Dongle_Device_ID { get; set; }
        public string New_Device_ID { get; set; }
        public ECAM_CardInfo Card_Info { get; set; }
        public string Checkout_ID { get; set; }
        public string Retry_Nex_Flag { get; set; }
        public string Trans_Success { get; set; }
        public string Trans_Msg { get; set; }
    }

    //---
    /*
    TMLocationID	string	特約商店店號(分店代碼)  T5503
    Store_ID	string	特約商店代號  T4200
    Pos_Trans_Num	integer	交易續號 T1100
    Host_Serial_Num	integer	Host Serial Num T1101
    Batch_No	string	交易批號 T5501
    Dongle_Device_ID	string	Dongle 機編號 T4100
    New_Device_ID	string	二代設備編號 T4110
    Sign_On_Time	integer	SignOn時間 UnixTimeStamp

    {
	    "TMLocationID": "0000000001",
	    "Store_ID": "00010907",
	    "Batch_No": "23041201",
	    "Pos_Trans_Num": 191,
	    "Host_Serial_Num": 219964,
	    "Dongle_Device_ID": "06100D9B2A00",
	    "New_Device_ID": "0001090701304102",
	    "Sign_On_Time": 1681971127
    }
    */
    public class ECAMGetBasicInfoMsg//讀取悠遊卡機 基礎參數
    {
        public string TMLocationID { get; set; }
        public string Store_ID { get; set; }
        public string Batch_No { get; set; }
        public int Pos_Trans_Num { get; set; }
        public int Host_Serial_Num { get; set; }
        public string Dongle_Device_ID { get; set; }
        public string New_Device_ID { get; set; }
        public int Sign_On_Time { get; set; }
    }

    //----
    /*
    {
	    "SID": "",
	    "Message_Type": "0500",
	    "Trans_Code": "CHECKOUT",
	    "Trans_Date": "20230425",
	    "Trans_Time": 1682390727,
	    "Trans_Amount": 0,
	    "Auto_Add_Value": 0,
	    "TMLocationID": "0000000001",
	    "Store_ID": "00010907",
	    "Pos_ID": "01",
	    "Employee_ID": "0001",
	    "Pos_Trans_Num": 218,
	    "Host_Serial_Num": 220024,
	    "Batch_No": "23042501",
	    "Dongle_Device_ID": "06100D9B2A00",
	    "New_Device_ID": "0001090701304102",
	    "Precessing_Code": "900000",
	    "Precessing_Name": "",
	    "RRN": "23042522002400",
	    "Checkout_Info": {
		    "Checkout_ID": "34F07FABBC1F470E94C38C3B7F1C09ED",
		    "Batch_No": "23042501",
		    "Pos_Trans_Num": 218,
		    "Host_Serial_Num": 220024,
		    "TMLocationID": "0000000001",
		    "Store_ID": "00010907",
		    "Pos_ID": "01",
		    "Employee_ID": "0001",
		    "Dongle_Device_ID": "06100D9B2A00",
		    "New_Device_ID": "0001090701304102",
		    "Sale_Count": 0,
		    "Sale_Amount": 0,
		    "Refund_Count": 0,
		    "Refund_Amount": 0,
		    "Auto_Add_Value_Count": 0,
		    "Auto_Add_Value_Amount": 0,
		    "Trans_Count": 0,
		    "Trans_Amount": 0,
		    "Checkout_Time": 1682390727,
		    "Checkout_Success": "Y",
		    "Checkout_Match": "Y",
		    "Checkout_Msg": "01"
	    },
	    "Retry_Nex_Flag": "N",
	    "Trans_Success": "Y",
	    "Trans_Msg": "OK"
    }
    */
    public class ECAMCheckoutInfo
    {
        public string Checkout_ID { get; set; }
        public string Batch_No { get; set; }
        public int Pos_Trans_Num { get; set; }
        public int Host_Serial_Num { get; set; }
        public string TMLocationID { get; set; }
        public string Store_ID { get; set; }
        public string Pos_ID { get; set; }
        public string Employee_ID { get; set; }
        public string Dongle_Device_ID { get; set; }
        public string New_Device_ID { get; set; }
        public int Sale_Count { get; set; }
        public int Sale_Amount { get; set; }
        public int Refund_Count { get; set; }
        public int Refund_Amount { get; set; }
        public int Auto_Add_Value_Count { get; set; }
        public int Auto_Add_Value_Amount { get; set; }
        public int Trans_Count { get; set; }
        public int Trans_Amount { get; set; }
        public int Checkout_Time { get; set; }
        public string Checkout_Success { get; set; }
        public string Checkout_Match { get; set; }
        public string Checkout_Msg { get; set; }
    }

    public class easycard_print_checkout_data // [ECAMCheckout]//悠遊卡 結帳資訊
    {
        public string SID { get; set; }
        public string Message_Type { get; set; }
        public string Trans_Code { get; set; }
        public string Trans_Date { get; set; }
        public string trans_time_year { get; set; }
        public string trans_time_month { get; set; }
        public string trans_time_day { get; set; }
        public string trans_time_hours { get; set; }
        public string trans_time_minutes { get; set; }
        public int Trans_Time { get; set; }
        public int Trans_Amount { get; set; }
        public int Auto_Add_Value { get; set; }
        public string TMLocationID { get; set; }
        public string Store_ID { get; set; }
        public string Pos_ID { get; set; }
        public string Employee_ID { get; set; }
        public int Pos_Trans_Num { get; set; }
        public int Host_Serial_Num { get; set; }
        public string Batch_No { get; set; }
        public string Dongle_Device_ID { get; set; }
        public string New_Device_ID { get; set; }
        public string Precessing_Code { get; set; }
        public string Precessing_Name { get; set; }
        public string RRN { get; set; }
        public ECAMCheckoutInfo Checkout_Info { get; set; }
        public string Retry_Nex_Flag { get; set; }
        public string Trans_Success { get; set; }
        public string Trans_Msg { get; set; }
        public int Net_Total {  get; set; }//購貨類總淨額

        public void SetVariable()
        {
            DateTime DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(Trans_Time);
            if (DateTimeBuf != null)
            {
                trans_time_year = DateTimeBuf.ToString("yyyy");
                trans_time_month = DateTimeBuf.ToString("MM");
                trans_time_day = DateTimeBuf.ToString("dd");
                trans_time_hours = DateTimeBuf.ToString("HH");
                trans_time_minutes = DateTimeBuf.ToString("mm");
            }

            Net_Total = Checkout_Info.Sale_Amount - Checkout_Info.Refund_Amount;//購貨類總淨額
        }
    }

}
