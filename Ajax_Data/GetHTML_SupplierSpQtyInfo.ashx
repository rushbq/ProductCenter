<%@ WebHandler Language="C#" Class="GetHTML_SupplierSpQtyInfo" %>

using System;
using System.Web;
using System.Text;
using SupplierData.Controllers;
using PKLib_Method.Methods;

/// <summary>
/// 供應商採購記錄使用
/// </summary>
public class GetHTML_SupplierSpQtyInfo : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {

        try
        {
            //[接收參數] 查詢字串
            string modelNo = context.Request["modelNo"];
            string supID = context.Request["supID"];
            string validDate = context.Request["validDate"];
            string company = context.Request["company"];

            if (string.IsNullOrEmpty(modelNo))
            {
                context.Response.ContentType = "text/html";
                context.Response.Write("Fail..");
                return;
            }
            
            
            //----- 宣告:資料參數 -----
            SupplierRepository _data = new SupplierRepository();

            //----- 原始資料:取得所有資料 -----
            var query = _data.GetSpQtyInfo(modelNo, supID, company, validDate);

            //----- 資料整理:顯示Html -----
            StringBuilder html = new StringBuilder();
            int row = 0;

            html.Append("<td colspan=\"13\">");
            html.Append("    <div class=\"container\">");
            html.Append("        <table class=\"bordered centered yellow lighten-5\" style=\"width:70%\">");
            html.Append("            <thead>");
            html.Append("                <tr>");
            html.Append("                    <th>幣別</th>");
            html.Append("                    <th>數量以上</th>");
            html.Append("                    <th>金額</th>");
            html.Append("                </tr>");
            html.Append("            </thead>");
            html.Append("            <tbody>");

            //資料迴圈
            foreach (var item in query)
            {
                html.Append("<tr>");
                html.Append("<td>{0}</td><td>{1}</td><td>{2}</td>".FormatThis(
                    item.Currency
                    , item.spQty
                    , item.spPrice
                    ));
                html.Append("</tr>");

                row++;
            }

            html.Append("            </tbody>");
            html.Append("        </table>");
            html.Append("    </div>");
            html.Append("</td>");


            //若無資料
            if (row.Equals(0))
            {
                //clear html
                html.Clear();
                html.Append("<td colspan=\"13\" class=\"center-text\">--- 無分量計價 ---</td>");
            }


            //輸出Html
            context.Response.ContentType = "text/html";
            context.Response.Write(html.ToString());
            
        }
        catch (Exception ex)
        {
            //輸出Html
            context.Response.ContentType = "text/html";
            context.Response.Write("<td colspan=\"13\" class=\"center-text red-text\">--- 壞掉了 ---<br>{0}</td>".FormatThis(
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