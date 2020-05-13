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

/// <summary>
/// Tags
/// </summary>
public partial class AC_ProdTags : System.Web.UI.Page
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

            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine("SELECT TOP 100 Tag_ID AS id, Tag_Name AS label ");
                SBSql.AppendLine(" FROM Prod_Tags WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (1 = 1)");
                SBSql.AppendLine(" AND ( ");
                SBSql.AppendLine("      (Tag_Name LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine(" ) ");
                SBSql.AppendLine(" ORDER BY Tag_Name ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKWeb, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        Response.Write("");
                    }
                    else
                    {
                        Response.Write(JsonConvert.SerializeObject(DT, Formatting.Indented));
                    }
                }
            }

        }

    }
}