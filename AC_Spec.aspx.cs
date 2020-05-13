using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;

/// <summary>
/// 列出規格欄位
/// </summary>
public partial class AC_Spec : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[檢查參數] - 查詢關鍵字
            string keywordString = "";
            if (null != Request["q"])
            {
                keywordString = fn_stringFormat.Filter_Html(Request["q"].Trim());
            }
            if (string.IsNullOrEmpty(keywordString))
            {
                Response.Write("");
                return;
            }
            if (keywordString.Length < 1)
            {
                Response.Write("");
                return;
            }

            //[參數宣告] - SqlCommand
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT TOP 100 (SpecID + ' - ' + SpecName_zh_TW) AS label, SpecID ");
                SBSql.AppendLine(" FROM Prod_Spec WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (");
                SBSql.AppendLine("   (SpecID LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("    OR (SpecName_zh_TW LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("    OR (SpecName_en_US LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("    OR (SpecName_zh_CN LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine(" ) ");
                SBSql.AppendLine(" ORDER BY Sort, SpecID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));                
                //[參數宣告] - DataTable
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    StringBuilder SBHtml = new StringBuilder();
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        SBHtml.Clear();
                        SBHtml.Append(DT.Rows[i]["label"].ToString().Replace("|", "｜"));
                        SBHtml.AppendLine("|" + DT.Rows[i]["SpecID"].ToString().Replace("|", "｜"));
                        Response.Write(SBHtml);
                    }
                }
            }

        }

    }
}