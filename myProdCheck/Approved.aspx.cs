using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;
using ProdCheckData.Models;
using ProdPhotoData.Controllers;

public partial class myProdCheck_Approved : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("515", out ErrMsg) == false)
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
            this.ph_btn.Visible = false;
            this.ph_Data.Visible = false;
            this.lt_ShowMsg.Text = "無法取得資料";
            return;
        }


        //判斷是否已Approved(以Approved_Time判斷)
        if (!string.IsNullOrEmpty(query.Approved_Time))
        {
            this.lt_MailTime.Text = ".....&nbsp;{0}".FormatThis(query.Approved_Time);

            this.ph_btn.Visible = false;
            this.ph_OK.Visible = true;
        }

        //Get Data
        string modelNo = query.ModelNo;
        string corp = query.Corp_UID.ToString();
        string firstID = query.FirstID;
        string secondID = query.SecondID;
        string finish = query.IsFinished;

        //填入資料
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


    protected void lbtn_No_Click(object sender, EventArgs e)
    {
        Response.Redirect(Page_SearchUrl);

    }

    protected void lbtn_Yes_Click(object sender, EventArgs e)
    {
        try
        {
            //----- 宣告:資料參數 -----
            ProdCheckRepository _data = new ProdCheckRepository();


            //----- 設定:資料欄位 -----
            var data = new ProdCheck
            {
                Data_ID = new Guid(Req_DataID),
                Approved_Who = fn_Param.CurrentUser
            };

            //----- 方法:更新資料 -----
            if (false == _data.Update_Approve(data))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "資料更新時發生錯誤!";
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl);

            }

        }
        catch (Exception)
        {

            throw;
        }
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
            return "{0}myProdCheck/Approved.aspx?DataID={1}".FormatThis(
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