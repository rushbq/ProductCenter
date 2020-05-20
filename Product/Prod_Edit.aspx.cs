using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using Resources;

public partial class Prod_Edit : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - 主檔編輯
                if (fn_CheckAuth.CheckAuth_User("120", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Product/Prod_Search.aspx";

                //[按鈕] - 加入BlockUI
                this.btn_Save.Attributes["onclick"] = fn_Extensions.BlockJs(
                    "Add",
                    "<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>");

                //[取得/檢查參數] - Model_No (品號)
                Param_ModelNo = string.IsNullOrEmpty(Request.QueryString["Model_No"]) ? "" : fn_stringFormat.Filter_Html(Request.QueryString["Model_No"].ToString().Trim());

                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                    return;
                }

                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = "1";
                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;

                //[帶出選單] - 規格分類
                Get_ClassMenu();
                //[帶出選單] - 理想用途
                Get_PubUseMenu();

                //[帶出資料]
                LookupData();
            }
            catch (Exception)
            {
                fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
                return;
            }
        }
    }

    #region -- 資料取得 --
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
                SBSql.AppendLine(" SELECT lv1.SpecClassID AS MtClassID, lv1.ClassName_zh_TW AS MtClassName ");
                SBSql.AppendLine("  , Lv2.SpecClassID AS SubClassID, Lv2.ClassName_zh_TW AS SubClassName ");
                SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY lv1.ClassName_zh_TW ORDER BY lv1.Sort, lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ASC) AS GP_Rank ");
                SBSql.AppendLine(" FROM Prod_Spec_Class AS Lv1 ");
                SBSql.AppendLine("  LEFT JOIN Prod_Spec_Class AS Lv2 ON Lv1.SpecClassID = Lv2.UpClass ");
                SBSql.AppendLine(" WHERE (Lv1.Display = 'Y') AND (Lv1.UpClass IS NULL) AND (Lv2.Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Lv1.Sort, Lv1.SpecClassID, Lv2.Sort, Lv2.SpecClassID ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_SpecClass.Items.Clear();
                    if (DT.Rows.Count == 0)
                    {
                        this.ddl_SpecClass.Items.Add(new ListItem("-- 尚無分類資料 --", ""));
                        return;
                    }
                    //輸出選項
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //判斷GP_Rank, 若為第一項，則輸出群組名稱
                        if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                        {
                            this.ddl_SpecClass.AddItemGroup(DT.Rows[row]["MtClassID"].ToString() + " - " + DT.Rows[row]["MtClassName"].ToString());
                        }
                        //子項目
                        this.ddl_SpecClass.Items.Add(
                            new ListItem(DT.Rows[row]["SubClassID"].ToString() + " - " + DT.Rows[row]["SubClassName"].ToString()
                            , DT.Rows[row]["SubClassID"].ToString()));
                    }
                    this.ddl_SpecClass.Items.Insert(0, new ListItem("-- 選擇分類 --", ""));
                    this.ddl_SpecClass.SelectedIndex = 0;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
        }
    }

    private void Get_PubUseMenu()
    {
        try
        {
            //[初始化]
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Param_Name, Param_Value ");
                SBSql.AppendLine(" FROM Param_Public ");
                SBSql.AppendLine(" WHERE (Param_Kind = '理想用途') ");
                SBSql.AppendLine(" ORDER BY Param_Value ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    this.ddl_PubUse.DataTextField = "Param_Name";
                    this.ddl_PubUse.DataValueField = "Param_Value";
                    this.ddl_PubUse.DataSource = DT.DefaultView;
                    this.ddl_PubUse.DataBind();

                    this.ddl_PubUse.Items.Insert(0, new ListItem("-- 選擇項目 --", ""));
                    this.ddl_PubUse.SelectedIndex = 0;
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產生分類選單！", "");
        }
    }

    /// <summary>
    /// 讀取主檔資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            string ErrMsg;

            //[取得資料] - 主檔
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT ");
                //銷售類別名稱
                SBSql.AppendLine(string.Format(" (SELECT TOP 1 Class_Name_{0} FROM Prod_Class WHERE (Prod_Class.Class_ID = Main.Class_ID)) AS Class_Name", fn_Language.Param_Lang));
                //倉管類別名稱
                SBSql.AppendLine(string.Format(" , (SELECT TOP 1 Class_Name_{0} FROM Warehouse_Class WHERE (Warehouse_Class.Class_ID = Main.Warehouse_Class_ID)) AS Warehouse_Class_Name", fn_Language.Param_Lang));
                //子件型號
                SBSql.AppendLine(" , (SELECT TOP 1 RTRIM(Part_No) FROM Prod_Rel_PartNo WHERE (Prod_Rel_PartNo.Model_No = Main.Model_No)) AS Part_No");
                //主件型號(若有主件則顯示主件型號連結)
                SBSql.AppendLine(" , (SELECT TOP 1 RTRIM(Model_No) FROM Prod_Rel_PartNo WHERE (Prod_Rel_PartNo.Part_No = Main.Model_No)) AS Main_No");
                //圖片(產品圖-判斷圖片中心 2->1->3->4->5->7->8->9)
                SBSql.AppendLine("   , (SELECT TOP 1 (ISNULL(Pic02,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'') ");
                SBSql.AppendLine("      + '|' + ISNULL(Pic05,'') + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')) AS PicGroup");
                SBSql.AppendLine("      FROM ProdPic_Photo WHERE (ProdPic_Photo.Model_No = Main.Model_No)");
                SBSql.AppendLine("   ) AS PhotoGroup ");
                //圖片(最小包裝圖) - from 圖片資料庫->產品圖->配件包材
                SBSql.AppendLine(" , (SELECT TOP 1 Pic05 FROM ProdPic_Photo WHERE (ProdPic_Photo.Model_No = Main.Model_No)) AS Photo_Packing");
                //圖片(產品線圖) - from 圖片資料庫->尺寸示意圖->第一張
                SBSql.AppendLine(" , (SELECT TOP 1 Pic_File FROM ProdPic_Group WHERE (Model_No = Main.Model_No) AND (Pic_Class = 13) ORDER BY Sort, Pic_ID) AS Photo_Outline");
                //維護資訊
                SBSql.AppendLine(" , Create_Time, Update_Time");
                SBSql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Create_Who)) AS Create_Name ");
                SBSql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Account_Name = Main.Update_Who)) AS Update_Name ");
                //共同欄位
                SBSql.AppendLine(" , DBS, Main.Model_No, Main.Item_No, SpecClassID ");
                SBSql.AppendLine(" , Main.Model_Name_zh_TW, Main.Model_Name_zh_CN, Main.Model_Name_en_US, Main.Model_Desc ");
                SBSql.AppendLine(" , Substitute_Model_No_TW, Substitute_Model_No_SH, Main.Substitute_Model_No_SZ ");
                SBSql.AppendLine(" , Cases_Of_Failure_Date_TW, Cases_Of_Failure_Date_SH, Main.Cases_Of_Failure_Date_SZ ");
                SBSql.AppendLine(" , Pub_Patent_No, Pub_Standard1, Pub_Standard2, Pub_Standard3 ");
                SBSql.AppendLine(" , Pub_Logo, Pub_Logo_Printing, Main.Pub_Individual_Packing_zh_TW, Main.Pub_Individual_Packing_en_US ");
                SBSql.AppendLine(" , Pub_PW_L, Pub_PW_W, Pub_PW_H, Pub_PW_Other ");
                SBSql.AppendLine(" , Pub_IP_Net_Weight, Pub_IP_L, Pub_IP_W, Pub_IP_H ");
                SBSql.AppendLine(" , Pub_IB_NW, Pub_IB_GW, Pub_IB_L, Pub_IB_W, Pub_IB_H, Pub_IB_CUFT ");
                SBSql.AppendLine(" , Pub_Recommended ");
                SBSql.AppendLine(" , Pub_Use, Pub_Accessories, Pub_Select_ModelNo, Pub_Compare_ModelNo ");
                SBSql.AppendLine(" , Main.Class_ID, Main.Warehouse_Class_ID ");
                SBSql.AppendLine(" , Main.BarCode, Main.Catelog_Vol, Main.Page, Main.Ship_From, Main.Date_Of_Listing, Main.Stop_Offer_Date ");
                SBSql.AppendLine(" , Main.Pub_IP_Weight, Main.Pub_IB_Qty ");
                SBSql.AppendLine(" , Main.Pub_Carton_Qty_CTN, Main.Pub_Carton_Qty, Main.Pub_Carton_NW, Main.Pub_Carton_GW ");
                SBSql.AppendLine(" , Main.Pub_Carton_L, Main.Pub_Carton_W, Main.Pub_Carton_H, Main.Pub_Carton_CUFT ");
                SBSql.AppendLine(" , Main.Pub_QC_Category, Main.Pub_Notes ");
                SBSql.AppendLine(" , Main.Pub_Card_Model_No, Main.Pub_Alternate2, Main.Pub_Message ");
                SBSql.AppendLine(" FROM Prod_Item Main ");

                SBSql.AppendLine(" WHERE (Main.Model_No = @Param_ModelNo) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Session["BackListUrl"].ToString());
                        return;
                    }
                    else
                    {
                        //填入基本資料
                        #region -基本資料-
                        this.lb_Model_No.Text = DT.Rows[0]["Model_No"].ToString().Trim();     //品號
                        this.lb_Item_No.Text = DT.Rows[0]["Item_No"].ToString().Trim();       //貨號
                        this.lb_Class_ID.Text = "{0} - {1}".FormatThis(DT.Rows[0]["Class_ID"].ToString(), DT.Rows[0]["Class_Name"].ToString());     //銷售類別
                        this.lb_Warehouse_Class_ID.Text = DT.Rows[0]["Warehouse_Class_Name"].ToString();     //倉管類別
                        this.lt_Ship_From.Text = DT.Rows[0]["Ship_From"].ToString();   //主要出貨地
                        //規格類別
                        this.ddl_SpecClass.SelectedIndex = this.ddl_SpecClass.Items.IndexOf(
                          this.ddl_SpecClass.Items.FindByValue(DT.Rows[0]["SpecClassID"].ToString())
                          );
                        this.lt_Date_Of_Listing.Text = DT.Rows[0]["Date_Of_Listing"].ToString().ToDateString_ERP("-");   //上市日期
                        this.tb_Stop_Offer_Date.Text = DT.Rows[0]["Stop_Offer_Date"].ToString().ToDateString("yyyy-MM-dd");   //停售日期
                        this.tb_Part_No.Text = DT.Rows[0]["Part_No"].ToString().Trim();   //子件型號
                        this.tb_Model_Name_zh_TW.Text = DT.Rows[0]["Model_Name_zh_TW"].ToString();   //品名(繁中)
                        this.tb_Model_Name_zh_CN.Text = DT.Rows[0]["Model_Name_zh_CN"].ToString();   //品名(簡中)
                        this.tb_Model_Name_en_US.Text = DT.Rows[0]["Model_Name_en_US"].ToString();   //品名(英文)
                        this.lt_Model_Desc.Text = DT.Rows[0]["Model_Desc"].ToString();       //規格
                        this.lt_BarCode.Text = DT.Rows[0]["BarCode"].ToString();       //條碼
                        this.lt_Catelog_Vol.Text = DT.Rows[0]["Catelog_Vol"].ToString();   //目錄
                        this.lt_Page.Text = DT.Rows[0]["Page"].ToString();   //頁次

                        //主件品號
                        Param_MainNo = DT.Rows[0]["Main_No"].ToString();
                        if (!string.IsNullOrEmpty(Param_MainNo))
                        {
                            ph_MainNo.Visible = true;
                        }

                        //替代品號
                        this.lt_Substitute_Model_No_TW.Text = DT.Rows[0]["Substitute_Model_No_TW"].ToString();
                        this.lt_Substitute_Model_No_SH.Text = DT.Rows[0]["Substitute_Model_No_SH"].ToString();
                        this.lt_Substitute_Model_No_SZ.Text = DT.Rows[0]["Substitute_Model_No_SZ"].ToString();

                        //個案失效日 -> 判斷出貨地，顯示資料
                        switch (this.lt_Ship_From.Text.ToUpper())
                        {
                            case "TW":
                                this.lt_Cases_Of_Failure_Date.Text = DT.Rows[0]["Cases_Of_Failure_Date_TW"].ToString().ToDateString_ERP("-");
                                break;

                            case "SH":
                                this.lt_Cases_Of_Failure_Date.Text = DT.Rows[0]["Cases_Of_Failure_Date_SH"].ToString().ToDateString_ERP("-");
                                break;

                            case "SZ":
                                this.lt_Cases_Of_Failure_Date.Text = DT.Rows[0]["Cases_Of_Failure_Date_SZ"].ToString().ToDateString_ERP("-");
                                break;
                        }

                        //圖片
                        string PhotoGroup = DT.Rows[0]["PhotoGroup"].ToString();
                        if (string.IsNullOrEmpty(PhotoGroup))
                        {
                            this.lt_Photo.Text = "<img src=\"../images/NoPic.png\" width=\"250px\">";
                        }
                        else
                        {
                            //拆解圖片值 "|"
                            string Photo = "";
                            string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");
                            for (int row = 0; row < strAry.Length; row++)
                            {
                                if (false == string.IsNullOrEmpty(strAry[row].ToString()))
                                {
                                    Photo = strAry[row].ToString();
                                    break;
                                }
                            }

                            //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
                            if (false == string.IsNullOrEmpty(Photo))
                            {
                                lt_Photo.Text = string.Format(
                                    "<img src=\"{0}\" width=\"250px\" border=\"0\" rel=\"PicGroup\" class=\"PicGroup\" href=\"{0}\" title=\"{1}\" style=\"cursor:pointer\">"
                                    , Param_WebFolder + Server.UrlEncode(Param_ModelNo) + "/1/" + Photo
                                    , Param_ModelNo + " - 產品圖");
                            }
                        }
                        #endregion

                        //填入共用欄位資料
                        #region -共用欄位資料-
                        this.tb_Pub_Patent_No.Text = DT.Rows[0]["Pub_Patent_No"].ToString();
                        this.tb_Pub_Standard1.Text = DT.Rows[0]["Pub_Standard1"].ToString();
                        this.tb_Pub_Standard2.Text = DT.Rows[0]["Pub_Standard2"].ToString();
                        this.tb_Pub_Standard3.Text = DT.Rows[0]["Pub_Standard3"].ToString();
                        this.rbl_Pub_Logo.SelectedValue = DT.Rows[0]["Pub_Logo"].ToString();
                        this.tb_Pub_Logo_Printing.Text = DT.Rows[0]["Pub_Logo_Printing"].ToString();

                        // - 產品單一裸重
                        this.tb_Pub_IP_Net_Weight.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_Net_Weight"].ToString(), 4);    //裸重(g) - 小數點4位
                        this.tb_Pub_PW_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_PW_L"].ToString(), 2);  //單一裸重.尺寸:長(cm) - 小數點2位
                        this.tb_Pub_PW_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_PW_W"].ToString(), 2);  //單一裸重.尺寸:寬(cm) - 小數點2位
                        this.tb_Pub_PW_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_PW_H"].ToString(), 2);  //單一裸重.尺寸:高(cm) - 小數點2位
                        this.tb_Pub_PW_Other.Text = DT.Rows[0]["Pub_PW_Other"].ToString();

                        // - Individual Packing
                        this.lt_Pub_Individual_Packing_zh_TW.Text = DT.Rows[0]["Pub_Individual_Packing_zh_TW"].ToString();
                        this.lt_Pub_Individual_Packing_en_US.Text = DT.Rows[0]["Pub_Individual_Packing_en_US"].ToString();
                        this.lt_Pub_IP_Weight.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_Weight"].ToString(), 3);    //最小包裝.單重 (Kg) - 小數點3位
                        this.tb_Pub_IP_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_L"].ToString(), 2);  //最小包裝.尺寸:長(cm) - 小數點2位
                        this.tb_Pub_IP_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_W"].ToString(), 2);  //最小包裝.尺寸:寬(cm) - 小數點2位
                        this.tb_Pub_IP_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_H"].ToString(), 2);  //最小包裝.尺寸:高(cm) - 小數點2位
                        //最小包裝圖片  
                        if (string.IsNullOrEmpty(DT.Rows[0]["Photo_Packing"].ToString()) == false)
                        {
                            lt_Photo_Packing.Text = string.Format(
                             "<img src=\"{0}\" width=\"100px\" border=\"0\" rel=\"PicGroup\" class=\"PicGroup\" href=\"{0}\" title=\"{1}\" style=\"cursor:pointer\">"
                             , Param_WebFolder + Server.UrlEncode(Param_ModelNo) + "/1/" + DT.Rows[0]["Photo_Packing"].ToString()
                             , Param_ModelNo + " - 最小包裝圖");
                        }
                        // - Inner box
                        this.lt_Pub_IB_Qty.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_Qty"].ToString(), 0);  //內盒產品數量 - 整數
                        this.tb_Pub_IB_NW.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_NW"].ToString(), 2);    //內箱淨重/Kg - 小數點2位
                        this.tb_Pub_IB_GW.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_GW"].ToString(), 2);    //內箱毛重/Kg - 小數點2位
                        this.tb_Pub_IB_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_L"].ToString(), 2);  //內箱尺寸:長(cm) - 小數點2位
                        this.tb_Pub_IB_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_W"].ToString(), 2);  //內箱尺寸:寬(cm) - 小數點2位
                        this.tb_Pub_IB_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_H"].ToString(), 2);  //內箱尺寸:高(cm) - 小數點2位
                        this.lt_Pub_IB_CUFT.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_CUFT"].ToString(), 2);    //內箱材積(CUFT) - 小數點2位(L*W*H/28317)
                        // - Carton
                        this.lt_Pub_Carton_Qty_CTN.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_Qty_CTN"].ToString(), 0);  //整箱數量 - 整數
                        this.lt_Pub_Carton_Qty.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_Qty"].ToString(), 0);  //外包裝含內盒數 - 整數
                        this.lt_Pub_Carton_NW.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_NW"].ToString(), 2);    //外箱淨重/Kg - 小數點2位
                        this.lt_Pub_Carton_GW.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_GW"].ToString(), 2);    //外箱毛重/Kg - 小數點2位
                        this.lt_Pub_Carton_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_L"].ToString(), 2);  //外箱尺寸:長(cm) - 小數點2位
                        this.lt_Pub_Carton_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_W"].ToString(), 2);  //外箱尺寸:寬(cm) - 小數點2位
                        this.lt_Pub_Carton_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_H"].ToString(), 2);  //外箱尺寸:高(cm) - 小數點2位
                        this.lt_Pub_Carton_CUFT.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_Carton_CUFT"].ToString(), 2);    //外箱材積(CUFT) - 小數點2位
                        // - 其他
                        this.lt_Pub_QC_Category.Text = DT.Rows[0]["Pub_QC_Category"].ToString();
                        this.rbl_Pub_Recommended.Text = DT.Rows[0]["Pub_Recommended"].ToString();
                        this.lt_Pub_Notes.Text = DT.Rows[0]["Pub_Notes"].ToString().Replace("\r\n", "<BR/>");
                        //this.tb_Pub_Features_Remark.Text = DT.Rows[0]["Pub_Features_Remark"].ToString();  //--removed
                        this.lt_Pub_Card_Model_No.Text = DT.Rows[0]["Pub_Card_Model_No"].ToString();
                        this.lt_Pub_Alternate2.Text = DT.Rows[0]["Pub_Alternate2"].ToString();
                        this.lt_Pub_Message.Text = DT.Rows[0]["Pub_Message"].ToString().Replace("\r\n", "<BR/>");
                        //產品線圖  
                        if (string.IsNullOrEmpty(DT.Rows[0]["Photo_Outline"].ToString()) == false)
                        {
                            lt_Photo_Outline.Text = string.Format(
                             "<img src=\"{0}\" width=\"100px\" border=\"0\" rel=\"PicGroup\" class=\"PicGroup\" href=\"{0}\" title=\"{1}\" style=\"cursor:pointer\">"
                             , Param_WebFolder + Server.UrlEncode(Param_ModelNo) + "/13/" + DT.Rows[0]["Photo_Outline"].ToString()
                             , Param_ModelNo + " - 產品線圖");
                        }
                        //帶出產品檢驗輔助圖片
                        Lookup_QCPics();
                        //帶出品管檢驗項目
                        Lookup_QCItems(DT.Rows[0]["Ship_From"].ToString(), DT.Rows[0]["Pub_QC_Category"].ToString(), DT.Rows[0]["Model_No"].ToString());

                        //理想用途
                        this.tb_Use_Item_Val.Text = DT.Rows[0]["Pub_Use"].ToString();
                        this.lt_UseItems.Text = Get_PubUseHtml(this.tb_Use_Item_Val.Text);
                        //主件/配件
                        this.rbl_Pub_Accessories.SelectedValue = DT.Rows[0]["Pub_Accessories"].ToString();
                        //選購配件型號
                        this.tb_Select_Item_Val.Text = DT.Rows[0]["Pub_Select_ModelNo"].ToString();
                        this.lt_SelectItems.Text = Get_ModelNoHtml("Select", this.tb_Select_Item_Val.Text);
                        //對應主件型號
                        this.tb_Compare_Item_Val.Text = DT.Rows[0]["Pub_Compare_ModelNo"].ToString();
                        this.lt_CompareItems.Text = Get_ModelNoHtml("Compare", this.tb_Compare_Item_Val.Text);

                        //帶出認證資料
                        LookupCertList();

                        //帶出規格符號
                        Lookup_Icons();

                        #endregion

                        //填入建立 & 修改資料
                        this.lt_Create_Who.Text = DT.Rows[0]["Create_Name"].ToString();
                        this.lt_Create_Time.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                        this.lt_Update_Who.Text = DT.Rows[0]["Update_Name"].ToString();
                        this.lt_Update_Time.Text = DT.Rows[0]["Update_Time"].ToString().ToDateString("yyyy-MM-dd HH:mm");
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 基本資料！", "");
            return;
        }

    }

    /// <summary>
    /// 帶出品管檢驗項目
    /// </summary>
    /// <param name="ShipFrom">主要出貨地</param>
    /// <param name="QC_Category">品管類別</param>
    /// <param name="ModelNo">品號</param>
    private void Lookup_QCItems(string ShipFrom, string QC_Category, string ModelNo)
    {
        try
        {
            string ErrMsg = "";
            //[取得資料] - 品管檢驗項目
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                //[SQL] - 查詢, '檢驗名稱', '缺點等級', '檢驗標準說明', '備註'
                SBSql.AppendLine(" SELECT ERP_QMSME.ME002, ERP_QMSMG.MG005, ERP_QMSMG.MG006, ERP_QMSMG.MG007");
                SBSql.AppendLine(" FROM ERP_QMSMG ");
                //[SQL] - 關聯性
                //QMSMG.MG003 = QMSME.ME001, QMSMG.MG004 = QMSME.ME003
                //QMSMG.DBS = QMSME.DBS
                // AND ERP_QMSMG.MG004 = ERP_QMSME.ME003
                SBSql.AppendLine("  INNER JOIN ERP_QMSME ON ERP_QMSMG.MG003 = ERP_QMSME.ME001 ");
                SBSql.AppendLine("   AND ERP_QMSMG.DBS = ERP_QMSME.DBS ");
                //[SQL] - 條件, MG001:品管類別, MG002:品號, DBS:資料來源
                SBSql.AppendLine(" WHERE (ERP_QMSME.DBS = @ShipFrom) AND (ERP_QMSMG.MG001 = @QC_Category) AND (ERP_QMSMG.MG002 = @ModelNo) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("ShipFrom", ShipFrom);
                cmd.Parameters.AddWithValue("QC_Category", QC_Category);
                cmd.Parameters.AddWithValue("ModelNo", ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 品管檢驗項目！", "");
            return;
        }

    }

    /// <summary>
    /// 帶出產品檢驗輔助圖片
    /// </summary>
    private void Lookup_QCPics()
    {
        try
        {
            string ErrMsg = "";
            this.lt_QC_Pics.Text = "";
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Pic_File, Pic_Desc");
                SBSql.AppendLine(" FROM ProdPic_Group ");
                SBSql.AppendLine(" WHERE (Pic_Class = '14') AND (Model_No = @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY Sort ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        this.lt_QC_Pics.Text += string.Format(
                                  "<img src=\"{0}\" width=\"50px\" border=\"0\" rel=\"QCPicGroup\" class=\"PicGroup L2Img\" href=\"{0}\" title=\"{1}\" style=\"cursor:pointer\">&nbsp;"
                                  , Param_WebFolder + Server.UrlEncode(Param_ModelNo) + "/14/" + DT.Rows[i]["Pic_File"].ToString()
                                  , DT.Rows[i]["Pic_Desc"].ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 產品檢驗輔助圖片！", "");
            return;
        }

    }

    /// <summary>
    /// 帶出規格符號, 預設帶符號資料庫, Icon_Type = Product的所有資料
    /// </summary>
    private void Lookup_Icons()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT Icon_Pics.Pic_ID, Icon_Pics.Pic_File, Rel.Pic_ID AS RelID ");
                SBSql.AppendLine("  FROM Icon ");
                SBSql.AppendLine("      INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                SBSql.AppendLine("      LEFT JOIN Icon_Rel_PKWeb Rel ON Icon_Pics.Pic_ID = Rel.Pic_ID AND Rel.Model_No = @Model_No ");
                SBSql.AppendLine(" WHERE (Icon.Icon_Type = 'Product') ");
                SBSql.AppendLine(" ORDER BY Icon.Sort, Icon.Icon_ID ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvIconList.DataSource = DT.DefaultView;
                    this.lvIconList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 規格符號！", "");
            return;
        }
    }

    protected void lvIconList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //[判斷] - 官網是否顯示(Icon_Rel_PKWeb)
            RadioButtonList rbl_Display = (RadioButtonList)e.Item.FindControl("rbl_Display");
            string IsShow = DataBinder.Eval(dataItem.DataItem, "RelID").ToString();
            if (string.IsNullOrEmpty(IsShow))
            {
                rbl_Display.SelectedIndex = rbl_Display.Items.IndexOf(rbl_Display.Items.FindByValue("N"));
            }
            else
            {
                rbl_Display.SelectedIndex = rbl_Display.Items.IndexOf(rbl_Display.Items.FindByValue("Y"));
            }
        }
    }
    #endregion

    #region -- 認證資料取得 --
    /// <summary>
    /// 副程式 - 取得認證資料列表
    /// </summary>
    private void LookupCertList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg = "";

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" SELECT CertDtl.* ");
                SBSql.AppendLine("  FROM Prod_Certification Cert ");
                SBSql.AppendLine("   INNER JOIN Prod_Certification_Detail CertDtl ON Cert.Cert_ID = CertDtl.Cert_ID ");
                SBSql.AppendLine(" WHERE (Cert.Model_No= @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY CertDtl.Cert_Type ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvCertList.DataSource = DT.DefaultView;
                    this.lvCertList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 認證資料！", "");
        }
    }

    protected void lvCertList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //證書類別
            Literal lt_CertType = (Literal)e.Item.FindControl("lt_CertType");
            lt_CertType.Text = DataBinder.Eval(dataItem.DataItem, "Cert_Type").ToString();
            if (false == string.IsNullOrEmpty(DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString()))
            {
                lt_CertType.Text += " - " + DataBinder.Eval(dataItem.DataItem, "Cert_TypeText").ToString();
            }

            //圖片顯示 - 認證符號
            Literal lt_Icon = ((Literal)e.Item.FindControl("lt_Icon"));
            //判斷是否為CE
            if (DataBinder.Eval(dataItem.DataItem, "IsCE").ToString() == "Y")
            {
                lt_Icon.Text = "<img src=\"" + Application["File_WebUrl"] + "icon_cert/108.jpg\" width=\"35\" />";
            }
            else
            {
                lt_Icon.Text = fn_Extensions.GetCertIcon(
                    DataBinder.Eval(dataItem.DataItem, "Cert_ID").ToString()
                    , DataBinder.Eval(dataItem.DataItem, "Detail_ID").ToString());
            }

            #region --檔案處理--
            //檔案顯示 - 證書
            Literal lt_CertFile = ((Literal)e.Item.FindControl("lt_CertFile"));
            lt_CertFile.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File").ToString());

            //檔案顯示 - TestReport
            Literal lt_FileTestReport = ((Literal)e.Item.FindControl("lt_FileTestReport"));
            lt_FileTestReport.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Report").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File_Report").ToString());

            //檔案顯示 - 自我宣告
            Literal lt_FileCE = ((Literal)e.Item.FindControl("lt_FileCE"));
            lt_FileCE.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE").ToString());

            Literal lt_FileCE_enUS = ((Literal)e.Item.FindControl("lt_FileCE_enUS"));
            lt_FileCE_enUS.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_en_US").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_en_US").ToString());

            Literal lt_FileCE_zhCN = ((Literal)e.Item.FindControl("lt_FileCE_zhCN"));
            lt_FileCE_zhCN.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_CE_zh_CN").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_CE_zh_CN").ToString());

            //檔案顯示 - 自我檢測
            Literal lt_FileCheck = ((Literal)e.Item.FindControl("lt_FileCheck"));
            lt_FileCheck.Text = setFileUri(
                DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check").ToString().Trim()
                , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check").ToString());

            Literal lt_FileCheck_enUS = ((Literal)e.Item.FindControl("lt_FileCheck_enUS"));
            lt_FileCheck_enUS.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_en_US").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_en_US").ToString());

            Literal lt_FileCheck_zhCN = ((Literal)e.Item.FindControl("lt_FileCheck_zhCN"));
            lt_FileCheck_zhCN.Text = setFileUri(
              DataBinder.Eval(dataItem.DataItem, "Cert_OrgFile_Check_zh_CN").ToString().Trim()
              , DataBinder.Eval(dataItem.DataItem, "Cert_File_Check_zh_CN").ToString());
            #endregion
        }
    }

    #endregion

    #region -- 按鈕區 --
    /// <summary>
    /// 儲存鈕
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料更新, 規格符號
                if (this.lvIconList.Items.Count > 0)
                {
                    SBSql.AppendLine(" DELETE FROM Icon_Rel_PKWeb WHERE (Model_No = @Param_ModelNo); ");

                    for (int row = 0; row < lvIconList.Items.Count; row++)
                    {
                        //[取得參數] - 編號
                        string lvParam_ID = ((HiddenField)this.lvIconList.Items[row].FindControl("hf_PicID")).Value;
                        //[取得參數] - 顯示
                        string lvParam_Disp = ((RadioButtonList)this.lvIconList.Items[row].FindControl("rbl_Display")).SelectedValue;

                        if (lvParam_Disp.Equals("Y"))
                        {
                            SBSql.Append(" INSERT INTO Icon_Rel_PKWeb(Pic_ID, Model_No ");
                            SBSql.Append("  ) VALUES ( ");
                            SBSql.Append(string.Format(
                                " @lvParam_ID_{0}, @Param_ModelNo "
                                , row));
                            SBSql.AppendLine(" );");
                            cmd.Parameters.AddWithValue("lvParam_ID_" + row, lvParam_ID);
                        }
                    }
                }

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Prod_Item SET ");
                SBSql.AppendLine("  SpecClassID = @SpecClassID, Stop_Offer_Date = @Stop_Offer_Date");
                SBSql.AppendLine("  , Model_Name_zh_TW = @Model_Name_zh_TW, Model_Name_zh_CN = @Model_Name_zh_CN, Model_Name_en_US = @Model_Name_en_US");
                SBSql.AppendLine("  , Pub_Patent_No = @Pub_Patent_No, Pub_Standard1 = @Pub_Standard1, Pub_Standard2 = @Pub_Standard2, Pub_Standard3 = @Pub_Standard3");
                SBSql.AppendLine("  , Pub_Logo = @Pub_Logo, Pub_Logo_Printing = @Pub_Logo_Printing");
                SBSql.AppendLine("  , Pub_PW_L = @Pub_PW_L, Pub_PW_W = @Pub_PW_W, Pub_PW_H = @Pub_PW_H, Pub_PW_Other = @Pub_PW_Other");
                SBSql.AppendLine("  , Pub_IP_Net_Weight = @Pub_IP_Net_Weight, Pub_IP_L = @Pub_IP_L, Pub_IP_W = @Pub_IP_W, Pub_IP_H = @Pub_IP_H");
                SBSql.AppendLine("  , Pub_IB_NW = @Pub_IB_NW, Pub_IB_GW = @Pub_IB_GW, Pub_IB_L = @Pub_IB_L, Pub_IB_W = @Pub_IB_W, Pub_IB_H = @Pub_IB_H, Pub_IB_CUFT = @Pub_IB_CUFT");
                SBSql.AppendLine("  , Pub_Recommended = @Pub_Recommended, Pub_Accessories = @Pub_Accessories");
                SBSql.AppendLine("  , Pub_Use = @Pub_Use, Pub_Select_ModelNo = @Pub_Select_ModelNo, Pub_Compare_ModelNo = @Pub_Compare_ModelNo");
                SBSql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE() ");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo); ");

                //子件型號 Part_No (Prod_Rel_PartNo)
                if (false == string.IsNullOrEmpty(this.val_Part_No.Text))
                {
                    SBSql.AppendLine(" DELETE FROM Prod_Rel_PartNo WHERE (Model_No = @Param_ModelNo); ");
                    SBSql.AppendLine(" INSERT INTO Prod_Rel_PartNo (Model_No, Part_No, Create_Who, Create_Time) ");
                    SBSql.AppendLine("  VALUES (@Param_ModelNo, @Part_No, @Create_Who, GETDATE()); ");

                    cmd.Parameters.AddWithValue("Part_No", this.val_Part_No.Text);
                }
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Create_Who", fn_Param.CurrentAccount.ToString());
                cmd.Parameters.AddWithValue("Update_Who", fn_Param.CurrentAccount.ToString());
                #region "欄位"
                //規格類別, SpecClassID
                cmd.Parameters.AddWithValue("SpecClassID", this.ddl_SpecClass.SelectedValue);
                //停售日期, Stop_Offer_Date
                cmd.Parameters.AddWithValue("Stop_Offer_Date"
                    , this.tb_Stop_Offer_Date.Text == "" ? DBNull.Value : (object)this.tb_Stop_Offer_Date.Text.ToDateString("yyyy-MM-dd"));
                //品名
                cmd.Parameters.AddWithValue("Model_Name_zh_TW", this.tb_Model_Name_zh_TW.Text.Trim());
                cmd.Parameters.AddWithValue("Model_Name_zh_CN", this.tb_Model_Name_zh_CN.Text.Trim());
                cmd.Parameters.AddWithValue("Model_Name_en_US", this.tb_Model_Name_en_US.Text.Trim());
                //專利號碼, Pub_Patent_No
                cmd.Parameters.AddWithValue("Pub_Patent_No", this.tb_Pub_Patent_No.Text.Trim());
                cmd.Parameters.AddWithValue("Pub_Standard1", this.tb_Pub_Standard1.Text.Trim());
                cmd.Parameters.AddWithValue("Pub_Standard2", this.tb_Pub_Standard2.Text.Trim());
                cmd.Parameters.AddWithValue("Pub_Standard3", this.tb_Pub_Standard3.Text.Trim());
                //Logo/呈現方式, Pub_Logo/Pub_Logo_Printing
                cmd.Parameters.AddWithValue("Pub_Logo", this.rbl_Pub_Logo.SelectedValue);
                cmd.Parameters.AddWithValue("Pub_Logo_Printing", this.tb_Pub_Logo_Printing.Text.Trim());

                //單一裸重.尺寸:長(cm), Pub_PW_L
                cmd.Parameters.AddWithValue("Pub_PW_L", this.tb_Pub_PW_L.Text.Trim());
                //單一裸重.尺寸:寬(cm), Pub_PW_W
                cmd.Parameters.AddWithValue("Pub_PW_W", this.tb_Pub_PW_W.Text.Trim());
                //單一裸重.尺寸:高(cm), Pub_PW_H
                cmd.Parameters.AddWithValue("Pub_PW_H", this.tb_Pub_PW_H.Text.Trim());
                //單一裸重.其他
                cmd.Parameters.AddWithValue("Pub_PW_Other", this.tb_Pub_PW_Other.Text.Trim());

                //裸重(g), Pub_IP_Net_Weight
                cmd.Parameters.AddWithValue("Pub_IP_Net_Weight", this.tb_Pub_IP_Net_Weight.Text.Trim());
                //最小包裝.尺寸:長(cm), Pub_IP_L
                cmd.Parameters.AddWithValue("Pub_IP_L", this.tb_Pub_IP_L.Text.Trim());
                //最小包裝.尺寸:寬(cm), Pub_IP_W
                cmd.Parameters.AddWithValue("Pub_IP_W", this.tb_Pub_IP_W.Text.Trim());
                //最小包裝.尺寸:高(cm), Pub_IP_H
                cmd.Parameters.AddWithValue("Pub_IP_H", this.tb_Pub_IP_H.Text.Trim());
                //內箱淨重/Kg, Pub_IB_NW
                cmd.Parameters.AddWithValue("Pub_IB_NW", this.tb_Pub_IB_NW.Text.Trim());
                //內箱毛重/Kg, Pub_IB_GW
                cmd.Parameters.AddWithValue("Pub_IB_GW", this.tb_Pub_IB_GW.Text.Trim());
                //內箱尺寸:長(cm), Pub_IB_L
                cmd.Parameters.AddWithValue("Pub_IB_L", this.tb_Pub_IB_L.Text.Trim());
                //內箱尺寸:寬(cm), Pub_IB_W
                cmd.Parameters.AddWithValue("Pub_IB_W", this.tb_Pub_IB_W.Text.Trim());
                //內箱尺寸:高(cm), Pub_IB_H
                cmd.Parameters.AddWithValue("Pub_IB_H", this.tb_Pub_IB_H.Text.Trim());
                //內箱尺寸:材積(CUFT), 小數點2位(L*W*H/28317)
                double IB_CUFT =
                    (Convert.ToDouble(string.IsNullOrEmpty(this.tb_Pub_IB_L.Text) ? "0" : this.tb_Pub_IB_L.Text.Trim())
                    * Convert.ToDouble(string.IsNullOrEmpty(this.tb_Pub_IB_W.Text) ? "0" : this.tb_Pub_IB_W.Text.Trim())
                    * Convert.ToDouble(string.IsNullOrEmpty(this.tb_Pub_IB_H.Text) ? "0" : this.tb_Pub_IB_H.Text.Trim()))
                    / 28317;
                cmd.Parameters.AddWithValue("Pub_IB_CUFT", fn_stringFormat.Decimal_Format(IB_CUFT.ToString(), 2));

                //主廚推薦, Pub_Recommended
                cmd.Parameters.AddWithValue("Pub_Recommended", this.rbl_Pub_Recommended.Text.Trim());
                //特點說明, Pub_Features_Remark(舊欄位,不維護 2017-11-02)
                //cmd.Parameters.AddWithValue("Pub_Features_Remark", this.tb_Pub_Features_Remark.Text);
                //主/配件, Pub_Accessories
                cmd.Parameters.AddWithValue("Pub_Accessories", this.rbl_Pub_Accessories.SelectedValue);
                //理想用途, Pub_Use
                cmd.Parameters.AddWithValue("Pub_Use", Filter_Value(this.tb_Use_Item_Val.Text));
                //選購配件型號, Pub_Select_ModelNo
                cmd.Parameters.AddWithValue("Pub_Select_ModelNo", Filter_Value(this.tb_Select_Item_Val.Text));
                //對應主件型號, Pub_Compare_ModelNo
                cmd.Parameters.AddWithValue("Pub_Compare_ModelNo", Filter_Value(this.tb_Compare_Item_Val.Text));
                #endregion
                if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                {
                    fn_Extensions.JsAlert("儲存失敗！", "");
                    return;
                }
                else
                {
                    fn_Extensions.JsAlert("儲存成功！", "script:location.replace('Prod_Edit.aspx?Model_No=" + Server.UrlEncode(Param_ModelNo) + "');");
                    return;
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存！", "");
            return;
        }
    }
    #endregion

    #region -- 其他功能 --
    /// <summary>
    /// 過濾重複資料
    /// </summary>
    /// <param name="inputValue"></param>
    /// <returns></returns>
    private string Filter_Value(string inputValue)
    {
        if (string.IsNullOrEmpty(inputValue))
        {
            return "";
        }

        ArrayList aryItem = new ArrayList();
        //拆解值
        string[] strAry = Regex.Split(inputValue, @"\|{4}");
        //篩選，移除重複資料
        var query = from el in strAry
                    group el by el.ToString().Trim() into gp
                    select new
                    {
                        Val = gp.Key
                    };
        foreach (var item in query)
        {
            aryItem.Add(item.Val);
        }
        //回傳篩選後的資料
        return string.Join("||||", aryItem.ToArray());
    }

    /// <summary>
    /// 理想用途Html
    /// </summary>
    /// <param name="inputValue"></param>
    /// <returns></returns>
    private string Get_PubUseHtml(string inputValue)
    {
        if (string.IsNullOrEmpty(inputValue))
        {
            return "";
        }
        string ErrMsg;

        //拆解值
        string[] strAry = Regex.Split(inputValue, @"\|{4}");
        //[取得資料] - 主檔
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder SBSql = new StringBuilder();
            //[SQL] - 查詢 Param_Public
            SBSql.AppendLine(" SELECT Param_Name, Param_Value FROM Param_Public ");
            SBSql.AppendLine(" WHERE (Param_Kind = '理想用途') ");
            SBSql.AppendLine(string.Format(" AND (Param_Value IN ({0}))"
                , string.Join(",", strAry)));
            //[SQL] - 執行
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                StringBuilder html = new StringBuilder();

                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    html.AppendLine("<li id=\"li_Use" + row + "\" class=\"as-selection-item blur\">");
                    html.AppendLine(DT.Rows[row]["Param_Name"].ToString());
                    html.AppendLine("<input type=\"text\" class=\"Item_Val\" value=\"" + DT.Rows[row]["Param_Value"].ToString() + "\" style=\"display:none\" />");
                    html.AppendLine("<a style=\"background:transparent\" href=\"javascript:Delete_Item('Use" + row + "');\"><span class=\"JQ-ui-icon ui-icon-trash\"></span></a>");
                    html.AppendLine("</li>");
                }


                return html.ToString();
            }
        }
    }

    /// <summary>
    /// 回傳型號Html
    /// </summary>
    /// <param name="inputKind">輸入類別</param>
    /// <param name="inputValue">輸入值</param>
    /// <returns></returns>
    private string Get_ModelNoHtml(string inputKind, string inputValue)
    {
        if (string.IsNullOrEmpty(inputValue))
        {
            return "";
        }
        //拆解值，回傳Html
        string[] strAry = Regex.Split(inputValue, @"\|{4}");
        var query = from el in strAry
                    select new
                    {
                        Val = el.ToString().Trim()
                    };
        StringBuilder html = new StringBuilder();

        int row = 0;
        foreach (var item in query)
        {
            row++;
            html.AppendLine("<li id=\"li_" + inputKind + row + "\" class=\"as-selection-item blur\">");
            html.AppendLine(item.Val);
            html.AppendLine("<input type=\"text\" class=\"Item_Val\" value=\"" + item.Val + "\" style=\"display:none\" />");
            html.AppendLine("<a style=\"background:transparent\" href=\"javascript:Delete_Item('" + inputKind + row + "');\"><span class=\"JQ-ui-icon ui-icon-trash\"></span></a>");
            html.AppendLine("</li>");
        }

        return html.ToString();
    }

    /// <summary>
    /// 取得符號圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <returns>string</returns>
    public string PicUrl(string PicName)
    {
        string preView = "";

        //判斷是否為圖片
        string strFileExt = ".jpg||.png||.gif";
        if (fn_Extensions.CheckStrWord(PicName, strFileExt, "|", 2))
        {
            //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
            preView = string.Format(
                "<div class=\"L2Img\" style=\"text-align:center\"> " +
                "<img src=\"{0}\" width=\"50px\" border=\"0\">" +
                "</div>"
                , Param_IconWebFolder + PicName
                );

        }

        //輸出Html
        return preView;
    }

    /// <summary>
    /// 設定檔案下載連結
    /// </summary>
    /// <param name="orgName">原始檔名</param>
    /// <param name="fileName">真實檔名</param>
    /// <returns>string</returns>
    private string setFileUri(string orgName, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }
        else
        {
            string uri = string.Format("../FileDownload.ashx?OrgiName={0}&FilePath={1}"
                , Server.UrlEncode(orgName.Trim())
                , Server.UrlEncode(Cryptograph.Encrypt(Cert_DiskFolder + fileName)));

            return "<a href=\"" + uri + "\">下載</a>";
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return this._Param_WebFolder != null ? this._Param_WebFolder : Application["File_WebUrl"] + @"ProductPic/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }
    /// <summary>
    /// [參數] - Disk資料夾路徑
    /// </summary>
    private string _Cert_DiskFolder;
    public string Cert_DiskFolder
    {
        get
        {
            return Application["File_DiskUrl"] + string.Format(@"Certification\{0}\", Param_ModelNo);
        }
        set
        {
            this._Cert_DiskFolder = value;
        }
    }
    /// <summary>
    /// [參數] - Web資料夾路徑 (符號)
    /// </summary>
    private string _Param_IconWebFolder;
    public string Param_IconWebFolder
    {
        get
        {
            return this._Param_IconWebFolder != null ? this._Param_IconWebFolder : Application["File_WebUrl"] + @"Icons/";
        }
        set
        {
            this._Param_IconWebFolder = value;
        }
    }
    /// <summary>
    /// 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return this._Param_ModelNo != null ? this._Param_ModelNo : this.lb_Model_No.Text.Trim().ToUpper();
        }
        private set
        {
            this._Param_ModelNo = value;
        }
    }

    /// <summary>
    /// 主件品號
    /// </summary>
    private string _Param_MainNo;
    public string Param_MainNo
    {
        get;
        set;
    }
    #endregion

    #region -- 語系參數 --
    /// <summary>
    /// [Navi] - 系統首頁
    /// </summary>
    private string _Navi_系統首頁;
    public string Navi_系統首頁
    {
        get
        {
            return Res_Navi.系統首頁;
        }
        private set
        {
            this._Navi_系統首頁 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料庫;
    public string Navi_產品資料庫
    {
        get
        {
            return Res_Navi.產品資料庫;
        }
        private set
        {
            this._Navi_產品資料庫 = value;
        }
    }
    /// <summary>
    /// [Navi] - 產品資料
    /// </summary>
    private string _Navi_產品資料;
    public string Navi_產品資料
    {
        get
        {
            return Res_Navi.產品資料;
        }
        private set
        {
            this._Navi_產品資料 = value;
        }
    }

    #endregion
}
