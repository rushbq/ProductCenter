using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProdNewsData.Controllers;
using ProdNewsData.Models;

public partial class myProdNews_View : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("106", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{1}Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg), fn_Param.WebUrl), true);
                    return;
                }


                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Data.Visible = false;
                }
                else
                {
                    this.ph_Data.Visible = true;

                    //載入資料
                    LookupData();
                    LookupData_Prod();
                    LookupData_SubProd();
                    LookupData_Files("3", this.lv_Files_Mail);
                    LookupData_Files("1", this.lv_Files_Other);
                    LookupData_Files("2", this.lv_Files_BPM);

                }

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示 --
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
        var query = _data.GetDataList(search).Take(1);

        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            this.lt_DataID.Text = item.NewsID.ToString();
            this.lt_BPMSno.Text = item.BPM_Sno ?? "---";
            this.lt_BPMFormNo.Text = item.BPM_FormNo ?? "";
            this.ddl_Class.SelectedValue = item.ClassID.ToString();
            this.ddl_Lang.SelectedValue = item.Lang;
            this.tb_Subject.Text = item.Subject;
            this.rbl_TimingType.SelectedValue = item.TimingType.ToString();
            this.tb_TimingDate.Text = item.TimingDate;
            this.tb_Desc1.Text = HttpUtility.HtmlDecode(item.Desc1);
            this.tb_Desc2.Text = item.Desc2;

            //發送對象
            Lookup_Target();


            //維護資訊
            this.lt_Creater.Text = item.Create_Name;
            this.lt_CreateTime.Text = item.Create_Time;
            this.lt_Updater.Text = item.Update_Name;
            this.lt_UpdateTime.Text = item.Update_Time;
            this.lt_Sender.Text = item.Send_Name;
            this.lt_SendTime.Text = item.Send_Time;
        }
    }


    /// <summary>
    /// 發送對象
    /// </summary>
    private void Lookup_Target()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetRelTarget(Req_DataID);

        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            foreach (ListItem cbItem in this.cbl_Target.Items)
            {
                if (item.TargetID.Equals(cbItem.Value))
                {
                    cbItem.Selected = true;
                }
            }
        }
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


    #endregion


    #region -- 資料顯示:產品品號 --

    /// <summary>
    /// 顯示產品品號關聯
    /// </summary>
    private void LookupData_Prod()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _dataList = new ProdNewsRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetRelModelList(Req_DataID);

        //----- 資料整理:繫結 ----- 
        this.lv_Prod.DataSource = query;
        this.lv_Prod.DataBind();

        //Release
        query = null;
    }

    #endregion


    #region -- 資料顯示:產品替代品號 --

    /// <summary>
    /// 顯示產品品號關聯
    /// </summary>
    private void LookupData_SubProd()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _dataList = new ProdNewsRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetRelSubModelList(Req_DataID);

        //----- 資料整理:繫結 ----- 
        this.lv_SubProd.DataSource = query;
        this.lv_SubProd.DataBind();

        //Release
        query = null;
    }

    #endregion


    #region -- 參數設定 --
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
                Url = "{0}myProdNews/Search.aspx".FormatThis(fn_Param.WebUrl);
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

    /// <summary>
    /// 取得參數 - 資料編號
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
    /// BPM上傳目錄
    /// </summary>
    private string _BPM_UploadFolder;
    public string BPM_UploadFolder
    {
        get
        {
            return "EFGP/{0}/".FormatThis(this.lt_BPMFormNo.Text);
        }
        set
        {
            this._BPM_UploadFolder = value;
        }
    }

    #endregion
}