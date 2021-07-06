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
                if (fn_CheckAuth.CheckAuth_User(GetInfoValue("Auth", Param_InfoLang), out ErrMsg) == false)
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
                string sql = @"
                    SELECT Base.PMID, RTRIM(Base.Model_No) AS Model_No, Base.PicFile, Base.PicDesc, Base.Sort
                     , Base.Create_Time, Base.Update_Time
                     , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Account_Name = Base.Create_Who)), '') AS Create_Name
                     , ISNULL((SELECT Account_Name + ' (' + Display_Name + ')' FROM [PKSYS].dbo.User_Profile WHERE (Account_Name = Base.Update_Who)), '') AS Update_Name
                    FROM Prod_MallPic Base
                    WHERE (Base.Model_No = @Model_No) AND (UPPER(LangCode) = UPPER(@LangCode))
                    ORDER BY Base.Sort ASC, Base.Create_Time DESC
                    ";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                cmd.Parameters.AddWithValue("LangCode", Param_InfoLang);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    //Layout:下載按鈕
                    if (DT.Rows.Count > 0)
                    {
                        Literal lt_DownloadBtn = (Literal)this.lvDataList.FindControl("lt_DownloadBtn");
                        lt_DownloadBtn.Text = "<a href=\"{0}\" class=\"btn btn-info\"><i class=\"glyphicon glyphicon-save\"></i>&nbsp;下載壓縮包</a>"
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

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
            string Get_FileName = ((HiddenField)e.Item.FindControl("hf_FileName")).Value;

            using (SqlCommand cmd = new SqlCommand())
            {
                //刪除資料
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Prod_MallPic WHERE (PMID = @DataID); ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Get_DataID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料處理失敗", "");
                    return;
                }
                else
                {
                    //刪除檔案
                    IOManage.DelFile(Param_FileFolder, Get_FileName);

                    //更新圖片集Zip
                    Update_ZipFiles(Param_ModelNo, Param_InfoLang);

                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }
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
        string AuthID, TabIndex;
        switch (CheckLang.ToUpper())
        {
            case "ZH-CN":
                AuthID = "141";
                TabIndex = "8";
                break;

            case "ZH-TW":
                AuthID = "142";
                TabIndex = "9";
                break;

            case "EN-US":
                AuthID = "143";
                TabIndex = "10";
                break;

            default:
                AuthID = "";
                TabIndex = "";
                break;
        }
        //判斷來源
        switch (CheckType)
        {
            case "Auth":
                return AuthID;

            case "TabIndex":
                return TabIndex;

            default:
                return "";
        }

    }
    #endregion

    #region -- 資料編輯 --
    /// <summary>
    /// 上傳圖片
    /// </summary>
    protected void btn_Upload_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();
            HttpFileCollection hfc = Request.Files;
            for (int i = 0; i <= hfc.Count - 1; i++)
            {
                HttpPostedFile hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpf);
                    ITempList.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hpf));
                }
            }
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - Statement
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" Declare @New_ID AS INT ");

                for (int row = 0; row < ITempList.Count; row++)
                {
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(PMID), 0) + 1 FROM Prod_MallPic) ");
                    SBSql.AppendLine(" INSERT INTO Prod_MallPic( ");
                    SBSql.AppendLine("  PMID, LangCode, Model_No");
                    SBSql.AppendLine("  , PicFile, PicDesc, Sort");
                    SBSql.AppendLine("  , Create_Who, Create_Time");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @LangCode, @Model_No");
                    SBSql.AppendLine("  , @FileNewName_{0} , '', 999".FormatThis(row));
                    SBSql.AppendLine("  , @CreateWho, GETDATE() ");
                    SBSql.AppendLine(" ); ");
                    cmd.Parameters.AddWithValue("FileNewName_" + row, ITempList[row].Param_Pic);

                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("LangCode", Param_InfoLang);
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                cmd.Parameters.AddWithValue("CreateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    Response.Write(ErrMsg);
                    fn_Extensions.JsAlert("資料新增失敗！", Page_CurrentUrl);
                }
                else
                {
                    //[IO] - 儲存檔案
                    for (int row = 0; row < ITempList.Count; row++)
                    {
                        HttpPostedFile hpf = ITempList[row].Param_hpf;
                        if (hpf.ContentLength > 0)
                        {
                            IOManage.Save(hpf, Param_FileFolder, ITempList[row].Param_Pic, Param_Width, Param_Height);
                        }
                    }

                    //更新圖片集Zip
                    Update_ZipFiles(Param_ModelNo, Param_InfoLang);

                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 上傳！", "");
        }
    }

    /// <summary>
    /// 儲存列表欄位
    /// </summary>
    protected void btn_SaveList_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("目前尚無資料可設定！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - Statement
                StringBuilder SBSql = new StringBuilder();
                for (int row = 0; row < lvDataList.Items.Count; row++)
                {
                    //[取得參數] - 編號
                    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_DataID")).Value;
                    //[取得參數] - 說明
                    string lvParam_Desc = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Desc")).Text.Trim();
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Sort")).Text;

                    SBSql.AppendLine(" UPDATE Prod_MallPic SET ");
                    SBSql.AppendLine("  Update_Who = @Param_UpdateWho, Update_Time = GETDATE()");
                    SBSql.AppendLine("  , PicDesc = @PicDesc_{0}, Sort = @Sort_{0}".FormatThis(row));
                    SBSql.AppendLine(" WHERE (PMID = @PMID_{0});".FormatThis(row));

                    cmd.Parameters.AddWithValue("PMID_" + row, lvParam_ID);
                    cmd.Parameters.AddWithValue("PicDesc_" + row, lvParam_Desc);
                    cmd.Parameters.AddWithValue("Sort_" + row, lvParam_Sort);
                }
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", Page_CurrentUrl);
                }
                else
                {
                    Response.Redirect(Page_CurrentUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存排序！", "");
        }
    }

    /// <summary>
    /// 更新圖片集壓縮檔
    /// </summary>
    /// <param name="ModelNo">品號</param>
    /// <param name="myClass">類別</param>
    private bool Update_ZipFiles(string ModelNo, string myClass)
    {
        //來源資料夾
        string sourceFolder = @"{0}{1}\{2}\{3}".FormatThis(Application["File_DiskUrl"], "MallPic", ModelNo, myClass);
        //目標資料夾
        string targetFolder = @"{0}{1}\".FormatThis(Application["File_DiskUrl"], "MallPic_Zip");
        //目標檔案
        string targetFileName = "{0}_gallery_{1}.zip".FormatThis(ModelNo, myClass);

        return fn_CustomIO.exec_ZipFiles(sourceFolder, targetFolder, targetFileName);
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

    #region -- 上傳參數 --
    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 1200;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 1200;
        }
        set
        {
            this._Param_Height = value;
        }
    }
    #endregion


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
        }

        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Pic">系統檔名</param>
        /// <param name="Param_OrgPic">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
        }
    }

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
