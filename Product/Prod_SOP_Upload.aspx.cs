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
using Resources;
using LogRecord;
using ExtensionIO;


public partial class Prod_SOP_Upload : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg;

            //[權限判斷] - SOP上傳
            if (fn_CheckAuth.CheckAuth_User("126", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("無使用權限！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得/檢查參數] - Model_No (品號)
            if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[帶出資料]
            LookupDataList();
        }

    }

    #region -- 資料讀取 --
    /// <summary>
    /// 副程式 - 取得資料列表
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
                SBSql.AppendLine(" SELECT SID, SOP_File, SOP_OrgFile, Create_Time ");
                SBSql.AppendLine(" FROM Prod_File_SOP ");
                SBSql.AppendLine(" WHERE (RTRIM(Model_No)= @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY Create_Time DESC ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                cmd.Parameters.Clear();

                //[取得參數] - 編號
                string GetDataID = ((Literal)e.Item.FindControl("lt_SID")).Text;

                //[SQL] - 取得已上傳的檔案
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT SOP_File ");
                SBSql.AppendLine(" FROM Prod_File_SOP WHERE (SID = @Param_ID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                //[暫存參數] - 檔案名稱
                List<string> ListFiles = new List<string>();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //暫存檔案名稱
                        ListFiles.Add(DT.Rows[row]["SOP_File"].ToString());
                    }
                }

                //[SQL] - 刪除資料
                SBSql.Clear();
                SBSql.AppendLine(" DELETE FROM Prod_File_SOP WHERE (SID = @Param_ID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料刪除失敗！", "");
                }
                else
                {
                    //刪除檔案
                    for (int idx = 0; idx < ListFiles.Count; idx++)
                    {
                        IOManage.DelFile(Param_FileFolder, ListFiles[idx]);
                    }

                    //Log
                    fn_Log.Log_Rec("SOP上傳"
                        , Param_ModelNo
                        , "刪除SOP檔案, 編號:{0}, 檔名:{1}".FormatThis(GetDataID, string.Join(",", ListFiles))
                        , fn_Param.CurrentAccount.ToString());

                    //頁面跳至明細
                    fn_Extensions.JsAlert("", PageUrl);
                }

            }
        }
    }
    #endregion

    #region -- 前端事件觸發 --
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
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(SID), 0) + 1 FROM Prod_File_SOP) ");
                    SBSql.AppendLine(" INSERT INTO Prod_File_SOP( ");
                    SBSql.AppendLine("  SID, Model_No, SOP_File, SOP_OrgFile");
                    SBSql.AppendLine("  , Create_Who, Create_Time");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Param_ModelNo");
                    SBSql.AppendLine(string.Format(", @FileNewName_{0}, @FileFullName_{0}", row));
                    SBSql.AppendLine("  , @Param_CreateWho, GETDATE() ");
                    SBSql.AppendLine(" ) ");
                    cmd.Parameters.AddWithValue("FileNewName_" + row, ITempList[row].Param_Pic);
                    cmd.Parameters.AddWithValue("FileFullName_" + row, ITempList[row].Param_OrgPic);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料新增失敗！", "");
                }
                else
                {
                    //[IO] - 儲存檔案
                    for (int i = 0; i < ITempList.Count; i++)
                    {
                        HttpPostedFile hpf = ITempList[i].Param_hpf;
                        if (hpf.ContentLength > 0)
                        {
                            //[IO] - 儲存檔案
                            IOManage.Save(hpf, Param_FileFolder, ITempList[i].Param_Pic);
                        }
                    }

                    //Log
                    fn_Log.Log_Rec("SOP上傳"
                        , Param_ModelNo
                        , "上傳SOP檔案"
                        , fn_Param.CurrentAccount.ToString());

                    //轉頁
                    fn_Extensions.JsAlert("", PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 上傳！", "");
        }
    }

    #endregion

    #region --參數設定--
    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"SOP_PDF\" + Param_ModelNo + @"\";
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
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"SOP_PDF/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["Model_No"]) ? "" : fn_stringFormat.Filter_Html(Request.QueryString["Model_No"].ToString().Trim()).ToUpper();
        }
        set
        {
            this._Param_ModelNo = value;
        }
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "pdf";
        }
        set
        {
            this._FileExtLimit = value;
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
            return this._PageUrl != null ? this._PageUrl : "Prod_SOP_Upload.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo) + "&r=" + String.Format("{0:mmssfff}", DateTime.Now);
        }
        set
        {
            this._PageUrl = value;
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
}