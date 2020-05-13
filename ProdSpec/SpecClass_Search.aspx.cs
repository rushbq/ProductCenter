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

public partial class SpecClass_Search : SecurityIn
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
            //[帶出選單] - 大分類
            Get_ClassParent();
            //[帶出選單] - 小分類
            Get_ClassMenu();

            //[取得/檢查參數] - 大分類
            if (fn_Extensions.String_字數(Request.QueryString["ClassID"], "5", "5", out ErrMsg))
            {
                this.ddl_Class.SelectedIndex = this.ddl_Class.Items.IndexOf(
                           this.ddl_Class.Items.FindByValue(fn_stringFormat.Filter_Html(Request.QueryString["ClassID"].ToString().Trim()))
                           );
            }
            //[取得/檢查參數] - 小分類
            if (fn_Extensions.String_字數(Request.QueryString["SpecClass"], "5", "5", out ErrMsg))
            {
                this.ddl_SpecClass.SelectedIndex = this.ddl_SpecClass.Items.IndexOf(
                           this.ddl_SpecClass.Items.FindByValue(fn_stringFormat.Filter_Html(Request.QueryString["SpecClass"].ToString().Trim()))
                           );
            }
            //[取得/檢查參數] - 類別型態
            if (fn_Extensions.String_字數(Request.QueryString["ClassType"], "1", "1", out ErrMsg))
            {
                this.ddl_ClassType.SelectedIndex = this.ddl_ClassType.Items.IndexOf(
                           this.ddl_ClassType.Items.FindByValue(fn_stringFormat.Filter_Html(Request.QueryString["ClassType"].ToString().Trim()))
                           );
            }
            //[取得/檢查參數] - page(頁數)
            int page = 1;
            if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
            {
                page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
            }
            //[代入Ascx參數] - 快速選單
            Ascx_QuickMenu1.Param_CurrItem = "1";

            //[帶出資料]
            LookupDataList(page);

        }
    }

    #region "資料取得"
    /// <summary>
    /// 產生分類選單 - 大分類
    /// </summary>
    private void Get_ClassParent()
    {
        try
        {
            //[初始化]
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT SpecClassID, ClassName_zh_TW ");
                SBSql.AppendLine(" FROM Prod_Spec_Class WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (UpClass IS NULL) ");
                SBSql.AppendLine(" ORDER BY Sort, SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_Class.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_Class.Items.Add(new ListItem("-- 尚無分類資料 --", ""));
                        return;
                    }
                    //輸出選項
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //子項目
                        this.ddl_Class.Items.Add(
                            new ListItem(DT.Rows[row]["SpecClassID"].ToString() + " - " + DT.Rows[row]["ClassName_zh_TW"].ToString()
                            , DT.Rows[row]["SpecClassID"].ToString()));
                    }
                    this.ddl_Class.Items.Insert(0, new ListItem("-- 所有資料 --", ""));
                    this.ddl_Class.SelectedIndex = 0;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
        }
    }

    /// <summary>
    /// 產生分類選單 - 小分類
    /// </summary>
    private void Get_ClassMenu()
    {
        try
        {
            //[初始化]
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT lv1.SpecClassID AS MtClassID, lv1.ClassName_zh_TW AS MtClassName ");
                SBSql.AppendLine("  , Lv2.SpecClassID AS SubClassID, Lv2.ClassName_zh_TW AS SubClassName ");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY lv1.ClassName_zh_TW ORDER BY lv1.Sort, lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ASC) AS GP_Rank ");
                SBSql.AppendLine(" FROM Prod_Spec_Class AS Lv1 ");
                SBSql.AppendLine("  LEFT JOIN Prod_Spec_Class AS Lv2 ON Lv1.SpecClassID = Lv2.UpClass ");
                SBSql.AppendLine(" WHERE (Lv1.Display = 'Y') AND (Lv1.UpClass IS NULL) AND (Lv2.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Lv1.Sort, Lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_SpecClass.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_SpecClass.Items.Add(new ListItem("-- 尚無分類資料 --", ""));
                        return;
                    }
                    //輸出選項
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //判斷GP_Rank, 若為第一項，則輸出群組名稱
                        if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                        {
                            this.ddl_SpecClass.AddItemGroup(DT.Rows[row]["MtClassID"].ToString() + " - " + DT.Rows[row]["MtClassName"].ToString());
                            //this.ddl_SpecClass.Items.Add(
                            //   new ListItem(DT.Rows[row]["MtClassID"].ToString() + " - " + DT.Rows[row]["MtClassName"].ToString()
                            //   , DT.Rows[row]["MtClassID"].ToString()));
                        }
                        //子項目
                        this.ddl_SpecClass.Items.Add(
                            new ListItem(DT.Rows[row]["SubClassID"].ToString() + " - " + DT.Rows[row]["SubClassName"].ToString()
                            , DT.Rows[row]["SubClassID"].ToString()));
                    }
                    this.ddl_SpecClass.Items.Insert(0, new ListItem("-- 所有資料 --", ""));
                    this.ddl_SpecClass.SelectedIndex = 0;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
        }
    }

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
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "ProdSpec/SpecClass_Search.aspx?func=Spec";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string ClassID = fn_stringFormat.Filter_Html(this.ddl_Class.SelectedValue);
            string SpecClassID = fn_stringFormat.Filter_Html(this.ddl_SpecClass.SelectedValue);
            string ClassType = fn_stringFormat.Filter_Html(this.ddl_ClassType.SelectedValue);

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("   SELECT ");
            SBSql.AppendLine("      SpecClassID, ClassName_zh_TW, UpClass, Display, Sort ");
            SBSql.AppendLine("      , (SELECT (SpecClassID + ' - ' + ClassName_zh_TW) FROM Prod_Spec_Class UpCs WHERE (UpCs.SpecClassID = Prod_Spec_Class.UpClass) ) AS UpClassName ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Sort, SpecClassID) AS RowRank ");
            SBSql.AppendLine("   FROM Prod_Spec_Class ");
            SBSql.AppendLine("   WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 大分類
            if (this.ddl_Class.SelectedIndex > 0)
            {
                SBSql.Append(" AND (UpClass = @UpClass) ");

                cmd.Parameters.AddWithValue("UpClass", ClassID);

                this.ViewState["Page_LinkStr"] += "&ClassID=" + Server.UrlEncode(ClassID);
            }
            //[查詢條件] - 小分類
            if (this.ddl_SpecClass.SelectedIndex > 0)
            {
                SBSql.Append(" AND (SpecClassID = @SpecClassID) ");

                cmd.Parameters.AddWithValue("SpecClassID", SpecClassID);

                this.ViewState["Page_LinkStr"] += "&SpecClass=" + Server.UrlEncode(SpecClassID);
            }
            //[查詢條件] - 類別型態
            if (this.ddl_ClassType.SelectedIndex > 0)
            {
                switch (ClassType)
                {
                    case "1":
                        SBSql.Append(" AND (UpClass IS NULL) ");
                        break;

                    case "2":
                        SBSql.Append(" AND (UpClass IS NOT NULL) ");
                        break;
                }
                this.ViewState["Page_LinkStr"] += "&ClassType=" + Server.UrlEncode(ClassType);
            }
            #endregion

            SBSql.AppendLine("  ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Prod_Spec_Class ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - 大分類
            if (this.ddl_Class.SelectedIndex > 0)
            {
                SBSql.Append(" AND (UpClass = @UpClass) ");

                cmdTotalCnt.Parameters.AddWithValue("UpClass", ClassID);
            }
            //[查詢條件] - 小分類
            if (this.ddl_SpecClass.SelectedIndex > 0)
            {
                SBSql.Append(" AND (SpecClassID = @SpecClassID) ");

                cmdTotalCnt.Parameters.AddWithValue("SpecClassID", SpecClassID);
            }
            //[查詢條件] - 類別型態
            if (this.ddl_ClassType.SelectedIndex > 0)
            {
                switch (ClassType)
                {
                    case "1":
                        SBSql.Append(" AND (UpClass IS NULL) ");
                        break;

                    case "2":
                        SBSql.Append(" AND (UpClass IS NOT NULL) ");
                        break;
                }
            }
            #endregion

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();

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
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"] + "&page=" + this.ViewState["page"];
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

                //[判斷] - 是否顯示
                RadioButtonList rbl_Display = (RadioButtonList)e.Item.FindControl("rbl_Display");
                rbl_Display.SelectedIndex =
                    rbl_Display.Items.IndexOf(
                    rbl_Display.Items.FindByValue(DataBinder.Eval(dataItem.DataItem, "Display").ToString()));

                //[判斷] - 大分類不設定關聯ph_SetRel
                if (string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "UpClass").ToString()))
                {
                    ((PlaceHolder)e.Item.FindControl("ph_SetRel")).Visible = false;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - ItemDataBound！", "");
        }

    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    string ErrMsg;

                    //取得編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_SpecClassID")).Value;

                    //[SQL] - 清除參數設定
                    cmd.Parameters.Clear();
                    StringBuilder SBSql = new StringBuilder();

                    //[SQL] - 檢查是否已被關聯
                    SBSql.AppendLine(" DECLARE @RowNum1 AS INT, @RowNum2 AS INT, @RowNum3 AS INT ");
                    SBSql.AppendLine(" SET @RowNum1 = (SELECT COUNT(*) FROM Prod_SpecClass_Rel_Spec WHERE (SpecClassID = @Param_SpecClassID)) ");
                    SBSql.AppendLine(" SET @RowNum2 = (SELECT COUNT(*) FROM Prod_Spec_List WHERE (SpecClassID = @Param_SpecClassID)) ");
                    SBSql.AppendLine(" SET @RowNum3 = (SELECT COUNT(*) FROM Prod_Item WHERE (SpecClassID = @Param_SpecClassID)) ");
                    SBSql.AppendLine(" IF(@RowNum1 + @RowNum2 + @RowNum3) > 0 ");
                    SBSql.AppendLine("  SELECT 'Y' AS IsRel ");
                    SBSql.AppendLine(" ELSE ");
                    SBSql.AppendLine("  SELECT 'N' AS IsRel ");
                    cmd.Parameters.AddWithValue("Param_SpecClassID", GetDataID);
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows[0]["IsRel"].Equals("Y"))
                        {
                            fn_Extensions.JsAlert("無法刪除，此分類已被使用！", this.ViewState["Page_LinkStr"].ToString());
                            return;
                        }
                    }

                    //[SQL] - 刪除資料
                    SBSql.Clear();
                    SBSql.AppendLine(" DELETE FROM Prod_Spec_Class WHERE (SpecClassID = @Param_SpecClassID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_SpecClassID", GetDataID);
                    if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料刪除失敗！", "");
                    }
                    else
                    {
                        fn_Extensions.JsAlert("資料刪除成功！", this.ViewState["Page_LinkStr"].ToString() + "&page=" + this.ViewState["page"].ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - ItemCommand！", "");
        }

    }

    #endregion

    #region "前端頁面控制"
    /// <summary>
    /// 下拉選單 - 分頁跳轉
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    /// <summary>
    /// 按鈕 - 搜尋
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("SpecClass_Search.aspx?func=Spec");

            //[查詢條件] - 大分類
            if (this.ddl_Class.SelectedIndex > 0)
            {
                SBUrl.Append("&ClassID=" + Server.UrlEncode(this.ddl_Class.SelectedValue));
            }

            //[查詢條件] - 小分類
            if (this.ddl_SpecClass.SelectedIndex > 0)
            {
                SBUrl.Append("&SpecClass=" + Server.UrlEncode(this.ddl_SpecClass.SelectedValue));
            }

            //[取得/檢查參數] - 類別型態
            if (this.ddl_ClassType.SelectedIndex > 0)
            {
                SBUrl.Append("&ClassType=" + Server.UrlEncode(this.ddl_ClassType.SelectedValue));
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

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                StringBuilder SBSql = new StringBuilder();
                for (int row = 0; row < lvDataList.Items.Count; row++)
                {
                    //[取得參數] - 編號
                    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_SpecClassID")).Value;
                    //[取得參數] - 顯示
                    string lvParam_Disp = ((RadioButtonList)this.lvDataList.Items[row].FindControl("rbl_Display")).SelectedValue;
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Sort")).Text.Trim();

                    SBSql.AppendLine(" UPDATE Prod_Spec_Class SET ");
                    SBSql.AppendLine(string.Format(
                        " Display = @lvParam_Disp_{0}, Sort = @lvParam_Sort_{0} " +
                        " WHERE (SpecClassID = @lvParam_ID_{0}); "
                        , row));

                    cmd.Parameters.AddWithValue("lvParam_ID_" + row, lvParam_ID);
                    cmd.Parameters.AddWithValue("lvParam_Disp_" + row, lvParam_Disp);
                    cmd.Parameters.AddWithValue("lvParam_Sort_" + row, lvParam_Sort);
                }
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", "");
                }
                else
                {
                    fn_Extensions.JsAlert("資料儲存成功！", this.ViewState["Page_LinkStr"].ToString());
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存設定！", "");
        }
    }
    #endregion

}
