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
using System.Text.RegularExpressions;
using LogRecord;
using ExtensionIO;

public partial class Cert_Edit_DTL : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg = "";

                //[權限判斷] - 認證資料庫 - 編輯
                if (fn_CheckAuth.CheckAuth_User("201", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("無使用權限！", "script:parent.$.fancybox.close()");
                    return;
                }

                //[按鈕] - 加入BlockUI
                this.btn_Edit.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "GPAdd",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - Cert_ID(系統編號)
                Param_CertID = string.IsNullOrEmpty(Request.QueryString["Cert_ID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["Cert_ID"].ToString());
                if (fn_Extensions.Num_正整數(Param_CertID, "1", "999999999", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                    return;
                }
                this.hf_Cert_ID.Value = Param_CertID; //暫存參數
                //[取得/檢查參數] - Detail_ID
                Param_DetailID = string.IsNullOrEmpty(Request.QueryString["Detail_ID"]) ? "" : Cryptograph.Decrypt(Request.QueryString["Detail_ID"].ToString());

                //[取得參數] - 品號
                if (Request.QueryString["ModelNo"] != null)
                {
                    this.hf_ModelNo.Value = Cryptograph.Decrypt(Request.QueryString["ModelNo"].ToString());
                }

                //[取得資料] - 類別選單
                if (fn_Extensions.CertMenu(this.ddl_Cert_Type, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("類別選單產生失敗！", "script:parent.$.fancybox.close()");
                    return;
                }
                //[取得資料] - 符號表
                if (fn_Extensions.CertIconMenu(this.cbl_Icon, Param_CertID, Param_DetailID, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("符號表產生失敗！", "script:parent.$.fancybox.close()");
                    return;
                }

                //[參數判斷] - 是否為修改資料
                if (string.IsNullOrEmpty(Param_DetailID) == false)
                {
                    LookupData(ErrMsg);
                }

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "script:parent.$.fancybox.close()");
            }

        }
    }

    #region ---按鈕區---
    protected void btn_Edit_Click(object sender, EventArgs e)
    {
        try
        {
            #region "欄位檢查"
            string ErrMsg;
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 必填項目
            if (Param_CertApproveDate.IsDate() == false)
            {
                SBAlert.Append("「發證日期」格式錯誤\\n");
            }

            //[參數檢查] - 選填項目            
            if (fn_Extensions.String_字數(Param_CertNo, "0", "50", out ErrMsg) == false)
            { SBAlert.Append("「證書編號」請輸入1 ~ 50個字\\n"); }
            if (fn_Extensions.String_字數(Param_CertCmd, "0", "100", out ErrMsg) == false)
            { SBAlert.Append("「認證指令」請輸入1 ~ 100個字\\n"); }
            if (fn_Extensions.String_字數(Param_CertNorm, "0", "255", out ErrMsg) == false)
            { SBAlert.Append("「認證規範」請輸入1 ~ 255個字\\n"); }
            if (fn_Extensions.String_字數(Param_CertDesc1, "0", "300", out ErrMsg) == false)
            { SBAlert.Append("「測試器/主機/安全等級」請輸入1 ~ 300個字\\n"); }
            if (fn_Extensions.String_字數(Param_CertDesc2, "0", "300", out ErrMsg) == false)
            { SBAlert.Append("「測試棒/安全等級」請輸入1 ~ 300個字\\n"); }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }
            #endregion

            #region --檔案處理--
            //[參數檢查] - 證書
            HttpPostedFile hpFile = this.fu_Cert_File.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_CertFile = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_cert" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_CertFileFullName = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;

            //[參數檢查] - Test Report
            hpFile = this.fu_Cert_File_Report.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileTestReport = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_report" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameTestReport = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;

            //[參數檢查] - 自我宣告
            hpFile = this.fu_Cert_File_CE.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileCE = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_ce" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameCE = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;
            //[參數檢查] - 自我檢測
            hpFile = this.fu_Cert_File_Check.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileCheck = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_check" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameCheck = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;

            //[參數檢查] - 自我宣告_en_US
            hpFile = this.fu_Cert_File_CE_en_US.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileCE_en_US = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_ce_en_US" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameCE_en_US = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;
            //[參數檢查] - 自我檢測_en_US
            hpFile = this.fu_Cert_File_Check_en_US.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileCheck_en_US = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_check_en_US" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameCheck_en_US = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;

            //[參數檢查] - 自我宣告_zh_CN
            hpFile = this.fu_Cert_File_CE_zh_CN.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileCE_zh_CN = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_ce_zh_CN" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameCE_zh_CN = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;
            //[參數檢查] - 自我檢測_zh_CN
            hpFile = this.fu_Cert_File_Check_zh_CN.PostedFile;
            if (hpFile.ContentLength != 0)
            {
                //[IO] - 取得副檔名(.xxx)
                string GetFileExtend = Path.GetExtension(hpFile.FileName);
                //[IO] - 檔案重新命名
                Param_FileCheck_zh_CN = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + "_check_zh_CN" + GetFileExtend;
                //[IO] - 取得完整檔名
                Param_FullNameCheck_zh_CN = Path.GetFileName(hpFile.FileName);
            }
            hpFile = null;
            #endregion

            #region "資料儲存"
            switch (this.btn_Edit.Text)
            {
                case "新增":
                    Add_Data();
                    break;

                case "修改":
                    Edit_Data();
                    break;

                default:
                    fn_Extensions.JsAlert("操作錯誤", "script:parent.$.fancybox.close();");
                    break;
            }
            #endregion
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤", "");
        }

    }

    protected void btn_Del_File_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File = '', Cert_OrgFile = ''", this.ViewState["CertFile"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_Report_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_Report = '', Cert_OrgFile_Report = ''", this.ViewState["FileReport"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_CE_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_CE = '', Cert_OrgFile_CE = ''", this.ViewState["FileCE"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_CE_en_US_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_CE_en_US = '', Cert_OrgFile_CE_en_US = ''", this.ViewState["FileCE_en_US"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_CE_zh_CN_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_CE_zh_CN = '', Cert_OrgFile_CE_zh_CN = ''", this.ViewState["FileCE_zh_CN"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_Check_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_Check = '', Cert_OrgFile_Check = ''", this.ViewState["FileCheck"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_Check_en_US_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_Check_en_US = '', Cert_OrgFile_Check_en_US = ''", this.ViewState["FileCheck_en_US"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    protected void btn_Del_Check_zh_CN_Click(object sender, EventArgs e)
    {
        string ErrMsg;
        if (DelFile(",Cert_File_Check_zh_CN = '', Cert_OrgFile_Check_zh_CN = ''", this.ViewState["FileCheck_zh_CN"].ToString()
            , out ErrMsg))
        {
            fn_Extensions.JsAlert("檔案刪除成功！", PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("檔案刪除失敗！", "");
            return;
        }
    }

    /// <summary>
    /// 刪除檔案
    /// </summary>
    /// <param name="sqlColumn"></param>
    /// <param name="certFile"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool DelFile(string sqlColumn, string certFile, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Prod_Certification_Detail ");
                SBSql.AppendLine(" SET ");
                SBSql.AppendLine("  Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                SBSql.AppendLine(sqlColumn);
                SBSql.AppendLine(" WHERE (Cert_ID = @Param_CertID) AND (Detail_ID = @Param_DetailID ) ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_CertID", Param_CertID);
                cmd.Parameters.AddWithValue("Param_DetailID", Param_DetailID);
                cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    return false;
                }
                else
                {
                    //寫入Log
                    fn_Log.Log_Rec("認證資料"
                        , Param_ModelNo
                        , "刪除認證檔案,品號:{0}, 編號:{1}, SQL欄位:「{2}」".FormatThis(Param_ModelNo, Param_DetailID, sqlColumn)
                        , fn_Param.CurrentAccount.ToString());

                    //Delete File
                    IOManage.DelFile(Param_DiskFolder, certFile);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }

    }
    #endregion ---按鈕區---

    #region ---資料顯示---
    private void LookupData(string ErrMsg)
    {
        //[取得/檢查參數] - Detail_ID(明細編號)
        if (fn_Extensions.Num_正整數(Param_DetailID, "1", "999999999", out ErrMsg) == false)
        {
            fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
            return;
        }

        //[取得資料] - 讀取資料
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT Cert.Model_No, CertDTL.* ");
            SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = CertDTL.Create_Who)) AS Create_Name ");
            SBSql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = CertDTL.Update_Who)) AS Update_Name ");
            SBSql.AppendLine(" FROM Prod_Certification Cert INNER JOIN Prod_Certification_Detail CertDTL ");
            SBSql.AppendLine("  ON Cert.Cert_ID = CertDTL.Cert_ID ");
            SBSql.AppendLine(" WHERE (CertDTL.Cert_ID = @Param_CertID) AND (CertDTL.Detail_ID = @Param_DetailID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Param_CertID", Param_CertID);
            cmd.Parameters.AddWithValue("Param_DetailID", Param_DetailID);
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
                    this.hf_Cert_ID.Value = DT.Rows[0]["Cert_ID"].ToString();
                    this.hf_Detail_ID.Value = DT.Rows[0]["Detail_ID"].ToString();
                    this.hf_ModelNo.Value = DT.Rows[0]["Model_No"].ToString();
                    this.ddl_Cert_Type.SelectedIndex = this.ddl_Cert_Type.Items.IndexOf(
                        this.ddl_Cert_Type.Items.FindByValue(DT.Rows[0]["Cert_Type"].ToString().Trim())
                        );
                    this.tb_Cert_TypeText.Text = DT.Rows[0]["Cert_TypeText"].ToString();
                    //if (DT.Rows[0]["IsCE"].ToString().Trim() == "Y")
                    //{ this.cb_IsCE.Checked = true; }
                    //else
                    //{ this.cb_IsCE.Checked = false; }
                    this.rbl_IsWebDW.SelectedValue = DT.Rows[0]["IsWebDW"].ToString();
                    this.tb_Cert_ApproveDate.Text = DT.Rows[0]["Cert_ApproveDate"].ToString().ToDateString("yyyy/MM/dd");
                    this.tb_Cert_No.Text = DT.Rows[0]["Cert_No"].ToString().Trim();
                    this.tb_Cert_RptNo.Text = DT.Rows[0]["Cert_RptNo"].ToString().Trim();
                    this.tb_Cert_Cmd.Text = DT.Rows[0]["Cert_Cmd"].ToString().Trim();
                    this.tb_Cert_Norm.Text = DT.Rows[0]["Cert_Norm"].ToString().Trim();
                    this.tb_Cert_Desc1.Text = DT.Rows[0]["Cert_Desc1"].ToString().Trim();
                    this.tb_Cert_Desc2.Text = DT.Rows[0]["Cert_Desc2"].ToString().Trim();
                    this.tb_Cert_ValidDate.Text = DT.Rows[0]["Cert_ValidDate"].ToString().ToDateString("yyyy/MM/dd");

                    #region --檔案處理--
                    //暫存參數 - 檔案 CertFile
                    this.ViewState["CertFile"] = DT.Rows[0]["Cert_File"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["CertFile"].ToString()) == false)
                    {
                        this.lt_Cert_File.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile"].ToString().Trim(), this.ViewState["CertFile"].ToString());

                        this.btn_Del_File.Visible = true;
                    }

                    //暫存參數 - 檔案 FileReport
                    this.ViewState["FileReport"] = DT.Rows[0]["Cert_File_Report"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileReport"].ToString()) == false)
                    {
                        this.lt_Cert_File_Report.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_Report"].ToString().Trim(), this.ViewState["FileReport"].ToString());

                        this.btn_Del_Report.Visible = true;
                    }

                    //暫存參數 - 檔案 FileCE
                    this.ViewState["FileCE"] = DT.Rows[0]["Cert_File_CE"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileCE"].ToString()) == false)
                    {
                        this.lt_Cert_File_CE.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_CE"].ToString().Trim(), this.ViewState["FileCE"].ToString());

                        this.btn_Del_CE.Visible = true;
                    }
                    //暫存參數 - 檔案 FileCheck
                    this.ViewState["FileCheck"] = DT.Rows[0]["Cert_File_Check"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileCheck"].ToString()) == false)
                    {
                        this.lt_Cert_File_Check.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_Check"].ToString().Trim(), this.ViewState["FileCheck"].ToString());

                        this.btn_Del_Check.Visible = true;
                    }

                    //暫存參數 - 檔案 FileCE_en_US
                    this.ViewState["FileCE_en_US"] = DT.Rows[0]["Cert_File_CE_en_US"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileCE_en_US"].ToString()) == false)
                    {
                        this.lt_Cert_File_CE_en_US.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_CE_en_US"].ToString().Trim(), this.ViewState["FileCE_en_US"].ToString());

                        this.btn_Del_CE_en_US.Visible = true;
                    }
                    //暫存參數 - 檔案 FileCheck_en_US
                    this.ViewState["FileCheck_en_US"] = DT.Rows[0]["Cert_File_Check_en_US"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileCheck_en_US"].ToString()) == false)
                    {
                        this.lt_Cert_File_Check_en_US.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_Check_en_US"].ToString().Trim(), this.ViewState["FileCheck_en_US"].ToString());

                        this.btn_Del_Check_en_US.Visible = true;
                    }

                    //暫存參數 - 檔案 FileCE_zh_CN
                    this.ViewState["FileCE_zh_CN"] = DT.Rows[0]["Cert_File_CE_zh_CN"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileCE_zh_CN"].ToString()) == false)
                    {
                        this.lt_Cert_File_CE_zh_CN.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_CE_zh_CN"].ToString().Trim(), this.ViewState["FileCE_zh_CN"].ToString());

                        this.btn_Del_CE_zh_CN.Visible = true;
                    }
                    //暫存參數 - 檔案 FileCheck_zh_CN
                    this.ViewState["FileCheck_zh_CN"] = DT.Rows[0]["Cert_File_Check_zh_CN"].ToString().Trim();
                    if (string.IsNullOrEmpty(this.ViewState["FileCheck_zh_CN"].ToString()) == false)
                    {
                        this.lt_Cert_File_Check_zh_CN.Text = setFileUri(
                            DT.Rows[0]["Cert_OrgFile_Check_zh_CN"].ToString().Trim(), this.ViewState["FileCheck_zh_CN"].ToString());

                        this.btn_Del_Check_zh_CN.Visible = true;
                    }
                    #endregion

                    //填入建立 & 修改資料
                    this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                    this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                    this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                    this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");

                    //[按鈕設定]
                    this.btn_Edit.Text = "修改";
                }
            }
        }

    }

    /// <summary>
    /// 設定檔案下載連結
    /// </summary>
    /// <param name="orgName">原始檔名</param>
    /// <param name="fileName">真實檔名</param>
    /// <returns>string</returns>
    private string setFileUri(string orgName, string fileName)
    {
        string uri = string.Format("../FileDownload.ashx?OrgiName={0}&FilePath={1}"
            , Server.UrlEncode(orgName.Trim())
            , Server.UrlEncode(Cryptograph.Encrypt(Param_DiskFolder + fileName)));

        return "&lt;<a href=\"" + uri + "\">檔案下載</a>&gt;&nbsp;置換檔案,";
    }
    #endregion ---資料顯示 End---

    #region ---資料編輯---
    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data()
    {
        string ErrMsg = "";
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料判斷(證書編號)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Prod_Certification_Detail ");
            SBSql.AppendLine(" WHERE (Cert_ID = @Param_CertID) AND (Cert_No = @Param_CertNo) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_CertID", Param_CertID);
            cmd.Parameters.AddWithValue("Param_CertNo", Param_CertNo);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("「證書編號」重複新增！", "");
                    return;
                }
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料新增
            SBSql.AppendLine(" Declare @New_ID AS INT SET @New_ID = (SELECT ISNULL(MAX(Detail_ID), 0) + 1 FROM Prod_Certification_Detail WHERE (Cert_ID = @Param_CertID)) ");
            SBSql.AppendLine(" INSERT INTO Prod_Certification_Detail( ");
            SBSql.AppendLine("  Cert_ID, Detail_ID");
            SBSql.AppendLine("  , Cert_Type, Cert_TypeText, IsCE, Cert_No, Cert_RptNo, Cert_Cmd, Cert_Norm");
            SBSql.AppendLine("  , Cert_ApproveDate, Cert_ValidDate, Cert_Desc1, Cert_Desc2");
            SBSql.AppendLine("  , Cert_File, Cert_OrgFile, Cert_File_Report, Cert_OrgFile_Report");
            SBSql.AppendLine("  , Cert_File_CE, Cert_OrgFile_CE, Cert_File_Check, Cert_OrgFile_Check");
            SBSql.AppendLine("  , Cert_File_CE_en_US, Cert_OrgFile_CE_en_US, Cert_File_Check_en_US, Cert_OrgFile_Check_en_US ");
            SBSql.AppendLine("  , Cert_File_CE_zh_CN, Cert_OrgFile_CE_zh_CN, Cert_File_Check_zh_CN, Cert_OrgFile_Check_zh_CN ");
            SBSql.AppendLine("  , IsWebDW ");
            SBSql.AppendLine("  , Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @Param_CertID, @New_ID");
            SBSql.AppendLine("  , @Param_CertType, @Param_CertTypeText, 'N', @Param_CertNo, @Param_CertRptNo, @Param_CertCmd, @Param_CertNorm");
            SBSql.AppendLine("  , @Param_CertApproveDate, @Param_CertValidDate, @Param_CertDesc1, @Param_CertDesc2");
            SBSql.AppendLine("  , @Param_CertFile, @Param_CertFileFullName, @Param_FileTestReport, @Param_FullNameTestReport");
            SBSql.AppendLine("  , @Param_FileCE, @Param_FullNameCE, @Param_FileCheck, @Param_FullNameCheck");
            SBSql.AppendLine("  , @Param_FileCE_en_US, @Param_FullNameCE_en_US, @Param_FileCheck_en_US, @Param_FullNameCheck_en_US");
            SBSql.AppendLine("  , @Param_FileCE_zh_CN, @Param_FullNameCE_zh_CN, @Param_FileCheck_zh_CN, @Param_FullNameCheck_zh_CN");
            SBSql.AppendLine("  , @Param_IsWebDW ");
            SBSql.AppendLine("  , @Param_CreateWho, GETDATE() ");
            SBSql.AppendLine(" ); ");

            //認證符號
            #region * 認證符號 *
            if (false == string.IsNullOrEmpty(Param_CertIcon))
            {
                string[] strAry = Regex.Split(Param_CertIcon, @"\,{1}");
                for (int row = 0; row < strAry.Length; row++)
                {
                    SBSql.AppendLine(" INSERT INTO Icon_Rel_Certification ( ");
                    SBSql.AppendLine("  Pic_ID, Cert_ID, Detail_ID ");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Pic_ID_" + row + ", @Param_CertID, @New_ID ");
                    SBSql.AppendLine(" ); ");
                    cmd.Parameters.AddWithValue("Pic_ID_" + row, strAry[row].ToString());
                }
            }

            //Update time
            SBSql.AppendLine(" UPDATE Prod_Certification ");
            SBSql.AppendLine(" SET Update_Who = @Param_CreateWho, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Cert_ID = @Param_CertID);");

            ////判斷是否為CE, 若是則新增圖片關聯(自訂編號 = 108)
            //if (Param_IsCE.Equals("Y"))
            //{
            //    SBSql.AppendLine(" INSERT INTO Icon_Rel_Certification ");
            //    SBSql.AppendLine(" SELECT Pic_ID, @Param_CertID, @New_ID FROM Icon_Pics WHERE Icon_ID IN ( ");
            //    SBSql.AppendLine("  SELECT Icon_ID FROM Icon WHERE CID = 108 ");
            //    SBSql.AppendLine(" ); ");
            //}
            #endregion

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_CertID", Param_CertID);
            cmd.Parameters.AddWithValue("Param_CertType", Param_CertType);
            cmd.Parameters.AddWithValue("Param_CertTypeText", Param_CertTypeText);
            //cmd.Parameters.AddWithValue("Param_IsCE", Param_IsCE);
            cmd.Parameters.AddWithValue("Param_CertNo", Param_CertNo);
            cmd.Parameters.AddWithValue("Param_CertRptNo", Param_CertRptNo);
            cmd.Parameters.AddWithValue("Param_CertCmd", Param_CertCmd);
            cmd.Parameters.AddWithValue("Param_CertNorm", Param_CertNorm);
            cmd.Parameters.AddWithValue("Param_CertApproveDate", Param_CertApproveDate);
            cmd.Parameters.AddWithValue("Param_CertValidDate", Param_CertValidDate == "" ? DBNull.Value : (object)Param_CertValidDate);
            cmd.Parameters.AddWithValue("Param_CertDesc1", Param_CertDesc1);
            cmd.Parameters.AddWithValue("Param_CertDesc2", Param_CertDesc2);
            #region --檔案--
            cmd.Parameters.AddWithValue("Param_CertFile", Param_CertFile);
            cmd.Parameters.AddWithValue("Param_CertFileFullName", Param_CertFileFullName);
            cmd.Parameters.AddWithValue("Param_FileTestReport", Param_FileTestReport);
            cmd.Parameters.AddWithValue("Param_FullNameTestReport", Param_FullNameTestReport);
            cmd.Parameters.AddWithValue("Param_FileCE", Param_FileCE);
            cmd.Parameters.AddWithValue("Param_FullNameCE", Param_FullNameCE);
            cmd.Parameters.AddWithValue("Param_FileCheck", Param_FileCheck);
            cmd.Parameters.AddWithValue("Param_FullNameCheck", Param_FullNameCheck);
            cmd.Parameters.AddWithValue("Param_FileCE_en_US", Param_FileCE_en_US);
            cmd.Parameters.AddWithValue("Param_FullNameCE_en_US", Param_FullNameCE_en_US);
            cmd.Parameters.AddWithValue("Param_FileCheck_en_US", Param_FileCheck_en_US);
            cmd.Parameters.AddWithValue("Param_FullNameCheck_en_US", Param_FullNameCheck_en_US);
            cmd.Parameters.AddWithValue("Param_FileCE_zh_CN", Param_FileCE_zh_CN);
            cmd.Parameters.AddWithValue("Param_FullNameCE_zh_CN", Param_FullNameCE_zh_CN);
            cmd.Parameters.AddWithValue("Param_FileCheck_zh_CN", Param_FileCheck_zh_CN);
            cmd.Parameters.AddWithValue("Param_FullNameCheck_zh_CN", Param_FullNameCheck_zh_CN);
            #endregion
            cmd.Parameters.AddWithValue("Param_IsWebDW", Param_IsWebDW);
            cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                Response.Write(ErrMsg);
                fn_Extensions.JsAlert("資料新增失敗！", "");
                return;
            }
            else
            {
                #region --檔案處理--
                //[判斷檔案] - 證書, 檔案上傳
                IOManage.Save(this.fu_Cert_File.PostedFile, Param_DiskFolder, Param_CertFile);
                //[判斷檔案] - TestReport, 檔案上傳
                IOManage.Save(this.fu_Cert_File_Report.PostedFile, Param_DiskFolder, Param_FileTestReport);
                //[判斷檔案] - 自我宣告, 檔案上傳
                IOManage.Save(this.fu_Cert_File_CE.PostedFile, Param_DiskFolder, Param_FileCE);
                //[判斷檔案] - 自我檢測, 檔案上傳
                IOManage.Save(this.fu_Cert_File_Check.PostedFile, Param_DiskFolder, Param_FileCheck);
                //[判斷檔案] - 自我宣告_en_US, 檔案上傳
                IOManage.Save(this.fu_Cert_File_CE_en_US.PostedFile, Param_DiskFolder, Param_FileCE_en_US);
                //[判斷檔案] - 自我檢測_en_US, 檔案上傳
                IOManage.Save(this.fu_Cert_File_Check_en_US.PostedFile, Param_DiskFolder, Param_FileCheck_en_US);
                //[判斷檔案] - 自我宣告_zh_CN, 檔案上傳
                IOManage.Save(this.fu_Cert_File_CE_zh_CN.PostedFile, Param_DiskFolder, Param_FileCE_zh_CN);
                //[判斷檔案] - 自我檢測_zh_CN, 檔案上傳
                IOManage.Save(this.fu_Cert_File_Check_zh_CN.PostedFile, Param_DiskFolder, Param_FileCheck_zh_CN);
                #endregion

                fn_Extensions.JsAlert("資料新增成功！", "script:parent.location.href=\'Cert_Edit.aspx?id=" + Param_CertID + "&r=" + String.Format("{0:mmssfff}", DateTime.Now) + "#section1\'");
                return;
            }
        }
    }

    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        string ErrMsg = "";
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料判斷(證書編號)
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT COUNT(*) AS CheckNum FROM Prod_Certification_Detail ");
            SBSql.AppendLine(" WHERE (Cert_ID = @Param_CertID) AND (Detail_ID <> @Param_DetailID ) AND (Cert_No = @Param_CertNo) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_CertID", Param_CertID);
            cmd.Parameters.AddWithValue("Param_CertNo", Param_CertNo);
            cmd.Parameters.AddWithValue("Param_DetailID", Param_DetailID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (Convert.ToInt32(DT.Rows[0]["CheckNum"]) > 0)
                {
                    fn_Extensions.JsAlert("「證書編號」重複新增！", "");
                    return;
                }
            }

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();
            //[SQL] - 資料更新
            SBSql.AppendLine(" UPDATE Prod_Certification_Detail ");
            SBSql.AppendLine(" SET Cert_Type = @Param_CertType, Cert_TypeText = @Param_CertTypeText ");
            SBSql.AppendLine("  , IsCE = 'N'");
            SBSql.AppendLine("  , Cert_No = @Param_CertNo, Cert_RptNo = @Param_CertRptNo, Cert_Cmd = @Param_CertCmd, Cert_Norm = @Param_CertNorm ");
            SBSql.AppendLine("  , Cert_ApproveDate = @Param_CertApproveDate, Cert_ValidDate = @Param_CertValidDate ");
            SBSql.AppendLine("  , Cert_Desc1 = @Param_CertDesc1, Cert_Desc2 = @Param_CertDesc2 ");
            SBSql.AppendLine("  , IsWebDW = @Param_IsWebDW ");
            SBSql.AppendLine("  , Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
            #region --檔案--
            //[SQL] - 判斷是否有上傳檔案
            if (false == string.IsNullOrEmpty(Param_CertFile))
            {
                SBSql.AppendLine("  , Cert_File = @Param_CertFile, Cert_OrgFile = @Param_CertFileFullName");
                cmd.Parameters.AddWithValue("Param_CertFile", Param_CertFile);
                cmd.Parameters.AddWithValue("Param_CertFileFullName", Param_CertFileFullName);
            }
            if (false == string.IsNullOrEmpty(Param_FileTestReport))
            {
                SBSql.AppendLine("  , Cert_File_Report = @Param_FileTestReport, Cert_OrgFile_Report = @Param_FullNameTestReport");
                cmd.Parameters.AddWithValue("Param_FileTestReport", Param_FileTestReport);
                cmd.Parameters.AddWithValue("Param_FullNameTestReport", Param_FullNameTestReport);
            }
            if (false == string.IsNullOrEmpty(Param_FileCE))
            {
                SBSql.AppendLine("  , Cert_File_CE = @Param_FileCE, Cert_OrgFile_CE = @Param_FullNameCE");
                cmd.Parameters.AddWithValue("Param_FileCE", Param_FileCE);
                cmd.Parameters.AddWithValue("Param_FullNameCE", Param_FullNameCE);
            }
            if (false == string.IsNullOrEmpty(Param_FileCheck))
            {
                SBSql.AppendLine("  , Cert_File_Check = @Param_FileCheck, Cert_OrgFile_Check = @Param_FullNameCheck");
                cmd.Parameters.AddWithValue("Param_FileCheck", Param_FileCheck);
                cmd.Parameters.AddWithValue("Param_FullNameCheck", Param_FullNameCheck);
            }

            if (false == string.IsNullOrEmpty(Param_FileCE_en_US))
            {
                SBSql.AppendLine("  , Cert_File_CE_en_US = @Param_FileCE_en_US, Cert_OrgFile_CE_en_US = @Param_FullNameCE_en_US");
                cmd.Parameters.AddWithValue("Param_FileCE_en_US", Param_FileCE_en_US);
                cmd.Parameters.AddWithValue("Param_FullNameCE_en_US", Param_FullNameCE_en_US);
            }
            if (false == string.IsNullOrEmpty(Param_FileCheck_en_US))
            {
                SBSql.AppendLine("  , Cert_File_Check_en_US = @Param_FileCheck_en_US, Cert_OrgFile_Check_en_US = @Param_FullNameCheck_en_US");
                cmd.Parameters.AddWithValue("Param_FileCheck_en_US", Param_FileCheck_en_US);
                cmd.Parameters.AddWithValue("Param_FullNameCheck_en_US", Param_FullNameCheck_en_US);
            }

            if (false == string.IsNullOrEmpty(Param_FileCE_zh_CN))
            {
                SBSql.AppendLine("  , Cert_File_CE_zh_CN = @Param_FileCE_zh_CN, Cert_OrgFile_CE_zh_CN = @Param_FullNameCE_zh_CN");
                cmd.Parameters.AddWithValue("Param_FileCE_zh_CN", Param_FileCE_zh_CN);
                cmd.Parameters.AddWithValue("Param_FullNameCE_zh_CN", Param_FullNameCE_zh_CN);
            }
            if (false == string.IsNullOrEmpty(Param_FileCheck_zh_CN))
            {
                SBSql.AppendLine("  , Cert_File_Check_zh_CN = @Param_FileCheck_zh_CN, Cert_OrgFile_Check_zh_CN = @Param_FullNameCheck_zh_CN");
                cmd.Parameters.AddWithValue("Param_FileCheck_zh_CN", Param_FileCheck_zh_CN);
                cmd.Parameters.AddWithValue("Param_FullNameCheck_zh_CN", Param_FullNameCheck_zh_CN);
            }
            #endregion
            SBSql.AppendLine(" WHERE (Cert_ID = @Param_CertID) AND (Detail_ID = @Param_DetailID ); ");

            //認證符號
            #region * 認證符號 *
            SBSql.AppendLine(" DELETE FROM Icon_Rel_Certification WHERE (Cert_ID = @Param_CertID) AND (Detail_ID = @Param_DetailID); ");

            if (false == string.IsNullOrEmpty(Param_CertIcon))
            {
                string[] strAry = Regex.Split(Param_CertIcon, @"\,{1}");
                for (int row = 0; row < strAry.Length; row++)
                {
                    SBSql.AppendLine(" INSERT INTO Icon_Rel_Certification ( ");
                    SBSql.AppendLine("  Pic_ID, Cert_ID, Detail_ID ");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @Pic_ID_" + row + ", @Param_CertID, @Param_DetailID ");
                    SBSql.AppendLine(" ); ");
                    cmd.Parameters.AddWithValue("Pic_ID_" + row, strAry[row].ToString());
                }
            }

            ////判斷是否為CE, 若是則新增圖片關聯(自訂編號 = 108)
            //if (Param_IsCE.Equals("Y"))
            //{
            //    SBSql.AppendLine(" IF (SELECT COUNT(*) FROM Icon_Rel_Certification WHERE (Cert_ID = @Param_CertID) AND (Detail_ID = @Param_DetailID) AND (Pic_ID IN (SELECT Icon_ID FROM Icon WHERE CID = 108))) = 0 ");
            //    SBSql.AppendLine(" BEGIN");
            //    SBSql.AppendLine("  INSERT INTO Icon_Rel_Certification ");
            //    SBSql.AppendLine("  SELECT Pic_ID, @Param_CertID, @Param_DetailID FROM Icon_Pics WHERE Icon_ID IN ( ");
            //    SBSql.AppendLine("   SELECT Icon_ID FROM Icon WHERE CID = 108 ");
            //    SBSql.AppendLine("  ); ");
            //    SBSql.AppendLine(" END");
            //}
            #endregion

            //Update time
            SBSql.AppendLine(" UPDATE Prod_Certification ");
            SBSql.AppendLine(" SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Cert_ID = @Param_CertID);");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_CertID", Param_CertID);
            cmd.Parameters.AddWithValue("Param_DetailID", Param_DetailID);
            cmd.Parameters.AddWithValue("Param_CertType", Param_CertType);
            cmd.Parameters.AddWithValue("Param_CertTypeText", Param_CertTypeText);
            //cmd.Parameters.AddWithValue("Param_IsCE", Param_IsCE);
            cmd.Parameters.AddWithValue("Param_CertNo", Param_CertNo);
            cmd.Parameters.AddWithValue("Param_CertRptNo", Param_CertRptNo);
            cmd.Parameters.AddWithValue("Param_CertCmd", Param_CertCmd);
            cmd.Parameters.AddWithValue("Param_CertNorm", Param_CertNorm);
            cmd.Parameters.AddWithValue("Param_CertApproveDate", Param_CertApproveDate);
            cmd.Parameters.AddWithValue("Param_CertValidDate", Param_CertValidDate == "" ? DBNull.Value : (object)Param_CertValidDate);
            cmd.Parameters.AddWithValue("Param_CertDesc1", Param_CertDesc1);
            cmd.Parameters.AddWithValue("Param_CertDesc2", Param_CertDesc2);
            cmd.Parameters.AddWithValue("Param_IsWebDW", Param_IsWebDW);
            cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
                return;
            }
            else
            {
                #region --檔案處理--
                //[判斷檔案] - 證書, 檔案上傳
                if (false == string.IsNullOrEmpty(Param_CertFile))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["CertFile"].ToString());
                    IOManage.Save(this.fu_Cert_File.PostedFile, Param_DiskFolder, Param_CertFile);
                }
                //[判斷檔案] - TestReport, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileTestReport))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileReport"].ToString());
                    IOManage.Save(this.fu_Cert_File_Report.PostedFile, Param_DiskFolder, Param_FileTestReport);
                }
                //[判斷檔案] - 自我宣告, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileCE))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileCE"].ToString());
                    IOManage.Save(this.fu_Cert_File_CE.PostedFile, Param_DiskFolder, Param_FileCE);
                }
                //[判斷檔案] - 自我檢測, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileCheck))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileCheck"].ToString());
                    IOManage.Save(this.fu_Cert_File_Check.PostedFile, Param_DiskFolder, Param_FileCheck);
                }
                //[判斷檔案] - 自我宣告_en_US, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileCE_en_US))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileCE_en_US"].ToString());
                    IOManage.Save(this.fu_Cert_File_CE_en_US.PostedFile, Param_DiskFolder, Param_FileCE_en_US);
                }
                //[判斷檔案] - 自我檢測_en_US, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileCheck_en_US))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileCheck_en_US"].ToString());
                    IOManage.Save(this.fu_Cert_File_Check_en_US.PostedFile, Param_DiskFolder, Param_FileCheck_en_US);
                }
                //[判斷檔案] - 自我宣告_zh_CN, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileCE_zh_CN))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileCE_zh_CN"].ToString());
                    IOManage.Save(this.fu_Cert_File_CE_zh_CN.PostedFile, Param_DiskFolder, Param_FileCE_zh_CN);
                }
                //[判斷檔案] - 自我檢測_zh_CN, 檔案上傳/刪除舊檔
                if (false == string.IsNullOrEmpty(Param_FileCheck_zh_CN))
                {
                    IOManage.DelFile(Param_DiskFolder, this.ViewState["FileCheck_zh_CN"].ToString());
                    IOManage.Save(this.fu_Cert_File_Check_zh_CN.PostedFile, Param_DiskFolder, Param_FileCheck_zh_CN);
                }

                #endregion

                //寫入Log
                fn_Log.Log_Rec("認證資料"
                    , Param_ModelNo
                    , "修改認證明細,品號:{0}, 編號:{1}".FormatThis(Param_ModelNo, Param_DetailID)
                    , fn_Param.CurrentAccount.ToString());
                
                fn_Extensions.JsAlert("資料更新成功！", "script:parent.location.href=\'Cert_Edit.aspx?id=" + Param_CertID + "&r=" + String.Format("{0:mmssfff}", DateTime.Now) + "#section1\'");
                return;
            }
        }
    }
    #endregion ---資料編輯 End---

    #region --參數設定--
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return string.Format(@"Cert_Edit_DTL.aspx?Cert_ID={0}&Detail_ID={1}"
                , HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_CertID))
                , HttpUtility.UrlEncode(Cryptograph.Encrypt(Param_DetailID)));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// [參數] - Disk資料夾路徑
    /// </summary>
    private string _Param_DiskFolder;
    public string Param_DiskFolder
    {
        get
        {
            return Application["File_DiskUrl"] + string.Format(@"Certification\{0}\", Param_ModelNo);
        }
        set
        {
            this._Param_DiskFolder = value;
        }
    }

    //[參數] - 品號
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return this._Param_ModelNo != null ? this._Param_ModelNo : this.hf_ModelNo.Value;
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }

    //[參數] - 編號
    private string _Param_CertID;
    public string Param_CertID
    {
        get
        {
            return this._Param_CertID != null ? this._Param_CertID : this.hf_Cert_ID.Value;
        }
        private set
        {
            this._Param_CertID = value;
        }
    }

    //[參數] - 明細編號
    private string _Param_DetailID;
    public string Param_DetailID
    {
        get
        {
            return this._Param_DetailID != null ? this._Param_DetailID : this.hf_Detail_ID.Value;
        }
        private set
        {
            this._Param_DetailID = value;
        }
    }

    //[參數] - 證書類別
    private string _Param_CertType;
    public string Param_CertType
    {
        get
        {
            return this._Param_CertType != null ? this._Param_CertType : this.ddl_Cert_Type.SelectedValue;
        }
        private set
        {
            this._Param_CertType = value;
        }
    }

    //[參數] - 證書類別 - 其他
    private string _Param_CertTypeText;
    public string Param_CertTypeText
    {
        get
        {
            return this._Param_CertTypeText != null ? this._Param_CertTypeText : this.tb_Cert_TypeText.Text.Trim();
        }
        private set
        {
            this._Param_CertTypeText = value;
        }
    }

    ////[參數] - 是否為CE
    //private string _Param_IsCE;
    //public string Param_IsCE
    //{
    //    get
    //    {
    //        if (this._Param_IsCE != null)
    //        {
    //            return this._Param_IsCE;
    //        }
    //        else
    //        {
    //            if (this.cb_IsCE.Checked)
    //                return "Y";
    //            else
    //                return "N";
    //        }
    //    }
    //    private set
    //    {
    //        this._Param_IsCE = value;
    //    }
    //}

    //[參數] - 認證符號
    private string _Param_CertIcon;
    public string Param_CertIcon
    {
        get
        {
            var selectedValues = from ListItem item in this.cbl_Icon.Items where item.Selected select item.Value;
            var delimitedString = "";
            if (selectedValues.Count() > 0)
            {
                delimitedString = selectedValues.Aggregate((x, y) => x + "," + y);
            }
            return this._Param_CertIcon != null ? this._Param_CertIcon : delimitedString;
        }
        private set
        {
            this._Param_CertIcon = value;
        }
    }

    //[參數] - 發證日期
    private string _Param_CertApproveDate;
    public string Param_CertApproveDate
    {
        get
        {
            return this._Param_CertApproveDate != null ? this._Param_CertApproveDate : this.tb_Cert_ApproveDate.Text.Trim();
        }
        private set
        {
            this._Param_CertApproveDate = value;
        }
    }

    //[參數] - 證書編號
    private string _Param_CertNo;
    public string Param_CertNo
    {
        get
        {
            return this._Param_CertNo != null ? this._Param_CertNo : this.tb_Cert_No.Text.Trim();
        }
        set
        { this._Param_CertNo = value; }
    }

    private string _Param_CertRptNo;
    public string Param_CertRptNo
    {
        get
        {
            return this._Param_CertRptNo != null ? this._Param_CertRptNo : this.tb_Cert_RptNo.Text.Trim();
        }
        set
        { this._Param_CertRptNo = value; }
    }

    //[參數] - 認證指令
    private string _Param_CertCmd;
    public string Param_CertCmd
    {
        get
        {
            return this._Param_CertCmd != null ? this._Param_CertCmd : this.tb_Cert_Cmd.Text.Trim();
        }
        set
        {
            this._Param_CertCmd = value;
        }
    }

    //[參數] - 認證規範
    private string _Param_CertNorm;
    public string Param_CertNorm
    {
        get
        {
            return this._Param_CertNorm != null ? this._Param_CertNorm : this.tb_Cert_Norm.Text.Trim();
        }
        set
        {
            this._Param_CertNorm = value;
        }
    }

    //[參數] - 測試器/主機/安全等級
    private string _Param_CertDesc1;
    public string Param_CertDesc1
    {
        get
        {
            return this._Param_CertDesc1 != null ? this._Param_CertDesc1 : this.tb_Cert_Desc1.Text.Trim();
        }
        set
        {
            this._Param_CertDesc1 = value;
        }
    }

    //[參數] - 測試棒/安全等級
    private string _Param_CertDesc2;
    public string Param_CertDesc2
    {
        get
        {
            return this._Param_CertDesc2 != null ? this._Param_CertDesc2 : this.tb_Cert_Desc2.Text.Trim();
        }
        set
        {
            this._Param_CertDesc2 = value;
        }
    }

    //[參數] - 有效日期
    private string _Param_CertValidDate;
    public string Param_CertValidDate
    {
        get
        {
            return this._Param_CertValidDate != null ? this._Param_CertValidDate : this.tb_Cert_ValidDate.Text.Trim();
        }
        set
        {
            this._Param_CertValidDate = value;
        }
    }

    //[參數] - 官網可否下載
    private string _Param_IsWebDW;
    public string Param_IsWebDW
    {
        get
        {
            return this._Param_IsWebDW != null ? this._Param_IsWebDW : this.rbl_IsWebDW.SelectedValue;
        }
        private set
        {
            this._Param_IsWebDW = value;
        }
    }
    #endregion

    #region --檔案--
    //[參數] - 檔案上傳(原始檔名), 證書
    private string _Param_CertFileFullName;
    public string Param_CertFileFullName
    {
        get
        {
            return this._Param_CertFileFullName != null ? this._Param_CertFileFullName : "";
        }
        set
        {
            this._Param_CertFileFullName = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 證書
    private string _Param_CertFile;
    public string Param_CertFile
    {
        get
        {
            return this._Param_CertFile != null ? this._Param_CertFile : "";
        }
        set
        {
            this._Param_CertFile = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), Test Report
    private string _Param_FullNameTestReport;
    public string Param_FullNameTestReport
    {
        get
        {
            return this._Param_FullNameTestReport != null ? this._Param_FullNameTestReport : "";
        }
        set
        {
            this._Param_FullNameTestReport = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), Test Report
    private string _Param_FileTestReport;
    public string Param_FileTestReport
    {
        get
        {
            return this._Param_FileTestReport != null ? this._Param_FileTestReport : "";
        }
        set
        {
            this._Param_FileTestReport = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), 自我宣告
    private string _Param_FullNameCE;
    public string Param_FullNameCE
    {
        get
        {
            return this._Param_FullNameCE != null ? this._Param_FullNameCE : "";
        }
        set
        {
            this._Param_FullNameCE = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 自我宣告
    private string _Param_FileCE;
    public string Param_FileCE
    {
        get
        {
            return this._Param_FileCE != null ? this._Param_FileCE : "";
        }
        set
        {
            this._Param_FileCE = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), 自我檢測
    private string _Param_FullNameCheck;
    public string Param_FullNameCheck
    {
        get
        {
            return this._Param_FullNameCheck != null ? this._Param_FullNameCheck : "";
        }
        set
        {
            this._Param_FullNameCheck = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 自我檢測
    private string _Param_FileCheck;
    public string Param_FileCheck
    {
        get
        {
            return this._Param_FileCheck != null ? this._Param_FileCheck : "";
        }
        set
        {
            this._Param_FileCheck = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), 自我宣告_en_US
    private string _Param_FullNameCE_en_US;
    public string Param_FullNameCE_en_US
    {
        get
        {
            return this._Param_FullNameCE_en_US != null ? this._Param_FullNameCE_en_US : "";
        }
        set
        {
            this._Param_FullNameCE_en_US = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 自我宣告_en_US
    private string _Param_FileCE_en_US;
    public string Param_FileCE_en_US
    {
        get
        {
            return this._Param_FileCE_en_US != null ? this._Param_FileCE_en_US : "";
        }
        set
        {
            this._Param_FileCE_en_US = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), 自我檢測_en_US
    private string _Param_FullNameCheck_en_US;
    public string Param_FullNameCheck_en_US
    {
        get
        {
            return this._Param_FullNameCheck_en_US != null ? this._Param_FullNameCheck_en_US : "";
        }
        set
        {
            this._Param_FullNameCheck_en_US = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 自我檢測
    private string _Param_FileCheck_en_US;
    public string Param_FileCheck_en_US
    {
        get
        {
            return this._Param_FileCheck_en_US != null ? this._Param_FileCheck_en_US : "";
        }
        set
        {
            this._Param_FileCheck_en_US = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), 自我宣告_zh_CN
    private string _Param_FullNameCE_zh_CN;
    public string Param_FullNameCE_zh_CN
    {
        get
        {
            return this._Param_FullNameCE_zh_CN != null ? this._Param_FullNameCE_zh_CN : "";
        }
        set
        {
            this._Param_FullNameCE_zh_CN = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 自我宣告_zh_CN
    private string _Param_FileCE_zh_CN;
    public string Param_FileCE_zh_CN
    {
        get
        {
            return this._Param_FileCE_zh_CN != null ? this._Param_FileCE_zh_CN : "";
        }
        set
        {
            this._Param_FileCE_zh_CN = value;
        }
    }

    //[參數] - 檔案上傳(原始檔名), 自我檢測_zh_CN
    private string _Param_FullNameCheck_zh_CN;
    public string Param_FullNameCheck_zh_CN
    {
        get
        {
            return this._Param_FullNameCheck_zh_CN != null ? this._Param_FullNameCheck_zh_CN : "";
        }
        set
        {
            this._Param_FullNameCheck_zh_CN = value;
        }
    }
    //[參數] - 檔案上傳(系統檔名), 自我檢測
    private string _Param_FileCheck_zh_CN;
    public string Param_FileCheck_zh_CN
    {
        get
        {
            return this._Param_FileCheck_zh_CN != null ? this._Param_FileCheck_zh_CN : "";
        }
        set
        {
            this._Param_FileCheck_zh_CN = value;
        }
    }
    #endregion

}