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
using System.Xml;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Collections;
using System.Text.RegularExpressions;
using LogRecord;
using ExtensionIO;


public partial class Cert_Copy : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg = "";

            //[權限判斷] - 証書複製
            if (fn_CheckAuth.CheckAuth_User("203", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[按鈕] - (開始複製) 加入onclick屬性，取得目標品號值的組合
            this.btn_Step3_Next.Attributes.Add("onclick", "Get_Item();");
        }
    }

    #region ***按鈕區***
    /// <summary>
    /// Step1 - 下一步
    /// </summary>
    protected void btn_Step1_Next_Click(object sender, EventArgs e)
    {
        if (this.tb_ModelNo_from.Text.Equals(this.val_ModelNo_from.Text) == false)
        {
            fn_Extensions.JsAlert("請選擇正確的「品號」！", "");
            return;
        }
        in_Step2();
        in_Step3();
    }

    ///// <summary>
    ///// Step2 - 上一步
    ///// </summary>
    //protected void btn_Step2_Prev_Click(object sender, EventArgs e)
    //{
    //    in_Step1();
    //}
    ///// <summary>
    ///// Step2 - 下一步
    ///// </summary>
    //protected void btn_Step2_Next_Click(object sender, EventArgs e)
    //{
    //    in_Step3();
    //}

    /// <summary>
    /// Step3 - 上一步
    /// </summary>
    protected void btn_Step3_Prev_Click(object sender, EventArgs e)
    {
        in_Step1();
    }
    /// <summary>
    /// Step3 - 開始複製 
    /// </summary>
    protected void btn_Step3_Next_Click(object sender, EventArgs e)
    {
        string ErrMsg = "";

        //[檢查參數] - 來源品號
        if (string.IsNullOrEmpty(this.tb_ModelNo_from.Text))
        {
            fn_Extensions.JsAlert("「來源品號」不可空白！", "");
            return;
        }
        //[檢查參數] - 證書項目是否選取
        bool checkItem = false;
        for (int i = 0; i < this.lvDataList.Items.Count; i++)
        {
            CheckBox cb_CertItem = (CheckBox)this.lvDataList.Items[i].FindControl("cb_CertItem");
            if (cb_CertItem.Checked)
            {
                checkItem = true;
                break;
            }
        }
        if (checkItem == false)
        {
            fn_Extensions.JsAlert("至少要選擇一筆「證書資料」！", "");
            return;
        }
        //[檢查參數] - 目標品號
        if (string.IsNullOrEmpty(this.tb_Item_Val.Text))
        {
            fn_Extensions.JsAlert("至少要有一筆「目標品號」！", "");
            return;
        }

        //[取得參數] - 已勾選的項目(證書明細編號)
        List<string> selItems = new List<string>();
        for (int i = 0; i < this.lvDataList.Items.Count; i++)
        {
            CheckBox cb_CertItem = (CheckBox)this.lvDataList.Items[i].FindControl("cb_CertItem");
            if (cb_CertItem.Checked)
            {
                if (false == string.IsNullOrEmpty(cb_CertItem.InputAttributes["value"]))
                    selItems.Add(cb_CertItem.InputAttributes["value"]);
            }
        }
        if (selItems.Count == 0)
        {
            fn_Extensions.JsAlert("勾選的資料皆無證書編號！", "");
            return;
        }

        //[取得參數] - 目標品號
        string[] strAry = Regex.Split(this.tb_Item_Val.Text, @"\|{4}");
        //篩選目標品號，移除重複資料
        List<string> validItem = new List<string>();
        var query = from el in strAry
                    group el by el.ToString() into gp
                    select new
                    {
                        Val = gp.Key
                    };
        foreach (var item in query)
        {
            validItem.Add(item.Val);
        }
        //恢復目標品號選項Html
        this.lt_Items.Text = GetItemList(true, validItem);

        //[結果區] - 顯示/隱藏
        this.lb_Status.Text = "";  //複製狀態
        this.pl_Result.Visible = false;  //結果描述
        this.ph_Message.Visible = true;  //區塊 - 複製結果

        //[執行複製] - 來源品號, 所選項目, 目標品號, 回傳已新增的編號
        List<TempParam_M> ListGetIDs = new List<TempParam_M>();
        if (CopyData(this.val_ModelNo_from.Text, selItems, validItem, ListGetIDs, out ErrMsg) == false)
        {
            //輸出資訊
            this.pl_Result.Visible = false;
            this.lb_Status.CssClass = "styleRed B";
            this.lb_Status.Text = "複製失敗! 錯誤描述：" + ErrMsg;
        }
        else
        {
            //輸出資訊
            this.pl_Result.Visible = true;
            this.lb_Status.CssClass = "styleGreen B";
            this.lb_Status.Text = "複製完成!";

            //完成時的動作(傳入已新增的編號)
            in_Finish(ListGetIDs);
        }
    }

    /// <summary>
    /// 繼續複製
    /// </summary>
    protected void lbtn_Yes_Click(object sender, EventArgs e)
    {
        //顯示Step2~3
        in_Step2();
        in_Step3();
        //隱藏結果欄
        this.ph_Message.Visible = false;
    }
    /// <summary>
    /// 重新選擇
    /// </summary>
    protected void lbtn_No_Click(object sender, EventArgs e)
    {
        Response.Redirect("Cert_Copy.aspx", true);
    }
    #endregion

    #region ***步驟***
    /// <summary>
    /// 步驟 - Step1
    /// </summary>
    private void in_Step1()
    {
        //[顯示] - 按鈕, Step1下一步
        this.tr_Step1.Visible = true;
        //[隱藏] - 區塊, Step2
        this.ph_Step2.Visible = false;
        this.ph_Step3.Visible = false;
        //[解鎖] - 品號輸入欄
        this.tb_ModelNo_from.Enabled = true;
    }

    /// <summary>
    /// 步驟 - Step2
    /// </summary>
    private void in_Step2()
    {
        //[顯示] - 區塊
        this.ph_Step1.Visible = true;
        this.ph_Step2.Visible = true;
        //[隱藏] - 區塊
        this.ph_Step3.Visible = false;

        //[顯示] - 按鈕, Step2下一步
        //this.tr_Step2.Visible = true;
        //[隱藏] - 按鈕, Step1下一步
        this.tr_Step1.Visible = false;

        //[鎖住] - 品號輸入欄
        this.tb_ModelNo_from.Enabled = false;

        //取得認證資料
        LookupDataList(this.val_ModelNo_from.Text);

        //判斷是否有資料
        if (this.lvDataList.Items.Count > 0)
        {
            CheckBox cbSelectAll = (CheckBox)this.lvDataList.FindControl("cbSelectAll");
            cbSelectAll.Checked = false;
        }

    }

    /// <summary>
    /// 步驟 - Step3
    /// </summary>
    private void in_Step3()
    {
        //[顯示] - 區塊, Step3
        this.ph_Step3.Visible = true;

        //[顯示] - 按鈕, Step3下一步
        this.tr_Step3.Visible = true;
        //[隱藏] - 按鈕, Step2下一步
        //this.tr_Step2.Visible = false;
    }

    /// <summary>
    /// 完成複製
    /// </summary>
    /// <param name="ListGetIDs">已複製的資料編號/品號</param>
    private void in_Finish(List<TempParam_M> ListGetIDs)
    {
        //隱藏, Step 1~3
        this.ph_Step1.Visible = false;
        this.ph_Step2.Visible = false;
        this.ph_Step3.Visible = false;

        //顯示,目標品號 - 檢視連結
        this.lt_ViewUrl.Text = "快速檢視：";
        this.lt_ViewUrl.Text += "<ul style=\"list-style-type:decimal\">";
        for (int i = 0; i < ListGetIDs.Count; i++)
        {
            this.lt_ViewUrl.Text += "<li>";
            this.lt_ViewUrl.Text +=
                string.Format("<a href=\"{0}\" class=\"ViewBox\">{1}</a>"
                , Application["WebUrl"] + "Certification/Cert_View.aspx?id=" + ListGetIDs[i].Param_CertID
                , ListGetIDs[i].Param_ModelNo.ToString());
            this.lt_ViewUrl.Text += "</li>";
        }
        this.lt_ViewUrl.Text += "</ul>";
    }
    #endregion

    #region ***Function Methods***

    #region "認證明細資料"
    /// <summary>
    /// 副程式 - 取得資料列表
    /// </summary>
    /// <param name="inputID">編號</param>
    private void LookupDataList(string inputID)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg = "";

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                SBSql.AppendLine("  Cert.Cert_ID, Cert.Model_No, CertDtl.* ");
                SBSql.AppendLine("  FROM Prod_Certification Cert ");
                SBSql.AppendLine("       INNER JOIN Prod_Certification_Detail CertDtl ON Cert.Cert_ID = CertDtl.Cert_ID ");
                SBSql.AppendLine(" WHERE (RTRIM(Cert.Model_No) = @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY CertDtl.Cert_Type ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", inputID);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 認證資料！", "");
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //Checkbox 新增屬性(證書編號Cert_No)
            CheckBox cb_CertItem = (CheckBox)e.Item.FindControl("cb_CertItem");
            cb_CertItem.InputAttributes["value"] = DataBinder.Eval(dataItem.DataItem, "Cert_No").ToString();

            //證書類別
            Literal lt_CertType = (Literal)e.Item.FindControl("lt_CertType");
            lt_CertType.Text = DataBinder.Eval(dataItem.DataItem, "Cert_Type").ToString();
            if (false == string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString()))
            {
                lt_CertType.Text += " - " + DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString();
            }

            //圖片顯示 - 認證符號
            Literal lt_Icon = ((Literal)e.Item.FindControl("lt_Icon"));
            //判斷是否為CE
            if (DataBinder.Eval(dataItem.DataItem, "IsCE").ToString() == "Y")
            {
                lt_Icon.Text = "<img src=\"" + Application["File_WebUrl"] + "icon_cert/108.jpg\" width=\"35\" />";
            }
            else
            {
                lt_Icon.Text = fn_Extensions.GetCertIcon(
                     DataBinder.Eval(dataItem.DataItem, "Cert_ID").ToString()
                     , DataBinder.Eval(dataItem.DataItem, "Detail_ID").ToString());
            }

            #region --檔案處理--
            //Get Value
            string _modelNo = DataBinder.Eval(dataItem.DataItem, "Model_No").ToString();

            //檔案顯示 - 證書
            Literal lt_CertFile = ((Literal)e.Item.FindControl("lt_CertFile"));
            lt_CertFile.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File").ToString()
                , _modelNo);

            //檔案顯示 - TestReport
            Literal lt_FileTestReport = ((Literal)e.Item.FindControl("lt_FileTestReport"));
            lt_FileTestReport.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Report").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File_Report").ToString()
                , _modelNo);

            //檔案顯示 - 自我宣告
            Literal lt_FileCE = ((Literal)e.Item.FindControl("lt_FileCE"));
            lt_FileCE.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE").ToString()
                , _modelNo);

            Literal lt_FileCE_enUS = ((Literal)e.Item.FindControl("lt_FileCE_enUS"));
            lt_FileCE_enUS.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_en_US").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_en_US").ToString()
                , _modelNo);

            Literal lt_FileCE_zhCN = ((Literal)e.Item.FindControl("lt_FileCE_zhCN"));
            lt_FileCE_zhCN.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_zh_CN").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_zh_CN").ToString()
                , _modelNo);

            //檔案顯示 - 自我檢測
            Literal lt_FileCheck = ((Literal)e.Item.FindControl("lt_FileCheck"));
            lt_FileCheck.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check").ToString()
                , _modelNo);

            Literal lt_FileCheck_enUS = ((Literal)e.Item.FindControl("lt_FileCheck_enUS"));
            lt_FileCheck_enUS.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_en_US").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_en_US").ToString()
                , _modelNo);

            Literal lt_FileCheck_zhCN = ((Literal)e.Item.FindControl("lt_FileCheck_zhCN"));
            lt_FileCheck_zhCN.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_zh_CN").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_zh_CN").ToString()
                , _modelNo);
            #endregion
        }
    }
    #endregion

    /// <summary>
    /// 恢復選項Html
    /// </summary>
    /// <param name="IsDel">是否能刪除</param>
    /// <param name="validItem">選項值的組合</param>
    /// <returns></returns>
    private string GetItemList(bool IsDel, List<string> validItem)
    {
        try
        {
            //[顯示Html]
            StringBuilder sbHtml = new StringBuilder();
            for (int i = 0; i < validItem.Count; i++)
            {
                sbHtml.AppendLine("<li id=\"li_" + i + "\">");
                sbHtml.AppendLine(" <table>");
                sbHtml.AppendLine("  <tr>");
                //欄位
                sbHtml.AppendLine("     <td class=\"FEItemIn\">");
                sbHtml.AppendLine("         <input type=\"text\" class=\"Item_Val\" maxlength=\"40\" style=\"width:200px\" value=\"" + validItem[i].ToString() + "\" readonly />");
                sbHtml.AppendLine("     </td>");
                //刪除鈕
                sbHtml.AppendLine("     <td class=\"FEItemControl\">");
                if (IsDel)
                    sbHtml.Append("<a href=\"javascript:Delete_Item('" + i + "');\">刪除</a>");
                sbHtml.AppendLine("     </td>");
                sbHtml.AppendLine("  </tr>");
                sbHtml.AppendLine(" </table>");
                sbHtml.AppendLine("</li>");
            }

            return sbHtml.ToString();

        }
        catch (Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// 複製功能 - 認證資料
    /// </summary>
    /// <param name="ModelNo">來源品號</param>
    /// <param name="selItems">所選項目</param>
    /// <param name="targetModelNo">目標品號</param>
    /// <param name="ListGetIDs">回傳新增的認證編號/品號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    /// <remarks>
    ///  - 判斷主檔是否存在
    ///  - 否 -> 新增主檔 
    ///  - 是 -> 刪除明細檔 -> 新增明細檔
    /// </remarks>
    private bool CopyData(string ModelNo, List<string> selItems, List<string> targetModelNo, List<TempParam_M> ListGetIDs
        , out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[初始化]
                StringBuilder SBSql = new StringBuilder();
                cmd.Parameters.Clear();

                #region *** 1. 暫存要刪除的檔案 ***

                //[SQL] - 取得欲刪除的目標檔(查詢目標品號中,證書編號含有勾選項目的資料)
                SBSql.AppendLine(" SELECT RTRIM(Cert.Model_No) AS ModelNo, CertDTL.Cert_ID, CertDTL.Detail_ID ");
                SBSql.AppendLine("  , Cert_File, Cert_File_Report ");
                SBSql.AppendLine("  , Cert_File_CE, Cert_OrgFile_CE, Cert_File_Check, Cert_OrgFile_Check");
                SBSql.AppendLine("  , Cert_File_CE_en_US, Cert_OrgFile_CE_en_US, Cert_File_Check_en_US, Cert_OrgFile_Check_en_US ");
                SBSql.AppendLine("  , Cert_File_CE_zh_CN, Cert_OrgFile_CE_zh_CN, Cert_File_Check_zh_CN, Cert_OrgFile_Check_zh_CN ");
                SBSql.AppendLine(" FROM Prod_Certification Cert INNER JOIN Prod_Certification_Detail CertDTL ");
                SBSql.AppendLine("  ON Cert.Cert_ID = CertDTL.Cert_ID");
                SBSql.AppendLine(" WHERE (1 = 1) ");
                #region -Where條件參數-
                //[SQL] - 代入暫存參數(目標品號)
                SBSql.AppendLine(" AND RTRIM(Model_No) IN (" + GetSQLParam(targetModelNo, "ParamTmp") + ") ");
                for (int row = 0; row < targetModelNo.Count; row++)
                {
                    cmd.Parameters.AddWithValue("ParamTmp" + row, targetModelNo[row]);
                }
                //[SQL] - 代入暫存參數(所選項目)
                SBSql.AppendLine(" AND Cert_No IN (" + GetSQLParam(selItems, "ParamCertNo") + ") ");
                for (int row = 0; row < selItems.Count; row++)
                {
                    cmd.Parameters.AddWithValue("ParamCertNo" + row, selItems[row]);
                }
                #endregion
                //[SQL] - 執行
                cmd.CommandText = SBSql.ToString();

                //[暫存參數] - 要刪除的檔案
                List<TempParam> ListDelFiles = new List<TempParam>();
                //[暫存參數] - 要刪除的資料編號
                List<TempParam_IDs> ListDelIDs = new List<TempParam_IDs>();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //暫存刪除檔案
                        string filePath = Param_DiskFolder + DT.Rows[row]["ModelNo"].ToString() + "\\";
                        string fileCheck = DT.Rows[row]["Cert_File"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_Report"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_CE"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_CE_en_US"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_CE_zh_CN"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_Check"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_Check_en_US"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        fileCheck = DT.Rows[row]["Cert_File_Check_zh_CN"].ToString();
                        if (false == string.IsNullOrEmpty(fileCheck))
                            ListDelFiles.Add(new TempParam(fileCheck, filePath));

                        //暫存刪除編號
                        ListDelIDs.Add(
                            new TempParam_IDs(DT.Rows[row]["Cert_ID"].ToString(), DT.Rows[row]["Detail_ID"].ToString()));
                    }
                }
                #endregion

                #region *** 2. 更新目標資料 ***
                //[暫存參數] - 基本資料欄位
                string dtDoc_Path = "", dtSelf_Cert = "", dtSupplier_ItemNo = "", dtSupplier = "", dtRemark = "";
                //[清除參數]
                SBSql.Clear();
                cmd.Parameters.Clear();
                //[SQL] - 取得來源資料
                SBSql.AppendLine(" SELECT CertDTL.* ");
                SBSql.AppendLine("  , Cert.Cert_ID, Cert.Doc_Path, Cert.Self_Cert, Cert.Supplier_ItemNo, Cert.Supplier, Cert.Remark");
                SBSql.AppendLine(" FROM Prod_Certification Cert INNER JOIN Prod_Certification_Detail CertDTL ");
                SBSql.AppendLine("  ON Cert.Cert_ID = CertDTL.Cert_ID ");
                SBSql.AppendLine(" WHERE (RTRIM(Cert.Model_No) = @Param_ModelNo) ");
                //[SQL] - 代入暫存參數(所選項目)
                SBSql.AppendLine(" AND Cert_No IN (" + GetSQLParam(selItems, "ParamCertNo") + ") ");
                for (int i = 0; i < selItems.Count; i++)
                {
                    cmd.Parameters.AddWithValue("ParamCertNo" + i, selItems[i]);
                }
                //[SQL] - 執行
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", ModelNo);

                //[暫存參數] - 來源檔案
                List<TempParam> ListSourceFiles = new List<TempParam>();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        dtDoc_Path = DT.Rows[0]["Doc_Path"].ToString();
                        dtSelf_Cert = DT.Rows[0]["Self_Cert"].ToString();
                        dtSupplier_ItemNo = DT.Rows[0]["Supplier_ItemNo"].ToString();
                        dtSupplier = DT.Rows[0]["Supplier"].ToString();
                        dtRemark = DT.Rows[0]["Remark"].ToString();

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //暫存來源檔案
                            string filePath = Param_DiskFolder + ModelNo + "\\";
                            string fileCheck = DT.Rows[row]["Cert_File"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_Report"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_CE"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_CE_en_US"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_CE_zh_CN"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_Check"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_Check_en_US"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));

                            fileCheck = DT.Rows[row]["Cert_File_Check_zh_CN"].ToString();
                            if (false == string.IsNullOrEmpty(fileCheck))
                                ListSourceFiles.Add(new TempParam(fileCheck, filePath));
                        }

                        /*
                         * 新增目標品號不存在的主檔
                         *  - 資料來源:產品主檔
                         * 刪除原有的明細檔
                         *  - 資料來源:要刪除的資料暫存編號
                         * 新增明細檔
                         *  - 資料來源:目標主檔(取得主檔ID) & 來源明細檔(取得明細資料)
                         */
                        #region - 判斷&新增目標主檔 -
                        SBSql.Clear();
                        cmd.Parameters.Clear();
                        SBSql.AppendLine(" Declare @New_ID AS INT ");
                        //目標型號Loop
                        for (int i = 0; i < targetModelNo.Count; i++)
                        {
                            //-判斷是否存在
                            SBSql.AppendLine(string.Format(
                                "IF (SELECT COUNT(*) FROM Prod_Certification WHERE (Model_No = '{0}')) = 0 "
                                , targetModelNo[i].ToString()));
                            //-開始新增資料
                            SBSql.AppendLine(" BEGIN ");
                            SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Cert_ID), 0) + 1 FROM Prod_Certification) ");
                            SBSql.AppendLine(" INSERT INTO Prod_Certification( ");
                            SBSql.AppendLine("  Cert_ID, Model_No, Create_Who, Create_Time");
                            SBSql.AppendLine("  ,  Doc_Path, Self_Cert, Supplier_ItemNo, Supplier, Remark");
                            SBSql.AppendLine(" ) ");
                            SBSql.AppendLine(string.Format(
                                " SELECT @New_ID, '{0}', @Param_CreateWho, GETDATE() " +
                                "  , '{1}', '{2}', '{3}', '{4}', '{5}' " +
                                " FROM Prod_Item WHERE (Model_No = '{0}'); "
                                , targetModelNo[i].ToString()
                                , dtDoc_Path, dtSelf_Cert, dtSupplier_ItemNo, dtSupplier, dtRemark
                                ));
                            SBSql.AppendLine(" END ");
                        }
                        //[SQL] - 執行
                        cmd.CommandText = SBSql.ToString();
                        cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
                        if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                        {
                            ErrMsg = "新增目標資料(主檔)失敗," + ErrMsg;
                            return false;
                        }
                        #endregion

                        #region - 刪除原有的明細檔 -
                        if (ListDelIDs.Count > 0)
                        {
                            SBSql.Clear();
                            cmd.Parameters.Clear();

                            for (int i = 0; i < ListDelIDs.Count; i++)
                            {
                                //認證明細
                                SBSql.AppendLine(
                                    string.Format(" DELETE FROM Prod_Certification_Detail WHERE (Cert_ID = {0}) AND (Detail_ID = {1}); "
                                    , ListDelIDs[i].Param_CertID
                                    , ListDelIDs[i].Param_CertDTLID));
                                //認證符號
                                SBSql.AppendLine(
                                   string.Format(" DELETE FROM Icon_Rel_Certification WHERE (Cert_ID = {0}) AND (Detail_ID = {1}); "
                                   , ListDelIDs[i].Param_CertID
                                   , ListDelIDs[i].Param_CertDTLID));

                            }
                            //[SQL] - 執行
                            cmd.CommandText = SBSql.ToString();
                            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                            {
                                ErrMsg = "清除目標資料(明細檔)失敗," + ErrMsg;
                                return false;
                            }
                        }
                        #endregion

                        #region - 新增明細檔 -
                        //清除參數
                        SBSql.Clear();
                        cmd.Parameters.Clear();

                        //先取得目標主檔編號,暫存主檔編號
                        SBSql.AppendLine(" SELECT Cert_ID, RTRIM(Model_No) AS Model_No FROM Prod_Certification ");
                        //[SQL] - 代入暫存參數(目標品號)
                        SBSql.AppendLine(" WHERE RTRIM(Model_No) IN (" + GetSQLParam(targetModelNo, "ParamTmp") + ") ");
                        for (int row = 0; row < targetModelNo.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("ParamTmp" + row, targetModelNo[row]);
                        }
                        cmd.CommandText = SBSql.ToString();
                        using (DataTable DT_ID = dbConClass.LookupDT(cmd, out ErrMsg))
                        {
                            //暫存參數 - (來源)主檔資料的編號/品號
                            for (int ids = 0; ids < DT_ID.Rows.Count; ids++)
                            {
                                ListGetIDs.Add(new TempParam_M(
                                    DT_ID.Rows[ids]["Cert_ID"].ToString()
                                    , DT_ID.Rows[ids]["Model_No"].ToString()));
                            }
                        }
                        if (ListGetIDs.Count > 0)
                        {
                            for (int i = 0; i < ListGetIDs.Count; i++)
                            {
                                //清除參數
                                SBSql.Clear();
                                cmd.Parameters.Clear();
                                SBSql.AppendLine(" Declare @New_ID AS INT ");

                                //來源資料Loop
                                int dataIdx = 0;
                                for (int row = 0; row < DT.Rows.Count; row++)
                                {
                                    dataIdx += 1;
                                    //認證明細
                                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Detail_ID), 0) + " + dataIdx + " FROM Prod_Certification_Detail WHERE (Cert_ID = @Param_CertID)) ");
                                    SBSql.AppendLine(" INSERT INTO Prod_Certification_Detail( ");
                                    SBSql.AppendLine("  Cert_ID, Detail_ID");
                                    SBSql.AppendLine("  , Cert_Type, Cert_TypeText, IsCE, Cert_No, Cert_Cmd, Cert_Norm, IsWebDW");
                                    SBSql.AppendLine("  , Cert_ApproveDate, Cert_ValidDate, Cert_Desc1, Cert_Desc2");
                                    SBSql.AppendLine("  , Cert_File, Cert_OrgFile, Cert_File_Report, Cert_OrgFile_Report");
                                    SBSql.AppendLine("  , Cert_File_CE, Cert_OrgFile_CE, Cert_File_Check, Cert_OrgFile_Check");
                                    SBSql.AppendLine("  , Cert_File_CE_en_US, Cert_OrgFile_CE_en_US, Cert_File_Check_en_US, Cert_OrgFile_Check_en_US ");
                                    SBSql.AppendLine("  , Cert_File_CE_zh_CN, Cert_OrgFile_CE_zh_CN, Cert_File_Check_zh_CN, Cert_OrgFile_Check_zh_CN ");
                                    SBSql.AppendLine("  , Create_Who, Create_Time");
                                    SBSql.AppendLine(" ) VALUES ( ");
                                    SBSql.AppendLine("@Param_CertID, @New_ID");
                                    SBSql.AppendLine(string.Format(", @Param_CertType{0}, @Param_CertTypeText{0}, @Param_IsCE{0}, @Param_CertNo{0}, @Param_CertCmd{0}, @Param_CertNorm{0}, @IsWebDW{0}", row));
                                    SBSql.AppendLine(string.Format(", @Param_CertApproveDate{0}, @Param_CertValidDate{0}, @Param_CertDesc1{0}, @Param_CertDesc2{0}", row));
                                    SBSql.AppendLine(string.Format(", @Param_CertFile{0}, @Param_CertFileFullName{0}, @Param_FileTestReport{0}, @Param_FullNameTestReport{0}", row));
                                    SBSql.AppendLine(string.Format(", @Param_FileCE{0}, @Param_FullNameCE{0}, @Param_FileCheck{0}, @Param_FullNameCheck{0}", row));
                                    SBSql.AppendLine(string.Format(", @Param_FileCE_enUS{0}, @Param_FullNameCE_enUS{0}, @Param_FileCheck_enUS{0}, @Param_FullNameCheck_enUS{0}", row));
                                    SBSql.AppendLine(string.Format(", @Param_FileCE_zhCN{0}, @Param_FullNameCE_zhCN{0}, @Param_FileCheck_zhCN{0}, @Param_FullNameCheck_zhCN{0}", row));
                                    SBSql.AppendLine("  , @Param_CreateWho, GETDATE() ");
                                    SBSql.AppendLine(" ); ");
                                    cmd.Parameters.AddWithValue("Param_CertType" + row, DT.Rows[row]["Cert_Type"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertTypeText" + row, DT.Rows[row]["Cert_TypeText"].ToString());
                                    cmd.Parameters.AddWithValue("Param_IsCE" + row, DT.Rows[row]["IsCE"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertNo" + row, DT.Rows[row]["Cert_No"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertCmd" + row, DT.Rows[row]["Cert_Cmd"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertNorm" + row, DT.Rows[row]["Cert_Norm"].ToString());
                                    cmd.Parameters.AddWithValue("IsWebDW" + row, DT.Rows[row]["IsWebDW"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertApproveDate" + row, DT.Rows[row]["Cert_ApproveDate"].ToString().ToDateString("yyyy/MM/dd"));
                                    cmd.Parameters.AddWithValue("Param_CertValidDate" + row, DT.Rows[row]["Cert_ValidDate"].ToString() == "" ? DBNull.Value : (object)DT.Rows[row]["Cert_ValidDate"].ToString().ToDateString("yyyy/MM/dd"));
                                    cmd.Parameters.AddWithValue("Param_CertDesc1" + row, DT.Rows[row]["Cert_Desc1"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertDesc2" + row, DT.Rows[row]["Cert_Desc2"].ToString());

                                    #region --檔案--
                                    cmd.Parameters.AddWithValue("Param_CertFile" + row, DT.Rows[row]["Cert_File"].ToString());
                                    cmd.Parameters.AddWithValue("Param_CertFileFullName" + row, DT.Rows[row]["Cert_OrgFile"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileTestReport" + row, DT.Rows[row]["Cert_File_Report"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameTestReport" + row, DT.Rows[row]["Cert_OrgFile_Report"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileCE" + row, DT.Rows[row]["Cert_File_CE"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameCE" + row, DT.Rows[row]["Cert_OrgFile_CE"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileCheck" + row, DT.Rows[row]["Cert_File_Check"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameCheck" + row, DT.Rows[row]["Cert_OrgFile_Check"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileCE_enUS" + row, DT.Rows[row]["Cert_File_CE_en_US"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameCE_enUS" + row, DT.Rows[row]["Cert_OrgFile_CE_en_US"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileCheck_enUS" + row, DT.Rows[row]["Cert_File_Check_en_US"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameCheck_enUS" + row, DT.Rows[row]["Cert_OrgFile_Check_en_US"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileCE_zhCN" + row, DT.Rows[row]["Cert_File_CE_zh_CN"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameCE_zhCN" + row, DT.Rows[row]["Cert_OrgFile_CE_zh_CN"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FileCheck_zhCN" + row, DT.Rows[row]["Cert_File_Check_zh_CN"].ToString());
                                    cmd.Parameters.AddWithValue("Param_FullNameCheck_zhCN" + row, DT.Rows[row]["Cert_OrgFile_Check_zh_CN"].ToString());
                                    #endregion

                                    //認證符號
                                    SBSql.AppendLine(" INSERT INTO Icon_Rel_Certification(Pic_ID, Cert_ID, Detail_ID) ");
                                    SBSql.AppendLine(" SELECT Pic_ID, @Param_CertID, @New_ID FROM Icon_Rel_Certification ");
                                    SBSql.AppendLine(string.Format("WHERE (Cert_ID = @Src_CertID{0}) AND (Detail_ID = @Src_DtlID{0}) ", row));
                                    cmd.Parameters.AddWithValue("Src_CertID" + row, DT.Rows[row]["Cert_ID"].ToString());
                                    cmd.Parameters.AddWithValue("Src_DtlID" + row, DT.Rows[row]["Detail_ID"].ToString());
                                }

                                //[SQL] - Command
                                cmd.CommandText = SBSql.ToString();
                                cmd.Parameters.AddWithValue("Param_CertID", ListGetIDs[i].Param_CertID);
                                cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
                                //[SQL] - 執行
                                dbConClass.ExecuteSql(cmd, out ErrMsg);
                            }
                        }
                        #endregion
                    }
                }

                #endregion

                #region *** 3. 刪除目標檔案 ***
                for (int row = 0; row < ListDelFiles.Count; row++)
                {
                    IOManage.DelFile(ListDelFiles[row].Param_FilePath, ListDelFiles[row].Param_FileName);
                }
                #endregion

                #region *** 4. 複製來源檔案 ***
                for (int row = 0; row < ListSourceFiles.Count; row++)
                {
                    //來源檔案名稱
                    string filename = ListSourceFiles[row].Param_FileName;
                    //取得完整檔案路徑
                    string FullFileName = System.IO.Path.Combine(ListSourceFiles[row].Param_FilePath, filename);

                    //目標品號Loop
                    for (int i = 0; i < targetModelNo.Count; i++)
                    {
                        string targetPath = Param_DiskFolder + targetModelNo[i].ToString() + "\\";
                        string targetFile = System.IO.Path.Combine(targetPath, filename);
                        //判斷目標資料夾是否存在
                        if (!System.IO.Directory.Exists(targetPath))
                        {
                            System.IO.Directory.CreateDirectory(targetPath);
                        }
                        //複製檔案
                        System.IO.File.Copy(FullFileName, targetFile, true);
                    }
                }
                #endregion

                //寫入Log
                fn_Log.Log_Rec("認證資料"
                    , ModelNo
                    , "複製認證資料,來源品號:{0}, 目標品號:{1}".FormatThis(ModelNo, string.Join(",", targetModelNo))
                    , fn_Param.CurrentAccount.ToString());
            }

            return true;
        }

        catch (Exception ex)
        {
            ErrMsg = "複製發生錯誤，錯誤描述：" + ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 設定檔案下載連結
    /// </summary>
    /// <param name="orgName">原始檔名</param>
    /// <param name="fileName">真實檔名</param>
    /// <returns>string</returns>
    private string setFileUri(string orgName, string fileName, string _modelNo)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }
        else
        {
            string uri = string.Format("../FileDownload.ashx?OrgiName={0}&FilePath={1}"
                , Server.UrlEncode(orgName.Trim())
                , Server.UrlEncode(Cryptograph.Encrypt(Param_DiskFolder + _modelNo + @"\" + fileName)));

            return "<a href=\"" + uri + "\">下載</a>";
        }
    }

    /// <summary>
    /// SQL參數組合 - Where IN
    /// </summary>
    /// <param name="listSrc">來源資料(List)</param>
    /// <param name="paramName">參數名稱</param>
    /// <returns>參數字串</returns>
    string GetSQLParam(List<string> listSrc, string paramName)
    {
        if (listSrc.Count == 0)
        {
            return "";
        }

        //組合參數字串
        ArrayList aryParam = new ArrayList();
        for (int row = 0; row < listSrc.Count; row++)
        {
            aryParam.Add(string.Format("@{0}{1}", paramName, row));
        }
        //回傳以 , 為分隔符號的字串
        return string.Join(",", aryParam.ToArray());
    }

    #endregion

    #region ***參數設定***
    /// <summary>
    /// [參數] - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return Application["File_WebUrl"] + @"Certification/";
        }
        set
        {
            this._Param_WebFolder = value;
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
            return Application["File_DiskUrl"] + @"Certification\";
        }
        set
        {
            this._Param_DiskFolder = value;
        }
    }
    #endregion

    #region ***暫存參數***
    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 檔名
        /// </summary>
        private string _Param_FileName;
        public string Param_FileName
        {
            get { return this._Param_FileName; }
            set { this._Param_FileName = value; }
        }

        /// <summary>
        /// [參數] - 路徑
        /// </summary>
        private string _Param_FilePath;
        public string Param_FilePath
        {
            get { return this._Param_FilePath; }
            set { this._Param_FilePath = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_FileName">檔名</param>
        /// <param name="Param_FilePath">路徑</param>
        public TempParam(string Param_FileName, string Param_FilePath)
        {
            this._Param_FileName = Param_FileName;
            this._Param_FilePath = Param_FilePath;
        }
    }

    /// <summary>
    /// 暫存參數 - 要刪除的明細資料
    /// </summary>
    public class TempParam_IDs
    {
        /// <summary>
        /// [參數] - 認證編號
        /// </summary>
        private string _Param_CertID;
        public string Param_CertID
        {
            get { return this._Param_CertID; }
            set { this._Param_CertID = value; }
        }

        /// <summary>
        /// [參數] - 認證明細編號
        /// </summary>
        private string _Param_CertDTLID;
        public string Param_CertDTLID
        {
            get { return this._Param_CertDTLID; }
            set { this._Param_CertDTLID = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_CertID">認證編號</param>
        /// <param name="Param_CertDTLID">認證明細編號</param>
        public TempParam_IDs(string Param_CertID, string Param_CertDTLID)
        {
            this._Param_CertID = Param_CertID;
            this._Param_CertDTLID = Param_CertDTLID;
        }
    }

    /// <summary>
    /// 暫存參數 - 主檔資料的編號/品號
    /// </summary>
    public class TempParam_M
    {
        /// <summary>
        /// [參數] - 認證編號
        /// </summary>
        private string _Param_CertID;
        public string Param_CertID
        {
            get { return this._Param_CertID; }
            set { this._Param_CertID = value; }
        }

        /// <summary>
        /// [參數] - 品號
        /// </summary>
        private string _Param_ModelNo;
        public string Param_ModelNo
        {
            get { return this._Param_ModelNo; }
            set { this._Param_ModelNo = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_CertID">認證編號</param>
        /// <param name="Param_ModelNo">品號</param>
        public TempParam_M(string Param_CertID, string Param_ModelNo)
        {
            this._Param_CertID = Param_CertID;
            this._Param_ModelNo = Param_ModelNo;
        }
    }
    #endregion

}
