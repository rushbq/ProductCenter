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

public partial class PDFSet_byClass : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg = "";

                //[權限判斷] - 規格設定
                if (fn_CheckAuth.CheckAuth_User("102", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //[帶出選單] - 規格分類
                Get_ClassMenu();

                //[取得/檢查參數] - 規格分類
                if (fn_Extensions.String_字數(Request.QueryString["SpecClass"], "5", "5", out ErrMsg))
                {
                    this.ddl_SpecClass.SelectedIndex = this.ddl_SpecClass.Items.IndexOf(
                               this.ddl_SpecClass.Items.FindByValue(fn_stringFormat.Filter_Html(Request.QueryString["SpecClass"].ToString().Trim()))
                               );
                    //判斷分類是否有選擇
                    if (this.ddl_SpecClass.SelectedIndex > 0)
                    {
                        //[帶出資料]
                        LookupData();
                    }
                }

                //[代入Ascx參數] - 快速選單
                Ascx_QuickMenu1.Param_CurrItem = "10";

                //[按鈕] - 加入BlockUI
                this.btn_GetRelID.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Save",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

            }

            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 產生分類選單
    /// </summary>
    private void Get_ClassMenu()
    {
        try
        {
            //[初始化]
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT lv1.SpecClassID AS MtClassID, lv1.ClassName_zh_TW AS MtClassName ");
                SBSql.AppendLine("  , Lv2.SpecClassID AS SubClassID, Lv2.ClassName_zh_TW AS SubClassName ");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY lv1.ClassName_zh_TW ORDER BY lv1.Sort, lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ASC) AS GP_Rank ");
                SBSql.AppendLine(" FROM Prod_Spec_Class AS Lv1 ");
                SBSql.AppendLine("  LEFT JOIN Prod_Spec_Class AS Lv2 ON Lv1.SpecClassID = Lv2.UpClass ");
                SBSql.AppendLine(" WHERE (Lv1.Display = 'Y') AND (Lv1.UpClass IS NULL) AND (Lv2.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Lv1.Sort, Lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_SpecClass.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_SpecClass.Items.Add(new ListItem("-- 尚無分類資料 --", ""));
                        this.ph_link.Visible = true;
                        return;
                    }
                    //輸出選項
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //判斷GP_Rank, 若為第一項，則輸出群組名稱
                        if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                        {
                            this.ddl_SpecClass.AddItemGroup(DT.Rows[row]["MtClassID"].ToString() + " - " + DT.Rows[row]["MtClassName"].ToString());
                        }
                        //子項目
                        this.ddl_SpecClass.Items.Add(
                            new ListItem(DT.Rows[row]["SubClassID"].ToString() + " - " + DT.Rows[row]["SubClassName"].ToString()
                            , DT.Rows[row]["SubClassID"].ToString()));
                    }
                    this.ddl_SpecClass.Items.Insert(0, new ListItem("-- 選擇分類 --", ""));
                    this.ddl_SpecClass.SelectedIndex = 0;
                    this.ph_link.Visible = false;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
        }
    }

    /// <summary>
    /// 資料取得
    /// </summary>
    void LookupData()
    {
        try
        {
            string ErrMsg;
            StringBuilder SBHtml = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Cate.CateID, Cate.CateName_zh_TW AS CateName ");
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY Cate.CateID ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID ASC) AS Cate_Rank ");
                SBSql.AppendLine("     , Rel.SpecClassID ");
                SBSql.AppendLine("     , Spec.SpecID, Spec.SpecName_zh_TW, Spec.SpecType ");
                SBSql.AppendLine("     , RelPDF.SpecID AS IsChecked ");
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_PDF_byClass PDF WHERE (PDF.SpecClassID = Rel.SpecClassID AND PDF.CateID = Rel.CateID)) AS SetCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Category Cate ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec_Rel_Category Rel ON Cate.CateID = Rel.CateID ");
                SBSql.AppendLine("     INNER JOIN Prod_SpecClass_Rel_Spec ClassRel ON ClassRel.SpecClassID = Rel.SpecClassID AND ClassRel.SpecID = Rel.SpecID ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec Spec ON Rel.SpecID = Spec.SpecID ");
                SBSql.AppendLine("     LEFT JOIN Prod_PDF_byClass RelPDF ON RelPDF.SpecClassID = Rel.SpecClassID AND RelPDF.SpecID = Rel.SpecID AND RelPDF.CateID = Rel.CateID ");
                SBSql.AppendLine(" WHERE (Cate.Display = 'Y') AND (Spec.Display = 'Y') AND (ClassRel.SpecClassID = @SpecClassID) ");
                SBSql.AppendLine(" ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("SpecClassID", this.ddl_SpecClass.SelectedValue);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.lt_TreeView.Text = "<div style=\"padding:5px 5px 15px 5px\"><span class=\"JQ-ui-icon ui-icon-alert\"></span>此分類尚未設定，請前往<a href=\"SpecCategory_Search.aspx\">規格分類</a>設定關聯。</div>";
                        return;
                    }

                    //[Html] - 根目錄
                    SBHtml.AppendLine("<ul id=\"TreeView\" class=\"filetree\">");
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //[取得欄位資料]
                        #region * 取得欄位資料 *
                        string CateID = DT.Rows[row]["CateID"].ToString();
                        string Cate_Rank = DT.Rows[row]["Cate_Rank"].ToString();
                        string CateName = DT.Rows[row]["CateName"].ToString().Trim();
                        string SpecClassID = DT.Rows[row]["SpecClassID"].ToString();
                        string SpecID = DT.Rows[row]["SpecID"].ToString();
                        string SpecName = DT.Rows[row]["SpecName_zh_TW"].ToString();
                        string IsChecked = DT.Rows[row]["IsChecked"].ToString();
                        int SetCnt = Convert.ToInt32(DT.Rows[row]["SetCnt"]);
                        string SpecType = fn_Desc.Prod.InputType(DT.Rows[row]["SpecType"].ToString());
						
                        #endregion

                        //[HTML] - 顯示 規格分類, 每類標頭 (Cate_Rank = 1)
                        if (Convert.ToInt16(Cate_Rank).Equals(1))
                        {
                            SBHtml.AppendLine(string.Format(
                                "<li>" +
                                "<span class=\"folder\"><a></a></span>&nbsp;" +
                                "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"\" {2}><strong class=\"Font14\">{1}</strong></label>"
                                , CateID
                                , CateName
                                , (SetCnt > 0) ? "checked" : ""));

                            //[HTML] - 子層的tag開頭
                            SBHtml.AppendLine(" <ul>");
                        }

                        //[HTML] - 規格內容
                        SBHtml.AppendLine(string.Format(
                                  "<li><span class=\"file\"><a></a></span>&nbsp;" +
                                  "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"{3}\" rel=\"cb_{4}\" {2}><font class=\"styleBlue\">{1}</font></label>"
                                  , CateID + "_" + SpecID
                                  , SpecID + " - " + SpecName + " (" + SpecType + ")"
                                  , string.IsNullOrEmpty(IsChecked) ? "" : "checked"
                                  , CateID + "|" + SpecClassID + "|" + SpecID
                                  , CateID
                                  ));
                        SBHtml.AppendLine(" </li>");

                        /* [HTML]
                         * 計算每類的資料數, (Cate_Rank = 總數)
                         * 顯示子層的tag結尾
                         */
                        var queryCnt =
                            from el in DT.AsEnumerable()
                            where el.Field<int>("CateID").Equals(Convert.ToInt32(CateID))
                            select el;
                        if (Convert.ToInt32(Cate_Rank).Equals(queryCnt.Count()))
                        {
                            SBHtml.AppendLine(" </ul>");
                            SBHtml.AppendLine("</li>");
                        }
                    }
                    SBHtml.AppendLine("</ul>");
                }
            }

            //輸出Html
            this.lt_TreeView.Text = SBHtml.ToString();
        }
        catch (Exception)
        {
            throw new Exception("資料取得發生錯誤");
        }
    }

    #endregion

    #region -- 按鈕區 --
    /// <summary>
    /// 按鈕 - 選擇分類
    /// </summary>
    protected void btn_SpecClass_Click(object sender, EventArgs e)
    {
        try
        {
            string Url = "PDFSet_byClass.aspx?func=Set&SpecClass=" + Server.UrlEncode(this.ddl_SpecClass.SelectedValue);

            Response.Redirect(Url, false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 選擇分類！", "");
        }
    }

    //設定
    protected void btn_GetRelID_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            if (this.ddl_SpecClass.SelectedIndex == 0)
            {
                fn_Extensions.JsAlert("請選擇分類！", "");
                return;
            }

            //取得關聯編號, 先分析Checkbox(,) , 再分析Value(|)
            string[] aryRelID = Regex.Split(this.hf_RelID.Value, @"\,{1}");
            if (aryRelID == null)
            {
                fn_Extensions.JsAlert("設定失敗！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                //[SQL] - 刪除原關聯
                SBSql.AppendLine(" DELETE FROM Prod_PDF_byClass WHERE (SpecClassID = @SpecClassID); ");
                if (false == string.IsNullOrEmpty(this.hf_RelID.Value))
                {
                    //[SQL] - 新增關聯
                    for (int row = 0; row < aryRelID.Length; row++)
                    {
                        //分析Value(|)
                        string[] aryValue = Regex.Split(aryRelID[row], @"\|{1}");
                        SBSql.AppendLine(string.Format(
                            " INSERT INTO Prod_PDF_byClass(CateID, SpecClassID, SpecID) VALUES ('{0}','{1}','{2}'); "
                            , aryValue[0]
                            , aryValue[1]
                            , aryValue[2]));
                    }
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("SpecClassID", this.ddl_SpecClass.SelectedValue);
                if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("設定失敗！", "");
                    return;
                }
                else
                {
                    fn_Extensions.JsAlert("設定成功！"
                        , "PDFSet_byClass.aspx?func=Set&SpecClass=" + Server.UrlEncode(this.ddl_SpecClass.SelectedValue));
                    return;
                }

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定關聯！", "");
            return;
        }
    }
    #endregion

}
