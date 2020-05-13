using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using LogRecord;


public partial class ProdPic_Photo : SecurityIn
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

            #region -權限控制-
            //[權限判斷] - 圖片資料庫 - 編輯
            if (fn_CheckAuth.CheckAuth_User("303", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }
            //[權限判斷] - 頁簽
            if (Param_flag == "302")
            {
                fn_Extensions.JsAlert("無修改權限！", Session["BackListUrl"].ToString());
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

                //帶出圖片資料
                GetData();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
        }
    }

    #region -- 資料讀取 --
    /// <summary>
    /// 取得資料
    /// </summary>
    private void GetData()
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
                SBSql.AppendLine(" SELECT Pic_ID, PicSource ");
                //[SQL] - 取得檔案
                for (int i = 1; i <= 11; i++)
                {
                    SBSql.AppendLine(string.Format(
                        ", Pic{0}, Pic{0}_OrgFile, Pic{0}_UpdTime", ("0" + Convert.ToString(i)).Right(2)));
                }
                //維護資訊
                SBSql.AppendLine(" , Create_Time, Update_Time");
                SBSql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = ProdPic_Photo.Create_Who)) AS Create_Name ");
                SBSql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = ProdPic_Photo.Update_Who)) AS Update_Name ");
                SBSql.AppendLine(" FROM ProdPic_Photo ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //判斷是否有資料, 控制相關按鈕
                        this.hf_PicID.Value = "";
                        this.btn_DelAll.Visible = false;
                    }
                    else
                    {
                        this.tb_PicSource.Text = DT.Rows[0]["PicSource"].ToString();

                        //填入建立 & 修改資料
                        this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                        this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");

                        //判斷是否有資料, 控制相關按鈕
                        this.hf_PicID.Value = DT.Rows[0]["Pic_ID"].ToString();

                        //[按鈕] - 全部刪除
                        this.btn_DelAll.Visible = true;
                        this.btn_DelAll.Attributes.Add("onClick", "return confirm('是否確定刪除!?\\n此品號的圖檔及關聯性將一併刪除!')");

                        //填入圖片資料
                        /* 未跟ProdPic_Param 做結合 */
                        #region --檔案處理--
                        for (int i = 1; i <= 11; i++)
                        {
                            //宣告參數
                            string idxNum = ("0" + Convert.ToString(i)).Right(2);
                            string PicFile = DT.Rows[0]["Pic" + idxNum].ToString().Trim();

                            //判斷是否有檔案, 填入連結
                            if (string.IsNullOrEmpty(PicFile) == false)
                            {
                                //顯示圖片
                                Literal lt_Pic = (Literal)Page.FindControl("lt_Pic" + idxNum);
                                lt_Pic.Text = Get_PicUrl(PicFile, DT.Rows[0]["Pic" + idxNum + "_OrgFile"].ToString());

                                //更新日期
                                Literal lt_PicUpdTime = (Literal)Page.FindControl("lt_PicUpdTime" + idxNum);
                                lt_PicUpdTime.Text = "更新日期：{0}".FormatThis(DT.Rows[0]["Pic" + idxNum + "_UpdTime"].ToString().ToDateString("yyyy-MM-dd HH:mm"));

                                //檔案原始名稱
                                Literal lt_PicName = (Literal)Page.FindControl("lt_PicName" + idxNum);
                                lt_PicName.Text = "{0}".FormatThis(DT.Rows[0]["Pic" + idxNum + "_OrgFile"].ToString());

                                //圖片外部連結
                                //http://api.prokits.com.tw/ProdImg/1/1/1PK-NT029
                                Literal lt_PicUrl = (Literal)Page.FindControl("lt_PicUrl" + idxNum);                              
                                lt_PicUrl.Text = "<div><span class=\"styleBluelight\">原始圖：</span><input type=\"text\" value=\"{0}\" style=\"width: 280px; cursor: pointer;color: #555\" readonly=\"readonly\" /></div>"
                                    .FormatThis(
                                        "{0}ProdImg/{1}/{2}/{3}/"
                                        .FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["API_WebUrl"]
                                            , Param_Class
                                            , i
                                            , Server.UrlEncode(Param_ModelNo)
                                        )
                                    );

                                //縮圖Url
                                string thumbUrl500 = "<div>縮圖：<a href=\"{0}500x500_{1}\" target=\"_blank\">500x500</a></div>".FormatThis(Param_WebFolder, PicFile);
                                string thumbUrl1000 = "<div>縮圖：<a href=\"{0}1000x1000_{1}\" target=\"_blank\">1000x1000</a></div>".FormatThis(Param_WebFolder, PicFile);
                                lt_PicUrl.Text += thumbUrl500 + thumbUrl1000;

                            }
                        }
                        #endregion
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
        }

    }

    #endregion

    #region -- 資料編輯 (作廢)20180605--
    //存檔
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            //判斷是否有資料
            if (string.IsNullOrEmpty(this.hf_PicID.Value))
            {
                //Insert
                Add_Data();
            }
            else
            {
                //Update
                Edit_Data();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("存檔發生錯誤！", "");
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
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料新增
            StringBuilder SBSql = new StringBuilder();

            SBSql.AppendLine("IF (SELECT COUNT(*) FROM ProdPic_Photo WHERE (Model_No = @Param_ModelNo)) = 0 ");
            SBSql.AppendLine(" BEGIN ");
            SBSql.AppendLine(" Declare @New_ID AS INT SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Photo) ");
            SBSql.AppendLine(" INSERT INTO ProdPic_Photo( ");
            SBSql.AppendLine("  Pic_ID, Model_No, PicSource, Create_Who, Create_Time");
            SBSql.AppendLine(" ) VALUES ( ");
            SBSql.AppendLine("  @New_ID, @Param_ModelNo, @PicSource, @Param_Who, GETDATE()");
            SBSql.AppendLine(" )");
            SBSql.AppendLine(" END ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
            cmd.Parameters.AddWithValue("PicSource", this.tb_PicSource.Text);
            cmd.Parameters.AddWithValue("Param_Who", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料新增成功！", PageUrl);
            }
        }
    }

    /// <summary>
    /// 資料更新
    /// </summary>
    private void Edit_Data()
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {
            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //[SQL] - 資料更新
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" UPDATE ProdPic_Photo ");
            SBSql.AppendLine(" SET PicSource = @PicSource, Update_Who = @Param_Who, Update_Time = GETDATE() ");
            SBSql.AppendLine(" WHERE (Pic_ID = @Param_PicID) ");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_PicID", this.hf_PicID.Value);
            cmd.Parameters.AddWithValue("PicSource", this.tb_PicSource.Text);
            cmd.Parameters.AddWithValue("Param_Who", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
            }
            else
            {
                fn_Extensions.JsAlert("資料更新成功！", PageUrl);
            }
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

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 刪除資料
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM ProdPic_Photo ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo);");

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

                    //更新圖片集Zip
                    Update_ZipFiles(Param_ModelNo, Param_Class);

                    fn_Extensions.JsAlert("刪除成功！", PageUrl);
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 全部刪除！！", "");
        }
    }
    #endregion

    #region -- 自訂功能 --
    /// <summary>
    /// 更新圖片集壓縮檔-(作廢)20180605
    /// </summary>
    /// <param name="ModelNo">品號</param>
    /// <param name="ClassID">類別代號</param>
    private bool Update_ZipFiles(string ModelNo, string ClassID)
    {
        //來源資料夾(產品圖CID = 1)
        string sourceFolder = @"{0}{1}\{2}\{3}".FormatThis(Application["File_DiskUrl"], "ProductPic", ModelNo, ClassID);
        //目標資料夾
        string targetFolder = @"{0}{1}\".FormatThis(Application["File_DiskUrl"], "ProductPic_Zip");
        //目標檔案
        string targetFileName = "{0}_gallery{1}.zip".FormatThis(ModelNo, ClassID);

        return fn_CustomIO.exec_ZipFiles(sourceFolder, targetFolder, targetFileName);
    }

    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <param name="OrgPicName">原始檔名</param>
    public string Get_PicUrl(string PicName, string OrgPicName)
    {
        try
        {
            if (string.IsNullOrEmpty(PicName))
            {
                return "";
            }
            else
            {
                return "<a class=\"PicGroup\" rel=\"PicGroup\" href=\"{0}\" title=\"{1}\"><img src=\"{0}\" width=\"150px\" border=\"0\" style=\"padding: 5px;border: 1px solid #efefef;\"></a>"
                    .FormatThis(Param_WebFolder + PicName, OrgPicName);
            }
        }
        catch (Exception)
        {
            return "";
        }
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
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"ProductPic\" + Server.UrlEncode(Param_ModelNo) + @"\" + Param_Class + @"\";
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
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + "ProductPic/" + Server.UrlEncode(Param_ModelNo) + "/" + Param_Class + "/";
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
            return this._PageUrl != null ? this._PageUrl : Application["WebUrl"] + "ProdPic/ProdPic_Photo.aspx?flag=" + Server.UrlEncode(Param_flag) + "&C_ID=" + Server.UrlEncode(Param_Class) + "&ModelNo=" + Server.UrlEncode(Param_ModelNo) + "&r=" + String.Format("{0:mmssfff}", DateTime.Now);
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

    #endregion

}
