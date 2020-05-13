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

public partial class SpecClass_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - 規格設定
                if (fn_CheckAuth.CheckAuth_User("102", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無使用權限！", "script:parent.$.fancybox.close()");
                    return;
                }
                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "ProdSpec/SpecClass_Search.aspx";
                
                //[按鈕] - 加入BlockUI
                this.btn_Edit.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "GPAdd",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[帶出選單] - 分類編號
                Get_ClassMenu();

                //[參數判斷] - 載入資料
                Load_Data(Cryptograph.Decrypt(Request.QueryString["SpecClassID"]));
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "script:parent.$.fancybox.close()");
            }

        }
    }

    #region ***按鈕區***
    protected void btn_Edit_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (false == fn_Extensions.String_字數(this.tb_ClassName_zh_TW.Text, "1", "50", out ErrMsg))
            { SBAlert.Append("「分類名稱 - 繁中」請輸入1 ~ 50個字\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_ClassName_en_US.Text, "1", "100", out ErrMsg))
            { SBAlert.Append("「分類名稱 - 英文」請輸入1 ~ 100個字\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_ClassName_zh_CN.Text, "1", "50", out ErrMsg))
            { SBAlert.Append("「分類名稱 - 簡中」請輸入1 ~ 50個字\\n"); }
            if (false == fn_Extensions.Num_正整數(this.tb_Sort.Text, "1", "999", out ErrMsg))
            { SBAlert.Append("「排序」請輸入1 ~ 999的數字\\n"); }

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

    #endregion

    #region ***Functions & Methods***
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
                SBSql.AppendLine(" SELECT SpecClassID, ClassName_zh_TW ");
                SBSql.AppendLine(" FROM Prod_Spec_Class ");
                SBSql.AppendLine(" WHERE (UpClass IS NULL) ");
                SBSql.AppendLine(" ORDER BY Sort, SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_UpClass.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_UpClass.Items.Add(new ListItem("-- 尚無分類資料 --", ""));
                    }
                    else
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            this.ddl_UpClass.Items.Add(
                                new ListItem(DT.Rows[row]["SpecClassID"].ToString() + " - " + DT.Rows[row]["ClassName_zh_TW"].ToString()
                                , DT.Rows[row]["SpecClassID"].ToString()));
                        }
                        this.ddl_UpClass.Items.Insert(0, new ListItem("-- 選擇大分類 --", ""));
                        this.ddl_UpClass.SelectedIndex = 0;
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
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
            //[取得參數] - SpecClassID(系統編號)
            Param_SpecClassID = string.IsNullOrEmpty(inputID) ? "" : inputID;
            if (string.IsNullOrEmpty(Param_SpecClassID))
            {
                return;
            }
            //[檢查參數] - SpecClassID(系統編號)
            if (false == fn_Extensions.String_輸入限制(Param_SpecClassID, fn_Extensions.InputType.大寫英文開頭混數字, "5", "5", out ErrMsg))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  SpecClassID, ClassName_zh_TW, ClassName_en_US, ClassName_zh_CN, UpClass, Display, Sort ");
                SBSql.AppendLine(" FROM Prod_Spec_Class ");
                SBSql.AppendLine(" WHERE (SpecClassID = @Param_SpecClassID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_SpecClassID", Param_SpecClassID);
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
                        this.lt_SpecClassID.Text = DT.Rows[0]["SpecClassID"].ToString();
                        this.tb_ClassName_zh_TW.Text = DT.Rows[0]["ClassName_zh_TW"].ToString();
                        this.tb_ClassName_en_US.Text = DT.Rows[0]["ClassName_en_US"].ToString();
                        this.tb_ClassName_zh_CN.Text = DT.Rows[0]["ClassName_zh_CN"].ToString();
                        this.ddl_UpClass.SelectedIndex = this.ddl_UpClass.Items.IndexOf(
                            this.ddl_UpClass.Items.FindByValue(DT.Rows[0]["UpClass"].ToString())
                            );
                        this.rbl_Display.SelectedIndex = this.rbl_Display.Items.IndexOf(
                            this.rbl_Display.Items.FindByValue(DT.Rows[0]["Display"].ToString())
                            );
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //[按鈕設定]
                        this.btn_Edit.Text = "修改";
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 載入資料！", "");
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
            SBSql.AppendLine(" --取得編號(第2位取4碼) ");
            SBSql.AppendLine(" DECLARE @Max_ID AS INT ");
            SBSql.AppendLine(" SET @Max_ID = (SELECT ISNULL(MAX(SUBSTRING(SpecClassID,2,4)), 0) + 1 FROM Prod_Spec_Class) ");
            SBSql.AppendLine(" --設定編號(5碼:C1234) ");
            SBSql.AppendLine(" DECLARE @New_ID AS NCHAR(5) ");
            SBSql.AppendLine(" SET @New_ID = 'C' + RIGHT(CAST('000' + CAST(@Max_ID AS NVARCHAR) AS NVARCHAR), 4) ");
            //[SQL] - Insert
            SBSql.AppendLine(" INSERT INTO Prod_Spec_Class( ");
            SBSql.AppendLine("      SpecClassID, ClassName_zh_TW, ClassName_en_US, ClassName_zh_CN, UpClass, Display, Sort");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("      @New_ID, @ClassName_zh_TW, @ClassName_en_US, @ClassName_zh_CN, @Param_UpClass, @Param_Display, @Param_Sort");
            SBSql.AppendLine(" )");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("ClassName_zh_TW", this.tb_ClassName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("ClassName_en_US", this.tb_ClassName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("ClassName_zh_CN", this.tb_ClassName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Param_UpClass", this.ddl_UpClass.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！", "script:parent.$.fancybox.close();" + PageUrl);
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
            SBSql.AppendLine(" UPDATE Prod_Spec_Class ");
            SBSql.AppendLine(" SET ClassName_zh_TW = @ClassName_zh_TW, ClassName_en_US = @ClassName_en_US, ClassName_zh_CN = @ClassName_zh_CN ");
            SBSql.AppendLine("     , UpClass = @Param_UpClass, Display = @Param_Display, Sort = @Param_Sort ");
            SBSql.AppendLine(" WHERE (SpecClassID = @Param_SpecClassID)");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_SpecClassID", Param_SpecClassID);
            cmd.Parameters.AddWithValue("ClassName_zh_TW", this.tb_ClassName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("ClassName_en_US", this.tb_ClassName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("ClassName_zh_CN", this.tb_ClassName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Param_UpClass", (string.IsNullOrEmpty(this.ddl_UpClass.SelectedValue))? DBNull.Value : (object)this.ddl_UpClass.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", "script:parent.$.fancybox.close();" + PageUrl);
            }
        }
    }
    #endregion

    #region "參數設定"
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            //判斷來源是否為規格總覽
            string funFrom = "";
            if (Request.QueryString["func"] != null)
            {
                funFrom = fn_stringFormat.Filter_Html(Request.QueryString["func"].ToString());
            }
            if (funFrom.Equals("map"))
            {
                return string.Format("parent.location.reload();");
            }
            else
            {
                return string.Format("parent.location.href='{0}';", Session["BackListUrl"].ToString());
            }
        }
        set
        {
            this._PageUrl = value;
        }
    }

    //[參數] - 編號
    private string _Param_SpecClassID;
    public string Param_SpecClassID
    {
        get
        {
            return this._Param_SpecClassID != null ? this._Param_SpecClassID : this.lt_SpecClassID.Text;
        }
        set
        {
            this._Param_SpecClassID = value;
        }
    }

    #endregion

}