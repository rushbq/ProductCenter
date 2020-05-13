using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using Resources;
using System.Collections;

public partial class Spec_Map : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg = "";

                //[代入Ascx參數] - 快速選單
                Ascx_QuickMenu1.Param_CurrItem = "7";

                //[帶出選單] - 規格分類
                Get_ClassMenu();

                //[取得/檢查參數] - 規格分類
                if (false == fn_Extensions.String_字數(Request.QueryString["ClassID"], "5", "5", out ErrMsg))
                {
                    this.lt_TreeView.Text = "&nbsp;<span class=\"styleBlue\">請先選擇分類..</span>";
                    return;
                }
                Param_ClassID = fn_stringFormat.Filter_Html(Request.QueryString["ClassID"].ToString());
                this.ddl_Class.SelectedIndex =
                    this.ddl_Class.Items.IndexOf(this.ddl_Class.Items.FindByValue(Param_ClassID));

                //[取得資料] - 規格總覽(by 分類)
                StringBuilder SBHtml = new StringBuilder();
                if (GetMenu_1st(Param_ClassID, SBHtml, out ErrMsg))
                {
                    this.lt_TreeView.Text = SBHtml.ToString();
                }
                else
                {
                    this.lt_TreeView.Text = "&nbsp;<span class=\"styleRed\">查無資料..<BR>" + ErrMsg + "</span>";
                }

            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "");
                return;
            }
        }
    }

    #region "資料取得"
    /// <summary>
    /// 產生分類選單
    /// </summary>
    private void Get_ClassMenu()
    {
        try
        {
            //[初始化]
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT SpecClassID, ClassName_zh_TW ");
                SBSql.AppendLine(" FROM Prod_Spec_Class WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (UpClass IS NULL) ");
                SBSql.AppendLine(" ORDER BY Sort, SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_Class.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_Class.Items.Add(new ListItem("-- 尚無分類資料 --", ""));
                        return;
                    }
                    //輸出選項
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //子項目
                        this.ddl_Class.Items.Add(
                            new ListItem(DT.Rows[row]["SpecClassID"].ToString() + " - " + DT.Rows[row]["ClassName_zh_TW"].ToString()
                            , DT.Rows[row]["SpecClassID"].ToString()));
                    }
                    this.ddl_Class.Items.Insert(0, new ListItem("-- 選擇大分類 --", ""));
                    this.ddl_Class.SelectedIndex = 0;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
        }
    }

    /// <summary>
    /// [建立樹狀圖] - 第 1 層
    /// </summary>
    /// <param name="ClassID">分類編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool GetMenu_1st(string ClassID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SBHtml.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT SpecClassID AS UpClassID, ClassName_zh_TW AS UpClassName ");
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Prod_Spec_Class Sub WHERE (Sub.Display = 'Y') AND (Sub.UpClass = Prod_Spec_Class.SpecClassID)) AS ChildCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Class WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (UpClass IS NULL) AND (SpecClassID = @ClassID) ");
                SBSql.AppendLine(" ORDER BY Sort, SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ClassID", ClassID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return true;
                    }
                    //[Html] - 根目錄
                    SBHtml.AppendLine("<ul id=\"TreeView\" class=\"filetree\">");
                    //[Html] - 第 1 層 Start
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        SBHtml.AppendLine(string.Format(
                                " <li><span class=\"folder\"><a></a></span>&nbsp;<strong class=\"Font14\">{0} - {1}</strong>"
                                , DT.Rows[row]["UpClassID"]
                                , DT.Rows[row]["UpClassName"]));

                        //[判斷] - 取得下層資料
                        if (Convert.ToInt16(DT.Rows[row]["ChildCnt"]) > 0)
                        {
                            if (false == GetMenu_2nd(DT.Rows[row]["UpClassID"].ToString(), SBHtml, out ErrMsg))
                            {
                                SBHtml.AppendLine(string.Format("<ul><li>資料取得錯誤..{0}</li></ul>", ErrMsg));
                            }
                        }


                        SBHtml.AppendLine(" </li>");
                    }
                    //[Html] - 第 1 層 End
                    SBHtml.AppendLine("</ul>");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// [建立樹狀圖] - 第 2 層
    /// </summary>
    /// <param name="UpID">上層編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool GetMenu_2nd(string UpID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT SpecClassID AS SubClassID, ClassName_zh_TW AS SubClassName ");
                SBSql.AppendLine("  , (SELECT COUNT(*) FROM Prod_SpecClass_Rel_Spec Rel WHERE (Rel.SpecClassID = Prod_Spec_Class.SpecClassID)) AS ChildCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Class WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Display = 'Y') AND (UpClass = @UpID) ");
                SBSql.AppendLine(" ORDER BY Sort, SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UpID", UpID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return true;
                    }
                    SBHtml.AppendLine("<ul>");
                    //[Html] - 第 2 層 Start
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //SubClassID
                        string SubClassID = DT.Rows[row]["SubClassID"].ToString();

                        //定義Tooltip按鈕
                        string btnAdd = string.Format("<a href='{0}' class='EditBox BtnFour'>新增項目</a>"
                            , "../ProdSpec/Spec_Edit.aspx?func=map&SpecClass=" + Server.UrlEncode(SubClassID));
                        string btnEdit = string.Format("<a href='{0}' class='EditBox Edit'>編輯</a>"
                            , "../ProdSpec/SpecClass_Edit.aspx?func=map&SpecClassID=" + Server.UrlEncode(Cryptograph.Encrypt(SubClassID)));

                        SBHtml.AppendLine(string.Format(
                                " <li><span class=\"folder\"><a></a></span>&nbsp;<font class=\"styleBlue tooltip\" title=\"{2}\">{0} - {1}</font>"
                                , DT.Rows[row]["SubClassID"]
                                , DT.Rows[row]["SubClassName"]
                                , btnAdd + "" + btnEdit
                                ));

                        //[判斷] - 取得下層資料
                        if (Convert.ToInt16(DT.Rows[row]["ChildCnt"]) > 0)
                        {
                            if (false == GetMenu_3rd(DT.Rows[row]["SubClassID"].ToString(), SBHtml, out ErrMsg))
                            {
                                SBHtml.AppendLine(string.Format("<ul><li>資料取得錯誤..{0}</li></ul>", ErrMsg));
                            }
                        }

                        SBHtml.AppendLine(" </li>");
                    }
                    //[Html] - 第 2 層 End
                    SBHtml.AppendLine("</ul>");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// [建立樹狀圖] - 第 3 層
    /// </summary>
    /// <param name="UpID">上層編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool GetMenu_3rd(string UpID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //[宣告暫存參數]
                List<TempGPData> listGP = new List<TempGPData>();
                ArrayList aryItem_ID = new ArrayList();
                ArrayList aryItem_Name = new ArrayList();
                ArrayList aryItem_Value = new ArrayList();

                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ");
                SBSql.AppendLine(" SELECT Spec.SpecID, Spec.SpecName_zh_TW, Spec.SpecType, Spec.OptionGID ");
                SBSql.AppendLine("   , Opt.OptionID, Opt.Spec_OptionValue, Opt.Spec_OptionName_zh_TW ");
                SBSql.AppendLine("   , (SELECT COUNT(*) FROM Prod_Spec_Option WHERE (OptionGID = Spec.OptionGID) AND (Display = 'Y')) AS ChildCnt ");
                SBSql.AppendLine("   , ROW_NUMBER() OVER(PARTITION BY Spec.SpecID ORDER BY Spec.SpecID ASC) AS GP_Rank ");
                SBSql.AppendLine("  FROM Prod_Spec AS Spec ");
                SBSql.AppendLine("       INNER JOIN Prod_SpecClass_Rel_Spec Rel ON Spec.SpecID = Rel.SpecID ");
                SBSql.AppendLine("       LEFT JOIN Prod_Spec_Option Opt ON Spec.OptionGID = Opt.OptionGID AND Opt.Display = 'Y' ");
                SBSql.AppendLine("  WHERE (Spec.Display = 'Y') AND (Rel.SpecClassID = @UpID) ");
                SBSql.AppendLine("  ORDER BY Spec.Sort, Spec.SpecID, Opt.Sort, Opt.Spec_OptionValue ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UpID", UpID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return true;
                    }

                    #region -- 暫存處理 --
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        int GP_Rank = Convert.ToInt16(DT.Rows[row]["GP_Rank"]);
                        int ChildCnt = Convert.ToInt16(DT.Rows[row]["ChildCnt"]);

                        //[暫存] - 無子項目, 加入群組
                        if (ChildCnt == 0 && GP_Rank == 1)
                        {
                            listGP.Add(new TempGPData(
                                DT.Rows[row]["SpecID"].ToString()
                                , DT.Rows[row]["SpecName_zh_TW"].ToString()
                                , DT.Rows[row]["SpecType"].ToString()
                                , DT.Rows[row]["OptionGID"].ToString()
                                , null, null, null));
                        }

                        //[暫存] - 加入項目Array
                        if (ChildCnt > 0)
                        {
                            aryItem_ID.Add(DT.Rows[row]["OptionID"].ToString());
                            aryItem_Name.Add(DT.Rows[row]["Spec_OptionName_zh_TW"].ToString());
                            aryItem_Value.Add(DT.Rows[row]["Spec_OptionValue"].ToString());

                            //[暫存] - 判斷為最後一筆時, 加入群組 & 項目資料
                            if (ChildCnt == GP_Rank)
                            {
                                //[暫存] - 宣告新的Array來接收
                                ArrayList aryNew_ID = new ArrayList(aryItem_ID);
                                ArrayList aryNew_Name = new ArrayList(aryItem_Name);
                                ArrayList aryNew_Value = new ArrayList(aryItem_Value);

                                listGP.Add(new TempGPData(
                                   DT.Rows[row]["SpecID"].ToString()
                                   , DT.Rows[row]["SpecName_zh_TW"].ToString()
                                   , DT.Rows[row]["SpecType"].ToString()
                                   , DT.Rows[row]["OptionGID"].ToString()
                                   , aryNew_ID, aryNew_Name, aryNew_Value));

                                //清除目前暫存的Array
                                aryItem_ID.Clear();
                                aryItem_Name.Clear();
                                aryItem_Value.Clear();
                            }
                        }
                    }
                    #endregion
                }

                //[Html] - 第 3 層 Start
                #region -- 資料顯示 --
                if (listGP != null)
                {
                    SBHtml.AppendLine("<ul>");

                    for (int row = 0; row < listGP.Count; row++)
                    {

                        //取得群組資料
                        string SpecID = listGP[row].GP_ID;
                        string SpecName = listGP[row].GP_Name;
                        string SpecType = listGP[row].GP_Type;
                        string OptionGID = listGP[row].GP_OptGID;

                        //定義Tooltip按鈕
                        string btnAdd = string.Format("<a href='{0}' class='EditBox BtnFour'>新增選項</a>"
                            , "../ProdSpec/SpecOption_Edit.aspx?func=map&OptionGID=" + Server.UrlEncode(OptionGID));
                        string btnEdit = string.Format("<a href='{0}' class='EditBox Edit'>編輯</a>"
                            , "../ProdSpec/Spec_Edit.aspx?func=map&SpecID=" + Server.UrlEncode(Cryptograph.Encrypt(SpecID)));

                        //顯示群組資料
                        SBHtml.AppendLine(string.Format(
                                "<li><span class=\"{2}\"><a></a></span>&nbsp;<font class=\"tooltip\" title=\"{4}\">{0} - {1}</font> (<span class=\"styleCafe\">{3}</span>)"
                                , SpecID
                                , SpecName
                                , listGP[row].Item_ID == null ? "file" : "folder"
                                , fn_Desc.Prod.InputType(SpecType)
                                , btnAdd + "" + btnEdit
                                ));

                        //判斷 & 顯示項目資料 
                        if (listGP[row].Item_ID != null)
                        {
                            //[Html] - 第 4 層 Start
                            SBHtml.AppendLine(" <ul>");
                            for (int col = 0; col < listGP[row].Item_ID.Count; col++)
                            {
                                //取得項目資料
                                string Item_ID = listGP[row].Item_ID[col].ToString();
                                string Item_Name = listGP[row].Item_Name[col].ToString();
                                string Item_Value = listGP[row].Item_Value[col].ToString();

                                //定義Tooltip按鈕
                                btnEdit = string.Format("<a href='{0}' class='EditBox Edit'>編輯</a>"
                                    , "../ProdSpec/SpecOption_Edit.aspx?func=map&OptionID=" + Server.UrlEncode(Cryptograph.Encrypt(Item_ID)));

                                SBHtml.AppendLine(string.Format(
                                        " <li><span class=\"file\"><a></a></span>" +
                                        "&nbsp;<span class=\"styleGraylight tooltip\" title=\"{2}\">{0} - {1}</span></li>"
                                        , Item_Value
                                        , Item_Name
                                        , btnEdit
                                        ));
                            }
                            SBHtml.AppendLine(" </ul>");
                            //[Html] - 第 4 層 End
                        }

                        SBHtml.AppendLine("</li>");
                    }
                    SBHtml.AppendLine("</ul>");
                }
                #endregion
                //[Html] - 第 3 層 End
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }
    #endregion

    protected void ddl_Class_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //搜尋網址
            StringBuilder SBUrl = new StringBuilder();
            SBUrl.Append("Spec_Map.aspx?ClassID=" + Server.UrlEncode(this.ddl_Class.SelectedValue));

            //執行轉頁
            Response.Redirect(SBUrl.ToString(), false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 搜尋！", "");
        }
    }

    #region -- 參數設定 Start --
    public class TempGPData
    {
        /// <summary>
        /// 群組編號
        /// </summary>
        private string _GP_ID;
        public string GP_ID
        {
            get { return this._GP_ID; }
            set { this._GP_ID = value; }
        }
        /// <summary>
        /// 群組名稱
        /// </summary>
        private string _GP_Name;
        public string GP_Name
        {
            get { return this._GP_Name; }
            set { this._GP_Name = value; }
        }
        /// <summary>
        /// 群組類別
        /// </summary>
        private string _GP_Type;
        public string GP_Type
        {
            get { return this._GP_Type; }
            set { this._GP_Type = value; }
        }
        /// <summary>
        /// 群組選項代號
        /// </summary>
        private string _GP_OptGID;
        public string GP_OptGID
        {
            get { return this._GP_OptGID; }
            set { this._GP_OptGID = value; }
        }

        /// <summary>
        /// 項目編號
        /// </summary>
        private ArrayList _Item_ID;
        public ArrayList Item_ID
        {
            get { return this._Item_ID; }
            set { this._Item_ID = value; }
        }
        /// <summary>
        /// 項目名稱
        /// </summary>
        private ArrayList _Item_Name;
        public ArrayList Item_Name
        {
            get { return this._Item_Name; }
            set { this._Item_Name = value; }
        }
        /// <summary>
        /// 項目值
        /// </summary>
        private ArrayList _Item_Value;
        public ArrayList Item_Value
        {
            get { return this._Item_Value; }
            set { this._Item_Value = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="GP_ID">群組編號</param>
        /// <param name="GP_Name">群組名稱</param>
        /// <param name="GP_Type">群組類別</param>
        /// <param name="GP_OptGID">群組選項代號</param>
        /// <param name="Item_ID">項目編號(Array)</param>
        /// <param name="Item_Name">項目名稱(Array)</param>
        /// <param name="Item_Value">項目值(Array)</param>
        public TempGPData(string GP_ID, string GP_Name, string GP_Type, string GP_OptGID
              , ArrayList Item_ID, ArrayList Item_Name, ArrayList Item_Value)
        {
            this._GP_ID = GP_ID;
            this._GP_Name = GP_Name;
            this._GP_Type = GP_Type;
            this._GP_OptGID = GP_OptGID;

            this._Item_ID = Item_ID;
            this._Item_Name = Item_Name;
            this._Item_Value = Item_Value;
        }
    }

    //[參數] - 分類編號
    private string _Param_ClassID;
    public string Param_ClassID
    {
        get;
        set;
    }
    #endregion -- 參數設定 End --

}
