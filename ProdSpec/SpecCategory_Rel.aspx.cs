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
using System.Text.RegularExpressions;

public partial class SpecCategory_Rel : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string ErrMsg;

                //[權限判斷] - 規格設定
                if (fn_CheckAuth.CheckAuth_User("102", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "ProdSpec/SpecCategory_Search.aspx";

                //[檢查參數] - CateID (規格分類)
                if (fn_Extensions.Num_正整數(Request.QueryString["CateID"], "1", "2147483600", out ErrMsg))
                {
                    if (Request.QueryString["CateName"] == null)
                    {
                        fn_Extensions.JsAlert("參數傳遞錯誤", Session["BackListUrl"].ToString());
                        return;
                    }

                    this.tb_SpecCateName.Text = fn_stringFormat.Filter_Html(Request.QueryString["CateName"].ToString());
                    this.tb_SpecCateID.Text = fn_stringFormat.Filter_Html(Request.QueryString["CateID"].ToString());

                    //[取得資料]
                    LookupData();
                }

                //[代入Ascx參數] - 快速選單
                Ascx_QuickMenu1.Param_CurrItem = "8";

                //[按鈕] - 加入BlockUI
                this.btn_GetRelID.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Save",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
    void LookupData()
    {
        string ErrMsg;

        //[取得資料]
        StringBuilder SBHtml = new StringBuilder();
        if (GetMenu_1st(this.tb_SpecCateID.Text.Trim(), SBHtml, out ErrMsg))
        {
            this.lt_TreeView.Text = SBHtml.ToString();
        }
        else
        {
            this.lt_TreeView.Text = "&nbsp;<span class=\"styleRed\">查無資料..<BR>" + ErrMsg + "</span>";
        }
    }

    /// <summary>
    /// [建立樹狀圖] - 第 1 層
    /// </summary>
    /// <param name="CateID">規格分類編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private bool GetMenu_1st(string CateID, StringBuilder SBHtml, out string ErrMsg)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SBHtml.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ");
                SBSql.AppendLine(" SELECT Class.SpecClassID AS MtClassID, Class.ClassName_zh_TW AS MtClassName ");
                SBSql.AppendLine(" 	, (SELECT COUNT(*) FROM Prod_Spec_Class Sub WHERE (Sub.Display = 'Y') AND (Sub.UpClass = Class.SpecClassID)) AS ChildCnt ");
                SBSql.AppendLine(" FROM Prod_Spec_Class Class ");
                SBSql.AppendLine(" WHERE (Class.Display = 'Y') AND (Class.UpClass IS NULL) ");
                SBSql.AppendLine(" ORDER BY Class.Sort, Class.SpecClassID ");
                cmd.CommandText = SBSql.ToString();
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
                        //[取得欄位資料]
                        string MtClassID = DT.Rows[row]["MtClassID"].ToString();
                        string MtClassName = DT.Rows[row]["MtClassName"].ToString();

                        SBHtml.AppendLine(string.Format(
                                        "<li><span class=\"folder\"><a></a></span>&nbsp;" +
                                        "<strong class=\"Font14\">{0} - {1}</strong>"
                                        , MtClassID
                                        , MtClassName));

                        //[判斷] - 取得下層資料
                        if (Convert.ToInt16(DT.Rows[row]["ChildCnt"]) > 0)
                        {
                            if (false == GetMenu_2nd(CateID, MtClassID, SBHtml, out ErrMsg))
                            {
                                SBHtml.AppendLine(string.Format("<ul><li>資料取得錯誤..{0}</li></ul>", ErrMsg));
                            }
                        }
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
    /// [建立樹狀圖] - 第 2 ~ 3 層
    /// </summary>
    /// <param name="CateID">規格分類編號</param>
    /// <param name="UpID">上層編號</param>
    /// <param name="SBHtml">Html</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool GetMenu_2nd(string CateID, string UpID, StringBuilder SBHtml, out string ErrMsg)
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
                //[欄位] - 小分類
                SBSql.AppendLine(" SELECT Class.SpecClassID AS SubClassID, Class.ClassName_zh_TW AS SubClassName ");
                //[欄位] - 規格欄位
                SBSql.AppendLine("     , Spec.SpecID, Spec.SpecName_zh_TW ");
                //[欄位] - 關聯編號
                SBSql.AppendLine("     , RelCate.CateID ");
                //[欄位] - 子項目數
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_SpecClass_Rel_Spec Rel WHERE (Rel.SpecClassID = Class.SpecClassID)) AS ChildCnt ");
                //[欄位] - 規格分類關聯數
                SBSql.AppendLine("     , (SELECT COUNT(*) FROM Prod_Spec_Rel_Category inRelCate WHERE (inRelCate.SpecClassID = Class.SpecClassID) AND (inRelCate.CateID = @CateID)) AS CheckedCnt ");
                //[欄位] - 群組編號
                SBSql.AppendLine("     , ROW_NUMBER() OVER(PARTITION BY Class.SpecClassID ORDER BY Class.Sort, Class.SpecClassID, Spec.Sort, Spec.SpecID ASC) AS GP_Rank ");
                SBSql.AppendLine(" FROM Prod_Spec_Class Class ");
                SBSql.AppendLine("     LEFT JOIN Prod_SpecClass_Rel_Spec Rel ON Class.SpecClassID = Rel.SpecClassID ");
                SBSql.AppendLine("     LEFT JOIN Prod_Spec Spec ON Rel.SpecID = Spec.SpecID AND (Spec.Display = 'Y') ");
                //[資料表] - 規格分類關聯檔
                SBSql.AppendLine("     LEFT JOIN Prod_Spec_Rel_Category RelCate ");
                SBSql.AppendLine("      ON RelCate.SpecClassID = Rel.SpecClassID AND RelCate.SpecID = Rel.SpecID AND RelCate.CateID = @CateID ");
                //[條件] - 大分類/分類顯示='Y'
                SBSql.AppendLine(" WHERE (Class.UpClass = @UpID) AND (Class.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Class.Sort, Class.SpecClassID, Spec.Sort, Spec.SpecID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UpID", UpID);
                cmd.Parameters.AddWithValue("CateID", CateID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return true;
                    }

                    #region -- 暫存處理 --
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //[取得欄位資料]
                        Int16 GP_Rank = Convert.ToInt16(DT.Rows[row]["GP_Rank"]);
                        int ChildCnt = Convert.ToInt32(DT.Rows[row]["ChildCnt"]);
                        int CheckedCnt = Convert.ToInt32(DT.Rows[row]["CheckedCnt"]);
                        string SubClassID = DT.Rows[row]["SubClassID"].ToString();      //分類編號
                        string SubClassName = DT.Rows[row]["SubClassName"].ToString();  //分類名稱
                        string SpecID = DT.Rows[row]["SpecID"].ToString();              //規格編號
                        string SpecName = DT.Rows[row]["SpecName_zh_TW"].ToString();          //規格名稱
                        string RelID = DT.Rows[row]["CateID"].ToString();    //關聯編號

                        //[暫存] - 無子項目, 加入群組
                        if (ChildCnt == 0 && GP_Rank == 1)
                        {
                            listGP.Add(new TempGPData(
                                SubClassID, SubClassName, CheckedCnt
                                , null, null, null));
                        }

                        //[暫存] - 加入項目Array
                        if (ChildCnt > 0)
                        {
                            aryItem_ID.Add(SpecID);
                            aryItem_Name.Add(SpecName);
                            aryItem_Value.Add(RelID);

                            //[暫存] - 判斷為最後一筆時, 加入群組 & 項目資料
                            if (ChildCnt == GP_Rank)
                            {
                                //[暫存] - 宣告新的Array來接收
                                ArrayList aryNew_ID = new ArrayList(aryItem_ID);
                                ArrayList aryNew_Name = new ArrayList(aryItem_Name);
                                ArrayList aryNew_Value = new ArrayList(aryItem_Value);

                                listGP.Add(new TempGPData(
                                   SubClassID, SubClassName, CheckedCnt
                                   , aryNew_ID, aryNew_Name, aryNew_Value));

                                //清除目前暫存的Array
                                aryItem_ID.Clear();
                                aryItem_Name.Clear();
                                aryItem_Value.Clear();
                            }
                        }
                    }
                    #endregion

                    //[Html] - 第 2~3 層 Start
                    #region -- 資料顯示 --
                    if (listGP != null)
                    {
                        SBHtml.AppendLine("<ul>");

                        for (int row = 0; row < listGP.Count; row++)
                        {
                            //取得群組資料
                            string ClassID = listGP[row].GP_ID;
                            string ClassName = listGP[row].GP_Name;

                            //顯示群組資料
                            //[Checkbox] - id:cb_分類編號
                            SBHtml.AppendLine(string.Format(
                                    "<li><span class=\"{2}\"><a></a></span>&nbsp;" +
                                    "<label><input type=\"checkbox\" id=\"cb_{0}\" runat=\"server\" value=\"\" {3}><font class=\"styleBlue\">{0} - {1}</font></label>"
                                    , ClassID
                                    , ClassName
                                    , listGP[row].Item_ID == null ? "file" : "folder"
                                    , listGP[row].GP_RelCnt > 0 ? "checked" : ""
                                    ));

                            //判斷 & 顯示項目資料 
                            if (listGP[row].Item_ID != null)
                            {
                                //[Html] - 第 3 層 Start
                                SBHtml.AppendLine(" <ul>");
                                for (int col = 0; col < listGP[row].Item_ID.Count; col++)
                                {
                                    //取得項目資料
                                    string SpecID = listGP[row].Item_ID[col].ToString();
                                    string SpecName = listGP[row].Item_Name[col].ToString();
                                    string RelID = listGP[row].Item_Value[col].ToString();

                                    //判斷是否有勾選
                                    string hasChecked = string.IsNullOrEmpty(RelID) ? "" : "checked";

                                    //[Checkbox] - id:cb_上層分類_規格編號, rel:cb_上層分類, value:上層分類|規格編號
                                    SBHtml.AppendLine(string.Format(
                                            "<li><span class=\"file\"><a></a></span>" +
                                            "<label><input type=\"checkbox\" id=\"cb_{2}_{0}\" runat=\"server\" rel=\"cb_{2}\" value=\"{2}|{0}\" {3}>{0} - {1}</label>" +
                                            "</li>"
                                            , SpecID
                                            , SpecName
                                            , ClassID
                                            , hasChecked
                                            ));
                                }
                                SBHtml.AppendLine(" </ul>");
                                //[Html] - 第 3 層 End
                            }

                            SBHtml.AppendLine("</li>");
                        }
                        SBHtml.AppendLine("</ul>");
                    }
                    #endregion
                    //[Html] - 第 2~3 層 End
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
    #endregion

    #region -- 按鈕區 --
    //帶出資料
    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            //[初始化]
            string ErrMsg;

            //[檢查參數] - CateID (規格分類)
            if (false == fn_Extensions.Num_正整數(this.tb_SpecCateID.Text, "1", "2147483600", out ErrMsg))
            {
                fn_Extensions.JsAlert("請選擇正確的「規格分類」！", "");
                return;
            }

            Response.Redirect("SpecCategory_Rel.aspx?CateID=" + Server.UrlEncode(this.tb_SpecCateID.Text.Trim()) +
                "&CateName=" + Server.UrlEncode(this.tb_SpecCateName.Text.Trim())
                , false);
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
        }
    }

    //設定
    protected void btn_GetRelID_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            //取得關聯編號, 先分析Checkbox(,) , 再分析Value(|)
            string[] aryRelID = Regex.Split(this.hf_RelID.Value, @"\,{1}");
            if (aryRelID == null)
            {
                fn_Extensions.JsAlert("未勾選任何項目！", "");
                return;
            }
            //設定權限
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                //[SQL] - 刪除原關聯
                SBSql.AppendLine(" DELETE FROM Prod_Spec_Rel_Category WHERE (CateID = @CateID); ");
                //[SQL] - 新增關聯
                for (int row = 0; row < aryRelID.Length; row++)
                {
                    //分析Value(|)
                    string[] aryValue = Regex.Split(aryRelID[row], @"\|{1}");
                    SBSql.AppendLine(string.Format(
                        " INSERT INTO Prod_Spec_Rel_Category(SpecClassID, SpecID, CateID) VALUES ('{0}','{1}',@CateID); "
                        , aryValue[0]
                        , aryValue[1]));
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("CateID", this.tb_SpecCateID.Text);
                if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("關聯設定失敗！", "");
                    return;
                }
                else
                {
                    fn_Extensions.JsAlert("關聯設定成功！"
                        , "SpecCategory_Rel.aspx?CateID=" + Server.UrlEncode(this.tb_SpecCateID.Text) + "&CateName=" + Server.UrlEncode(this.tb_SpecCateName.Text.Trim()));
                    return;
                }

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 設定關聯！", "");
            return;
        }
    }
    #endregion

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
        /// 關聯項數
        /// </summary>
        private int _GP_RelCnt;
        public int GP_RelCnt
        {
            get { return this._GP_RelCnt; }
            set { this._GP_RelCnt = value; }
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
        /// <param name="GP_RelCnt">關聯項數</param>
        /// <param name="Item_ID">項目編號(Array)</param>
        /// <param name="Item_Name">項目名稱(Array)</param>
        /// <param name="Item_Value">項目值(Array)</param>
        public TempGPData(string GP_ID, string GP_Name, int GP_RelCnt
            , ArrayList Item_ID, ArrayList Item_Name, ArrayList Item_Value)
        {
            this._GP_ID = GP_ID;
            this._GP_Name = GP_Name;
            this._GP_RelCnt = GP_RelCnt;

            this._Item_ID = Item_ID;
            this._Item_Name = Item_Name;
            this._Item_Value = Item_Value;
        }
    }

    #endregion -- 參數設定 End --
}
