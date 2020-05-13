<%@ WebHandler Language="C#" Class="GetFile_CheckView" %>

using System;
using System.Web;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;
using SelectPdf;

/// <summary>
/// 外驗查檢表 - 下載PDF
/// 查檢表Html 轉成PDF並下載
/// </summary>
public class GetFile_CheckView : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string modelNo = context.Request["modelNo"];
        string dataID = context.Request["dataID"];


        //Html Url
        string url = "{0}myProdCheck/Html_CheckView.aspx?DataID={1}".FormatThis(
            context.Application["WebUrl"]
            , dataID);

        
        //convert
        convertPDF(url, modelNo);


    }


    /// <summary>
    /// 轉換PDF
    /// </summary>
    /// <param name="html"></param>
    private void convertPDF(string url, string modelNo)
    {
        //宣告 html to pdf converter
        HtmlToPdf converter = new HtmlToPdf();

        #region -- PDF Options --

        //LicenseKey(重要)
        SelectPdf.GlobalProperties.LicenseKey = System.Web.Configuration.WebConfigurationManager.AppSettings["PDF_Key"]; ;
        //指定 Select.Html.dep 路徑(重要)
        SelectPdf.GlobalProperties.HtmlEngineFullPath = HttpContext.Current.Server.MapPath("~/bin/Select.Html.dep");

        //-PageSize
        converter.Options.PdfPageSize = PdfPageSize.A4;
        //-Page orientation
        converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape; //直向-Portrait, 橫向-Landscape
        //-Web page options
        //converter.Options.WebPageWidth = 800;  //預設1024
        //converter.Options.WebPageHeight = 0;  //預設auto

        //-Page margins
        converter.Options.MarginTop = 10;
        converter.Options.MarginRight = 15;
        converter.Options.MarginBottom = 0; //若加入footer就不要設bottom邊界, 不然會多出空白頁
        converter.Options.MarginLeft = 15;

        //-footer 
        converter.Options.DisplayFooter = true;
        converter.Footer.DisplayOnFirstPage = true;
        converter.Footer.DisplayOnOddPages = true;
        converter.Footer.DisplayOnEvenPages = true;
        converter.Footer.Height = 30;


        #endregion

        //ConvertHtml
        //string urlContent = html;
        //PdfDocument doc = converter.ConvertHtmlString(urlContent);
        //取得Url並轉換PDF
        PdfDocument doc = converter.ConvertUrl(url);


        // save pdf document
        byte[] byteDoc = doc.Save();

        // close pdf document
        doc.Close();


        //將檔案輸出
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/octet-stream";

        // 設定強制下載標頭
        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename={0}".FormatThis(
            "查檢表-{0}.pdf".FormatThis(modelNo)));

        // 輸出檔案
        HttpContext.Current.Response.OutputStream.Write(byteDoc, 0, byteDoc.Length);
        HttpContext.Current.Response.OutputStream.Flush();
        HttpContext.Current.Response.OutputStream.Close();

    }



    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}