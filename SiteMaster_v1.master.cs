using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;

public partial class SiteMaster : MasterPage, IProgID
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {
        // 下面的程式碼有助於防禦 XSRF 攻擊
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // 使用 Cookie 中的 Anti-XSRF 權杖
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // 產生新的防 XSRF 權杖並儲存到 cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;
    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 設定 Anti-XSRF 權杖
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // 驗證 Anti-XSRF 權杖
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Anti-XSRF 權杖驗證失敗。");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           

        }
    }




    #region Imaster 設定
    /// <summary>
    /// ContentPage 回傳程式編號, 用以判斷選單是否為active
    /// </summary>
    /// <param name="UpID"></param>
    /// <param name="SubID"></param>
    public void setProgID(string UpID, string SubID)
    {
        Prog_UpID = UpID;
        Prog_SubID = SubID;
    }

    /// <summary>
    /// 共用參數, 第一層選單編號
    /// </summary>
    private string _Prog_UpID;
    public string Prog_UpID
    {
        get
        {
            return this._Prog_UpID != null ? this._Prog_UpID : "";
        }
        set
        {
            this._Prog_UpID = value;
        }
    }

    /// <summary>
    /// 共用參數, 第二層選單編號
    /// </summary>
    private string _Prog_SubID;
    public string Prog_SubID
    {
        get
        {
            return this._Prog_SubID != null ? this._Prog_SubID : "";
        }
        set
        {
            this._Prog_SubID = value;
        }
    }
    #endregion

}