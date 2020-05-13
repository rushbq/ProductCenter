using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using System.IO;
using System.Collections;

public partial class ProdPic_Group_View : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //[初始化]
            string ErrMsg = "";
          
            //[帶入參數] - 圖片類別
            Param_Class = Request.QueryString["C_ID"].Trim();
            //[帶入參數] - 品號
            Param_ModelNo = Request.QueryString["ModelNo"].Trim();
            //[帶入參數] - 來源參數
            Param_flag = Request.QueryString["flag"].Trim();
            //判斷是否有上一頁暫存參數
            if (Session["BackListUrl"] == null)
                Session["BackListUrl"] = Application["WebUrl"] + "ProdPic/ProdPic_Search.aspx?flag=" + Param_flag;


             //若類別 = 說明書(8), 導向至產品檔案頁
            if (Param_Class.Equals("8"))
            {
                Response.Redirect("../Product/Prod_Files_View.aspx?Model_No={0}".FormatThis(Server.UrlEncode(Param_ModelNo)));
                return;
            }

            //[權限判斷] - 圖片資料庫 (依 flag 判斷)
            if (fn_CheckAuth.CheckAuth_User(Param_flag, out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[ASCX 頁簽] - 加入屬性
            Ascx_ProdPicClass1.Param_CurrPage = Param_Class;
            Ascx_ProdPicClass1.Param_ModelNo = Param_ModelNo;
            Ascx_ProdPicClass1.Param_flag = Param_flag;

            //[ASCX 基本資料] - 加入屬性        
            Ascx_ProdData1.Param_C_ID = Param_Class;
            Ascx_ProdData1.Param_ModelNo = Param_ModelNo;

            if (!IsPostBack)
            {
                //[判斷 & 取得參數] - C_ID 圖片類別
                if (fn_Extensions.Num_正整數(Param_Class, "1", "999999999", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 圖片類別！", Session["BackListUrl"].ToString());
                }
                //[判斷 & 取得參數] - Model_No 品號
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 品號！", Session["BackListUrl"].ToString());
                }
                //[取得/檢查參數] - flag 
                if (fn_Extensions.String_字數(Request.QueryString["flag"], "1", "3", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 來源參數！", Session["BackListUrl"].ToString());
                }

                //帶出圖片列表
                LookupDataList();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", Session["BackListUrl"].ToString());
        }

    }

    #region -- 圖片明細資料 --
    /// <summary>
    /// 副程式 - 取得明細資料列表
    /// </summary>
    private void LookupDataList()
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
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("      Pic_ID, Lang, Pic_OrgFile, Pic_File, Pic_Desc, Sort ");
                SBSql.AppendLine("      , (CASE WHEN Update_Time IS NULL THEN Create_Time ELSE Update_Time END) AS LastTime ");
                SBSql.AppendLine("      , Create_Who, Create_Time, Update_Who, Update_Time ");
                SBSql.AppendLine("  FROM ProdPic_Group ");
                SBSql.AppendLine(" WHERE (Pic_Class= @Param_Class) AND (Model_No= @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY Sort ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Class", Param_Class);
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    if (DT.Rows.Count > 0)
                    {
                        //[判斷參數] - 圖片類別, 控制標頭
                        System.Web.UI.HtmlControls.HtmlTableCell tdHeadLang = (System.Web.UI.HtmlControls.HtmlTableCell)this.lvDataList.FindControl("tdHeadLang");
                        switch (Param_Class)
                        {
                            case "3":
                                tdHeadLang.Visible = true;
                                break;

                            default:
                                tdHeadLang.Visible = false;
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 明細資料！", "");
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            string ErrMsg = "";
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //[判斷參數] - 圖片類別, 控制列表欄位
            System.Web.UI.HtmlControls.HtmlTableCell td_Lang = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("td_Lang");
            switch (Param_Class)
            {
                case "3":
                    //類別=3(DM), 顯示語言別
                    td_Lang.Visible = true;
                    //語系別
                    ((Literal)e.Item.FindControl("lt_Lang")).Text =
                        fn_Extensions.GetLangValue(DataBinder.Eval(dataItem.DataItem, "Lang").ToString(), out ErrMsg);
                    break;

                default:
                    td_Lang.Visible = false;
                    break;
            }
        }
    }
    #endregion

    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <param name="OrgPicName">原始檔名</param>
    /// <returns>string</returns>
    public string PicUrl(string PicName, string OrgPicName)
    {
        string preView = "";

        //判斷是否為圖片
        string strFileExt = ".jpg||.png";
        if (fn_Extensions.CheckStrWord(PicName, strFileExt, "|", 2))
        {
            //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
            preView = string.Format(
                "<td class=\"L2Img\" width=\"120px\"><a class=\"PicGroup\" rel=\"PicGroup\" href=\"{0}\" title=\"{1}\">" +
                "<img src=\"{0}\" width=\"100px\" border=\"0\">" +
                "</a></td>"
                , Param_WebFolder + Param_ModelNo + "/" + Param_Class + "/" + PicName
                , OrgPicName);
        }
        else
        {
            //非圖片，顯示下載連結
            preView = string.Format(
               "<td class=\"L2Info\" width=\"200px\"> " +
               "<a href=\"../FileDownload.ashx?OrgiName={2}&FilePath={1}\">{0}</a>" +
               "</td>"
               , OrgPicName
               , Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + PicName))
               , Server.UrlEncode(OrgPicName)
               );
        }

        //輸出Html
        return preView;
    }

    /// <summary>
    /// 取得外部圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <returns>string</returns>
    public string DealerPicUrl(string PicFile)
    {
        return "<span class=\"styleBluelight\">路徑：</span><input type=\"text\" value=\"{0}\" style=\"width: 280px; cursor: pointer;color: #555\" readonly=\"readonly\" />"
           .FormatThis(
               "{0}ProductPic/{1}"
               .FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["ref_WebUrl"]
                   , Param_ModelNo + "/" + Param_Class + "/" + PicFile
               )
           );
    }

    #region "參數設定"
    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"ProductPic\" + Param_ModelNo + @"\" + Param_Class + @"\";
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"ProductPic/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 本頁路徑
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return this._PageUrl != null ? this._PageUrl : "ProdPic_Group.aspx?flag=" + Server.UrlEncode(Param_flag) + "&C_ID=" + Server.UrlEncode(Param_Class) + "&ModelNo=" + Server.UrlEncode(Param_ModelNo) + "&r=" + String.Format("{0:mmssfff}", DateTime.Now) + "&#35;PicList";
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// [參數] - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get;
        set;
    }

    /// <summary>
    /// [參數] - 圖片類別
    /// </summary>
    private string _Param_Class;
    public string Param_Class
    {
        get;
        set;
    }

    /// <summary>
    /// 來源參數
    /// </summary>
    private string _Param_flag;
    public string Param_flag
    {
        get;
        set;
    }

    #endregion
}
