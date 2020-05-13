using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using MailMethods;


/// <summary>
/// 沿用舊版格式-20180418
/// </summary>
public partial class Prod_News_Send : SecurityIn
{
    public string _fileFolder = System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[宣告]
                string ErrMsg;

                //[權限判斷] - 發信
                if (fn_CheckAuth.CheckAuth_User("133", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("權限不足", "");
                    return;
                }
                //[參數判斷] - 編號
                if (string.IsNullOrEmpty(Param_thisID))
                {
                    fn_Extensions.JsAlert("資料已不存在或是未正確使用本功能", "");
                    return;
                }

                //[宣告] - 存放路徑
                string myPath = @"{0}{2}\Prod_News\{1}\".FormatThis(Application["File_DiskUrl"].ToString(), Param_thisID, _fileFolder);
                string myFileName = "News.html";

                //產生Html
                string ProcMsg;
                if (false == Create_Html(myPath, myFileName, out ProcMsg))
                {
                    fn_Extensions.JsAlert("產生Html失敗", "");
                    return;
                }

                //讀取 & 顯示Html
                this.lt_Container.Text = fn_Extensions.IORequest_GET(myPath + myFileName);

            }
            catch (Exception)
            {

                throw;
            }
        }

    }

    #region -- 資料讀取 --
    /// <summary>
    /// 產生Html
    /// </summary>
    /// <param name="myPath"></param>
    /// <param name="myFileName"></param>
    /// <param name="ProcMsg"></param>
    /// <returns></returns>
    private bool Create_Html(string myPath, string myFileName, out string ProcMsg)
    {
        #region -- 取得內容 --
        string ErrMsg;

        //取得Html內容
        StringBuilder html = new StringBuilder();

        //[取得資料] - 單頭資料
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT Main.* ");
            SBSql.AppendLine(" , (");
            SBSql.AppendLine("  CASE Main.Lang ");
            SBSql.AppendLine("      WHEN 'zh-TW' THEN Cls.ClassName_zh_TW");
            SBSql.AppendLine("      WHEN 'zh-CN' THEN Cls.ClassName_zh_CN");
            SBSql.AppendLine("      WHEN 'en-US' THEN Cls.ClassName_en_US");
            SBSql.AppendLine("  END");
            SBSql.AppendLine(" ) AS ClassName");
            SBSql.AppendLine(" FROM Prod_News AS Main ");
            SBSql.AppendLine("      INNER JOIN Prod_News_Class Cls ON Main.ClassID = Cls.ClassID");
            SBSql.AppendLine(" WHERE (Main.NewsID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    ProcMsg = "查無資料";
                    return false;
                }
                else
                {
                    html.Append("Dear All<br/>");

                    //大類/小類/發送對象/品號
                    string MainClass = DT.Rows[0]["ClassName"].ToString();
                    string myTarget = LookupTarget();
                    string myModels = Lookup_Models();


                    html.Append("<div>{0}, {1}</div>".FormatThis(MainClass, myTarget));
                    html.Append("<div>{0}<br/><br/></div>".FormatThis(myModels));

                    //主旨
                    string Subject = DT.Rows[0]["Subject"].ToString();
                    html.Append("<div style=\"padding-top:10px;\">");
                    html.Append("    <span style=\"font-size:14px;line-height:25px;font-weight:bold\">【主旨】</span>");
                    html.Append("    <span style=\"padding-left:5px;\">{0}</span>".FormatThis(Subject));
                    html.Append("</div>");

                    //時間點
                    html.Append("<div style=\"padding-top:10px;\">");
                    html.Append("    <span style=\"font-size:14px;line-height:25px;font-weight:bold\">【時間點】</span>");
                    html.Append("    <span style=\"padding-left:5px;\">{0}</span>".FormatThis(
                        (DT.Rows[0]["TimingType"].ToString().Equals("1")) ?
                            DT.Rows[0]["TimingDate"].ToString().ToDateString("yyyy-MM-dd") :
                            GetTimingType(DT.Rows[0]["TimingType"].ToString())
                        ));
                    html.Append("</div>");

                    //替代品號
                    string mySubModels = Lookup_SubModels();
                    if (!string.IsNullOrEmpty(mySubModels))
                    {
                        html.Append("<div style=\"padding-top:10px;\">");
                        html.Append("    <span style=\"font-size:14px;line-height:25px;font-weight:bold\">【替代品號】</span>");
                        html.Append("    <span style=\"padding-left:5px;\">{0}</span>".FormatThis(mySubModels));
                        html.Append("</div>");
                    }

                    //附圖
                    string Attachment = LookupAttachment();
                    if (!string.IsNullOrEmpty(Attachment))
                    {
                        html.Append("<div style=\"padding-top:10px;\">");
                        html.Append("    <span style=\"font-size:14px;line-height:25px;font-weight:bold\">【附圖】</span><br/>");
                        html.Append(Attachment);
                        html.Append("</div>");
                    }

                    //內文
                    string Desc1 = DT.Rows[0]["Desc1"].ToString();
                    if (!string.IsNullOrEmpty(Desc1))
                    {
                        html.Append("<div style=\"padding-top:10px;\">");
                        html.Append("    <span style=\"font-size:14px;line-height:25px;font-weight:bold\">【內文】</span>");
                        html.Append("    <div style=\"padding-left:5px;\">{0}</div>".FormatThis(HttpUtility.HtmlDecode(Desc1)));
                        html.Append("</div>");
                    }

                    //配合事項
                    string Desc2 = DT.Rows[0]["Desc2"].ToString();
                    if (!string.IsNullOrEmpty(Desc2))
                    {
                        html.Append("<div style=\"padding-top: 10px;\">");
                        html.Append("    <span style=\"font-size:14px;line-height:25px;font-weight:bold\">【配合事項】</span>");
                        html.AppendLine("<ol>");
                        string[] readlines = Regex.Split(DT.Rows[0]["Desc2"].ToString(), Environment.NewLine);
                        for (int row = 0; row < readlines.Length; row++)
                        {
                            if (!string.IsNullOrEmpty(readlines[row]))
                            {
                                html.AppendLine("<li>{0}</li>".FormatThis(readlines[row]));
                            }
                        }
                        html.AppendLine("</ol>");
                        html.Append("</div>");
                    }

                    //Url//http://localhost/ProductCenter/myProdNews/View.aspx
                    html.Append("<div style=\"padding-top: 10px;\">{0}</div>".FormatThis(
                         "詳細產銷訊息請見產品中心：<a href=\"{0}\" target=\"_blank\">{1}</a>".FormatThis(
                            "{0}myProdNews/View.aspx?&DataID={1}".FormatThis(
                                    Application["WebUrl"].ToString()
                                    , Param_thisID
                                )
                            , Subject
                         )
                        ));

                    //暫存郵件主旨 (大類 /對象/ 小類)
                    //this.ViewState["MailSubject"] = "[產品訊息] {0}:{1}, {2}".FormatThis(MainClass, myTarget, SubClass);
                    this.ViewState["MailSubject"] = "[產品訊息] {0}".FormatThis(Subject);

                }
            }
        }

        #endregion

        //[宣告] - Html 模版
        string myTemplate = @"{0}\Prod_News_Template.html".FormatThis(Application["File_DiskUrl"].ToString());

        return fn_Extensions.Generate_Html(myPath, myFileName, myTemplate, html, out ProcMsg);
    }

    /// <summary>
    /// 明細資料 - 發送對象
    /// </summary>
    /// <returns></returns>
    private string LookupTarget()
    {
        //[取得資料] - 發送對象
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT Para.Param_Value AS TarName ");
            SBSql.AppendLine(" FROM Prod_News_Target Tar ");
            SBSql.AppendLine("  INNER JOIN Param_Public Para ON Tar.TargetID = Para.Param_Name ");
            SBSql.AppendLine(" WHERE (Para.Param_Kind = '產品訊息發送對象') AND (NewsID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                List<string> listName = new List<string>();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    listName.Add(DT.Rows[row]["TarName"].ToString());
                }

                return string.Join("/", listName.ToArray());
            }
        }

    }

    /// <summary>
    /// 明細資料 - 品號
    /// </summary>
    private string Lookup_Models()
    {
        //[取得資料] - 品號
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT Model_No ");
            SBSql.AppendLine(" FROM Prod_News_Models ");
            SBSql.AppendLine(" WHERE (NewsID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                List<string> listName = new List<string>();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    listName.Add(DT.Rows[row]["Model_No"].ToString().Trim());
                }

                return string.Join(" / ", listName.ToArray());

            }
        }

    }

    /// <summary>
    /// 明細資料 - 替代品號
    /// </summary>
    private string Lookup_SubModels()
    {
        //[取得資料] - 品號
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine(" SELECT Model_No ");
            SBSql.AppendLine(" FROM Prod_News_SubModels ");
            SBSql.AppendLine(" WHERE (NewsID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                List<string> listName = new List<string>();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    listName.Add(DT.Rows[row]["Model_No"].ToString().Trim());
                }

                return string.Join(" / ", listName.ToArray());

            }
        }

    }

    /// <summary>
    /// 查詢HTML附圖
    /// </summary>
    /// <returns></returns>
    private string LookupAttachment()
    {
        //[取得資料] - HTML附圖
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();
            StringBuilder html = new StringBuilder();

            SBSql.AppendLine(" SELECT AttOrgName, AttName, AttDesc ");
            SBSql.AppendLine(" FROM Prod_News_Attachment ");
            SBSql.AppendLine(" WHERE (NewsID = @DataID) AND (AttType = 3) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                html.Append("<table width=\"95%\" border=\"1\" style=\"padding-left:10px;border-collapse: collapse;\">");
                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    html.Append(" <tr>");
                    html.Append("  <td align=\"center\" width=\"70%\" style=\"text-align:center;\">{0}</td>".FormatThis(
                        GetImgUrl(DT.Rows[row]["AttOrgName"].ToString(), DT.Rows[row]["AttName"].ToString())
                        ));
                    html.Append("  <td valign=\"top\" width=\"30%\">{0}</td>".FormatThis(
                        DT.Rows[row]["AttDesc"].ToString().Replace(Environment.NewLine, "<br/>")
                        ));
                    html.Append(" </tr>");
                }

                html.Append("</table>");


                return html.ToString();
            }
        }

    }

    /// <summary>
    /// 查詢寄件者 (依目前登入者查詢)
    /// </summary>
    /// <returns></returns>
    private string LookupMailSender()
    {
        //[取得資料] - 發送對象
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT Display_Name, Email ");
            SBSql.AppendLine(" FROM User_Profile ");
            SBSql.AppendLine(" WHERE (Account_Name = @Account_Name) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Account_Name", fn_Param.CurrentAccount.ToString());
            using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    //找不到資料, 使用系統寄件人
                    return System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];
                }
                else
                {
                    return DT.Rows[0]["Email"].ToString();
                }
            }
        }
    }


    /// <summary>
    /// 查詢發送對象的EmailAddress
    /// </summary>
    /// <returns></returns>
    private List<string> LookupMailList()
    {
        //[取得資料] - 發送對象的EmailAddress
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT Para.Param_Value, Para.Param_Desc ");
            SBSql.AppendLine(" FROM Prod_News_Target Tar ");
            SBSql.AppendLine("     INNER JOIN Param_Public Para ON Tar.TargetID = Para.Param_Name ");
            SBSql.AppendLine(" WHERE (Para.Param_Kind = '產品訊息發送對象') AND (Tar.NewsID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return null;
                }

                List<string> listName = new List<string>();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    string mailAddr = DT.Rows[row]["Param_Desc"].ToString();
                    if (!string.IsNullOrEmpty(mailAddr))
                    {
                        listName.Add(DT.Rows[row]["Param_Desc"].ToString());
                    }
                }

                return listName;
            }
        }
    }

    /// <summary>
    /// 查詢固定CC的EmailAddress
    /// </summary>
    /// <returns></returns>
    private List<string> LookupCCList()
    {
        //[取得資料] - 發送對象的EmailAddress
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" SELECT Para.Param_Desc ");
            SBSql.AppendLine(" FROM Param_Public Para ");
            SBSql.AppendLine(" WHERE (Para.Param_Kind = '產品訊息轉寄') AND (Para.Param_Name = 'CC') ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return null;
                }

                List<string> listName = new List<string>();
                string[] readlines = Regex.Split(DT.Rows[0]["Param_Desc"].ToString(), ";");

                for (int row = 0; row < readlines.Length; row++)
                {
                    listName.Add(readlines[row]);
                }

                return listName;
            }
        }
    }

    /// <summary>
    /// 更新狀態為已發信
    /// </summary>
    /// <returns></returns>
    private bool UpdateMailStatus()
    {
        //[取得資料] - 發送對象的EmailAddress
        using (SqlCommand cmd = new SqlCommand())
        {
            string ErrMsg;
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" UPDATE Prod_News SET IsMail = 'Y', IsClose = 'Y' ");
            SBSql.AppendLine("  , Send_Who = @Send_Who, Send_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (NewsID = @DataID) ");
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DataID", Param_thisID);
            cmd.Parameters.AddWithValue("Send_Who", fn_Param.CurrentAccount.ToString());

            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }

    #endregion

    #region -- 其他功能 --
    /// <summary>
    /// 取得圖片路徑
    /// </summary>
    /// <param name="OrgFileName"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    private string GetImgUrl(string OrgFileName, string FileName)
    {

        return "<img src=\"{0}\" alt=\"{1}\" width=\"100%\" style=\"width:100%;\">"
            .FormatThis(
                 IMG_WebFolder + FileName
                , OrgFileName);
    }

    /// <summary>
    /// 取得時間點類型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string GetTimingType(string input)
    {
        //1-日期 ; 2-庫存用完後變更
        switch (input)
        {
            case "1":
                return "開始日";

            case "2":
                return "庫存用完後變更";

            case "3":
                return "即日起";

            default:
                return "其他";
        }
    }

    protected void btn_Send_Top_Click(object sender, EventArgs e)
    {
        SendMail();
    }
    protected void btn_Send_Bottom_Click(object sender, EventArgs e)
    {
        SendMail();
    }

    /// <summary>
    /// 發送郵件
    /// </summary>
    private void SendMail()
    {
        try
        {
            //[設定參數] - 建立者
            fn_Mail.Create_Who = fn_Param.CurrentAccount.ToString();

            //[設定參數] - 來源程式/功能
            fn_Mail.FromFunc = "ProductCenter, 產品訊息發送";

            //[設定參數] - 寄件人
            fn_Mail.Sender = LookupMailSender();

            //[設定參數] - 寄件人顯示名稱
            fn_Mail.SenderName = "產品訊息";

            //[設定參數] - 收件人
            if (LookupMailList() == null)
            {
                fn_Extensions.JsAlert("收件人空白，請確認「發送對象」是否已勾選", "");
                return;
            }
            fn_Mail.Reciever = LookupMailList();

            //[設定參數] - 轉寄人群組
            fn_Mail.CC = LookupCCList();

            //[設定參數] - 密件轉寄人群組
            fn_Mail.BCC = null;

            //[設定參數] - 郵件主旨
            fn_Mail.Subject = this.ViewState["MailSubject"].ToString();

            //[設定參數] - 郵件內容
            #region 郵件內容

            string myPath = @"{0}{2}\Prod_News\{1}\".FormatThis(Application["File_DiskUrl"].ToString(), Param_thisID, _fileFolder);
            string myFileName = "News.html";

            StringBuilder MailBody = new StringBuilder();
            MailBody.Append(fn_Extensions.IORequest_GET(myPath + myFileName));

            fn_Mail.MailBody = MailBody;

            #endregion

            //[設定參數] - 指定檔案 - 路徑
            fn_Mail.FilePath = "";

            //[設定參數] - 指定檔案 - 檔名
            fn_Mail.FileName = "";

            //發送郵件
            fn_Mail.SendMail();

            //[判斷參數] - 寄件是否成功
            if (!fn_Mail.MessageCode.Equals(200))
            {
                //Response.Write(fn_Mail.Message);
                fn_Extensions.JsAlert("發送時發生了一點小問題, 錯誤代碼:{0}\\n{1}".FormatThis(fn_Mail.MessageCode, fn_Mail.Message), PageUrl);
                return;
            }

            if (UpdateMailStatus())
            {
                fn_Extensions.JsAlert("發送成功,請關閉視窗", "");
                return;
            }
            else
            {
                fn_Extensions.JsAlert("發送成功，但資料更新失敗", "");
                return;
            }

        }
        catch (Exception)
        {

            throw;
        }

    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 本筆資料的編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            return string.IsNullOrEmpty(Request.QueryString["DataID"]) ? "" : Request.QueryString["DataID"].ToString();
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// Image Web Folder
    /// </summary>
    private string _IMG_WebFolder;
    public string IMG_WebFolder
    {
        get
        {
            return this._IMG_WebFolder != null ? this._IMG_WebFolder : "{0}Prod_News/{1}/".FormatThis(
                System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"] + _fileFolder
                , Param_thisID);
        }
        set
        {
            this._IMG_WebFolder = value;
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
            return string.Format(@"Send.aspx?DataID={0}"
                , string.IsNullOrEmpty(Param_thisID) ? "" : Param_thisID);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    #endregion
}