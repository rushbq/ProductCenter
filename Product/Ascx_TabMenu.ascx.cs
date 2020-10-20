using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PKLib_Method.Methods;

public partial class Product_Ascx_TabMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //產生Tab選單
            List<TabMenu> listTab = new List<TabMenu>();
            listTab.Add(new TabMenu("1", "Prod_Edit.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo), "主檔資料", "_self"));
            listTab.Add(new TabMenu("2", "Prod_DtlEdit.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo), "規格明細", "_self"));
            listTab.Add(new TabMenu("3", "PdFSet_byItem.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo), "PDF匯出設定", "_self"));
            listTab.Add(new TabMenu("4", "Prod_InfoEdit.aspx?Lang=zh-CN&Model_No=" + Server.UrlEncode(Param_ModelNo), "產品資訊(簡)", "_self"));
            listTab.Add(new TabMenu("5", "Prod_InfoEdit.aspx?Lang=zh-TW&Model_No=" + Server.UrlEncode(Param_ModelNo), "產品資訊(繁)", "_self"));
            listTab.Add(new TabMenu("6", "Prod_InfoEdit.aspx?Lang=en-US&Model_No=" + Server.UrlEncode(Param_ModelNo), "產品資訊(英)", "_self"));
            listTab.Add(new TabMenu("7", "http://pkef.prokits.com.tw?t=dwfiles", "檔案維護", "_blank"));
            listTab.Add(new TabMenu("8", "Prod_MallPic.aspx?Lang=zh-CN&Model_No=" + Server.UrlEncode(Param_ModelNo), "商城輔圖(簡)", "_self"));
            listTab.Add(new TabMenu("9", "Prod_MallPic.aspx?Lang=zh-TW&Model_No=" + Server.UrlEncode(Param_ModelNo), "商城輔圖(繁)", "_self"));
            listTab.Add(new TabMenu("10", "Prod_MallPic.aspx?Lang=en-US&Model_No=" + Server.UrlEncode(Param_ModelNo), "商城輔圖(英)", "_self"));
            listTab.Add(new TabMenu("20", "SupplierHistory.aspx?DataID=" + Server.UrlEncode(Param_ModelNo), "供應商採購記錄", "_self"));
            listTab.Add(new TabMenu("30", "{0}EcLife/ProductEdit.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Server.UrlEncode(Param_ModelNo)), "良興商品維護", "_self"));
            listTab.Add(new TabMenu("40", "{0}myProd_Extend/Prod_SetClass.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Server.UrlEncode(Param_ModelNo)), "電子目錄分類", "_self"));
            listTab.Add(new TabMenu("50", "{0}myProd_Extend/Toy_SetClass.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Server.UrlEncode(Param_ModelNo)), "科玩分類", "_self"));

            StringBuilder sbTab = new StringBuilder();
            sbTab.AppendLine("<ul>");
            for (int row = 0; row < listTab.Count; row++)
            {
                //判斷是否為目前位置
                if (listTab[row].TabIndex.Equals(Param_CurrItem))
                {
                    sbTab.AppendLine("<li class=\"TabAc\">");
                }
                else
                {
                    sbTab.AppendLine("<li>");
                }
                //Url
                sbTab.AppendLine(string.Format(
                    "<a href=\"{0}\" target=\"{2}\">{1}</a>"
                    , listTab[row].TabUrl
                    , listTab[row].TabName
                    , listTab[row].TabTarget
                    ));
                sbTab.AppendLine("</li>");
            }
            sbTab.AppendLine("</ul>");

            this.lt_TabMenu.Text = sbTab.ToString();
        }
    }


    /// <summary>
    /// [參數] - 目前選項
    /// </summary>
    private string _Param_CurrItem;
    public string Param_CurrItem
    {
        get;
        set;
    }

    /// <summary>
    /// [參數] - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get;
        set;
    }

    /// <summary>
    /// Tab選單
    /// </summary>
    public class TabMenu
    {
        /// <summary>
        /// [參數] - Tab位置
        /// </summary>
        private string _TabIndex;
        public string TabIndex
        {
            get { return this._TabIndex; }
            set { this._TabIndex = value; }
        }

        /// <summary>
        /// [參數] - Tab連結
        /// </summary>
        private string _TabUrl;
        public string TabUrl
        {
            get { return this._TabUrl; }
            set { this._TabUrl = value; }
        }

        /// <summary>
        /// [參數] - Tab名稱
        /// </summary>
        private string _TabName;
        public string TabName
        {
            get { return this._TabName; }
            set { this._TabName = value; }
        }

        /// <summary>
        /// [參數] - Tab Target
        /// </summary>
        private string _TabTarget;
        public string TabTarget
        {
            get { return this._TabTarget; }
            set { this._TabTarget = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="TabIndex">Tab位置</param>
        /// <param name="TabUrl">Tab連結</param>
        /// <param name="TabName">Tab名稱</param>
        public TabMenu(string TabIndex, string TabUrl, string TabName, string TabTarget)
        {
            this._TabIndex = TabIndex;
            this._TabUrl = TabUrl;
            this._TabName = TabName;
            this._TabTarget = TabTarget;
        }
    }
}