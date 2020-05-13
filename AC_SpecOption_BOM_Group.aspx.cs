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
/// 列出欄位屬性有新增過的群組
/// </summary>
public partial class AC_SpecOption_BOM_Group : SecurityIn
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
                string ErrMsg = "";
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT (OptionGID + ' - ' + OptionGName) AS FullName, OptionGID ");
                SBSql.AppendLine(" FROM Prod_BOMSpec_OptionGroup WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (");
                SBSql.AppendLine("   (OptionGID LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine("    OR (OptionGName LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine(" ) ");
                SBSql.AppendLine(" ORDER BY Sort, OptionGID ");
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
                        SBHtml.Append(DT.Rows[i]["FullName"].ToString().Replace("|", "｜"));
                        SBHtml.AppendLine("|" + DT.Rows[i]["OptionGID"].ToString().Replace("|", "｜"));
                        Response.Write(SBHtml);
                    }
                }
            }
            
        }
       
    }
}