using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using CustomController;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;
using ProdSampleData.Controllers;

public partial class mySample_Search : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷] Start
                #region --權限--

                if (fn_CheckAuth.CheckAuth_User("146", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", Server.UrlEncode(ErrMsg)), true);
                    return;
                }

                #endregion
                //[權限判斷] End

                //Get Class
                LookupClassList(myClass.source, this.filter_Source, "所有來源");
                LookupClassList(myClass.check, this.filter_Check, "所有類別");
                LookupClassList(myClass.status, this.filter_Status, "所有狀態");
                //Get Users
                Get_UserList(this.filter_Who, true);


                //Get Data
                LookupDataList(Req_PageIdx);


                //[取得/檢查參數] - Company
                if (!string.IsNullOrEmpty(Req_Company))
                {
                    this.filter_Company.SelectedIndex = this.filter_Company.Items.IndexOf(this.filter_Company.Items.FindByValue(Req_Company));
                }
                //[取得/檢查參數] - Source
                if (!string.IsNullOrEmpty(Req_Source))
                {
                    this.filter_Source.SelectedIndex = this.filter_Source.Items.IndexOf(this.filter_Source.Items.FindByValue(Req_Source));
                }
                //[取得/檢查參數] - Check
                if (!string.IsNullOrEmpty(Req_Check))
                {
                    this.filter_Check.SelectedIndex = this.filter_Check.Items.IndexOf(this.filter_Check.Items.FindByValue(Req_Check));
                }
                //[取得/檢查參數] - Status
                if (!string.IsNullOrEmpty(Req_Status))
                {
                    this.filter_Status.SelectedIndex = this.filter_Status.Items.IndexOf(this.filter_Status.Items.FindByValue(Req_Status));
                }
                //[取得/檢查參數] - Who
                if (!string.IsNullOrEmpty(Req_Who))
                {
                    this.filter_Who.SelectedIndex = this.filter_Who.Items.IndexOf(this.filter_Who.Items.FindByValue(Req_Who));
                }

                //[取得/檢查參數] - DateType
                if (!string.IsNullOrEmpty(Req_DateType))
                {
                    this.filter_DateType.SelectedIndex = this.filter_DateType.Items.IndexOf(this.filter_DateType.Items.FindByValue(Req_DateType));
                }
                //[取得/檢查參數] - DateRange
                if (!string.IsNullOrEmpty(Req_DateRange))
                {
                    this.filter_DateType.SelectedIndex = this.filter_DateType.Items.IndexOf(this.filter_DateType.Items.FindByValue(Req_DateRange));
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
                //[取得/檢查參數] - Keyword
                if (!string.IsNullOrEmpty(Req_Keyword))
                {
                    this.filter_Keyword.Text = Req_Keyword;
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
    /// <param name="pageIndex"></param>
    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:網址參數 -----
        int RecordsPerPage = 10;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        int DataCnt = 0;
        ArrayList PageParam = new ArrayList();  //條件參數,for pager

        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        #region >> 條件篩選 <<

        //[取得/檢查參數] - Keyword
        if (!string.IsNullOrEmpty(Req_Keyword))
        {
            search.Add("Keyword", Req_Keyword);

            PageParam.Add("Keyword=" + Server.UrlEncode(Req_Keyword));
        }
        //[取得/檢查參數] - Company
        if (!string.IsNullOrEmpty(Req_Company))
        {
            search.Add("Company", Req_Company);

            PageParam.Add("Company=" + Server.UrlEncode(Req_Company));
        }
        //[取得/檢查參數] - Source
        if (!string.IsNullOrEmpty(Req_Source))
        {
            search.Add("Source", Req_Source);

            PageParam.Add("Source=" + Server.UrlEncode(Req_Source));
        }
        //[取得/檢查參數] - Check
        if (!string.IsNullOrEmpty(Req_Check))
        {
            search.Add("Check", Req_Check);

            PageParam.Add("Check=" + Server.UrlEncode(Req_Check));
        }
        //[取得/檢查參數] - Status
        if (!string.IsNullOrEmpty(Req_Status))
        {
            search.Add("Status", Req_Status);

            PageParam.Add("Status=" + Server.UrlEncode(Req_Status));
        }
        //[取得/檢查參數] - Who
        if (!string.IsNullOrEmpty(Req_Who))
        {
            search.Add("AssignWho", Req_Who);

            PageParam.Add("Who=" + Server.UrlEncode(Req_Who));
        }

        //[取得/檢查參數] - DateType
        if (!string.IsNullOrEmpty(Req_DateType))
        {
            search.Add("DateType", Req_DateType);

            PageParam.Add("DateType=" + Server.UrlEncode(Req_DateType));
        }
        //[取得/檢查參數] - DateRange
        if (!string.IsNullOrEmpty(Req_DateRange))
        {
            PageParam.Add("DateRange=" + Server.UrlEncode(Req_DateRange));
        }
        //[取得/檢查參數] - sDate
        if (!string.IsNullOrEmpty(Req_sDate))
        {
            search.Add("StartDate", Req_sDate);

            PageParam.Add("sDate=" + Server.UrlEncode(Req_sDate));
        }
        //[取得/檢查參數] - eDate
        if (!string.IsNullOrEmpty(Req_eDate))
        {
            search.Add("EndDate", Req_eDate);

            PageParam.Add("eDate=" + Server.UrlEncode(Req_eDate));
        }

        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetProdSample_List(search, StartRow, RecordsPerPage
            , out DataCnt, out ErrMsg);

        //----- 資料整理:取得總筆數 -----
        TotalRow = DataCnt;

        //----- 資料整理:頁數判斷 -----
        if (pageIndex > ((TotalRow / RecordsPerPage) + ((TotalRow % RecordsPerPage) > 0 ? 1 : 0)) && TotalRow > 0)
        {
            StartRow = 0;
            pageIndex = 1;
        }

        //----- 資料整理:繫結 ----- 
        lvDataList.DataSource = query;
        lvDataList.DataBind();


        //----- 資料整理:顯示分頁(放在DataBind之後) ----- 
        if (query.Count() == 0)
        {
            ph_EmptyData.Visible = true;
            ph_Data.Visible = false;

            //Clear
            CustomExtension.setCookie("ProdSample", "", -1);
        }
        else
        {
            ph_EmptyData.Visible = false;
            ph_Data.Visible = true;

            //分頁設定
            string getPager = CustomExtension.Pagination(TotalRow, RecordsPerPage, pageIndex, 5
                , thisPage, PageParam, false, true);

            lt_Pager.Text = getPager;

            //重新整理頁面Url
            string reSetPage = "{0}?page={1}{2}".FormatThis(
                thisPage
                , pageIndex
                , (PageParam.Count == 0 ? "" : "&") + string.Join("&", PageParam.ToArray()));

            //暫存頁面Url, 給其他頁使用
            CustomExtension.setCookie("ProdSample", Server.UrlEncode(reSetPage), 1);

        }

    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        //取得Key值
        //string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

    }


    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                ////判斷是否已加入排程,顯示編輯鈕
                //string _onTask = DataBinder.Eval(dataItem.DataItem, "IsOnTask").ToString();
                //PlaceHolder ph_Edit = (PlaceHolder)e.Item.FindControl("ph_Edit");
                //ph_Edit.Visible = _onTask.Equals("N");


            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 取得類別資料 
    /// </summary>
    /// <param name="cls">類別參數</param>
    /// <param name="ddl">下拉選單object</param>
    /// <param name="rootName">第一選項顯示名稱</param>
    private void LookupClassList(myClass cls, DropDownList ddl, string rootName)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls);

        //----- 資料整理:xx -----
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem(rootName, ""));

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        query = null;
    }


    /// <summary>
    /// 取得人員列表
    /// </summary>
    /// <param name="menu">選單object</param>
    /// <param name="showRoot">是否顯示root</param>
    private void Get_UserList(DropDownListGP menu, bool showRoot)
    {
        //----- 宣告:資料參數 -----
        UsersRepository _users = new UsersRepository();

        //----- 原始資料:取得所有資料 -----
        Dictionary<int, string> deptID = new Dictionary<int, string>();
        deptID.Add(1, "140");
        deptID.Add(2, "170");
        deptID.Add(3, "270");
        deptID.Add(4, "316");
        deptID.Add(5, "240");

        var query = _users.GetUsers(null, deptID);

        //index 0
        if (showRoot)
        {
            menu.Items.Add(new ListItem("所有資料", ""));
        }

        //Item list
        foreach (var item in query)
        {
            //判斷GP_Rank, 若為第一項, 則輸出群組名稱
            if (item.GP_Rank.Equals(1))
            {
                menu.AddItemGroup(item.DeptName);
            }

            //Item Name  
            menu.Items.Add(new ListItem(item.ProfName, item.ProfID));
        }

        //Release
        query = null;
    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// [按鈕] - 查詢
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        //執行查詢
        Response.Redirect(filterUrl(), false);
    }


    #endregion


    #region -- 附加功能 --

    /// <summary>
    /// 含查詢條件的完整網址(新查詢)
    /// </summary>
    /// <returns></returns>
    public string filterUrl()
    {
        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?page=1".FormatThis(thisPage));

        //[查詢條件] - Company
        if (this.filter_Company.SelectedIndex > 0)
        {
            url.Append("&Company=" + Server.UrlEncode(this.filter_Company.SelectedValue));
        }
        //[查詢條件] - Source
        if (this.filter_Source.SelectedIndex > 0)
        {
            url.Append("&Source=" + Server.UrlEncode(this.filter_Source.SelectedValue));
        }
        //[查詢條件] - Check
        if (this.filter_Check.SelectedIndex > 0)
        {
            url.Append("&Check=" + Server.UrlEncode(this.filter_Check.SelectedValue));
        }
        //[查詢條件] - Status
        if (this.filter_Status.SelectedIndex > 0)
        {
            url.Append("&Status=" + Server.UrlEncode(this.filter_Status.SelectedValue));
        }
        //[查詢條件] - Who
        if (this.filter_Who.SelectedIndex > 0)
        {
            url.Append("&Who=" + Server.UrlEncode(this.filter_Who.SelectedValue));
        }

        //[查詢條件] - Keyword
        if (!string.IsNullOrEmpty(this.filter_Keyword.Text))
        {
            url.Append("&Keyword=" + Server.UrlEncode(this.filter_Keyword.Text));
        }

        //[查詢條件] - DateType
        url.Append("&DateType=" + Server.UrlEncode(this.filter_DateType.SelectedValue));

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

        return url.ToString();
    }

    #endregion


    #region -- 網址參數 --

    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}mySample/".FormatThis(
            fn_Param.WebUrl);
    }

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}SampleList.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;

    #endregion


    #region -- 傳遞參數 --

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


    private string _Req_Company;
    public string Req_Company
    {
        get
        {
            String data = Request.QueryString["Company"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Company = value;
        }
    }

    private string _Req_Source;
    public string Req_Source
    {
        get
        {
            String data = Request.QueryString["Source"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Source = value;
        }
    }

    private string _Req_Check;
    public string Req_Check
    {
        get
        {
            String data = Request.QueryString["Check"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Check = value;
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
    /// 取得傳遞參數 - Who
    /// </summary>
    private string _Req_Who;
    public string Req_Who
    {
        get
        {
            String data = Request.QueryString["Who"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Who = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - DateType
    /// </summary>
    private string _Req_DateType;
    public string Req_DateType
    {
        get
        {
            String data = Request.QueryString["DateType"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DateType = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - DateRange
    /// </summary>
    private string _Req_DateRange;
    public string Req_DateRange
    {
        get
        {
            String data = Request.QueryString["DateRange"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DateRange = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - sDate
    /// </summary>
    private string _Req_sDate;
    public string Req_sDate
    {
        get
        {
            String data = Request.QueryString["sDate"];
            return (CustomExtension.String_資料長度Byte(data, "1", "10", out ErrMsg)) ? data : "";
        }
        set
        {
            this._Req_sDate = value;
        }
    }

    /// <summary>
    /// 取得傳遞參數 - eDate
    /// </summary>
    private string _Req_eDate;
    public string Req_eDate
    {
        get
        {
            String data = Request.QueryString["eDate"];
            return (CustomExtension.String_資料長度Byte(data, "1", "10", out ErrMsg)) ? data : "";
        }
        set
        {
            this._Req_eDate = value;
        }
    }

    #endregion


}