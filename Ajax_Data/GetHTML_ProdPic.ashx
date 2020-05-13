<%@ WebHandler Language="C#" Class="GetHTML_ProdPic" %>

using System;
using System.Web;
using System.Text;
using System.Linq;
using SupplierData.Controllers;
using PKLib_Method.Methods;
using ProdPhotoData.Controllers;

/// <summary>
/// 取得產品圖片 - 檢驗記錄使用
/// </summary>
public class GetHTML_ProdPic : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {

        try
        {
            //[接收參數] 查詢字串
            string modelNo = context.Request["modelNo"];
            string classID = context.Request["clsID"];

            if (string.IsNullOrEmpty(modelNo) || string.IsNullOrEmpty(classID))
            {
                context.Response.ContentType = "text/html";
                context.Response.Write("Fail..");
                return;
            }


            //----- 宣告:資料參數 -----
            ProdPhotoRepository _data = new ProdPhotoRepository();


            //----- 原始資料:取得所有資料 -----
            var query = _data.GetPhotos(modelNo, classID);


            //----- 資料整理:顯示Html -----
            StringBuilder html = new StringBuilder();

            if (query.Count() == 0)
            {
                html.Append("<h5>-- 無資料 --</h5>");
            }
            else
            {
                switch (classID)
                {
                    case "1":
                    case "2":

                        #region -- 類別1-2 --

                        html.Append("<div class=\"row\">");
                        foreach (var item in query)
                        {
                            if (!string.IsNullOrEmpty(item.ColValue))
                            {
                                html.Append(" <div class=\"col s6 m4 l3\">");
                                html.Append(" <div class=\"card\">");
                                html.Append("  <div class=\"card-image\">");
                                html.Append("<img src=\"{0}\" class=\"materialboxed\" data-caption=\"{1}\">".FormatThis(
                                    "{0}ProductPic/{1}/{2}/{3}".FormatThis(
                                        System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"].ToString(), HttpUtility.UrlEncode(modelNo), classID, item.ColValue)
                                    , item.ColName
                                    ));
                                html.Append("  </div>");
                                html.Append("  <div class=\"card-content\"><span class=\"card-title\">{0}</span></div>".FormatThis(item.ColName));
                                html.Append(" </div>");
                                html.Append(" </div>");
                            }
                        }

                        html.Append("</div>");
                        
                        #endregion

                        break;

                    default:

                        #region -- 其他類別 --

                        html.Append("<div class=\"collection\">");
                        foreach (var item in query)
                        {
                            if (!string.IsNullOrEmpty(item.ColValue))
                            {
                                html.Append("<a href=\"{0}\" class=\"collection-item\" target=\"_blank\">{1}</a>".FormatThis(
                                    "{0}ProductPic/{1}/{2}/{3}".FormatThis(
                                        System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"].ToString(), HttpUtility.UrlEncode(modelNo), classID, item.ColValue)
                                    , item.ColName
                                    ));
                            }
                        }

                        html.Append("</div>");
                        
                        #endregion

                        break;
                }

            }


            //輸出Html
            context.Response.ContentType = "text/html";
            context.Response.Write(html.ToString());

        }
        catch (Exception ex)
        {
            //輸出Html
            context.Response.ContentType = "text/html";
            context.Response.Write("<div class=\"center-text red-text\">--- 壞掉了 ---<br>{0}</div>".FormatThis(
                ex.Message.ToString()));
        }


    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}