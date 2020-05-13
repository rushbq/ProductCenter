using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExtensionMethods;
using EcLifeData.Controllers;

/// <summary>
/// 自訂參數
/// </summary>
public class fn_Param
{
    /// <summary>
    /// 良興 Token
    /// </summary>
    public static string Token
    {
        get
        {
            //----- 宣告:資料參數 -----
            EcLifeRepository _data = new EcLifeRepository();

            return _data.GetApiValue("token");
        }
        private set
        {
            _Token = value;
        }
    }
    private static string _Token;


    /// <summary>
    /// 全站分類
    /// </summary>
    public static string Category
    {
        get
        {
            //----- 宣告:資料參數 -----
            EcLifeRepository _data = new EcLifeRepository();

            return _data.GetApiValue("category");
        }
        private set
        {
            _Category = value;
        }
    }
    private static string _Category;


    /// <summary>
    /// 良興API Key
    /// </summary>
    public static string ApiKey
    {
        get
        {
            return "E36CE58C930C4ACF9D719B5D92B81E6F";
        }
        private set
        {
            _ApiKey = value;
        }
    }
    private static string _ApiKey;


    #region -- public --

    /// <summary>
    /// 網站名稱
    /// </summary>
    public static string WebName
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["Web_Name"];
        }
        set
        {
            _WebName = value;
        }
    }
    private static string _WebName;


    /// <summary>
    /// 網站網址
    /// </summary>
    public static string WebUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        }
        set
        {
            _WebUrl = value;
        }
    }
    private static string _WebUrl;


    /// <summary>
    /// CDN網址
    /// </summary>
    public static string CDNUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["CDN_Url"];
        }
        set
        {
            _CDNUrl = value;
        }
    }
    private static string _CDNUrl;


    /// <summary>
    /// API網址
    /// </summary>
    public static string ApiUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["API_WebUrl"];
        }
        set
        {
            _ApiUrl = value;
        }
    }
    private static string _ApiUrl;

    /// <summary>
    /// Ref網址
    /// </summary>
    public static string RefUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"];
        }
        set
        {
            _RefUrl = value;
        }
    }
    private static string _RefUrl;


    /// <summary>
    /// BPM網址
    /// </summary>
    public static string BPM_Url
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["BPMUrl"];
        }
        set
        {
            _BPM_Url = value;
        }
    }
    private static string _BPM_Url;


    /// <summary>
    /// 目前使用者GUID
    /// </summary>
    public static string CurrentUser
    {
        get
        {
            var id = UnobtrusiveSession.Session["Login_GUID"];

            return (id == null) ? "" : id.ToString();
        }
        set
        {
            _CurrentUser = value;
        }
    }
    private static string _CurrentUser;



    /// <summary>
    /// 目前使用者工號
    /// </summary>
    public static string CurrentAccount
    {
        get
        {
            var id = UnobtrusiveSession.Session["Login_UserID"];

            return (id == null) ? "" : id.ToString();
        }
        set
        {
            _CurrentAccount = value;
        }
    }
    private static string _CurrentAccount;


    /// <summary>
    /// UserAccountName
    /// </summary>
    public static string UserAccountName
    {
        get
        {
            return UnobtrusiveSession.Session["Login_UserName"].ToString();
        }
        private set
        {
            _UserAccountName = value;
        }
    }
    private static string _UserAccountName;

    #endregion


    #region -- FTP參數 --
    /// <summary>
    /// FTP帳號
    /// </summary>
    private static string _myFtp_Username;
    public static string myFtp_Username
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Username"];
        }
        set
        {
            _myFtp_Username = value;
        }
    }

    /// <summary>
    /// FTP密碼
    /// </summary>
    private static string _myFtp_Password;
    public static string myFtp_Password
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Password"];
        }
        set
        {
            _myFtp_Password = value;
        }
    }

    /// <summary>
    /// FTP伺服器路徑
    /// </summary>
    private static string _myFtp_ServerUrl;
    public static string myFtp_ServerUrl
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_Url"];
        }
        set
        {
            _myFtp_ServerUrl = value;
        }
    }
    #endregion
}