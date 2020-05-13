using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public partial class AC_VoltoPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[檢查參數] - 查詢關鍵字
            string keywordString = "";
            if (null != Request.Form["q"])
            {
                keywordString = fn_stringFormat.Filter_Html(Request.Form["q"].Trim());
            }

            //[參數宣告] - SqlCommand
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT TOP 100 Tbl.* FROM ( ");
                SBSql.AppendLine("     SELECT Catelog_Vol AS category ,Page AS label ");
                SBSql.AppendLine("     FROM Prod_Item WITH(NOLOCK) ");
                SBSql.AppendLine("     WHERE (Catelog_Vol <> '') AND (Page <> '') ");

                //[條件篩選] Start
                SBSql.AppendLine("      AND (");
                SBSql.AppendLine("        (Catelog_Vol LIKE '%' + @Keyword + '%')");
                SBSql.AppendLine("        OR (Page LIKE '%' + @Keyword + '%')");
                SBSql.AppendLine("      )");
                //[條件篩選] End

                SBSql.AppendLine("     GROUP BY Catelog_Vol, Page ");

                //SBSql.AppendLine("     UNION ALL ");

                //SBSql.AppendLine("     SELECT Catelog_Vol AS category ,Page AS label ");
                //SBSql.AppendLine("     FROM Prod_ItemSZ WITH(NOLOCK) ");
                //SBSql.AppendLine("     WHERE (Catelog_Vol <> '') AND (Page <> '') ");

                ////[條件篩選] Start
                //SBSql.AppendLine("      AND (");
                //SBSql.AppendLine("        (Catelog_Vol LIKE '%' + @Keyword + '%')");
                //SBSql.AppendLine("        OR (Page LIKE '%' + @Keyword + '%')");
                //SBSql.AppendLine("      )");
                ////[條件篩選] End

                //SBSql.AppendLine("     GROUP BY Catelog_Vol, Page ");
                SBSql.AppendLine(" ) AS Tbl ");
                SBSql.AppendLine(" GROUP BY Tbl.category, Tbl.label ");
                SBSql.AppendLine(" ORDER BY Tbl.category, Tbl.label ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));

                //[參數宣告] - DataTable
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                }
            }
        }

    }
}