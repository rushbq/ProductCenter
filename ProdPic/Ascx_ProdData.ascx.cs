using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// 共用控制項 - 基本資料顯示
/// </summary>
public partial class Ascx_ProdData : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //讓網頁不暫存
        Response.ExpiresAbsolute = DateTime.Now;
        Response.Expires = -1441;
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.AddHeader("Pragma", "no-store");
        Response.AddHeader("cache-control", "no-cache");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoServerCaching();

        //帶出基本資料
        GetData();
    }

    /// <summary>
    /// 取得基本資料
    /// </summary>
    private void GetData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg = "";
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT  ");
                SBSql.AppendLine("   (SELECT COUNT(*) FROM ProdPic_Group WHERE (ProdPic_Group.Model_No = Prod_Item.Model_No) ");
                SBSql.AppendLine("     AND (ProdPic_Group.Pic_Class = @Param_C_ID) ");
                SBSql.AppendLine("   ) AS PicCnt ");
                SBSql.AppendLine("   , Model_No, Class_ID, Ship_From, Catelog_Vol, Page, BarCode, Date_Of_Listing, Stop_Offer_Date ");
                SBSql.AppendLine("   , Model_Name_zh_TW, Model_Name_en_US ");
                SBSql.AppendLine("   , Pub_Card_Model_No, Pub_Accessories ");
                //替代品號 & 個案失效日
                SBSql.AppendLine("   , Substitute_Model_No_TW, Cases_Of_Failure_Date_TW");
                SBSql.AppendLine("   , Substitute_Model_No_SH, Cases_Of_Failure_Date_SH");
                SBSql.AppendLine("   , Substitute_Model_No_SZ, Cases_Of_Failure_Date_SZ");
                //子件型號
                SBSql.AppendLine("   , (SELECT TOP 1 Part_No FROM Prod_Rel_PartNo WHERE (Prod_Rel_PartNo.Model_No = Prod_Item.Model_No)) AS Part_No");
                SBSql.AppendLine(" FROM Prod_Item ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_C_ID", Param_C_ID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        string js = "alert('查無資料！');location.href=\'" + Session["BackListUrl"] + "\';";
                        ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
                        return;
                    }
                    else
                    {
                        //填入基本資料
                        Param_ShipFrom = DT.Rows[0]["Ship_From"].ToString();
                        Param_Parts = DT.Rows[0]["Part_No"].ToString();
                        Param_Vol = DT.Rows[0]["Catelog_Vol"].ToString();
                        Param_Page = DT.Rows[0]["Page"].ToString();
                        Param_onLine = DT.Rows[0]["Date_Of_Listing"].ToString();
                        Param_offLine = DT.Rows[0]["Stop_Offer_Date"].ToString();
                        Param_BarCode = DT.Rows[0]["BarCode"].ToString();
                        Param_Name_zhTW = DT.Rows[0]["Model_Name_zh_TW"].ToString();
                        Param_Name_enUS = DT.Rows[0]["Model_Name_en_US"].ToString();
                        Param_PicCnt = Convert.ToInt32(DT.Rows[0]["PicCnt"]);
                        Param_Pub_Card_Model_No = DT.Rows[0]["Pub_Card_Model_No"].ToString();
                        Param_Pub_Accessories = fn_Desc.Prod.Accessories(DT.Rows[0]["Pub_Accessories"].ToString());
                        //替代品號
                        Hashtable ht = new Hashtable();
                        ht.Add("TW", DT.Rows[0]["Substitute_Model_No_TW"].ToString());
                        ht.Add("SH", DT.Rows[0]["Substitute_Model_No_SH"].ToString());
                        ht.Add("SZ", DT.Rows[0]["Substitute_Model_No_SZ"].ToString());
                        Param_Substitute_Model_No = showDetail(ht);
                        //個案失效日
                        ht.Clear();
                        ht.Add("TW", DT.Rows[0]["Cases_Of_Failure_Date_TW"].ToString());
                        ht.Add("SH", DT.Rows[0]["Cases_Of_Failure_Date_SH"].ToString());
                        ht.Add("SZ", DT.Rows[0]["Cases_Of_Failure_Date_SZ"].ToString());
                        Param_Cases_Of_Failure_Date = showDetail(ht);

                        //帶出認證資料
                        LookupCertList();
                    }
                }
            }
        }
        catch (Exception)
        {
            string js = "alert('系統發生錯誤 - 讀取資料！');";
            ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
            return;
        }

    }

    /// <summary>
    /// 顯示明細 (替代品號 & 個案失效日)
    /// </summary>
    /// <param name="inputValue">輸入值</param>
    /// <returns></returns>
    private string showDetail(Hashtable inputValue)
    {
        StringBuilder sb = new StringBuilder();

        foreach (DictionaryEntry de in inputValue)
        {
            if (false == string.IsNullOrEmpty(de.Value.ToString()))
            {
                sb.AppendLine("<div style=\"padding-bottom: 4px\">");
                sb.AppendLine(string.Format(" <span class=\"styleGraylight\">({0})</span>&nbsp;{1}"
                    , de.Key
                    , de.Value));
                sb.AppendLine("</div>");
            }
        }

        return sb.ToString();
    }

    #region -- 認證資料取得 --
    /// <summary>
    /// 副程式 - 取得認證資料列表
    /// </summary>
    private void LookupCertList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg = "";

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT CertDtl.* ");
                SBSql.AppendLine("  FROM Prod_Certification Cert ");
                SBSql.AppendLine("   INNER JOIN Prod_Certification_Detail CertDtl ON Cert.Cert_ID = CertDtl.Cert_ID ");
                SBSql.AppendLine(" WHERE (Cert.Model_No= @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY CertDtl.Cert_Type ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvCertList.DataSource = DT.DefaultView;
                    this.lvCertList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 認證資料！", "");
        }
    }

    protected void lvCertList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //證書類別
            Literal lt_CertType = (Literal)e.Item.FindControl("lt_CertType");
            lt_CertType.Text = DataBinder.Eval(dataItem.DataItem, "Cert_Type").ToString();
            if (false == string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString()))
            {
                lt_CertType.Text += " - " + DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString();
            }

            //圖片顯示 - 認證符號
            Literal lt_Icon = ((Literal)e.Item.FindControl("lt_Icon"));
            //判斷是否為CE
            if (DataBinder.Eval(dataItem.DataItem, "IsCE").ToString() == "Y")
            {
                lt_Icon.Text = "<img src=\"" + Application["File_WebUrl"] + "icon_cert/108.jpg\" width=\"35\" />";
            }
            else
            {
                lt_Icon.Text = fn_Extensions.GetCertIcon(
                    DataBinder.Eval(dataItem.DataItem, "Cert_ID").ToString()
                    , DataBinder.Eval(dataItem.DataItem, "Detail_ID").ToString());
            }

            #region --檔案處理--
            //檔案顯示 - 證書
            Literal lt_CertFile = ((Literal)e.Item.FindControl("lt_CertFile"));
            lt_CertFile.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File").ToString());

            //檔案顯示 - TestReport
            Literal lt_FileTestReport = ((Literal)e.Item.FindControl("lt_FileTestReport"));
            lt_FileTestReport.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Report").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File_Report").ToString());

            //檔案顯示 - 自我宣告
            Literal lt_FileCE = ((Literal)e.Item.FindControl("lt_FileCE"));
            lt_FileCE.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE").ToString());

            Literal lt_FileCE_enUS = ((Literal)e.Item.FindControl("lt_FileCE_enUS"));
            lt_FileCE_enUS.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_en_US").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_en_US").ToString());

            Literal lt_FileCE_zhCN = ((Literal)e.Item.FindControl("lt_FileCE_zhCN"));
            lt_FileCE_zhCN.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_zh_CN").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_zh_CN").ToString());

            //檔案顯示 - 自我檢測
            Literal lt_FileCheck = ((Literal)e.Item.FindControl("lt_FileCheck"));
            lt_FileCheck.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check").ToString());

            Literal lt_FileCheck_enUS = ((Literal)e.Item.FindControl("lt_FileCheck_enUS"));
            lt_FileCheck_enUS.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_en_US").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_en_US").ToString());

            Literal lt_FileCheck_zhCN = ((Literal)e.Item.FindControl("lt_FileCheck_zhCN"));
            lt_FileCheck_zhCN.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_zh_CN").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_zh_CN").ToString());
            #endregion
        }
    }

    /// <summary>
    /// 設定檔案下載連結
    /// </summary>
    /// <param name="orgName">原始檔名</param>
    /// <param name="fileName">真實檔名</param>
    /// <returns>string</returns>
    private string setFileUri(string orgName, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }
        else
        {
            string uri = string.Format("../FileDownload.ashx?OrgiName={0}&FilePath={1}"
                , Server.UrlEncode(orgName.Trim())
                , Server.UrlEncode(Cryptograph.Encrypt(Cert_DiskFolder + fileName)));

            return "<a href=\"" + uri + "\">下載</a>";
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 認證Disk資料夾路徑
    /// </summary>
    private string _Cert_DiskFolder;
    public string Cert_DiskFolder
    {
        get
        {
            return Application["File_DiskUrl"] + string.Format(@"Certification\{0}\", Param_ModelNo);
        }
        set
        {
            this._Cert_DiskFolder = value;
        }
    }

    //[參數] - 品號
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get;
        set;
    }

    //[參數] - 主要出貨地
    private string _Param_ShipFrom;
    public string Param_ShipFrom
    {
        get;
        set;
    }

    //[參數] - 子件品號
    private string _Param_Parts;
    public string Param_Parts
    {
        get;
        set;
    }

    //[參數] - 目錄
    private string _Param_Vol;
    public string Param_Vol
    {
        get;
        set;
    }

    //[參數] - 頁次
    private string _Param_Page;
    public string Param_Page
    {
        get;
        set;
    }

    //[參數] - 上市日期
    private string _Param_onLine;
    public string Param_onLine
    {
        get;
        set;
    }

    //[參數] - 停售日期
    private string _Param_offLine;
    public string Param_offLine
    {
        get;
        set;
    }

    //[參數] - 條碼
    private string _Param_BarCode;
    public string Param_BarCode
    {
        get;
        set;
    }

    //[參數] - 中文品名
    private string _Param_Name_zhTW;
    public string Param_Name_zhTW
    {
        get;
        set;
    }

    //[參數] - 英文品名
    private string _Param_Name_enUS;
    public string Param_Name_enUS
    {
        get;
        set;
    }

    //[參數] - 圖片類別
    private string _Param_C_ID;
    public string Param_C_ID
    {
        get;
        set;
    }

    //[參數] - 圖片數
    private int _Param_PicCnt;
    public int Param_PicCnt
    {
        get;
        set;
    }

    //[參數] - 卡片品號
    private string _Param_Pub_Card_Model_No;
    public string Param_Pub_Card_Model_No
    {
        get;
        set;
    }

    //[參數] - 替代品號
    private string _Param_Substitute_Model_No;
    public string Param_Substitute_Model_No
    {
        get;
        set;
    }

    //[參數] - 個案失效日
    private string _Param_Cases_Of_Failure_Date;
    public string Param_Cases_Of_Failure_Date
    {
        get;
        set;
    }

    //[參數] - 主/配件
    private string _Param_Pub_Accessories;
    public string Param_Pub_Accessories
    {
        get;
        set;
    }

    #endregion
}