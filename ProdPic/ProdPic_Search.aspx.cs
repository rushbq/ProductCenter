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
using System.IO;
using System.Xml;
using System.Xml.Linq;

public partial class ProdPic_Search : SecurityIn
{
    //[設定暫存參數] - 圖片類別
    List<TempParam> ITempList = new List<TempParam>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg = "";

                //[取得/檢查參數] - flag (連結來源 - 行企or品保, 判斷是否有權限)
                if (fn_Extensions.String_字數(Request.QueryString["flag"], "1", "3", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("來源參數錯誤！", "../Main.aspx");
                }
                //[權限判斷] - 圖片資料庫 (依 flag 判斷)
                if (fn_CheckAuth.CheckAuth_User(Param_flag, out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[取得資料] - 類別選單
                if (fn_Extensions.ProdClassMenu(this.ddl_Class_ID, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("類別選單產生失敗！", "");
                }
                this.ddl_Class_ID.Items.Insert(0, "-- 所有資料 --");
                this.ddl_Class_ID.Items[0].Value = "All";

                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());

                //[取得/檢查參數] - Model_No(品號)
                if (fn_Extensions.String_字數(Request.QueryString["Model_No"], "1", "40", out ErrMsg))
                {
                    this.tb_Model_No.Text = Request.QueryString["Model_No"].ToString().Trim();
                }

                //[取得/檢查參數] - Class_ID(類別)
                if (fn_Extensions.String_字數(Request.QueryString["Class_ID"], "1", "6", out ErrMsg))
                    this.ddl_Class_ID.SelectedValue = Request.QueryString["Class_ID"].ToString().Trim();

                //[暫存參數]
                InputTemp();
                //[帶出資料]
                LookupDataList(page);
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "");
            }

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
        string ErrMsg = "";
        try
        {
            //[參數宣告] - 設定本頁Url
            this.ViewState["Page_LinkStr"] = Application["WebUrl"] + "ProdPic/ProdPic_Search.aspx?func=ProdPic&flag=" + Param_flag;

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
            SBSql.Clear();
            SBSql.AppendLine(" SELECT TBL.* ");
            SBSql.AppendLine(" FROM ( ");
            SBSql.AppendLine("  SELECT ");
            SBSql.AppendLine("      Prod_Item.Model_No, Prod_Item.Ship_From, Prod_Item.BarCode ");
            SBSql.AppendLine("      , Prod_Item.Class_ID");
            SBSql.AppendLine(string.Format(", Prod_Class.Class_Name_{0} AS Class_Name ", fn_Language.Param_Lang));
            //[SQL] - 判斷各圖類是否有資料
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Photo AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) ) AS 產品圖");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Figure AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) ) AS 產品輔圖");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 3) ) AS DM");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 4) ) AS 彩盒");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 5) ) AS 彩標");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 6) ) AS 貼紙");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 7) ) AS 卡片");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 8) ) AS 說明書");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 9) ) AS Pounch袋");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 10) ) AS 本體設計");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 11) ) AS 袖套");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 12) ) AS 吊卡");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 13) ) AS 尺寸示意圖");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 99) ) AS 其他");
            SBSql.AppendLine("      , (SELECT COUNT(CntPic.Pic_ID) FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 14) ) AS 品保");
            //[SQL] - 取得最後更新時間(類別3~end)
            SBSql.AppendLine("      , (SELECT TOP 1 (CASE WHEN Update_Time IS NULL THEN Create_Time ELSE Update_Time END) AS LastTime FROM ProdPic_Photo AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) ) AS Time1");
            SBSql.AppendLine("      , (SELECT TOP 1 (CASE WHEN Update_Time IS NULL THEN Create_Time ELSE Update_Time END) AS LastTime FROM ProdPic_Figure AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) ) AS Time2");
            for (int i = 3; i <= 14; i++)
            {
                SBSql.AppendLine("      , (SELECT TOP 1 (CASE WHEN Update_Time IS NULL THEN Create_Time ELSE Update_Time END) AS LastTime FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = " + i + ") ) AS Time" + i);
            }
            SBSql.AppendLine("      , (SELECT TOP 1 (CASE WHEN Update_Time IS NULL THEN Create_Time ELSE Update_Time END) AS LastTime FROM ProdPic_Group AS CntPic WHERE (CntPic.Model_No = Prod_Item.Model_No) AND (CntPic.Pic_Class = 99) ) AS Time99");

            SBSql.AppendLine("      , ROW_NUMBER() OVER (ORDER BY Prod_Item.Model_No, Prod_Item.Class_ID) AS RowRank");
            SBSql.AppendLine("  FROM Prod_Item INNER JOIN Prod_Class ON Prod_Item.Class_ID = Prod_Class.Class_ID ");
            SBSql.AppendLine("  WHERE (1 = 1) ");
            #region "查詢條件"
            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
            {
                SBSql.Append(" AND (UPPER(Prod_Item.Model_No) LIKE UPPER(@Model_No) + '%') ");
                cmd.Parameters.AddWithValue("Model_No", this.tb_Model_No.Text);

                this.ViewState["Page_LinkStr"] += "&Model_No=" + Server.UrlEncode(this.tb_Model_No.Text);
            }

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Class_ID = @Class_ID) ");
                cmd.Parameters.AddWithValue("Class_ID", this.ddl_Class_ID.SelectedValue);

                this.ViewState["Page_LinkStr"] += "&Class_ID=" + Server.UrlEncode(this.ddl_Class_ID.SelectedValue);
            }
            #endregion
            //SBSql.AppendLine("  GROUP BY Prod_Item.Model_No, Prod_Item.Class_ID, Prod_Class.Class_Name_zh_TW, Prod_Item.Ship_From, Prod_Item.BarCode ");
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
            SBSql.AppendLine(" FROM Prod_Item INNER JOIN Prod_Class ON Prod_Item.Class_ID = Prod_Class.Class_ID ");
            SBSql.AppendLine(" WHERE (1 = 1) ");

            #region "查詢條件"
            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
            {
                SBSql.Append(" AND (UPPER(Prod_Item.Model_No) LIKE UPPER(@Model_No) + '%') ");
                cmdTotalCnt.Parameters.AddWithValue("Model_No", this.tb_Model_No.Text);
            }

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
            {
                SBSql.Append(" AND (Prod_Item.Class_ID = @Class_ID) ");
                cmdTotalCnt.Parameters.AddWithValue("Class_ID", this.ddl_Class_ID.SelectedValue);
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
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            string ErrMsg = "";

            ListViewDataItem dataItem = (ListViewDataItem)e.Item;
            //取得品號
            string ModelNo = DataBinder.Eval(dataItem.DataItem, "Model_No").ToString().Trim();

            //[權限判斷] - 圖片資料庫 (依 flag 判斷)
            bool showUrl;
            switch (Param_flag)
            {
                case "301":
                    if (fn_CheckAuth.CheckAuth_User("303", out ErrMsg))
                        showUrl = true;
                    else
                        showUrl = false;
                    break;

                case "302":
                    if (fn_CheckAuth.CheckAuth_User("304", out ErrMsg))
                        showUrl = true;
                    else
                        showUrl = false;
                    break;

                default:
                    showUrl = false;
                    break;
            }

            //判斷&顯示編輯連結
            Literal lt_Edit = (Literal)e.Item.FindControl("lt_Edit");
            if (showUrl)
            {
                string Url = "";
                string Url_CID = "";
                if (Param_flag == "301")
                {
                    Url = "ProdPic_Photo.aspx";
                    Url_CID = "1";
                }
                else
                {
                    Url = "ProdPic_Group.aspx";
                    Url_CID = "14";
                }

                lt_Edit.Text = string.Format("<a class=\"L2MainHead\" href=\"{0}\">{1}<span class=\"JQ-ui-icon ui-icon-gear\"></span></a>"
                    , Url + "?flag=" + Param_flag + "&C_ID=" + Url_CID + "&ModelNo=" + HttpUtility.UrlEncode(ModelNo)
                    , ModelNo);
            }
            else
            {
                lt_Edit.Text = string.Format("<span class=\"L2MainHead\">{0}</span>", ModelNo);
            }

            //判斷各圖片種類，是否有資料
            for (int i = 0; i < ITempList.Count; i++)
            {
                if (Convert.ToInt16(DataBinder.Eval(dataItem.DataItem, ITempList[i].Param_ClassName)) > 0)
                {
                    //取得控制項
                    string classID = ITempList[i].Param_ClassID;
                    Literal ltView = ((Literal)e.Item.FindControl("lt_View" + classID));

                    //設定內容
                    ltView.Text =
                        string.Format("<a href=\"{0}\">{1}</a>"
                        , ITempList[i].Param_ClassPage.Replace(".aspx", "_View.aspx") +
                          "?flag=" + Param_flag + "&C_ID=" + classID +
                          "&ModelNo=" + HttpUtility.UrlEncode(ModelNo)
                        , DataBinder.Eval(dataItem.DataItem, "Time" + classID).ToString().ToDateString("yyyy-MM-dd HH:mm"));

                    string ts = DateTime.Now.ToString().ToDateString("yyMMddHHmm");
                    //設定壓縮檔下載路徑
                    if (classID.Equals("1") || classID.Equals("2"))
                    {
                        string zipFile = "<br><a href=\"{0}ProductPic_Zip/{1}_gallery{2}.zip?v={3}\" target=\"_blank\">zip下載</a>".FormatThis(
                                Application["File_WebUrl"]
                                , ModelNo
                                , classID
                                , ts
                            );
                        ltView.Text += zipFile;
                    }

                }
            }
        }
    }

    #endregion

    #region "前端頁面控制"
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
            SBUrl.Append("ProdPic_Search.aspx?func=ProdPic&flag=" + Param_flag);

            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
                SBUrl.Append("&Model_No=" + Server.UrlEncode(this.tb_Model_No.Text));

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
                SBUrl.Append("&Class_ID=" + Server.UrlEncode(this.ddl_Class_ID.SelectedValue));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    #endregion

    /// <summary>
    /// 將圖片類別存入暫存區
    /// ** 需注意SQL Select項目及排序是否與Xml符合
    /// </summary>
    void InputTemp()
    {
        //取得Xml
        string XmlResult = fn_Extensions.WebRequest_GET(
            System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/ProdPicClass.xml");
        //將Xml字串轉成byte
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
        //讀取Xml
        using (XmlReader reader = XmlTextReader.Create(stream))
        {
            //使用XElement載入Xml
            XElement XmlDoc = XElement.Load(reader);

            var Results = from result in XmlDoc.Elements("Class")
                          orderby Convert.ToInt16(result.Element("Sort").Value) ascending
                          select new
                          {
                              ID = result.Attribute("ID").Value,
                              Name = result.Element("Name").Value,
                              Page = result.Element("Page").Value
                          };

            foreach (var result in Results)
            {
                ITempList.Add(new TempParam(result.ID, result.Name, result.Page));
            }

        }
    }

    /// <summary>
    /// 來源參數
    /// </summary>
    private string _Param_flag;
    public string Param_flag
    {
        get
        {
            return this._Param_flag != null ? this._Param_flag : Request.QueryString["flag"].ToString();
        }
        set
        {
            this._Param_flag = value;
        }
    }
}

/// <summary>
/// 暫存參數
/// </summary>
public class TempParam
{
    #region "參數設定"
    /// <summary>
    /// [參數] - 圖片類別編號
    /// </summary>
    private string _Param_ClassID;
    public string Param_ClassID
    {
        get { return this._Param_ClassID; }
        set { this._Param_ClassID = value; }
    }

    /// <summary>
    /// [參數] - 圖片類別名稱
    /// </summary>
    private string _Param_ClassName;
    public string Param_ClassName
    {
        get { return this._Param_ClassName; }
        set { this._Param_ClassName = value; }
    }

    /// <summary>
    /// [參數] - 指向網頁
    /// </summary>
    private string _Param_ClassPage;
    public string Param_ClassPage
    {
        get { return this._Param_ClassPage; }
        set { this._Param_ClassPage = value; }
    }
    #endregion

    /// <summary>
    /// 設定參數值
    /// </summary>
    /// <param name="Param_ClassID">圖片類別編號</param>
    /// <param name="Param_ClassName">圖片類別名稱</param>
    /// <param name="Param_ClassPage">指向網頁</param>
    public TempParam(string Param_ClassID, string Param_ClassName, string Param_ClassPage)
    {
        this._Param_ClassID = Param_ClassID;
        this._Param_ClassName = Param_ClassName;
        this._Param_ClassPage = Param_ClassPage;
    }
}