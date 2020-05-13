using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProdNewsData.Models
{
    /// <summary>
    /// 產品訊息
    /// </summary>
    public class Items
    {
        public Int32 NewsID { get; set; }
        public Int32 ClassID { get; set; }
        public string ClassName { get; set; }
        public string Lang { get; set; }
        public string Subject { get; set; }
        public Int16? TimingType { get; set; }
        public string TimingDate { get; set; }
        public string TimingDesc { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string IsMail { get; set; }
        public string IsClose { get; set; }
        public string BPM_Sno { get; set; }
        public string BPM_Oid { get; set; }
        public string BPM_WorkItemID { get; set; }
        public string BPM_FormNo { get; set; }

        public IEnumerable<string> SendTarget { get; set; }

        public string Create_Who { get; set; }
        public string Create_Name { get; set; }
        public string Create_Time { get; set; }
        public string Update_Who { get; set; }
        public string Update_Name { get; set; }
        public string Update_Time { get; set; }
        public string Send_Who { get; set; }
        public string Send_Name { get; set; }
        public string Send_Time { get; set; }
      
    }


    /// <summary>
    /// 附件
    /// </summary>
    public class AttachFiles
    {
        public Int32 NewsID { get; set; }
        public Int32 AttID { get; set; }
        public Int16 AttType { get; set; }
        public string AttachFile_Name { get; set; }
        public string AttachFile { get; set; }
        public string AttDesc { get; set; }
        public string Create_Who { get; set; }
        public string Create_Time { get; set; }
    }


    /// <summary>
    /// 品號
    /// </summary>
    public class RelModelNo
    {
        public Int32 NewsID { get; set; }
        public string Model_No { get; set; }
    }

    /// <summary>
    /// 發送對象
    /// </summary>
    public class RelTarget
    {
        public Int32 NewsID { get; set; }
        public string TargetID { get; set; }
    }
}
