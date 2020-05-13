using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.IO;

public partial class SpecSummary_byClass_Excel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string ErrMsg;
           
            StringBuilder SBHtml = new StringBuilder();            
            if (false == GetMenu_1st("", SBHtml, out ErrMsg)) {
                Response.Write("<div class=\"styleRed\">查無資料..<BR>" + ErrMsg + "</div>");
                return;
            }
            string dtNow = DateTime.Now.ToString().ToDateString("yyyyMMdd");
            Response.AddHeader("content-disposition", "attachment; filename=SpecSummary_" + dtNow + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Write("<meta http-equiv=Content-Type content=text/html;charset=utf-8>");
            StringWriter tw = new System.IO.StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            Label excel = new Label();
            excel.Text = SBHtml.ToString();
            excel.RenderControl(hw);
            System.Text.Encoding.GetEncoding("UTF-8");
            Response.Write(tw.ToString());
        }
    }

    #region "資料取得"
    /// <summary>
    /// [查詢資料] - 表頭(小類別)
    /// </summary>
    /// <param name="chkClassID">小類別(比對用)</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    /// <remarks>
    /// 取得大/小分類，暫存小分類編號(Array) - 欄
    /// </remarks>
    private bool GetMenu_1st(string chkClassID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[清除參數]
                cmd.Parameters.Clear();
                SBHtml.Clear();

                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ");
                SBSql.AppendLine(" SELECT lv1.SpecClassID AS MtClassID, lv1.ClassName_zh_TW AS MtClassName ");
                SBSql.AppendLine("  , Lv2.SpecClassID AS SubClassID, Lv2.ClassName_zh_TW AS SubClassName ");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY lv1.ClassName_zh_TW ORDER BY lv1.Sort, lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ASC) AS Lv1_Rank ");
                //計數, 第2層的項數
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Prod_Spec_Class Sub WHERE (Sub.UpClass = Lv1.SpecClassID) AND (Sub.Display = 'Y')) AS SubCnt ");
                //計數, 第3層的項數
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Prod_SpecClass_Rel_Spec Rel WHERE (Rel.SpecClassID = Lv2.SpecClassID)) AS ChildCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Class AS Lv1 ");
                SBSql.AppendLine("  LEFT JOIN Prod_Spec_Class AS Lv2 ON Lv1.SpecClassID = Lv2.UpClass ");
                SBSql.AppendLine(" WHERE (Lv1.Display = 'Y') AND (Lv1.UpClass IS NULL) AND (Lv2.Display = 'Y')  ");
                SBSql.AppendLine(" ORDER BY Lv1.Sort, Lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return true;
                    }

                    SBHtml.AppendLine("<table>");

                    //[Html] - 第 1 層 Start
                    SBHtml.AppendLine("<tr align=\"center\">");
                    SBHtml.Append("<th rowspan=\"2\">分類/欄位</th>");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        if (DT.Rows[row]["Lv1_Rank"].ToString().Equals("1"))
                        {
                            SBHtml.Append(string.Format("<th colspan=\"{2}\">{0}-{1}</th>"
                                , DT.Rows[row]["MtClassID"].ToString()
                                , DT.Rows[row]["MtClassName"].ToString()
                                , DT.Rows[row]["SubCnt"].ToString()));
                        }
                    }
                    SBHtml.AppendLine("</tr>");
                    //[Html] - 第 1 層 End

                    //[宣告暫存] - 分類編號
                    ArrayList aryClassID = new ArrayList();

                    //[Html] - 第 2 層 Start
                    SBHtml.AppendLine("<tr align=\"center\">");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        SBHtml.AppendLine(string.Format("<th>{0}-{1}</th>"
                                , DT.Rows[row]["SubClassID"].ToString()
                                , DT.Rows[row]["SubClassName"].ToString()));

                        //[暫存參數] = 子分類
                        aryClassID.Add(DT.Rows[row]["SubClassID"].ToString());
                    }
                    SBHtml.AppendLine("</tr>");
                    //[Html] - 第 2 層 End
                    
                    //下層Html
                    if (aryClassID.Count == 0)
                    {
                        ErrMsg = "無資料顯示..."; ;
                        return true;
                    }
                    if (false == GetMenu_2nd(chkClassID, aryClassID, SBHtml, out ErrMsg))
                    {
                        ErrMsg = "壞掉囉~!<BR>" + ErrMsg;
                        return false;
                    }

                    SBHtml.AppendLine("</table>");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// [查詢資料] - 各類內容
    /// </summary>
    /// <param name="chkClassID">小類別(比對用)</param>
    /// <param name="ClassID">小分類編號(Array)</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    /// <remarks>
    /// 依分類編號，查詢條件範圍內的Spec資料，並將Spec暫存 - 列
    /// </remarks>
    private bool GetMenu_2nd(string chkClassID, ArrayList aryClassID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[清除參數]
                cmd.Parameters.Clear();

                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ");
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("    Spec.SpecID, Spec.SpecName_zh_TW, Spec.SpecType, Spec.OptionGID ");
                SBSql.AppendLine("    , Opt.OptionID, Opt.Spec_OptionValue, Opt.Spec_OptionName_zh_TW ");
                SBSql.AppendLine("    , Rel.SpecClassID ");
                //[SQL] - 計數, 類別範圍下的選項數               
                SBSql.AppendLine("    , ( ");
                SBSql.AppendLine("      SELECT COUNT(*) from Prod_Spec inSpec ");
                SBSql.AppendLine("       INNER JOIN Prod_Spec_Option inOpt ON Opt.OptionGID = inSpec.OptionGID ");
                SBSql.AppendLine("       INNER JOIN Prod_SpecClass_Rel_Spec inRel ON inSpec.SpecID = inRel.SpecID ");
                SBSql.AppendLine("      WHERE (inOpt.Display = 'Y') AND (inSpec.Display = 'Y') ");
                SBSql.AppendLine("       AND (inOpt.OptionGID = Spec.OptionGID) AND (inSpec.SpecID = Spec.SpecID) ");
                //[SQL] - 代入暫存參數(分類編號)
                SBSql.AppendLine("       AND (inRel.SpecClassID IN(" + GetSQLParam(aryClassID, "ParamTmp") + ")) ");
                SBSql.AppendLine("    ) AS ChildCnt ");
                //[SQL] - 群組編號
                SBSql.AppendLine("    , ROW_NUMBER() OVER(PARTITION BY Spec.SpecID ORDER BY Spec.SpecID, Opt.Sort, Opt.Spec_OptionValue ASC) AS GP_Rank ");
                SBSql.AppendLine("   FROM Prod_Spec AS Spec ");
                SBSql.AppendLine("        INNER JOIN Prod_SpecClass_Rel_Spec Rel ON Spec.SpecID = Rel.SpecID ");
                SBSql.AppendLine("        LEFT JOIN Prod_Spec_Option Opt ON Spec.OptionGID = Opt.OptionGID AND Opt.Display = 'Y' ");
                SBSql.AppendLine("   WHERE (Spec.Display = 'Y') ");
                //[SQL] - 代入暫存參數(分類編號)
                SBSql.AppendLine(" AND (Rel.SpecClassID IN(" + GetSQLParam(aryClassID, "ParamTmp") + ")) ");
                for (int row = 0; row < aryClassID.Count; row++)
                {
                    cmd.Parameters.AddWithValue("ParamTmp" + row, aryClassID[row]);
                }
                SBSql.AppendLine(" ORDER BY Spec.Sort, Spec.SpecName_zh_TW, Spec.SpecID, Opt.Sort, Opt.Spec_OptionValue ");
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return true;
                    }

                    #region -- 暫存處理 --

                    //[宣告暫存參數]
                    List<TempGPData> listGP = new List<TempGPData>();
                    ArrayList aryItem_ID = new ArrayList();
                    ArrayList aryItem_Name = new ArrayList();
                    ArrayList aryItem_Value = new ArrayList();
                    ArrayList aryItem_ClassID = new ArrayList();

                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        int GP_Rank = Convert.ToInt16(DT.Rows[row]["GP_Rank"]);
                        int ChildCnt = Convert.ToInt16(DT.Rows[row]["ChildCnt"]);

                        //[暫存] - 無子項目, 加入群組 (內容為空值)
                        if (ChildCnt == 0 && GP_Rank == 1)
                        {
                            listGP.Add(new TempGPData(
                                DT.Rows[row]["SpecID"].ToString()
                                , DT.Rows[row]["SpecName_zh_TW"].ToString()
                                , DT.Rows[row]["SpecType"].ToString()
                                , DT.Rows[row]["OptionGID"].ToString()
                                , null, null, null, null));
                        }

                        //[暫存] - 加入項目Array (有選項值者)
                        if (ChildCnt > 0)
                        {
                            aryItem_ID.Add(DT.Rows[row]["OptionID"].ToString());
                            aryItem_Name.Add(DT.Rows[row]["Spec_OptionName_zh_TW"].ToString());
                            aryItem_Value.Add(DT.Rows[row]["Spec_OptionValue"].ToString());
                            aryItem_ClassID.Add(DT.Rows[row]["SpecClassID"].ToString());

                            //[暫存] - 判斷為最後一筆時, 加入群組 & 項目資料 (使用子項數與群組編號比對)
                            if (ChildCnt == GP_Rank)
                            {
                                //[暫存] - 宣告新的Array來接收 (*)
                                ArrayList aryNew_ID = new ArrayList(aryItem_ID);
                                ArrayList aryNew_Name = new ArrayList(aryItem_Name);
                                ArrayList aryNew_Value = new ArrayList(aryItem_Value);
                                ArrayList aryNew_ClassID = new ArrayList(aryItem_ClassID);

                                listGP.Add(new TempGPData(
                                   DT.Rows[row]["SpecID"].ToString()
                                   , DT.Rows[row]["SpecName_zh_TW"].ToString()
                                   , DT.Rows[row]["SpecType"].ToString()
                                   , DT.Rows[row]["OptionGID"].ToString()
                                   , aryNew_ID, aryNew_Name, aryNew_Value, aryNew_ClassID));

                                //清除目前暫存的Array
                                aryItem_ID.Clear();
                                aryItem_Name.Clear();
                                aryItem_Value.Clear();
                                aryItem_ClassID.Clear();
                            }
                        }
                    }
                    #endregion

                    #region -- 資料顯示 --

                    //[Html] - 顯示欄位內容(群組迴圈)
                    for (int row = 0; row < listGP.Count; row++)
                    {
                        //取得群組資料
                        string SpecID = listGP[row].GP_ID;
                        string SpecName = listGP[row].GP_Name;
                        string SpecType = listGP[row].GP_Type;
                        string OptionGID = listGP[row].GP_OptGID;

                        //[Html] - 左方欄位
                        SBHtml.AppendLine("<tr>");
                        SBHtml.Append(string.Format(
                                "<td>{0} ({1})</td>"
                                , SpecName
                                , fn_Desc.Prod.InputType(SpecType)
                                ));

                        //判斷Item資料是否為空
                        if (listGP[row].Item_ID != null)
                        {
                            //[Html] - 右方內容欄位 Start (類別迴圈)
                            for (int idx = 0; idx < aryClassID.Count; idx++)
                            {
                                SBHtml.Append(" <td>");
                                //(Item迴圈)
                                for (int col = 0; col < listGP[row].Item_ID.Count; col++)
                                {
                                    //取得項目資料
                                    string Item_ID = listGP[row].Item_ID[col].ToString();
                                    string Item_Name = listGP[row].Item_Name[col].ToString();
                                    string Item_Value = listGP[row].Item_Value[col].ToString();
                                    string Item_ClassID = listGP[row].Item_ClassID[col].ToString();
                                    //判斷分類編號是否符合
                                    if (aryClassID[idx].ToString().Equals(Item_ClassID))
                                    {
                                        SBHtml.Append(string.Format(
                                                " {0} - {1}<br/>"
                                                , Item_Value
                                                , Item_Name
                                                ));
                                    }
                                }
                                SBHtml.Append(" </td>");
                            }
                            //[Html] - 右方內容欄位 End
                        }
                        //else
                        //{
                        //    //[Html] - 右方內容(空)欄位 Start (類別迴圈)
                        //    for (int idx = 0; idx < ClassID.Count; idx++)
                        //    {
                        //        //string btnAdd = string.Format("<a href='{0}' class='EditBox'>關聯</a>"
                        //        //    , "../ProdSpec/Spec_Rel_SpecClass.aspx?SpecClass=" + Server.UrlEncode(ClassID[idx].ToString()) + "&Keyword=" + Server.UrlEncode(SpecID));

                        //        //要比對的分類加上hightlight
                        //        string currClass = "";
                        //        if (ClassID[idx].Equals(chkClassID))
                        //        {
                        //            currClass = "class=\"BgPink styleBlue B\"";
                        //        }
                        //        SBHtml.Append(" <td " + currClass + ">");
                        //        SBHtml.AppendLine(" </td>");
                        //    }
                        //    //[Html] - 右方內容(空)欄位 End (類別迴圈)
                        //}

                        SBHtml.AppendLine("</tr>");
                    }
                    #endregion
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// SQL參數組合 - Where IN
    /// </summary>
    /// <param name="listSrc">來源資料(List)</param>
    /// <param name="paramName">參數名稱</param>
    /// <returns>參數字串</returns>
    string GetSQLParam(ArrayList listSrc, string paramName)
    {
        if (listSrc.Count == 0)
        {
            return "";
        }

        //組合參數字串
        ArrayList aryParam = new ArrayList();
        for (int row = 0; row < listSrc.Count; row++)
        {
            aryParam.Add(string.Format("@{0}{1}", paramName, row));
        }
        //回傳以 , 為分隔符號的字串
        return string.Join(",", aryParam.ToArray());
    }
    #endregion

    #region -- 參數設定 Start --
    public class TempGPData
    {
        /// <summary>
        /// 群組編號
        /// </summary>
        private string _GP_ID;
        public string GP_ID
        {
            get { return this._GP_ID; }
            set { this._GP_ID = value; }
        }
        /// <summary>
        /// 群組名稱
        /// </summary>
        private string _GP_Name;
        public string GP_Name
        {
            get { return this._GP_Name; }
            set { this._GP_Name = value; }
        }
        /// <summary>
        /// 群組類型
        /// </summary>
        private string _GP_Type;
        public string GP_Type
        {
            get { return this._GP_Type; }
            set { this._GP_Type = value; }
        }
        /// <summary>
        /// 群組選項代號
        /// </summary>
        private string _GP_OptGID;
        public string GP_OptGID
        {
            get { return this._GP_OptGID; }
            set { this._GP_OptGID = value; }
        }

        /// <summary>
        /// 項目編號
        /// </summary>
        private ArrayList _Item_ID;
        public ArrayList Item_ID
        {
            get { return this._Item_ID; }
            set { this._Item_ID = value; }
        }
        /// <summary>
        /// 項目名稱
        /// </summary>
        private ArrayList _Item_Name;
        public ArrayList Item_Name
        {
            get { return this._Item_Name; }
            set { this._Item_Name = value; }
        }
        /// <summary>
        /// 項目值
        /// </summary>
        private ArrayList _Item_Value;
        public ArrayList Item_Value
        {
            get { return this._Item_Value; }
            set { this._Item_Value = value; }
        }
        /// <summary>
        /// 項目類別ID
        /// </summary>
        private ArrayList _Item_ClassID;
        public ArrayList Item_ClassID
        {
            get { return this._Item_ClassID; }
            set { this._Item_ClassID = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="GP_ID">群組編號</param>
        /// <param name="GP_Name">群組名稱</param>
        /// <param name="GP_Type">群組類別</param>
        /// <param name="GP_OptGID">群組選項代號</param>
        /// <param name="Item_ID">項目編號(Array)</param>
        /// <param name="Item_Name">項目名稱(Array)</param>
        /// <param name="Item_Value">項目值(Array)</param>
        public TempGPData(string GP_ID, string GP_Name, string GP_Type, string GP_OptGID
              , ArrayList Item_ID, ArrayList Item_Name, ArrayList Item_Value, ArrayList Item_ClassID)
        {
            this._GP_ID = GP_ID;
            this._GP_Name = GP_Name;
            this._GP_Type = GP_Type;
            this._GP_OptGID = GP_OptGID;

            this._Item_ID = Item_ID;
            this._Item_Name = Item_Name;
            this._Item_Value = Item_Value;
            this._Item_ClassID = Item_ClassID;
        }
    }

    #endregion -- 參數設定 End --

}