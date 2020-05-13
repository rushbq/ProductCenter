using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Collections;
using ExtensionMethods;

/// <summary>
/// 左方功能選單
/// </summary>
/// <remarks>
/// 判斷權限
/// </remarks>
public partial class Left : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg = "";

                //[產生功能選單]
                StringBuilder SBHtml = new StringBuilder();
                if (CreateMenu(SBHtml, out ErrMsg))
                    this.lt_Menu.Text = SBHtml.ToString();
                else
                    this.lt_Menu.Text = "無任何選單...";
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
                return;
            }
        }

    }

    /// <summary>
    /// [建立選單] - 第一層
    /// </summary>
    /// <param name="SBHtml">選單Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool CreateMenu(StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除Html
                SBHtml.Clear();
                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();
                //[SQL] - 執行SQL
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("	    SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.CssStyle ");
                //[SQL] - 判斷&顯示(目前語系)
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine("		FROM Program INNER JOIN User_Profile_Rel_Program UserRel ON Program.Prog_ID = UserRel.Prog_ID ");
                SBSql.AppendLine("		WHERE (Program.Up_Id = 0) AND (Program.Display = 'Y') AND (UserRel.Guid = @UserGUID) ");
                SBSql.AppendLine("      ORDER BY Program.Sort, Program.Prog_ID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("UserGUID", fn_Param.CurrentUser);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        //組合Html
                        SBHtml.AppendLine(string.Format("<li id=\"li_up_{0}\" class=\"{1}\" style=\"cursor: pointer;\" "
                                   , DT.Rows[i]["Sort"].ToString()
                                   , DT.Rows[i]["CssStyle"].ToString()));
                        //判斷是否有Url
                        if (!string.IsNullOrEmpty(DT.Rows[i]["Prog_Link"].ToString()))
                        {
                            SBHtml.Append(string.Format(" onclick=\"fmenu('{0}', 'Y', '{1}');SubClick('');parent.mainFrame.location.href = '{2}';\""
                              , DT.Rows[i]["Sort"].ToString()
                              , DT.Rows[i]["CssStyle"].ToString()
                              , DT.Rows[i]["Prog_Link"].ToString()));
                        }
                        else
                        {
                            SBHtml.Append(string.Format(" onclick=\"fmenu('{0}', '', '{1}');\""
                                    , DT.Rows[i]["Sort"].ToString()
                                    , DT.Rows[i]["CssStyle"].ToString()));
                        }
                        SBHtml.Append(string.Format("><a>{0}</a></li>"
                            , DT.Rows[i]["Prog_Name"].ToString()));

                        //判斷是否有下層資料並回傳
                        CreateSubMenu(
                             DT.Rows[i]["Prog_ID"].ToString()
                             , DT.Rows[i]["Sort"].ToString()
                             , DT.Rows[i]["CssStyle"].ToString()
                             , SBHtml
                             , 2
                             , out ErrMsg);
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
    /// [建立選單] = 第二層
    /// </summary>
    /// <param name="Up_ID">上層編號</param>
    /// <param name="Sort">排序(定義js元素編號使用)</param>
    /// <param name="CssStyle">Css樣式</param>
    /// <param name="SBHtml">選單Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns></returns>
    private bool CreateSubMenu(string Up_ID, string Sort, string CssStyle, StringBuilder SBHtml, int lv, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除cmd參數
                cmd.Parameters.Clear();
                //[SQL] - 執行SQL
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("	    SELECT Program.Prog_ID, Program.Prog_Link, Program.Sort, Program.CssStyle ");
                //[SQL] - 判斷&顯示(目前語系)
                SBSql.AppendLine(string.Format(", Program.Prog_Name_{0} AS Prog_Name ", fn_Language.Param_Lang));
                SBSql.AppendLine("		FROM Program INNER JOIN User_Profile_Rel_Program UserRel ON Program.Prog_ID = UserRel.Prog_ID ");
                SBSql.AppendLine("	    WHERE (Program.Display = 'Y') AND (Program.LeftMenu_Display = 'Y') AND (Program.Lv = @Lv) ");
                SBSql.AppendLine("        AND (Program.Up_Id = @Param_UpID) AND (UserRel.Guid = @UserGUID)");
                SBSql.AppendLine("      ORDER BY Program.Sort, Program.Prog_ID ");
             
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("UserGUID", fn_Param.CurrentUser);
                cmd.Parameters.AddWithValue("Param_UpID", Up_ID);
                cmd.Parameters.AddWithValue("Lv", lv);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        if (lv == 2)
                        {
                            SBHtml.AppendLine(string.Format("<li class=\"MenuFirst2\" id=\"li_SubMenu_{0}\" style=\"display: none\">", Sort));
                        }
                        else
                        {
                            SBHtml.AppendLine("<li class=\"MenuFirst3\">");
                        }
                        
                        SBHtml.AppendLine(" <ul>");
                        for (int i = 0; i <= DT.Rows.Count - 1; i++)
                        {
                            string ProgID = DT.Rows[i]["Prog_ID"].ToString();


                            //Html
                            SBHtml.Append("<li id=\"li_{0}\">".FormatThis(ProgID));

                            SBHtml.AppendLine(string.Format("<a href=\"{1}\" onclick=\"fmenu('{2}', 'Y', '{4}');SubClick('{0}');\">{3}</a>"
                                        , DT.Rows[i]["Prog_ID"].ToString()
                                        , DT.Rows[i]["Prog_Link"].ToString()
                                        , Sort
                                        , DT.Rows[i]["Prog_Name"].ToString()
                                        , CssStyle));

                            SBHtml.Append("</li>");
                           
                        }
                        SBHtml.AppendLine(" </ul>");
                        SBHtml.AppendLine("</li>");
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
}