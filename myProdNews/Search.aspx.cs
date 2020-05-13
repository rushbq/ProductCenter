using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProdNewsData.Controllers;
using ProdNewsData.Models;

public partial class myProdNews_Search : SecurityIn
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
                //[權限判斷] - 訊息維護
                this.ph_Edit.Visible = fn_CheckAuth.CheckAuth_User("132", out ErrMsg);


                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
                }

                //[取得/檢查參數] - Status
                if (!string.IsNullOrEmpty(Req_Status))
                {
                    this.filter_Status.SelectedIndex = this.filter_Status.Items.IndexOf(this.filter_Status.Items.FindByValue(Req_Status));
                }


                //Get Data
                LookupDataList(Req_PageIdx);


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
    /// <param name="pageIndex"></param>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:分頁參數 -----
        int RecordsPerPage = 20;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        ArrayList PageParam = new ArrayList();  //條件參數

        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----

        #region >> 條件篩選 <<

        //目前使用者
        search.Add((int)mySearch.UserID, fn_Param.CurrentAccount);


        //[取得參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add((int)mySearch.Keyword, Req_Keyword);

            PageParam.Add("keyword=" + Server.UrlEncode(Req_Keyword));
        }

        //[取得參數] - Status
        if (!string.IsNullOrEmpty(Req_Status))
        {
            search.Add((int)mySearch.Status, Req_Status);

            PageParam.Add("Status=" + Server.UrlEncode(Req_Status));
        }

        #endregion


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search);


        //----- 資料整理:取得總筆數 -----
        TotalRow = query.Count();

        //----- 資料整理:頁數判斷 -----
        if (pageIndex > TotalRow && TotalRow > 0)
        {
            StartRow = 0;
            pageIndex = 1;
        }

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
            string getPager = CustomExtension.PageControl(TotalRow, RecordsPerPage, pageIndex, 5, PageUrl, PageParam, false
                , true, CustomExtension.myStyle.Goole);

            Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
            lt_Pager.Text = getPager;

            //Literal lt_TopPager = (Literal)this.lvDataList.FindControl("lt_TopPager");
            //lt_TopPager.Text = getPager;

            //重新整理頁面Url
            string thisPage = "{0}?Page={1}{2}".FormatThis(
                PageUrl
                , pageIndex
                , "&" + string.Join("&", PageParam.ToArray()));


            //暫存頁面Url, 給其他頁使用
            Session["BackListUrl"] = thisPage;
        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string dataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
            string isMail = ((HiddenField)e.Item.FindControl("hf_IsMail")).Value;

            switch (e.CommandName.ToUpper())
            {
                case "DOCLOSE":
                    //----- 宣告:資料參數 -----
                    ProdNewsRepository _data = new ProdNewsRepository();

                    //----- 設定:資料欄位 -----
                    var data = new Items
                    {
                        NewsID = Convert.ToInt32(dataID),
                        IsMail = isMail,
                        IsClose = "Y"
                    };

                    //----- 方法:更新資料 -----
                    if (false == _data.Update_Status(data))
                    {
                        Response.Write("設定失敗");

                        _data = null;
                        return;
                    }
                    else
                    {
                        //導向本頁
                        Response.Redirect("{0}?Page={1}".FormatThis(PageUrl, Req_PageIdx));
                    }

                    break;
            }
        }
    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //取得資料
                string Get_IsMail = DataBinder.Eval(dataItem.DataItem, "IsMail").ToString(); //發信
                string Get_IsClose = DataBinder.Eval(dataItem.DataItem, "IsClose").ToString();  //結案
                string Get_BPM_WorkItemID = DataBinder.Eval(dataItem.DataItem, "BPM_WorkItemID").ToString();  //BPM_WorkItemID
                string Get_BPM_Sno = DataBinder.Eval(dataItem.DataItem, "BPM_Sno").ToString();  //BPM_Sno
                string Get_Subject = DataBinder.Eval(dataItem.DataItem, "Subject").ToString();  //Subject


                //取得控制項
                PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                PlaceHolder ph_Send = (PlaceHolder)e.Item.FindControl("ph_Send");
                PlaceHolder ph_Close = (PlaceHolder)e.Item.FindControl("ph_Close");
                Literal lt_BPMSno = (Literal)e.Item.FindControl("lt_BPMSno");
                Literal lt_Subject = (Literal)e.Item.FindControl("lt_Subject");

                //處理過長主旨
                if (Get_Subject.Length > 50)
                {
                    lt_Subject.Text = "<span class=\"tooltipped\" data-position=\"bottom\" data-tooltip=\"{1}\">{0}...</span>".FormatThis(
                        Get_Subject.Left(50)
                        , Get_Subject);
                }
                else
                {
                    lt_Subject.Text = Get_Subject;
                }

                /* 判斷是否有EFGP表單檢視權限
                   連結參數:hdnWorkItemOID = WorkItemID / hdnUserId = 目前使用者工號
                 * 流程序號
                */
                if (string.IsNullOrWhiteSpace(Get_BPM_WorkItemID))
                {
                    lt_BPMSno.Text = Get_BPM_Sno;
                }
                else
                {
                    lt_BPMSno.Text = "<a href=\"{0}GP/PerformWorkFromMail?hdnMethod=performWorkFromMail&hdnUserId={1}&hdnWorkItemOID={2}\" target=\"_blank\" title=\"查看BPM表單內容\">{3}</a>"
                        .FormatThis(
                         fn_Param.BPM_Url
                         , fn_Param.CurrentAccount
                         , Get_BPM_WorkItemID
                         , Get_BPM_Sno + "&nbsp;<i class=\"tiny material-icons\">open_in_new</i>"
                        );
                }


                /*
                判斷權限
                 編輯:修改 / 結案
                 發信:發信 / 結案
                */
                ph_Edit.Visible = Auth_Edit;
                ph_Send.Visible = Auth_SendMail;
                ph_Close.Visible = (Auth_Edit || Auth_SendMail);

                /*
                判斷狀態
                 發信=Y:隱藏結案,修改
                 結案=Y:隱藏結案,修改
                */
                if (Get_IsMail.Equals("Y") || Get_IsClose.Equals("Y"))
                {
                    ph_Close.Visible = false;
                    //ph_Edit.Visible = false;
                }

            }
        }
        catch (Exception)
        {

            throw new Exception("系統發生錯誤 - ItemDataBound！");
        }
    }


    protected string Get_Status(string st)
    {
        if (st.Equals("Y"))
        {
            return "<span class=\"green-text\"><i class=\"material-icons\">done</i></span>";
        }
        else
        {
            return "<span class=\"grey-text text-lighten-2\"><i class=\"material-icons\">remove</i></span>";
        }
    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 查詢按鈕
    /// </summary>
    protected void btn_KeySearch_Click(object sender, EventArgs e)
    {
        doSearch();
    }


    /// <summary>
    /// 執行查詢
    /// </summary>
    /// <param name="keyword"></param>
    private void doSearch()
    {
        StringBuilder url = new StringBuilder();
        string keyword = this.filter_Keyword.Text;
        string status = this.filter_Status.SelectedValue;


        url.Append("{0}?Page=1".FormatThis(PageUrl));


        //[查詢條件] - 關鍵字
        if (!string.IsNullOrEmpty(keyword))
        {
            url.Append("&Keyword=" + Server.UrlEncode(keyword));
        }

        //[查詢條件] - status
        url.Append("&status=" + Server.UrlEncode(status));


        //執行轉頁
        Response.Redirect(url.ToString(), false);
    }



    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - PageIdx(目前索引頁)
    /// </summary>
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
    private int _Req_PageIdx;


    /// <summary>
    /// 取得傳遞參數 - Keyword
    /// </summary>
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
    private string _Req_Keyword;


    /// <summary>
    /// 取得傳遞參數 - Status
    /// </summary>
    public string Req_Status
    {
        get
        {
            String data = Request.QueryString["Status"];
            return (CustomExtension.String_資料長度Byte(data, "1", "1", out ErrMsg)) ? data.Trim() : "";
        }
        set
        {
            this._Req_Status = value;
        }
    }
    private string _Req_Status;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}myProdNews/Search.aspx".FormatThis(fn_Param.WebUrl);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


    /// <summary>
    /// 維護權限
    /// </summary>
    private bool _Auth_Edit;
    public bool Auth_Edit
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("132", out ErrMsg);
        }
        private set
        {
            this._Auth_Edit = value;
        }
    }

    /// <summary>
    /// 發信權限
    /// </summary>
    private bool _Auth_SendMail;
    public bool Auth_SendMail
    {
        get
        {
            return fn_CheckAuth.CheckAuth_User("133", out ErrMsg);
        }
        private set
        {
            this._Auth_SendMail = value;
        }
    }

    #endregion
}