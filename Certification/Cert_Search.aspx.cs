using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using PKLib_Method.Methods;

public partial class Cert_Search : SecurityIn
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

                if (fn_CheckAuth.CheckAuth_User("202", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                #endregion
                //[權限判斷] End


                #region --傳遞參數處理--

                //[取得資料] - 類別選單
                if (ExtensionMethods.fn_Extensions.ProdClassMenu(this.ddl_Class_ID, out ErrMsg) == false)
                {
                    CustomExtension.AlertMsg("類別選單產生失敗！", "");
                }
                this.ddl_Class_ID.Items.Insert(0, "-- 所有資料 --");
                this.ddl_Class_ID.Items[0].Value = "";


                //[取得/檢查參數] - page(頁數)
                int page = 1;
                if (ExtensionMethods.fn_Extensions.Num_正整數(Request.QueryString["page"], "1", "1000000", out ErrMsg))
                {
                    page = Convert.ToInt16(Request.QueryString["page"].ToString().Trim());
                }

                //[取得/檢查參數] - Model_No(品號)
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["Model_No"], "1", "40", out ErrMsg))
                {
                    this.tb_Model_No.Text = Request.QueryString["Model_No"].ToString().Trim().ToUpper();
                }

                //[取得/檢查參數] - Class_ID(類別)
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["Class_ID"], "1", "6", out ErrMsg))
                {
                    this.ddl_Class_ID.SelectedValue = Request.QueryString["Class_ID"].ToString().Trim();
                }

                //[取得/檢查參數] - Keyword
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["Keyword"], "1", "50", out ErrMsg))
                {
                    this.tb_Keyword.Text = Request.QueryString["Keyword"].ToString().Trim();
                }

                //[取得/檢查參數] - IsExpired(認證是否到期)
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["IsExpired"], "1", "1", out ErrMsg))
                {
                    if (Request.QueryString["IsExpired"].ToString().Trim() == "Y")
                        this.cb_IsExpired.Checked = true;
                }

                //[取得/檢查參數] - Vol
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["Vol"], "1", "10", out ErrMsg))
                {
                    this.tb_Vol.Text = Request.QueryString["Vol"].ToString().Trim();
                }

                //[取得/檢查參數] - IsCE
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["IsCE"], "1", "1", out ErrMsg))
                {
                    if (Request.QueryString["IsCE"].ToString().Trim() == "Y")
                        this.cb_IsCE.Checked = true;
                }

                //[取得/檢查參數] - IsCheck
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["IsCheck"], "1", "1", out ErrMsg))
                {
                    if (Request.QueryString["IsCheck"].ToString().Trim() == "Y")
                        this.cb_IsCheck.Checked = true;
                }

                //[取得/檢查參數] - Self_Cert
                if (ExtensionMethods.fn_Extensions.String_字數(Request.QueryString["Self_Cert"], "1", "1", out ErrMsg))
                {
                    this.ddl_SelfCert.SelectedValue = Request.QueryString["Self_Cert"].ToString().Trim();
                }

                #endregion


                //Get Data
                LookupDataList(Req_PageIdx);


                #region --編輯權限判斷--
                if (fn_CheckAuth.CheckAuth_User("201", out ErrMsg))
                {
                    //[取得物件] - 判斷資料列表
                    if (this.lvDataList.Items.Count > 0)
                    {
                        for (int i = 0; i < lvDataList.Items.Count; i++)
                        {
                            //[取得物件] - 編輯區
                            PlaceHolder ph_Edit = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_Edit"));
                            //[取得物件] - 檢視區
                            PlaceHolder ph_View = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_View"));
                            ph_Edit.Visible = true;
                            ph_View.Visible = false;
                        }
                    }
                }
                else
                {
                    //[取得物件] - 判斷資料列表
                    if (this.lvDataList.Items.Count > 0)
                    {
                        for (int i = 0; i < lvDataList.Items.Count; i++)
                        {
                            //[取得物件] - 編輯區
                            PlaceHolder ph_Edit = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_Edit"));
                            //[取得物件] - 檢視區
                            PlaceHolder ph_View = ((PlaceHolder)this.lvDataList.Items[i].FindControl("ph_View"));
                            ph_Edit.Visible = false;
                            ph_View.Visible = true;
                        }
                    }
                }
                #endregion
            }
        }
        catch (Exception)
        {

            throw;
        }
    }



    #region -- 資料顯示 --

    private void LookupDataList(int pageIndex)
    {
        //----- 宣告:網址參數 -----
        int RecordsPerPage = 20;    //每頁筆數
        int StartRow = (pageIndex - 1) * RecordsPerPage;    //第n筆開始顯示
        int TotalRow = 0;   //總筆數
        int DataCnt = 0;
        ArrayList PageParam = new ArrayList();  //條件參數,for pager

        //----- 宣告:資料參數 -----
        Dictionary<string, string> search = new Dictionary<string, string>();

        #region >> 條件篩選 <<

        string Model_No = tb_Model_No.Text.Trim();
        string Class_ID = ddl_Class_ID.SelectedValue;
        string Keyword = tb_Keyword.Text.Trim();
        string Vol = tb_Vol.Text.Trim();
        string IsExpired = cb_IsExpired.Checked ? "Y" : "N";
        string IsCE = cb_IsCE.Checked ? "Y" : "N";
        string IsCheck = cb_IsCheck.Checked ? "Y" : "N";
        string BothCE = (cb_IsCE.Checked && cb_IsCheck.Checked) ? "Y" : "N";
        string SelfCert = ddl_SelfCert.SelectedValue;

        //[查詢條件] - Model_No
        if (!string.IsNullOrWhiteSpace(Model_No))
        {
            search.Add("Model_No", Model_No);
            PageParam.Add("Model_No=" + Server.UrlEncode(Model_No));
        }
        //[查詢條件] - Class_ID
        if (!string.IsNullOrWhiteSpace(Class_ID))
        {
            search.Add("Class_ID", Class_ID);
            PageParam.Add("Class_ID=" + Server.UrlEncode(Class_ID));
        }
        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            search.Add("Keyword", Keyword);
            PageParam.Add("Keyword=" + Server.UrlEncode(Keyword));
        }
        //[查詢條件] - Vol
        if (!string.IsNullOrWhiteSpace(Vol))
        {
            search.Add("Vol", Vol);
            PageParam.Add("Vol=" + Server.UrlEncode(Vol));
        }
        //[查詢條件] - Self_Cert
        if (!string.IsNullOrWhiteSpace(SelfCert))
        {
            search.Add("SelfCert", SelfCert);
            PageParam.Add("Self_Cert=" + Server.UrlEncode(SelfCert));
        }

        //[查詢條件] - IsExpired
        if (IsExpired.Equals("Y"))
        {
            search.Add("IsExpired", IsExpired);
            PageParam.Add("IsExpired=" + Server.UrlEncode(IsExpired));
        }

        //[查詢條件] - IsCE && IsCheck
        if (BothCE.Equals("Y"))
        {
            search.Add("BothCE", BothCE);
            PageParam.Add("IsCE=" + Server.UrlEncode(IsCE));
            PageParam.Add("IsCheck=" + Server.UrlEncode(IsCheck));
        }
        else
        {
            //[查詢條件] - IsCE
            if (IsCE.Equals("Y"))
            {
                search.Add("IsCE", IsCE);
                PageParam.Add("IsCE=" + Server.UrlEncode(IsCE));
            }
            //[查詢條件] - IsCheck
            if (IsCheck.Equals("Y"))
            {
                search.Add("IsCheck", IsCheck);
                PageParam.Add("IsCheck=" + Server.UrlEncode(IsCheck));
            }
        }


        #endregion

        //----- 原始資料:取得所有資料 -----
        DataTable query = GetCertList(search, StartRow, RecordsPerPage
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
            CustomExtension.setCookie("Certification", "", -1);
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
            string reSetPage = thisPage + "?page="
                + pageIndex
                + (PageParam.Count == 0 ? "" : "&") + string.Join("&", PageParam.ToArray());

            //暫存頁面Url, 給其他頁使用
            CustomExtension.setCookie("Certification", Server.UrlEncode(reSetPage), 1);

        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    List<string> ListFiles = new List<string>();
                    StringBuilder SBSql = new StringBuilder();

                    //取得編號
                    string GetDataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
                    string ModelNo = ((HiddenField)e.Item.FindControl("hf_ModelNo")).Value;

                    //[SQL] - 刪除資料
                    SBSql.AppendLine(" DELETE FROM Icon_Rel_Certification WHERE (Cert_ID = @Param_CertID) ");
                    SBSql.AppendLine(" DELETE FROM Prod_Certification_Detail WHERE (Cert_ID = @Param_CertID) ");
                    SBSql.AppendLine(" DELETE FROM Prod_Certification WHERE (Cert_ID = @Param_CertID) ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Param_CertID", GetDataID);
                    if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                    {
                        CustomExtension.AlertMsg("資料刪除失敗！", "");
                    }
                    else
                    {
                        //刪除檔案(整個目錄)
                        IOManage.DelFolder(Param_DiskFolder + ModelNo);

                        string tempUrl = CustomExtension.getCookie("Certification");
                        CustomExtension.AlertMsg("資料刪除成功", Server.UrlDecode(tempUrl));
                    }

                }
            }
            catch (Exception)
            {
                CustomExtension.AlertMsg("系統發生錯誤 - ItemCommand！", "");
            }

        }
    }

    #endregion


    #region -- Data Controller --
    /// <summary>
    /// [發送清單] 資料清單
    /// </summary>
    /// <param name="search">search集合</param>
    /// <param name="startRow">StartRow</param>
    /// <param name="endRow">RecordsPerPage</param>
    /// <param name="DataCnt">傳址參數(資料總筆數)</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    public DataTable GetCertList(Dictionary<string, string> search, int startRow, int endRow
        , out int DataCnt, out string ErrMsg)
    {
        ErrMsg = "";

        try
        {
            /* 開始/結束筆數計算 */
            int cntStartRow = startRow + 1;
            int cntEndRow = startRow + endRow;

            //----- 宣告 -----
            //List<SupInvList> dataList = new List<SupInvList>(); //資料容器
            List<SqlParameter> sqlParamList = new List<SqlParameter>(); //SQL參數容器
            List<SqlParameter> subParamList = new List<SqlParameter>(); //SQL參數取得
            StringBuilder sql = new StringBuilder(); //SQL語法容器
            StringBuilder subSql = new StringBuilder(); //條件SQL取得
            DataCnt = 0;    //資料總數

            //取得SQL語法
            subSql = CertListSQL(search);
            //取得SQL參數集合
            subParamList = CertListParams(search);


            #region >> 資料筆數SQL查詢 <<
            using (SqlCommand cmdCnt = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.Clear();
                sql.AppendLine(" SELECT COUNT(TbAll.Cert_ID) AS TotalCnt FROM (");

                //子查詢SQL
                sql.Append(subSql);

                sql.AppendLine(" ) AS TbAll");

                //----- SQL 執行 -----
                cmdCnt.CommandText = sql.ToString();
                cmdCnt.Parameters.Clear();
                sqlParamList.Clear();
                //cmd.CommandTimeout = 60;   //單位:秒

                //----- SQL 固定參數 -----
                //sqlParamList.Add(new SqlParameter("@CC_Type", CCType));

                //----- SQL 條件參數 -----
                //加入篩選後的參數
                sqlParamList.AddRange(subParamList);

                //加入參數陣列
                cmdCnt.Parameters.AddRange(sqlParamList.ToArray());

                //Execute
                using (DataTable DTCnt = dbConClass.LookupDT(cmdCnt, out ErrMsg))
                {
                    //資料總筆數
                    if (DTCnt.Rows.Count > 0)
                    {
                        DataCnt = Convert.ToInt32(DTCnt.Rows[0]["TotalCnt"]);
                    }
                }

                //*** 在SqlParameterCollection同個循環內不可有重複的SqlParam,必須清除才能繼續使用. ***
                cmdCnt.Parameters.Clear();
            }
            #endregion


            #region >> 主要資料SQL查詢 <<

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.Clear();
                sql.AppendLine(" SELECT TbAll.* FROM (");

                //子查詢SQL
                sql.Append(subSql);

                sql.AppendLine(" ) AS TbAll");

                sql.AppendLine(" WHERE (TbAll.RowIdx >= @startRow) AND (TbAll.RowIdx <= @endRow)");
                sql.AppendLine(" ORDER BY TbAll.RowIdx");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.Clear();
                sqlParamList.Clear();
                //cmd.CommandTimeout = 60;   //單位:秒

                //----- SQL 固定參數 -----
                sqlParamList.Add(new SqlParameter("@startRow", cntStartRow));
                sqlParamList.Add(new SqlParameter("@endRow", cntEndRow));

                //----- SQL 條件參數 -----
                //加入篩選後的參數
                sqlParamList.AddRange(subParamList);

                //加入參數陣列
                cmd.Parameters.AddRange(sqlParamList.ToArray());

                //Execute
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    return DT;
                }

            }

            #endregion

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
        }
    }


    /// <summary>
    /// [發送清單] 取得SQL查詢
    /// ** TSQL查詢條件寫在此 **
    /// </summary>
    /// <param name="search">search集合</param>
    /// <param name="fieldLang">欄位語系(ex:zh_TW)</param>
    /// <returns></returns>
    private StringBuilder CertListSQL(Dictionary<string, string> search)
    {
        StringBuilder sql = new StringBuilder();

        sql.AppendLine(" SELECT Certi.Cert_ID, RTRIM(Certi.Model_No) AS Model_No, Certi.Self_Cert");
        sql.AppendLine("  , Certi.Supplier, Certi.Supplier_ItemNo");
        sql.AppendLine("  , Prod_Item.Ship_From, ISNULL(REPLACE(Prod_Item.Catelog_Vol, 'NULL', ''), '') AS Vol, ISNULL(REPLACE(Prod_Item.Page, 'NULL', ''), '') AS Page");
        sql.AppendLine("  , (");
        sql.AppendLine("   SELECT Prod_Class.Class_Name_zh_TW ");
        sql.AppendLine("   FROM Prod_Class ");
        sql.AppendLine("   WHERE Prod_Class.Class_ID = Prod_Item.Class_ID ");
        sql.AppendLine("  ) AS Class_Name");
        //sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE ([Guid] = Base.Create_Who)) AS Create_Name");
        //sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE ([Guid] = Base.Update_Who)) AS Update_Name");
        sql.AppendLine("  , ROW_NUMBER() OVER(ORDER BY Certi.Model_No) AS RowIdx");
        sql.AppendLine(" FROM Prod_Certification Certi");
        sql.AppendLine("  INNER JOIN Prod_Item ON Certi.Model_No = Prod_Item.Model_No");
        sql.AppendLine(" WHERE (1 = 1)");

        /* Search */
        #region >> filter <<

        if (search != null)
        {
            //過濾空值
            var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

            //查詢內容
            foreach (var item in thisSearch)
            {
                switch (item.Key)
                {
                    case "Model_No":
                        sql.Append(" AND (UPPER(Certi.Model_No) LIKE UPPER(@Model_No) + '%')");

                        break;

                    case "Class_ID":
                        sql.Append(" AND (Prod_Item.Class_ID = @Class_ID)");

                        break;

                    case "Keyword":
                        //關鍵字
                        sql.Append(" AND (");
                        sql.Append("  Certi.Cert_ID IN (");
                        sql.Append("  SELECT Cert_ID FROM Prod_Certification_Detail ");
                        sql.Append("  WHERE (Cert_Cmd LIKE '%' + @Keyword + '%') OR (Cert_No LIKE '%' + @Keyword + '%')");
                        sql.Append("  ) ");
                        sql.Append(" )");

                        break;

                    case "Vol":
                        //目錄
                        sql.Append(" AND (Prod_Item.Catelog_Vol LIKE '%' + @Vol + '%')");
                        break;

                    case "IsExpired":
                        //認證是否到期
                        sql.Append(" AND (Certi.Cert_ID IN (");
                        sql.Append("  SELECT Cert_ID FROM Prod_Certification_Detail");
                        sql.Append("  WHERE (Cert_ValidDate < GETDATE())");
                        sql.Append(" ))");

                        break;

                    case "IsCE":
                        //自我宣告
                        sql.Append(" AND (Certi.Cert_ID IN (");
                        sql.Append("  SELECT Dtl.Cert_ID ");
                        sql.Append("  FROM Prod_Certification_Detail Dtl");
                        sql.Append("  WHERE ((Dtl.Cert_File_CE <> '') OR (Dtl.Cert_File_CE_en_US <> '') OR (Dtl.Cert_File_CE_zh_CN <> ''))");
                        sql.Append(" ))");

                        break;

                    case "IsCheck":
                        //自我檢測
                        sql.Append(" AND (Certi.Cert_ID IN (");
                        sql.Append("  SELECT Dtl.Cert_ID ");
                        sql.Append("  FROM Prod_Certification_Detail Dtl");
                        sql.Append("  WHERE ((Dtl.Cert_File_Check <> '') OR (Dtl.Cert_File_Check_en_US <> '') OR (Dtl.Cert_File_Check_zh_CN <> ''))");
                        sql.Append(" ))");

                        break;

                    case "BothCE":
                        //自我宣告 + 自我檢測
                        sql.Append("AND (");
                        sql.Append("  (Certi.Cert_ID IN (");
                        sql.Append("  SELECT Dtl.Cert_ID ");
                        sql.Append("  FROM Prod_Certification_Detail Dtl");
                        sql.Append("  WHERE ((Dtl.Cert_File_CE <> '') OR (Dtl.Cert_File_CE_en_US <> '') OR (Dtl.Cert_File_CE_zh_CN <> ''))");
                        sql.Append(" ))");
                        sql.Append(" OR (Certi.Cert_ID IN (");
                        sql.Append("  SELECT Dtl.Cert_ID ");
                        sql.Append("  FROM Prod_Certification_Detail Dtl");
                        sql.Append("  WHERE ((Dtl.Cert_File_Check <> '') OR (Dtl.Cert_File_Check_en_US <> '') OR (Dtl.Cert_File_Check_zh_CN <> ''))");
                        sql.Append(" ))");
                        sql.Append(")");

                        break;

                    case "SelfCert":
                        //自有認證
                        sql.Append(" AND (Certi.Self_Cert = @Self_Cert) ");

                        break;


                }
            }
        }
        #endregion

        return sql;
    }


    /// <summary>
    /// [發送清單] 取得條件參數
    /// ** SQL參數設定寫在此 **
    /// </summary>
    /// <param name="search">search集合</param>
    /// <returns></returns>
    private List<SqlParameter> CertListParams(Dictionary<string, string> search)
    {
        //declare
        List<SqlParameter> sqlParamList = new List<SqlParameter>();

        //get values
        if (search != null)
        {
            //過濾空值
            var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

            //查詢內容
            foreach (var item in thisSearch)
            {
                switch (item.Key)
                {
                    //case "DataID":
                    //    sqlParamList.Add(new SqlParameter("@Data_ID", item.Value));

                    //    break;

                    case "Model_No":
                        sqlParamList.Add(new SqlParameter("@Model_No", item.Value));

                        break;

                    case "Class_ID":
                        sqlParamList.Add(new SqlParameter("@Class_ID", item.Value));

                        break;

                    case "Keyword":
                        //關鍵字
                        sqlParamList.Add(new SqlParameter("@Keyword", item.Value));

                        break;

                    case "Vol":
                        //目錄
                        sqlParamList.Add(new SqlParameter("@Vol", item.Value));

                        break;

                    case "SelfCert":
                        //自有認證
                        sqlParamList.Add(new SqlParameter("@Self_Cert", item.Value));

                        break;

                }
            }
        }


        return sqlParamList;
    }

    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// [按鈕] - 匯出
    /// </summary>
    protected void lbtn_Excel_Click(object sender, EventArgs e)
    {
        //----- 原始資料:條件篩選 -----
        Dictionary<string, string> search = new Dictionary<string, string>();

        #region >> 條件篩選 <<

        string Model_No = tb_Model_No.Text.Trim();
        string Class_ID = ddl_Class_ID.SelectedValue;
        string Keyword = tb_Keyword.Text.Trim();
        string Vol = tb_Vol.Text.Trim();
        string IsExpired = cb_IsExpired.Checked ? "Y" : "N";
        string IsCE = cb_IsCE.Checked ? "Y" : "N";
        string IsCheck = cb_IsCheck.Checked ? "Y" : "N";
        string BothCE = (cb_IsCE.Checked && cb_IsCheck.Checked) ? "Y" : "N";
        string SelfCert = ddl_SelfCert.SelectedValue;

        //[查詢條件] - Model_No
        if (!string.IsNullOrWhiteSpace(Model_No))
        {
            search.Add("Model_No", Model_No);
        }
        //[查詢條件] - Class_ID
        if (!string.IsNullOrWhiteSpace(Class_ID))
        {
            search.Add("Class_ID", Class_ID);
        }
        //[查詢條件] - Keyword
        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            search.Add("Keyword", Keyword);
        }
        //[查詢條件] - Vol
        if (!string.IsNullOrWhiteSpace(Vol))
        {
            search.Add("Vol", Vol);
        }
        //[查詢條件] - Self_Cert
        if (!string.IsNullOrWhiteSpace(SelfCert))
        {
            search.Add("SelfCert", SelfCert);
        }

        //[查詢條件] - IsExpired
        if (IsExpired.Equals("Y"))
        {
            search.Add("IsExpired", IsExpired);
        }

        //[查詢條件] - IsCE && IsCheck
        if (BothCE.Equals("Y"))
        {
            search.Add("BothCE", BothCE);
        }
        else
        {
            //[查詢條件] - IsCE
            if (IsCE.Equals("Y"))
            {
                search.Add("IsCE", IsCE);
            }
            //[查詢條件] - IsCheck
            if (IsCheck.Equals("Y"))
            {
                search.Add("IsCheck", IsCheck);
            }
        }

        #endregion

        //----- 原始資料:取得所有資料 -----
        var query = LookupExpData(search, out ErrMsg).AsEnumerable()
            .Select(fld => new
            {
                自有認證 = fld.Field<string>("Self_Cert"),
                品號 = fld.Field<string>("ModelNo"),
                類別 = fld.Field<string>("ClassName"),
                目錄 = fld.Field<string>("Catelog_Vol"),
                頁次 = fld.Field<string>("Page"),
                出貨地 = fld.Field<string>("Ship_From"),
                廠商 = fld.Field<string>("Supplier"),
                廠商料號 = fld.Field<string>("Supplier_ItemNo"),
                備註 = fld.Field<string>("Remark"),
                證書類別 = fld.Field<string>("Cert_Type"),
                證書類別說明 = fld.Field<string>("Cert_TypeText"),
                發證日期 = fld.Field<DateTime?>("Cert_ApproveDate"),
                有效日期 = fld.Field<DateTime?>("Cert_ValidDate"),
                證書編號 = fld.Field<string>("Cert_No"),
                認證指令 = fld.Field<string>("Cert_Cmd"),
                認證規範 = fld.Field<string>("Cert_Norm"),
                測試器_主機_安全等級 = fld.Field<string>("Cert_Desc1"),
                測試棒_安全等級 = fld.Field<string>("Cert_Desc2"),
            });

        //Check null
        if (query.Count() == 0)
        {
            CustomExtension.AlertMsg("查無資料", "");
            return;
        }

        //將IQueryable轉成DataTable
        DataTable myDT = CustomExtension.LINQToDataTable(query);

        //release
        query = null;

        //匯出Excel
        CustomExtension.ExportExcel(
            myDT
            , "{0}-{1}.xlsx".FormatThis("認證明細", DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd"))
            , false);
    }


    /// <summary>
    /// 資料匯出Excel
    /// </summary>
    /// <returns></returns>
    private DataTable LookupExpData(Dictionary<string, string> search, out string ErrMsg)
    {
        //----- 宣告:資料參數 -----
        StringBuilder sql = new StringBuilder(); //SQL語法容器

        try
        {
            //SQL 查詢
            using (SqlCommand cmd = new SqlCommand())
            {
                sql.AppendLine(" SELECT Base.Self_Cert, RTRIM(Base.Model_No) AS ModelNo, Cls.Class_Name_zh_TW AS ClassName");
                sql.AppendLine("     , Prod.Catelog_Vol, Prod.Page, Prod.Ship_From");
                sql.AppendLine("     , Base.Supplier, Base.Supplier_ItemNo, Base.Remark");
                sql.AppendLine("     , DT.Cert_Type, DT.Cert_TypeText, DT.Cert_ApproveDate, DT.Cert_ValidDate");
                sql.AppendLine("     , DT.Cert_No, DT.Cert_Cmd, DT.Cert_Norm, DT.Cert_Desc1, DT.Cert_Desc2");
                sql.AppendLine(" FROM Prod_Certification Base");
                sql.AppendLine("     INNER JOIN Prod_Certification_Detail DT ON Base.Cert_ID = DT.Cert_ID");
                sql.AppendLine("     INNER JOIN Prod_Item Prod ON Base.Model_No = Prod.Model_No");
                sql.AppendLine("     INNER JOIN Prod_Class Cls ON Prod.Class_ID = Cls.Class_ID");
                sql.AppendLine(" WHERE (1=1)");

                #region >> filter <<

                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    //查詢內容
                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "Model_No":
                                sql.Append(" AND (UPPER(Base.Model_No) LIKE UPPER(@Model_No) + '%')");
                                cmd.Parameters.AddWithValue("Model_No", item.Value);

                                break;

                            case "Class_ID":
                                sql.Append(" AND (Prod.Class_ID = @Class_ID)");
                                cmd.Parameters.AddWithValue("Class_ID", item.Value);

                                break;

                            case "Keyword":
                                //關鍵字
                                sql.Append(" AND (");
                                sql.Append("  (DT.Cert_Cmd LIKE '%' + @Keyword + '%') OR (DT.Cert_No LIKE '%' + @Keyword + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);

                                break;

                            case "Vol":
                                //目錄
                                sql.Append(" AND (Prod.Catelog_Vol LIKE '%' + @Vol + '%')");

                                cmd.Parameters.AddWithValue("Vol", item.Value);

                                break;

                            case "IsExpired":
                                //認證是否到期
                                sql.Append(" AND (DT.Cert_ValidDate < GETDATE())");

                                break;

                            case "IsCE":
                                //自我宣告
                                sql.Append(" AND (");
                                sql.Append("  (DT.Cert_File_CE <> '') OR (DT.Cert_File_CE_en_US <> '') OR (DT.Cert_File_CE_zh_CN <> '')");
                                sql.Append(" )");

                                break;

                            case "IsCheck":
                                //自我檢測
                                sql.Append(" AND (");
                                sql.Append("  (DT.Cert_File_Check <> '') OR (DT.Cert_File_Check_en_US <> '') OR (DT.Cert_File_Check_zh_CN <> '')");
                                sql.Append(" )");

                                break;

                            case "BothCE":
                                //自我宣告 + 自我檢測
                                sql.Append(" AND (");
                                sql.Append("  (");
                                sql.Append("  (DT.Cert_File_CE <> '') OR (DT.Cert_File_CE_en_US <> '') OR (DT.Cert_File_CE_zh_CN <> '')");
                                sql.Append(" )");
                                sql.Append(" OR (");
                                sql.Append("  (DT.Cert_File_Check <> '') OR (DT.Cert_File_Check_en_US <> '') OR (DT.Cert_File_Check_zh_CN <> '')");
                                sql.Append(" )");
                                sql.Append(")");

                                break;

                            case "SelfCert":
                                //自有認證
                                sql.Append(" AND (Base.Self_Cert = @Self_Cert) ");
                                cmd.Parameters.AddWithValue("Self_Cert", item.Value);

                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY Base.Model_No ASC, DT.Cert_ApproveDate DESC");

                cmd.CommandText = sql.ToString();

                return dbConClass.LookupDT(cmd, out ErrMsg);
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    /// <summary>
    /// 搜尋
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Cert_Search.aspx?func=cert");

            //[查詢條件] - Model_No(品號)
            if (string.IsNullOrEmpty(this.tb_Model_No.Text) == false)
                SBUrl.Append("&Model_No=" + Server.UrlEncode(this.tb_Model_No.Text.Trim()));

            //[查詢條件] - Class_ID(類別)
            if (this.ddl_Class_ID.SelectedIndex > 0)
                SBUrl.Append("&Class_ID=" + Server.UrlEncode(this.ddl_Class_ID.SelectedValue));

            //[查詢條件] - Keyword
            if (string.IsNullOrEmpty(this.tb_Keyword.Text) == false)
                SBUrl.Append("&Keyword=" + Server.UrlEncode(this.tb_Keyword.Text.Trim()));

            //[查詢條件] - IsExpired(認證是否到期)
            if (this.cb_IsExpired.Checked)
                SBUrl.Append("&IsExpired=Y");

            //[查詢條件] - Vol
            if (string.IsNullOrEmpty(this.tb_Vol.Text) == false)
                SBUrl.Append("&Vol=" + Server.UrlEncode(this.tb_Vol.Text.Trim()));

            //[查詢條件] - IsCE
            if (this.cb_IsCE.Checked)
                SBUrl.Append("&IsCE=Y");

            //[查詢條件] - IsCheck
            if (this.cb_IsCheck.Checked)
                SBUrl.Append("&IsCheck=Y");

            //[查詢條件] - Self_Cert
            if (this.ddl_SelfCert.SelectedIndex > 0)
                SBUrl.Append("&Self_Cert=" + Server.UrlEncode(this.ddl_SelfCert.SelectedValue));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            CustomExtension.AlertMsg("系統發生錯誤 - 搜尋！", "");
        }
    }

    #endregion


    /// <summary>
    /// [參數] - Disk資料夾路徑
    /// </summary>
    private string _Param_DiskFolder;
    public string Param_DiskFolder
    {
        get
        {
            return Application["File_DiskUrl"] + @"Certification\";
        }
        set
        {
            this._Param_DiskFolder = value;
        }
    }

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
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return fn_Param.WebUrl + "Certification/Cert_Search.aspx";
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;
}