using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatalogMenuData.Models
{
    public class MenuItem
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public int Parent_ID { get; set; }
        public int Menu_Level { get; set; }
        public int Class_ID { get; set; }
    }
}
