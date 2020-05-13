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
using LogRecord;
using ExtensionIO;


public partial class ProdPic_Group : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";
            //[帶入參數] - 圖片類別
            Param_Class = Request.QueryString["C_ID"].Trim();
            //[帶入參數] - 品號
            Param_ModelNo = Request.QueryString["ModelNo"].Trim();
            //[帶入參數] - 來源參數
            Param_flag = Request.QueryString["flag"].Trim();
            //判斷是否有上一頁暫存參數
            if (Session["BackListUrl"] == null)
                Session["BackListUrl"] = Application["WebUrl"] + "ProdPic/ProdPic_Search.aspx?flag=" + Param_flag;

            //若類別 = 說明書(8), 導向至產品檔案頁
            if (Param_Class.Equals("8"))
            {
                Response.Redirect("../Product/Prod_Files_View.aspx?Model_No={0}".FormatThis(Server.UrlEncode(Param_ModelNo)));
                return;
            }

            #region -權限控制-
            //[權限判斷] - 編輯權限
            switch (Param_flag)
            {
                case "301":
                    if (fn_CheckAuth.CheckAuth_User("303", out ErrMsg) == false)
                    {
                        Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                        return;
                    }
                    break;

                case "302":
                    if (fn_CheckAuth.CheckAuth_User("304", out ErrMsg) == false)
                    {
                        Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                        return;
                    }
                    break;
            }
            //[權限判斷] - 頁簽(Param_Class-14:品保)
            if (Param_flag == "302")
            {
                //品保權限, 判斷是否於品保的頁簽
                if (Param_Class != "14")
                {
                    fn_Extensions.JsAlert("無修改權限！", Session["BackListUrl"].ToString());
                }
            }
            else
            {
                //行企權限, 判斷是否於行企的頁簽
                if (Param_Class == "14")
                {
                    fn_Extensions.JsAlert("無修改權限！", Session["BackListUrl"].ToString());
                }
            }
            #endregion

            //[ASCX 頁簽] - 加入屬性
            Ascx_ProdPicClass1.Param_CurrPage = Param_Class;
            Ascx_ProdPicClass1.Param_ModelNo = Param_ModelNo;
            Ascx_ProdPicClass1.Param_flag = Param_flag;

            //[ASCX 基本資料] - 加入屬性        
            Ascx_ProdData1.Param_C_ID = Param_Class;
            Ascx_ProdData1.Param_ModelNo = Param_ModelNo;

            if (!IsPostBack)
            {
                //[按鈕] - 加入BlockUI
                this.btn_Upload.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[判斷 & 取得參數] - C_ID 圖片類別
                if (fn_Extensions.Num_正整數(Param_Class, "1", "999999999", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 圖片類別！", Session["BackListUrl"].ToString());
                }
                //[判斷 & 取得參數] - Model_No 品號
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 品號！", Session["BackListUrl"].ToString());
                }
                //[取得/檢查參數] - flag (連結來源 - 行企or品保, 判斷是否有權限)
                if (fn_Extensions.String_字數(Request.QueryString["flag"], "1", "3", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 來源參數！", Session["BackListUrl"].ToString());
                }
                //[產生選單] - 語系
                if (fn_Extensions.LangMenu(this.ddl_Lang, out ErrMsg) == false)
                {
                    this.ddl_Lang.Items.Clear();
                }
                //[判斷參數] - 圖片類別, 類別=3(DM)時, 顯示語言別
                switch (Param_Class)
                {
                    case "3":
                    case "99":
                        ph_Lang.Visible = true;
                        break;

                    default:
                        ph_Lang.Visible = false;
                        break;
                }

                //帶出圖片列表
                LookupDataList();

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", Session["BackListUrl"].ToString());
        }
    }


    #region "明細資料"
    /// <summary>
    /// 副程式 - 取得明細資料列表
    /// </summary>
    private void LookupDataList()
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
                SBSql.AppendLine("      Pic_ID, Lang, Pic_OrgFile, Pic_File, Pic_Desc, Sort ");
                SBSql.AppendLine("      , (CASE WHEN Update_Time IS NULL THEN Create_Time ELSE Update_Time END) AS LastTime ");
                SBSql.AppendLine("      , Create_Who, Create_Time, Update_Who, Update_Time ");
                SBSql.AppendLine("  FROM ProdPic_Group ");
                SBSql.AppendLine(" WHERE (Pic_Class= @Param_Class) AND (RTRIM(Model_No)= @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY Sort ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Class", Param_Class);
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);

                //[SQL] - 取得資料
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
                        //[按鈕] - 全部刪除
                        this.btn_DelAll.Visible = true;

                        //[判斷參數] - 圖片類別, 控制標頭
                        System.Web.UI.HtmlControls.HtmlTableCell tdHeadLang = (System.Web.UI.HtmlControls.HtmlTableCell)this.lvDataList.FindControl("tdHeadLang");
                        switch (Param_Class)
                        {
                            case "3":
                            case "99":
                                tdHeadLang.Visible = true;
                                break;

                            default:
                                tdHeadLang.Visible = false;
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 明細資料！", "");
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            string ErrMsg;
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //[判斷參數] - 圖片類別, 控制列表欄位
            PlaceHolder ph_Txt = (PlaceHolder)e.Item.FindControl("ph_Txt");
            System.Web.UI.HtmlControls.HtmlTableCell td_Lang = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("td_Lang");
            switch (Param_Class)
            {
                case "3":
                case "99":
                    //類別=3(DM), 顯示語言別
                    td_Lang.Visible = true;
                    ph_Txt.Visible = false;
                    //語系別
                    ((Literal)e.Item.FindControl("lt_Lang")).Text =
                        fn_Extensions.GetLangValue(DataBinder.Eval(dataItem.DataItem, "Lang").ToString(), out ErrMsg);
                    break;

                case "14":
                    //類別=14(品保), 顯示文字欄
                    ph_Txt.Visible = true;
                    td_Lang.Visible = false;
                    break;

                default:
                    td_Lang.Visible = false;
                    ph_Txt.Visible = false;
                    break;
            }
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                cmd.Parameters.Clear();

                //[取得參數] - 編號
                string GetDataID = ((Literal)e.Item.FindControl("lt_PicID")).Text;

                //[SQL] - 取得已上傳的檔案
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Pic_File ");
                SBSql.AppendLine(" FROM ProdPic_Group WHERE (Pic_ID = @Param_ID) AND (RTRIM(Model_No) = @Param_ModelNo) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
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
                SBSql.AppendLine(" DELETE FROM ProdPic_Group WHERE (Pic_Class = @Pic_Class) AND (Pic_ID = @Param_ID) AND (RTRIM(Model_No) = @Param_ModelNo); ");
                //[SQL] - 刪除關聯
                SBSql.AppendLine(" DELETE FROM ProdPic_Rel WHERE (Pic_Class = @Pic_Class) AND (Pic_Column = @Param_ID) AND (Model_No = @Param_ModelNo)");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ID", GetDataID);
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料刪除失敗！", "");
                }
                else
                {
                    //寫入Log
                    fn_Log.Log_Rec("圖片資料庫"
                        , Param_ModelNo
                        , "刪除圖片,品號:{0}, 圖片類別:{1}, 編號:{2}".FormatThis(Param_ModelNo, Param_Class, GetDataID)
                        , fn_Param.CurrentAccount.ToString());

                    //刪除檔案
                    for (int idx = 0; idx < ListFiles.Count; idx++)
                    {
                        IOManage.DelFile(Param_FileFolder, ListFiles[idx]);
                    }

                    //頁面跳至明細
                    fn_Extensions.JsAlert("資料刪除成功！", PageUrl);
                }

            }
        }
    }
    #endregion

    #region "按鈕"
    /// <summary>
    /// 上傳圖片
    /// </summary>
    protected void btn_Upload_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

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

                //[SQL] - Statement
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" Declare @New_ID AS INT ");

                for (int i = 0; i < ITempList.Count; i++)
                {
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Group WHERE (RTRIM(Model_No) = @Param_ModelNo)) ");
                    SBSql.AppendLine(" INSERT INTO ProdPic_Group( ");
                    SBSql.AppendLine("  Pic_ID, Pic_Class, Model_No, Lang");
                    SBSql.AppendLine("  , Pic_OrgFile, Pic_File");
                    SBSql.AppendLine("  , Create_Who, Create_Time");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Param_Class, @Param_ModelNo, @Param_Lang");
                    SBSql.AppendLine(string.Format("  , @FileFullName_{0} , @FileNewName_{0} ", i));
                    SBSql.AppendLine("  , @Param_CreateWho, GETDATE() ");
                    SBSql.AppendLine(" ) ");
                    cmd.Parameters.AddWithValue("FileFullName_" + i, ITempList[i].Param_OrgPic);
                    cmd.Parameters.AddWithValue("FileNewName_" + i, ITempList[i].Param_Pic);
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_Class", Param_Class);
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_Lang", this.ddl_Lang.SelectedValue);
                cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料新增失敗！", "");
                }
                else
                {
                    //[IO] - 儲存檔案
                    for (int i = 0; i < ITempList.Count; i++)
                    {
                        HttpPostedFile hpf = ITempList[i].Param_hpf;
                        if (hpf.ContentLength > 0)
                        {
                            string newFileName = ITempList[i].Param_Pic;

                            //原始圖
                            IOManage.Save(hpf, Param_FileFolder, newFileName, Param_Width, Param_Height);

                            //儲存成縮圖
                            //IOManage.Save(hpf, Param_FileFolder, "thumb_" + newFileName, 400, 400);
                        }
                    }

                    //寫入Log
                    fn_Log.Log_Rec("圖片資料庫"
                        , Param_ModelNo
                        , "上傳圖片,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                        , fn_Param.CurrentAccount.ToString());

                    fn_Extensions.JsAlert("資料新增成功！", PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 上傳！", "");
        }
    }

    /// <summary>
    /// 儲存排序
    /// </summary>
    protected void btn_SaveSort_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

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
                    //[取得參數] - 說明
                    string lvParam_Desc = ((TextBox)this.lvDataList.Items[i].FindControl("tb_PicDesc")).Text.Trim();
                    //[取得參數] - 排序
                    string lvParam_Sort = ((TextBox)this.lvDataList.Items[i].FindControl("tb_Sort")).Text.Trim();

                    SBSql.AppendLine(" UPDATE ProdPic_Group SET ");
                    SBSql.AppendLine(" Update_Who = @Param_UpdateWho, Update_Time = GETDATE()");
                    SBSql.AppendLine(string.Format(
                        ", Pic_Desc = @lvParam_PicDesc_{0}, Sort = @lvParam_Sort_{0} " +
                        " WHERE (Pic_ID = @lvParam_ID_{0}) AND (RTRIM(Model_No) = @Param_ModelNo); "
                        , i));

                    cmd.Parameters.AddWithValue("lvParam_ID_" + i, lvParam_ID);
                    cmd.Parameters.AddWithValue("lvParam_PicDesc_" + i, lvParam_Desc);
                    cmd.Parameters.AddWithValue("lvParam_Sort_" + i, lvParam_Sort);
                }
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("資料儲存失敗！", "");
                }
                else
                {
                    //寫入Log
                    fn_Log.Log_Rec("圖片資料庫"
                        , Param_ModelNo
                        , "設定排序,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                        , fn_Param.CurrentAccount.ToString());

                    fn_Extensions.JsAlert("資料儲存成功！", PageUrl);
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
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 刪除資料
                SBSql.AppendLine(" DELETE FROM ProdPic_Group ");
                SBSql.AppendLine(" WHERE (RTRIM(Model_No) = @Param_ModelNo) AND (Pic_Class = @Param_Class) ");

                //[SQL] - 刪除關聯性
                SBSql.AppendLine(" DELETE FROM ProdPic_Rel ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) AND (Pic_Class = @Param_Class) ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Param_Class", Param_Class);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("刪除失敗！", "");
                }
                else
                {
                    //寫入Log
                    fn_Log.Log_Rec("圖片資料庫"
                        , Param_ModelNo
                        , "刪除全部圖片,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                        , fn_Param.CurrentAccount.ToString());

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

    #endregion

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
                "<td class=\"L2Img\" width=\"120px\"> " +
                "<a class=\"PicGroup L2Img\" rel=\"PicGroup\" href=\"{0}\" title=\"{1}\"><img src=\"{0}\" border=\"0\" style=\"width:120px\"></a>" +
                "</td>"
                , Param_WebFolder + Param_ModelNo + "/" + Param_Class + "/" + PicName
                , OrgPicName);
        }
        else
        {
            //非圖片，顯示下載連結
            preView = string.Format(
                "<td class=\"L2Info\" width=\"200px\"> " +
                "<a href=\"../FileDownload.ashx?OrgiName={2}&FilePath={1}\">{0}</a>" +
                "</td>"
                , OrgPicName
                , Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + PicName))
                , Server.UrlEncode(OrgPicName)
                );
        }

        //輸出Html
        return preView;
    }

    /// <summary>
    /// 取得外部圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <returns>string</returns>
    public string DealerPicUrl(string PicFile)
    {
        return "<span class=\"styleBluelight\">路徑：</span><input type=\"text\" value=\"{0}\" style=\"width: 280px; cursor: pointer;color: #555\" readonly=\"readonly\" />"
           .FormatThis(
               "{0}ProductPic/{1}"
               .FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["ref_WebUrl"]
                   , Param_ModelNo + "/" + Param_Class + "/" + PicFile
               )
           );
    }


    /// <summary>
    /// 取得維護網址
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    public string Get_MaintainUrl(string col)
    {
        return "{0}ProdPic/myPic_Maintain.aspx?ModelNo={1}&Cls={2}&Col={3}&rt={4}".FormatThis(
                Application["WebUrl"]
                , Server.UrlEncode(Param_ModelNo)
                , Server.UrlEncode(Param_Class)
                , col
                , Server.UrlEncode(PageUrl)
            );
    }

    #region "參數設定"
    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"ProductPic\" + Param_ModelNo + @"\" + Param_Class + @"\";
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
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"ProductPic/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 本頁路徑
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return this._PageUrl != null ? this._PageUrl : "ProdPic_Group.aspx?flag=" + Server.UrlEncode(Param_flag) + "&C_ID=" + Server.UrlEncode(Param_Class) + "&ModelNo=" + Server.UrlEncode(Param_ModelNo) + "&r=" + String.Format("{0:mmssfff}", DateTime.Now);
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// [參數] - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get;
        set;
    }

    /// <summary>
    /// [參數] - 圖片類別
    /// </summary>
    private string _Param_Class;
    public string Param_Class
    {
        get;
        set;
    }

    /// <summary>
    /// 來源參數
    /// </summary>
    private string _Param_flag;
    public string Param_flag
    {
        get;
        set;
    }

    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png|pdf";
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
            return 1600;
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
            return 1600;
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
