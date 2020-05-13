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
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Linq;

public partial class PDFSet_byItem : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - PDF匯出設定
                if (fn_CheckAuth.CheckAuth_User("122", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Product/Prod_Search.aspx";

                //[取得/檢查參數] - Model_No (品號)
                String Model_No = Request.QueryString["Model_No"];
                this.lb_Model_No.Text = string.IsNullOrEmpty(Model_No) ? "" : fn_stringFormat.Filter_Html(Model_No.Trim());
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                    return;
                }

                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = "3";
                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;

                //[取得資料] - 基本資料
                LookupBaseData(Param_ModelNo);
                //[取得資料] - PDF匯出設定
                LookupData(Param_ModelNo, Param_SpecClass);

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
    /// 基本資料
    /// </summary>
    /// <param name="ModelNo">品號</param>
    void LookupBaseData(string ModelNo)
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[清除參數]
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT SpecClassID ");
                SBSql.AppendLine(" FROM Prod_Item ");
                SBSql.AppendLine(" WHERE (Model_No = @Model_No) AND (SpecClassID IS NOT NULL) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Model_No", ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料或規格類別未設定！", "script:history.back();");
                        return;
                    }
                    //[暫存規格類別]
                    this.lt_SpecClassID.Text = DT.Rows[0]["SpecClassID"].ToString();
                }
            }
        }
        catch (Exception)
        {

            throw new Exception("基本資料取得發生錯誤");
        }
    }

    /// <summary>
    /// PDF設定資料
    /// </summary>
    /// <remarks>
    /// 判斷 Prod_PDF_byItem 是否有資料，若無則預設帶 Prod_PDF_byClass 的設定
    /// </remarks>
    void LookupData(string ModelNo, string SpecClassID)
    {
        try
        {
            string ErrMsg;
            StringBuilder SBHtml = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Prod_PDF_byItem WHERE (Model_No = @Model_No)) = 0 ");
                SBSql.AppendLine("  BEGIN ");
                //[SQL] - Search on Prod_PDF_byClass ↓↓
                SBSql.AppendLine(" SELECT Cate.CateID, Cate.CateName_zh_TW AS CateName ");
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY Cate.CateID ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID ASC) AS Cate_Rank ");
                SBSql.AppendLine("     , Rel.SpecClassID ");
                SBSql.AppendLine("     , Spec.SpecID, Spec.SpecName_zh_TW ");
                SBSql.AppendLine("     , RelPDF.SpecID AS IsChecked ");
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_PDF_byClass PDF WHERE (PDF.SpecClassID = Rel.SpecClassID AND PDF.CateID = Rel.CateID)) AS SetCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Category Cate ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec_Rel_Category Rel ON Cate.CateID = Rel.CateID ");
                SBSql.AppendLine("     INNER JOIN Prod_SpecClass_Rel_Spec ClassRel ON ClassRel.SpecClassID = Rel.SpecClassID AND ClassRel.SpecID = Rel.SpecID ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec Spec ON Rel.SpecID = Spec.SpecID ");
                SBSql.AppendLine("     LEFT JOIN Prod_PDF_byClass RelPDF ON RelPDF.SpecClassID = Rel.SpecClassID AND RelPDF.SpecID = Rel.SpecID AND RelPDF.CateID = Rel.CateID ");
                SBSql.AppendLine(" WHERE (Cate.Display = 'Y') AND (Spec.Display = 'Y') AND (ClassRel.SpecClassID = @SpecClassID) ");
                SBSql.AppendLine(" ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID ");
                //[SQL] - Search on Prod_PDF_byClass ↑↑
                SBSql.AppendLine("  END ");
                SBSql.AppendLine(" ELSE ");
                SBSql.AppendLine("  BEGIN ");
                //[SQL] - Search on Prod_PDF_byItem ↓↓
                SBSql.AppendLine(" SELECT Cate.CateID, Cate.CateName_zh_TW AS CateName ");
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY Cate.CateID ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID ASC) AS Cate_Rank ");
                SBSql.AppendLine("     , Rel.SpecClassID ");
                SBSql.AppendLine("     , Spec.SpecID, Spec.SpecName_zh_TW ");
                SBSql.AppendLine("     , RelPDF.SpecID AS IsChecked ");
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_PDF_byItem PDF WHERE (PDF.SpecClassID = Rel.SpecClassID AND PDF.CateID = Rel.CateID AND PDF.Model_No = @Model_No)) AS SetCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Category Cate ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec_Rel_Category Rel ON Cate.CateID = Rel.CateID  ");
                SBSql.AppendLine("     INNER JOIN Prod_SpecClass_Rel_Spec ClassRel ON ClassRel.SpecClassID = Rel.SpecClassID AND ClassRel.SpecID = Rel.SpecID ");
                SBSql.AppendLine("     INNER JOIN Prod_Spec Spec ON Rel.SpecID = Spec.SpecID ");
                SBSql.AppendLine("     LEFT JOIN Prod_PDF_byItem RelPDF ON RelPDF.SpecClassID = Rel.SpecClassID AND RelPDF.SpecID = Rel.SpecID ");
                SBSql.AppendLine("      AND RelPDF.CateID = Rel.CateID AND (RelPDF.Model_No = @Model_No) ");
                SBSql.AppendLine(" WHERE (Cate.Display = 'Y') AND (Spec.Display = 'Y') AND (ClassRel.SpecClassID = @SpecClassID) ");
                SBSql.AppendLine(" ORDER BY Cate.Sort, Cate.CateID, Spec.Sort, Spec.SpecID ");
                //[SQL] - Search on Prod_PDF_byItem ↑↑
                SBSql.AppendLine("  END ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Model_No", ModelNo);
                cmd.Parameters.AddWithValue("SpecClassID", SpecClassID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        this.lt_TreeView.Text = "<div style=\"padding:5px 5px 15px 5px\"><span class=\"JQ-ui-icon ui-icon-alert\"></span>尚未設定規格分類，請前往<a href=\"../ProdSpec/SpecCategory_Search.aspx\">規格分類</a>設定關聯。</div>";
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
                        string SpecID = DT.Rows[row]["SpecID"].ToString();
                        string SpecName = DT.Rows[row]["SpecName_zh_TW"].ToString();
                        string IsChecked = DT.Rows[row]["IsChecked"].ToString();
                        int SetCnt = Convert.ToInt32(DT.Rows[row]["SetCnt"]);
                        #endregion

                        //[HTML] - 顯示 規格分類, 每類標頭 (Cate_Rank = 1)
                        if (Convert.ToInt16(Cate_Rank).Equals(1))
                        {
                            SBHtml.AppendLine(string.Format(
                                "<li>" +
                                "<span class=\"folder\"><a></a></span>&nbsp;" +
                                "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"\" {2}><strong class=\"Font14\">{1}</strong></label>"
                                , CateID + "00"
                                , CateName
                                , (SetCnt > 0) ? "checked" : ""));

                            //[HTML] - 子層的tag開頭
                            SBHtml.AppendLine(" <ul>");
                        }

                        //[HTML] - 規格內容
                        SBHtml.AppendLine(string.Format(
                                  "<li><span class=\"file\"><a></a></span>&nbsp;" +
                                  "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"{3}\" rel=\"cb_{4}\" {2}><font class=\"styleBlue\">{1}</font></label>"
                                  , CateID + "00_" + SpecID
                                  , SpecID + " - " + SpecName
                                  , string.IsNullOrEmpty(IsChecked) ? "" : "checked"
                                  , CateID + "|" + SpecID
                                  , CateID + "00"
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

    //設定
    protected void btn_GetRelID_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            if (string.IsNullOrEmpty(Param_SpecClass))
            {
                fn_Extensions.JsAlert("規格類別未設定！", "Prod_Edit.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo));
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
                SBSql.AppendLine(" DELETE FROM Prod_PDF_byItem WHERE (SpecClassID = @SpecClassID) AND (Model_No = @Model_No); ");
                if (false == string.IsNullOrEmpty(this.hf_RelID.Value))
                {
                    //[SQL] - 新增關聯
                    for (int row = 0; row < aryRelID.Length; row++)
                    {
                        //分析Value(|)
                        string[] aryValue = Regex.Split(aryRelID[row], @"\|{1}");
                        SBSql.AppendLine(string.Format(
                            " INSERT INTO Prod_PDF_byItem(Model_No, SpecClassID, CateID, SpecID) VALUES (@Model_No,  @SpecClassID, '{0}','{1}'); "
                            , aryValue[0]
                            , aryValue[1]));
                    }
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("SpecClassID", Param_SpecClass);
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("設定失敗！", "");
                    return;
                }
                else
                {
                    fn_Extensions.JsAlert("設定成功！"
                        , "PDFSet_byItem.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo) + "&SpecClass=" + Server.UrlEncode(Param_SpecClass));
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

    #region -- 參數設定 --
    /// <summary>
    /// 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return this._Param_ModelNo != null ? this._Param_ModelNo : this.lb_Model_No.Text.Trim().ToUpper();
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }

    private string _Param_SpecClass;
    public string Param_SpecClass
    {
        get
        {
            return this._Param_SpecClass != null ? this._Param_SpecClass : this.lt_SpecClassID.Text.Trim();
        }
        private set
        {
            this._Param_SpecClass = value;
        }
    }
    #endregion

    #region -- 語系參數 --
    /// <summary>
    /// [Navi] - 系統首頁
    /// </summary>
    private string _Navi_系統首頁;
    public string Navi_系統首頁
    {
        get
        {
            return Res_Navi.系統首頁;
        }
        private set
        {
            this._Navi_系統首頁 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料庫;
    public string Navi_產品資料庫
    {
        get
        {
            return Res_Navi.產品資料庫;
        }
        private set
        {
            this._Navi_產品資料庫 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料;
    public string Navi_產品資料
    {
        get
        {
            return Res_Navi.產品資料;
        }
        private set
        {
            this._Navi_產品資料 = value;
        }
    }

    #endregion

}
