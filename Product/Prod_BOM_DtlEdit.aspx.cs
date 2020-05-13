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

public partial class Prod_BOM_DtlEdit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - 品規編輯
                if (fn_CheckAuth.CheckAuth_User("121", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
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

                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = "2";
                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;

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
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[清除參數]
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("     BOMSpec.SpecID, BOMSpec.BOM_SpecID, ISNULL(Val.RowID, 1) AS RowID, ISNULL(Val.Sort, 999) AS Sort ");
                SBSql.AppendLine("     , BOMSpec.SpecName_zh_TW AS BOMSpecName, BOMSpec.SpecType, BOMSpec.IsRequired, BOMSpec.OptionGID, BOMSpec.SpecDESC ");
                SBSql.AppendLine("     , ISNULL(Val.Spec_ListID, 0) AS Spec_ListID, Val.ListSymbol, Val.ListValue ");
                SBSql.AppendLine("     , Opt.Spec_OptionName_zh_TW ");
                SBSql.AppendLine("     , Icon_Pics.Pic_File AS SpecIcon ");
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY BOMSpec.BOM_SpecID ORDER BY BOMSpec.Sort, BOMSpec.BOM_SpecID, Val.Spec_ListID ASC) AS Spec_Rank ");
                SBSql.AppendLine("     , (SELECT CateName_zh_TW FROM Prod_Spec_Category WHERE (CateID = @CateID)) AS CateName ");
                SBSql.AppendLine("     , (SELECT SpecName_zh_TW FROM Prod_Spec WHERE (SpecID = BOMSpec.SpecID)) AS SpecName ");
                SBSql.AppendLine(" FROM ");
                SBSql.AppendLine("     Prod_BOMSpec BOMSpec ");
                SBSql.AppendLine("     LEFT JOIN Prod_BOMSpec_List Val ON BOMSpec.BOM_SpecID = Val.BOM_SpecID AND BOMSpec.SpecID = Val.SpecID AND Val.Model_No = @Model_No ");
                SBSql.AppendLine("      AND Val.CateID = @CateID AND Val.SpecClassID = @SpecClassID ");
                SBSql.AppendLine("     LEFT JOIN Prod_BOMSpec_Option Opt ON Opt.OptionGID = BOMSpec.OptionGID AND Opt.Spec_OptionValue = Val.ListValue ");
                SBSql.AppendLine("     LEFT JOIN Icon_Rel_BOMSpec RelIcon ON RelIcon.BOM_SpecID = BOMSpec.BOM_SpecID ");
                SBSql.AppendLine("     LEFT JOIN Icon_Pics ON Icon_Pics.Pic_ID = RelIcon.Pic_ID ");
                SBSql.AppendLine(" WHERE (BOMSpec.Display = 'Y') AND (BOMSpec.SpecID = @SpecID)");
                SBSql.AppendLine(" ORDER BY BOMSpec.Sort, BOMSpec.BOM_SpecID, Val.Spec_ListID ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                cmd.Parameters.AddWithValue("CateID", Param_CateID);
                cmd.Parameters.AddWithValue("SpecClassID", Param_SpecClassID);
                cmd.Parameters.AddWithValue("SpecID", Param_SpecID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("尚未設定「組合明細規格」！", "script:history.back(-1);");
                        return;
                    }

                    //標頭訊息
                    this.lb_CateName.Text = DT.Rows[0]["CateName"].ToString();
                    this.lb_ModelNo.Text = Param_ModelNo;
                    this.lb_SpecInfo.Text = "{0} - {1}".FormatThis(DT.Rows[0]["SpecID"].ToString(), DT.Rows[0]["SpecName"].ToString());

                    //[輸出Html]
                    StringBuilder html = new StringBuilder();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //[取得欄位資料]
                        #region * 取得欄位資料 *
                        string SpecID = DT.Rows[row]["SpecID"].ToString();
                        string BOM_SpecID = DT.Rows[row]["BOM_SpecID"].ToString();
                        string BOMSpecName = DT.Rows[row]["BOMSpecName"].ToString().Trim();
                        string SpecType = DT.Rows[row]["SpecType"].ToString().Trim();
                        string IsRequired = DT.Rows[row]["IsRequired"].ToString();
                        string OptionGID = DT.Rows[row]["OptionGID"].ToString().Trim();
                        string SpecDESC = DT.Rows[row]["SpecDESC"].ToString().Trim();
                        string Spec_Rank = DT.Rows[row]["Spec_Rank"].ToString();    //群組編號 - 規格
                        string ListID = DT.Rows[row]["Spec_ListID"].ToString().Trim();  //值 - DB系統編號
                        string ListSymbol = DT.Rows[row]["ListSymbol"].ToString().Trim();  //值 - 前置描述/符號
                        string ListValue = DT.Rows[row]["ListValue"].ToString().Trim();    //值 - 編號/輸入文字
                        string Spec_OptionName = DT.Rows[row]["Spec_OptionName_zh_TW"].ToString().Trim();  //值 - 文字描述
                        string IconPic = DT.Rows[row]["SpecIcon"].ToString();   //規格符號
                        #endregion

                        //[HTML] - 顯示 規格欄位  
                        if (Spec_Rank.ToString().Equals("1"))
                        {
                            //[Table] - Row
                            html.AppendLine(string.Format("<tr {0} id=\"{1}\">"
                                , IsRequired.Equals("Y") ? "class=\"Must\"" : "", BOM_SpecID
                                ));

                            //[Table] - Column (Head), 左方欄位 ,Start  ----------
                            html.AppendLine(string.Format(" <td class=\"TableModifyTdHead\" style=\"width: 250px\">{2}{1} - {0} (<span class=\"styleCafe\">{3}</span>)</td>"
                                , BOMSpecName
                                , BOM_SpecID
                                , IsRequired.Equals("Y") ? "<em>(*)</em>" : ""
                                , fn_Desc.Prod.InputType(SpecType)));
                            //[Table] - Column (Head), 左方欄位 ,End  ----------

                            //[Table] - Column (Icon), 符號欄 ,Start  ----------
                            html.AppendLine(string.Format("<td class=\"TableModifyTd\" style=\"width: 20px; text-align:center;\">{0}</td>"
                                , IconUrl(IconPic)));
                            //[Table] - Column (Icon), 符號欄 ,End  ----------

                            //[Table] - Column (Content), 右方欄位 ,Start ----------
                            html.AppendLine("<td class=\"TableModifyTd\">");
                            html.AppendLine("<ul class=\"FucList\">");

                            //新增功能 (Insert), 新增欄 ,Start  ----------
                            html.Append("<li style=\"background:#ede8f7;\">New：{0}&nbsp;{1}</li>".FormatThis(
                                Generate_Ctrl(BOM_SpecID, SpecType, OptionGID, IsRequired, ListID, "", "", "", "", "Insert")
                                , "<input type=\"button\" class=\"btnBlock colorGray doInsert\" value=\"嵌入\" SpecType=\"{0}\" BOMSpecID=\"{1}\" id=\"addBtn_{1}\">".FormatThis(
                                    SpecType, BOM_SpecID
                                    )
                                ));

                            //新增功能 (Insert), 新增欄 ,End  ----------

                            #region * 內容欄位處理 Start *
                            //GroupBy 取得RowID, Sort
                            var queryRow =
                                from el in DT.AsEnumerable()
                                where el.Field<string>("BOM_SpecID").Equals(BOM_SpecID)
                                orderby el.Field<string>("BOM_SpecID"), el.Field<int>("Sort"), el.Field<int>("Spec_ListID") ascending
                                group el by new
                                {
                                    rowID = el.Field<int>("RowID"),
                                    Sort = el.Field<int>("Sort")
                                } into gp
                                select new
                                {
                                    RowID = gp.Key.rowID,
                                    Sort = gp.Key.Sort
                                };

                            //比對RowID
                            foreach (var rows in queryRow)
                            {
                                //取得Group的RowID, Sort
                                string RowID = rows.RowID.ToString();
                                string Sort = rows.Sort.ToString();

                                html.AppendLine("<li id=\"{0}_{1}\">".FormatThis(BOM_SpecID, RowID));

                                //更新功能 (Save), 儲存欄 ,Start  ----------
                                html.AppendLine((string.IsNullOrEmpty(ListValue)) ? "" :
                                    "<span class=\"Save\" title=\"儲存\" style=\"cursor:pointer\" SpecType=\"{0}\" BOMSpecID=\"{1}\" RowID=\"{2}\"><span class=\"JQ-ui-icon ui-icon-disk\"></span></span>&nbsp;"
                                    .FormatThis(SpecType
                                        , BOM_SpecID
                                        , RowID)
                                    );
                                html.Append("<img src=\"{0}loading.gif\" id=\"load_{1}\" width=\"20\" alt=\"loading\" style=\"display:none\">"
                                    .FormatThis(Application["WebUrl"] + "images/"
                                        , BOM_SpecID + RowID)
                                    );
                                //更新功能 (Save), 儲存欄 ,End  ----------

                                //更新功能 (Delete), 刪除欄 ,Start  ----------
                                html.AppendLine((string.IsNullOrEmpty(ListValue)) ? "" :
                                    "<span class=\"Remove\" title=\"移除填寫值\" style=\"cursor:pointer\" SpecType=\"{0}\" BOMSpecID=\"{1}\" RowID=\"{2}\"><span class=\"JQ-ui-icon ui-icon-trash\"></span></span>"
                                    .FormatThis(SpecType
                                        , BOM_SpecID
                                        , RowID)
                                    );
                                //更新功能 (Delete), 刪除欄 ,End  ----------

                                /* LinQ to DataTable
                                 * 功用: 取得同規格編號的其他設定資料
                                 * 條件: ListValue不為null, BOM_SpecID, RowID
                                 * 結果:
                                 *   - 無資料:直接顯示欄位
                                 *   - 有資料:取得query後的資料，分析&組合後顯示欄位
                                 */
                                var query =
                                    from el in DT.AsEnumerable()
                                    where el.Field<string>("BOM_SpecID").Equals(BOM_SpecID)
                                        && el.Field<string>("ListValue") != null
                                        && el.Field<int>("RowID").Equals(Convert.ToInt32(RowID))
                                    select new
                                    {
                                        qryListID = el.Field<int>("Spec_ListID"),
                                        qrySymbol = el.Field<string>("ListSymbol"),
                                        qryValue = el.Field<string>("ListValue"),
                                        qryOptName = el.Field<string>("Spec_OptionName_zh_TW")
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
                                        //OptionName, 暫存值 - 文字描述
                                        aryValName.Add(item.qryOptName);
                                        //ListSymbol, 暫存符號(非空白 & 只會有一筆)
                                        if (false == string.IsNullOrEmpty(item.qrySymbol)) itemSymbol = item.qrySymbol;
                                    }
                                    //產生輸入欄 (有資料), 多值欄位皆組合成以"||||"為分隔的字串
                                    html.Append(Generate_Ctrl(BOM_SpecID, SpecType, OptionGID, IsRequired
                                        , string.Join("||||", aryID.ToArray())
                                        , itemSymbol
                                        , string.Join("||||", aryVal.ToArray())
                                        , string.Join("||||", aryValName.ToArray())
                                        , RowID
                                        , "Update"
                                        ));

                                    //排序Sort
                                    html.Append("<input type=\"text\" id=\"sort_{0}_{1}\" specid=\"{0}\" rowid=\"{1}\" value=\"{2}\" maxlength=\"6\" {3} style=\"width:60px;text-align:center;\" title=\"排序\">"
                                        .FormatThis(BOM_SpecID
                                            , RowID
                                            , Sort
                                            , validString_Num()
                                        ));

                                    //儲存時間
                                    html.Append("<span id=\"time_{0}\" class=\"styleEarth\"></span>".FormatThis(BOM_SpecID + RowID));
                                }

                                query = null;

                                html.AppendLine("</li>");
                            }

                            queryRow = null;

                            #endregion * 內容欄位處理 End *

                            html.AppendLine("</ul>");
                            html.AppendLine("</td>");
                            //[Table] - Column (Content) 右方欄位 ,End ----------

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

    #region -- 自訂功能 --
    /// <summary>
    /// 產生輸入欄
    /// </summary>
    /// <param name="BOM_SpecID">BOM_SpecID</param>
    /// <param name="SpecType">輸入方式</param>
    /// <param name="OptionGID">選單單頭</param>
    /// <param name="IsRequired">是否必填</param>
    /// <param name="ListID">DB系統編號 - 值</param>
    /// <param name="SymbolValue">符號欄 - 值</param>
    /// <param name="InputValue">輸入欄 - 值</param>
    /// <param name="InputValueName">輸入欄 - 值(文字描述)</param>
    /// <param name="CateID">規格類別編號</param>
    /// <returns></returns>
    /// <remarks>
    /// {0}_{1}_val
    ///  {0}:BOM_SpecID
    ///  {1}:RowID
    /// 
    /// </remarks>
    private string Generate_Ctrl(string BOM_SpecID, string SpecType, string OptionGID, string IsRequired, string ListID
        , string SymbolValue, string InputValue, string InputValueName
        , string RowID, string Mode)
    {
        StringBuilder html = new StringBuilder();

        //判斷輸入方式
        switch (SpecType.ToUpper())
        {
            //[格式] ____ 開窗(不要放dataid, 每次Save皆是清空後Insert)
            case "SINGLESELECT":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                                " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:100px; display:none;\" readonly>" +
                                " {7} "
                                , Mode.Equals("Insert") ? "{0}_newval".FormatThis(BOM_SpecID) : "{0}_{1}_val".FormatThis(BOM_SpecID, RowID)
                                , BOM_SpecID
                                , SpecType
                                , OptionGID
                                , IsRequired
                                , ""
                                , InputValue
                                , option_Url(BOM_SpecID, SpecType, OptionGID, InputValue, InputValueName, RowID, Mode)
                                ));
                break;

            //[格式] ____ 開窗(不要放dataid, 每次Save皆是清空後Insert)
            case "MULTISELECT":
                html.Append(string.Format(
                               "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                               " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:100px; display:none;\" readonly>" +
                               " {7} "
                               , Mode.Equals("Insert") ? "{0}_newval".FormatThis(BOM_SpecID) : "{0}_{1}_val".FormatThis(BOM_SpecID, RowID)
                               , BOM_SpecID
                               , SpecType
                               , OptionGID
                               , IsRequired
                               , ""
                               , InputValue
                               , option_Url(BOM_SpecID, SpecType, OptionGID, InputValue, InputValueName, RowID, Mode)
                               ));
                break;

            //[格式] ____
            case "INT":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                                " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {7}>"
                                , Mode.Equals("Insert") ? "{0}_newval".FormatThis(BOM_SpecID) : "{0}_{1}_val".FormatThis(BOM_SpecID, RowID)
                                , BOM_SpecID
                                , SpecType
                                , OptionGID
                                , IsRequired
                                , ListID
                                , InputValue
                                , validString_Num()
                                ));
                break;

            case "SINGLETYPE":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                                " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:400px\" maxlength=\"100\"> "
                                , Mode.Equals("Insert") ? "{0}_newval".FormatThis(BOM_SpecID) : "{0}_{1}_val".FormatThis(BOM_SpecID, RowID)
                                , BOM_SpecID
                                , SpecType
                                , OptionGID
                                , IsRequired
                                , ListID
                                , InputValue
                                ));
                break;

            //[格式] ____ 符號 ____
            case "DEVIATIONINT":
            case "BETWEENINT":
            case "INTGREATERSMALL":
            case "RATIO":
                //拆解值
                string[] aryID = Regex.Split(ListID, @"\|{4}");
                string[] aryVal = Regex.Split(InputValue, @"\|{4}");

                //開始欄
                html.Append(string.Format(
                               "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                               " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {7}> "
                               , Mode.Equals("Insert") ? "{0}_newval_min".FormatThis(BOM_SpecID) : "{0}_{1}_val_min".FormatThis(BOM_SpecID, RowID)
                               , BOM_SpecID
                               , SpecType
                               , OptionGID
                               , IsRequired
                               , (aryID != null) ? aryID[0] : ""
                               , (aryVal != null) ? aryVal[0] : ""
                               , validString_Num()
                               ));
                //符號選單
                html.Append(menu_Symbol(
                    Mode.Equals("Insert") ? "newsymbol_{0}".FormatThis(BOM_SpecID) : "symbol_{0}_{1}".FormatThis(BOM_SpecID, RowID)
                    , SpecType.ToUpper()
                    , SymbolValue));

                //結束欄
                html.Append(string.Format(
                               "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                               " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {7}> "
                               , Mode.Equals("Insert") ? "{0}_newval_max".FormatThis(BOM_SpecID) : "{0}_{1}_val_max".FormatThis(BOM_SpecID, RowID)
                               , BOM_SpecID
                               , SpecType
                               , OptionGID
                               , IsRequired
                               , (aryID.Length == 2 && aryID != null) ? aryID[1] : ""
                               , (aryVal.Length == 2 && aryVal != null) ? aryVal[1] : ""
                               , validString_Num()
                               ));
                break;

            //[格式] 符號 ____
            case "GREATERSMALL":
                //符號選單
                html.Append(menu_Symbol("symbol_" + BOM_SpecID, SpecType.ToUpper(), SymbolValue));

                //輸入欄位
                html.Append(string.Format(
                               "<input type=\"text\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                               " requiredfld=\"{4}\" dataid=\"{5}\" value=\"{6}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {7}> "
                               , Mode.Equals("Insert") ? "{0}_newval".FormatThis(BOM_SpecID) : "{0}_{1}_val".FormatThis(BOM_SpecID, RowID)
                               , BOM_SpecID
                               , SpecType
                               , OptionGID
                               , IsRequired
                               , ListID
                               , InputValue
                               , validString_Num()
                               ));
                break;

            //[格式] ____ Textarea
            case "MULTITYPE":
                html.Append(string.Format(
                              "<textarea cols=\"80\" rows=\"3\" id=\"{0}\" specid=\"{1}\" kind=\"{2}\" optgid=\"{3}\" " +
                              " requiredfld=\"{4}\" dataid=\"{5}\">{6}</textarea>"
                              , Mode.Equals("Insert") ? "{0}_newval".FormatThis(BOM_SpecID) : "{0}_{1}_val".FormatThis(BOM_SpecID, RowID)
                              , BOM_SpecID
                              , SpecType
                              , OptionGID
                              , IsRequired
                              , ListID
                              , InputValue
                              ));
                break;
        }

        return html.ToString();
    }

    /// <summary>
    /// 回傳字串 - js判斷, ,只能輸入數字(負數)(小數點)
    /// </summary>
    /// <returns></returns>
    private string validString_Num()
    {
        return "onkeyup=\"if(!this.value.match(/^[\\+\\-]?\\d*?\\.?\\d*?\\^?\\d*?$/))execCommand('undo')\" onafterpaste=\"if(!this.value.match(/^[\\+\\-]?\\d*?\\.?\\d*?\\^?\\d*?$/))execCommand('undo')\"";
    }

    /// <summary>
    /// 下拉選單 - 符號表
    /// </summary>
    /// <param name="inputID">選單ID</param>
    /// <param name="inputType">選單類別</param>
    /// <param name="inputValue">選單值</param>
    /// <returns></returns>
    private string menu_Symbol(string inputID, string inputType, string inputValue)
    {
        StringBuilder html = new StringBuilder();

        html.Append("<select id=\"" + inputID + "\">");

        #region * 輸入方式Xml *
        //取得符號
        string XmlResult = fn_Extensions.WebRequest_GET(Application["File_WebUrl"] + "Xml_Data/ProdSpecType.xml");
        if (string.IsNullOrEmpty(XmlResult))
        {
            return "";
        }
        //將Xml字串轉成byte
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
        //讀取Xml
        using (XmlReader reader = XmlTextReader.Create(stream))
        {
            //使用XElement載入Xml
            XElement XmlDoc = XElement.Load(reader);

            var Results = from result in XmlDoc.Elements("SpecType")
                          where result.Attribute("ID").Value.ToUpper().Equals(inputType.ToUpper())
                          select new
                          {
                              Icons = result.Element("Icons").Elements()
                          };
            foreach (var result in Results)
            {
                //無資料, 回傳空值
                if (result.Icons.Count() == 0)
                {
                    return "";
                }
                //選項資料
                foreach (var icon in result.Icons)
                {
                    html.Append(string.Format("<option value=\"{0}\" {2}>{1}</option>"
                        , icon.Value
                        , fn_stringFormat.Chr(Convert.ToInt32(icon.Value))
                        , icon.Value.Equals(inputValue) ? "selected" : ""));
                }
            }
        }
        #endregion

        html.Append("</select>");

        return html.ToString();
    }

    /// <summary>
    /// 開窗 - 選單選擇視窗
    /// </summary>
    /// <param name="BOM_SpecID">規格編號</param>
    /// <param name="SpecType">輸入方式</param>
    /// <param name="OptionGID">選單單頭</param>
    /// <param name="inputValue">輸入欄 - 值</param>
    /// <param name="InputValueName">輸入欄 - 值(文字描述)</param>
    /// <param name="RowID">RowID</param>
    /// <param name="Mode">新增或修改</param>
    /// <returns></returns>
    /// <remarks>
    /// url_{0}_{1}
    ///  {0}:BOM_SpecID
    ///  {1}:RowID
    /// 
    /// </remarks>
    private string option_Url(string BOM_SpecID, string SpecType, string OptionGID, string inputValue, string InputValueName, string RowID, string Mode)
    {
        if (string.IsNullOrEmpty(OptionGID))
        {
            return "<span class=\"styleEarth\">(選單未設定)</span>";
        }
        else
        {
            string[] strAry = Regex.Split(inputValue, @"\|{4}");
            return string.Format(
                "<a id=\"{0}\" class=\"{4}\" href=\"Prod_BOM_Dtl_WinBox.aspx?IsMulti={3}&OptionGID={2}&SpecID={1}&RowID={6}&Mode={7}\">{5}</a>"
                , "url_{0}_{1}".FormatThis(BOM_SpecID, RowID)
                , BOM_SpecID
                , OptionGID
                , SpecType.ToUpper() == "SINGLESELECT" ? "N" : "Y"  //單選或複選
                , (string.IsNullOrEmpty(inputValue)) ? "selectBox styleGreen" : "selectBox styleBlue"
                , (string.IsNullOrEmpty(inputValue)) ? "請先選擇項目" : string.Format("已選取 {0} 個項目：{1}"
                    , strAry.Length, InputValueName.Replace("||||", ", "))  //顯示文字
                , RowID
                , Mode
               );
        }
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

    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}Product/Prod_BOM_DtlEdit.aspx?Model_No={1}&CateID={2}&SpecClassID={3}&SpecID={4}".FormatThis(
             Application["WebUrl"]
             , Server.UrlEncode(Param_ModelNo)
             , Param_CateID
             , Param_SpecClassID
             , Param_SpecID
            );
        }
        private set { this._ValidCode = value; }
    }

    /// <summary>
    /// 產生MD5驗証碼
    /// SessionID + 登入帳號 + 自訂字串
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get { return Cryptograph.MD5(Session.SessionID + fn_Param.CurrentAccount + System.Web.Configuration.WebConfigurationManager.AppSettings["ValidCode_Pwd"], 32); }
        private set { this._ValidCode = value; }
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
