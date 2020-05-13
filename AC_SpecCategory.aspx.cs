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
public partial class AC_SpecCategory : System.Web.UI.Page
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

            //[參數宣告] - SqlCommand
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT CateName_zh_TW AS label, CateID ");
                SBSql.AppendLine(" FROM Prod_Spec_Category WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (");
                SBSql.AppendLine("   (CateName_zh_TW LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("    OR (CateName_en_US LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("    OR (CateName_zh_CN LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine(" ) ");
                SBSql.AppendLine(" AND (Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort, CateID ");
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
                        SBHtml.AppendLine("|" + DT.Rows[i]["CateID"].ToString().Replace("|", "｜"));
                        Response.Write(SBHtml);
                    }
                }
            }

        }

    }
}