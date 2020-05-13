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
/// 列出認證資料有新增過的廠商
/// </summary>
public partial class AC_Cert_Supplier : System.Web.UI.Page
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
                SBSql.AppendLine("SELECT TOP 100 Supplier AS label FROM Prod_Certification WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Supplier LIKE '%' + @Keyword + '%') ");
                SBSql.AppendLine(" GROUP BY Supplier ");
                SBSql.AppendLine(" ORDER BY Supplier ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
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