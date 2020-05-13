using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using Resources;
using System.Collections;

public partial class Spec_Rel_ProdItem : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";

            //[權限判斷] - 規格設定
            if (fn_CheckAuth.CheckAuth_User("102", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[取得/檢查參數] - 品號
            if (fn_Extensions.String_字數(Request.QueryString["ModelNo"], "1", "40", out ErrMsg))
            {
                this.tb_ModelNo.Text = fn_stringFormat.Filter_Html(Request.QueryString["ModelNo"].ToString().Trim());
                this.val_ModelNo.Text = fn_stringFormat.Filter_Html(Request.QueryString["ModelNo"].ToString().Trim());

                //判斷分類是否有選擇
                if (false == string.IsNullOrEmpty(this.val_ModelNo.Text))
                {
                    this.ph_Search.Visible = true;
                    this.ph_List.Visible = true;
                }
            }
            else
            {
                this.ph_Search.Visible = false;
                this.ph_List.Visible = false;
            }

            //[取得/檢查參數] - page(頁數)
            int page = 1;
            if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
            {
                page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
            }
            //[取得/檢查參數] - 關鍵字
            if (fn_Extensions.String_字數(Request.QueryString["Keyword"], "1", "50", out ErrMsg))
            {
                this.tb_Keyword.Text = fn_stringFormat.Filter_Html(Request.QueryString["Keyword"].ToString().Trim());
            }
            //[取得/檢查參數] - 選單單頭
            if (fn_Extensions.String_字數(Request.QueryString["GID"], "1", "5", out ErrMsg))
            {
                this.tb_GID.Text = fn_stringFormat.Filter_Html(Request.QueryString["GID"].ToString().Trim().ToUpper());
            }
            //[代入Ascx參數] - 快速選單
            Ascx_QuickMenu1.Param_CurrItem = "5";

            //[帶出資料]
            LookupDataList(page);

        }
    }

    #region "資料取得"
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="page">頁數</param>
    private void LookupDataList(int page)
    {
        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        string ErrMsg;
        try
        {
            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string Keyword = this.tb_Keyword.Text;
            string GID = this.tb_GID.Text;
            string Model_No = this.val_ModelNo.Text;

            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "ProdSpec/Spec_Rel_ProdItem.aspx?func=Rel&ModelNo=" + Server.UrlEncode(Model_No);

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("   SELECT ");
            SBSql.AppendLine("      Rel.Model_No AS Model_No ");
            SBSql.AppendLine("      , Prod_Spec.SpecID, SpecName_zh_TW, SpecType, IsRequired, OptionGID, SpecDESC ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Prod_Spec.SpecID) AS RowRank ");
            SBSql.AppendLine("   FROM Prod_Spec LEFT JOIN Prod_Item_Rel_Spec AS Rel ");
            SBSql.AppendLine("      ON Prod_Spec.SpecID = Rel.SpecID AND Rel.Model_No = @Model_No ");
            SBSql.AppendLine("   WHERE (Prod_Spec.Display = 'Y') ");

            #region "查詢條件"
            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(Prod_Spec.SpecID) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Spec.SpecName_zh_TW) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }
            //[查詢條件] - GID
            if (string.IsNullOrEmpty(GID) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(Prod_Spec.OptionGID) LIKE '%' + UPPER(@GID) + '%') ");
                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("GID", GID);

                this.ViewState["Page_LinkStr"] += "&GID=" + Server.UrlEncode(GID);
            }
            #endregion

            SBSql.AppendLine("  ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
            cmd.Parameters.AddWithValue("Model_No", Model_No);

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine("   FROM Prod_Spec LEFT JOIN Prod_Item_Rel_Spec AS Rel ");
            SBSql.AppendLine("      ON Prod_Spec.SpecID = Rel.SpecID AND Rel.Model_No = @Model_No ");
            SBSql.AppendLine(" WHERE (Prod_Spec.Display = 'Y') ");

            #region "查詢條件"
            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(Prod_Spec.SpecID) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Spec.SpecName_zh_TW) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
            }
            //[查詢條件] - GID
            if (string.IsNullOrEmpty(GID) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(Prod_Spec.OptionGID) LIKE '%' + UPPER(@GID) + '%') ");
                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("GID", GID);
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            cmdTotalCnt.Parameters.AddWithValue("Model_No", Model_No);
            //[SQL] - 取得資料
            using (DataTable DT = dbConClass.LookupDTwithPage(cmd, cmdTotalCnt, out TotalCnt, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    //判斷是否為頁碼過大, 帶往最後一頁
                    if (TotalCnt > 0 & BgItem > TotalCnt)
                    {
                        if (TotalCnt % PageSize == 0)
                        {
                            TotalPage = Convert.ToInt16(TotalCnt / PageSize);
                        }
                        else
                        {
                            TotalPage = Convert.ToInt16(Math.Floor((double)TotalCnt / PageSize)) + 1;
                        }
                        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + TotalPage);
                    }
                    else
                    {
                        //隱藏分頁
                        this.pl_Page.Visible = false;
                    }
                }
                else
                {
                    #region "分頁控制"
                    //計算總頁數
                    if (TotalCnt % PageSize == 0)
                    {
                        TotalPage = Convert.ToInt16(TotalCnt / PageSize);
                    }
                    else
                    {
                        TotalPage = Convert.ToInt16(Math.Floor((double)TotalCnt / PageSize)) + 1;
                    }
                    //判斷頁數
                    if (page < 1)
                        page = 1;
                    if (page > TotalPage)
                        page = TotalPage;
                    //一次n頁的頁碼
                    int PageTen = 0;
                    if (page % PageRoll == 0)
                        PageTen = page;
                    else
                        PageTen = (Convert.ToInt16(Math.Floor((double)page / PageRoll)) + 1) * PageRoll;
                    //帶入頁數資料
                    int FirstItem = (page - 1) * PageSize + 1;
                    int LastItem = FirstItem + (PageSize - 1);
                    if (LastItem > TotalCnt)
                        LastItem = TotalCnt;
                    //填入頁數資料
                    int i = 0;
                    for (i = 1; i <= TotalPage; i++)
                    {
                        this.ddl_Page_List.Items.Insert(i - 1, Convert.ToString(i));
                        this.ddl_Page_List.Items[i - 1].Value = Convert.ToString(i);
                    }
                    this.ddl_Page_List.SelectedValue = Convert.ToString(page); //頁碼下拉選單
                    this.lt_TotalPage.Text = Convert.ToString(TotalPage);  // n 頁
                    this.lt_Page_DataCntInfo.Text = "第 " + FirstItem + " - " + LastItem + " 筆，共 " + TotalCnt + " 筆";

                    //[分頁] - 顯示頁碼
                    StringBuilder sb = new StringBuilder();

                    //[頁碼] - 第一頁
                    if (page >= 2)
                    {
                        sb.AppendFormat("<a href=\"{0}&page=1\" class=\"PagePre\">第一頁</a>", this.ViewState["Page_LinkStr"]);
                    }
                    //[頁碼] - 數字頁碼
                    sb.AppendLine("<div class=\"Pages\">");

                    for (i = PageTen - (PageRoll - 1); i <= PageTen; i++)
                    {
                        if (i > TotalPage)
                            break;
                        if (i == page)
                        {
                            sb.AppendFormat("<span>{0}</span>", i);
                        }
                        else
                        {
                            sb.AppendFormat("<a href=\"{0}&page={1}\">{1}</a>", this.ViewState["Page_LinkStr"], i);
                        }
                    }
                    sb.AppendLine("</div>");

                    //[頁碼] - 最後1頁
                    if (page < TotalPage)
                    {
                        sb.AppendFormat("<a href=\"{0}&page={1}\" class=\"PageNext\">{2}</a>", this.ViewState["Page_LinkStr"], TotalPage, "最後一頁");
                    }
                    //顯示分頁
                    this.pl_Page.Visible = true;
                    this.lt_Page_Link.Text = sb.ToString();

                    #endregion
                }

                //暫存目前頁碼 
                this.ViewState["page"] = page;
                //暫存目前Url
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"];
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 資料列表！", "");
        }
        finally
        {
            if (cmd != null)
                cmd.Dispose();
            if (cmdTotalCnt != null)
                cmdTotalCnt.Dispose();
        }

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;

                //[判斷參數] - 是否勾選
                CheckBox cb_Item = (CheckBox)e.Item.FindControl("cb_Item");
                if (false == string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "Model_No").ToString()))
                {
                    cb_Item.Checked = true;

                    //[設定參數] - 將已勾選填入編號
                    HiddenField hf_CheckedID = (HiddenField)e.Item.FindControl("hf_CheckedID");
                    hf_CheckedID.Value = DataBinder.Eval(dataItem.DataItem, "SpecID").ToString();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - ItemDataBound！", "");
        }

    }

    #endregion

    #region "前端頁面控制"

    //分頁跳轉
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    /// <summary>
    /// 按鈕 - 選擇品號
    /// </summary>
    protected void btn_ModelNo_Click(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Spec_Rel_ProdItem.aspx?func=Rel&ModelNo=" + Server.UrlEncode(this.val_ModelNo.Text));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 選擇品號！", "");
        }
    }

    /// <summary>
    /// 按鈕 - 搜尋
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Spec_Rel_ProdItem.aspx?func=Rel&ModelNo=" + Server.UrlEncode(this.val_ModelNo.Text));

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text));
            }
            //[查詢條件] - 選單單頭
            if (string.IsNullOrEmpty(this.tb_GID.Text) == false)
            {
                SBUrl.Append("&GID=" + Server.UrlEncode(this.tb_GID.Text));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    /// <summary>
    /// 按鈕 - 儲存設定
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("目前尚無資料可設定！", "");
                return;
            }

            //[暫存參數] - 原本已勾選的項目
            ArrayList aryCheckItem = new ArrayList();
            //[暫存參數] - 目前勾選的項目
            ArrayList aryItem = new ArrayList();
            for (int row = 0; row < lvDataList.Items.Count; row++)
            {
                //[取得參數] - 原本已勾選的編號
                string lvChecked_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_CheckedID")).Value;
                if (false == string.IsNullOrEmpty(lvChecked_ID))
                {
                    aryCheckItem.Add(lvChecked_ID);
                }

                //[取得參數] - 資料編號
                string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_SpecID")).Value;
                CheckBox cbItem = (CheckBox)this.lvDataList.Items[row].FindControl("cb_Item");
                if (cbItem.Checked)
                {
                    aryItem.Add(lvParam_ID);
                }
            }

            /*
             * 處理關聯
             *  - 刪除本頁暫存已勾選項目
             *  - 新增本頁目前勾選項目
             */
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 刪除原勾選項目
                if (aryCheckItem.Count > 0)
                {
                    SBSql.AppendLine(" DELETE FROM Prod_Item_Rel_Spec WHERE (Model_No = @Model_No) ");

                    //組合參數字串
                    ArrayList aryParam = new ArrayList();
                    for (int row = 0; row < aryCheckItem.Count; row++)
                    {
                        aryParam.Add("@ParCheckedID_" + row);
                    }
                    SBSql.AppendLine(" AND SpecID IN (" + string.Join(",", aryParam.ToArray()) + "); ");

                    //代入參數值
                    for (int row = 0; row < aryCheckItem.Count; row++)
                    {
                        cmd.Parameters.AddWithValue("ParCheckedID_" + row, aryCheckItem[row]);
                    }
                }
                //[SQL] - 新增目前勾選項目
                for (int row = 0; row < aryItem.Count; row++)
                {
                    SBSql.AppendLine(" INSERT INTO Prod_Item_Rel_Spec (Model_No, SpecID) ");
                    SBSql.AppendLine(string.Format(" VALUES (@Model_No, @Param_SpecID{0}); ", row));

                    cmd.Parameters.AddWithValue("Param_SpecID" + row, aryItem[row]);
                }
                cmd.Parameters.AddWithValue("Model_No", this.val_ModelNo.Text);
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("關聯設定失敗！", "");
                }
                else
                {
                    fn_Extensions.JsAlert("關聯設定成功！"
                        , this.ViewState["Page_LinkStr"].ToString() + "&page=" + this.ViewState["page"]);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定關聯！", "");
        }
    }
    #endregion

}
