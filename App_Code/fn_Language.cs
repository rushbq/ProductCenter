using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 取得語系
/// </summary>
public class fn_Language
{
    /// <summary>
    /// [判斷&取得參數] - 語系
    /// </summary>
    private static string _ProductCenter_Lang;
    public static string ProductCenter_Lang
    {
        get
        {
            return HttpContext.Current.Request.Cookies["ProductCenter_Lang"].Value.ToString();
        }
        private set
        {
            _ProductCenter_Lang = value;
        }
    }

    /// <summary>
    /// 參數用語系, "-" 改 "_"
    /// </summary>
    private static string _Param_Lang;
    public static string Param_Lang
    {
        get
        {
            return HttpContext.Current.Request.Cookies["ProductCenter_Lang"].Value.ToString().Replace("-", "_");
        }
        private set
        {
            _Param_Lang = value;
        }
    }

    /// <summary>
    /// 取得資料庫語系字串
    /// </summary>
    /// <param name="lang">tw/cn/en</param>
    /// <returns></returns>
    public static string Get_DBLangCode(string lang)
    {
        switch (lang.ToUpper())
        {
            case "TW":
                return "zh_TW";

            case "CN":
                return "zh_CN";

            default:
                return "en_US";
        }
    }
}