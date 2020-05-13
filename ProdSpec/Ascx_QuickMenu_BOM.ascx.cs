using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.Text;

public partial class Ascx_QuickMenu_BOM : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //產生Tab選單
            List<TabMenu> listTab = new List<TabMenu>();
            listTab.Add(new TabMenu("1", "Spec_BOM_Search.aspx", "1.組合明細規格欄位"));
            listTab.Add(new TabMenu("2", "SpecOptionGP_BOM_Search.aspx", "2.組合明細選單單頭設定"));
            listTab.Add(new TabMenu("3", "SpecOption_BOM_Search.aspx", "3.組合明細選單"));

            StringBuilder sbTab = new StringBuilder();
            sbTab.AppendLine("<div class=\"SysTab\">");
            sbTab.AppendLine(" <ul>");
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
                sbTab.AppendLine(string.Format(
                    "<a style=\"cursor: pointer;\" onclick=\"top.mainFrame.location.href='{0}'\">{1}</a>"
                    , listTab[row].TabUrl
                    , listTab[row].TabName));
                sbTab.AppendLine("</li>");
            }
            sbTab.AppendLine(" </ul>");
            sbTab.AppendLine("</div>");

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
        /// 設定參數值
        /// </summary>
        /// <param name="TabIndex">Tab位置</param>
        /// <param name="TabUrl">Tab連結</param>
        /// <param name="TabName">Tab名稱</param>
        public TabMenu(string TabIndex, string TabUrl, string TabName)
        {
            this._TabIndex = TabIndex;
            this._TabUrl = TabUrl;
            this._TabName = TabName;
        }
    }
}