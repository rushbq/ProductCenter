using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ExtensionMethods;

public partial class myProd_ProdCopy : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("111", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //讀取Log
                GetLog();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    protected void lbtn_Copy_Click(object sender, EventArgs e)
    {
        //檢查欄位
        StringBuilder alert = new StringBuilder();

        string srcItem = this.hf_src_ModelNo.Value;
        if (string.IsNullOrEmpty(this.hf_src_ModelNo.Value))
        {
            alert.Append("來源品號空白\\n");
        }

        string tarItem = this.hf_tar_ModelNo.Value;
        if (string.IsNullOrEmpty(this.hf_tar_ModelNo.Value))
        {
            alert.Append("目標品號空白\\n");
        }

        var getCopyItems = from ListItem item in this.cbl_Option.Items
                           where item.Selected
                           select item.Value;
        if (getCopyItems.Count() == 0)
        {
            alert.Append("未選擇複製選項\\n");
        }
        //[JS] - 判斷是否有警示訊息
        if (!string.IsNullOrEmpty(alert.ToString()))
        {
            fn_Extensions.JsAlert(alert.ToString(), "");
            return;
        }

        //jobs
        #region -- 執行複製功能 --

        alert.Clear();

        foreach (var item in getCopyItems)
        {
            switch (item)
            {
                case "1":
                    if (job1(srcItem, tarItem))
                    {
                        Create_Log(srcItem, tarItem, "基本資料");
                    }
                    else
                    {
                        alert.Append("複製失敗-基本資料\\n");
                    }

                    break;

                case "2":
                    if (job2(srcItem, tarItem))
                    {
                        Create_Log(srcItem, tarItem, "規格明細");
                    }
                    else
                    {
                        alert.Append("複製失敗-規格明細\\n");
                    }

                    break;

                case "3":
                    if (job3(srcItem, tarItem))
                    {
                        Create_Log(srcItem, tarItem, "PDF匯出設定");
                    }
                    else
                    {
                        alert.Append("複製失敗-PDF匯出設定\\n");
                    }

                    break;

                default:
                    break;
            }
        }
        if (!string.IsNullOrEmpty(alert.ToString()))
        {
            fn_Extensions.JsAlert(alert.ToString(), PageUrl);
        }
        else
        {
            fn_Extensions.JsAlert("{0} 資料複製成功!!!".FormatThis(tarItem), PageUrl);
        }

        #endregion

    }



    /// <summary>
    /// 複製 - 基本資料
    /// </summary>
    /// <param name="srcItem">來源品號</param>
    /// <param name="tarItem">目標品號</param>
    /// <returns></returns>
    private bool job1(string srcItem, string tarItem)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine("UPDATE Prod_Item ");
            sql.AppendLine("SET SpecClassID = SrcItem.SpecClassID ");
            sql.AppendLine(", Pub_Patent_No = SrcItem.Pub_Patent_No ");
            sql.AppendLine(", Pub_Standard1 = SrcItem.Pub_Standard1 ");
            sql.AppendLine(", Pub_Standard2 = SrcItem.Pub_Standard2 ");
            sql.AppendLine(", Pub_Standard3 = SrcItem.Pub_Standard3 ");
            sql.AppendLine(", Pub_Logo = SrcItem.Pub_Logo ");
            sql.AppendLine(", Pub_Logo_Printing = SrcItem.Pub_Logo_Printing ");
            sql.AppendLine(", Pub_PW_L = SrcItem.Pub_PW_L ");
            sql.AppendLine(", Pub_PW_W = SrcItem.Pub_PW_W ");
            sql.AppendLine(", Pub_PW_H = SrcItem.Pub_PW_H ");
            sql.AppendLine(", Pub_PW_Other = SrcItem.Pub_PW_Other ");
            sql.AppendLine(", Pub_IP_L = SrcItem.Pub_IP_L ");
            sql.AppendLine(", Pub_IP_W = SrcItem.Pub_IP_W ");
            sql.AppendLine(", Pub_IP_H = SrcItem.Pub_IP_H ");
            sql.AppendLine(", Pub_IB_NW = SrcItem.Pub_IB_NW ");
            sql.AppendLine(", Pub_IB_GW = SrcItem.Pub_IB_GW ");
            sql.AppendLine(", Pub_IB_L = SrcItem.Pub_IB_L ");
            sql.AppendLine(", Pub_IB_W = SrcItem.Pub_IB_W ");
            sql.AppendLine(", Pub_IB_H = SrcItem.Pub_IB_H ");
            sql.AppendLine(", Pub_Recommended = SrcItem.Pub_Recommended ");
            sql.AppendLine(", Pub_Features_Remark = SrcItem.Pub_Features_Remark ");
            sql.AppendLine(", Pub_Use = SrcItem.Pub_Use ");
            sql.AppendLine(", Pub_Accessories = SrcItem.Pub_Accessories ");
            sql.AppendLine(", Pub_Select_ModelNo = SrcItem.Pub_Select_ModelNo ");
            sql.AppendLine(", Pub_Compare_ModelNo = SrcItem.Pub_Compare_ModelNo ");
            sql.AppendLine(", PDF_Type = SrcItem.PDF_Type ");
            sql.AppendLine("FROM ");
            sql.AppendLine(" ( ");
            sql.AppendLine("  SELECT SpecClassID ");
            sql.AppendLine("   , Pub_Patent_No, Pub_Standard1, Pub_Standard2, Pub_Standard3, Pub_Logo, Pub_Logo_Printing ");
            sql.AppendLine("   , Pub_PW_L, Pub_PW_W, Pub_PW_H, Pub_PW_Other, Pub_IP_L, Pub_IP_W, Pub_IP_H ");
            sql.AppendLine("   , Pub_IB_NW, Pub_IB_GW, Pub_IB_L, Pub_IB_W, Pub_IB_H ");
            sql.AppendLine("   , Pub_Recommended, Pub_Features_Remark, Pub_Use ");
            sql.AppendLine("   , Pub_Accessories, Pub_Select_ModelNo, Pub_Compare_ModelNo, PDF_Type ");
            sql.AppendLine("  FROM Prod_Item ");
            sql.AppendLine("  WHERE (Model_No = @Source_ID) ");
            sql.AppendLine(" ) AS SrcItem ");
            sql.AppendLine("WHERE Model_No = @Target_ID ");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Source_ID", srcItem);
            cmd.Parameters.AddWithValue("Target_ID", tarItem);


            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }


    /// <summary>
    /// 複製 - 規格明細
    /// </summary>
    /// <param name="srcItem">來源品號</param>
    /// <param name="tarItem">目標品號</param>
    /// <returns></returns>
    private bool job2(string srcItem, string tarItem)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            /* 規格明細 */
            sql.AppendLine("IF (SELECT COUNT(*) FROM Prod_Spec_List WHERE (Model_No = @Source_ID)) > 0 ");
            sql.AppendLine("BEGIN ");
            sql.AppendLine(" --//清除目標資料 (Prod_Item_Rel_Spec)(Prod_Spec_List) ");
            sql.AppendLine(" DELETE FROM Prod_Item_Rel_Spec WHERE (Model_No = @Target_ID) ");
            sql.AppendLine(" DELETE FROM Prod_Spec_List WHERE (Model_No = @Target_ID) ");

            sql.AppendLine(" --//新增目標資料 (Prod_Item_Rel_Spec) ");
            sql.AppendLine(" INSERT INTO Prod_Item_Rel_Spec( ");
            sql.AppendLine("    Model_No, SpecID ");
            sql.AppendLine(" ) ");
            sql.AppendLine(" SELECT @Target_ID, SpecID ");
            sql.AppendLine(" FROM Prod_Item_Rel_Spec ");
            sql.AppendLine(" WHERE (Model_No = @Source_ID) ");

            sql.AppendLine(" --//新增目標資料 (Prod_Spec_List) ");
            sql.AppendLine(" INSERT INTO Prod_Spec_List( ");
            sql.AppendLine("    Spec_ListID, Model_No, CateID, SpecClassID, SpecID, ListSymbol, ListValue ");
            sql.AppendLine(" ) ");
            sql.AppendLine(" SELECT Spec_ListID, @Target_ID, CateID, SpecClassID, SpecID, ListSymbol, ListValue ");
            sql.AppendLine(" FROM Prod_Spec_List ");
            sql.AppendLine(" WHERE (Model_No = @Source_ID) ");

            sql.AppendLine("END ");


            /* 組合明細 */
            sql.AppendLine("IF (SELECT COUNT(*) FROM Prod_BOMSpec_List WHERE (Model_No = @Source_ID)) > 0 ");
            sql.AppendLine("BEGIN ");
            sql.AppendLine(" --//清除目標資料 (Prod_BOMSpec_List) ");
            sql.AppendLine(" DELETE FROM Prod_BOMSpec_List WHERE (Model_No = @Target_ID) ");

            sql.AppendLine(" --//新增目標資料 (Prod_BOMSpec_List) ");
            sql.AppendLine(" INSERT INTO Prod_BOMSpec_List( ");
            sql.AppendLine("    Spec_ListID, Model_No, CateID, SpecClassID, SpecID, BOM_SpecID, RowID, ListSymbol, ListValue, Sort ");
            sql.AppendLine(" ) ");
            sql.AppendLine(" SELECT ");
            sql.AppendLine("    Spec_ListID, @Target_ID, CateID, SpecClassID, SpecID, BOM_SpecID, RowID, ListSymbol, ListValue, Sort ");
            sql.AppendLine(" FROM Prod_BOMSpec_List ");
            sql.AppendLine(" WHERE (Model_No = @Source_ID) ");

            sql.AppendLine("END ");


            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Source_ID", srcItem);
            cmd.Parameters.AddWithValue("Target_ID", tarItem);


            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }


    /// <summary>
    /// 複製 - PDF匯出設定(by Item)
    /// </summary>
    /// <param name="srcItem">來源品號</param>
    /// <param name="tarItem">目標品號</param>
    /// <returns></returns>
    private bool job3(string srcItem, string tarItem)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine("IF (SELECT COUNT(*) FROM Prod_PDF_byItem WHERE (Model_No = @Source_ID)) > 0 ");
            sql.AppendLine("BEGIN ");
            sql.AppendLine(" --//清除目標資料 (Prod_PDF_byItem) ");
            sql.AppendLine(" DELETE FROM Prod_PDF_byItem WHERE (Model_No = @Target_ID) ");

            sql.AppendLine(" --//新增目標資料 (Prod_PDF_byItem) ");
            sql.AppendLine(" INSERT INTO Prod_PDF_byItem( ");
            sql.AppendLine("    Model_No, CateID, SpecClassID, SpecID ");
            sql.AppendLine(" ) ");
            sql.AppendLine(" SELECT @Target_ID ,CateID, SpecClassID, SpecID ");
            sql.AppendLine(" FROM Prod_PDF_byItem ");
            sql.AppendLine(" WHERE (Model_No = @Source_ID) ");

            sql.AppendLine("END ");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Source_ID", srcItem);
            cmd.Parameters.AddWithValue("Target_ID", tarItem);


            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }


    /// <summary>
    /// Log
    /// </summary>
    /// <param name="srcItem">來源品號</param>
    /// <param name="tarItem">目標品號</param>
    /// <param name="typeName">複製選項</param>
    /// <returns></returns>
    private bool Create_Log(string srcItem, string tarItem, string typeName)
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" DECLARE @NewID AS VARCHAR(38)");
            sql.AppendLine(" SET @NewID = (SELECT CONVERT(VARCHAR(38), NEWID()))");
            sql.AppendLine(" INSERT INTO ProdCopy_Log( ");
            sql.AppendLine("  LogID, ModelNo, LogDesc, LogWho, LogTime");
            sql.AppendLine(" ) VALUES (");
            sql.AppendLine("  @NewID, @ModelNo, @LogDesc, @LogWho, GETDATE()");
            sql.AppendLine(" )");


            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("ModelNo", tarItem);
            cmd.Parameters.AddWithValue("LogDesc", "來源品號:{0}, 目標品號:{1}, 項目:{2}".FormatThis(
                srcItem, tarItem, typeName));
            cmd.Parameters.AddWithValue("LogWho", fn_Param.CurrentUser.ToString());


            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }



    /// <summary>
    /// 取得Log
    /// </summary>
    public void GetLog()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();


        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT TOP 10 Base.LogDesc, Base.LogTime");
            sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.LogWho)) AS LogWho");
            sql.AppendLine(" FROM ProdCopy_Log Base WITH(NOLOCK)");
            sql.AppendLine(" ORDER BY Base.LogTime DESC");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();


            //----- 資料取得 -----
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                this.lv_Log.DataSource = DT.DefaultView;
                this.lv_Log.DataBind();
            }

        }
    }


    /// <summary>
    /// [參數] - 本頁路徑
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}myProd/ProdCopy.aspx".FormatThis(Application["WebUrl"]);
        }
        set
        {
            this._PageUrl = value;
        }
    }
}