using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;
using ProdCheckData.Models;
using ProdPhotoData.Controllers;

public partial class myProdCheck_Edit : SecurityIn
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
                    this.ph_JobBtns.Visible = false;
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
    /// 取得類別
    /// </summary>
    /// <param name="inputValue"></param>
    /// <param name="ddl">下拉選單object</param>
    private void Get_ClassList(string inputValue, DropDownList ddl)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList("");


        //----- 資料整理 -----
        ddl.Items.Clear();


        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        if (!string.IsNullOrEmpty(inputValue))
        {
            ddl.SelectedValue = inputValue;
        }

        query = null;
    }


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
            this.ph_Data.Visible = false;
            this.lt_ShowMsg.Text = "無法取得資料";
            return;
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
        this.Date_Est.Text = query.Est_CheckDay.ToDateString("yyyy-MM-dd");
        this.Date_Act.Text = query.Act_CheckDay.ToDateString("yyyy-MM-dd");
        this.Remark.Text = query.Remark;
        this.lt_Substitute_Model_No_TW.Text = query.SubNo_TW;
        this.lt_Substitute_Model_No_SH.Text = query.SubNo_SH;
        this.lt_Substitute_Model_No_SZ.Text = query.SubNo_SZ;
        this.lt_Pub_Notes.Text = query.ProdNotes.Replace("\r\n", "<BR/>");


        //維護資訊
        this.lt_Creater.Text = query.Create_Name;
        this.lt_CreateTime.Text = query.Create_Time;
        this.lt_Updater.Text = query.Update_Name;
        this.lt_UpdateTime.Text = query.Update_Time;


        //取得類別選單
        Get_ClassList(query.Status.ToString(), this.ddl_Status);

        //檢驗數量加總
        int totalCnt = _data.GetTotalCnt(corp, Req_DataID, modelNo);
        this.lt_CheckTotal.Text = fn_stringFormat.C_format(totalCnt.ToString());


        //-- 載入其他資料 --
        //ERP Data
        LookupErpData(corp, firstID, secondID, modelNo);

        //Rel Data
        LookupData_Rel();

        //Files
        LookupData_Files("1", this.lv_Files_Check);
        LookupData_Files("2", this.lv_Files_Other);

        //圖片大類
        Lookup_PhotoClass();


        //判斷是否已結案
        if (finish.Equals("Y"))
        {
            this.ph_Lock.Visible = true;
            this.ph_JobBtns.Visible = false;
            this.ph_RelBtn.Visible = false;
            this.ph_UploadBtn.Visible = false;

            this.lbtn_Save.Visible = false;
            this.ddl_Status.Enabled = false;
            this.Date_Est.Enabled = false;
            this.Date_Act.Enabled = false;
            this.Remark.Enabled = false;

            if (this.lv_RelData.Items.Count > 0)
            {
                foreach (var item in this.lv_RelData.Items)
                {
                    item.FindControl("lbtn_Delete").Visible = false;
                }
            }
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
    /// 取得檔案資料
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    private void LookupData_Files(string type, ListView list)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _dataList = new ProdCheckRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetFileList(Req_DataID, type);


        //----- 資料整理:繫結 ----- 
        list.DataSource = query;
        list.DataBind();

        //Release
        query = null;
    }


    /// <summary>
    /// 取得PDF下載路徑
    /// </summary>
    /// <param name="webType">內部或外部(insde/outside)</param>
    /// <param name="lang">語系</param>
    /// <param name="modelNo">品號</param>
    /// <returns></returns>
    public string GetPDFUrl(string webType, string lang, string modelNo)
    {
        string Token = "";

        #region -- PDF AccessToken --

        //[取得API Token]
        string LoginID = System.Web.Configuration.WebConfigurationManager.AppSettings["API_PDFLoginID"];
        string LoginPwd = Cryptograph.MD5(System.Web.Configuration.WebConfigurationManager.AppSettings["API_PDFLoginPwd"]);

        //Get Token Request (v1)
        string Url = "{0}GetAccessToken/".FormatThis(Application["API_WebUrl"]);

        string GetTokenJson = CustomExtension.WebRequest_byPOST(Url
             , "LoginID={0}&LoginPwd={1}".FormatThis(LoginID, LoginPwd));

        if (string.IsNullOrEmpty(GetTokenJson))
        {
            Response.Write("Token取得失敗");
        }

        //解析Json
        JObject jObject = JObject.Parse(GetTokenJson);

        //填入資料
        if (jObject["tokenID"] != null)
        {
            Token = jObject["tokenID"].ToString();
        }

        #endregion



        return "{0}PDF/{1}/?u={2}&f={3}".FormatThis(
              Application["API_WebUrl"].ToString()
              , HttpUtility.UrlEncode(Token)
              , HttpUtility.UrlEncode("http://view.prokits.com.tw/{0}/{1}/{2}/".FormatThis(webType.Equals("inside") ? "SIP-I" : "SIP-O", lang, HttpUtility.UrlEncode(modelNo)))
              , "{0}.pdf".FormatThis(HttpUtility.UrlEncode(modelNo))
          );
    }

    public string GetViewUrl(string webType, string lang, string modelNo)
    {
        return "http://view.prokits.com.tw/{0}/{1}/{2}/".FormatThis(webType.Equals("inside") ? "SIP-I" : "SIP-O", lang, HttpUtility.UrlEncode(modelNo));
    }


    /// <summary>
    /// 圖片大類別
    /// </summary>
    private void Lookup_PhotoClass()
    {
        //----- 宣告:資料參數 -----
        ProdPhotoRepository _dataList = new ProdPhotoRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetPhotoClass();
        StringBuilder html = new StringBuilder();

        //----- 資料整理:繫結 ----- 
        if (query.Count() > 0)
        {
            html.Append("<div class=\"row\">");
            html.Append(" <div class=\"col s12\">");
            html.Append("  <ul class=\"tabs tabs-fixed-width\">");
            //Top menu
            foreach (var item in query)
            {
                html.AppendLine("<li class=\"tab\"><a href=\"#cls{0}\" data-id=\"{0}\" target-id=\"cls{0}\">{1}</a></li>".FormatThis(item.ID, item.Label));
            }
            html.Append("   </ul>");
            html.Append(" </div>");

            //Content
            foreach (var item in query)
            {
                html.AppendLine("<div id=\"cls{0}\" class=\"col s12\">{1}</div>".FormatThis(item.ID, (item.ID).Equals(1) ? Get_DefPhotoContent() : item.Label));
            }
            html.Append("</div>");
        }


        //Release
        query = null;

        //output
        this.lt_PhotoContainer.Text = html.ToString();
    }


    /// <summary>
    /// 載入第一個Tab的產品圖 (picClass=1)
    /// </summary>
    /// <returns></returns>
    private string Get_DefPhotoContent()
    {
        //----- 宣告:資料參數 -----
        ProdPhotoRepository _data = new ProdPhotoRepository();


        //----- 原始資料:取得所有資料 -----
        string ModelNo = Param_ModelNo;
        string MCls = "1";

        var query = _data.GetPhotos(ModelNo, MCls);

        StringBuilder html = new StringBuilder();

        if (query.Count() > 0)
        {
            html.Append("<div class=\"row\">");
            foreach (var item in query)
            {
                if (!string.IsNullOrEmpty(item.ColValue))
                {
                    html.Append(" <div class=\"col s6 m4 l3\">");
                    html.Append(" <div class=\"card\">");
                    html.Append("  <div class=\"card-image\">");
                    html.Append("<img src=\"{0}\" class=\"materialboxed\" data-caption=\"{1}\">".FormatThis(
                        "{0}ProductPic/{1}/{2}/{3}".FormatThis(Application["RefUrl"].ToString(), ModelNo, MCls, item.ColValue)
                        , item.ColName
                        ));
                    html.Append("  </div>");
                    html.Append("  <div class=\"card-content\"><span class=\"card-title\">{0}</span></div>".FormatThis(item.ColName));
                    html.Append(" </div>");
                    html.Append(" </div>");
                }
            }

            html.Append("</div>");
        }


        return html.ToString();
    }

    #endregion


    #region -- 資料編輯 --

    protected void lbtn_Save_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();


        //----- 設定:資料欄位 -----
        var data = new ProdCheck
        {
            Data_ID = new Guid(Req_DataID),
            Est_CheckDay = this.Date_Est.Text,
            Act_CheckDay = this.Date_Act.Text,
            Status = Convert.ToInt32(this.ddl_Status.SelectedValue),
            Remark = this.Remark.Text,
            Update_Who = fn_Param.CurrentUser.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update(data))
        {
            this.ph_ErrMessage.Visible = true;
            this.lt_ShowMsg.Text = "資料更新失敗";
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl);
        }
    }

    /// <summary>
    /// 設為結案 
    /// </summary>
    protected void lbtn_Finish_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();


        //----- 設定:資料欄位 -----
        var data = new ProdCheck
        {
            Data_ID = new Guid(Req_DataID),
            Update_Who = fn_Param.CurrentUser.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update_Finish(data))
        {
            this.ph_ErrMessage.Visible = true;
            this.lt_ShowMsg.Text = "結案失敗";
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl);
        }
    }


    /// <summary>
    /// 設為隱藏
    /// </summary>
    protected void lbtn_Lock_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();


        //----- 設定:資料欄位 -----
        var data = new ProdCheck
        {
            Data_ID = new Guid(Req_DataID),
            Update_Who = fn_Param.CurrentUser.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update_Lock(data))
        {
            this.ph_ErrMessage.Visible = true;
            this.lt_ShowMsg.Text = "設定隱藏失敗";
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl);
        }
    }

    #endregion


    #region -- 資料顯示:已關聯列表 --

    private void LookupData_Rel()
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetRelList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_RelData.DataSource = query;
        this.lv_RelData.DataBind();
    }

    protected void lv_RelData_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string firstID = ((HiddenField)e.Item.FindControl("hf_Fid")).Value;
            string secondID = ((HiddenField)e.Item.FindControl("hf_Sid")).Value;


            //----- 宣告:資料參數 -----
            ProdCheckRepository _data = new ProdCheckRepository();


            //----- 設定:資料欄位 -----
            var inst = new RelData
            {
                DataID = Req_DataID,
                FirstID = firstID,
                SecondID = secondID
            };

            //----- 方法:刪除資料 -----
            if (false == _data.Delete_RelData(inst))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "關聯資料刪除失敗";
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#dataRel");
            }

        }
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return this._Param_ModelNo != null ? this._Param_ModelNo : this.lt_ModelNo.Text.Trim().ToUpper();
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }

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
            return "{0}myProdCheck/Edit.aspx?DataID={1}".FormatThis(
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