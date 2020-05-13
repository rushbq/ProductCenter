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

public partial class ProdPic_Photo_View : SecurityIn
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

            //[權限判斷] - 圖片資料庫 (依 flag 判斷)
            if (fn_CheckAuth.CheckAuth_User(Param_flag, out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

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

                //[取得資料]
                GetData();
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
        }
    }

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
                SBSql.AppendLine(" SELECT Pic_ID");
                //[SQL] - 取得檔案
                for (int i = 1; i <= 11; i++)
                {
                    SBSql.AppendLine(string.Format(
                        ", Pic{0}, Pic{0}_OrgFile, Pic{0}_UpdTime", ("0" + Convert.ToString(i)).Right(2)));
                }
                SBSql.AppendLine(" FROM ProdPic_Photo ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                    }
                    else
                    {
                        //填入圖片資料
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

                                //圖片外部連結
                                Literal lt_PicUrl = (Literal)Page.FindControl("lt_PicUrl" + idxNum);
                                lt_PicUrl.Text = "<span class=\"styleBluelight\">原始圖：</span><input type=\"text\" value=\"{0}\" style=\"width: 280px; cursor: pointer;color: #555\" readonly=\"readonly\" />"
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

    #region -- 自訂功能 --
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
