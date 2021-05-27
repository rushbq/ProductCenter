using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProdAttributeData.Models
{
    /// <summary>
    /// 貨號屬性
    /// </summary>
    public class ItemProp
    {
        public string Item_No { get; set; }
        public string OnlineDate { get; set; }
        public string StopDate { get; set; }
        public string ShipFrom { get; set; }
        public string NewProdClass { get; set; }
        public string SaleClass { get; set; }
        public string PurClass { get; set; }
        public string WareHouseClass { get; set; }
        public string CCCode { get; set; }
      
    }


}
