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

public partial class SpecCategory_Edit : SecurityIn
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

                //[參數判斷] - 載入資料
                Load_Data(Cryptograph.Decrypt(Request.QueryString["CateID"]));
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
            if (false == fn_Extensions.String_字數(this.tb_CateName_zh_TW.Text, "0", "50", out ErrMsg))
            { SBAlert.Append("「分類名稱」請輸入1 ~ 50個字\\n"); }
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
            //[取得參數] - CateID(系統編號)
            Param_CateID = string.IsNullOrEmpty(inputID) ? "" : inputID;
            if (string.IsNullOrEmpty(Param_CateID))
            {
                return;
            }
            //[檢查參數] - CateID(系統編號)
            if (false == fn_Extensions.Num_正整數(Param_CateID, "1", "2147483600", out ErrMsg))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  CateID, CateName_zh_TW, CateName_en_US, CateName_zh_CN, Display, Sort ");
                SBSql.AppendLine(" FROM Prod_Spec_Category ");
                SBSql.AppendLine(" WHERE (CateID = @Param_CateID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_CateID", Param_CateID);
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
                        this.lt_CateID.Text = DT.Rows[0]["CateID"].ToString();
                        this.tb_CateName_zh_TW.Text = DT.Rows[0]["CateName_zh_TW"].ToString();
                        this.tb_CateName_en_US.Text = DT.Rows[0]["CateName_en_US"].ToString();
                        this.tb_CateName_zh_CN.Text = DT.Rows[0]["CateName_zh_CN"].ToString();
                        this.rbl_Display.SelectedIndex = this.rbl_Display.Items.IndexOf(
                            this.rbl_Display.Items.FindByValue(DT.Rows[0]["Display"].ToString())
                            );
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //[按鈕設定]
                        this.btn_Edit.Text = "修改";
                        this.TrRel.Visible = true;
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

            //[SQL] - 取得編號(系統編號)
            SBSql.AppendLine(" DECLARE @Sys_ID AS INT ");
            SBSql.AppendLine(" SET @Sys_ID = (SELECT ISNULL(MAX(CateID), 0) + 1 FROM Prod_Spec_Category) ");
            SBSql.AppendLine(" SELECT @Sys_ID AS MyNewID ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                Param_CateID = DT.Rows[0]["MyNewID"].ToString();
            }
            if (string.IsNullOrEmpty(Param_CateID))
            {
                fn_Extensions.JsAlert("無法取得系統編號！", "");
                return;
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - Insert
            SBSql.AppendLine(" INSERT INTO Prod_Spec_Category( ");
            SBSql.AppendLine("      CateID, CateName_zh_TW, CateName_en_US, CateName_zh_CN, Display, Sort");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("      @MyNewID, @CateName_zh_TW, @CateName_en_US, @CateName_zh_CN, @Display, @Sort");
            SBSql.AppendLine(" )");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("MyNewID", Param_CateID);
            cmd.Parameters.AddWithValue("CateName_zh_TW", this.tb_CateName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("CateName_en_US", this.tb_CateName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("CateName_zh_CN", this.tb_CateName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！\\n請繼續設定關聯。"
                    , "script:location.href='SpecCategory_Edit.aspx?CateID="+ Cryptograph.Encrypt(Param_CateID)+"';");
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
            SBSql.AppendLine(" UPDATE Prod_Spec_Category ");
            SBSql.AppendLine(" SET CateName_zh_TW = @CateName_zh_TW, CateName_en_US = @CateName_en_US, CateName_zh_CN = @CateName_zh_CN ");
            SBSql.AppendLine("      , Display = @Display, Sort = @Sort ");
            SBSql.AppendLine(" WHERE (CateID = @Param_CateID)");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_CateID", Param_CateID);
            cmd.Parameters.AddWithValue("CateName_zh_TW", this.tb_CateName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("CateName_en_US", this.tb_CateName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("CateName_zh_CN", this.tb_CateName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text.Trim());
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
            return string.Format("parent.location.href='{0}';", Session["BackListUrl"].ToString());
        }
        set
        {
            this._PageUrl = value;
        }
    }

    //[參數] - 編號
    private string _Param_CateID;
    public string Param_CateID
    {
        get
        {
            return this._Param_CateID != null ? this._Param_CateID : this.lt_CateID.Text;
        }
        set
        {
            this._Param_CateID = value;
        }
    }

    #endregion

}