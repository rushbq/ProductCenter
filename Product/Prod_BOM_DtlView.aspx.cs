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
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Linq;

public partial class Prod_BOM_DtlView : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[權限判斷] - 品規資料
                if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //[權限判斷] - 按鈕權限
                if (fn_CheckAuth.CheckAuth_User("121", out ErrMsg))
                {
                    this.ph_Url.Visible = true;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Product/Prod_Search.aspx";

                //[取得/檢查參數] - Model_No (品號)
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                    return;
                }

                //[帶出資料]
                LookupData();
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("     BOMSpec.SpecID, BOMSpec.BOM_SpecID, ISNULL(Val.RowID, 1) AS RowID, ISNULL(Val.Sort, 999) AS ValSort ");
                SBSql.AppendLine("     , BOMSpec.SpecName_zh_TW AS BOM_SpecName, BOMSpec.SpecType, BOMSpec.OptionGID, BOMSpec.SpecDESC ");
                SBSql.AppendLine("     , ISNULL(Val.Spec_ListID, 0) AS Spec_ListID, Val.ListSymbol, Val.ListValue ");
                SBSql.AppendLine("     , Opt.Spec_OptionName_zh_TW AS Spec_OptionName ");
                SBSql.AppendLine("     , (SELECT CateName_zh_TW FROM Prod_Spec_Category WHERE (CateID = @CateID)) AS CateName ");
                SBSql.AppendLine("     , (SELECT SpecName_zh_TW FROM Prod_Spec WHERE (SpecID = BOMSpec.SpecID)) AS SpecName ");
                SBSql.AppendLine("     , Val.Spec_ListID, Val.ListSymbol, Val.ListValue ");
                SBSql.AppendLine("     , IconPic.Pic_File AS OptIcon, SpecIcon.Pic_File AS SpecIcon ");
                SBSql.AppendLine(" FROM ");
                SBSql.AppendLine("     Prod_BOMSpec BOMSpec ");
                SBSql.AppendLine("     INNER JOIN Prod_BOMSpec_List Val ON BOMSpec.BOM_SpecID = Val.BOM_SpecID ");
                SBSql.AppendLine("      AND BOMSpec.SpecID = Val.SpecID AND Val.Model_No = @Model_No");
                SBSql.AppendLine("      AND Val.CateID = @CateID AND Val.SpecClassID = @SpecClassID");
                SBSql.AppendLine("     LEFT JOIN Prod_BOMSpec_Option Opt ON Opt.OptionGID = BOMSpec.OptionGID AND Opt.Spec_OptionValue = Val.ListValue ");
                SBSql.AppendLine("     LEFT JOIN Icon_Rel_BOMSpec RelIcon ON RelIcon.BOM_SpecID = BOMSpec.BOM_SpecID ");
                SBSql.AppendLine("     LEFT JOIN Icon_Pics IconPic ON Opt.Spec_OptionPic = IconPic.Pic_ID ");   //選單符號
                SBSql.AppendLine("     LEFT JOIN Icon_Pics SpecIcon ON SpecIcon.Pic_ID = RelIcon.Pic_ID "); //規格符號
                SBSql.AppendLine(" WHERE (BOMSpec.Display = 'Y') AND (BOMSpec.SpecID = @SpecID)");
                SBSql.AppendLine(" ORDER BY BOMSpec.Sort, BOMSpec.BOM_SpecID, Val.Spec_ListID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                cmd.Parameters.AddWithValue("CateID", Param_CateID);
                cmd.Parameters.AddWithValue("SpecClassID", Param_SpecClassID);
                cmd.Parameters.AddWithValue("SpecID", Param_SpecID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("尚未設定「組合明細」！", "script:history.back(-1);");
                        return;
                    }

                    //標頭訊息
                    this.lb_CateName.Text = DT.Rows[0]["CateName"].ToString();
                    this.lb_ModelNo.Text = Param_ModelNo;
                    this.lb_SpecInfo.Text = "{0} - {1}".FormatThis(DT.Rows[0]["SpecID"].ToString(), DT.Rows[0]["SpecName"].ToString());


                    //[宣告]
                    StringBuilder html = new StringBuilder();
                    int x = 0, y = 0;   //變數
                    List<TempParam> ITempList = new List<TempParam>();


                    //** 取得標題Group(BOM Spec Name) **
                    var queryGP =
                        from el in DT.AsEnumerable()
                        orderby el.Field<string>("BOM_SpecID"), el.Field<int>("ValSort") ascending
                        group el by new
                        {
                            BomID = el.Field<string>("BOM_SpecID"),
                            BomName = el.Field<string>("BOM_SpecName")
                        } into gp
                        select new
                        {
                            BomID = gp.Key.BomID,
                            BomName = gp.Key.BomName
                        };

                    //** 輸出標題Group(BOM Spec Name) **
                    html.AppendLine("<thead><tr>");

                    //[output Html]
                    foreach (var gpItem in queryGP)
                    {
                        html.AppendLine(" <th>{0}</th>".FormatThis(gpItem.BomName));
                    }

                    html.AppendLine("</tr></thead>");


                    //** Body內容 Start **
                    html.AppendLine("<tbody>");

                    #region -- 資料整理 --

                    //** 欄位條件List(直欄) Start **
                    foreach (var gpItem in queryGP)
                    {
                        //init
                        y = 0;

                        //取得欄位條件編號
                        string colID = gpItem.BomID;

                        //取得共用欄位值(取一筆)
                        var queryTbItems = (
                         from el in DT.AsEnumerable()
                         where el.Field<string>("BOM_SpecID").Equals(colID)
                         select new
                         {
                             getSpecID = el.Field<string>("SpecID"),
                             getSpecType = el.Field<string>("SpecType"),
                             getOptionGID = el.Field<string>("OptionGID")
                         }).Take(1);


                        //取得比對用的RowID, Sort
                        var queryRow =
                         from el in DT.AsEnumerable()
                         where el.Field<string>("BOM_SpecID").Equals(colID)
                         orderby el.Field<string>("BOM_SpecID"), el.Field<int>("ValSort"), el.Field<int>("Spec_ListID") ascending
                         group el by new
                         {
                             rowID = el.Field<int>("RowID"),
                             Sort = el.Field<int>("ValSort")
                         } into gp
                         select new
                         {
                             RowID = gp.Key.rowID,
                             Sort = gp.Key.Sort
                         };

                        #region -- 欄位內容 --
                        //比對RowID
                        foreach (var rows in queryRow)
                        {
                            //取得RowID, Sort
                            string RowID = rows.RowID.ToString();
                            string Sort = rows.Sort.ToString();

                            /*
                             * [取得同規格編號的其他設定資料]
                             * 條件: ListValue不為null, BOM_SpecID, RowID
                             * 結果:
                             *   - 無資料:直接顯示欄位
                             *   - 有資料:取得query後的資料，分析&組合後顯示欄位
                             */
                            var query =
                                from el in DT.AsEnumerable()
                                where el.Field<string>("BOM_SpecID").Equals(colID)
                                    && el.Field<string>("ListValue") != null
                                    && el.Field<int>("RowID").Equals(Convert.ToInt32(RowID))
                                select new
                                {
                                    qryListID = el.Field<int>("Spec_ListID"),
                                    qrySymbol = el.Field<string>("ListSymbol"),
                                    qryValue = el.Field<string>("ListValue"),
                                    qryOptName = el.Field<string>("Spec_OptionName"),
                                    qryOptIcon = el.Field<string>("OptIcon")
                                };
                            if (query.Count() > 0)
                            {
                                //多值判斷 & 拆解
                                ArrayList aryID = new ArrayList();
                                ArrayList aryVal = new ArrayList();
                                ArrayList aryValName = new ArrayList();
                                string itemSymbol = "";

                                foreach (var item in query)
                                {
                                    //ListID, 暫存編號
                                    aryID.Add(item.qryListID.ToString());
                                    //ListValue, 暫存值
                                    aryVal.Add(item.qryValue);
                                    //OptionName, 暫存值 - 符號 + 文字描述(選項名稱)
                                    aryValName.Add(IconUrl(item.qryOptIcon) + item.qryOptName);
                                    //ListSymbol, 暫存符號(非空白 & 只會有一筆)
                                    if (false == string.IsNullOrEmpty(item.qrySymbol)) itemSymbol = item.qrySymbol;
                                }

                                //產生輸入欄 (有資料), 多值欄位皆組合成以"||||"為分隔的字串
                                foreach (var itemCol in queryTbItems)
                                {
                                    string BomValue = Generate_Ctrl(itemCol.getSpecType
                                        , itemSymbol
                                        , string.Join("||||", aryVal.ToArray())
                                        , string.Join("||||", aryValName.ToArray())
                                        );

                                    //新增暫存設定
                                    ITempList.Add(new TempParam(x, y, BomValue));
                                }
                            }

                            query = null;

                            y++;

                        }
                        #endregion

                        queryTbItems = null;
                        queryRow = null;

                        x++;

                    }
                    //** 欄位條件List(直欄) End **

                    #endregion


                    #region -- 資料輸出 --

                    //** 取出暫存容器, 輸出Html Start **
                    if (ITempList.Count > 0)
                    {
                        var queryTemp = from el in ITempList
                                        select new
                                        {
                                            colX = el.idx_x,
                                            colY = el.idx_y,
                                            colVal = el.Value
                                        };
                        int colX = queryTemp.Max(el => el.colX);    //x最大值
                        int colY = queryTemp.Max(el => el.colY);    //y最大值

                        for (int row = 0; row <= colY; row++)
                        {
                            //[output Html]
                            html.AppendLine("<tr>");

                            for (int col = 0; col <= colX; col++)
                            {
                                var colVal = from el in queryTemp
                                             where el.colX.Equals(col) && el.colY.Equals(row)
                                             select el.colVal;

                                foreach (var item in colVal)
                                {
                                    //[output Html]
                                    html.AppendLine("<td>{0}</td>".FormatThis(item));
                                }
                            }

                            //[output Html]
                            html.AppendLine("</tr>");
                        }
                    }
                    //** 取出暫存容器, 輸出Html End **

                    #endregion

                    //** Body內容 End **
                    html.AppendLine("</tbody>");


                    //輸出全部Html
                    this.lt_Content.Text = html.ToString();
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion

    #region -- 自訂功能 --
    /// <summary>
    /// 輸出欄位 值
    /// </summary>
    /// <param name="SpecID">SpecID</param>
    /// <param name="SpecType">輸入方式</param>
    /// <param name="OptionGID">選單單頭</param>
    /// <param name="IsRequired">是否必填</param>
    /// <param name="ListID">DB系統編號 - 值</param>
    /// <param name="SymbolValue">符號欄 - 值</param>
    /// <param name="InputValue">輸入欄 - 值</param>
    /// <param name="InputValueName">輸入欄 - 值(文字描述)</param>
    /// <returns></returns>
    private string Generate_Ctrl(string SpecType, string SymbolValue, string InputValue, string InputValueName)
    {
        StringBuilder html = new StringBuilder();

        //判斷輸入方式
        switch (SpecType.ToUpper())
        {
            case "SINGLESELECT":
            case "MULTISELECT":
                html.Append(InputValueName.Replace("||||", ", "));
                break;

            case "INT":
            case "SINGLETYPE":
            case "GREATERSMALL":
                //符號
                html.Append((string.IsNullOrEmpty(SymbolValue) ? "" : fn_stringFormat.Chr(Convert.ToInt32(SymbolValue)).ToString()));
                html.Append(InputValue);
                break;

            case "DEVIATIONINT":
            case "BETWEENINT":
            case "INTGREATERSMALL":
            case "RATIO":
                //拆解值
                string[] aryVal = Regex.Split(InputValue, @"\|{4}");

                //開始欄
                if (aryVal != null)
                {
                    //判斷是否有平方符號, 若有的話則加上<sup></sup>
                    string leftStr = aryVal[0];
                    if (leftStr.IndexOf("^") == -1)
                    {
                        html.Append(leftStr);
                    }
                    else
                    {
                        string[] aryinVal = Regex.Split(leftStr, @"\^{1}");
                        html.Append(string.Format("{0}<sup>{1}</sup>", aryinVal[0], aryinVal[1]));
                    }
                }

                //符號選單
                html.Append((string.IsNullOrEmpty(SymbolValue) ? "" : fn_stringFormat.Chr(Convert.ToInt32(SymbolValue)).ToString()));

                //結束欄
                if (aryVal.Length == 2 && aryVal != null)
                {
                    //判斷是否有平方符號, 若有的話則加上<sup></sup>
                    string rightStr = aryVal[1];
                    if (rightStr.IndexOf("^") == -1)
                    {
                        html.Append(rightStr);
                    }
                    else
                    {
                        string[] aryinVal = Regex.Split(rightStr, @"\^{1}");
                        html.Append(string.Format("{0}<sup>{1}</sup>", aryinVal[0], aryinVal[1]));
                    }
                }
                break;

            //[格式] ____ Textarea
            case "MULTITYPE":
                html.Append(InputValue.Replace("\r\n", "<BR>"));
                break;
        }

        return html.ToString();
    }

    /// <summary>
    /// 取得符號圖片連結
    /// </summary>
    /// <param name="PicName">檔名</param>
    /// <returns>string</returns>
    public string IconUrl(string PicName)
    {
        if (string.IsNullOrEmpty(PicName))
        {
            return "";
        }

        return string.Format("<img src=\"{0}\" width=\"20px\" border=\"0\">"
            , Application["File_WebUrl"] + @"Icons/" + PicName);

    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return Request.QueryString["Model_No"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["Model_No"].Trim().ToUpper());
        }
        set
        {
            this._Param_ModelNo = value;
        }
    }

    /// <summary>
    /// 規格分類編號, CateID
    /// </summary>
    private string _Param_CateID;
    public string Param_CateID
    {
        get
        {
            return Request.QueryString["CateID"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["CateID"].Trim());
        }
        set
        {
            this._Param_CateID = value;
        }
    }

    /// <summary>
    /// 規格類別代號, SpecClassID
    /// </summary>
    private string _Param_SpecClassID;
    public string Param_SpecClassID
    {
        get
        {
            return Request.QueryString["SpecClassID"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["SpecClassID"].Trim());
        }
        set
        {
            this._Param_SpecClassID = value;
        }
    }

    /// <summary>
    /// 規格代號, SpecID
    /// </summary>
    private string _Param_SpecID;
    public string Param_SpecID
    {
        get
        {
            return Request.QueryString["SpecID"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["SpecID"].Trim());
        }
        set
        {
            this._Param_SpecID = value;
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

    #region -- 暫存參數 --

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - x
        /// </summary>
        private int _idx_x;
        public int idx_x
        {
            get { return this._idx_x; }
            set { this._idx_x = value; }
        }

        /// <summary>
        /// [參數] - y
        /// </summary>
        private int _idx_y;
        public int idx_y
        {
            get { return this._idx_y; }
            set { this._idx_y = value; }
        }

        /// <summary>
        /// [參數] - 值
        /// </summary>
        private string _Value;
        public string Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="idx_y">x</param>
        /// <param name="idx_y">y</param>
        /// <param name="Value">值</param>
        public TempParam(int idx_x, int idx_y, string Value)
        {
            this._idx_x = idx_x;
            this._idx_y = idx_y;
            this._Value = Value;
        }
    }
    #endregion

}
