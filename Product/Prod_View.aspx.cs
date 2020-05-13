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
using System.Text.RegularExpressions;

public partial class Prod_View : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[初始化]
                string ErrMsg;

                //[權限判斷] - 產品資料
                if (fn_CheckAuth.CheckAuth_User("101", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //判斷是否有上一頁暫存參數
                if (Session["BackListUrl"] == null)
                    Session["BackListUrl"] = Application["WebUrl"] + "Product/Prod_Search.aspx";

                //[取得/檢查參數] - Model_No (品號)
                Param_ModelNo = string.IsNullOrEmpty(Request.QueryString["Model_No"]) ? "" : Request.QueryString["Model_No"].ToString().Trim();
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤！", Session["BackListUrl"].ToString());
                    return;
                }

                //[帶出資料]
                LookupData();

                //[代入Ascx參數] - 目前頁籤
                Ascx_TabMenu1.Param_CurrItem = "1";
                //[代入Ascx參數] - 主檔編號
                Ascx_TabMenu1.Param_ModelNo = Param_ModelNo;
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
    /// 取得基本資料
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
                        Param_ModelNo = DT.Rows[0]["Model_No"].ToString().Trim();     //品號
                        this.lb_Item_No.Text = DT.Rows[0]["Item_No"].ToString();       //貨號
                        this.lb_Class_ID.Text = "{0} - {1}".FormatThis(DT.Rows[0]["Class_ID"].ToString(), DT.Rows[0]["Class_Name"].ToString());     //銷售類別
                        this.lb_Warehouse_Class_ID.Text = DT.Rows[0]["Warehouse_Class_Name"].ToString();     //倉管類別
                        this.lt_Ship_From.Text = DT.Rows[0]["Ship_From"].ToString();   //主要出貨地
                        this.lt_Date_Of_Listing.Text = DT.Rows[0]["Date_Of_Listing"].ToString().ToDateString_ERP("-");   //上市日期
                        this.lt_Stop_Offer_Date.Text = DT.Rows[0]["Stop_Offer_Date"].ToString().ToDateString("yyyy-MM-dd");   //停售日期
                        this.lt_Part_No.Text = DT.Rows[0]["Part_No"].ToString();   //子件型號
                        this.lt_Model_Name_zh_TW.Text = DT.Rows[0]["Model_Name_zh_TW"].ToString();   //品名(繁中)
                        this.lt_Model_Name_zh_CN.Text = DT.Rows[0]["Model_Name_zh_CN"].ToString();   //品名(簡中)
                        this.lt_Model_Name_en_US.Text = DT.Rows[0]["Model_Name_en_US"].ToString();   //品名(英文)
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

                            //case "SZ":
                            //    this.lt_Cases_Of_Failure_Date.Text = DT.Rows[0]["Cases_Of_Failure_Date_SZ"].ToString().ToDateString_ERP("-");
                            //    break;
                        }

                        //圖片
                        string PhotoGroup = DT.Rows[0]["PhotoGroup"].ToString();
                        if (string.IsNullOrEmpty(PhotoGroup))
                        {
                            this.lt_Photo.Text = "<img src=\"../images/NoPic.png\" width=\"220px\">";
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
                                    "<img src=\"{0}\" width=\"220px\" border=\"0\" rel=\"PicGroup\" class=\"PicGroup\" href=\"{1}\" style=\"cursor:pointer\">"
                                    , Param_WebFolder + Server.UrlEncode(Param_ModelNo) + "/1/500x500_" + Photo
                                    , Param_WebFolder + Server.UrlEncode(Param_ModelNo) + "/1/" + Photo);
                            }
                        }
                        #endregion

                        //填入共用欄位資料
                        #region -共用欄位資料-
                        this.lt_Pub_Patent_No.Text = DT.Rows[0]["Pub_Patent_No"].ToString();
                        this.lt_Pub_Standard1.Text = DT.Rows[0]["Pub_Standard1"].ToString();
                        this.lt_Pub_Standard2.Text = DT.Rows[0]["Pub_Standard2"].ToString();
                        this.lt_Pub_Standard3.Text = DT.Rows[0]["Pub_Standard3"].ToString();
                        this.lt_Pub_Logo.Text = fn_Desc.Prod.Logo(DT.Rows[0]["Pub_Logo"].ToString());
                        this.lt_Pub_Logo_Printing.Text = DT.Rows[0]["Pub_Logo_Printing"].ToString();

                        // - 產品單一裸重
                        this.lt_Pub_IP_Net_Weight.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_Net_Weight"].ToString(), 4);    //裸重(g) - 小數點4位
                        this.lt_Pub_PW_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_PW_L"].ToString(), 2);  //單一裸重.尺寸:長(cm) - 小數點2位
                        this.lt_Pub_PW_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_PW_W"].ToString(), 2);  //單一裸重.尺寸:寬(cm) - 小數點2位
                        this.lt_Pub_PW_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_PW_H"].ToString(), 2);  //單一裸重.尺寸:高(cm) - 小數點2位
                        this.lt_Pub_PW_Other.Text = DT.Rows[0]["Pub_PW_Other"].ToString().Replace("\r\n", "<BR/>");

                        // - Individual Packing
                        this.lt_Pub_Individual_Packing_zh_TW.Text = DT.Rows[0]["Pub_Individual_Packing_zh_TW"].ToString();
                        this.lt_Pub_Individual_Packing_en_US.Text = DT.Rows[0]["Pub_Individual_Packing_en_US"].ToString();
                        this.lt_Pub_IP_Weight.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_Weight"].ToString(), 3);    //最小包裝.單重 (Kg) - 小數點3位
                        this.lt_Pub_IP_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_L"].ToString(), 2);  //最小包裝.尺寸:長(cm) - 小數點2位
                        this.lt_Pub_IP_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_W"].ToString(), 2);  //最小包裝.尺寸:寬(cm) - 小數點2位
                        this.lt_Pub_IP_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IP_H"].ToString(), 2);  //最小包裝.尺寸:高(cm) - 小數點2位
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
                        this.lt_Pub_IB_NW.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_NW"].ToString(), 2);    //內箱淨重/Kg - 小數點2位
                        this.lt_Pub_IB_GW.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_GW"].ToString(), 2);    //內箱毛重/Kg - 小數點2位
                        this.lt_Pub_IB_L.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_L"].ToString(), 2);  //內箱尺寸:長(cm) - 小數點2位
                        this.lt_Pub_IB_W.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_W"].ToString(), 2);  //內箱尺寸:寬(cm) - 小數點2位
                        this.lt_Pub_IB_H.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_H"].ToString(), 2);  //內箱尺寸:高(cm) - 小數點2位
                        this.lt_Pub_IB_CUFT.Text = fn_stringFormat.Decimal_Format(DT.Rows[0]["Pub_IB_CUFT"].ToString(), 2);    //內箱材積(CUFT) - 小數點2位
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
                        this.lt_Pub_Recommended.Text = fn_Desc.PubAll.YesNo(DT.Rows[0]["Pub_Recommended"].ToString());
                        this.lt_Pub_Notes.Text = DT.Rows[0]["Pub_Notes"].ToString().Replace("\r\n", "<BR/>");
                        //this.lt_Pub_Features_Remark.Text = DT.Rows[0]["Pub_Features_Remark"].ToString().Replace("\r\n", "<BR/>");
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
                        Lookup_QCItems(DT.Rows[0]["Ship_From"].ToString()
                            , DT.Rows[0]["Pub_QC_Category"].ToString()
                            , DT.Rows[0]["Model_No"].ToString());

                        //理想用途
                        this.lt_Pub_Use.Text = Get_PubUseHtml(DT.Rows[0]["Pub_Use"].ToString());
                        //主件/配件
                        this.lt_Pub_Accessories.Text = fn_Desc.Prod.Accessories(DT.Rows[0]["Pub_Accessories"].ToString());
                        //選購配件型號
                        this.lt_Pub_Select_ModelNo.Text = Get_ModelNoHtml(DT.Rows[0]["Pub_Select_ModelNo"].ToString());
                        //對應主件型號
                        this.lt_Pub_Compare_ModelNo.Text = Get_ModelNoHtml(DT.Rows[0]["Pub_Compare_ModelNo"].ToString());

                        //帶出認證資料
                        LookupCertList();

                        //帶出規格符號
                        Lookup_Icons();

                        //帶出SOP下載
                        Lookup_SOP();

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
            throw;
        }

    }

    /// <summary>
    /// 帶出品管檢驗項目
    /// </summary>
    /// <param name="ShipFrom">主要出貨地</param>
    /// <param name="QC_Category">品管類別</param>
    /// <param name="ModelNo">品號</param>
    void Lookup_QCItems(string ShipFrom, string QC_Category, string ModelNo)
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
    void Lookup_QCPics()
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
    /// 帶出規格符號
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
            string IsShow = DataBinder.Eval(dataItem.DataItem, "RelID").ToString();
            Label lb_Disp = (Label)e.Item.FindControl("lb_Disp");
            if (string.IsNullOrEmpty(IsShow))
            {
                lb_Disp.Text = "(隱藏)";
                lb_Disp.CssClass = "styleGraylight";
            }
            else
            {
                lb_Disp.Text = "(顯示)";
                lb_Disp.CssClass = "styleBlue";
            }
        }
    }

    /// <summary>
    /// 帶出SOP檔案
    /// </summary>
    private void Lookup_SOP()
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
                SBSql.AppendLine(" SELECT SID, SOP_File, SOP_OrgFile, Create_Time ");
                SBSql.AppendLine(" FROM Prod_File_SOP ");
                SBSql.AppendLine(" WHERE (RTRIM(Model_No)= @Param_ModelNo) ");
                SBSql.AppendLine(" ORDER BY Create_Time DESC ");
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);

                //[SQL] - 取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lv_SOP.DataSource = DT.DefaultView;
                    this.lv_SOP.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 帶出SOP檔案！", "");
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

    #region -- 功能 --
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
            SBSql.AppendLine(" SELECT Param_Name FROM Param_Public ");
            SBSql.AppendLine(" WHERE (Param_Kind = '理想用途') ");
            SBSql.AppendLine(string.Format(" AND (Param_Value IN ({0}))"
                , string.Join(",", strAry)));
            //[SQL] - 執行
            cmd.CommandText = SBSql.ToString();
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                StringBuilder html = new StringBuilder();
                html.AppendLine("<ul>");

                for (int row = 0; row < DT.Rows.Count; row++)
                {

                    html.AppendLine("<li>" + DT.Rows[row]["Param_Name"].ToString() + "</li>");

                }

                html.AppendLine("</ul>");

                return html.ToString();
            }
        }
    }

    /// <summary>
    /// 回傳型號Html
    /// </summary>
    /// <param name="inputValue">輸入值</param>
    /// <returns></returns>
    private string Get_ModelNoHtml(string inputValue)
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
        html.AppendLine("<ul>");
        foreach (var item in query)
        {
            html.AppendLine("<li>" + item.Val + "</li>");
        }
        html.AppendLine("</ul>");

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
    /// [參數] - 認證Disk資料夾路徑
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
    /// [參數] - 資料夾路徑 (SOP)
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"SOP_PDF\" + Param_ModelNo + @"\";
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get;
        set;
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