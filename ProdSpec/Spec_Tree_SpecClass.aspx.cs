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
using System.Collections;

public partial class Spec_Rel_SpecClass : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg = "";

                //[取得/檢查參數] - 規格分類
                if (false == fn_Extensions.String_字數(Request.QueryString["SpecClass"], "5", "5", out ErrMsg))
                {
                    this.lt_TreeView.Text = "&nbsp;<span class=\"styleBlue\">請先選擇左方分類..</span>";
                    return;
                }
                Param_ClassID = fn_stringFormat.Filter_Html(Request.QueryString["SpecClass"].ToString());

                //[取得資料] - 關聯資料
                StringBuilder SBHtml = new StringBuilder();
                if (CreateMenu(SBHtml, Param_ClassID, out ErrMsg))
                {
                    this.lt_TreeView.Text = SBHtml.ToString();
                }
                else
                {
                    this.lt_TreeView.Text = "&nbsp;<span class=\"styleRed\">查無資料.." + ErrMsg + "</span>";
                }
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 關聯資料！", "");
                return;
            }
        }
    }

    #region "資料取得"
    /// <summary>
    /// [建立選單] - 第 1~2 層
    /// </summary>
    /// <param name="SBHtml">Html</param>
    /// <param name="GUID">要查詢的編號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateMenu(StringBuilder SBHtml, string GUID, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SBHtml.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Class.SpecClassID, Class.ClassName_zh_TW, Spec.SpecID, Spec.SpecName_zh_TW, Spec.SpecType, Spec.OptionGID ");
                SBSql.AppendLine("    , (CASE WHEN Spec.OptionGID IS NULL THEN 0 ELSE 1 END) AS ChildCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Class Class ");
                SBSql.AppendLine("    INNER JOIN Prod_SpecClass_Rel_Spec Rel ON Class.SpecClassID = Rel.SpecClassID ");
                SBSql.AppendLine("    INNER JOIN Prod_Spec Spec ON Rel.SpecID = Spec.SpecID ");
                SBSql.AppendLine(" WHERE (Class.SpecClassID = @Param_ID) ");
                SBSql.AppendLine(" ORDER BY Spec.Sort, Spec.SpecID");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ID", GUID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return false;
                    }
                    SBHtml.AppendLine("<ul id=\"TreeView\" class=\"filetree\">");
                    //顯示第1層項目
                    SBHtml.AppendLine(string.Format(
                            " <li><span class=\"folder\"><a></a></span>&nbsp;<strong class=\"Font15\">{0} - {1}</strong>"
                            , DT.Rows[0]["SpecClassID"]
                            , DT.Rows[0]["ClassName_zh_TW"]));
                    SBHtml.AppendLine("  <ul>");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //顯示第2層項目
                        SBHtml.AppendLine(string.Format(
                            "<li><span class=\"{0}\"><a></a></span>&nbsp;{1} - {2}"
                            , SubMenuCss(Convert.ToInt16(DT.Rows[row]["ChildCnt"]))
                            , DT.Rows[row]["SpecID"]
                            , DT.Rows[row]["SpecName_zh_TW"]));

                        //判斷是否有下層資料並回傳
                        CreateSubMenu(DT.Rows[row]["OptionGID"].ToString(), SBHtml, out ErrMsg);

                        SBHtml.AppendLine("</li>");
                    }
                    SBHtml.AppendLine("  </ul>");
                    SBHtml.AppendLine(" </li>");
                    SBHtml.AppendLine("</ul>");
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
    /// [建立選單] - 第 3 層
    /// </summary>
    /// <param name="Up_ID">上一層編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateSubMenu(string Up_ID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            if (string.IsNullOrEmpty(Up_ID))
            {
                ErrMsg = "";
                return false;
            }
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT SpecOption.Spec_OptionValue, SpecOption.Spec_OptionName_zh_TW ");
                SBSql.AppendLine(" FROM Prod_Spec_Option SpecOption ");
                SBSql.AppendLine(" WHERE (SpecOption.OptionGID = @Param_ID) AND (SpecOption.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY SpecOption.Sort, SpecOption.Spec_OptionValue ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ID", Up_ID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        SBHtml.AppendLine("<ul>");
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //顯示第3層項目
                            SBHtml.AppendLine(string.Format(
                            "<li><span class=\"file\"><a></a></span>&nbsp;{0} - {1}"
                            , DT.Rows[row]["Spec_OptionValue"]
                            , DT.Rows[row]["Spec_OptionName_zh_TW"]));

                            SBHtml.AppendLine("</li>");
                        }
                        SBHtml.AppendLine("</ul>");
                    }
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
    /// [建立選單] - 判斷是否有子項目,顯示不同Css樣式
    /// </summary>
    /// <param name="Child_Cnt">子項目數</param>
    /// <returns></returns>
    private string SubMenuCss(int Child_Cnt)
    {
        if (Child_Cnt > 0)
            return "folder";
        else
            return "file";
    }
    #endregion

    //[參數] - 分類編號
    private string _Param_ClassID;
    public string Param_ClassID
    {
        get;
        set;
    }
}
