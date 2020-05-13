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
using System.Collections;
using ExtensionIO;


public partial class Icon_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[初始化]
                string ErrMsg;
                //[權限判斷] - 符號資料庫
                if (fn_CheckAuth.CheckAuth_User("401", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "IconSet/Icon_Search.aspx";

                //[取得資料] - 符號使用者
                if (fn_Extensions.IconUseMenu(this.ddl_Icon_Type, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("符號使用者選單產生失敗！", "");
                }

                //[參數判斷] - 是否為修改資料
                if (string.IsNullOrEmpty(Param_thisID) == false)
                {
                    View_Data();

                    this.pl_Detail.Visible = true;
                    this.pl_Tip1.Visible = false;
                    this.pl_Tip2.Visible = true;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");
                this.btn_Upload.Attributes["onclick"] = fn_Extensions.BlockJs(
                   "PicAdd",
                   "<div style=\"text-align:left\">圖片上傳中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", Session["BackListUrl"].ToString());
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 基本資料
    /// </summary>
    private void View_Data()
    {
        try
        {
            string ErrMsg;

            //[取得/檢查參數] - 系統編號
            if (fn_Extensions.Num_正整數(Param_thisID, "1", "999999999", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                return;
            }

            //[取得資料] - ICON單頭資料
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Icon.* ");
                //維護資訊
                SBSql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Icon.Create_Who)) AS Create_Name ");
                SBSql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Icon.Update_Who)) AS Update_Name ");
                SBSql.AppendLine(" FROM Icon ");
                SBSql.AppendLine(" WHERE (Icon_ID = @Icon_ID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Icon_ID", Param_thisID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //填入資料
                        this.lt_Icon_ID.Text = DT.Rows[0]["Icon_ID"].ToString();
                        this.tb_CID.Text = DT.Rows[0]["CID"].ToString();
                        this.ddl_Icon_Type.SelectedValue = DT.Rows[0]["Icon_Type"].ToString();
                        this.tb_IconName_zh_TW.Text = DT.Rows[0]["IconName_zh_TW"].ToString();
                        this.tb_IconName_en_US.Text = DT.Rows[0]["IconName_en_US"].ToString();
                        this.tb_IconName_zh_CN.Text = DT.Rows[0]["IconName_zh_CN"].ToString();
                        this.rbl_Display.SelectedValue = DT.Rows[0]["Display"].ToString();
                        this.tb_Sort.Text = DT.Rows[0]["Sort"].ToString();

                        //填入建立 & 修改資料
                        this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                        this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");

                        //Flag設定 & 欄位顯示/隱藏
                        this.hf_flag.Value = "Edit";

                        //顯示明細資料
                        LookupDataList();

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }

    /// <summary>
    /// 副程式 - 取得明細資料列表
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine("SELECT Icon_Pics.Pic_ID, Icon_Pics.Pic_File, Icon_Pics.Sort ");
                SBSql.AppendLine("  , Icon.IconName_zh_TW AS IconName ");
                SBSql.AppendLine("  FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                SBSql.AppendLine(" WHERE (Icon.Icon_ID = @Icon_ID) ");
                SBSql.AppendLine(" ORDER BY Sort ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Icon_ID", Param_thisID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    if (DT.Rows.Count == 0)
                    {
                        this.btn_SaveSort.Visible = false;
                        this.btn_DelAll.Visible = false;
                    }
                    else
                    {
                        this.btn_SaveSort.Visible = true;
                        this.btn_DelAll.Visible = true;
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 明細資料！", "");
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                switch (e.CommandName)
                {
                    case "Del":
                        #region * 刪除 *
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            string ErrMsg;
                            cmd.Parameters.Clear();

                            //[取得參數] - 編號
                            string GetDataID = ((Literal)e.Item.FindControl("lt_PicID")).Text;

                            //[SQL] - 取得已上傳的檔案
                            StringBuilder SBSql = new StringBuilder();
                            SBSql.AppendLine(" SELECT Pic_File ");
                            SBSql.AppendLine(" FROM Icon_Pics WHERE (Pic_ID = @Param_ID) ");
                            cmd.CommandText = SBSql.ToString();
                            cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                            //[暫存參數] - 檔案名稱
                            List<string> ListFiles = new List<string>();
                            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                            {
                                for (int row = 0; row < DT.Rows.Count; row++)
                                {
                                    //暫存檔案名稱
                                    ListFiles.Add(DT.Rows[row]["Pic_File"].ToString());
                                }
                            }

                            //[SQL] - 刪除資料
                            SBSql.Clear();
                            SBSql.AppendLine(" DELETE FROM Icon_Pics WHERE (Pic_ID = @Param_ID) ");
                            cmd.CommandText = SBSql.ToString();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                            {
                                if (ErrMsg.IndexOf("條件約束") != -1)
                                {
                                    fn_Extensions.JsAlert("此圖片已被使用！", "");
                                }
                                else
                                {
                                    fn_Extensions.JsAlert("圖片刪除失敗！", "");
                                }
                            }
                            else
                            {
                                //刪除檔案
                                for (int idx = 0; idx < ListFiles.Count; idx++)
                                {
                                    IOManage.DelFile(Param_FileFolder, ListFiles[idx]);
                                }

                                //頁面跳至明細
                                fn_Extensions.JsAlert("圖片刪除成功！", PageUrl);
                            }
                        }
                        #endregion

                        break;

                    case "NewPic":
                        #region * 置換 *
                        //宣告 - 暫存檔案名稱
                        List<TempParam> ITempList = new List<TempParam>();
                        //參數 - 取得舊檔案
                        string ParamOldFile = ((HiddenField)e.Item.FindControl("hf_OldFile")).Value;
                        //參數 - 取得列表上的控制項
                        FileUpload ParamObj1 = ((FileUpload)e.Item.FindControl("fu_NewPic"));
                        //參數 - 取得新檔案
                        HttpPostedFile hpNewPic = ParamObj1.PostedFile;
                        //參數 - 檔案名稱
                        string ParamNewFile = "";
                        if (hpNewPic.ContentLength == 0)
                        {
                            fn_Extensions.JsAlert("尚未選擇檔案！", "");
                            return;
                        }
                        else
                        {
                            //[IO] - 取得檔案名稱
                            IOManage.GetFileName(hpNewPic);
                            ParamNewFile = IOManage.FileNewName;
                        }

                        //[SQL] - 更新置換檔名
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            string ErrMsg;
                            cmd.Parameters.Clear();

                            //[取得參數] - 編號
                            string GetDataID = ((Literal)e.Item.FindControl("lt_PicID")).Text;

                            StringBuilder SBSql = new StringBuilder();
                            SBSql.AppendLine(" UPDATE Icon_Pics ");
                            SBSql.AppendLine("  SET Pic_File = @Pic_File, Update_Who = @Update_Who, Update_Time = GETDATE() ");
                            SBSql.AppendLine(" WHERE (Pic_ID = @Param_ID)");
                            cmd.CommandText = SBSql.ToString();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                            cmd.Parameters.AddWithValue("Pic_File", ParamNewFile);
                            cmd.Parameters.AddWithValue("Update_Who", fn_Param.CurrentAccount.ToString());
                            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                            {
                                fn_Extensions.JsAlert("圖片置換失敗！", "");
                            }
                            else
                            {
                                //[IO] - 刪除舊檔案
                                IOManage.DelFile(Param_FileFolder, ParamOldFile);

                                //[IO] - 儲存檔案
                                IOManage.Save(hpNewPic, Param_FileFolder, ParamNewFile);

                                //轉頁 
                                fn_Extensions.JsAlert("圖片置換成功！", PageUrl);
                            }
                        }

                        #endregion

                        break;
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    #endregion -- 資料顯示 End --

    #region -- 資料編輯 Start --
    /// <summary>
    /// 存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg = "";
            StringBuilder SBAlert = new StringBuilder();
            //[參數檢查] - 必填項目
            if (this.tb_CID.Text.IsNumeric() == false)
            { SBAlert.Append("請輸入正確的「自訂編號」\\n"); }

            //[參數檢查] - 選填項目
            if (fn_Extensions.String_字數(this.tb_IconName_zh_TW.Text, "0", "100", out ErrMsg) == false)
            { SBAlert.Append("「名稱 - 繁中」請輸入1 ~ 100個字\\n"); }
            if (fn_Extensions.String_字數(this.tb_IconName_zh_TW.Text, "0", "150", out ErrMsg) == false)
            { SBAlert.Append("「名稱 - 英文」請輸入1 ~ 150個字\\n"); }
            if (fn_Extensions.String_字數(this.tb_IconName_zh_TW.Text, "0", "100", out ErrMsg) == false)
            { SBAlert.Append("「名稱 - 簡中」請輸入1 ~ 100個字\\n"); }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region "資料儲存"
            //判斷是新增 or 修改
            switch (this.hf_flag.Value.ToUpper())
            {
                case "ADD":
                    Add_Data();
                    break;

                case "EDIT":
                    Edit_Data();
                    break;

                default:
                    throw new Exception("走錯路囉!");
            }
            #endregion

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 存檔", "");
            return;
        }

    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {
            //--- 判斷資料重複 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Icon ");
            SBSql.AppendLine(" WHERE (CID = @CID) AND (Icon_Type = @Icon_Type)  ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("CID", this.tb_CID.Text);
            cmd.Parameters.AddWithValue("Icon_Type", this.ddl_Icon_Type.SelectedValue);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("自訂編號重複！", "");
                    return;
                }
            }

            //--- 取得新編號 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 取得最新編號
            int New_ID;
            SBSql.AppendLine(" SELECT (ISNULL(MAX(Icon_ID), 0) + 1) AS New_ID FROM Icon ");
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                New_ID = Convert.ToInt32(DT.Rows[0]["New_ID"]);
            }

            //--- 開始新增資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料新增
            SBSql.AppendLine(" INSERT INTO Icon( ");
            SBSql.AppendLine("  Icon_ID, CID, Icon_Type");
            SBSql.AppendLine("  , IconName_zh_TW, IconName_en_US, IconName_zh_CN");
            SBSql.AppendLine("  , Display, Sort, Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @New_ID, @CID, @Icon_Type");
            SBSql.AppendLine("  , @IconName_zh_TW, @IconName_en_US, @IconName_zh_CN");
            SBSql.AppendLine("  , @Display, @Sort, @Param_CreateWho, GETDATE() ");
            SBSql.AppendLine(" )");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("New_ID", New_ID);
            cmd.Parameters.AddWithValue("CID", this.tb_CID.Text.Trim());
            cmd.Parameters.AddWithValue("Icon_Type", this.ddl_Icon_Type.SelectedValue);
            cmd.Parameters.AddWithValue("IconName_zh_TW", this.tb_IconName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("IconName_en_US", this.tb_IconName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("IconName_zh_CN", this.tb_IconName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！"
                    , string.Format(@"Icon_Edit.aspx?Icon_ID={0}", HttpUtility.UrlEncode(Cryptograph.Encrypt(Convert.ToString(New_ID))))
                );
                return;
            }
        }
    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {
            //--- 判斷資料重複 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Icon ");
            SBSql.AppendLine(" WHERE (Icon_ID <> @Icon_ID) AND (CID = @CID) AND (Icon_Type = @Icon_Type) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Icon_ID", this.lt_Icon_ID.Text);
            cmd.Parameters.AddWithValue("CID", this.tb_CID.Text);
            cmd.Parameters.AddWithValue("Icon_Type", this.ddl_Icon_Type.SelectedValue);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("自訂編號重複！", "");
                    return;
                }
            }

            //--- 開始更新資料 ---
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            SBSql.Clear();
            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Icon ");
            SBSql.AppendLine(" SET CID = @CID, Icon_Type = @Icon_Type ");
            SBSql.AppendLine("  , IconName_zh_TW = @IconName_zh_TW, IconName_en_US = @IconName_en_US, IconName_zh_CN = @IconName_zh_CN");
            SBSql.AppendLine("  , Display = @Display, Sort = @Sort");
            SBSql.AppendLine("  , Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Icon_ID = @Icon_ID) ");
            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Icon_ID", Param_thisID);
            cmd.Parameters.AddWithValue("CID", this.tb_CID.Text.Trim());
            cmd.Parameters.AddWithValue("Icon_Type", this.ddl_Icon_Type.SelectedValue);
            cmd.Parameters.AddWithValue("IconName_zh_TW", this.tb_IconName_zh_TW.Text.Trim());
            cmd.Parameters.AddWithValue("IconName_en_US", this.tb_IconName_en_US.Text.Trim());
            cmd.Parameters.AddWithValue("IconName_zh_CN", this.tb_IconName_zh_CN.Text.Trim());
            cmd.Parameters.AddWithValue("Display", this.rbl_Display.SelectedValue);
            cmd.Parameters.AddWithValue("Sort", this.tb_Sort.Text);
            cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", PageUrl);
                return;
            }
        }
    }

    /// <summary>
    /// 上傳圖片
    /// </summary>
    protected void btn_Upload_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            //[IO] - 暫存檔案名稱
            List<TempParam> ITempList = new List<TempParam>();
            HttpFileCollection hfc = Request.Files;
            for (int i = 0; i <= hfc.Count - 1; i++)
            {
                HttpPostedFile hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    //[IO] - 取得檔案名稱
                    IOManage.GetFileName(hpf);
                    ITempList.Add(new TempParam(IOManage.FileNewName, IOManage.FileFullName, hpf));
                }
            }
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" Declare @New_ID AS INT ");

                for (int i = 0; i < ITempList.Count; i++)
                {
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM Icon_Pics) ");
                    SBSql.AppendLine(" INSERT INTO Icon_Pics( ");
                    SBSql.AppendLine("  Icon_ID, Pic_ID, Sort, Create_Who, Create_Time");
                    SBSql.AppendLine("  ,Pic_File");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Icon_ID, @New_ID, 999, @Param_CreateWho, GETDATE()");
                    SBSql.AppendLine(string.Format(" , @FileNewName_{0} ", i));
                    SBSql.AppendLine(" ) ");
                    cmd.Parameters.AddWithValue("FileNewName_" + i, ITempList[i].Param_Pic);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Icon_ID", Param_thisID);
                cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("圖片上傳失敗！", "");
                }
                else
                {
                    //[IO] - 儲存檔案
                    for (int i = 0; i < ITempList.Count; i++)
                    {
                        HttpPostedFile hpf = ITempList[i].Param_hpf;
                        if (hpf.ContentLength > 0)
                        {
                            //[IO] - 儲存檔案
                            IOManage.Save(hpf, Param_FileFolder, ITempList[i].Param_Pic);
                        }
                    }

                    fn_Extensions.JsAlert("圖片上傳成功！", PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 上傳！", "");
            return;
        }
    }

    /// <summary>
    /// 儲存排序
    /// </summary>
    protected void btn_SaveSort_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("目前尚無資料可設定！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - Statement
                StringBuilder SBSql = new StringBuilder();
                for (int i = 0; i < lvDataList.Items.Count; i++)
                {
                    //[取得參數] - 編號
                    string lvParam_ID = ((Literal)this.lvDataList.Items[i].FindControl("lt_PicID")).Text;
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[i].FindControl("tb_Sort")).Text.Trim();

                    SBSql.AppendLine(" UPDATE Icon_Pics SET ");
                    SBSql.AppendLine(" Update_Who = @Param_UpdateWho, Update_Time = GETDATE()");
                    SBSql.AppendLine(string.Format(
                        ", Sort = @lvParam_Sort_{0} WHERE (Pic_ID = @lvParam_ID_{0}) ; "
                        , i));

                    cmd.Parameters.AddWithValue("lvParam_ID_" + i, lvParam_ID);
                    cmd.Parameters.AddWithValue("lvParam_Sort_" + i, lvParam_Sort);
                }
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("排序儲存失敗！", "");
                }
                else
                {
                    fn_Extensions.JsAlert("排序儲存成功！", PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存排序！", "");
        }
    }

    /// <summary>
    /// 全部刪除
    /// </summary>
    protected void btn_DelAll_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            if (this.lvDataList.Items.Count == 0)
            {
                fn_Extensions.JsAlert("目前無資料可刪除！", "");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - Statement
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Icon_Pics ");
                SBSql.AppendLine(" WHERE (Icon_ID = @Icon_ID) ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Icon_ID", Param_thisID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除失敗！", "");
                }
                else
                {
                    //[IO] - 刪除資料夾
                    IOManage.DelFolder(Param_FileFolder);

                    fn_Extensions.JsAlert("刪除成功！", PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 全部刪除！", "");
        }
    }

    #endregion -- 資料編輯 End --

    #region -- 其他功能 --
    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <param name="OrgPicName">原始檔名</param>
    /// <returns>string</returns>
    public string PicUrl(string PicName, string OrgPicName)
    {
        string preView = "";

        //判斷是否為圖片
        string strFileExt = ".jpg||.png||.gif";
        if (fn_Extensions.CheckStrWord(PicName, strFileExt, "|", 2))
        {
            //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
            preView = string.Format(
                "<td class=\"L2Img\" width=\"80px\" valign=\"top\"> " +
                "<img src=\"{0}\" width=\"80px\" border=\"0\" class=\"PicGroup L2Img\" rel=\"PicGroup\" href=\"{0}\" title=\"{1}\" style=\"cursor:pointer\">" +
                "</td>"
                , Param_WebFolder + PicName
                , OrgPicName);
        }

        //輸出Html
        return preView;
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_WebFolder : Application["File_DiskUrl"] + @"Icons\";
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"Icons/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// 本筆資料的編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["Icon_ID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["Icon_ID"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return string.Format(@"Icon_Edit.aspx?Icon_ID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_thisID)));
        }
        set
        {
            this._PageUrl = value;
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

        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_Pic">系統檔名</param>
        /// <param name="Param_OrgPic">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        public TempParam(string Param_Pic, string Param_OrgPic, HttpPostedFile Param_hpf)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_hpf = Param_hpf;
        }
    }

}
