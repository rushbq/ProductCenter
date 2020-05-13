using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using PKLib_Method.Methods;

public partial class Certification_Cert_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //[權限判斷] Start
            #region --權限--

            if (fn_CheckAuth.CheckAuth_User("202", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            #endregion
            //[權限判斷] End


            if (!IsPostBack)
            {
                //載入資料
                LookupData();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示:基本資料 --

    /// <summary>
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        StringBuilder sql = new StringBuilder(); //SQL語法容器

        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                sql.AppendLine(" SELECT Certi.Cert_ID, Certi.Model_No, Certi.Doc_Path, Certi.Supplier_ItemNo ");
                sql.AppendLine("  , Certi.Self_Cert, Certi.Supplier, Certi.Remark");
                sql.AppendLine("  , Prod_Item.Ship_From, ISNULL(Prod_Item.Catelog_Vol, '') AS Vol, ISNULL(Prod_Item.Page, '') AS Page");
                sql.AppendLine("  , Prod_Item.Model_Name_zh_TW AS ModelName, Prod_Item.Class_ID");
                sql.AppendLine("  , (");
                sql.AppendLine("   SELECT Prod_Class.Class_Name_zh_TW");
                sql.AppendLine("   FROM Prod_Class");
                sql.AppendLine("   WHERE Prod_Class.Class_ID = Prod_Item.Class_ID");
                sql.AppendLine("  ) AS Class_Name");
                sql.AppendLine("  , Certi.Create_Time, Certi.Update_Time");
                sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Certi.Create_Who)) AS Create_Name ");
                sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Certi.Update_Who)) AS Update_Name ");
                sql.AppendLine(" FROM Prod_Certification Certi INNER JOIN Prod_Item ON Certi.Model_No = Prod_Item.Model_No ");
                sql.AppendLine(" WHERE (Certi.Cert_ID = @ID) ");

                cmd.CommandText = sql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ID", Req_DataID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        CustomExtension.AlertMsg("無法取得資料,即將返回列表頁.", Page_SearchUrl);
                        return;
                    }

                    #region >> 欄位填寫 <<

                    //--- 填入基本資料 ---
                    lt_SeqNo.Text = DT.Rows[0]["Cert_ID"].ToString();

                    lt_ModelNo.Text = DT.Rows[0]["Model_No"].ToString().Trim();
                    lt_Class.Text = "{0} - {1}".FormatThis(DT.Rows[0]["Class_ID"].ToString(), DT.Rows[0]["Class_Name"].ToString());
                    lt_ShipFrom.Text = DT.Rows[0]["Ship_From"].ToString().Trim();
                    lb_Vol.Text = DT.Rows[0]["Vol"].ToString();
                    lb_Page.Text = DT.Rows[0]["Page"].ToString();
                    lt_Doc_Path.Text = DT.Rows[0]["Doc_Path"].ToString();
                    lt_Supplier.Text = DT.Rows[0]["Supplier"].ToString();
                    lt_Supplier_ItemNo.Text = DT.Rows[0]["Supplier_ItemNo"].ToString().Trim();
                    lt_Self_Cert.Text = DT.Rows[0]["Self_Cert"].ToString();
                    lt_Remark.Text = DT.Rows[0]["Remark"].ToString().Replace("\r\n", "<BR/>");

                    ph_Details.Visible = true;

                    #endregion


                    #region >> 其他功能 <<

                    //-- 載入其他資料 --
                    LookupData_Detail();

                    #endregion


                    //維護資訊
                    info_Creater.Text = DT.Rows[0]["Create_Name"].ToString();
                    info_CreateTime.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                    info_Updater.Text = DT.Rows[0]["Update_Name"].ToString();
                    info_UpdateTime.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                }
            }



        }
        catch (Exception)
        {

            throw;
        }

    }

    #endregion


    #region -- 資料顯示:明細資料 --

    private void LookupData_Detail()
    {
        //----- 宣告:資料參數 -----
        StringBuilder sql = new StringBuilder(); //SQL語法容器

        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {

                sql.AppendLine(" SELECT * ");
                sql.AppendLine("  FROM Prod_Certification_Detail");
                sql.AppendLine(" WHERE (Cert_ID = @ID)");
                sql.AppendLine(" ORDER BY Cert_ApproveDate DESC, Cert_Type");

                cmd.CommandText = sql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ID", Req_DataID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    lv_Detail.DataSource = DT;
                    lv_Detail.DataBind();

                }
            }

        }
        catch (Exception)
        {

            throw;
        }

    }

    protected void lv_Detail_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;
                string modelNo = lt_ModelNo.Text;

                //證書類別
                Literal lt_CertType = (Literal)e.Item.FindControl("lt_CertType");
                lt_CertType.Text = DataBinder.Eval(dataItem.DataItem, "Cert_Type").ToString();
                if (false == string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString()))
                {
                    lt_CertType.Text += " - " + DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString();
                }

                //圖片顯示 - 認證符號
                Literal lt_Icon = (Literal)e.Item.FindControl("lt_Icon");
                //判斷是否為CE
                if (DataBinder.Eval(dataItem.DataItem, "IsCE").ToString() == "Y")
                {
                    lt_Icon.Text = "<img src=\"" + Application["File_WebUrl"] + "icon_cert/108.jpg\" width=\"35\" />";
                }
                else
                {
                    lt_Icon.Text = ExtensionMethods.fn_Extensions.GetCertIcon(
                        DataBinder.Eval(dataItem.DataItem, "Cert_ID").ToString()
                        , DataBinder.Eval(dataItem.DataItem, "Detail_ID").ToString());
                }

                #region --檔案處理--
                //檔案顯示 - 證書
                Literal lt_CertFile = ((Literal)e.Item.FindControl("lt_CertFile"));
                lt_CertFile.Text = setFileUri(
                    DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile").ToString().Trim()
                    , DataBinder.Eval(dataItem.DataItem, "Cert_File").ToString()
                    , modelNo);

                //檔案顯示 - TestReport
                Literal lt_FileTestReport = ((Literal)e.Item.FindControl("lt_FileTestReport"));
                lt_FileTestReport.Text = setFileUri(
                    DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Report").ToString().Trim()
                    , DataBinder.Eval(dataItem.DataItem, "Cert_File_Report").ToString()
                    , modelNo);

                //檔案顯示 - 自我宣告
                Literal lt_FileCE = ((Literal)e.Item.FindControl("lt_FileCE"));
                lt_FileCE.Text = setFileUri(
                  DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE").ToString().Trim()
                  , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE").ToString()
                    , modelNo);

                Literal lt_FileCE_enUS = ((Literal)e.Item.FindControl("lt_FileCE_enUS"));
                lt_FileCE_enUS.Text = setFileUri(
                  DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_en_US").ToString().Trim()
                  , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_en_US").ToString()
                    , modelNo);

                Literal lt_FileCE_zhCN = ((Literal)e.Item.FindControl("lt_FileCE_zhCN"));
                lt_FileCE_zhCN.Text = setFileUri(
                  DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_zh_CN").ToString().Trim()
                  , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_zh_CN").ToString()
                    , modelNo);

                //檔案顯示 - 自我檢測
                Literal lt_FileCheck = ((Literal)e.Item.FindControl("lt_FileCheck"));
                lt_FileCheck.Text = setFileUri(
                    DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check").ToString().Trim()
                    , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check").ToString()
                    , modelNo);

                Literal lt_FileCheck_enUS = ((Literal)e.Item.FindControl("lt_FileCheck_enUS"));
                lt_FileCheck_enUS.Text = setFileUri(
                  DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_en_US").ToString().Trim()
                  , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_en_US").ToString()
                    , modelNo);

                Literal lt_FileCheck_zhCN = ((Literal)e.Item.FindControl("lt_FileCheck_zhCN"));
                lt_FileCheck_zhCN.Text = setFileUri(
                  DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_zh_CN").ToString().Trim()
                  , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_zh_CN").ToString()
                    , modelNo);
                #endregion
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 設定檔案下載連結
    /// </summary>
    /// <param name="orgName">原始檔名</param>
    /// <param name="fileName">真實檔名</param>
    /// <param name="modelNo">ModelNo</param>
    /// <returns>string</returns>
    /// <example>
    /// Application["File_DiskUrl"] + string.Format(@"Certification\{0}\", 品號) + 檔名
    /// </example>
    private string setFileUri(string orgName, string fileName, string modelNo)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }
        else
        {
            string uri = string.Format("../FileDownload.ashx?OrgiName={0}&FilePath={1}"
                , Server.UrlEncode(orgName.Trim())
                , Server.UrlEncode(Cryptograph.Encrypt(Param_DiskFolder + modelNo + "\\" + fileName)));

            return "<a href=\"" + uri + "\">下載</a>";
        }
    }
    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}Certification".FormatThis(
            fn_Param.WebUrl);
    }

    #endregion


    #region -- 傳遞參數 --

    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Request.QueryString["id"] == null ? "new" : Request.QueryString["id"].ToString();

            return DataID;
        }
        set
        {
            _Req_DataID = value;
        }
    }


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("Certification");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "/Cert_Search.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }

    /// <summary>
    /// [參數] - Disk資料夾路徑
    /// </summary>
    private string _Param_DiskFolder;
    public string Param_DiskFolder
    {
        get
        {
            return Application["File_DiskUrl"] + "Certification\\";
        }
        set
        {
            this._Param_DiskFolder = value;
        }
    }

    #endregion

}