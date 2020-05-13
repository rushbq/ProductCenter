using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using ExtensionMethods;
using Resources;

public partial class Prod_InfoEdit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - 產品資訊
                if (fn_CheckAuth.CheckAuth_User(GetInfoValue("Auth", Param_InfoLang), out ErrMsg) == false)
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

                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;

                //[帶出資料]
                LookupData();

                ////帶出規格符號
                //Lookup_Icons();

                ////帶出認證符號
                //Lookup_CertIcons();
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
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
                SBSql.AppendLine(" SELECT * ");
                SBSql.AppendLine(" FROM Prod_Info ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) AND (Lang = @Param_InfoLang) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_InfoLang", Param_InfoLang);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        this.tb_Info1.Text = string.IsNullOrEmpty(DT.Rows[0]["Info1"].ToString()) ? string.Empty : DT.Rows[0]["Info1"].ToString();
                        this.tb_Info2.Text = string.IsNullOrEmpty(DT.Rows[0]["Info2"].ToString()) ? string.Empty : DT.Rows[0]["Info2"].ToString();
                        this.tb_Info3.Text = string.IsNullOrEmpty(DT.Rows[0]["Info3"].ToString()) ? string.Empty : DT.Rows[0]["Info3"].ToString();
                        this.tb_Info4.Text = string.IsNullOrEmpty(DT.Rows[0]["Info4"].ToString()) ? string.Empty : DT.Rows[0]["Info4"].ToString();
                        this.tb_Info5.Text = string.IsNullOrEmpty(DT.Rows[0]["Info5"].ToString()) ? string.Empty : DT.Rows[0]["Info5"].ToString();

                        this.tb_Info6.Text = string.IsNullOrEmpty(DT.Rows[0]["Info6"].ToString()) ? string.Empty : DT.Rows[0]["Info6"].ToString();
                        this.tb_Info7.Text = string.IsNullOrEmpty(DT.Rows[0]["Info7"].ToString()) ? string.Empty : DT.Rows[0]["Info7"].ToString();
                        this.tb_Info8.Text = string.IsNullOrEmpty(DT.Rows[0]["Info8"].ToString()) ? string.Empty : DT.Rows[0]["Info8"].ToString();

                        this.tb_Info9.Text = string.IsNullOrEmpty(DT.Rows[0]["Info9"].ToString()) ? string.Empty : DT.Rows[0]["Info9"].ToString();
                        
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    //#region >> 規格符號 <<

    ///// <summary>
    ///// [規格符號] - 帶出符號
    ///// </summary>
    //private void Lookup_Icons()
    //{
    //    try
    //    {
    //        using (SqlCommand cmd = new SqlCommand())
    //        {
    //            string ErrMsg;

    //            //[SQL] - 清除參數設定
    //            cmd.Parameters.Clear();
    //            //[SQL] - 資料查詢
    //            StringBuilder SBSql = new StringBuilder();
    //            SBSql.AppendLine(" SELECT RelIcon.Pic_ID, Icon_Pics.Pic_File, IcoShow.Pic_ID AS IcoShowID ");
    //            SBSql.AppendLine("  FROM Prod_SpecClass_Rel_Spec RelSpec ");
    //            SBSql.AppendLine("      INNER JOIN Prod_Item Prod ON RelSpec.SpecClassID = Prod.SpecClassID ");
    //            SBSql.AppendLine("      INNER JOIN Icon_Rel_Spec RelIcon ON RelSpec.SpecID = RelIcon.SpecID ");
    //            SBSql.AppendLine("      INNER JOIN Icon_Pics ON Icon_Pics.Pic_ID = RelIcon.Pic_ID ");
    //            SBSql.AppendLine("      LEFT JOIN Icon_Rel_Spec_eService IcoShow ON RelIcon.Pic_ID = IcoShow.Pic_ID AND IcoShow.Model_No = @Model_No AND Lang = @Param_InfoLang ");
    //            SBSql.AppendLine(" WHERE (Prod.Model_No = @Model_No) ");
    //            SBSql.AppendLine(" GROUP BY RelIcon.Pic_ID, Icon_Pics.Pic_File, IcoShow.Pic_ID ");
    //            cmd.CommandText = SBSql.ToString();
    //            cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
    //            cmd.Parameters.AddWithValue("Param_InfoLang", Param_InfoLang);
    //            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
    //            {
    //                //DataBind            
    //                this.lvIconList.DataSource = DT.DefaultView;
    //                this.lvIconList.DataBind();
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        fn_Extensions.JsAlert("系統發生錯誤 - 規格符號！", "");
    //        return;
    //    }
    //}

    //protected void lvIconList_ItemDataBound(object sender, ListViewItemEventArgs e)
    //{
    //    if (e.Item.ItemType == ListViewItemType.DataItem)
    //    {
    //        ListViewDataItem dataItem = (ListViewDataItem)e.Item;

    //        //[判斷] - eService是否顯示
    //        //若Icon_Rel_Spec_eService 有資料，代表顯示
    //        RadioButtonList rbl_Display = (RadioButtonList)e.Item.FindControl("rbl_Display");
    //        string IsShow = DataBinder.Eval(dataItem.DataItem, "IcoShowID").ToString();
    //        if (!string.IsNullOrEmpty(IsShow))
    //        {
    //            rbl_Display.SelectedIndex = rbl_Display.Items.IndexOf(rbl_Display.Items.FindByValue("Y"));
    //        }
    //        else
    //        {
    //            rbl_Display.SelectedIndex = rbl_Display.Items.IndexOf(rbl_Display.Items.FindByValue("N"));
    //        }
    //    }
    //}

    ///// <summary>
    ///// [規格符號] - 取得圖片連結
    ///// </summary>
    ///// <param name="PicName">真實檔名</param>
    ///// <returns>string</returns>
    //public string SpecPicUrl(string PicName)
    //{
    //    string preView = "";

    //    //判斷是否為圖片
    //    string strFileExt = ".jpg||.png||.gif";
    //    if (fn_Extensions.CheckStrWord(PicName, strFileExt, "|", 2))
    //    {
    //        //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
    //        preView = string.Format(
    //            "<div class=\"L2Img\" style=\"text-align:center\"> " +
    //            "<img src=\"{0}\" width=\"80px\" href=\"{0}\">" +
    //            "</div>"
    //            , Param_IconWebFolder + PicName
    //            );
    //    }

    //    //輸出Html
    //    return preView;
    //}

    //#endregion

    //#region >> 認證符號 <<

    ///// <summary>
    ///// [認證符號] - 帶出符號
    ///// </summary>
    //private void Lookup_CertIcons()
    //{
    //    try
    //    {
    //        using (SqlCommand cmd = new SqlCommand())
    //        {
    //            string ErrMsg;

    //            //[SQL] - 清除參數設定
    //            cmd.Parameters.Clear();
    //            //[SQL] - 資料查詢
    //            StringBuilder SBSql = new StringBuilder();
    //            SBSql.AppendLine(" SELECT RelIcon.Pic_ID, Icon_Pics.Pic_File, IcoShow.Pic_ID AS IcoShowID ");
    //            SBSql.AppendLine("  FROM Prod_Certification Certi ");
    //            SBSql.AppendLine("      INNER JOIN Prod_Certification_Detail CertDtl ON Certi.Cert_ID = CertDtl.Cert_ID ");
    //            SBSql.AppendLine("      INNER JOIN Icon_Rel_Certification RelIcon ON RelIcon.Cert_ID = Certi.Cert_ID AND RelIcon.Detail_ID = CertDtl.Detail_ID ");
    //            SBSql.AppendLine("      INNER JOIN Icon_Pics ON Icon_Pics.Pic_ID = RelIcon.Pic_ID ");
    //            SBSql.AppendLine("      LEFT JOIN Icon_Rel_Certification_eService IcoShow ");
    //            SBSql.AppendLine("       ON RelIcon.Pic_ID = IcoShow.Pic_ID AND IcoShow.Model_No = @Model_No AND Lang = @Param_InfoLang ");
    //            SBSql.AppendLine(" WHERE (Certi.Model_No = @Model_No) ");
    //            SBSql.AppendLine(" GROUP BY RelIcon.Pic_ID, Icon_Pics.Pic_File, IcoShow.Pic_ID ");
    //            cmd.CommandText = SBSql.ToString();
    //            cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
    //            cmd.Parameters.AddWithValue("Param_InfoLang", Param_InfoLang);
    //            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
    //            {
    //                //DataBind            
    //                this.lvCertIconList.DataSource = DT.DefaultView;
    //                this.lvCertIconList.DataBind();
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        fn_Extensions.JsAlert("系統發生錯誤 - 規格符號！", "");
    //        return;
    //    }
    //}

    //protected void lvCertIconList_ItemDataBound(object sender, ListViewItemEventArgs e)
    //{
    //    if (e.Item.ItemType == ListViewItemType.DataItem)
    //    {
    //        ListViewDataItem dataItem = (ListViewDataItem)e.Item;

    //        //[判斷] - eService是否顯示
    //        //若Icon_Rel_Spec_eService 有資料，代表顯示
    //        RadioButtonList rbl_Display = (RadioButtonList)e.Item.FindControl("rbl_Display");
    //        string IsShow = DataBinder.Eval(dataItem.DataItem, "IcoShowID").ToString();
    //        if (!string.IsNullOrEmpty(IsShow))
    //        {
    //            rbl_Display.SelectedIndex = rbl_Display.Items.IndexOf(rbl_Display.Items.FindByValue("Y"));
    //        }
    //        else
    //        {
    //            rbl_Display.SelectedIndex = rbl_Display.Items.IndexOf(rbl_Display.Items.FindByValue("N"));
    //        }
    //    }
    //}

    ///// <summary>
    ///// [認證符號] - 取得圖片連結
    ///// </summary>
    ///// <param name="PicName">真實檔名</param>
    ///// <returns>string</returns>
    //public string CertPicUrl(string PicName)
    //{
    //    string preView = "";

    //    //判斷是否為圖片
    //    string strFileExt = ".jpg||.png||.gif";
    //    if (fn_Extensions.CheckStrWord(PicName, strFileExt, "|", 2))
    //    {
    //        //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
    //        preView = string.Format(
    //            "<div class=\"L2Img\" style=\"text-align:center\"> " +
    //            "<img src=\"{0}\" width=\"50px\" href=\"{0}\">" +
    //            "</div>"
    //            , Param_IconWebFolder + PicName
    //            );
    //    }

    //    //輸出Html
    //    return preView;
    //}

    //#endregion

    #endregion

    #region -- 按鈕區 --
    /// <summary>
    /// 儲存鈕
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料更新
                SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Prod_Info WHERE (Model_No = @Param_ModelNo) AND (Lang = @Param_InfoLang)) = 0 ");
                SBSql.AppendLine("  BEGIN");
                SBSql.AppendLine("   INSERT INTO Prod_Info(");
                SBSql.AppendLine("      Model_No, Lang, Info1, Info2, Info3, Info4, Info5, Create_Who, Create_Time");
                SBSql.AppendLine("      , Info6, Info7, Info8, Info9");
                SBSql.AppendLine("   )VALUES(");
                SBSql.AppendLine("      @Param_ModelNo, @Param_InfoLang, @Info1, @Info2, @Info3, @Info4, @Info5, @Create_Who, GETDATE()");
                SBSql.AppendLine("      , @Info6, @Info7, @Info8, @Info9");
                SBSql.AppendLine("   )");
                SBSql.AppendLine("  END");
                SBSql.AppendLine(" ELSE");
                SBSql.AppendLine("  BEGIN");
                SBSql.AppendLine("   UPDATE Prod_Info SET ");
                SBSql.AppendLine("      Info1 = @Info1, Info2 = @Info2, Info3 = @Info3, Info4 = @Info4, Info5 = @Info5");
                SBSql.AppendLine("      , Info6 = @Info6, Info7 = @Info7, Info8 = @Info8, Info9 = @Info9");
                SBSql.AppendLine("      , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                SBSql.AppendLine("   WHERE (Model_No = @Param_ModelNo) AND (Lang = @Param_InfoLang)");
                SBSql.AppendLine("  END");

                ////[SQL] - 資料更新, 規格符號
                //#region >> 規格符號 <<
                //if (this.lvIconList.Items.Count > 0)
                //{
                //    SBSql.AppendLine(" DELETE FROM Icon_Rel_Spec_eService WHERE (Model_No = @Param_ModelNo) AND (Lang = @Param_InfoLang); ");

                //    for (int row = 0; row < lvIconList.Items.Count; row++)
                //    {
                //        //[取得參數] - 編號
                //        string lvParam_ID = ((HiddenField)this.lvIconList.Items[row].FindControl("hf_PicID")).Value;
                //        //[取得參數] - 顯示
                //        string lvParam_Disp = ((RadioButtonList)this.lvIconList.Items[row].FindControl("rbl_Display")).SelectedValue;

                //        if (lvParam_Disp.Equals("Y"))
                //        {
                //            SBSql.Append(" INSERT INTO Icon_Rel_Spec_eService(Pic_ID, Model_No, Lang");
                //            SBSql.Append("  ) VALUES ( ");
                //            SBSql.Append(string.Format(
                //                " @lvParam_ID_{0}, @Param_ModelNo, @Param_InfoLang "
                //                , row));
                //            SBSql.AppendLine(" );");
                //            cmd.Parameters.AddWithValue("lvParam_ID_" + row, lvParam_ID);
                //        }
                //    }
                //}
                //#endregion

                ////[SQL] - 資料更新, 認證符號
                //#region >> 認證符號 <<
                //if (this.lvCertIconList.Items.Count > 0)
                //{
                //    SBSql.AppendLine(" DELETE FROM Icon_Rel_Certification_eService WHERE (Model_No = @Param_ModelNo) AND (Lang = @Param_InfoLang); ");

                //    for (int row = 0; row < lvCertIconList.Items.Count; row++)
                //    {
                //        //[取得參數] - 編號
                //        string lvParam_ID = ((HiddenField)this.lvCertIconList.Items[row].FindControl("hf_PicID")).Value;
                //        //[取得參數] - 顯示
                //        string lvParam_Disp = ((RadioButtonList)this.lvCertIconList.Items[row].FindControl("rbl_Display")).SelectedValue;

                //        if (lvParam_Disp.Equals("Y"))
                //        {
                //            SBSql.Append(" INSERT INTO Icon_Rel_Certification_eService(Pic_ID, Model_No, Lang");
                //            SBSql.Append("  ) VALUES ( ");
                //            SBSql.Append(string.Format(
                //                " @lvParam_CID_{0}, @Param_ModelNo, @Param_InfoLang "
                //                , row));
                //            SBSql.AppendLine(" );");
                //            cmd.Parameters.AddWithValue("lvParam_CID_" + row, lvParam_ID);
                //        }
                //    }
                //}
                //#endregion

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_InfoLang", Param_InfoLang);
                cmd.Parameters.AddWithValue("Create_Who", fn_Param.CurrentAccount.ToString());
                cmd.Parameters.AddWithValue("Update_Who", fn_Param.CurrentAccount.ToString());

                cmd.Parameters.AddWithValue("Info1", this.tb_Info1.Text);
                cmd.Parameters.AddWithValue("Info2", this.tb_Info2.Text);
                cmd.Parameters.AddWithValue("Info3", this.tb_Info3.Text);
                cmd.Parameters.AddWithValue("Info4", this.tb_Info4.Text);
                cmd.Parameters.AddWithValue("Info5", this.tb_Info5.Text);
                cmd.Parameters.AddWithValue("Info6", this.tb_Info6.Text);
                cmd.Parameters.AddWithValue("Info7", this.tb_Info7.Text);
                cmd.Parameters.AddWithValue("Info8", this.tb_Info8.Text);
                cmd.Parameters.AddWithValue("Info9", this.tb_Info9.Text);
                if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("儲存失敗！", "");
                    return;
                }
                else
                {
                    string Url = string.Format("Prod_InfoEdit.aspx?Model_No={0}&Lang={1}", Server.UrlEncode(Param_ModelNo), Param_InfoLang);
                    fn_Extensions.JsAlert("儲存成功！", "script:location.replace('" + Url + "');");
                    return;
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存！", "");
            return;
        }
    }


    /// <summary>
    /// 依語系判斷&回傳所屬ID
    /// </summary>
    /// <param name="CheckType">來源類別</param>
    /// <param name="CheckLang">語言別</param>
    /// <returns></returns>
    private string GetInfoValue(string CheckType, string CheckLang)
    {
        if (string.IsNullOrEmpty(CheckType) || string.IsNullOrEmpty(CheckLang))
        {
            return "";
        }
        //判斷語系
        string AuthID, TabIndex;
        switch (CheckLang.ToUpper())
        {
            case "ZH-CN":
                AuthID = "123";
                TabIndex = "4";
                break;

            case "ZH-TW":
                AuthID = "124";
                TabIndex = "5";
                break;

            case "EN-US":
                AuthID = "125";
                TabIndex = "6";
                break;

            default:
                AuthID = "";
                TabIndex = "";
                break;
        }
        //判斷來源
        switch (CheckType)
        {
            case "Auth":
                return AuthID;

            case "TabIndex":
                return TabIndex;

            default:
                return "";
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

    /// <summary>
    /// 產品資訊語言別
    /// </summary>
    private string _Param_InfoLang;
    public string Param_InfoLang
    {
        get
        {
            String Lang = Request.QueryString["Lang"];
            if (string.IsNullOrEmpty(Lang))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:history.back(-1);");
                return "";
            }
            else
            {
                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = GetInfoValue("TabIndex", Lang);

                return Lang;
            }
        }
        private set
        {
            this._Param_InfoLang = value;
        }
    }

    /// <summary>
    /// 經銷商前台網址
    /// </summary>
    public string _Param_DealerWebUrl;
    public string Param_DealerWebUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["Dealer_WebUrl"];
        }
        private set
        {
            this._Param_DealerWebUrl = value;
        }
    }

    /// <summary>
    /// [參數] - Web資料夾路徑 (符號)
    /// </summary>
    private string _Param_IconWebFolder;
    public string Param_IconWebFolder
    {
        get
        {
            return this._Param_IconWebFolder != null ? this._Param_IconWebFolder : Application["File_WebUrl"] + @"Icons/";
        }
        set
        {
            this._Param_IconWebFolder = value;
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
