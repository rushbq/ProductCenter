using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcLifeData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Product
    {
        public string ModelNo { get; set; }
        public string HouseNo { get; set; }
        public string HouseName { get; set; }
        public string LargeNo { get; set; }
        public string LargeName { get; set; }
        public string MediumNo { get; set; }
        public string MediumName { get; set; }
        public string ProductName { get; set; }
        public string SPName { get; set; }
        public string MType { get; set; }
        public int MaxBuy { get; set; }
        public int MaxBuyTot { get; set; }
        public string ProductMemo { get; set; }
        public Decimal Price_Cost { get; set; }
        public Decimal Price_Sale { get; set; }
        public Decimal Price_Spical { get; set; }
        public string Desc_Classics { get; set; }
        public string Desc_Feature { get; set; }
        public string Desc_Standards { get; set; }
        public string Desc_Introduce { get; set; }
        public string Desc_Services { get; set; }
        public string PicUrl { get; set; }

        public string Create_Who { get; set; }
        public string Create_Name { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Name { get; set; }
        public string Update_Time { get; set; }

        public string SyncStatus { get; set; }
        public string IsSync { get; set; }

        public string BarCode { get; set; }
    }

    /// <summary>
    /// 關鍵字欄位
    /// </summary>
    public class Tags
    {
        public int TagID { get; set; }
        public string TagName { get; set; }
        public string ModelNo { get; set; }
    }

    /// <summary>
    /// Log欄位
    /// </summary>
    public class Log
    {
        public string LogID { get; set; }
        public string ModelNo { get; set; }
        public Int16 LogType { get; set; }
        public string LogName { get; set; }
        public string LogValue { get; set; }
        public string LogWho { get; set; }
        public string LogTime { get; set; }
    }

}
