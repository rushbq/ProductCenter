using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProdNewsData.Controllers;
using ProdNewsData.Models;

/*
所有檔案全部上傳至 RC9/ProductCenter/Prod_News/

*/
public partial class myProdNews_EditUpload : SecurityIn
{

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(
        fn_Param.myFtp_Username
        , fn_Param.myFtp_Password
        , fn_Param.myFtp_ServerUrl);


    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("132", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{1}Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg), fn_Param.WebUrl), true);
                    return;
                }

                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_ErrMessage.Visible = true;
                    this.ph_Data.Visible = false;
                    this.lt_ShowMsg.Text = "操作方式有誤，請回上一頁重試.";
                }
                else
                {
                    //載入資料
                    LookupData();
                    LookupData_Files("3", this.lv_Files_Mail);
                    LookupData_Files("1", this.lv_Files_Other);
                }

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料取得 --


    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //目前使用者
        search.Add((int)mySearch.UserID, fn_Param.CurrentAccount);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            this.ph_ErrMessage.Visible = true;
            this.ph_Data.Visible = false;
            this.lt_ShowMsg.Text = "無法取得資料";
            return;
        }

        //Get Data
        this.lt_Title.Text = query.Subject;

    }


    /// <summary>
    /// 取得檔案資料
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    private void LookupData_Files(string type, ListView list)
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _dataList = new ProdNewsRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetFileList(Req_DataID, type);


        //----- 資料整理:繫結 ----- 
        list.DataSource = query;
        list.DataBind();

        //Release
        query = null;
    }


    protected void lv_Files_Mail_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string attachID = ((HiddenField)e.Item.FindControl("hf_AttachID")).Value;
            string fileName = ((HiddenField)e.Item.FindControl("hf_FileName")).Value;
            string attachDesc = ((TextBox)e.Item.FindControl("tb_Desc")).Text;

            switch (e.CommandName.ToUpper())
            {
                case "DODEL":
                    //執行刪除
                    Del_File(attachID, fileName);
                    break;

                case "DOEDIT":
                    //----- 宣告:資料參數 -----
                    ProdNewsRepository _data = new ProdNewsRepository();

                    //----- 設定:資料欄位 -----
                    var data = new AttachFiles
                    {
                        AttID = Convert.ToInt32(attachID),
                        AttDesc = attachDesc
                    };

                    //----- 方法:更新資料 -----
                    if (false == _data.Update_FileDesc(data))
                    {
                        this.ph_ErrMessage.Visible = true;
                        this.lt_ShowMsg.Text = "資料更新失敗，請聯絡系統管理員";

                        _data = null;
                        return;
                    }
                    else
                    {
                        //導向本頁
                        Response.Redirect(PageUrl + "#attach1");
                    }

                    break;
            }
        }
    }

    protected void lv_Files_Other_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string attachID = ((HiddenField)e.Item.FindControl("hf_AttachID")).Value;
            string fileName = ((HiddenField)e.Item.FindControl("hf_FileName")).Value;

            //執行刪除
            Del_File(attachID, fileName);
        }
    }


    private void Del_File(string id, string filename)
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();


        //----- 設定:資料欄位 -----
        var data = new AttachFiles
        {
            AttID = Convert.ToInt32(id)
        };

        //----- 方法:刪除資料 -----
        if (false == _data.Delete_Files(data))
        {
            this.ph_ErrMessage.Visible = true;
            this.lt_ShowMsg.Text = "刪除失敗";
            return;
        }
        else
        {
            //刪除實體檔案
            _ftp.FTP_DelFile(UploadFolder, filename);

            //導向本頁
            Response.Redirect(PageUrl);
        }
    }

    #endregion


    #region -- 資料編輯 --

    /// <summary>
    /// 檔案上傳 - Mail
    /// </summary>
    protected void lbtn_AddFiles_Click(object sender, EventArgs e)
    {
        doUpload_Files(3);
    }


    /// <summary>
    /// 檔案上傳 - 其他附件
    /// </summary>
    protected void lbtn_AddFiles_Other_Click(object sender, EventArgs e)
    {
        doUpload_Files(1);
    }

    /// <summary>
    /// 開始上傳
    /// </summary>
    /// <param name="type"></param>
    private void doUpload_Files(byte type)
    {
        #region -- 檔案處理 --

        //宣告
        List<IOTempParam> ITempList = new List<IOTempParam>();
        Random rnd = new Random();

        int GetFileCnt = 0;

        //取得上傳檔案集合
        HttpFileCollection hfc = Request.Files;


        //--- 限制上傳數量 ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > 0)
            {
                GetFileCnt++;
            }
        }
        if (GetFileCnt > FileCountLimit)
        {
            //[提示]
            this.lt_UploadMessage.Text = "單次上傳超出限制, 每次上傳僅限 {0} 個檔案.".FormatThis(FileCountLimit);
            return;
        }

        //Check null
        if (GetFileCnt == 0)
        {
            this.lt_UploadMessage.Text = "您並未選擇任何檔案, 至少要上傳一個檔案.";
            return;
        }

        //--- 檔案檢查 ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > FileSizeLimit)
            {
                //[提示]
                this.lt_UploadMessage.Text = "部份檔案大小超出限制, 每個檔案大小限制為 {0} MB".FormatThis(FileSizeLimit);
                return;
            }

            if (hpf.ContentLength > 0)
            {
                //取得原始檔名
                string OrgFileName = Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = Path.GetExtension(OrgFileName).ToLower();
                if (false == CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                {
                    //[提示]
                    this.lt_UploadMessage.Text = "部份檔案副檔名不符規定, 僅可上傳副檔名為 {0}".FormatThis(FileExtLimit.Replace("|", ", "));
                    return;
                }
            }
        }


        //--- 檔案暫存List ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > 0)
            {
                //取得原始檔名
                string OrgFileName = Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = Path.GetExtension(OrgFileName).ToLower();

                //設定檔名, 重新命名
                string myFullFile = String.Format(@"{0:yyMMddHHmmssfff}{1}{2}"
                    , DateTime.Now
                    , rnd.Next(0, 99)
                    , FileExt);


                //判斷副檔名, 未符合規格的檔案不上傳
                if (CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                {
                    //設定暫存-檔案
                    ITempList.Add(new IOTempParam(myFullFile, OrgFileName, hpf));
                }
            }
        }

        #endregion


        #region -- 儲存檔案 --

        if (ITempList.Count > 0)
        {
            int errCnt = 0;
            //判斷資料夾, 不存在則建立
            _ftp.FTP_CheckFolder(UploadFolder);

            //暫存檔案List
            for (int row = 0; row < ITempList.Count; row++)
            {
                //取得個別檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;

                //執行上傳
                if (false == _ftp.FTP_doUpload(hpf, UploadFolder, ITempList[row].Param_FileName))
                {
                    errCnt++;
                }
            }

            if (errCnt > 0)
            {
                this.lt_UploadMessage.Text = "檔案上傳失敗, 失敗筆數為 {0} 筆, 請重新整理後再上傳!".FormatThis(errCnt);
                return;
            }
        }

        #endregion


        #region -- 資料處理 --

        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();
        List<AttachFiles> dataList = new List<AttachFiles>();

        //----- 設定:資料欄位 -----
        for (int row = 0; row < ITempList.Count; row++)
        {
            var data = new AttachFiles
            {
                NewsID = Convert.ToInt32(Req_DataID),
                AttType = type,
                AttachFile = ITempList[row].Param_FileName,
                AttachFile_Name = ITempList[row].Param_OrgFileName,
                AttDesc = "",
                Create_Who = fn_Param.CurrentUser
            };

            dataList.Add(data);
        }

        //----- 方法:新增資料 -----
        if (false == _data.Create_Attachment(dataList))
        {
            this.ph_ErrMessage.Visible = true;
            this.lt_ShowMsg.Text = "糟糕!! 上傳檔案後,資料新增失敗了...";
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl);
        }
        #endregion
    }


    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得參數 - DataID
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}myProdNews/EditUpload.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;

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
            return "jpg|png|pdf|doc|docx|xls|xlsx|txt|rar|zip";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 限制上傳的檔案大小(1MB = 1024000), 50MB
    /// </summary>
    private int _FileSizeLimit;
    public int FileSizeLimit
    {
        get
        {
            return 51200000;
        }
        set
        {
            this._FileSizeLimit = value;
        }
    }

    /// <summary>
    /// 限制上傳檔案數
    /// </summary>
    private int _FileCountLimit;
    public int FileCountLimit
    {
        get
        {
            return 5;
        }
        set
        {
            this._FileCountLimit = value;
        }
    }

    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}Prod_News/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], Req_DataID);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class IOTempParam
    {
        /// <summary>
        /// [參數] - 檔名
        /// </summary>
        private string _Param_FileName;
        public string Param_FileName
        {
            get { return this._Param_FileName; }
            set { this._Param_FileName = value; }
        }

        /// <summary>
        /// [參數] -原始檔名
        /// </summary>
        private string _Param_OrgFileName;
        public string Param_OrgFileName
        {
            get { return this._Param_OrgFileName; }
            set { this._Param_OrgFileName = value; }
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
        /// <param name="Param_FileName">系統檔名</param>
        /// <param name="Param_OrgFileName">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public IOTempParam(string Param_FileName, string Param_OrgFileName, HttpPostedFile Param_hpf)
        {
            this._Param_FileName = Param_FileName;
            this._Param_OrgFileName = Param_OrgFileName;
            this._Param_hpf = Param_hpf;
        }

    }
    #endregion

}