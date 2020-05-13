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
using System.Text.RegularExpressions;

public partial class Spec_Rel_ProdSpec : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";

            //[權限判斷] - 規格設定
            if (fn_CheckAuth.CheckAuth_User("102", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[代入Ascx參數] - 快速選單
            Ascx_QuickMenu1.Param_CurrItem = "9";

            //[取得/檢查參數] - 規格欄位
            if (fn_Extensions.String_字數(Request.QueryString["SpecID"], "6", "6", out ErrMsg))
            {
                this.tb_SpecName.Text = fn_stringFormat.Filter_Html(Request.QueryString["SpecName"].ToString().Trim());
                this.tb_SpecID.Text = fn_stringFormat.Filter_Html(Request.QueryString["SpecID"].ToString().Trim());
                this.lb_SpecID.Text = fn_stringFormat.Filter_Html(Request.QueryString["SpecName"].ToString().Trim());

                //判斷規格欄位是否有選擇
                if (false == string.IsNullOrEmpty(this.tb_SpecID.Text))
                {
                    this.ph_Search.Visible = true;

                    //取得資料
                    LookupData(this.tb_SpecID.Text.Trim());
                }
            }
            else
            {
                this.ph_Search.Visible = false;
            }
        }
    }

    #region -- 按鈕區 --
    /// <summary>
    /// 按鈕 - 搜尋
    /// </summary>
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Spec_Rel_ProdSpec.aspx?func=rel");

            //[查詢條件] - SpecID
            if (string.IsNullOrEmpty(this.tb_SpecID.Text) == false)
            {
                SBUrl.Append("&SpecID=" + Server.UrlEncode(this.tb_SpecID.Text.Trim()));
                SBUrl.Append(string.IsNullOrEmpty(this.tb_SpecName.Text) ? "" : "&SpecName=" + Server.UrlEncode(this.tb_SpecName.Text.Trim()));
            }

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    /// <summary>
    /// 按鈕 - 儲存關聯
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            //[檢查參數] - 關聯品號
            if (string.IsNullOrEmpty(this.tb_Item_Val.Text))
            {
                fn_Extensions.JsAlert("至少要有一筆「關聯品號」！", "");
                return;
            }

            //[取得參數] - 關聯品號
            string[] strAry = Regex.Split(this.tb_Item_Val.Text, @"\|{4}");
            //篩選關聯品號，移除重複資料
            List<string> validItem = new List<string>();
            var query = from el in strAry
                        group el by el.ToString().Trim() into gp
                        select new
                        {
                            Val = gp.Key
                        };

            //儲存資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 刪除關聯
                SBSql.AppendLine(" DELETE FROM Prod_Item_Rel_Spec WHERE (SpecID = @SpecID); ");
                foreach (var item in query)
                {
                    //暫存品號
                    validItem.Add(item.Val);
                    //[SQL] - 新增關聯
                    SBSql.AppendLine(string.Format(
                        " INSERT INTO Prod_Item_Rel_Spec (Model_No, SpecID) VALUES ('{0}', @SpecID); "
                        , item.Val));
                }
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("SpecID", this.tb_SpecID.Text.Trim());
                if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("儲存設定失敗！", "");
                }
                else {
                    fn_Extensions.JsAlert("儲存設定成功！", "");
                }
            }

            //恢復關聯品號選項Html
            this.lt_Items.Text = GetItemList(true, validItem);

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存關聯！", "");
        }
    }
    #endregion

    #region -- 資料取得 --
    /// <summary>
    /// 讀取資料
    /// </summary>
    /// <param name="inputID">輸入編號</param>
    private void LookupData(string inputID)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Model_No, SpecID ");
                SBSql.AppendLine("  FROM Prod_Item_Rel_Spec Rel ");
                SBSql.AppendLine(" WHERE (SpecID = @SpecID) ");
                SBSql.AppendLine(" ORDER BY Model_No ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("SpecID", inputID);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    List<string> ListItem = new List<string>();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        ListItem.Add(DT.Rows[row]["Model_No"].ToString());
                    }

                    //恢復關聯品號選項Html
                    if (ListItem.Count > 0)
                    {
                        this.lt_Items.Text = GetItemList(true, ListItem);
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
        }
    }


    /// <summary>
    /// 恢復選項Html
    /// </summary>
    /// <param name="IsDel">是否能刪除</param>
    /// <param name="validItem">選項值的組合</param>
    /// <returns></returns>
    private string GetItemList(bool IsDel, List<string> validItem)
    {
        try
        {
            //[顯示Html]
            StringBuilder sbHtml = new StringBuilder();
            for (int i = 0; i < validItem.Count; i++)
            {
                sbHtml.AppendLine("<li id=\"li_" + i + "\">");
                sbHtml.AppendLine(" <table>");
                sbHtml.AppendLine("  <tr>");
                //欄位
                sbHtml.AppendLine("     <td class=\"FEItemIn\">");
                sbHtml.AppendLine("         <input type=\"text\" class=\"Item_Val\" maxlength=\"40\" style=\"width:200px\" value=\"" + validItem[i].ToString() + "\" readonly />");
                sbHtml.AppendLine("     </td>");
                //刪除鈕
                sbHtml.AppendLine("     <td class=\"FEItemControl\">");
                if (IsDel)
                    sbHtml.Append("<a href=\"javascript:Delete_Item('" + i + "');\">刪除</a>");
                sbHtml.AppendLine("     </td>");
                sbHtml.AppendLine("  </tr>");
                sbHtml.AppendLine(" </table>");
                sbHtml.AppendLine("</li>");
            }

            return sbHtml.ToString();

        }
        catch (Exception)
        {
            return "";
        }
    }
    #endregion
}
