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
using Resources;
using System.Collections;

public partial class SpecSummary_byColumn : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg;

            //[權限判斷] - 規格維護-by欄位
            if (fn_CheckAuth.CheckAuth_User("104", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }
        }
    }

    /// <summary>
    /// 產生MD5驗証碼
    /// SessionID + 登入帳號 + 自訂字串
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get { return Cryptograph.MD5(Session.SessionID + fn_Param.CurrentAccount + System.Web.Configuration.WebConfigurationManager.AppSettings["ValidCode_Pwd"], 32); }
        private set { this._ValidCode = value; }
    }
}
