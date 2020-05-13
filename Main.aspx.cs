using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using ExtensionMethods;

public partial class Main : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            //[判斷來源參數]
            if (Request.QueryString["t"] != null) {
                string target = Request.QueryString["t"];
                string ModelNo = Request.QueryString["ModelNo"];
                string dataID = Request.QueryString["dataID"];

                switch (target) { 
                    case "Certification":
                        //認證資料庫
                        Response.Redirect("Certification/Cert_Search.aspx");
                        break;

                    case "ProdPic_301":
                        //圖片資料庫 - 行企
                        Response.Redirect("ProdPic/ProdPic_Search.aspx?flag=301");
                        break;

                    case "ProdPic_302":
                        //圖片資料庫 - 品保/產研
                        Response.Redirect("ProdPic/ProdPic_Search.aspx?flag=302");
                        break;

                    case "Product":
                        //產品資料庫
                        Response.Redirect("Product/Prod_Search.aspx");
                        break;

                    case "ProdView":
                        //產品明細
                        Response.Redirect("Product/Prod_View.aspx?Model_No=" + Server.UrlEncode(ModelNo.ToUpper()));
                        break;

                    case "ProdNewsView":
                        //產品訊息明細
                        Response.Redirect("Product/Prod_News_View.aspx?DataID={0}".FormatThis(Server.UrlEncode(dataID)));
                        break;

                    default:
                        break;
                }
            }

          
        }
    }
}