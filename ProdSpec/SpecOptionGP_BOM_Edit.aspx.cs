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

public partial class SpecOptionGP_BOM_Edit : SecurityIn
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

                //[按鈕] - 加入BlockUI
                this.btn_Edit.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "GPAdd",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //判斷來源
                if (Param_Func.Equals("set"))
                {
                    this.lt_Head.Text = "Step1. 設定選單單頭";
                }

                //[參數判斷] - 載入資料
                Load_Data(Cryptograph.Decrypt(Request.QueryString["OptionGID"]));
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
            if (false == fn_Extensions.String_字數(this.tb_OptionGName.Text, "0", "50", out ErrMsg))
            { SBAlert.Append("「類別名稱」請輸入1 ~ 50個字\\n"); }
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
    /// 載入資料
    /// </summary>
    /// <param name="inputID">資料編號</param>
    private void Load_Data(string inputID)
    {
        try
        {
            //[初始化]
            string ErrMsg;
            //[取得參數] - OptionGID(系統編號)
            Param_OptionGID = string.IsNullOrEmpty(inputID) ? "" : inputID;
            if (string.IsNullOrEmpty(Param_OptionGID))
            {
                return;
            }
            //[檢查參數] - OptionGID(系統編號)
            if (false == fn_Extensions.String_輸入限制(Param_OptionGID, fn_Extensions.InputType.大寫英文開頭混數字, "5", "5", out ErrMsg))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  OptionGID, OptionGName, Display, Sort ");
                SBSql.AppendLine(" FROM Prod_BOMSpec_OptionGroup ");
                SBSql.AppendLine(" WHERE (OptionGID = @Param_OptionGID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_OptionGID", Param_OptionGID);
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
                        this.lt_OptionGID.Text = DT.Rows[0]["OptionGID"].ToString();
                        this.tb_OptionGName.Text = DT.Rows[0]["OptionGName"].ToString();
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
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷(群組名稱)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Prod_BOMSpec_OptionGroup ");
            SBSql.AppendLine(" WHERE (OptionGName = @Param_OptionGName) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_OptionGName", this.tb_OptionGName.Text);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("「選單單頭名稱」重複新增！", "");
                    return;
                }
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 取得編號    
            string MyNewID = "";
            SBSql.AppendLine(" --取得編號(第2位取4碼) ");
            SBSql.AppendLine(" DECLARE @Max_ID AS INT ");
            SBSql.AppendLine(" SET @Max_ID = (SELECT ISNULL(MAX(SUBSTRING(OptionGID,2,4)), 0) + 1 FROM Prod_BOMSpec_OptionGroup) ");
            SBSql.AppendLine(" --設定編號(5碼:G1234) ");
            SBSql.AppendLine(" DECLARE @New_ID AS NCHAR(5) ");
            SBSql.AppendLine(" SET @New_ID = 'G' + RIGHT(CAST('000' + CAST(@Max_ID AS NVARCHAR) AS NVARCHAR), 4) ");
            SBSql.AppendLine(" SELECT @New_ID AS MyNewID ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                MyNewID = DT.Rows[0]["MyNewID"].ToString();
            }
            if (string.IsNullOrEmpty(MyNewID))
            {
                fn_Extensions.JsAlert("無法取得系統編號！", "");
                return;
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - Insert
            SBSql.AppendLine(" INSERT INTO Prod_BOMSpec_OptionGroup( ");
            SBSql.AppendLine("      OptionGID, OptionGName, Display, Sort");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("      @New_ID, @Param_OptionGName, @Param_Display, @Param_Sort");
            SBSql.AppendLine(" )");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_OptionGName", this.tb_OptionGName.Text.Trim());
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            cmd.Parameters.AddWithValue("New_ID", MyNewID);
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                if (Param_Func.Equals("set"))
                {
                    fn_Extensions.JsAlert("資料新增成功！\\n請繼續新增「選單」資料。"
                        , "script:location.href='SpecOption_BOM_Edit.aspx?func=set&OptionGID=" + Server.UrlEncode(MyNewID) + "&SpecID=" + Server.UrlEncode(Param_SpecID) + "';");
                }
                else
                {
                    fn_Extensions.JsAlert("資料新增成功！", "script:parent.location.reload();");
                }

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
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷(群組名稱)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Prod_BOMSpec_OptionGroup ");
            SBSql.AppendLine(" WHERE (OptionGName = @Param_OptionGName) AND (OptionGID <> @Param_OptionGID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_OptionGName", this.tb_OptionGName.Text.Trim());
            cmd.Parameters.AddWithValue("Param_OptionGID", Param_OptionGID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("「選單單頭名稱」重複新增！", "");
                    return;
                }
            }
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Prod_BOMSpec_OptionGroup ");
            SBSql.AppendLine(" SET OptionGName = @Param_OptionGName ");
            SBSql.AppendLine("      , Display = @Param_Display, Sort = @Param_Sort ");
            SBSql.AppendLine(" WHERE (OptionGID = @Param_OptionGID)");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_OptionGID", Param_OptionGID);
            cmd.Parameters.AddWithValue("Param_OptionGName", this.tb_OptionGName.Text.Trim());
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

    /// <summary>
    /// 規格欄位編號
    /// </summary>
    private string _Param_SpecID;
    public string Param_SpecID
    {
        get
        {
            return Request.QueryString["SpecID"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["SpecID"].ToString());
        }
        set
        {
            this._Param_SpecID = value;
        }
    }

    //[參數] - 編號
    private string _Param_OptionGID;
    public string Param_OptionGID
    {
        get
        {
            return this._Param_OptionGID != null ? this._Param_OptionGID : this.lt_OptionGID.Text;
        }
        set
        {
            this._Param_OptionGID = value;
        }
    }

    #endregion

}