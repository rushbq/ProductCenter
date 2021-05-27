using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using Newtonsoft.Json.Linq;
using Resources;

public partial class Prod_Search : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg;

            //[權限判斷] - 產品資料
            if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[取得資料] - 類別選單
            if (fn_Extensions.ProdClassMenu(this.ddl_Class_ID, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("類別選單產生失敗！", "");
            }
            this.ddl_Class_ID.Items.Insert(0, "-- 全類別 --");
            this.ddl_Class_ID.Items[0].Value = "All";

            //[取得/檢查參數] - Model_No(品號)
            if (fn_Extensions.String_字數(Request.QueryString["Model_No"], "1", "50", out ErrMsg))
            {
                this.tb_Model_No.Text = Request.QueryString["Model_No"].ToString().Trim().ToUpper();
            }

            //[取得/檢查參數] - currModel(指定品號)
            if (fn_Extensions.String_字數(Request.QueryString["currModel"], "1", "40", out ErrMsg))
            {
                this.tb_CurrModelNo.Text = Request.QueryString["currModel"].ToString().Trim().ToUpper();
            }

            //[取得/檢查參數] - Class_ID(銷售類別)
            if (fn_Extensions.String_字數(Request.QueryString["Class_ID"], "1", "6", out ErrMsg))
            {
                this.ddl_Class_ID.SelectedValue = Request.QueryString["Class_ID"].ToString().Trim();
            }

            //[取得/檢查參數] - 上市日期
            if (fn_Extensions.String_字數(Request.QueryString["StartDate"], "1", "10", out ErrMsg))
            {
                this.tb_StartDate.Text = Request.QueryString["StartDate"].ToString();
            }
            if (fn_Extensions.String_字數(Request.QueryString["EndDate"], "1", "10", out ErrMsg))
            {
                this.tb_EndDate.Text = Request.QueryString["EndDate"].ToString();
            }

            //[取得/檢查參數] - 主要出貨地
            if (fn_Extensions.String_字數(Request.QueryString["ShipFrom"], "1", "4", out ErrMsg))
            {
                this.ddl_ShipFrom.SelectedValue = Request.QueryString["ShipFrom"].ToString();
            }

            //[取得/檢查參數] - Vol
            if (fn_Extensions.Get_Vol(this.ddl_Vol, Request.QueryString["Vol"], true, out ErrMsg) == false)
            {
                this.ddl_Vol.Items.Insert(0, new ListItem("選單產生失敗", ""));
            }

            //[取得/檢查參數] - 頁次
            if (fn_Extensions.String_字數(Request.QueryString["VolPage"], "1", "5", out ErrMsg))
            {
                this.tb_VoltoPage.Text = Request.QueryString["VolPage"].ToString();
            }

            //[取得/檢查參數] - Barcode
            if (fn_Extensions.String_字數(Request.QueryString["bc"], "1", "40", out ErrMsg))
            {
                this.tb_Barcode.Text = Request.QueryString["bc"].ToString();
            }

            //[取得/檢查參數] - page(頁數)
            int page = 1;
            if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
            {
                page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
            }


            #region -- PDF AccessToken --

            //[取得API Token]
            string LoginID = System.Web.Configuration.WebConfigurationManager.AppSettings["API_PDFLoginID"];
            string LoginPwd = Cryptograph.MD5(System.Web.Configuration.WebConfigurationManager.AppSettings["API_PDFLoginPwd"]);

            //Get Token Request
            string Url = "{0}GetAccessToken/".FormatThis(Application["API_WebUrl"]);
            string GetTokenJson = fn_Extensions.WebRequest_POST(
                Url
                , "LoginID={0}&LoginPwd={1}".FormatThis(LoginID, LoginPwd));

            if (string.IsNullOrEmpty(GetTokenJson))
            {
                Response.Write("Token取得失敗");
            }

            //解析Json
            JObject jObject = JObject.Parse(GetTokenJson);

            //填入資料
            if (jObject["tokenID"] != null)
            {
                Token = jObject["tokenID"].ToString();
            }

            #endregion


            //[帶出資料]
            LookupDataList(page);

            //[權限判斷]  - 編輯
            #region "--權限判斷--"
            bool myAuth = fn_CheckAuth.CheckAuth_User("120", out ErrMsg);

            //[取得物件] - 判斷資料列表
            if (this.lvDataList.Items.Count > 0)
            {
                for (int i = 0; i < lvDataList.Items.Count; i++)
                {
                    //[取得物件] - 編輯區
                    PlaceHolder ph_Edit = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_Edit"));
                    ph_Edit.Visible = myAuth;
                }
            }
            #endregion

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
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "Product/Prod_Search.aspx?func=Product";

            //[參數宣告] - 筆數/分頁設定
            int PageSize = 20;  //每頁筆數
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
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("    SELECT ");
            SBSql.AppendLine("      RTRIM(Prod_Item.Model_No) AS Model_No, Prod_Item.BarCode, REPLACE(Prod_Item.Catelog_Vol, 'NULL', '') Catelog_Vol, REPLACE(Prod_Item.Page, 'NULL', '') Page");
            SBSql.AppendLine("      , Prod_Item.Ship_From, Prod_Item.Date_Of_Listing, Prod_Item.Stop_Offer_Date");
            //圖片(判斷圖片中心 2->1->3->4->5->7->8->9)
            SBSql.AppendLine("      , (SELECT TOP 1 (ISNULL(Pic02,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'') ");
            SBSql.AppendLine("          + '|' + ISNULL(Pic05,'') + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')) AS PicGroup");
            SBSql.AppendLine("          FROM ProdPic_Photo WHERE (ProdPic_Photo.Model_No = Prod_Item.Model_No)");
            SBSql.AppendLine("      ) AS PhotoGroup ");
            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Prod_Item.Model_No) AS RowRank ");
            //類別(依語系)
            SBSql.AppendLine(string.Format(", Prod_Class.Class_Name_{0} AS Class_Name ", "zh_TW"));
            SBSql.Append(string.Format(", Prod_Item.Model_Name_{0} AS Model_Name ", "zh_TW"));

            /* EDM 停售發佈時間 */
            SBSql.Append(", (");
            SBSql.Append(" SELECT TOP 1 CONVERT(VARCHAR, Base.SendTime, 111) AS StopDate");
            SBSql.Append(" FROM [eDM].dbo.EDM_List Base INNER JOIN [eDM].dbo.EDM_Rel_ModelNo Rel ON Base.EDM_ID = Rel.EDM_ID");
            SBSql.Append(" WHERE (Base.Template_ClassID = 4) AND (Base.InProcess = 'Y') AND (Rel.Model_No = Prod_Item.Model_No)");
            SBSql.Append(" ORDER BY Base.SendTime DESC");
            SBSql.Append(" ) StopDate");

            SBSql.AppendLine("    FROM Prod_Item AS Prod_Item ");
            SBSql.AppendLine("         INNER JOIN Prod_Class ON Prod_Item.Class_ID = Prod_Class.Class_ID ");
            //[SQL] - 條件, 顯示不為 0類 的品號
            SBSql.AppendLine("    WHERE (LEFT(Prod_Item.Model_No,1) <> '0') ");


            #region -- 查詢條件 --

            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(Prod_Item.Model_No) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Item.Model_Name_zh_TW) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Item.Model_Name_zh_CN) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Item.Model_Name_en_US) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append(" ) ");
                cmd.Parameters.AddWithValue("Model_No", this.tb_Model_No.Text);

                this.ViewState["Page_LinkStr"] += "&Model_No=" + Server.UrlEncode(this.tb_Model_No.Text);
            }

            //[查詢條件] - currModel(指定品號)
            if (string.IsNullOrEmpty(this.tb_CurrModelNo.Text) == false)
            {
                SBSql.Append(" AND (UPPER(Prod_Item.Model_No) = UPPER(@currModel))");

                cmd.Parameters.AddWithValue("currModel", this.tb_CurrModelNo.Text);
            }

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Class_ID = @Class_ID) ");
                cmd.Parameters.AddWithValue("Class_ID", this.ddl_Class_ID.SelectedValue);

                this.ViewState["Page_LinkStr"] += "&Class_ID=" + Server.UrlEncode(this.ddl_Class_ID.SelectedValue);
            }

            //[查詢條件] - 上市日期
            if (string.IsNullOrEmpty(this.tb_StartDate.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Date_Of_Listing >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", this.tb_StartDate.Text.ToDateString("yyyyMMdd"));

                this.ViewState["Page_LinkStr"] += "&StartDate=" + Server.UrlEncode(this.tb_StartDate.Text);
            }
            if (string.IsNullOrEmpty(this.tb_EndDate.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Date_Of_Listing <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", this.tb_EndDate.Text.ToDateString("yyyyMMdd"));

                this.ViewState["Page_LinkStr"] += "&EndDate=" + Server.UrlEncode(this.tb_EndDate.Text);
            }

            //[查詢條件] - 主要出貨地
            if (this.ddl_ShipFrom.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Ship_From = @ShipFrom) ");
                cmd.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);

                this.ViewState["Page_LinkStr"] += "&ShipFrom=" + Server.UrlEncode(this.ddl_ShipFrom.SelectedValue);
            }

            //[取得/檢查參數] - Vol
            if (this.ddl_Vol.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Catelog_Vol = @Vol) ");
                cmd.Parameters.AddWithValue("Vol", this.ddl_Vol.SelectedValue);

                this.ViewState["Page_LinkStr"] += "&Vol=" + Server.UrlEncode(this.ddl_Vol.SelectedValue);
            }

            //[取得/檢查參數] - 頁次
            if (string.IsNullOrEmpty(this.tb_VoltoPage.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Page = @VolPage) ");
                cmd.Parameters.AddWithValue("VolPage", this.tb_VoltoPage.Text);

                this.ViewState["Page_LinkStr"] += "&VolPage=" + Server.UrlEncode(this.tb_VoltoPage.Text);
            }

            //[取得/檢查參數] - 條碼
            if (string.IsNullOrEmpty(this.tb_Barcode.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Barcode LIKE '%' + UPPER(@Barcode) + '%') ");
                cmd.Parameters.AddWithValue("Barcode", this.tb_Barcode.Text);

                this.ViewState["Page_LinkStr"] += "&bc=" + Server.UrlEncode(this.tb_Barcode.Text);
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
            SBSql.AppendLine(" FROM Prod_Item AS Prod_Item ");

            SBSql.AppendLine("      INNER JOIN Prod_Class ON Prod_Item.Class_ID = Prod_Class.Class_ID ");
            //[SQL] - 條件, 顯示不為 0類 的品號
            SBSql.AppendLine(" WHERE (LEFT(Prod_Item.Model_No,1) <> '0') ");

            #region -- 查詢條件 --
            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
            {
                SBSql.Append(" AND (");
                SBSql.Append("  (UPPER(Prod_Item.Model_No) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Item.Model_Name_zh_TW) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Item.Model_Name_zh_CN) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append("  OR (UPPER(Prod_Item.Model_Name_en_US) LIKE '%' + UPPER(@Model_No) + '%') ");
                SBSql.Append(" ) ");

                cmdTotalCnt.Parameters.AddWithValue("Model_No", this.tb_Model_No.Text);
            }

            //[查詢條件] - currModel(指定品號)
            if (string.IsNullOrEmpty(this.tb_CurrModelNo.Text) == false)
            {
                SBSql.Append(" AND (UPPER(Prod_Item.Model_No) = UPPER(@currModel))");

                cmdTotalCnt.Parameters.AddWithValue("currModel", this.tb_CurrModelNo.Text);
            }

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Class_ID = @Class_ID) ");
                cmdTotalCnt.Parameters.AddWithValue("Class_ID", this.ddl_Class_ID.SelectedValue);
            }

            //[查詢條件] - 上市日期
            if (string.IsNullOrEmpty(this.tb_StartDate.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Date_Of_Listing >= @StartDate) ");
                cmdTotalCnt.Parameters.AddWithValue("StartDate", this.tb_StartDate.Text.ToDateString("yyyyMMdd"));
            }
            if (string.IsNullOrEmpty(this.tb_EndDate.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Date_Of_Listing <= @EndDate) ");
                cmdTotalCnt.Parameters.AddWithValue("EndDate", this.tb_EndDate.Text.ToDateString("yyyyMMdd"));
            }

            //[查詢條件] - 主要出貨地
            if (this.ddl_ShipFrom.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Ship_From = @ShipFrom) ");
                cmdTotalCnt.Parameters.AddWithValue("ShipFrom", this.ddl_ShipFrom.SelectedValue);
            }

            //[取得/檢查參數] - Vol
            if (this.ddl_Vol.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Catelog_Vol = @Vol) ");
                cmdTotalCnt.Parameters.AddWithValue("Vol", this.ddl_Vol.SelectedValue);
            }

            //[取得/檢查參數] - 頁次
            if (string.IsNullOrEmpty(this.tb_VoltoPage.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Page = @VolPage) ");
                cmdTotalCnt.Parameters.AddWithValue("VolPage", this.tb_VoltoPage.Text);
            }

            //[取得/檢查參數] - 條碼
            if (string.IsNullOrEmpty(this.tb_Barcode.Text) == false)
            {
                SBSql.Append(" AND (Prod_Item.Barcode LIKE '%' + UPPER(@Barcode) + '%') ");
                cmdTotalCnt.Parameters.AddWithValue("Barcode", this.tb_Barcode.Text);
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

                //暫存目前Url
                Session["BackListUrl"] = this.ViewState["Page_LinkStr"] + "&page=" + page;

                //DataBind            
                this.lvDataList.DataSource = DT.DefaultView;
                this.lvDataList.DataBind();
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex);
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

            //圖片顯示
            Literal lt_Photo = ((Literal)e.Item.FindControl("lt_Photo"));
            if (string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "PhotoGroup").ToString()) == false)
            {
                //Web資料夾路徑
                string WebFolder = Application["File_WebUrl"] + @"ProductPic/";
                string ModelNo = DataBinder.Eval(dataItem.DataItem, "Model_No").ToString();
                string ModelName = DataBinder.Eval(dataItem.DataItem, "Model_Name").ToString();
                string PhotoGroup = DataBinder.Eval(dataItem.DataItem, "PhotoGroup").ToString();

                //拆解圖片值 "|"
                string Photo = "";
                string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");
                for (int row = 0; row < strAry.Length; row++)
                {
                    if (false == string.IsNullOrEmpty(strAry[row].ToString()))
                    {
                        Photo = strAry[row].ToString();
                        break;
                    }
                }

                //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
                if (false == string.IsNullOrEmpty(Photo))
                {
                    //取小圖
                    Photo = "500x500_{0}".FormatThis(Photo);

                    lt_Photo.Text = "<td width=\"110\">";
                    lt_Photo.Text += string.Format(
                        "<a rel=\"PicGroup\" class=\"PicGroup\" href=\"{3}\" title=\"{1}&lt;br&gt;{2}\"><img class=\"lazy img-thumbnail\" src=\"{0}\" data-original=\"{3}\" width=\"100px\" border=\"0\" style=\"min-width:100px\" /></a>"
                        , Application["WebUrl"] + "js/lazyload/grey.gif"
                        , Server.UrlEncode(ModelNo)
                        , ModelName
                        , WebFolder + Server.UrlEncode(ModelNo) + "/1/" + Photo);
                    lt_Photo.Text += "</td>";
                }
            }
        }
    }

    #endregion

    #region -- 其他功能 --
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
            SBUrl.Append("Prod_Search.aspx?func=Product");

            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
            {
                SBUrl.Append("&Model_No=" + Server.UrlEncode(this.tb_Model_No.Text));
            }

            //[查詢條件] - currModel(指定品號)
            if (string.IsNullOrEmpty(this.tb_CurrModelNo.Text) == false)
            {
                SBUrl.Append("&currModel=" + Server.UrlEncode(this.tb_CurrModelNo.Text));
            }

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
            {
                SBUrl.Append("&Class_ID=" + Server.UrlEncode(this.ddl_Class_ID.SelectedValue));
            }

            //[取得/檢查參數] - 上市日期
            if (string.IsNullOrEmpty(this.tb_StartDate.Text) == false)
            {
                SBUrl.Append("&StartDate=" + Server.UrlEncode(this.tb_StartDate.Text));
            }
            if (string.IsNullOrEmpty(this.tb_EndDate.Text) == false)
            {
                SBUrl.Append("&EndDate=" + Server.UrlEncode(this.tb_EndDate.Text));
            }

            //[查詢條件] - 主要出貨地
            if (this.ddl_ShipFrom.SelectedIndex > 0)
            {
                SBUrl.Append("&ShipFrom=" + Server.UrlEncode(this.ddl_ShipFrom.SelectedValue));
            }

            //[取得/檢查參數] - Vol
            if (this.ddl_Vol.SelectedIndex > 0)
            {
                SBUrl.Append("&Vol=" + Server.UrlEncode(this.ddl_Vol.SelectedValue));
            }

            //[取得/檢查參數] - 頁次
            if (string.IsNullOrEmpty(this.tb_VoltoPage.Text) == false)
            {
                SBUrl.Append("&VolPage=" + Server.UrlEncode(this.tb_VoltoPage.Text));
            }

            //[取得/檢查參數] - Barcode
            if (string.IsNullOrEmpty(this.tb_Barcode.Text) == false)
            {
                SBUrl.Append("&bc=" + Server.UrlEncode(this.tb_Barcode.Text));
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
    /// 取得PDF下載路徑
    /// </summary>
    /// <param name="webType">內部或外部(insde/outside)</param>
    /// <param name="lang">語系</param>
    /// <param name="modelNo">品號</param>
    /// <returns></returns>
    public string GetPDFUrl(string webType, string lang, string modelNo)
    {
        return "{0}PDF/{1}/?u={2}&f={3}".FormatThis(
              Application["API_WebUrl"].ToString()
              , Token
              , HttpUtility.UrlEncode("http://view.prokits.com.tw/{0}/{1}/{2}/".FormatThis(webType.Equals("inside") ? "SIP-I" : "SIP-O", lang, HttpUtility.UrlEncode(modelNo)))
              , "{0}.pdf".FormatThis(HttpUtility.UrlEncode(modelNo))
          );
    }

    public string GetViewUrl(string webType, string lang, string modelNo)
    {
        return "http://view.prokits.com.tw/{0}/{1}/{2}/".FormatThis(webType.Equals("inside") ? "SIP-I" : "SIP-O", lang, HttpUtility.UrlEncode(modelNo));
    }

    /// <summary>
    /// 產生token
    /// </summary>
    private string _Token;
    public string Token
    {
        get;
        set;
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
    /// [Navi] - 產品列表
    /// </summary>
    private string _Navi_產品列表;
    public string Navi_產品列表
    {
        get
        {
            return Res_Navi.產品列表;
        }
        private set
        {
            this._Navi_產品列表 = value;
        }
    }

    #endregion
}