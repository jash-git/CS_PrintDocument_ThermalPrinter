using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CS_PrintDocument_ThermalPrinter
{
    /*
    報表json格式 從測試機DB(DB_DATABASE=vteam-cloud-db DB_TABLE=upload_queue_data)取得
	{
		"version": "1.6.5.4",
		"device_code": "",
		"report_no": "DR-202302004",
		"report_time": 1675388227,
		"business_day": 1675353600,
		"class_name": "早班",
		"employee_no": "vteam-1",
		"order_start_time": 1675388169,
		"order_end_time": 1675388189,
		"order_count": 1,
		"discount_total": 0,
		"promotion_total": 0,
		"coupon_total": 0,
		"tax_total": 2,
		"service_total": 0,
		"stock_push_amount": 0,
		"stock_pull_amount": 0,
		"sale_total": 45,
		"sale_amount": 45,
		"sale_item_count": 1,
		"sale_item_avg_cost": 45,
		"payment_cash_total": 45,
		"expense_cash_total": 60,
		"trans_reversal": 0,
		"over_paid": 0,
		"cash_total": 45,
		"cancel_count": 0,
		"cancel_total": 0,
		"other_cancel_count": 0,
		"other_cancel_total": 0,
		"refund_cash_total": 0,
		"payment_info": [
			{
				"payment_sid": 1,
				"payment_code": "CASH",
				"payment_name": "現金",
				"payment_amount": 45,
				"total_count": 1
			}
		],
		"coupon_info": [],
		"expense_info": [
			{
				"account_code": "PETTY",
				"account_name": "零用金收入",
				"account_type": "R",
				"payment_code": "CASH",
				"payment_name": "現金",
				"money": 100
			},
			{
				"account_code": "A",
				"account_name": "水電費用",
				"account_type": "E",
				"payment_code": "CASH",
				"payment_name": "現金",
				"money": 50
			},
			{
				"account_code": "B",
				"account_name": "進貨費用",
				"account_type": "E",
				"payment_code": "CASH",
				"payment_name": "現金",
				"money": 10
			},
			{
				"account_code": "Z",
				"account_name": "回收物變賣",
				"account_type": "R",
				"payment_code": "CASH",
				"payment_name": "現金",
				"money": 20
			}
		],
		"inv_summery_info": {
			"version": "1.6.5.4",
			"device_code": "",
			"business_id": "28537502",
			"branch_no": "001",
			"pos_no": "001",
			"total_upload_inv": 1,
			"total_upload_cancel_inv": 0,
			"sale_quantity": 1,
			"sale_amount": 45,
			"cancel_quantity": 0,
			"cancel_amount": 0,
			"details": [
				{
					"inv_type": 1,
					"period": "11202",
					"track": "LC",
					"begin_no": "10028510",
					"end_no": "10028510",
					"quantity": 1,
					"amount": 45
				}
			]
		},
		"report_type": "D",
		"company_sid": 7,
		"terminal_sid": "VT-POS-2020-00002"
	}
    */
    public class DRDetail
    {
        public int inv_type { get; set; }
        public string period { get; set; }
        public string track { get; set; }
        public string begin_no { get; set; }
        public string end_no { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
    }

    public class DRExpenseInfo
    {
        public string account_code { get; set; }
        public string account_name { get; set; }
        public string account_type { get; set; }
        public string payment_code { get; set; }
        public string payment_name { get; set; }
        public int money { get; set; }
    }

    public class DRInvSummeryInfo
    {
        public string report_no { get; set; }//電子發票用
        public long report_time { get; set; }//電子發票用
        public string pos_ver { get; set; }//version
        public string device_code { get; set; }
        public string business_id { get; set; }
        public string branch_no { get; set; }
        public string pos_no { get; set; }
        public int total_upload_inv { get; set; }
        public int total_upload_cancel_inv { get; set; }
        public int sale_quantity { get; set; }
        public int sale_amount { get; set; }
        public int cancel_quantity { get; set; }
        public int invalid_quantity { get; set; }//電子發票用
        public int cancel_amount { get; set; }
        public int invalid_amount { get; set; }//電子發票用

        public List<DRDetail> details { get; set; }
        public List<DRDetail> sale_details { get; set; }//報表列印用
        public List<DRDetail> cancel_details { get; set; }//報表列印用
        public DRInvSummeryInfo()
        {
            details = new List<DRDetail>();
            sale_details = new List<DRDetail>();
            cancel_details = new List<DRDetail>();
        }
    }

    public class DRPaymentInfo
    {
        public int payment_sid { get; set; }
        public string payment_code { get; set; }
        public string payment_name { get; set; }
        public int payment_amount { get; set; }
        public int total_count { get; set; }
    }

    public class DRCouponInfo
    {
        public string coupon_issuer { get; set; }
        public string coupon_name { get; set; }
        public int coupon_amount { get; set; }
        public int total_count { get; set; }
    }

    public class DRCategorySalesStatistics
    {
        public String category_code { get; set; }
        public String category_name { get; set; }
        public int sort { get; set; }
        public int discount_fee { get; set; }
        public int quantity { get; set; }
        public int subtotal { get; set; }
        public int amount { get; set; }
    }

    public class DRPromotions_Info
    {
        public string type { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
    }

    public class report_print_data //daily_report
    {
        public string store_name { get; set; }

        public string pos_ver { get; set; }//version
        public string device_code { get; set; }
        public string report_no { get; set; }
        public long report_time { get; set; }
        public string report_time_year { get; set; }
        public string report_time_month { get; set; }
        public string report_time_day { get; set; }
        public string report_time_hours { get; set; }
        public string report_time_minutes { get; set; }
        public long business_day { get; set; }
        public string business_day_year { get; set; }
        public string business_day_month { get; set; }
        public string business_day_day { get; set; }
        public string business_day_hours { get; set; }
        public string business_day_minutes { get; set; }
        public string class_name { get; set; }
        public string employee_no { get; set; }
        public long order_start_time { get; set; }
        public string order_start_time_year { get; set; }
        public string order_start_time_month { get; set; }
        public string order_start_time_day { get; set; }
        public string order_start_time_hours { get; set; }
        public string order_start_time_minutes { get; set; }
        public long order_end_time { get; set; }
        public string order_end_time_year { get; set; }
        public string order_end_time_month { get; set; }
        public string order_end_time_day { get; set; }
        public string order_end_time_hours { get; set; }
        public string order_end_time_minutes { get; set; }
        public int order_count { get; set; }
        public int discount_total { get; set; }
        public int promotion_total { get; set; }
        public int coupon_total { get; set; }
        public int tax_total { get; set; }
        public int service_total { get; set; }
        public int stock_push_amount { get; set; }
        public int stock_pull_amount { get; set; }
        public int sale_total { get; set; }
        public int sale_amount { get; set; }
        public int sale_item_count { get; set; }
        public int sale_item_avg_cost { get; set; }
        public int payment_cash_total { get; set; }
        public int expense_cash_total { get; set; }
        public int trans_reversal { get; set; }
        public int over_paid { get; set; }
        public int cash_total { get; set; }
        public int cancel_count { get; set; }
        public int cancel_total { get; set; }
        public int other_cancel_count { get; set; }
        public int other_cancel_total { get; set; }
        public int refund_cash_total { get; set; }
        public int delivery_total { get; set; }
        public int user_sid { get; set; }
        public decimal invoice_amount { get; set; }
        public decimal point_discount { get; set; }
        public decimal collection_payment { get; set; }

        public List<DRPaymentInfo> payment_info { get; set; }//支付方式資料結構
        public List<DRCouponInfo> coupon_info { get; set; }//優惠/兌換券資料結構
        public List<DRExpenseInfo> expense_info { get; set; }//收支紀錄資料結構
        public DRInvSummeryInfo inv_summery_info { get; set; }//發票資料結構
        public List<DRCategorySalesStatistics> category_sale_info { get; set; }//商品類別銷售統計
        public List<DRPromotions_Info> promotions_info { get; set; }//優惠資料統計的內容
        public string report_type { get; set; }
        public string report_type_name { get; set; }
        public int company_sid { get; set; }
        public string terminal_sid { get; set; }
        public decimal amount_actually_received { get; set; }//實收金額
        public report_print_data()
        {
            category_sale_info = new List<DRCategorySalesStatistics>();
            inv_summery_info = new DRInvSummeryInfo();
            expense_info = new List<DRExpenseInfo>();
            coupon_info = new List<DRCouponInfo>();
            payment_info = new List<DRPaymentInfo>();
            promotions_info = new List<DRPromotions_Info>();
        }

        public void SetVariable()
        {
            report_type_name = (report_type == "Ｃ") ? "交班報表" : "關帳報表";

            //---
            //補上時間變數字串資料 因為JSON只能存取變數
            DateTime DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(report_time);
            if (DateTimeBuf != null)
            {
                report_time_year = DateTimeBuf.ToString("yyyy");
                report_time_month = DateTimeBuf.ToString("MM");
                report_time_day = DateTimeBuf.ToString("dd");
                report_time_hours = DateTimeBuf.ToString("HH");
                report_time_minutes = DateTimeBuf.ToString("mm");
            }

            DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(order_start_time);
            if (DateTimeBuf != null)
            {
                order_start_time_year = DateTimeBuf.ToString("yyyy");
                order_start_time_month = DateTimeBuf.ToString("MM");
                order_start_time_day = DateTimeBuf.ToString("dd");
                order_start_time_hours = DateTimeBuf.ToString("HH");
                order_start_time_minutes = DateTimeBuf.ToString("mm");
            }

            DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(order_end_time);
            if (DateTimeBuf != null)
            {
                order_end_time_year = DateTimeBuf.ToString("yyyy");
                order_end_time_month = DateTimeBuf.ToString("MM");
                order_end_time_day = DateTimeBuf.ToString("dd");
                order_end_time_hours = DateTimeBuf.ToString("HH");
                order_end_time_minutes = DateTimeBuf.ToString("mm");
            }

            DateTimeBuf = TimeConvert.UnixTimeStampToDateTime(business_day);
            if (DateTimeBuf != null)
            {
                business_day_year = DateTimeBuf.ToString("yyyy");
                business_day_month = DateTimeBuf.ToString("MM");
                business_day_day = DateTimeBuf.ToString("dd");
                business_day_hours = DateTimeBuf.ToString("HH");
                business_day_minutes = DateTimeBuf.ToString("mm");
            }
            //---補上時間變數字串資料 因為JSON只能存取變數

            amount_actually_received = collection_payment + sale_amount;//實收金額

            if ((inv_summery_info.details != null) && (inv_summery_info.details.Count > 0))
            {
                if(inv_summery_info.sale_details==null)
                {
                    inv_summery_info.sale_details=new List<DRDetail>();
                }
                if (inv_summery_info.cancel_details == null)
                {
                    inv_summery_info.cancel_details = new List<DRDetail>();
                }
                for (int i=0;i< inv_summery_info.details.Count;i++)
                {
                    switch(inv_summery_info.details[i].inv_type)
                    {
                        case 1:
                            inv_summery_info.sale_details.Add(inv_summery_info.details[i]);
                            break;
                        case 2:
                            inv_summery_info.cancel_details.Add(inv_summery_info.details[i]);
                            break;
                    }
                }
        }   }
    }

}
