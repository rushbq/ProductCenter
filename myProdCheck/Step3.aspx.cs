using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;
using ProdCheckData.Models;

public partial class myProdCheck_Step3 : SecurityIn
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

                //Check Null
                if (string.IsNullOrEmpty(Req_Corp))
                {
                    Go_Step1();
                    return;
                }
                if (string.IsNullOrEmpty(Req_FirstID) && string.IsNullOrEmpty(Req_SecondID)
                    && string.IsNullOrEmpty(Req_Year) && string.IsNullOrEmpty(Req_Vendor)
                    && string.IsNullOrEmpty(Req_ModelNo))
                {
                    Go_Step2();
                    return;
                }

                //[取得資料] - 公司別
                GetCorpData(Req_Corp);


                //[取得資料] - 採購資料
                GetPurData();
            }


        }
        catch (Exception)
        {

            throw;
        }

    }


    #region -- 資料取得 --

    /// <summary>
    /// 取得公司別資料
    /// </summary>
    private void GetCorpData(string corpUID)
    {
        //----- 宣告:資料參數 -----
        ParamsRepository _data = new ParamsRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(corpUID))
        {
            search.Add((int)Common.mySearch.DataID, corpUID);

        }

        //----- 原始資料:取得資料 -----
        var query = _data.GetCorpList(search).Take(1).FirstOrDefault();


        //Check Null
        if (query == null)
        {
            Go_Step2();
            return;
        }

        //Print Data
        this.lt_CorpName.Text = query.Corp_Name;

    }


    /// <summary>
    /// 取得採購單資料
    /// </summary>
    private void GetPurData()
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(Req_FirstID))
        {
            search.Add((int)mySearch.FirstID, Req_FirstID);
        }

        if (!string.IsNullOrEmpty(Req_SecondID))
        {
            search.Add((int)mySearch.SecondID, Req_SecondID);
        }


        if (!string.IsNullOrEmpty(Req_Year))
        {
            string sDate = "{0}0101".FormatThis(Req_Year);
            string eDate = "{0}1231".FormatThis(Req_Year);

            search.Add((int)mySearch.StartDate, sDate);
            search.Add((int)mySearch.EndDate, eDate);
        }

        if (!string.IsNullOrEmpty(Req_Vendor))
        {
            search.Add((int)mySearch.Vendor, Req_Vendor);
        }

        if (!string.IsNullOrEmpty(Req_ModelNo))
        {
            search.Add((int)mySearch.DataID, Req_ModelNo);
        }


        //----- 原始資料:取得資料 -----
        var query = _data.GetPurData(Req_Corp, search);


        //----- 資料整理:繫結 ----- 
        this.lv_List.DataSource = query;
        this.lv_List.DataBind();

        //Release
        query = null;
    }

    protected void lv_List_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得必要的資料
            string corp = Req_Corp;
            string firstID = ((HiddenField)e.Item.FindControl("hf_FirstID")).Value;
            string secondID = ((HiddenField)e.Item.FindControl("hf_SecondID")).Value;
            string modelNo = ((HiddenField)e.Item.FindControl("hf_ModelNo")).Value;
            string vendor = ((HiddenField)e.Item.FindControl("hf_Vendor")).Value;


            //----- 宣告:資料參數 -----
            ProdCheckRepository _data = new ProdCheckRepository();


            //----- 設定:資料欄位 -----
            //產生Guid
            string guid = CustomExtension.GetGuid();

            var data = new ProdCheck
            {
                Data_ID = new Guid(guid),
                Corp_UID = Convert.ToInt16(corp),
                FirstID = firstID,
                SecondID = secondID,
                ModelNo = modelNo,
                Vendor = vendor,
                Create_Who = fn_Param.CurrentUser.ToString()
            };

            //----- 方法:新增資料 -----
            if (false == _data.Create(data))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "資料新增失敗, 請截圖後並聯絡系統管理員.";
                return;
            }
            else
            {
                //更新Url
                string thisUrl = "{0}myProdCheck/Edit.aspx?DataID={1}".FormatThis(Application["WebUrl"], guid);

                //導向
                Response.Redirect(thisUrl);
            }
        }
    }

    protected void lv_List_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //取得控制項
            PlaceHolder ph_showAdded = (PlaceHolder)e.Item.FindControl("ph_showAdded");

            //取得資料
            Int32 RelCnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "RelCnt"));

            //判斷是否有使用過
            ph_showAdded.Visible = RelCnt > 0 ? true : false;
        }
    }


    #endregion


    #region -- 其他功能 --
    /// <summary>
    /// 回Step1
    /// </summary>
    private void Go_Step1()
    {
        this.ph_Data.Visible = false;
        this.ph_ErrMessage.Visible = true;
        this.lt_ShowMsg.Text = "<a href=\"{0}myProdCheck/Step1.aspx\" class=\"btn waves-effect waves-light light-blue\">請選擇正確的公司別, 點此回上一步重新選擇</a>".FormatThis(Application["WebUrl"]);
    }


    /// <summary>
    /// 回Step2
    /// </summary>
    private void Go_Step2()
    {
        this.ph_Data.Visible = false;
        this.ph_ErrMessage.Visible = true;
        this.lt_ShowMsg.Text = "<a href=\"{0}myProdCheck/Step2.aspx?corp={1}\" class=\"btn waves-effect waves-light light-blue\">請選擇正確的篩選條件, 點此回上一步重新選擇</a>".FormatThis(
            Application["WebUrl"]
            , Req_Corp);
    }


    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得參數 - Corp
    /// </summary>
    private string _Req_Corp;
    public string Req_Corp
    {
        get
        {
            String data = Request.QueryString["corp"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Corp = value;
        }
    }

    /// <summary>
    /// 取得參數 - FirstID
    /// </summary>
    private string _Req_FirstID;
    public string Req_FirstID
    {
        get
        {
            String data = Request.QueryString["fid"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_FirstID = value;
        }
    }

    /// <summary>
    /// 取得參數 - SecondID
    /// </summary>
    private string _Req_SecondID;
    public string Req_SecondID
    {
        get
        {
            String data = Request.QueryString["sid"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_SecondID = value;
        }
    }

    /// <summary>
    /// 取得參數 - Year
    /// </summary>
    private string _Req_Year;
    public string Req_Year
    {
        get
        {
            String data = Request.QueryString["year"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Year = value;
        }
    }

    /// <summary>
    /// 取得參數 - Vendor
    /// </summary>
    private string _Req_Vendor;
    public string Req_Vendor
    {
        get
        {
            String data = Request.QueryString["vendor"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Vendor = value;
        }
    }

    /// <summary>
    /// 取得參數 - ModelNo
    /// </summary>
    private string _Req_ModelNo;
    public string Req_ModelNo
    {
        get
        {
            String data = Request.QueryString["modelno"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }

    #endregion

}