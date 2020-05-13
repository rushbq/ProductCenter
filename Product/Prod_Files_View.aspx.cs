using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using ExtensionMethods;
using Resources;


public partial class Prod_Files_View : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 產品資料
                if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Product/Prod_Search.aspx";



                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //檔案列表
                LookupDataList(page);

                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = "3";
                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
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
        string ErrMsg;
        try
        {
            //[參數宣告] - 筆數/分頁設定
            int PageSize = 10;  //每頁筆數
            int PageRoll = 10;  //一次顯示10頁
            int BgItem = (page - 1) * PageSize + 1;  //開始筆數
            int EdItem = BgItem + (PageSize - 1);  //結束筆數
            int TotalCnt = 0;  //總筆數
            int TotalPage = 0;  //總頁數


            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            cmdTotalCnt.Parameters.Clear();

            //[SQL] - 資料查詢
            StringBuilder SBSql = new StringBuilder();

            SBSql.Append(" SELECT TBL.* ");
            SBSql.Append(" FROM ( ");
            SBSql.Append("    SELECT ");
            SBSql.Append("      Base.SeqNo, Base.File_ID, Base.FileName, Base.DisplayName");
            SBSql.Append("      , Cls.Class_Name, Cls.Class_ID");
            SBSql.Append("      , FType.Class_Name AS FileTypeName, FTarget.Class_Name AS TargetName, Lang.Class_Name AS LangName");
            SBSql.Append("      , ISNULL(Base.Update_Time, Base.Create_Time) AS MtTime");
            SBSql.Append("      , ROW_NUMBER() OVER (ORDER BY Cls.Sort, FType.Sort) AS RowRank ");
            SBSql.Append("    FROM File_List Base ");
            SBSql.Append("      INNER JOIN File_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = 'zh-tw'");
            SBSql.Append("      INNER JOIN File_LangType Lang ON Base.LangType_ID = Lang.Class_ID");
            SBSql.Append("      INNER JOIN File_Type FType ON Base.FileType_ID = FType.Class_ID AND LOWER(FType.LangCode) = 'zh-tw'");
            SBSql.Append("      INNER JOIN File_Target FTarget ON Base.Target = FTarget.Class_ID");
            SBSql.Append("    WHERE (Base.File_ID IN (");
            SBSql.Append("      SELECT File_ID");
            SBSql.Append("      FROM File_Rel_ModelNo");
            SBSql.Append("       WHERE Model_No = @ModelNo");
            SBSql.Append("      ))");
            SBSql.Append(" ) AS TBL");
            SBSql.Append(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
            SBSql.Append(" ORDER BY RowRank");


            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
            cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
            cmd.Parameters.AddWithValue("ModelNo", Param_ModelNo);


            //[SQL] - 計算資料總數
            SBSql.Clear();
            SBSql.Append(" SELECT COUNT(*) AS TOTAL_CNT ");
            SBSql.Append(" FROM File_List Base ");
            SBSql.Append("  INNER JOIN File_Class Cls ON Base.Class_ID = Cls.Class_ID AND LOWER(Cls.LangCode) = 'zh-tw'");
            SBSql.Append("  INNER JOIN File_LangType Lang ON Base.LangType_ID = Lang.Class_ID");
            SBSql.Append("  INNER JOIN File_Type FType ON Base.FileType_ID = FType.Class_ID AND LOWER(FType.LangCode) = 'zh-tw'");
            SBSql.Append("  INNER JOIN File_Target FTarget ON Base.Target = FTarget.Class_ID");
            SBSql.Append(" WHERE (Base.File_ID IN (");
            SBSql.Append("  SELECT File_ID");
            SBSql.Append("  FROM File_Rel_ModelNo");
            SBSql.Append("  WHERE Model_No = @ModelNo");
            SBSql.Append("  ))");

            //[SQL] - Command
            cmdTotalCnt.CommandText = SBSql.ToString();
            cmdTotalCnt.Parameters.AddWithValue("ModelNo", Param_ModelNo);

            //[SQL] - 取得資料
            using (DataTable DT = dbConClass.LookupDTwithPage(cmd, cmdTotalCnt, dbConClass.DBS.PKEF, out TotalCnt, out ErrMsg))
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
                        Response.Redirect(Page_CurrentUrl + "&page=" + TotalPage);
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
                        sb.AppendFormat("<a href=\"{0}&page=1\" class=\"PagePre\">第一頁</a>", Page_CurrentUrl);
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
                            sb.AppendFormat("<a href=\"{0}&page={1}\">{1}</a>", Page_CurrentUrl, i);
                        }
                    }
                    sb.AppendLine("</div>");

                    //[頁碼] - 最後1頁
                    if (page < TotalPage)
                    {
                        sb.AppendFormat("<a href=\"{0}&page={1}\" class=\"PageNext\">{2}</a>", Page_CurrentUrl, TotalPage, "最後一頁");
                    }
                    //顯示分頁
                    this.pl_Page.Visible = true;
                    this.lt_Page_Link.Text = sb.ToString();

                    #endregion
                }


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

    //分頁跳轉
    protected void ddl_Page_List_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Response.Redirect(Page_CurrentUrl + "&page=" + this.ddl_Page_List.SelectedValue);
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            String Model_No = Request.QueryString["Model_No"];
            if (fn_Extensions.String_字數(Model_No, "1", "40", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                return "";
            }
            else
            {
                return Model_No.Trim();

            }
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }


    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}Product/Prod_Files_View.aspx?Model_No={1}".FormatThis(
                Application["WebUrl"]
                , HttpUtility.UrlEncode(Param_ModelNo)
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }
    #endregion

    #region -- 語系參數 --
    /// <summary>
    /// [Navi] - 系統首頁
    /// </summary>
    private string _Navi_系統首頁;
    public string Navi_系統首頁
    {
        get
        {
            return Res_Navi.系統首頁;
        }
        private set
        {
            this._Navi_系統首頁 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料庫;
    public string Navi_產品資料庫
    {
        get
        {
            return Res_Navi.產品資料庫;
        }
        private set
        {
            this._Navi_產品資料庫 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料;
    public string Navi_產品資料
    {
        get
        {
            return Res_Navi.產品資料;
        }
        private set
        {
            this._Navi_產品資料 = value;
        }
    }

    #endregion
}
