using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProdAttributeData.Controllers;
using PKLib_Method.Methods;


public partial class ProdProp_Search : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("160", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
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
        //----- 宣告:網址參數 -----
        int RecordsPerPage = 20;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        int DataCnt = 0;
        ArrayList PageParam = new ArrayList();  //分類暫存條件參數

        //----- 宣告:資料參數 -----
        ProdItemPropRespository _data = new ProdItemPropRespository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        try
        {
            #region >> 條件篩選 <<
            //Params
            string _ItemNo = Req_ItemNo;
            string _ModelNo = Req_ModelNo;


            //[查詢條件] - ModelNo
            if (!string.IsNullOrWhiteSpace(_ItemNo))
            {
                search.Add("ItemNo", _ItemNo);
                PageParam.Add("itemno=" + Server.UrlEncode(_ItemNo));
                filter_ItemNo.Text = _ItemNo;
            }

            //[查詢條件] - ModelNo
            if (!string.IsNullOrWhiteSpace(_ModelNo))
            {
                search.Add("ModelNo", _ModelNo);
                PageParam.Add("modelno=" + Server.UrlEncode(_ModelNo));
                filter_ModelNo.Text = _ModelNo;
            }
            #endregion

            //----- 原始資料:取得所有資料 -----
            var query = _data.Get_ProdAttrList(search, StartRow, RecordsPerPage, true
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
            if (query.Rows.Count == 0)
            {
                ph_EmptyData.Visible = true;
                ph_Data.Visible = false;

                //Clear
                CustomExtension.setCookie("ProdProp", "", -1);
            }
            else
            {
                ph_EmptyData.Visible = false;
                ph_Data.Visible = true;

                //分頁設定
                string getPager = CustomExtension.Pagination(TotalRow, RecordsPerPage, pageIndex, 5
                    , thisPage, PageParam, false, true);

                Literal lt_Pager = (Literal)this.lvDataList.FindControl("lt_Pager");
                lt_Pager.Text = getPager;

                //重新整理頁面Url
                string reSetPage = "{0}?page={1}{2}".FormatThis(
                    thisPage
                    , pageIndex
                    , (PageParam.Count == 0 ? "" : "&") + string.Join("&", PageParam.ToArray()));

                //暫存頁面Url, 給其他頁使用
                CustomExtension.setCookie("ProdProp", Server.UrlEncode(reSetPage), 1);

            }
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _data = null;
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
                //Declare
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                ////Get Data
                //string _Remk_Normal = DataBinder.Eval(dataItem.DataItem, "Remk_Normal").ToString();
                //string _ID = DataBinder.Eval(dataItem.DataItem, "Data_ID").ToString();
                //int _pdfCnt = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "UpdFormCnt"));

                ////Remark controller
                //PlaceHolder ph_Modal_r1 = (PlaceHolder)e.Item.FindControl("ph_Modal_r1");
                //ph_Modal_r1.Visible = !string.IsNullOrWhiteSpace(_Remk_Normal);
                //Label lb_showMark = (Label)e.Item.FindControl("lb_showMark");
                //lb_showMark.Visible = !string.IsNullOrWhiteSpace(_Remk_Normal);
                //PlaceHolder ph_RemarkSection = (PlaceHolder)e.Item.FindControl("ph_RemarkSection");
                //ph_RemarkSection.Visible = !string.IsNullOrWhiteSpace(_Remk_Normal);


            }
        }
        catch (Exception)
        {
            throw;
        }
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
        //Params
        string _ModelNo = this.filter_ModelNo.Text;
        string _ItemNo = this.filter_ItemNo.Text;

        //url string
        StringBuilder url = new StringBuilder();

        //固定條件:Page
        url.Append("{0}?page=1".FormatThis(thisPage));

        //[查詢條件] - ModelNo
        if (!string.IsNullOrWhiteSpace(_ModelNo))
        {
            url.Append("&modelNo=" + Server.UrlEncode(_ModelNo));
        }

        //[查詢條件] - ItemNo
        if (!string.IsNullOrWhiteSpace(_ItemNo))
        {
            url.Append("&itemNo=" + Server.UrlEncode(_ItemNo));
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
        return "{0}myProd/".FormatThis(fn_Param.WebUrl);
    }

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
    /// 取得傳遞參數 - ModelNo
    /// </summary>
    public string Req_ModelNo
    {
        get
        {
            String _data = Request.QueryString["modelno"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "20", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }
    private string _Req_ModelNo;


    /// <summary>
    /// 取得傳遞參數 - ItemNo
    /// </summary>
    public string Req_ItemNo
    {
        get
        {
            String _data = Request.QueryString["itemno"];
            return (CustomExtension.String_資料長度Byte(_data, "1", "20", out ErrMsg)) ? _data.Trim() : "";
        }
        set
        {
            this._Req_ItemNo = value;
        }
    }
    private string _Req_ItemNo;


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}ProdProp_Search.aspx".FormatThis(FuncPath());
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;

    #endregion
}