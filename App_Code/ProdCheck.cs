using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
   外驗查檢表
 */
namespace ProdCheckData.Models
{
    /// <summary>
    /// ERP採購單資料
    /// </summary>
    public class ERP_PurData
    {
        /// <summary>
        /// 單別
        /// </summary>
        public string FirstID { get; set; }

        /// <summary>
        /// 單號
        /// </summary>
        public string SecondID { get; set; }

        /// <summary>
        /// 採購日
        /// </summary>
        public string BuyDate { get; set; }

        /// <summary>
        /// 廠商編號
        /// </summary>
        public string CustID { get; set; }

        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string CustName { get; set; }

        /// <summary>
        /// 品號
        /// </summary>
        public string ModelNo { get; set; }

        /// <summary>
        /// 採購數量
        /// </summary>
        public Decimal BuyQty { get; set; }

        /// <summary>
        /// 是否已新增過
        /// </summary>
        public int RelCnt { get; set; }
    }


    public class ProdCheck
    {
        public int SeqNo { get; set; }
        public Guid Data_ID { get; set; }
        public int Corp_UID { get; set; }
        public string Corp_Name { get; set; }
        public string FirstID { get; set; }
        public string SecondID { get; set; }
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public string ShipFrom { get; set; }
        public string QC_Category { get; set; }
        public string Vendor { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string Est_CheckDay { get; set; }
        public string Act_CheckDay { get; set; }
        public string IsFinished { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string IsRel { get; set; }
        public string IsReported { get; set; }
        public string IsLock { get; set; }

        public string SubNo_TW { get; set; }
        public string SubNo_SH { get; set; }
        public string SubNo_SZ { get; set; }
        public string ProdNotes { get; set; }

        public string Remark { get; set; }
        public string Create_Time { get; set; }
        public string Update_Time { get; set; }
        public string Mail_Time { get; set; }
        public string Approved_Time { get; set; }
        public string Create_Who { get; set; }
        public string Update_Who { get; set; }
        public string Mail_Who { get; set; }
        public string Approved_Who { get; set; }
        public string Create_Name { get; set; }
        public string Update_Name { get; set; }
        public string Mail_Name { get; set; }
        public string Approved_Name { get; set; }

    }


    /// <summary>
    /// 類別欄位
    /// </summary>
    public class CheckClass
    {
        public int ID { get; set; }
        public string Label { get; set; }
    }


    /// <summary>
    /// 人員欄位
    /// </summary>
    public class UpdateWho
    {
        public string ID { get; set; }
        public string Label { get; set; }
    }


    /// <summary>
    /// 關聯欄位
    /// </summary>
    public class RelData
    {
        public string DataID { get; set; }
        public string FirstID { get; set; }
        public string SecondID { get; set; }
    }


    /// <summary>
    /// 檢驗項目
    /// </summary>
    public class CheckItems
    {
        public string chkType { get; set; }
        public string Lv { get; set; }
        public string Spec { get; set; }
        public string Remark { get; set; }
    }


    /// <summary>
    /// 檢驗報告
    /// </summary>
    public class CheckFiles
    {
        public Guid Data_ID { get; set; }
        public int AttachID { get; set; }
        public byte AttachType { get; set; }
        public string AttachFile { get; set; }
        public string AttachFile_Name { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
    }


    public class ProdItems
    {
        public string ModelNo { get; set; }
        public string ModelName { get; set; }
        public int AttachCnt { get; set; }
    }


    /// <summary>
    /// 品號附件
    /// </summary>
    public class ProdItemsFiles
    {
        public string ModelNo { get; set; }
        public int AttachID { get; set; }
        public string AttachFile { get; set; }
        public string AttachFile_Name { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
    }
}
