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
using ExtensionIO;


public partial class SpecOption_Edit : SecurityIn
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
                    Session["BackListUrl"] = Application["WebUrl"] + "ProdSpec/SpecOption_Search.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_Edit.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "GPAdd",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //判斷來源
                if (Param_Func.Equals("set"))
                {
                    this.lt_Head.Text = "Step2. 設定選單資料";
                    this.btn_Next.Visible = true;
                }

                //[帶出選單] - 選單單頭編號
                Get_GroupIDMenu();

                //[判斷 & 取得參數] - 選單單頭代號
                if (Request.QueryString["OptionGID"] != null)
                {
                    this.ddl_OptionGID.SelectedIndex = this.ddl_OptionGID.Items.IndexOf(
                             this.ddl_OptionGID.Items.FindByValue(Request.QueryString["OptionGID"].ToString())
                             );
                }
                //[取得資料] - 符號表
                if (fn_Extensions.OptIconMenu(this.rbl_Icon, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("符號表產生失敗！", "script:parent.$.fancybox.close()");
                    return;
                }
                this.rbl_Icon.Items.Insert(0, new ListItem("無", ""));
                this.rbl_Icon.SelectedIndex = 0;

                //[參數判斷] - 載入資料
                Load_Data(Cryptograph.Decrypt(Request.QueryString["OptionID"]));
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "script:parent.$.fancybox.close()");
            }

        }
    }

    #region -- 資料讀取 --
    /// <summary>
    /// 產生單頭代號選單(來源:Prod_Spec_OptionGroup)
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
                SBSql.AppendLine(" FROM Prod_Spec_OptionGroup ");
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
                        this.ddl_OptionGID.Items.Insert(0, new ListItem("-- 選擇單頭代號 --", ""));
                        this.ddl_OptionGID.SelectedIndex = 0;
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生單頭代號選單！", "");
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
            //[取得參數] - OptionID(系統編號)
            Param_OptionID = string.IsNullOrEmpty(inputID) ? "" : inputID;
            if (string.IsNullOrEmpty(Param_OptionID))
            {
                return;
            }

            //[檢查參數] - OptionID(系統編號)
            if (false == fn_Extensions.Num_正整數(Param_OptionID, "1", "2147483600", out ErrMsg))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得資料] - 讀取資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  OptionID, OptionGID, OptionGName, Spec_OptionValue, Spec_OptionPic, Spec_OptionFile ");
                SBSql.AppendLine("  , Spec_OptionName_zh_TW, Spec_OptionName_en_US, Spec_OptionName_zh_CN ");
                SBSql.AppendLine("  , Display, Sort");
                SBSql.AppendLine(" FROM Prod_Spec_Option ");
                SBSql.AppendLine(" WHERE (OptionID = @Param_OptionID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_OptionID", Param_OptionID);
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
                        this.hf_OptionID.Value = DT.Rows[0]["OptionID"].ToString();
                        this.lt_OptionID.Text = DT.Rows[0]["OptionID"].ToString();
                        this.ddl_OptionGID.SelectedIndex = this.ddl_OptionGID.Items.IndexOf(
                          this.ddl_OptionGID.Items.FindByValue(DT.Rows[0]["OptionGID"].ToString())
                          );
                        this.lt_Spec_OptionValue.Text = DT.Rows[0]["Spec_OptionValue"].ToString();
                        this.tb_Spec_OptionName_zh_TW.Text = DT.Rows[0]["Spec_OptionName_zh_TW"].ToString();
                        this.tb_Spec_OptionName_en_US.Text = DT.Rows[0]["Spec_OptionName_en_US"].ToString();
                        this.tb_Spec_OptionName_zh_CN.Text = DT.Rows[0]["Spec_OptionName_zh_CN"].ToString();
                        this.rbl_Icon.SelectedIndex = this.rbl_Icon.Items.IndexOf(
                          this.rbl_Icon.Items.FindByValue(DT.Rows[0]["Spec_OptionPic"].ToString())
                          );
                        this.rbl_Display.SelectedIndex = this.rbl_Display.Items.IndexOf(
                          this.rbl_Display.Items.FindByValue(DT.Rows[0]["Display"].ToString())
                          );
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //判斷是否有自行上傳圖檔
                        string PicFile = DT.Rows[0]["Spec_OptionFile"].ToString();
                        if (false == string.IsNullOrEmpty(PicFile))
                        {
                            //[按鈕] - 刪除
                            this.btn_Del_File.Visible = true;

                            //填入圖片資料
                            this.lt_File.Text = "<img src=\"{0}\" width=\"80\" class=\"tooltip\" iconName=\"{1}\" />".FormatThis(
                                "{0}{1}".FormatThis(Param_WebFolder, PicFile)
                                , "");

                            //暫存檔名
                            this.hf_File.Value = PicFile;
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
    /// <summary>
    /// 編輯
    /// </summary>
    protected void btn_Edit_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (false == fn_Extensions.String_字數(this.ddl_OptionGID.SelectedValue, "5", "5", out ErrMsg))
            { SBAlert.Append("「單頭代號」空白\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_Spec_OptionName_zh_TW.Text, "1", "50", out ErrMsg))
            { SBAlert.Append("「選項顯示名稱 - 繁中」請輸入1 ~ 50個字\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_Spec_OptionName_en_US.Text, "1", "200", out ErrMsg))
            { SBAlert.Append("「選項顯示名稱 - 英文」請輸入1 ~ 200個字\\n"); }
            if (false == fn_Extensions.String_字數(this.tb_Spec_OptionName_zh_CN.Text, "1", "50", out ErrMsg))
            { SBAlert.Append("「選項顯示名稱 - 簡中」請輸入1 ~ 50個字\\n"); }
            if (false == fn_Extensions.Num_正整數(this.tb_Sort.Text, "1", "999", out ErrMsg))
            { SBAlert.Append("「排序」請輸入1 ~ 999的數字\\n"); }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //副檔名檢查參數
            int errExt = 0;
            //暫存檔案參數
            List<TempParam> PicTmp = new List<TempParam>();
            //判斷是否有上傳檔案
            FileUpload fu_Pic = (FileUpload)Page.FindControl("fu_File");
            HttpPostedFile hpFile = fu_Pic.PostedFile;
            if (hpFile != null)
            {
                if (hpFile.ContentLength != 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpFile);

                    //判斷副檔名，未符合規格的檔案不上傳
                    if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 2))
                    {
                        //取得目前檔名
                        HiddenField hf_Pic = (HiddenField)Page.FindControl("hf_File");
                        //暫存檔案資訊
                        PicTmp.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hf_Pic.Value, hpFile));
                    }
                    else
                    {
                        errExt++;
                    }
                }
            }

            //未符合檔案規格的警示訊息
            if (errExt > 0)
            {
                fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
                return;
            }

            #endregion

            #region "資料儲存"
            //判斷是否有資料
            if (string.IsNullOrEmpty(this.hf_OptionID.Value))
            {
                //Insert
                Add_Data(PicTmp);
            }
            else
            {
                //Update
                Edit_Data(PicTmp);
            }

            #endregion
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 編輯", "");
        }
    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        Add_Data(null);
    }

    private void Add_Data(List<TempParam> PicTmp)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料判斷(選項顯示名稱)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Prod_Spec_Option ");
            SBSql.AppendLine(" WHERE (Spec_OptionName_zh_TW = @Param_SpecName) AND (OptionGID = @Param_OptionGID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_SpecName", this.tb_Spec_OptionName_zh_TW.Text);
            cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("「選項顯示名稱」重複新增(同一單頭)！", "");
                    return;
                }
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();

            //[SQL] - 取得編號(系統編號)
            SBSql.AppendLine(" DECLARE @Sys_ID AS INT ");
            SBSql.AppendLine(" SET @Sys_ID = (SELECT ISNULL(MAX(OptionID), 0) + 1 FROM Prod_Spec_Option) ");
            SBSql.AppendLine(" SELECT @Sys_ID AS MyNewID ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                Param_OptionID = DT.Rows[0]["MyNewID"].ToString();
            }
            if (string.IsNullOrEmpty(Param_OptionID))
            {
                fn_Extensions.JsAlert("無法取得系統編號！", "");
                return;
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 取得編號(選項值)
            SBSql.AppendLine(" --取得編號(第2位取3碼) ");
            SBSql.AppendLine(" DECLARE @Max_ID AS INT ");
            SBSql.AppendLine(" SET @Max_ID = (SELECT ISNULL(MAX(SUBSTRING(Spec_OptionValue,2,3)), 0) + 1 FROM Prod_Spec_Option WHERE (OptionGID = @Param_OptionGID)) ");
            SBSql.AppendLine(" --設定編號(4碼:V123) ");
            SBSql.AppendLine(" DECLARE @New_ID AS NCHAR(4) ");
            SBSql.AppendLine(" SET @New_ID = 'V' + RIGHT(CAST('00' + CAST(@Max_ID AS NVARCHAR) AS NVARCHAR), 3) ");
            //[SQL] - 取得單頭名稱
            SBSql.AppendLine(" DECLARE @Param_OptionGName AS NVARCHAR(50) ");
            SBSql.AppendLine(" SET @Param_OptionGName = (SELECT OptionGName FROM Prod_Spec_OptionGroup WHERE (OptionGID = @Param_OptionGID)) ");
            //[SQL] - Insert
            SBSql.AppendLine(" INSERT INTO Prod_Spec_Option( ");
            SBSql.AppendLine("      OptionID, OptionGID, OptionGName, Spec_OptionValue, Spec_OptionPic, Spec_OptionFile, Display, Sort");
            SBSql.AppendLine("      , Spec_OptionName_zh_TW, Spec_OptionName_en_US, Spec_OptionName_zh_CN ");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("      @Sys_ID, @Param_OptionGID, @Param_OptionGName");
            SBSql.AppendLine("      , @New_ID, @Param_Spec_OptionPic, @Param_Spec_OptionFile, @Param_Display, @Param_Sort");
            SBSql.AppendLine("      , @Spec_OptionName_zh_TW, @Spec_OptionName_en_US, @Spec_OptionName_zh_CN ");
            SBSql.AppendLine(" )");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Spec_OptionPic", this.rbl_Icon.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Spec_OptionFile", PicTmp.Count == 0 ? "" : PicTmp[0].Param_Pic);
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            cmd.Parameters.AddWithValue("Sys_ID", Param_OptionID);
            cmd.Parameters.AddWithValue("Spec_OptionName_zh_TW", this.tb_Spec_OptionName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("Spec_OptionName_en_US", this.tb_Spec_OptionName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("Spec_OptionName_zh_CN", this.tb_Spec_OptionName_zh_CN.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                //[IO] - 儲存檔案(PostFile, 檔案路徑, 檔案名稱)
                if (PicTmp.Count > 0)
                {
                    IOManage.Save(PicTmp[0].Param_hpf, Param_DiskFolder, PicTmp[0].Param_Pic, Param_Width, Param_Height);
                }

                //動作判斷
                if (Param_Func.Equals("set") || Param_Func.Equals("add"))
                {
                    string url = "Spec_Edit.aspx?func=set&SpecID=" + Server.UrlEncode(Cryptograph.Encrypt(Param_SpecID)) + "&OptionGID=" + Server.UrlEncode(this.ddl_OptionGID.SelectedValue);
                    string js = "if(confirm('資料新增成功！\\n是否要繼續新增')){" + PageUrl + "}else{location.href='" + url + "';}";
                    ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
                    return;
                }
                else if (Param_Func.Equals("setedit"))
                {
                    fn_Extensions.JsAlert("資料新增成功！\\n請重新查詢。", "script:parent.$.fancybox.close();");
                }
                else
                {
                    fn_Extensions.JsAlert("資料新增成功！", "script:parent.$.fancybox.close();" + PageUrl);
                }
            }
        }
    }


    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        Edit_Data(null);
    }

    private void Edit_Data(List<TempParam> PicTmp)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷(選項顯示名稱)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Prod_Spec_Option ");
            SBSql.AppendLine(" WHERE (Spec_OptionName_zh_TW = @Param_SpecName) AND (OptionGID = @Param_OptionGID) AND (OptionID <> @Param_OptionID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_SpecName", this.tb_Spec_OptionName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
            cmd.Parameters.AddWithValue("Param_OptionID", Param_OptionID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("「選項顯示名稱」重複新增(同一單頭)！", "");
                    return;
                }
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料更新
            //[SQL] - 取得單頭名稱
            SBSql.AppendLine(" DECLARE @Param_OptionGName AS NVARCHAR(50) ");
            SBSql.AppendLine(" SET @Param_OptionGName = (SELECT OptionGName FROM Prod_Spec_OptionGroup WHERE (OptionGID = @Param_OptionGID)) ");
            SBSql.AppendLine(" UPDATE Prod_Spec_Option ");
            SBSql.AppendLine(" SET OptionGID = @Param_OptionGID, OptionGName = @Param_OptionGName ");
            SBSql.AppendLine("  , Spec_OptionValue = @Param_Spec_OptionValue, Spec_OptionPic = @Param_Spec_OptionPic ");
            SBSql.AppendLine("  , Display = @Param_Display, Sort = @Param_Sort ");
            SBSql.AppendLine("  , Spec_OptionName_zh_TW = @Spec_OptionName_zh_TW, Spec_OptionName_en_US = @Spec_OptionName_en_US, Spec_OptionName_zh_CN = @Spec_OptionName_zh_CN ");
            if (PicTmp.Count > 0)
            {
                SBSql.Append("  , Spec_OptionFile = @Param_Spec_OptionFile");
                cmd.Parameters.AddWithValue("Param_Spec_OptionFile", PicTmp[0].Param_Pic);
            }
            SBSql.AppendLine(" WHERE (OptionID = @Param_OptionID)");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_OptionID", Param_OptionID);
            cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Spec_OptionValue", this.lt_Spec_OptionValue.Text);
            cmd.Parameters.AddWithValue("Param_Spec_OptionPic", this.rbl_Icon.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Param_Sort", this.tb_Sort.Text.Trim());
            cmd.Parameters.AddWithValue("Spec_OptionName_zh_TW", this.tb_Spec_OptionName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("Spec_OptionName_en_US", this.tb_Spec_OptionName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("Spec_OptionName_zh_CN", this.tb_Spec_OptionName_zh_CN.Text.Trim());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
            }
            else
            {
                //[檔案處理]
                if (PicTmp.Count > 0)
                {
                    //[IO] - 儲存檔案(PostFile, 檔案路徑, 檔案名稱)
                    IOManage.Save(PicTmp[0].Param_hpf, Param_DiskFolder, PicTmp[0].Param_Pic, Param_Width, Param_Height);
                    //[IO] - 刪除舊檔案(檔案路徑, 舊檔案名稱)
                    IOManage.DelFile(Param_DiskFolder, PicTmp[0].Param_CurrentPic);
                }


                if (Param_Func.Equals("setedit"))
                {
                    fn_Extensions.JsAlert("資料更新成功！\\n請重新查詢。", "script:parent.$.fancybox.close();");
                }
                else
                {
                    fn_Extensions.JsAlert("資料更新成功！", "script:parent.$.fancybox.close();" + PageUrl);
                }
            }
        }
    }
    #endregion

    #region -- 按鈕區 --
    /// <summary>
    /// 刪除資料
    /// </summary>
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

                //[SQL] - 檢查是否已被關聯 / Prod_Spec所屬單頭資料，至少要有一筆
                SBSql.AppendLine(" DECLARE @RowNum1 AS INT, @RowNum2 AS INT, @RowNum3 AS INT ");
                SBSql.AppendLine(" SET @RowNum1 = (SELECT COUNT(*) FROM Prod_Spec WHERE (OptionGID = @Param_OptionGID)) ");
                SBSql.AppendLine(" SET @RowNum2 = (SELECT COUNT(*) FROM Prod_Spec Spec");
                SBSql.AppendLine("     INNER JOIN Prod_Spec_List List ON Spec.SpecID = List.SpecID ");
                SBSql.AppendLine("     WHERE (Spec.OptionGID = @Param_OptionGID) AND (List.ListValue = @Param_ListValue)");
                SBSql.AppendLine(" ) ");
                SBSql.AppendLine(" SET @RowNum3 = (SELECT COUNT(*) FROM Prod_Spec_Option WHERE (OptionGID = @Param_OptionGID)) ");
                //單頭代號有被使用, 判斷該代號至少要有一筆資料
                SBSql.AppendLine(" IF(@RowNum1 > 0 AND @RowNum3 < 2) ");
                SBSql.AppendLine("  SELECT 'Y' AS IsRel ");
                //判斷OptionID是否已被使用
                SBSql.AppendLine(" ELSE IF(@RowNum2 > 0)  ");
                SBSql.AppendLine("  SELECT 'Y' AS IsRel ");
                SBSql.AppendLine(" ELSE ");
                SBSql.AppendLine("  SELECT 'N' AS IsRel ");
                cmd.Parameters.AddWithValue("Param_ListValue", this.lt_Spec_OptionValue.Text);
                cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
                cmd.Parameters.AddWithValue("Param_OptionID", Param_OptionID);
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows[0]["IsRel"].Equals("Y"))
                    {
                        fn_Extensions.JsAlert("無法刪除，此項目已被使用！", "script:parent.$.fancybox.close()");
                        return;
                    }
                }

                //[SQL] - 刪除資料
                SBSql.Clear();
                SBSql.AppendLine(" DELETE FROM Prod_Spec_Option WHERE (OptionID = @Param_OptionID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_OptionID", Param_OptionID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料刪除失敗！", "");
                }
                else
                {
                    //刪除檔案
                    IOManage.DelFile(Param_DiskFolder, this.hf_File.Value);

                    fn_Extensions.JsAlert("資料刪除成功！", "script:parent.$.fancybox.close();" + PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除", "");
        }
    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 檢查此單頭代號是否已建立至少一筆選單
                SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Prod_Spec_Option WHERE (OptionGID = @Param_OptionGID)) = 0 ");
                SBSql.AppendLine("	SELECT 'N' AS IsSet ");
                SBSql.AppendLine(" ELSE ");
                SBSql.AppendLine("	SELECT 'Y' AS IsSet ");
                cmd.Parameters.AddWithValue("Param_OptionGID", this.ddl_OptionGID.SelectedValue);
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows[0]["IsSet"].Equals("N"))
                    {
                        fn_Extensions.JsAlert("尚未新增「選單」資料，至少要新增 1 筆！", "");
                        return;
                    }
                }
                //導向下一步
                Response.Redirect(
                    "Spec_Edit.aspx?func=set&SpecID=" + Server.UrlEncode(Cryptograph.Encrypt(Param_SpecID)) +
                    "&OptionGID=" + Server.UrlEncode(this.ddl_OptionGID.SelectedValue)
                    , false);
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 下一步", "");
        }
    }

    /// <summary>
    /// 刪除檔案
    /// </summary>
    protected void btn_Del_File_Click(object sender, EventArgs e)
    {
        try
        {
            //宣告
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 刪除資料
                SBSql.AppendLine(" UPDATE Prod_Spec_Option SET Spec_OptionFile = '' WHERE (OptionID = @Param_OptionID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_OptionID", Param_OptionID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("檔案刪除失敗！", "");
                }
                else
                {
                    //刪除檔案
                    IOManage.DelFile(Param_DiskFolder, this.hf_File.Value);

                    fn_Extensions.JsAlert("", "script:" + PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除", "");
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 參數 - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return Application["File_WebUrl"] + @"ProductSpec/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// 參數 - Disk資料夾路徑
    /// </summary>
    private string _Param_DiskFolder;
    public string Param_DiskFolder
    {
        get
        {
            return Application["File_DiskUrl"] + @"ProductSpec\";
        }
        set
        {
            this._Param_DiskFolder = value;
        }
    }

    /// <summary>
    /// 參數 - 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            //判斷來源
            if (Param_Func.Equals("map"))
            {
                return string.Format("parent.location.reload();");
            }
            else if (Param_Func.Equals("set") || Param_Func.Equals("add"))
            {
                return "location.href='SpecOption_Edit.aspx?func={0}&OptionGID={1}&SpecID={2}&r={3}'"
                   .FormatThis(Param_Func
                    , Server.UrlEncode(this.ddl_OptionGID.SelectedValue)
                    , Server.UrlEncode(Param_SpecID)
                    , String.Format("{0:mmssfff}", DateTime.Now));

            }
            else if (Param_Func.Equals("edit"))
            {

                return "location.href='SpecOption_Edit.aspx?func={0}&OptionID={1}&r={2}'"
                    .FormatThis(Param_Func
                        , Server.UrlEncode(Cryptograph.Encrypt(Param_OptionID))
                        , String.Format("{0:mmssfff}", DateTime.Now));
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
    /// 參數 - 規格欄位編號
    /// </summary>
    private string _Param_SpecID;
    public string Param_SpecID
    {
        get
        {
            return Request.QueryString["SpecID"] == null ? "" : fn_stringFormat.Filter_Html(Request.QueryString["SpecID"].ToString().Trim());
        }
        set
        {
            this._Param_SpecID = value;
        }
    }

    /// <summary>
    /// 參數 - 選單編號
    /// </summary>
    private string _Param_OptionID;
    public string Param_OptionID
    {
        get
        {
            return this._Param_OptionID != null ? this._Param_OptionID : this.lt_OptionID.Text;
        }
        set
        {
            this._Param_OptionID = value;
        }
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return ".jpg||.png";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 400;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 400;
        }
        set
        {
            this._Param_Height = value;
        }
    }
    #endregion


    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
        }

        /// <summary>
        /// [參數] - 目前圖片檔名
        /// </summary>
        private string _Param_CurrentPic;
        public string Param_CurrentPic
        {
            get { return this._Param_CurrentPic; }
            set { this._Param_CurrentPic = value; }
        }

        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Pic">圖片檔名</param>
        /// <param name="Param_OrgPic">圖片原始名稱</param>
        /// <param name="Param_CurrentPic">目前圖片檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        public TempParam(string Param_Pic, string Param_OrgPic, string Param_CurrentPic
                , HttpPostedFile Param_hpf)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_CurrentPic = Param_CurrentPic;
            this._Param_hpf = Param_hpf;
        }
    }
}