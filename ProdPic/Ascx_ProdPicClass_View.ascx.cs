using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;

/// <summary>
/// 共用控制項 - 頁籤,檢視模式
/// </summary>
/// <remarks>
/// 1. 讀取圖片類別Xml
/// 2. 取得相關屬性
///     - Class ID:圖片類別編號
///     - Name:頁籤名稱
///     - Page:編輯模式連結
///     - ViewPage:檢視模式連結
///     - Sort:排序
/// 3. 產生頁籤
/// </remarks>
public partial class Ascx_ProdPicClass_View : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    { 
        //取得Xml
        string XmlResult = fn_Extensions.WebRequest_GET(
            System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/ProdPicClass.xml");
        //將Xml字串轉成byte
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
        //讀取Xml
        using (XmlReader reader = XmlTextReader.Create(stream))
        {
            //使用XElement載入Xml
            XElement XmlDoc = XElement.Load(reader);

            var Results = from result in XmlDoc.Elements("Class")
                          orderby Convert.ToInt16(result.Element("Sort").Value) ascending
                          select new
                          {
                              ID = result.Attribute("ID").Value,
                              Name = result.Element("Name").Value,
                              Page = result.Element("ViewPage").Value
                          };
            //輸出圖片類別頁籤選單
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ul>");
            foreach (var result in Results)
            {
                if (Param_CurrPage == result.ID)
                {
                    sb.AppendLine("<li class=\"TabAc\">");
                }
                else
                {
                    sb.AppendLine("<li>");
                }
                sb.AppendLine(string.Format("<a href=\"{0}\" style=\"cursor: pointer;\">{1}</a>",
                    result.Page + "?flag=" + Server.UrlEncode(Param_flag) +"&C_ID=" + result.ID + "&ModelNo=" + Param_ModelNo,
                    result.Name));
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");

            //輸出HTML
            this.lt_Menu.Text = sb.ToString();
        }
    }

    //[參數] - 目前頁籤
    private string _Param_CurrPage;
    public string Param_CurrPage
    {
        get;
        set;
    }

    //[參數] - 品號
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get { return HttpUtility.UrlEncode(this._Param_ModelNo); }
        set {  this._Param_ModelNo = value;}
    }

    //[參數] - 來源參數
    private string _Param_flag;
    public string Param_flag
    {
        get;
        set;
    }
}