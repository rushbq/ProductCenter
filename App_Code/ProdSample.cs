using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProdSampleData.Models
{
    /// <summary>
    /// 資料欄位
    /// </summary>
    public class ProdSample
    {
        #region -- 資料庫欄位 --

        public int SeqNo { get; set; }
        public Guid SP_ID { get; set; }
        public string SerialNo { get; set; }
        public string Company { get; set; }
        public string Assign_Who { get; set; }
        public string Model_No { get; set; }
        public int? Sup_Corp { get; set; }
        public string Sup_ErpID { get; set; }
        public string Cust_ModelNo { get; set; }
        public string Cust_Newguy { get; set; }
        public int? Qty { get; set; }
        public int? Cls_Source { get; set; }
        public int? Cls_Check { get; set; }
        public int? Cls_Status { get; set; }
        public string Date_Come { get; set; }
        public string Date_Est { get; set; }
        public string Date_Actual { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string Remark { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Time { get; set; }

        #endregion


        #region -- 關聯欄位 --

        public string Company_Name { get; set; }
        public string Assign_Name { get; set; }
        public string Source_Name { get; set; }
        public string Check_Name { get; set; }
        public string Status_Name { get; set; }
        public string Cust_Name { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }

        #endregion

    }


    /// <summary>
    /// 類別欄位
    /// </summary>
    public class SampleClass
    {
        public int ID { get; set; }
        public string Label { get; set; }
    }

    public class SampleFiles
    {
        public int AttachID { get; set; }
        public Guid SP_ID { get; set; }
        public string AttachFile { get; set; }
        public string AttachFile_Name { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
    }

    public class RelModelNo
    {
        public Guid SP_ID { get; set; }
        public string Model_No { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
    }

    public class RelSampleID
    {
        public Guid SP_ID { get; set; }
        public Guid Rel_ID { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
    }
}
