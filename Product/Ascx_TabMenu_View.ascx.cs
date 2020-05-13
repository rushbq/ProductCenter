using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PKLib_Method.Methods;

public partial class ProductView_Ascx_TabMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //產生Tab選單
            List<TabMenu> listTab = new List<TabMenu>();
            listTab.Add(new TabMenu("1", "Prod_View.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo), "主檔資料", "_self"));
            listTab.Add(new TabMenu("2", "Prod_DtlView.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo), "規格明細", "_self"));
            listTab.Add(new TabMenu("3", "Prod_Files_View.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo), "相關檔案", "_self"));
            listTab.Add(new TabMenu("8", "Prod_MallPicView.aspx?Lang=zh-CN&Model_No=" + Server.UrlEncode(Param_ModelNo), "商城輔圖(簡)", "_self"));
            listTab.Add(new TabMenu("9", "Prod_MallPicView.aspx?Lang=zh-TW&Model_No=" + Server.UrlEncode(Param_ModelNo), "商城輔圖(繁)", "_self"));
            listTab.Add(new TabMenu("10", "Prod_MallPicView.aspx?Lang=en-US&Model_No=" + Server.UrlEncode(Param_ModelNo), "商城輔圖(英)", "_self"));
            
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