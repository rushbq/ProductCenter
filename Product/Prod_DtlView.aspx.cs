using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ExtensionMethods;
using Resources;

public partial class Prod_DtlView : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

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
                Param_ModelNo = string.IsNullOrEmpty(Request.QueryString["Model_No"]) ? "" : fn_stringFormat.Filter_Html(Request.QueryString["Model_No"].ToString().Trim());
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                    return;
                }

                //[帶出資料]
                LookupData(Param_ModelNo);

                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = "2";
                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;
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
    /// <param name="ModelNo">品號</param>
    private void LookupData(string ModelNo)
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[清除參數]
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Cate.CateID, Cate.CateName_zh_TW AS CateName ");
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY Cate.CateID ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID, Val.Spec_ListID ASC) AS Cate_Rank ");
                SBSql.AppendLine("     , Rel.SpecClassID ");
                SBSql.AppendLine("     , Spec.SpecID, Spec.SpecName_zh_TW, Spec.SpecType, Spec.IsRequired, Spec.OptionGID, Spec.SpecDESC ");
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY Spec.SpecID, Cate.CateID ORDER BY Spec.Sort, Spec.SpecID, Val.Spec_ListID ASC) AS Spec_Rank ");
                SBSql.AppendLine("     , Val.Spec_ListID, Val.ListSymbol, Val.ListValue ");
                SBSql.AppendLine("     , Opt.Spec_OptionName_zh_TW ");
                SBSql.AppendLine("     , IconPic.Pic_File AS OptIcon, SpecIcon.Pic_File AS SpecIcon ");
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_BOMSpec WHERE (SpecID = Spec.SpecID)) AS BOMSpec_Cnt ");
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_BOMSpec_List ");
                SBSql.AppendLine("       WHERE (Model_No = Prod.Model_No) AND (CateID = Rel.CateID) AND (SpecClassID = Rel.SpecClassID) AND (SpecID = Spec.SpecID)) AS BOMVal_Cnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Category Cate ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec_Rel_Category Rel ON Cate.CateID = Rel.CateID ");
                SBSql.AppendLine("     INNER JOIN Prod_SpecClass_Rel_Spec ClassRel ON ClassRel.SpecClassID = Rel.SpecClassID AND ClassRel.SpecID = Rel.SpecID ");
                SBSql.AppendLine("     INNER JOIN Prod_Item Prod ON Rel.SpecClassID = Prod.SpecClassID AND Prod.Model_No = @Param_ModelNo ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec Spec ON Rel.SpecID = Spec.SpecID ");
                SBSql.AppendLine("     LEFT JOIN Prod_Spec_List Val ON Rel.CateID = Val.CateID AND Rel.SpecClassID = Val.SpecClassID AND Rel.SpecID = Val.SpecID AND Val.Model_No = Prod.Model_No ");
                SBSql.AppendLine("     LEFT JOIN Prod_Spec_Option Opt ON Opt.OptionGID = Spec.OptionGID AND Opt.Spec_OptionValue = Val.ListValue ");
                //選單符號
                SBSql.AppendLine("     LEFT JOIN Icon_Pics IconPic ON Opt.Spec_OptionPic = IconPic.Pic_ID ");
                //規格符號
                SBSql.AppendLine("     LEFT JOIN Icon_Rel_Spec RelIcon ON RelIcon.SpecID = Rel.SpecID ");
                SBSql.AppendLine("     LEFT JOIN Icon_Pics SpecIcon ON SpecIcon.Pic_ID = RelIcon.Pic_ID ");
                SBSql.AppendLine(" WHERE (Cate.Display = 'Y') AND (Spec.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID, Val.Spec_ListID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("尚未設定「規格類別」或「規格關聯」！", "Prod_View.aspx?Model_No=" + Param_ModelNo);
                    }
                    //[暫存規格類別]
                    Param_SpecClass = DT.Rows[0]["SpecClassID"].ToString();

                    //[輸出Html]
                    StringBuilder html = new StringBuilder();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //[取得欄位資料]
                        #region * 取得欄位資料 *
                        string CateID = DT.Rows[row]["CateID"].ToString();
                        string CateName = DT.Rows[row]["CateName"].ToString().Trim();
                        string Cate_Rank = DT.Rows[row]["Cate_Rank"].ToString();   //群組編號 - 規格分類
                        string SpecID = DT.Rows[row]["SpecID"].ToString();
                        string SpecName = DT.Rows[row]["SpecName_zh_TW"].ToString().Trim();
                        string SpecType = DT.Rows[row]["SpecType"].ToString().Trim();
                        string IsRequired = DT.Rows[row]["IsRequired"].ToString();
                        string OptionGID = DT.Rows[row]["OptionGID"].ToString().Trim();
                        string SpecDESC = DT.Rows[row]["SpecDESC"].ToString().Trim();
                        string Spec_Rank = DT.Rows[row]["Spec_Rank"].ToString();    //群組編號 - 規格欄位
                        string ListID = DT.Rows[row]["Spec_ListID"].ToString().Trim();  //值 - DB系統編號
                        string ListSymbol = DT.Rows[row]["ListSymbol"].ToString().Trim();  //值 - 前置描述/符號
                        string ListValue = DT.Rows[row]["ListValue"].ToString().Trim();    //值 - 編號/輸入文字
                        string IconPic = DT.Rows[row]["SpecIcon"].ToString();   //規格符號
                        //int BOMSpec_Cnt = Convert.ToInt32(DT.Rows[row]["BOMSpec_Cnt"]);   //規格設定數
                        int BOMVal_Cnt = Convert.ToInt32(DT.Rows[row]["BOMVal_Cnt"]);   //組合明細已設定

                        #endregion

                        //[HTML] - 顯示 規格分類, 每類標頭 (Cate_Rank = 1)
                        if (Convert.ToInt16(Cate_Rank).Equals(1))
                        {
                            if (row > 0) html.AppendLine("</tbody>"); //tbody - 縮合功能使用

                            html.AppendLine("<tr class=\"ModifyHead DTtoggle\" rel=\"#dt" + row + "\" imgrel=\"#img" + row + "\" title=\"收合\" style=\"cursor: pointer\">");
                            html.AppendLine("<td colspan=\"4\">");
                            //顯示箭頭圖片
                            html.Append("<img src=\"../images/icon_top.png\" id=\"img" + row + "\" />");
                            html.AppendLine(CateName + "<em class=\"TableModifyTitleIcon\"></em></td>");
                            html.AppendLine("</tr>");
                            html.AppendLine("<tbody id=\"dt" + row + "\">"); //tbody - 縮合功能使用
                        }
                        //[HTML] - 顯示 規格欄位 (Spec_Rank = 1)
                        if (Convert.ToInt16(Spec_Rank).Equals(1))
                        {
                            //[Table] - Row
                            html.AppendLine(string.Format("<tr {0}>"
                                , IsRequired.Equals("Y") ? "class=\"Must\"" : ""));

                            //[Table] - Column (Head), 左方欄位 ,Start  ----------
                            html.AppendLine(string.Format(" <td class=\"TableModifyTdHead\" style=\"width: 250px\">{0} </td>"
                                , SpecName));
                            //[Table] - Column (Head), 左方欄位 ,End  ----------

                            //[Table] - Column (Icon), 符號欄 ,Start  ----------
                            html.AppendLine(string.Format("<td class=\"TableModifyTd\" style=\"width: 20px; text-align:center;\">{0}</td>"
                                , IconUrl(IconPic)));
                            //[Table] - Column (Icon), 符號欄 ,End  ----------

                            //[Table] - Column (BOM), BOM編輯按扭 ,Start  ----------
                            string BOMUrl = "";
                            if (BOMVal_Cnt > 0)
                            {
                                BOMUrl = ("<span class=\"JQ-ui-icon ui-icon-folder-open\"></span></span>{0}".FormatThis(
                                             "<a class=\"styleBluelight\" href=\"Prod_BOM_DtlView.aspx?Model_No={0}&CateID={1}&SpecClassID={2}&SpecID={3}\">組合明細</a>"
                                              .FormatThis(Server.UrlEncode(ModelNo), CateID, Param_SpecClass, SpecID)
                                         ));
                            }
                            html.AppendLine("<td class=\"TableModifyTd\" style=\"width: 80px; text-align:center;\">{0}</td>".FormatThis(BOMUrl));
                            //[Table] - Column (BOM), BOM編輯按扭 ,End  ----------

                            //[Table] - Column (Content), 右方欄位 ,Start ----------
                            html.AppendLine("<td class=\"TableModifyTd\">");

                            #region * 內容欄位處理 Start *
                            /* LinQ to DataTable
                             * 功用: 在 Spec_Rank = 1 的狀況下，以該規格的條件，去取得其他同規格的資料
                             * 條件: CateID, SpecID, ListValue不為null
                             * 結果:
                             *   - 無資料:直接顯示欄位
                             *   - 有資料:取得query後的資料，分析&組合後顯示欄位
                             */
                            var query =
                                from el in DT.AsEnumerable()
                                where el.Field<int>("CateID").Equals(Convert.ToInt32(CateID))
                                    && el.Field<string>("SpecID").Equals(SpecID)
                                    && el.Field<string>("ListValue") != null
                                select new
                                {
                                    qryListID = el.Field<int>("Spec_ListID"),
                                    qrySymbol = el.Field<string>("ListSymbol"),
                                    qryValue = el.Field<string>("ListValue"),
                                    qryOptName = el.Field<string>("Spec_OptionName_zh_TW"),
                                    qryOptIcon = el.Field<string>("OptIcon")
                                };
                            if (query.Count() == 0)
                            {
                                //產生輸入欄 (無資料)
                                html.Append(Generate_Ctrl(SpecID, SpecType, OptionGID, IsRequired, ListID, "", "", ""));
                            }
                            else
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
                                html.Append(Generate_Ctrl(SpecID, SpecType, OptionGID, IsRequired
                                    , string.Join("||||", aryID.ToArray())
                                    , itemSymbol
                                    , string.Join("||||", aryVal.ToArray())
                                    , string.Join("||||", aryValName.ToArray())
                                    ));
                            }

                            query = null;

                            #endregion * 內容欄位處理 End *

                            html.AppendLine("</td>");
                            //[Table] - Column (Content) ,End ----------

                            html.AppendLine("</tr>");
                        }
                    }

                    //輸出Html
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

    #region -- 功能設定 --
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
    private string Generate_Ctrl(string SpecID, string SpecType, string OptionGID, string IsRequired, string ListID
        , string SymbolValue, string InputValue, string InputValueName)
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
                //string[] aryID = Regex.Split(ListID, @"\|{4}");
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
            return this._Param_ModelNo != null ? this._Param_ModelNo : this.lb_Model_No.Text.Trim().ToUpper();
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }

    public string Param_SpecClass { get; set; }

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
