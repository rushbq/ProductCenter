using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using System.Text;

public partial class Ascx_QuickMenu : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //產生Tab選單
            List<TabMenu> listTab = new List<TabMenu>();
            listTab.Add(new TabMenu("1", "SpecClass_Search.aspx", "1.分類設定"));
            listTab.Add(new TabMenu("2", "Spec_Search.aspx", "2.規格欄位"));
            listTab.Add(new TabMenu("4", "Spec_Frame_SpecClass.aspx", "3.分類關聯"));
            listTab.Add(new TabMenu("8", "SpecCategory_Search.aspx", "4.規格分類"));
            listTab.Add(new TabMenu("6", "SpecOptionGP_Search.aspx", "5.選單單頭設定"));
            listTab.Add(new TabMenu("3", "SpecOption_Search.aspx", "6.選單"));
            listTab.Add(new TabMenu("5", "Spec_Rel_ProdItem.aspx", "7.產品關聯"));
            listTab.Add(new TabMenu("9", "Spec_Rel_ProdSpec.aspx", "8.功能關聯"));
            listTab.Add(new TabMenu("7", "Spec_Map.aspx", "9.規格樹狀圖"));
            listTab.Add(new TabMenu("10", "PDFSet_byClass.aspx", "10.PDF匯出設定(by類別)"));

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