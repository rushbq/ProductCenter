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

public partial class Prod_DtlEdit : SecurityIn
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
                this.lb_Model_No.Text = Param_ModelNo;
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
                LookupData(Param_ModelNo);
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
                SBSql.AppendLine("     , Icon_Pics.Pic_File AS SpecIcon ");
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
                SBSql.AppendLine("     LEFT JOIN Icon_Rel_Spec RelIcon ON RelIcon.SpecID = Rel.SpecID ");
                SBSql.AppendLine("     LEFT JOIN Icon_Pics ON Icon_Pics.Pic_ID = RelIcon.Pic_ID ");
                SBSql.AppendLine(" WHERE (Cate.Display = 'Y') AND (Spec.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID, Val.Spec_ListID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("尚未設定「規格類別」或「規格關聯」！", "Prod_Edit.aspx?Model_No=" + Param_ModelNo);
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
                        string Spec_Rank = DT.Rows[row]["Spec_Rank"].ToString();    //群組編號 - 規格
                        string ListID = DT.Rows[row]["Spec_ListID"].ToString().Trim();  //值 - DB系統編號
                        string ListSymbol = DT.Rows[row]["ListSymbol"].ToString().Trim();  //值 - 前置描述/符號
                        string ListValue = DT.Rows[row]["ListValue"].ToString().Trim();    //值 - 編號/輸入文字
                        string Spec_OptionName = DT.Rows[row]["Spec_OptionName_zh_TW"].ToString().Trim();  //值 - 文字描述
                        string IconPic = DT.Rows[row]["SpecIcon"].ToString();   //規格符號
                        int BOMSpec_Cnt = Convert.ToInt32(DT.Rows[row]["BOMSpec_Cnt"]);   //規格設定數
                        int BOMVal_Cnt = Convert.ToInt32(DT.Rows[row]["BOMVal_Cnt"]);   //組合明細已設定

                        #endregion

                        //[HTML] - 顯示 規格分類, 每類標頭 (Cate_Rank = 1)
                        if (Convert.ToInt16(Cate_Rank).Equals(1))
                        {
                            if (row > 0) html.AppendLine("</tbody>"); //tbody - 縮合功能使用

                            html.AppendLine("<tr class=\"ModifyHead DTtoggle\" rel=\"#dt" + row + "\" imgrel=\"#img" + row + "\" title=\"收合\" style=\"cursor: pointer\">");
                            html.AppendLine("<td colspan=\"6\">");
                            //--Title Left
                            html.AppendLine(" <div style=\"float:left\">");
                            //顯示箭頭圖片
                            html.Append("<img src=\"../images/icon_top.png\" id=\"img" + row + "\" />");
                            //顯示標頭
                            html.Append(CateName + "<em class=\"TableModifyTitleIcon\"></em>");
                            html.AppendLine(" </div>");
                            //--Title Right
                            html.AppendLine(" <div style=\"float:right\">");
                            //儲存鈕
                            html.Append("<input type=\"button\" class=\"doSave btnBlock colorBlue\" rel=\"dt" + row + "\" value=\"資料儲存\" style=\"width: 90px\" />");
                            //Loading圖示
                            html.Append("<img src=\"../images/loadingAnimation.gif\" class=\"Loading\" style=\"display: none;\" />");
                            html.AppendLine(" </div>");
                            html.AppendLine("</td>");
                            html.AppendLine("</tr>");
                            html.AppendLine("<tbody id=\"dt" + row + "\">"); //tbody - 縮合功能使用

                        }
                        //[HTML] - 顯示 規格欄位  
                        if (Spec_Rank.ToString().Equals("1"))
                        {
                            //[Table] - Row
                            html.AppendLine(string.Format("<tr {0}>"
                                , IsRequired.Equals("Y") ? "class=\"Must\"" : ""));

                            //[Table] - Column (Head), 左方欄位 ,Start  ----------
                            html.AppendLine(string.Format(" <td class=\"TableModifyTdHead\" style=\"width: 80px\">{1}<em class=\"Font10\">{0}</em></td>"
                                , SpecID
                                , IsRequired.Equals("Y") ? "<em>(*)</em>" : ""
                                ));

                            html.AppendLine(string.Format(" <td class=\"TableModifyTdHead\" style=\"width: 300px\"><span class=\"styleBlack\">{0}</span> (<span class=\"styleCafe\">{1}</span>)</td>"
                                , SpecName
                                , fn_Desc.Prod.InputType(SpecType)
                                ));
                            //[Table] - Column (Head), 左方欄位 ,End  ----------

                            //[Table] - Column (Icon), 符號欄 ,Start  ----------
                            html.AppendLine(string.Format("<td class=\"TableModifyTd\" style=\"width: 20px; text-align:center;\">{0}</td>"
                                , IconUrl(IconPic)));
                            //[Table] - Column (Icon), 符號欄 ,End  ----------

                            //[Table] - Column (Delete), 刪除欄 ,Start  ----------
                            html.AppendLine("<td class=\"TableModifyTd\" style=\"width: 20px; text-align:center;\">");
                            html.AppendLine((string.IsNullOrEmpty(ListValue)) ? "" :
                                string.Format("<span class=\"JQ-ui-state-default Remove\" title=\"移除填寫值\" style=\"cursor:pointer\" SpecID=\"{0}\" SpecType=\"{1}\" CateID=\"{2}\">" +
                                    "<span class=\"JQ-ui-icon ui-icon-trash\"></span></span>"
                                , SpecID
                                , SpecType
                                , CateID)
                            );
                            html.AppendLine("</td>");
                            //[Table] - Column (Delete), 刪除欄 ,End  ----------

                            //[Table] - Column (BOM), BOM編輯按扭 ,Start  ----------
                            string BOMUrl = "";
                            if (BOMSpec_Cnt > 0)
                            {
                                BOMUrl = ("<span class=\"JQ-ui-icon ui-icon-folder-open\"></span>{0}".FormatThis(
                                             "<a class=\"styleBluelight\" href=\"Prod_BOM_DtlEdit.aspx?Model_No={0}&CateID={1}&SpecClassID={2}&SpecID={3}\">組合明細</a> {4}"
                                              .FormatThis(
                                               Server.UrlEncode(ModelNo)
                                               , CateID
                                               , Param_SpecClass
                                               , SpecID
                                               , BOMVal_Cnt > 0 ? "<span class=\"JQ-ui-icon ui-icon-check\" title=\"已設定\"></span>" : "")
                                         ));
                            }
                            html.AppendLine("<td class=\"TableModifyTd\" style=\"width: 100px; text-align:center;\">{0}</td>".FormatThis(BOMUrl));
                            //[Table] - Column (BOM), BOM編輯按扭 ,End  ----------

                            //[Table] - Column (Content), 右方欄位 ,Start ----------
                            html.AppendLine("<td class=\"TableModifyTd\">");

                            #region * 內容欄位處理 Start *
                            /* LinQ to DataTable
                             * 功用: 取得同規格編號的其他設定資料
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
                                    qryOptName = el.Field<string>("Spec_OptionName_zh_TW")
                                };
                            if (query.Count() == 0)
                            {
                                //產生輸入欄 (無資料)
                                html.Append(Generate_Ctrl(SpecID, SpecType, OptionGID, IsRequired, ListID, "", "", "", CateID));
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
                                    //OptionName, 暫存值 - 文字描述
                                    aryValName.Add(item.qryOptName);
                                    //ListSymbol, 暫存符號(非空白 & 只會有一筆)
                                    if (false == string.IsNullOrEmpty(item.qrySymbol)) itemSymbol = item.qrySymbol;
                                }
                                //產生輸入欄 (有資料), 多值欄位皆組合成以"||||"為分隔的字串
                                html.Append(Generate_Ctrl(SpecID, SpecType, OptionGID, IsRequired
                                    , string.Join("||||", aryID.ToArray())
                                    , itemSymbol
                                    , string.Join("||||", aryVal.ToArray())
                                    , string.Join("||||", aryValName.ToArray())
                                    , CateID
                                    ));
                            }

                            query = null;

                            #endregion * 內容欄位處理 End *

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
    /// <param name="SpecID">SpecID</param>
    /// <param name="SpecType">輸入方式</param>
    /// <param name="OptionGID">選單單頭</param>
    /// <param name="IsRequired">是否必填</param>
    /// <param name="ListID">DB系統編號 - 值</param>
    /// <param name="SymbolValue">符號欄 - 值</param>
    /// <param name="InputValue">輸入欄 - 值</param>
    /// <param name="InputValueName">輸入欄 - 值(文字描述)</param>
    /// <param name="CateID">規格類別編號</param>
    /// <returns></returns>
    private string Generate_Ctrl(string SpecID, string SpecType, string OptionGID, string IsRequired, string ListID
        , string SymbolValue, string InputValue, string InputValueName
        , string CateID)
    {
        StringBuilder html = new StringBuilder();

        //判斷輸入方式
        switch (SpecType.ToUpper())
        {
            //[格式] ____ 開窗(不要放dataid, 每次Save皆是清空後Insert)
            case "SINGLESELECT":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"val_{7}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                                " required=\"{3}\" dataid=\"{4}\" value=\"{5}\" cateid=\"{7}\" style=\"width:100px; display:none;\" readonly>" +
                                " {6} "
                                , SpecID, SpecType, OptionGID, IsRequired
                                , ""
                                , InputValue
                                , option_Url(SpecID, SpecType, OptionGID, CateID, InputValue, InputValueName)
                                , CateID));

                break;

            //[格式] ____ 開窗(不要放dataid, 每次Save皆是清空後Insert)
            case "MULTISELECT":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"val_{7}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                                " required=\"{3}\" dataid=\"{4}\" value=\"{5}\" cateid=\"{7}\" style=\"width:100px; display:none;\" readonly>" +
                                " {6} "
                                , SpecID, SpecType, OptionGID, IsRequired
                                , ""
                                , InputValue
                                , option_Url(SpecID, SpecType, OptionGID, CateID, InputValue, InputValueName)
                                , CateID));
                break;

            //[格式] ____
            case "INT":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"val_{7}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                                " required=\"{3}\" dataid=\"{5}\" value=\"{6}\" cateid=\"{7}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {4}>"
                                , SpecID, SpecType, OptionGID, IsRequired
                                , validString_Num()
                                , ListID
                                , InputValue
                                , CateID));
                break;

            case "SINGLETYPE":
                html.Append(string.Format(
                                "<input type=\"text\" id=\"val_{6}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                                " required=\"{3}\" dataid=\"{4}\" value=\"{5}\" cateid=\"{6}\" style=\"width:400px\" maxlength=\"100\"> "
                                , SpecID, SpecType, OptionGID, IsRequired
                                , ListID
                                , InputValue
                                , CateID));
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
                               "<input type=\"text\" id=\"val_min_{7}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                               " required=\"{3}\" dataid=\"{5}\" value=\"{6}\" cateid=\"{7}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {4}> "
                               , SpecID, SpecType, OptionGID, IsRequired
                               , validString_Num()
                               , (aryID != null) ? aryID[0] : ""
                               , (aryVal != null) ? aryVal[0] : ""
                               , CateID
                               ));
                //符號選單
                html.Append(menu_Symbol("symbol_" + CateID + SpecID, SpecType.ToUpper(), SymbolValue));

                //結束欄
                html.Append(string.Format(
                               "<input type=\"text\" id=\"val_max_{7}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                               " required=\"{3}\" dataid=\"{5}\" value=\"{6}\" cateid=\"{7}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {4}> "
                               , SpecID, SpecType, OptionGID, IsRequired
                               , validString_Num()
                               , (aryID.Length == 2 && aryID != null) ? aryID[1] : ""
                               , (aryVal.Length == 2 && aryVal != null) ? aryVal[1] : ""
                               , CateID
                               ));
                break;

            //[格式] 符號 ____
            case "GREATERSMALL":
                //符號選單
                html.Append(menu_Symbol("symbol_" + CateID + SpecID, SpecType.ToUpper(), SymbolValue));

                //輸入欄位
                html.Append(string.Format(
                               "<input type=\"text\" id=\"val_{7}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                               " required=\"{3}\" dataid=\"{5}\" value=\"{6}\" cateid=\"{7}\" style=\"width:100px;text-align:center;\" maxlength=\"8\" {4}> "
                               , SpecID, SpecType, OptionGID, IsRequired
                               , validString_Num()
                               , ListID
                               , InputValue
                               , CateID
                               ));
                break;

            //[格式] ____ Textarea
            case "MULTITYPE":
                html.Append(string.Format(
                              "<textarea cols=\"80\" rows=\"3\" id=\"val_{6}{0}\" specid=\"{0}\" kind=\"{1}\" optgid=\"{2}\" " +
                              " required=\"{3}\" dataid=\"{4}\" cateid=\"{6}\">{5}</textarea>"
                              , SpecID, SpecType, OptionGID, IsRequired
                              , ListID
                              , InputValue
                              , CateID
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
    /// <param name="SpecID">規格編號</param>
    /// <param name="SpecType">輸入方式</param>
    /// <param name="OptionGID">選單單頭</param>
    /// <param name="CateID">規格類別編號</param>
    /// <param name="inputValue">輸入欄 - 值</param>
    /// <param name="InputValueName">輸入欄 - 值(文字描述)</param>
    /// <returns></returns>
    private string option_Url(string SpecID, string SpecType, string OptionGID, string CateID
        , string inputValue, string InputValueName)
    {
        if (string.IsNullOrEmpty(OptionGID))
        {
            return "<span class=\"styleEarth\">(選單未設定)</span>";
        }
        else
        {
            string[] strAry = Regex.Split(inputValue, @"\|{4}");
            return string.Format(
                "<a id=\"url_{5}{0}\" class=\"{3}\" href=\"Prod_Dtl_WinBox.aspx?IsMulti={2}&OptionGID={1}&SpecID={0}&CateID={5}\">{4}</a>"
                , SpecID
                , OptionGID
                //單選或複選
                , SpecType.ToUpper() == "SINGLESELECT" ? "N" : "Y"
                //Css
                , (string.IsNullOrEmpty(inputValue)) ? "selectBox" : "selectBox styleBlue"
                //顯示文字
                , (string.IsNullOrEmpty(inputValue)) ? "請先選擇項目" : string.Format("<div style=\"float:left\">{1}</div><div style=\"float:right\"><span class=\"badge badge-warning\">{0}</span></div>"
                    , strAry.Length, InputValueName.Replace("||||", ", "))
                , CateID
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

    public string Param_SpecClass { get; set; }

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
