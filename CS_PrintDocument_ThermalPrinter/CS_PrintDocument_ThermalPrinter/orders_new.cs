﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace CS_PrintDocument_ThermalPrinter
{
    public class FormulaList
    {
        public string condiment_code { get; set; }//配料編號
        public string condiment_name { get; set; }//配料名
        public string operator_key { get; set; }//運算元+-*/=
        public string operator_value { get; set; }//運算值
        public object to_change_code { get; set; }
        public string unit_name { get; set; }//單位:cc/杯...
        public string decline_level { get; set; }//降階旗標
    }
    public class MaterialList//材料
    {
        public string material_code { get; set; }//材料編號
        public string material_name { get; set; }//材料名
        public string material_value { get; set; }//數量
        public string material_unit { get; set; }//單位
        public string print_bill { get; set; }//帳單列印N
        public string is_display { get; set; }//列印配方
        public List<FormulaList> formula_list { get; set; }

        public MaterialList()
        {
            formula_list = new List<FormulaList>();
        }

        public void Clone(MaterialList MaterialListBuf)
        {
            material_code = MaterialListBuf.material_code;//材料編號
            material_name = MaterialListBuf.material_name;//材料名
            material_value = MaterialListBuf.material_value;//數量
            material_unit = MaterialListBuf.material_unit;//單位
            print_bill = MaterialListBuf.print_bill;//帳單列印N
            is_display = MaterialListBuf.is_display;//列印配方

            formula_list.Clear();
            for (int i = 0; i < MaterialListBuf.formula_list.Count; i++)
            {
                FormulaList FormulaListB = MaterialListBuf.formula_list[i];
                formula_list.Add(FormulaListB);
            }
        }
    }

    public class SetMeal
    {
        public string att_name { get; set; }
        public List<Product> product { get; set; }
        public SetMeal()
        {
            att_name = "";
            product = new List<Product>();
        }
    }

    public class Product
    {
        public object item_no { get; set; }
        public object category_code { get; set; }
        public object category_name { get; set; }
        public string type { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public object price { get; set; }
        public object quantity { get; set; }
        public object amount { get; set; }
        public object subtotal { get; set; }
        public object del_flag { get; set; }
        public object state_memo { get; set; }
        public object update_user_sid { get; set; }
        public object update_user_name { get; set; }
        public List<Condiment> condiments { get; set; }

        public Product()
        {
            condiments = new List<Condiment>();
        }
    }
    public class Condiment
    {
        public string condiment_code { get; set; }
        public string condiment_name { get; set; }
        public int price { get; set; }
        public int count { get; set; }
        public int quantity { get; set; }
        public int subtotal { get; set; }
        public int amount { get; set; }
    }
    public class DiscountInfo
    {
        public string external_id { get; set; }
        public string external_mode { get; set; }
        public int product_sid { get; set; }
        public string product_code { get; set; }
        public int subtotal { get; set; }
        public string showname { get; set; }
        public string hotkey_code { get; set; }
        public string hotkey_name { get; set; }
        public string discount_type { get; set; }
        public string discount_code { get; set; }
        public string discount_name { get; set; }
        public string val_mode { get; set; }
        public int val { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
    }

    public class OrderItem
    {
        public int item_no { get; set; }
        public string category_code { get; set; }
        public string category_name { get; set; }
        public string item_type { get; set; }
        public string product_type { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public int price { get; set; }
        public int count { get; set; }
        public int quantity { get; set; }
        public int subtotal { get; set; }
        public string discount_code { get; set; }
        public string discount_name { get; set; }
        public string discount_type { get; set; }
        public int discount_rate { get; set; }
        public int discount_fee { get; set; }
        public DiscountInfo discount_info { get; set; }
        public string external_id { get; set; }
        public string external_mode { get; set; }
        public int stock_remainder_quantity { get; set; }
        public int stock_push_price { get; set; }
        public int stock_push_quantity { get; set; }
        public int stock_push_amount { get; set; }
        public string stock_pull_code { get; set; }
        public string stock_pull_name { get; set; }
        public int stock_pull_price { get; set; }
        public int stock_pull_quantity { get; set; }
        public int stock_pull_amount { get; set; }
        public string tax_type { get; set; }
        public int tax_rate { get; set; }
        public int tax_fee { get; set; }
        public int amount { get; set; }
        public string customer_name { get; set; }
        public string print_flag { get; set; }
        public List<SetMeal> set_meals { get; set; }
        public List<Condiment> condiments { get; set; }
        public string QrCodeBlankingMachine { get; set; }//為了要在標籤印出對應QrCode圖片，所準備資料變數
        public List<Printer> printers { get; set; }
        public List<MaterialList> material_list { get; set; }//材料列表
        public string CompareString { get; set; }//產品名稱;配料名稱01;配料名稱02;.....
        public OrderItem()
        {
            set_meals = new List<SetMeal>();
            condiments = new List<Condiment>();
            printers = new List<Printer>();
            material_list = new List<MaterialList>();
        }

    }

    public class Package
    {
        public string package_code { get; set; }
        public string package_name { get; set; }
        public int price { get; set; }
        public int count { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
    }

    public class J2C_Payment
    {
        public int payment_sid { get; set; }
        public string payment_code { get; set; }
        public string payment_name { get; set; }
        public string payment_module_code { get; set; }
        public int coin_discount { get; set; }
        public int coupon_discount { get; set; }
        public int points_discount { get; set; }
        public int payment_amount { get; set; }
        public int received_fee { get; set; }
        public int change_fee { get; set; }
        public int payment_time { get; set; }
        public string payment_info { get; set; }
    }

    public class J2C_Coupon
    {
        public string external_id { get; set; }
        public string issuer { get; set; }
        public string mode { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public int val { get; set; }
        public int amount { get; set; }
        public string original { get; set; }
    }

    public class Tableware
    {
        public string code { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
    }

    public class Printer
    {
        public int printer_group_sid { get; set; }
        public int printer_order_type { get; set; }
        public int product_sid { get; set; }
        public string product_code { get; set; }
    }

    public class Invoice_Data
    {
        public string inv_period { get; set; }
        public string inv_no { get; set; }
        public string cust_ein { get; set; }
        public string donate_flag { get; set; }
        public string donate_code { get; set; }
        public string carrier_type { get; set; }
        public string carrier_code_1 { get; set; }
        public string carrier_code_2 { get; set; }
        public int batch_num { get; set; }
        public string random_code { get; set; }
        public string qrcode_aes_key { get; set; }
    }

    public class OrderPrintData//訂單 上傳/列印 資料結構
    {
        public string store_name { get; set; }
        public string pos_ver { get; set; }//version
        public string device_code { get; set; }
        public string order_no { get; set; }
        public string order_no_from { get; set; }
        public int order_time { get; set; }
        public string order_time_year { get; set; }
        public string order_time_month { get; set; }
        public string order_time_day { get; set; }
        public string order_time_hours { get; set; }
        public string order_time_minutes { get; set; }
        public int order_open_time { get; set; }
        public int order_state { get; set; }
        public string order_type { get; set; }
        public string order_type_name { get; set; }
        public string order_type_code { get; set; }
        public string terminal_sid { get; set; }
        public string pos_no { get; set; }
        public string class_name { get; set; }
        public string employee_no { get; set; }
        public string table_code { get; set; }
        public string table_name { get; set; }
        public string call_num { get; set; }
        public string meal_num { get; set; }
        public string member_flag { get; set; }
        public string member_no { get; set; }
        public string member_platform { get; set; }
        public string member_name { get; set; }
        public string member_phone { get; set; }
        public string outside_order_no { get; set; }
        public string outside_description { get; set; }
        public string takeaways_order_sid { get; set; }
        public string delivery_city_name { get; set; }
        public string delivery_district_name { get; set; }
        public string delivery_address { get; set; }
        public int item_count { get; set; }
        public int subtotal { get; set; }
        public int discount_fee { get; set; }
        public int promotion_fee { get; set; }
        public int coupon_discount { get; set; }
        public int stock_push_quantity { get; set; }
        public int stock_push_amount { get; set; }
        public int stock_pull_quantity { get; set; }
        public int stock_pull_amount { get; set; }
        public int service_fee { get; set; }
        public int service_rate { get; set; }
        public int trans_reversal { get; set; }
        public int over_paid { get; set; }
        public int tax_fee { get; set; }
        public int amount { get; set; }
        public string paid_flag { get; set; }
        public int cash_fee { get; set; }
        public int change_fee { get; set; }
        public string cust_ein { get; set; }
        public string invoice_flag { get; set; }
        public int business_day { get; set; }
        public string cancel_flag { get; set; }
        public int cancel_time { get; set; }
        public string cancel_class_name { get; set; }
        public string del_flag { get; set; }
        public int refund { get; set; }
        public string refund_type_sid { get; set; }
        public string remarks { get; set; }
        public double invoice_amount { get; set; }
        public double point_value { get; set; }
        public double point_discount { get; set; }
        public List<OrderItem> order_items { get; set; }
        public int product_sale_countCS0102 { get; set; }//product_sale_count
        public List<Package> packages { get; set; }
        public int set_product_sale_count { get; set; }
        public int package_sale_count { get; set; }
        public List<J2C_Payment> payments { get; set; }
        public List<J2C_Coupon> coupons { get; set; }
        public int company_sid { get; set; }
        public string upload_terminal_sid { get; set; }
        public string upload_ip_address { get; set; }
        public string license_type { get; set; }
        public Invoice_Data invoice_data { get; set; }
        public string strQrcodeInfor { get; set; }
        public string PrintInvReceipt { get; set; }//列印發票交易明細旗標  (print_config.print_inv_receipt=="Y") OR (cust_ein!="") =>Y/N

        public List<Tableware> tablewares { get; set; }

        public OrderPrintData()
        {
            order_items = new List<OrderItem>();
            packages = new List<Package>();
            payments = new List<J2C_Payment>();
            coupons = new List<J2C_Coupon>();
            invoice_data = new Invoice_Data();
            tablewares = new List<Tableware>();

            strQrcodeInfor = "";
            PrintInvReceipt = "N";//列印發票交易明細旗標  (print_config.print_inv_receipt=="Y") OR (cust_ein!="") =>Y/N
        }
        public OrderPrintData order_itemsDeepClone(int index)//深層複製
        {
            OrderPrintData OrderPrintDataBuf = (OrderPrintData)this.MemberwiseClone();//表層屬性複製
            OrderPrintDataBuf.order_items = new List<OrderItem>();
            OrderPrintDataBuf.order_items.Add(this.order_items[index]);
            return OrderPrintDataBuf;
        }

        public void mergeItems(bool blnSort = true)
        {
            List<OrderItem> orderItemBuf = new List<OrderItem>();
            orderItemBuf.Clear();
            if (order_items.Count > 0)
            {
                OrderItem orderItem = order_items[0];
                orderItemBuf.Add(orderItem);
            }

            for (int i = 1; i < order_items.Count; i++)
            {
                bool blnfind = false;
                OrderItem orderItemM = order_items[i];

                for (int j = 0; j < orderItemBuf.Count; j++)
                {
                    if (orderItemM.CompareString == orderItemBuf[j].CompareString)
                    {
                        orderItemBuf[j].count += orderItemM.count;
                        orderItemBuf[j].quantity += orderItemM.count;
                        blnfind = true;
                        break;
                    }
                }

                if (!blnfind)//未找到
                {
                    orderItemBuf.Add(orderItemM);
                }
            }

            if (blnSort)
            {
                orderItemBuf.Sort((x, y) => {
                    //int ret = (x.item_no> y.item_no)? x.item_no: y.item_no;
                    int ret = String.Compare(x.product_name, y.product_name);
                    return ret;
                });
            }

            order_items = orderItemBuf;
        }

    }
    public class OrderPrintDataResult
    {
        public string status { get; set; }
        public string message { get; set; }
    }

}
