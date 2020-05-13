using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;
using Resources;

public partial class Prod_MallPic : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 商城輔圖
                if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Product/Prod_Search.aspx";


                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;

                //帶出檔案列表
                LookupData_List();

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 資料列表
    /// </summary>
    private void LookupData_List()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Base.PMID, RTRIM(Base.Model_No) AS Model_No, Base.PicFile, Base.PicDesc, Base.Sort ");
                SBSql.AppendLine(" FROM Prod_MallPic Base ");
                SBSql.AppendLine(" WHERE (Base.Model_No = @Model_No) AND (LangCode = @LangCode) ");
                SBSql.AppendLine(" ORDER BY Base.Sort ASC, Base.Create_Time DESC");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                cmd.Parameters.AddWithValue("LangCode", Param_InfoLang);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    //Layout元件處理
                    if (DT.Rows.Count > 0)
                    {
                        this.lt_DownloadBtn.Text = "<a href=\"{0}\" class=\"btn btn-info\"><i class=\"glyphicon glyphicon-save\"></i>&nbsp;下載壓縮包</a>"
                            .FormatThis(ZipDownloadPath);
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 依語系判斷&回傳所屬ID
    /// </summary>
    /// <param name="CheckType">來源類別</param>
    /// <param name="CheckLang">語言別</param>
    /// <returns></returns>
    private string GetInfoValue(string CheckType, string CheckLang)
    {
        if (string.IsNullOrEmpty(CheckType) || string.IsNullOrEmpty(CheckLang))
        {
            return "";
        }
        //判斷語系
        string TabIndex;
        switch (CheckLang.ToUpper())
        {
            case "ZH-CN":
                TabIndex = "8";
                break;

            case "ZH-TW":
                TabIndex = "9";
                break;

            case "EN-US":
                TabIndex = "10";
                break;

            default:
                TabIndex = "";
                break;
        }
        //判斷來源
        switch (CheckType)
        {
            case "TabIndex":
                return TabIndex;

            default:
                return "";
        }

    }
    #endregion

    
    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            String Model_No = Request.QueryString["Model_No"];
            if (fn_Extensions.String_字數(Model_No, "1", "40", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                return "";
            }
            else
            {
                return Model_No.Trim();

            }
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }

    /// <summary>
    /// 語言別
    /// </summary>
    private string _Param_InfoLang;
    public string Param_InfoLang
    {
        get
        {
            String Lang = Request.QueryString["Lang"];
            if (string.IsNullOrEmpty(Lang))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:history.back(-1);");
                return "";
            }
            else
            {
                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = GetInfoValue("TabIndex", Lang);

                return Lang;
            }
        }
        private set
        {
            this._Param_InfoLang = value;
        }
    }

    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : @"{0}MallPic\{1}\{2}\".FormatThis(
                  Application["File_DiskUrl"]
                  , Param_ModelNo
                  , Param_InfoLang);
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 圖片資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null ? this._Param_WebFolder : "{0}MallPic/{1}/{2}/".FormatThis(
                Application["File_WebUrl"]
                , Server.UrlEncode(Param_ModelNo)
                , Param_InfoLang);
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}Product/Prod_MallPic.aspx?Lang={1}&Model_No={2}".FormatThis(
                Application["WebUrl"]
                , Param_InfoLang
                , HttpUtility.UrlEncode(Param_ModelNo)
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// ZIP下載路徑
    /// </summary>
    private string _ZipDownloadPath;
    public string ZipDownloadPath
    {
        get
        {
            return "{0}MallPic_Zip/{1}_gallery_{2}.zip".FormatThis(
                Application["File_WebUrl"]
                , Server.UrlEncode(Param_ModelNo)
                , Param_InfoLang);
        }
        set
        {
            this._ZipDownloadPath = value;
        }
    }
    #endregion

   
    #region -- 語系參數 --
    /// <summary>
    /// [Navi] - 系統首頁
    /// </summary>
    private string _Navi_系統首頁;
    public string Navi_系統首頁
    {
        get
        {
            return Res_Navi.系統首頁;
        }
        private set
        {
            this._Navi_系統首頁 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料庫;
    public string Navi_產品資料庫
    {
        get
        {
            return Res_Navi.產品資料庫;
        }
        private set
        {
            this._Navi_產品資料庫 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料;
    public string Navi_產品資料
    {
        get
        {
            return Res_Navi.產品資料;
        }
        private set
        {
            this._Navi_產品資料 = value;
        }
    }

    #endregion
}
