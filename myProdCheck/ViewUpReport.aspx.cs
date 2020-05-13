using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;

/// <summary>
/// 列出已上傳的報告
/// 使用Remote Modal開啟本頁
/// 此開啟方式無法使用PostBack事件
/// 若要使用請用iframe的開啟方式
/// </summary>
public partial class myProdCheck_ViewUpReport : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("530", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", Application["WebUrl"], HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //Files
                LookupData_Files("1", this.lv_Files_Check);
                LookupData_Files("2", this.lv_Files_Other);
            }

        }
        catch (Exception)
        {

            throw;
        }
    }



    /// <summary>
    /// 取得檔案資料
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    private void LookupData_Files(string type, ListView list)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _dataList = new ProdCheckRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetFileList(Req_DataID, type);


        //----- 資料整理:繫結 ----- 
        list.DataSource = query;
        list.DataBind();

        //Release
        query = null;
    }



    #region -- 參數設定 --

    /// <summary>
    /// 取得參數 - DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;



    /// <summary>
    /// 取得參數 - ModelNo
    /// </summary>
    public string Req_ModelNo
    {
        get
        {
            String data = Request.QueryString["ModelNo"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_ModelNo = value;
        }
    }
    private string _Req_ModelNo;


    /// <summary>
    /// 上傳目錄 - 品號附件
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}ProdCheck/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], Req_DataID);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}