using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using MailMethods;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;
using ProdCheckData.Models;

public partial class myProdCheck_Mail : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("520", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", Application["WebUrl"], Server.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Data.Visible = false;
                    this.ph_ErrMessage.Visible = true;
                    this.lt_ShowMsg.Text = "操作方式有誤，請回上一頁重試.";
                    return;
                }
                else
                {
                    this.ph_Data.Visible = true;


                    //載入資料
                    LookupData();
          
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
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            this.ph_ErrMessage.Visible = true;
            this.ph_Mail.Visible = false;
            this.ph_Data.Visible = false;
            this.lt_ShowMsg.Text = "無法取得資料";
            return;
        }

        //IsReported = Y 才能發
        if (!query.IsReported.Equals("Y"))
        {
            this.ph_ErrMessage.Visible = true;
            this.ph_Mail.Visible = false;
            this.lt_ShowMsg.Text = "------ 檢驗報告尚未上傳! ------";
            return;
        }

        //判斷是否已發信(以Mail_Time判斷)
        if (!string.IsNullOrEmpty(query.Mail_Time))
        {
            this.lt_MailTime.Text = ".....&nbsp;{0}".FormatThis(query.Mail_Time);
            //this.ph_MailBtn.Visible = false;
            //this.ph_Mail.Visible = false;
            this.ph_OK.Visible = true;
        }

        //Get Data
        string modelNo = query.ModelNo;
        string corp = query.Corp_UID.ToString();
        string firstID = query.FirstID;
        string secondID = query.SecondID;
        string finish = query.IsFinished;

        //填入資料
        this.lt_Title.Text = modelNo;
        this.lt_Corp.Text = query.Corp_Name;
        this.lt_Date_Est.Text = query.Est_CheckDay.ToDateString("yyyy-MM-dd");
        this.lt_Date_Act.Text = query.Act_CheckDay.ToDateString("yyyy-MM-dd");
        this.lt_Remark.Text = query.Remark.Replace("\r", "<br/>");
        this.lt_Status.Text = query.StatusName;

        //檢驗數量加總
        int totalCnt = _data.GetTotalCnt(corp, Req_DataID, modelNo);
        this.lt_CheckTotal.Text = fn_stringFormat.C_format(totalCnt.ToString());


        //-- 載入其他資料 --
        //ERP Data
        LookupErpData(corp, firstID, secondID, modelNo);


        //Files
        LookupData_Files("1", this.lv_Files_Check);
        LookupData_Files("2", this.lv_Files_Other);


        //判斷是否已結案
        if (finish.Equals("Y"))
        {
            this.ph_Lock.Visible = true;
        }
    }


    /// <summary>
    /// 取得ERP採購單資料
    /// </summary>
    /// <param name="corp"></param>
    /// <param name="fid"></param>
    /// <param name="sid"></param>
    /// <param name="modelNo"></param>
    private void LookupErpData(string corp, string fid, string sid, string modelNo)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, modelNo);
        search.Add((int)mySearch.FirstID, fid);
        search.Add((int)mySearch.SecondID, sid);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetPurData(corp, search).Take(1).FirstOrDefault();

        if (query == null)
        {
            this.ph_ErrMessage.Visible = true;
            this.ph_Data.Visible = false;
            this.lt_ShowMsg.Text = "無法取得ERP資料";
            return;
        }

        //Get Data
        this.lt_ErpID.Text = query.FirstID + " - " + query.SecondID;
        this.lt_BuyDate.Text = query.BuyDate;
        this.lt_BuyCnt.Text = fn_stringFormat.C_format(query.BuyQty.ToString());
        this.lt_Vendor.Text = "{0} ({1})".FormatThis(query.CustName, query.CustID);
        this.lt_ModelNo.Text = query.ModelNo;
        modelNo = query.ModelNo;
    }


    /// <summary>
    /// 取得檔案資料-void
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    private void LookupData_Files(string type, ListView list)
    {
        //----- 原始資料:取得所有資料 -----
        var query = GetFiles(Req_DataID, type);


        //----- 資料整理:繫結 ----- 
        list.DataSource = query;
        list.DataBind();

        //Release
        query = null;
    }


    /// <summary>
    /// 取得檔案資料-source
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private IQueryable<CheckFiles> GetFiles(string id, string type)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _dataList = new ProdCheckRepository();

        return _dataList.GetFileList(id, type);
    }

    #endregion


    #region -- 功能按鈕 --

    protected void btn_Setting_Click(object sender, EventArgs e)
    {
        try
        {
            /*
             * Step1:將勾選清單存入 Prod_Check_MailList
             * Step2:自動Update Prod_Check_MailList.MailAddress = PKSYS.User_Profile.Email
             * Step3:取得Mail清單，開始發信
             */

            //建立Mail清單
            if (!Create_MailList())
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "資料處理發生錯誤!";
                return;
            }

            //取得Mail清單，開始發信
            List<string> mailList = Get_MailList();

            #region -- 發信 --

            //[設定參數] - 建立者
            fn_Mail.Create_Who = "System";

            //[設定參數] - 來源程式/功能
            fn_Mail.FromFunc = "ProductCenter, 外驗查檢表發送";

            //[設定參數] - 寄件人
            fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

            //[設定參數] - 寄件人顯示名稱
            fn_Mail.SenderName = "Pro'sKit Mail System";

            //[設定參數] - 收件人
            if (mailList == null)
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "無法取得收件人清單";
                return;
            }
            fn_Mail.Reciever = mailList;

            //[設定參數] - 轉寄人群組
            fn_Mail.CC = null;

            //[設定參數] - 密件轉寄人群組
            fn_Mail.BCC = null;

            //[設定參數] - 郵件主旨
            fn_Mail.Subject = "[外驗查檢表] {0} ,{1}".FormatThis(this.lt_Vendor.Text, this.lt_ModelNo.Text);

            //[設定參數] - 郵件內容
            fn_Mail.MailBody = Get_MailBody();

            //[設定參數] - 指定檔案 - 路徑
            fn_Mail.FilePath = "";

            //[設定參數] - 指定檔案 - 檔名
            fn_Mail.FileName = "";

            //發送郵件
            fn_Mail.SendMail();

            //[判斷參數] - 寄件是否成功
            if (!fn_Mail.MessageCode.Equals(200))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "發送Mail時發生了一點小問題, 錯誤代碼:{0}".FormatThis(fn_Mail.MessageCode);
                return;
            }
            else
            {
                //Update Mail Info
                //----- 宣告:資料參數 -----
                ProdCheckRepository _data = new ProdCheckRepository();


                //----- 設定:資料欄位 -----
                var data = new ProdCheck
                {
                    Data_ID = new Guid(Req_DataID),
                    Mail_Who = fn_Param.CurrentUser.ToString()
                };

                //----- 方法:更新資料 -----
                if (false == _data.Update_MailSent(data))
                {
                    this.ph_ErrMessage.Visible = true;
                    this.lt_ShowMsg.Text = "Mail已發送，但資料更新時發生錯誤!";
                    return;
                }
                else
                {
                    //導向本頁
                    Response.Redirect(PageUrl);
                    
                }

            }

            #endregion


        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// 建立Mail清單
    /// </summary>
    /// <returns></returns>
    private bool Create_MailList()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" DECLARE @NewID AS INT ");
            sql.AppendLine(" DELETE FROM Prod_Check_MailList WHERE (Data_ID = @DataID)");

            //[取得參數值] - 編號組合(工號)
            string[] strAry = Regex.Split(this.tb_Values_User.Text, @"\,{1}");
            var query = from el in strAry
                        select new
                        {
                            Val = el.ToString().Trim()
                        };

            //[SQL] - 資料新增
            foreach (var item in query)
            {
                //-- 產生序號 --
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(MailID), 0) + 1 FROM Prod_Check_MailList WHERE (Data_ID = @DataID));");

                //新增資料
                sql.AppendLine(" INSERT INTO Prod_Check_MailList( ");
                sql.AppendLine("  Data_ID, MailID, MailTo, MailAddress, Create_Who, Create_Time");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT @DataID, @NewID, N'{0}', Email, @Create_Who, GETDATE()".FormatThis(item.Val));
                sql.AppendLine(" FROM PKSYS.dbo.User_Profile ");
                sql.AppendLine(" WHERE (Email IS NOT NULL) AND (Email <> '') AND (Guid = N'{0}'); ".FormatThis(item.Val));
            }


            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);
            cmd.Parameters.AddWithValue("Create_Who", fn_Param.CurrentUser.ToString());

            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }


    /// <summary>
    /// 取得Mail清單
    /// </summary>
    /// <returns>List</returns>
    private List<string> Get_MailList()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();
        List<string> GetEmail = new List<string>();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT MailAddress");
            sql.AppendLine(" FROM Prod_Check_MailList WITH (NOLOCK)");
            sql.AppendLine(" WHERE (Data_ID = @DataID)");


            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);

            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                }
            }
        }

        //return
        return GetEmail;
    }


    /// <summary>
    /// 取得寄信內容
    /// </summary>
    /// <returns></returns>
    private StringBuilder Get_MailBody()
    {
        //Html Url(CDN)
        string myHtmlUrl = "{0}ProductCenter/Prod_Check/Mail.html".FormatThis(fn_Param.CDNUrl);

        //宣告
        StringBuilder html = new StringBuilder();

        //取得html內容
        html.Append(CustomExtension.WebRequest_byGET(myHtmlUrl).ToString());

        //填入資料
        html.Replace("#ModelNo#", this.lt_ModelNo.Text);
        html.Replace("#vendor#", this.lt_Vendor.Text);
        html.Replace("#erpID#", this.lt_ErpID.Text);
        html.Replace("#purDate#", this.lt_BuyDate.Text);
        html.Replace("#corp#", this.lt_Corp.Text);
        html.Replace("#purQty#", this.lt_BuyCnt.Text);

        html.Replace("#status#", this.lt_Status.Text);
        html.Replace("#checkQty#", this.lt_CheckTotal.Text);
        html.Replace("#estDate#", this.lt_Date_Est.Text);
        html.Replace("#actDate#", this.lt_Date_Act.Text);
        html.Replace("#remark#", this.lt_Remark.Text);
        html.Replace("#今年#", DateTime.Now.Year.ToString());

        //主管核准
        html.Replace("#approveItems#"
            , "<a href=\"{0}myProdCheck/Approved.aspx?DataID={1}\" target=\"_blank\" style=\"color: #FFFFFF; text-decoration: none; display: block; padding: 5px;\">請點此前往核准</a>".FormatThis(
                fn_Param.WebUrl, Req_DataID
            ));

        //[取得檔案]
        var query_1 = GetFiles(Req_DataID, "1");
        var query_2 = GetFiles(Req_DataID, "2");

        string fileHtml = "";

        foreach (var item in query_1)
        {
            fileHtml += "<div style=\"background-color: #43a047; border: 1px solid #43a047; text-align: center; margin: 10px;\">";
            fileHtml += "<a href=\"{0}\" target=\"_blank\" style=\"color: #FFFFFF; text-decoration: none; display: block; padding: 5px;\">{1}</a>"
                .FormatThis(fn_Param.RefUrl + UploadFolder + item.AttachFile
                , item.AttachFile_Name);
            fileHtml += "</div>";
        }

        foreach (var item in query_2)
        {
            fileHtml += "<div style=\"background-color: #43a047; border: 1px solid #43a047; text-align: center; margin: 10px;\">";
            fileHtml += "<a href=\"{0}\" target=\"_blank\" style=\"color: #FFFFFF; text-decoration: none; display: block; padding: 5px;\">{1}</a>"
                .FormatThis(fn_Param.RefUrl + UploadFolder + item.AttachFile
                , item.AttachFile_Name);
            fileHtml += "</div>";
        }

        //Release
        query_1 = null;
        query_2 = null;

        html.Replace("#loopItems#", fileHtml);

        //return
        return html;
    }

    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得參數 - DataID
    /// </summary>
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
    private string _Req_DataID;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}myProdCheck/Mail.aspx?DataID={1}".FormatThis(
                Application["WebUrl"]
                , Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;



    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}ProdCheck/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], Req_DataID);
        }
        set
        {
            this._UploadFolder = value;
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
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}myProdCheck/Search.aspx".FormatThis(fn_Param.WebUrl);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

    #endregion


}