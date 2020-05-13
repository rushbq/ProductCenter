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
using System.IO;

public partial class Spec_BOM_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - BOM規格設定
                if (fn_CheckAuth.CheckAuth_User("105", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無使用權限！", "script:parent.$.fancybox.close()");
                    return;
                }
                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "ProdSpec/Spec_BOM_Search.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_Edit.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "GPAdd",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                this.tb_SpecID.Attributes.Add("readonly", "true");

                //[帶出選單] - 輸入方式
                if (fn_Extensions.ProdSpecTypeMenu(this.ddl_SpecType, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("輸入方式選單產生失敗！", "");
                }

                //[帶出選單] - 選單單頭代號
                Get_GroupIDMenu();

                //[取得資料] - 符號表
                if (fn_Extensions.SpecIconMenu(this.rbl_Icon, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("符號表產生失敗！", "script:parent.$.fancybox.close()");
                    return;
                }
                this.rbl_Icon.Items.Insert(0, new ListItem("無", ""));
                this.rbl_Icon.SelectedIndex = 0;

                //[參數判斷] - 載入資料
                Load_Data(Cryptograph.Decrypt(Request.QueryString["BOM_SpecID"]));

                //[取得/檢查參數] - 選單單頭代號(放在載入資料之後)
                if (Request.QueryString["OptionGID"] != null)
                {
                    if (this.ddl_OptionGID.SelectedIndex == 0)
                    {
                        this.ddl_OptionGID.SelectedIndex = this.ddl_OptionGID.Items.IndexOf(
                                this.ddl_OptionGID.Items.FindByValue(Request.QueryString["OptionGID"].ToString().Trim())
                                );
                    }
                }
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "script:parent.$.fancybox.close()");
            }

        }
    }

    #region -- 資料讀取 --
    /// <summary>
    /// 產生選單單頭選單
    /// </summary>
    private void Get_GroupIDMenu()
    {
        try
        {
            //[初始化]
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT OptionGID, OptionGName ");
                SBSql.AppendLine(" FROM Prod_BOMSpec_OptionGroup ");
                SBSql.AppendLine(" WHERE (Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort, OptionGID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_OptionGID.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_OptionGID.Items.Add(new ListItem("-- 尚無單頭資料 --", ""));
                    }
                    else
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            this.ddl_OptionGID.Items.Add(
                                new ListItem(DT.Rows[row]["OptionGID"].ToString() + " - " + DT.Rows[row]["OptionGName"].ToString()
                                , DT.Rows[row]["OptionGID"].ToString()));
                        }
                        this.ddl_OptionGID.Items.Insert(0, new ListItem("-- 選擇選單單頭 --", ""));
                        this.ddl_OptionGID.SelectedIndex = 0;
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生選單單頭選單！", "");
        }
    }

    /// <summary>
    /// 載入資料
    /// </summary>
    /// <param name="inputID">資料編號</param>
    private void Load_Data(string inputID)
    {
        try
        {
            //[初始化]
            string ErrMsg;
            //[取得參數] - BOM_SpecID(系統編號)
            Param_BOM_SpecID = string.IsNullOrEmpty(inputID) ? "" : inputID;
            if (string.IsNullOrEmpty(Param_BOM_SpecID))
            {
                return;
            }

            //[檢查參數] - BOM_SpecID(系統編號)
            if (false == fn_Extensions.String_輸入限制(Param_BOM_SpecID, fn_Extensions.InputType.大寫英文開頭混數字, "7", "7", out ErrMsg))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  Prod_BOMSpec.* ");
                SBSql.AppendLine("  , Rel.Pic_ID ");
                SBSql.AppendLine("  , (SELECT SpecName_zh_TW FROM Prod_Spec WHERE (SpecID = Prod_BOMSpec.SpecID)) AS SpecName");
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Prod_BOMSpec_List WHERE (Prod_BOMSpec_List.BOM_SpecID = Prod_BOMSpec.BOM_SpecID)) AS UseCnt ");
                SBSql.AppendLine(" FROM Prod_BOMSpec LEFT JOIN Icon_Rel_BOMSpec Rel ON Prod_BOMSpec.BOM_SpecID = Rel.BOM_SpecID ");
                SBSql.AppendLine(" WHERE (Prod_BOMSpec.BOM_SpecID = @Param_BOM_SpecID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_BOM_SpecID", Param_BOM_SpecID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", "script:parent.$.fancybox.close()");
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.lt_BOM_SpecID.Text = DT.Rows[0]["BOM_SpecID"].ToString();
                        this.tb_SpecID.Text = DT.Rows[0]["SpecID"].ToString();
                        this.tb_SpecName.Text = DT.Rows[0]["SpecName"].ToString();
                        this.tb_SpecName_zh_TW.Text = DT.Rows[0]["SpecName_zh_TW"].ToString();
                        this.tb_SpecName_en_US.Text = DT.Rows[0]["SpecName_en_US"].ToString();
                        this.tb_SpecName_zh_CN.Text = DT.Rows[0]["SpecName_zh_CN"].ToString();
                        this.ddl_SpecType.SelectedIndex = this.ddl_SpecType.Items.IndexOf(
                            this.ddl_SpecType.Items.FindByValue(DT.Rows[0]["SpecType"].ToString())
                            );
                        this.rbl_IsRequired.SelectedIndex = this.rbl_IsRequired.Items.IndexOf(
                            this.rbl_IsRequired.Items.FindByValue(DT.Rows[0]["IsRequired"].ToString())
                            );
                        this.ddl_OptionGID.SelectedIndex = this.ddl_OptionGID.Items.IndexOf(
                            this.ddl_OptionGID.Items.FindByValue(DT.Rows[0]["OptionGID"].ToString())
                            );
                        this.tb_SpecDESC.Text = DT.Rows[0]["SpecDESC"].ToString();
                        //符號
                        this.rbl_Icon.SelectedIndex = this.rbl_Icon.Items.IndexOf(
                            this.rbl_Icon.Items.FindByValue(DT.Rows[0]["Pic_ID"].ToString())
                            );
                        this.rbl_Display.SelectedIndex = this.rbl_Display.Items.IndexOf(
                            this.rbl_Display.Items.FindByValue(DT.Rows[0]["Display"].ToString())
                            );
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();
                        //判斷是否已使用
                        if (Convert.ToInt32(DT.Rows[0]["UseCnt"]) > 0)
                        {
                            this.lb_SpecType_Warning.Text = "(** 此規格已被使用，若變更輸入方式，將有可能造成資料不正確 **)";
                        }

                        //[按鈕設定]
                        this.btn_Edit.Text = "修改";
                        this.btn_Del.Enabled = true;
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 載入資料！", "");
        }
    }

    #endregion

    #region -- 資料編輯 --
    protected void btn_Edit_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填規格欄位
            if (false == fn_Extensions.String_字數(this.tb_SpecName_zh_TW.Text, "1", "50", out ErrMsg))
            { SBAlert.Append("「規格欄位名稱 - 繁中」請輸入1 ~ 50個字\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_SpecName_en_US.Text, "1", "100", out ErrMsg))
            { SBAlert.Append("「規格欄位名稱 - 英文」請輸入1 ~ 100個字\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_SpecName_zh_CN.Text, "1", "50", out ErrMsg))
            { SBAlert.Append("「規格欄位名稱 - 簡中」請輸入1 ~ 50個字\\n"); }
            if (false == fn_Extensions.Num_正整數(this.tb_Sort.Text, "1", "999", out ErrMsg))
            { SBAlert.Append("「排序」請輸入1 ~ 999的數字\\n"); }

            //[參數檢查] - 選填規格欄位
            if (false == fn_Extensions.String_字數(this.tb_SpecDESC.Text, "0", "50", out ErrMsg))
            { SBAlert.Append("「規格欄位名稱」請輸入1 ~ 50個字\\n"); }


            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "資料儲存"
            switch (this.btn_Edit.Text)
            {
                case "新增":
                    Add_Data(out ErrMsg);
                    break;

                case "修改":
                    Edit_Data(out ErrMsg);
                    break;

                default:
                    fn_Extensions.JsAlert("操作錯誤", "script:parent.$.fancybox.close();");
                    break;
            }
            #endregion
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 編輯", "");
        }
    }

    protected void btn_Del_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 檢查是否已被關聯
                SBSql.AppendLine(" DECLARE @RowNum1 AS INT ");
                SBSql.AppendLine(" SET @RowNum1 = (SELECT COUNT(*) FROM Prod_BOMSpec_List WHERE (BOM_SpecID = @Param_BOM_SpecID)) ");
                SBSql.AppendLine(" IF(@RowNum1) > 0 ");
                SBSql.AppendLine("  SELECT 'Y' AS IsRel ");
                SBSql.AppendLine(" ELSE ");
                SBSql.AppendLine("  SELECT 'N' AS IsRel ");
                cmd.Parameters.AddWithValue("Param_BOM_SpecID", Param_BOM_SpecID);
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows[0]["IsRel"].Equals("Y"))
                    {
                        fn_Extensions.JsAlert("無法刪除，此規格欄位已被使用！", "script:parent.$.fancybox.close()");
                        return;
                    }
                }

                //[SQL] - 刪除資料
                SBSql.Clear();
                SBSql.AppendLine(" DELETE FROM Icon_Rel_BOMSpec WHERE (BOM_SpecID = @Param_BOM_SpecID) ");
                SBSql.AppendLine(" DELETE FROM Prod_BOMSpec WHERE (BOM_SpecID = @Param_BOM_SpecID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_BOM_SpecID", Param_BOM_SpecID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料刪除失敗！", "");
                }
                else
                {
                    fn_Extensions.JsAlert("資料刪除成功！", "script:parent.location.reload();");
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除", "");
        }
    }


    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data(out string ErrMsg)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 取得編號
            SBSql.AppendLine(" --取得編號(第3位取5碼) ");
            SBSql.AppendLine(" DECLARE @Max_ID AS INT ");
            SBSql.AppendLine(" SET @Max_ID = (SELECT ISNULL(MAX(SUBSTRING(BOM_SpecID,3,5)), 0) + 1 FROM Prod_BOMSpec) ");
            SBSql.AppendLine(" --設定編號(7碼:BS12345) ");
            SBSql.AppendLine(" DECLARE @New_ID AS NCHAR(7) ");
            SBSql.AppendLine(" SET @New_ID = 'BS' + RIGHT(CAST('0000' + CAST(@Max_ID AS NVARCHAR) AS NVARCHAR), 5) ");
            //[SQL] - Insert
            SBSql.AppendLine(" INSERT INTO Prod_BOMSpec( ");
            SBSql.AppendLine("   BOM_SpecID, SpecID, SpecName_zh_TW, SpecName_en_US, SpecName_zh_CN");
            SBSql.AppendLine("   , SpecType, IsRequired, OptionGID, SpecDESC, Display, Sort");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("      @New_ID, @SpecID, @SpecName_zh_TW, @SpecName_en_US, @SpecName_zh_CN ");
            SBSql.AppendLine("      , @Param_SpecType, @Param_IsRequired");
            SBSql.AppendLine("      , @Param_OptionGID, @Param_SpecDESC, @Param_Display, @Param_Sort");
            SBSql.AppendLine(" ); ");

            //[SQL] - 判斷是否有選擇符號
            if (this.rbl_Icon.SelectedIndex > 0)
            {
                SBSql.AppendLine(" INSERT INTO Icon_Rel_BOMSpec (Pic_ID, BOM_SpecID) ");
                SBSql.AppendLine(" VALUES (@Pic_ID, @New_ID) ");

                cmd.Parameters.AddWithValue("Pic_ID", this.rbl_Icon.SelectedValue);
            }
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("SpecID", this.tb_SpecID.Text);
            cmd.Parameters.AddWithValue("SpecName_zh_TW", this.tb_SpecName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("SpecName_en_US", this.tb_SpecName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("SpecName_zh_CN", this.tb_SpecName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Param_SpecType", this.ddl_SpecType.SelectedValue);
            cmd.Parameters.AddWithValue("Param_IsRequired", this.rbl_IsRequired.SelectedValue);
            cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
            cmd.Parameters.AddWithValue("Param_SpecDESC", this.tb_SpecDESC.Text.Trim());
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！", "script:parent.location.reload();");
            }
        }
    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data(out string ErrMsg)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Prod_BOMSpec ");
            SBSql.AppendLine(" SET SpecID = @SpecID, SpecName_zh_TW = @SpecName_zh_TW, SpecName_en_US = @SpecName_en_US, SpecName_zh_CN = @SpecName_zh_CN ");
            SBSql.AppendLine("      , SpecType = @Param_SpecType, IsRequired = @Param_IsRequired, OptionGID = @Param_OptionGID, SpecDESC = @Param_SpecDESC ");
            SBSql.AppendLine("      , Display = @Param_Display, Sort = @Param_Sort ");
            SBSql.AppendLine(" WHERE (BOM_SpecID = @Param_BOM_SpecID)");
            //[SQL] - 判斷是否有選擇符號
            SBSql.AppendLine(" DELETE FROM Icon_Rel_BOMSpec WHERE (BOM_SpecID = @Param_BOM_SpecID) ");
            if (this.rbl_Icon.SelectedIndex > 0)
            {
                SBSql.AppendLine(" INSERT INTO Icon_Rel_BOMSpec (Pic_ID, BOM_SpecID) ");
                SBSql.AppendLine(" VALUES (@Pic_ID, @Param_BOM_SpecID) ");

                cmd.Parameters.AddWithValue("Pic_ID", this.rbl_Icon.SelectedValue);
            }
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_BOM_SpecID", Param_BOM_SpecID);
            cmd.Parameters.AddWithValue("SpecID", this.tb_SpecID.Text);
            cmd.Parameters.AddWithValue("SpecName_zh_TW", this.tb_SpecName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("SpecName_en_US", this.tb_SpecName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("SpecName_zh_CN", this.tb_SpecName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Param_SpecType", this.ddl_SpecType.SelectedValue);
            cmd.Parameters.AddWithValue("Param_IsRequired", this.rbl_IsRequired.SelectedValue);
            cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
            cmd.Parameters.AddWithValue("Param_SpecDESC", this.tb_SpecDESC.Text.Trim());
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", "script:parent.location.reload();");
            }
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 參數 - 功能來源
    /// </summary>
    private string _Param_Func;
    public string Param_Func
    {
        get
        {
            return Request.QueryString["func"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["func"].ToString());
        }
        set
        {
            this._Param_Func = value;
        }
    }

    //[參數] - 編號
    private string _Param_BOM_SpecID;
    public string Param_BOM_SpecID
    {
        get
        {
            return this._Param_BOM_SpecID != null ? this._Param_BOM_SpecID : this.lt_BOM_SpecID.Text;
        }
        set
        {
            this._Param_BOM_SpecID = value;
        }
    }
    #endregion

}