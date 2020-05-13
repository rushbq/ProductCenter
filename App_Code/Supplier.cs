using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupplierData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class Supplier
    {
        public string SetYear { get; set; }
        public string SupID { get; set; }
        public string SupName { get; set; }
        public string Company { get; set; }
        public string Currency { get; set; }
        public Decimal? LastPrice { get; set; }
        public string ValidDate { get; set; }
    }

    public class Supplier_TradeLog
    {
        public string ModelNo { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string FirstTradeDate { get; set; }
        public string SupModelNo { get; set; }
        public string CheckDate { get; set; }
        public string LastInvDate { get; set; }
        public string IsSpQty { get; set; }
        public Decimal? BuyPrice { get; set; }
        public string Remark { get; set; }
        public string IsTax { get; set; }
        public string ValidDate { get; set; }
        public string InValidDate { get; set; }
    }

    public class Supplier_SpQtyInfo
    {
        public string Currency { get; set; }
        public int spQty { get; set; }
        public Decimal? spPrice { get; set; }
    }
}
