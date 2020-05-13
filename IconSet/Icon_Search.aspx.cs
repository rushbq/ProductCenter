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
using ExtensionIO;


public partial class Icon_Search : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";
            //[權限判斷] - 符號資料庫
            if (fn_CheckAuth.CheckAuth_User("401", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }
            //[取得資料] - 符號使用者
            if (fn_Extensions.IconUseMenu(this.ddl_Icon_Type, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("符號使用者選單產生失敗！", "");
            }
            this.ddl_Icon_Type.Items.Insert(0, new ListItem("-- 所有資料 --", ""));
            this.ddl_Icon_Type.SelectedIndex = 0;

            //[取得/檢查參數] - 符號使用者
            if (fn_Extensions.String_字數(Request.QueryString["Icon_Type"], "1", "20", out ErrMsg))
            {
                this.ddl_Icon_Type.SelectedIndex = this.ddl_Icon_Type.Items.IndexOf(
                           this.ddl_Icon_Type.Items.FindByValue(Request.QueryString["Icon_Type"].ToString().Trim())
                           );
            }
            //[取得/檢查參數] - 關鍵字
            if (fn_Extensions.String_字數(Request.QueryString["Keyword"], "1", "50", out ErrMsg))
            {
                this.tb_Keyword.Text = fn_stringFormat.Filter_Html(Request.QueryString["Keyword"].ToString().Trim());
            }

            //[取得/檢查參數] - page(頁數)
            int page = 1;
            if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
            {
                page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
            }

            //[帶出資料]
            LookupDataList(page);
        }

    }

    #region -- 資料取得 --
    /// <summary>
    /// 副程式 - 取得資料列表 (分頁)
    /// </summary>
    /// <param name="page">頁數</param>
    private void LookupDataList(int page)
    {
        //[參數宣告] - 共用參數
        SqlCommand cmd = new SqlCommand();
        SqlCommand cmdTotalCnt = new SqlCommand();
        string ErrMsg = "";
        try
        {
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "IconSet/Icon_Search.aspx?func=IconSet";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數

            //[取得參數]
            string Keyword = this.tb_Keyword.Text;
            string IconType = this.ddl_Icon_Type.SelectedValue;

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      Icon_ID, CID, Display, Sort, Icon_Type, Parm.Param_Name AS Icon_TypeName ");
            SBSql.AppendLine("      , IconName_zh_TW, IconName_zh_CN, IconName_en_US");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Icon.Sort, CID) AS RowRank ");
            //[SQL] - 計數, 是否有設定明細
            SBSql.AppendLine("      , (SELECT COUNT(*) FROM Icon_Pics Rel WHERE (Rel.Icon_ID = Icon.Icon_ID)) AS RelCnt ");
            SBSql.AppendLine("    FROM Icon INNER JOIN Param_Public Parm ON Icon.Icon_Type = Parm.Param_Value AND Parm.Param_Kind = '符號使用者' ");
            SBSql.AppendLine("    WHERE (1 = 1) ");
            #region "查詢條件"
            //[查詢條件] - 符號使用者
            if (this.ddl_Icon_Type.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Icon_Type = @Icon_Type) ");
                cmd.Parameters.AddWithValue("Icon_Type", IconType);

                this.ViewState["Page_LinkStr"] += "&Icon_Type=" + Server.UrlEncode(IconType);
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(IconName_zh_TW) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(IconName_zh_CN) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(IconName_en_US) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append(" )");
                cmd.Parameters.AddWithValue("Keyword", Keyword);

                this.ViewState["Page_LinkStr"] += "&Keyword=" + Server.UrlEncode(Keyword);
            }

            #endregion

            SBSql.AppendLine("       ) AS TBL ");
            SBSql.AppendLine(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.AppendLine(" ORDER BY RowRank ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);

            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.AppendLine(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.AppendLine(" FROM Icon WHERE (1=1) ");

            #region "查詢條件"
            //[查詢條件] - 符號使用者
            if (this.ddl_Icon_Type.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Icon_Type = @Icon_Type) ");
                cmdTotalCnt.Parameters.AddWithValue("Icon_Type", IconType);
            }

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(IconName_zh_TW) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(IconName_zh_CN) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append("  OR (UPPER(IconName_en_US) LIKE '%' + UPPER(@Keyword) + '%') ");
                SBSql.Append(" )");
                cmdTotalCnt.Parameters.AddWithValue("Keyword", Keyword);
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
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"] + "&page=" + page;
                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 取得資料列表！", "");
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
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //[判斷] - 是否顯示
            RadioButtonList rbl_Display = (RadioButtonList)e.Item.FindControl("rbl_Display");
            rbl_Display.SelectedIndex =
                rbl_Display.Items.IndexOf(
                rbl_Display.Items.FindByValue(DataBinder.Eval(dataItem.DataItem, "Display").ToString()));

            //[判斷] - 是否有設定關聯, 顯示所屬分類
            if (Convert.ToInt16(DataBinder.Eval(dataItem.DataItem, "RelCnt")) > 0)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    //[初始化]
                    string ErrMsg;
                    Literal lt_Pics = (Literal)e.Item.FindControl("lt_Pics");
                    lt_Pics.Text = "";

                    //取得編號
                    string Icon_ID = DataBinder.Eval(dataItem.DataItem, "Icon_ID").ToString();

                    //[SQL] - 清除參數設定
                    cmd.Parameters.Clear();

                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine(" SELECT Pic_File ");
                    SBSql.AppendLine(" FROM Icon_Pics ");
                    SBSql.AppendLine(" WHERE (Icon_ID = @Icon_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Icon_ID", Icon_ID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count > 0)
                        {
                            //顯示關聯項目
                            for (int row = 0; row < DT.Rows.Count; row++)
                            {
                                lt_Pics.Text += string.Format(
                                    "<a rel=\"PicGroup\" class=\"PicGroup\" href=\"{0}\"><img src=\"{0}\" width=\"50px\" border=\"0\"></a>"
                                    , Param_WebFolder + DT.Rows[row]["Pic_File"].ToString()
                                 );
                            }
                        }

                    }
                }
            }
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
                    StringBuilder SBSql = new StringBuilder();

                    //取得編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_Icon_ID")).Value;
                    string Icon_Type = ((HiddenField)e.Item.FindControl("hf_Icon_Type")).Value;

                    //[SQL] - 檢查是否已被關聯
                    SBSql.AppendLine(" DECLARE @RowNum1 AS INT,@RowNum2 AS INT ");
                    SBSql.AppendLine(" SET @RowNum1 = (SELECT COUNT(*) FROM Icon_Rel_Certification WHERE (Pic_ID = @Param_ID)) ");
                    SBSql.AppendLine(" SET @RowNum2 = (SELECT COUNT(*) FROM Icon_Rel_Spec WHERE (Pic_ID = @Param_ID)) ");
                    SBSql.AppendLine(" IF(@RowNum1 > 0 OR @RowNum2> 0) ");
                    SBSql.AppendLine("  SELECT 'Y' AS IsRel ");
                    SBSql.AppendLine(" ELSE ");
                    SBSql.AppendLine("  SELECT 'N' AS IsRel ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows[0]["IsRel"].Equals("Y"))
                        {
                            fn_Extensions.JsAlert("無法刪除，此項目已被使用！", "");
                            return;
                        }
                    }

                    //取得檔案
                    List<string> ListFiles = new List<string>();

                    SBSql.Clear();
                    SBSql.AppendLine("SELECT Pic_File FROM Icon_Pics WHERE (Icon_ID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            ListFiles.Add(DT.Rows[row]["Pic_File"].ToString());
                        }
                    }

                    //[SQL] - 刪除資料
                    SBSql.Clear();
                    SBSql.AppendLine(" DELETE FROM Icon_Pics WHERE (Icon_ID = @Param_ID) ");
                    SBSql.AppendLine(" DELETE FROM Icon WHERE (Icon_ID = @Param_ID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                    if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        fn_Extensions.JsAlert("資料刪除失敗！", "");
                    }
                    else
                    {
                        //刪除檔案
                        for (int row = 0; row < ListFiles.Count; row++)
                        {
                            IOManage.DelFile(Param_FileFolder + Icon_Type + @"\", ListFiles[row].ToString());
                        }

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

    #region -- 前端頁面控制 --
    //分頁跳轉
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(this.ViewState["Page_LinkStr"] + "&page=" + this.ddl_Page_List.SelectedValue);
    }

    //搜尋
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Icon_Search.aspx?func=IconSet");

            //[查詢條件] - 符號使用者
            if (this.ddl_Icon_Type.SelectedIndex > 0)
            {
                SBUrl.Append("&Icon_Type=" + Server.UrlEncode(this.ddl_Icon_Type.SelectedValue));
            }
            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
            {
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text.Trim()));
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
                    string lvParam_ID = ((HiddenField)this.lvDataList.Items[row].FindControl("hf_Icon_ID")).Value;
                    //[取得參數] - 顯示
                    string lvParam_Disp = ((RadioButtonList)this.lvDataList.Items[row].FindControl("rbl_Display")).SelectedValue;
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[row].FindControl("tb_Sort")).Text.Trim();

                    SBSql.AppendLine(" UPDATE Icon SET ");
                    SBSql.AppendLine(string.Format(
                        " Display = @lvParam_Disp_{0}, Sort = @lvParam_Sort_{0} " +
                        " WHERE (Icon_ID = @lvParam_ID_{0}); "
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

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"Icons\";
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"Icons/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }
    #endregion
}