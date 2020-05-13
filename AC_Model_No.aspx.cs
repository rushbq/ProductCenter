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
/// 列出品號/貨號
/// </summary>
public partial class AC_Model_No : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[檢查參數] - 查詢關鍵字
            string keywordString = Request.QueryString["q"].Trim();
            if (string.IsNullOrEmpty(keywordString))
            {
                Response.Write("");
                return;
            }
            if (keywordString.Length < 2)
            {
                Response.Write("");
                return;
            }

            //[參數宣告] - SqlCommand
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg = "";
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();
                //[判斷參數] - 來源
                switch (Request.QueryString["Srh_Type"])
                {
                    case "Item":  //貨號
                        SBSql.AppendLine("SELECT RTRIM(Item_No) AS Search_Value, '' AS Ship_From ");
                        SBSql.AppendLine(" FROM Prod_Item WITH (NOLOCK)");
                        SBSql.AppendLine(" WHERE (Item_No LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" GROUP BY Item_No ");
                        SBSql.AppendLine(" ORDER BY Item_No ");

                        break;

                    case "CertModel":  //品號 - 認證資料庫使用
                        SBSql.AppendLine("SELECT RTRIM(Model_No) AS Search_Value, Ship_From ");
                        SBSql.AppendLine(" FROM Prod_Item WITH (NOLOCK)");
                        SBSql.AppendLine(" WHERE (Model_No LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" ORDER BY Model_No ");

                        break;

                    case "PicModel":  //品號 - 複製時使用
                        SBSql.AppendLine("SELECT RTRIM(Model_No) AS Search_Value, Ship_From ");
                        SBSql.AppendLine(" FROM Prod_Item WITH (NOLOCK)");
                        SBSql.AppendLine(" WHERE (Model_No LIKE '%' + @Keyword + '%') AND (Model_No <> @CurrModelNo) ");
                        SBSql.AppendLine(" ORDER BY Model_No ");
                        cmd.Parameters.AddWithValue("CurrModelNo", Request.QueryString["CurrModelNo"].Trim());

                        break;

                    case "AllModel":  //品號
                        SBSql.AppendLine("SELECT RTRIM(Model_No) AS Search_Value, Ship_From ");
                        SBSql.AppendLine(" FROM Prod_Item WITH (NOLOCK)");
                        SBSql.AppendLine(" WHERE (Model_No LIKE '%' + @Keyword + '%') ");
                        SBSql.AppendLine(" ORDER BY Model_No ");

                        break;

                    default:
                        Response.Write("");
                        break;
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Keyword", keywordString.Replace("%", "[%]").Replace("_", "[_]"));

                //[參數宣告] - DataTable
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    StringBuilder SBHtml = new StringBuilder();
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        SBHtml.Clear();
                        SBHtml.Append(DT.Rows[i]["Search_Value"].ToString().Replace("|", "｜"));
                        SBHtml.AppendLine("|" + DT.Rows[i]["Ship_From"].ToString().Replace("|", "｜"));
                        Response.Write(SBHtml);
                        //output.Remove(0, output.Length);  //StringBuilder會自動GC
                    }
                }
            }

        }

    }
}