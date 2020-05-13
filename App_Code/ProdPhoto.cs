using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
   產品圖片
 */
namespace ProdPhotoData.Models
{
    public class PhotoItem
    {
        public string ColID { get; set; }
        public string ColName { get; set; }
        public string ColValue { get; set; }
    }

    public class PhotoClass
    {
        public int ID { get; set; }
        public string Label { get; set; }
    }
}
