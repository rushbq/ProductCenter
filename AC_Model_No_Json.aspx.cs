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

/// <summary>
/// 列出品號
/// </summary>
public partial class AC_Model_No_Json : System.Web.UI.Page
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
                SBSql.AppendLine(" SELECT TOP 100 ");
                SBSql.AppendLine("  RTRIM(Prod_Item.Model_No) AS label, ISNULL(Prod_Item.Ship_From, '') AS shipfrom ");
                SBSql.AppendLine("  , Prod_Class.Class_Name_zh_TW AS category, Prod_Class.Class_ID AS categoryID ");
                SBSql.AppendLine("  , ISNULL(Prod_Item.Catelog_Vol, '') AS vol, ISNULL(Prod_Item.Page, '') AS page");
                SBSql.AppendLine(" FROM Prod_Item WITH (NOLOCK) ");
                SBSql.AppendLine("  INNER JOIN Prod_Class WITH (NOLOCK) ON Prod_Item.Class_ID = Prod_Class.Class_ID ");
                SBSql.AppendLine(" WHERE (Prod_Item.Model_No LIKE '%' + @Keyword + '%') ");
                //[條件] - 過濾指定品號
                if (null != Request["CurrModelNo"])
                {
                    SBSql.AppendLine(" AND (Prod_Item.Model_No <> @CurrModelNo) ");
                    cmd.Parameters.AddWithValue("CurrModelNo", fn_stringFormat.Filter_Html(Request["CurrModelNo"].Trim()));
                }
                SBSql.AppendLine(" ORDER BY Prod_Class.Class_ID, label ");
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