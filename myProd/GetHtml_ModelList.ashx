<%@ WebHandler Language="C#" Class="GetHtml_ModelList" %>

using System;
using System.Web;
using ProdAttributeData.Controllers;
using PKLib_Method.Methods;

public class GetHtml_ModelList : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string ErrMsg = "";

        System.Threading.Thread.Sleep(1000);

        try
        {
            //ContentType
            context.Response.ContentType = "text/html";

            //[接收參數]
            string _id = context.Request["id"];

            if (string.IsNullOrWhiteSpace(_id))
            {
                context.Response.Write("Load fail..");
                return;
            }

            //----- 宣告:資料參數 -----
            ProdItemPropRespository _data = new ProdItemPropRespository();

            //----- 原始資料:取得所有資料 -----
            var query = _data.Get_Models(_id, out ErrMsg);


            //----- 資料整理:顯示Html -----
            System.Text.StringBuilder html = new System.Text.StringBuilder();

            html.Append("<i class=\"horizontally flipped level up alternate icon grey-text\"></i>");

            for (int row = 0; row < query.Rows.Count; row++)
            {
                string _model = query.Rows[row]["ModelNo"].ToString();
                html.Append("<div class=\"ui basic label\">{0}</div>".FormatThis(_model));
            }


            //若無資料
            if (query.Rows.Count == 0)
            {
                html.Clear();
                html.Append("<p class=\"red-text\">查無資料....</p>");
            }

            query = null;

            //輸出Html
            context.Response.Write(html.ToString());

        }
        catch (Exception ex)
        {
            context.Response.Write("<p class=\"red-text\">資料查詢時發生錯誤!!</p>" + ex.Message.ToString() + ErrMsg);
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