using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using SelectPdf;

public partial class Product_Test_PDF : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string url = "http://localhost/ProductCenter/myProdCheck/Html_CheckView.aspx?DataID=4e936529-32aa-4baa-90e3-030347dbceba";
        String html = fn_Extensions.WebRequest_GET(url);

        this.TextBox1.Text = html;
    }

    protected void Button2_Click(object sender, EventArgs e)
    {

        //宣告 html to pdf converter
        HtmlToPdf converter = new HtmlToPdf();

        #region -- PDF Options --

        //LicenseKey(重要)
        SelectPdf.GlobalProperties.LicenseKey = System.Web.Configuration.WebConfigurationManager.AppSettings["PDF_Key"]; ;
        //指定 Select.Html.dep 路徑(重要)
        SelectPdf.GlobalProperties.HtmlEngineFullPath = Server.MapPath("~/bin/Select.Html.dep");

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

        // 加入頁碼
        //PdfTextSection text = new PdfTextSection(0, 0, "{page_number}", new System.Drawing.Font("Arial", 10));
        //text.HorizontalAlign = PdfTextHorizontalAlign.Center;
        //converter.Footer.Add(text);


        //-PDF開啟權限
        //converter.Options.SecurityOptions.OwnerPassword = "1234";   //檢視,修改權限
        //converter.Options.SecurityOptions.UserPassword = "4321";    //檢視權限

        #endregion

        //-test
        //string urlContent = fn_Extensions.WebRequest_GET(url);
        //string urlContent = this.TextBox1.Text;
        //PdfDocument doc = converter.ConvertHtmlString(urlContent);


        //取得Url並轉換PDF
        PdfDocument doc = converter.ConvertUrl("http://localhost/ProductCenter/myProdCheck/Html_CheckView.aspx?DataID=4e936529-32aa-4baa-90e3-030347dbceba");

        //加入字型
        doc.AddFont(PdfStandardFont.Helvetica);
        doc.AddFont(new System.Drawing.Font("Microsoft JhengHei", 14));
        doc.AddFont(new System.Drawing.Font("Microsoft YaHei", 14));


        // save pdf document
        byte[] byteDoc = doc.Save();

        // close pdf document
        doc.Close();


        //將檔案輸出至瀏覽器
        Response.Clear();
        Response.AddHeader("Content-Disposition", "attachment;filename={0}".FormatThis("test.pdf"));
        Response.ContentType = "application/octet-stream";
        Response.OutputStream.Write(byteDoc, 0, byteDoc.Length);
        Response.OutputStream.Flush();
        Response.OutputStream.Close();
        Response.Flush();
        Response.End();
    }
}