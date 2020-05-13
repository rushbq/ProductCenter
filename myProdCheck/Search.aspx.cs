using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;
using ProdCheckData.Models;

public partial class myProdCheck_Search : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("530", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", Application["WebUrl"], HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //載入選單-狀態
                Get_ClassList(Req_Status, this.filter_Status);
                //載入選單-人員
                Get_UpdWhoList(Req_Who, this.filter_Who);


                //載入資料
                LookupDataList(Req_PageIdx);


                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
                }

                //[取得/檢查參數] - Corp
                if (!string.IsNullOrEmpty(Req_CorpID))
                {
                    this.filter_CorpID.SelectedIndex = this.filter_CorpID.Items.IndexOf(this.filter_CorpID.Items.FindByValue(Req_CorpID));
                }


                //[取得/檢查參數] - sDate
                if (!string.IsNullOrEmpty(Req_sDate))
                {
                    this.filter_sDate.Text = Req_sDate;
                }

                //[取得/檢查參數] - eDate
                if (!string.IsNullOrEmpty(Req_eDate))
                {
                    this.filter_eDate.Text = Req_eDate;
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
    /// <param name="pageIndex"></param>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:分頁參數 -----
        int RecordsPerPage = 10;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數
        bool doRedirect = false;    //是否重新導向

        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)mySearch.Keyword, Req_Keyword);

            PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
        }
        //[取得/檢查參數] - Status
        if (!string.IsNullOrEmpty(Req_Status))
        {
            search.Add((int)mySearch.Status, Req_Status);

            PageParam.Add("Status=" + Server.UrlEncode(Req_Status));
        }

        //[取得參數] - sDate
        if (!string.IsNullOrEmpty(Req_sDate))
        {
            search.Add((int)mySearch.StartDate, Req_sDate);

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        //[取得參數] - eDate
        if (!string.IsNullOrEmpty(Req_eDate))
        {
            search.Add((int)mySearch.EndDate, Req_eDate);

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }

        //[取得/檢查參數] - CorpID
        if (!string.IsNullOrEmpty(Req_CorpID))
        {
            search.Add((int)mySearch.CorpID, Req_CorpID);

            PageParam.Add("CorpID=" + Server.UrlEncode(Req_CorpID));
        }

        //[取得/檢查參數] - Who
        if (!string.IsNullOrEmpty(Req_Who))
        {
            search.Add((int)mySearch.UpdateWho, Req_Who);

            PageParam.Add("Who=" + Server.UrlEncode(Req_Who));
        }
        #endregion

        //IsLock 
        search.Add((int)mySearch.IsLock, "N");


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search);


        //----- 資料整理:取得總筆數 -----
        TotalRow = query.Count();

        //----- 資料整理:頁數判斷 -----

        #region >> 頁數判斷 <<

        if (pageIndex > TotalRow && TotalRow > 0)
        {
            pageIndex = 1;

            doRedirect = true;
        }

        if (StartRow >= TotalRow && TotalRow > 0)
        {
            //當指定page的資料數已不符合計算出來的數量時, 重新導向
            //當前頁數-1
            pageIndex = pageIndex - 1;

            doRedirect = true;
        }

        if (doRedirect)
        {
            //重新整理頁面Url
            string thisPage = "{0}?Page={1}{2}".FormatThis(
                PageUrl
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));

            //重新導向
            Response.Redirect(thisPage);
        }

        #endregion

        //----- 資料整理:選取每頁顯示筆數 -----
        var data = query.Skip(StartRow).Take(RecordsPerPage);

        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = data;
        this.lvDataList.DataBind();

        //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
        if (query.Count() == 0)
        {
            Session.Remove("BackListUrl");
        }
        else
        {
            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false
                , false, CustomExtension.myStyle.Goole);


            //重新整理頁面Url
            string thisPage = "{0}?Page={1}{2}".FormatThis(
                PageUrl
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));


            //暫存頁面Url, 給其他頁使用
            Session["BackListUrl"] = thisPage;
        }

    }


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


        ddl.Items.Add(new ListItem("所有狀態", ""));

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
    /// 取得更新人員
    /// </summary>
    /// <param name="inputValue"></param>
    /// <param name="ddl">下拉選單object</param>
    private void Get_UpdWhoList(string inputValue, DropDownList ddl)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetUpdWhoList();


        //----- 資料整理 -----
        ddl.Items.Clear();


        ddl.Items.Add(new ListItem("所有人員(建立者)", ""));

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


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得必要的資料
            string dataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;


            //----- 宣告:資料參數 -----
            ProdCheckRepository _data = new ProdCheckRepository();


            //----- 設定:資料欄位 -----
            var data = new ProdCheck
            {
                Data_ID = new Guid(dataID),
                Update_Who = fn_Param.CurrentUser.ToString()
            };

            //----- 方法:更新資料 -----
            if (false == _data.Update_Lock(data))
            {
                Response.Write("設定隱藏失敗");
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl);
            }
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //取得控制項
            PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
            Label lb_finish = (Label)e.Item.FindControl("lb_finish");
            Label lb_isRel = (Label)e.Item.FindControl("lb_isRel");
            Label lb_isLock = (Label)e.Item.FindControl("lb_isLock");
            Label lb_isApproved = (Label)e.Item.FindControl("lb_isApproved");
            
            PlaceHolder ph_ShowRpt = (PlaceHolder)e.Item.FindControl("ph_ShowRpt");
            PlaceHolder ph_Approve = (PlaceHolder)e.Item.FindControl("ph_Approve");
            

            //取得資料
            string isFinished = DataBinder.Eval(dataItem.DataItem, "IsFinished").ToString();
            string isRel = DataBinder.Eval(dataItem.DataItem, "IsRel").ToString();
            string isLock = DataBinder.Eval(dataItem.DataItem, "IsLock").ToString();
            string isReported = DataBinder.Eval(dataItem.DataItem, "IsReported").ToString();
            string isApproved = DataBinder.Eval(dataItem.DataItem, "Approved_Time").ToString();

            //判斷是否結案
            ph_Edit.Visible = isFinished.Equals("N");
            lb_finish.Visible = isFinished.Equals("Y");

            //判斷是否關聯
            lb_isRel.Visible = isRel.Equals("Y");

            //判斷是否隱藏
            lb_isLock.Visible = isLock.Equals("Y");

            //ph_ShowRpt
            ph_ShowRpt.Visible = isReported.Equals("Y");

            //approved
            lb_isApproved.Visible = !string.IsNullOrEmpty(isApproved);
            ph_Approve.Visible = string.IsNullOrEmpty(isApproved);

        }
    }


    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 關鍵字快查
    /// </summary>
    protected void btn_KeySearch_Click(object sender, EventArgs e)
    {
        string keyword = fn_stringFormat.Filter_Html(this.filter_Keyword.Text);

        doSearch(keyword);
    }


    /// <summary>
    /// 重置條件
    /// </summary>
    protected void lbtn_Reset_Click(object sender, EventArgs e)
    {
        Response.Redirect(PageUrl);
    }



    /// <summary>
    /// 執行查詢
    /// </summary>
    /// <param name="keyword"></param>
    private void doSearch(string keyword)
    {
        StringBuilder url = new StringBuilder();

        url.Append("{0}?Page=1".FormatThis(PageUrl));


        //[查詢條件] - Status
        if (this.filter_Status.SelectedIndex > 0)
        {
            url.Append("&Status=" + Server.UrlEncode(this.filter_Status.SelectedValue));
        }


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
        }

        //[查詢條件] - sDate
        if (!string.IsNullOrEmpty(this.filter_sDate.Text))
        {
            url.Append("&sDate=" + Server.UrlEncode(this.filter_sDate.Text));
        }
        //[查詢條件] - eDate
        if (!string.IsNullOrEmpty(this.filter_eDate.Text))
        {
            url.Append("&eDate=" + Server.UrlEncode(this.filter_eDate.Text));
        }

        //[查詢條件] - CorpID
        if (this.filter_CorpID.SelectedIndex > 0)
        {
            url.Append("&CorpID=" + Server.UrlEncode(this.filter_CorpID.SelectedValue));
        }

        //[查詢條件] - Who
        if (this.filter_Who.SelectedIndex > 0)
        {
            url.Append("&Who=" + Server.UrlEncode(this.filter_Who.SelectedValue));
        }

        //執行轉頁
        Response.Redirect(url.ToString(), false);
    }


    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
    private int _Req_PageIdx;
    public int Req_PageIdx
    {
        get
        {
            int data = Request.QueryString["Page"] == null ? 1 : Convert.ToInt32(Request.QueryString["Page"]);
            return data;
        }
        set
        {
            this._Req_PageIdx = value;
        }
    }

    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}myProdCheck/Search.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
    private string _Req_Keyword;
    public string Req_Keyword
    {
        get
        {
            String Keyword = Request.QueryString["Keyword"];
            return (CustomExtension.String_資料長度Byte(Keyword, "1", "50", out ErrMsg)) ? Keyword.Trim() : "";
        }
        set
        {
            this._Req_Keyword = value;
        }
    }


    private string _Req_Status;
    public string Req_Status
    {
        get
        {
            String data = Request.QueryString["Status"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Status = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - sDate
    /// </summary>
    public string Req_sDate
    {
        get
        {
            String data = Request.QueryString["sDate"];
            return (CustomExtension.String_資料長度Byte(data, "1", "10", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_sDate = value;
        }
    }
    private string _Req_sDate;


    /// <summary>
    /// 取得傳遞參數 - eDate
    /// </summary>
    public string Req_eDate
    {
        get
        {
            String data = Request.QueryString["eDate"];
            return (CustomExtension.String_資料長度Byte(data, "1", "50", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_eDate = value;
        }
    }
    private string _Req_eDate;


    /// <summary>
    /// 取得傳遞參數 - Who
    /// </summary>
    public string Req_Who
    {
        get
        {
            String data = Request.QueryString["who"];
            return (CustomExtension.String_資料長度Byte(data, "1", "38", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_Who = value;
        }
    }
    private string _Req_Who;


    /// <summary>
    /// 取得傳遞參數 - CorpID
    /// </summary>
    public string Req_CorpID
    {
        get
        {
            String data = Request.QueryString["CorpID"];
            return (CustomExtension.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_CorpID = value;
        }
    }
    private string _Req_CorpID;

    #endregion

}