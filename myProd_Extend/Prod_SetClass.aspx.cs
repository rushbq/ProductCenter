using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using CatalogMenuData.Controllers;
using PKLib_Method.Methods;

public partial class myProd_Extend_Prod_SetClass : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("113", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //載入資料
                LookupData();

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料讀取 --
    /// <summary>
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT RTRIM(Base.Model_No) AS ModelNo, Base.Model_Name_zh_TW AS ModelName, Base.Class_ID");
            sql.AppendLine(" , Rel.Rel_ID, Rel.StyleID");
            sql.AppendLine(" , (SELECT COUNT(*) FROM [PKCatalog].dbo.Prod_Rel_Menu WHERE (ModelNo = Base.Model_No)) AS MenuCnt");
            sql.AppendLine(" FROM Prod_Item Base");
            sql.AppendLine("  LEFT JOIN [PKCatalog].dbo.Prod_Rel_Style Rel ON Base.Model_No = Rel.ModelNo");
            sql.AppendLine(" WHERE (UPPER(Base.Model_No) = UPPER(@ID))");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("ID", Req_DataID);

            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    CustomExtension.AlertMsg("無法取得資料,即將返回上一頁.", backPage);
                    return;
                }

                //取出資料
                string modelNo = DT.Rows[0]["ModelNo"].ToString();
                string modelName = DT.Rows[0]["ModelName"].ToString();
                string classID = DT.Rows[0]["Class_ID"].ToString();
                string styleID = DT.Rows[0]["StyleID"].ToString();
                int menuCnt = Convert.ToInt32(DT.Rows[0]["MenuCnt"]);

                //填入資料
                lt_nav.Text = modelNo;
                ddl_Style.SelectedValue = styleID;
                hf_classID.Value = classID;

                //警示
                int errcnt = 0;
                if (string.IsNullOrWhiteSpace(styleID))
                {
                    lt_ShowMsg.Text += "款式未設定";
                    errcnt++;
                }
                if (menuCnt == 0)
                {
                    lt_ShowMsg.Text += "分類階層未設定";
                    errcnt++;
                }
                if (errcnt > 0)
                {
                    ph_ErrMessage.Visible = true;
                }

                //表頭
                Page.Title = "[{0}] {1}".FormatThis(modelNo, Page.Title);

                //建立一階選單
                CreateMenu(ddl_Lv1, "1", classID, "");

                //載入分類清單
                LookupData_ClassMenu();
            }
        }

    }


    /// <summary>
    /// 取得分類階層清單
    /// </summary>
    private void LookupData_ClassMenu()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT RelMenu.Rel_ID");
            sql.AppendLine(" , RelMenu.Menu_Lv1, RelMenu.Menu_Lv2, RelMenu.Menu_Lv3");
            sql.AppendLine(" , (SELECT MenuName_en_US FROM [PKCatalog].dbo.Catalog_Menu WITH(NOLOCK) WHERE Menu_ID = RelMenu.Menu_Lv1) AS MenuNameLv1");
            sql.AppendLine(" , (SELECT MenuName_en_US FROM [PKCatalog].dbo.Catalog_Menu WITH(NOLOCK) WHERE Menu_ID = RelMenu.Menu_Lv2) AS MenuNameLv2");
            sql.AppendLine(" , (SELECT MenuName_en_US FROM [PKCatalog].dbo.Catalog_Menu WITH(NOLOCK) WHERE Menu_ID = RelMenu.Menu_Lv3) AS MenuNameLv3");
            sql.AppendLine(" , RelMenu.SortType");
            sql.AppendLine(" FROM [PKCatalog].dbo.Prod_Rel_Menu RelMenu");
            sql.AppendLine(" WHERE (UPPER(RelMenu.ModelNo) = UPPER(@ID))");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("ID", Req_DataID);

            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                lvDataList.DataSource = DT.DefaultView;
                lvDataList.DataBind();
            }
        }
    }


    /// <summary>
    /// 排序方式名稱
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string getTypeName(string type)
    {
        switch (type)
        {
            case "1":
                return "依品號";

            default:
                return "依成本價";
        }
    }

    #endregion


    #region -- 資料編輯 --

    /// <summary>
    /// 新增分類
    /// </summary>
    protected void btn_Add_Click(object sender, EventArgs e)
    {
        //取得輸入值
        string _modelNo = Req_DataID;
        string _classID = hf_classID.Value;
        string _lv1 = ddl_Lv1.SelectedValue;
        string _lv2 = ddl_Lv2.SelectedValue;
        string _lv3 = ddl_Lv3.SelectedValue;
        string _sort = ddl_sort.SelectedValue;

        //Check null
        if (string.IsNullOrWhiteSpace(_lv1) || string.IsNullOrWhiteSpace(_lv2) || string.IsNullOrWhiteSpace(_lv3))
        {
            CustomExtension.AlertMsg("未正確選擇分類，請重新確認!", "");
            return;
        }

        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            #region ** 判斷重複 **

            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT Rel_ID");
            sql.AppendLine(" FROM [PKCatalog].dbo.Prod_Rel_Menu");
            sql.AppendLine(" WHERE (UPPER(ModelNo) = UPPER(@ModelNo))");
            sql.AppendLine(" AND (Menu_Lv1 = @Menu_Lv1) AND (Menu_Lv2 = @Menu_Lv2) AND (Menu_Lv3 = @Menu_Lv3)");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("ModelNo", _modelNo);
            cmd.Parameters.AddWithValue("Menu_Lv1", _lv1);
            cmd.Parameters.AddWithValue("Menu_Lv2", _lv2);
            cmd.Parameters.AddWithValue("Menu_Lv3", _lv3);

            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count > 0)
                {
                    CustomExtension.AlertMsg("分類重複新增，請重新確認!", "");
                    return;
                }
            }

            #endregion

            #region ** 執行新增 **

            //reset
            cmd.Parameters.Clear();
            sql.Clear();

            //----- SQL 查詢語法 -----
            sql.AppendLine(" DECLARE @NewID AS INT ");
            sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(Rel_ID), 0) + 1 FROM [PKCatalog].dbo.Prod_Rel_Menu);");
            sql.AppendLine(" INSERT INTO [PKCatalog].dbo.Prod_Rel_Menu( ");
            sql.AppendLine("  Rel_ID, ModelNo, Menu_Lv1, Menu_Lv2, Menu_Lv3");
            sql.AppendLine("  , Class_ID, SortType");
            sql.AppendLine(" ) VALUES (");
            sql.AppendLine("  @NewID, @ModelNo, @Menu_Lv1, @Menu_Lv2, @Menu_Lv3");
            sql.AppendLine("  , @Class_ID, @SortType");
            sql.AppendLine(" )");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("ModelNo", _modelNo);
            cmd.Parameters.AddWithValue("Menu_Lv1", _lv1);
            cmd.Parameters.AddWithValue("Menu_Lv2", _lv2);
            cmd.Parameters.AddWithValue("Menu_Lv3", _lv3);
            cmd.Parameters.AddWithValue("Class_ID", _classID);
            cmd.Parameters.AddWithValue("SortType", _sort);

            if (!dbConClass.ExecuteSql(cmd, out ErrMsg))
            {
                CustomExtension.AlertMsg("新增失敗", "");
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(thisPage);
            }

            #endregion

        }

    }

    /// <summary>
    /// 款式設定
    /// </summary>
    protected void btn_SaveStyle_Click(object sender, EventArgs e)
    {
        //取得輸入值
        string _modelNo = Req_DataID;
        string _style = ddl_Style.SelectedValue;

        //Check null
        if (string.IsNullOrWhiteSpace(_style))
        {
            CustomExtension.AlertMsg("未正確選擇款式，請重新確認!", "");
            return;
        }

        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" DELETE FROM [PKCatalog].[dbo].Prod_Rel_Style");
            sql.AppendLine(" WHERE (UPPER(ModelNo) = UPPER(@ModelNo));");

            sql.AppendLine(" DECLARE @NewID AS INT ");
            sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(Rel_ID), 0) + 1 FROM [PKCatalog].dbo.Prod_Rel_Style);");
            sql.AppendLine(" INSERT INTO [PKCatalog].[dbo].Prod_Rel_Style (");
            sql.AppendLine(" Rel_ID, ModelNo, StyleID");
            sql.AppendLine(" ) VALUES (");
            sql.AppendLine(" @NewID, @ModelNo, @StyleID");
            sql.AppendLine(" )");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("ModelNo", _modelNo);
            cmd.Parameters.AddWithValue("StyleID", _style);

            if (!dbConClass.ExecuteSql(cmd, out ErrMsg))
            {
                CustomExtension.AlertMsg("設定失敗", "");
                return;
            }
            else
            {
                //導向本頁
                CustomExtension.AlertMsg("設定成功", thisPage);
                return;
            }
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        //取得Key值
        string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;


        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 語法 -----
            sql.AppendLine(" DELETE FROM [PKCatalog].dbo.Prod_Rel_Menu");
            sql.AppendLine(" WHERE (Rel_ID = @dataID)");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("dataID", Get_DataID);

            if (!dbConClass.ExecuteSql(cmd, out ErrMsg))
            {
                CustomExtension.AlertMsg("刪除失敗", thisPage);
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(thisPage);
            }
        }
    }

    #endregion


    #region -- 附加功能 --

    protected void ddl_Lv1_SelectedIndexChanged(object sender, EventArgs e)
    {
        //呼叫二階、清除三階
        CreateMenu(ddl_Lv2, "2", "", ddl_Lv1.SelectedValue);
        //ResetMenu(ddl_Lv3);
    }
    protected void ddl_Lv2_SelectedIndexChanged(object sender, EventArgs e)
    {
        //呼叫三階
        CreateMenu(ddl_Lv3, "3", "", ddl_Lv2.SelectedValue);
    }

    /// <summary>
    /// 建立選單
    /// </summary>
    /// <param name="drp">控制項</param>
    /// <param name="_lv">階層</param>
    /// <param name="_cid">銷售類別</param>
    /// <param name="_pid">上層編號</param>
    private void CreateMenu(DropDownList drp, string _lv, string _cid, string _pid)
    {
        //清空選項
        drp.Items.Clear();

        //取得資料
        var data = GetMenuList(_lv, _cid, _pid);

        //建立子項
        foreach (var item in data)
        {
            drp.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        //建立root item
        drp.Items.Insert(0, new ListItem("請選擇", ""));
    }

    /// <summary>
    /// 重置選單
    /// </summary>
    /// <param name="drp">控制項</param>
    private void ResetMenu(DropDownList drp)
    {
        drp.Items.Clear();
    }


    /// <summary>
    /// 取得選單資料
    /// </summary>
    /// <param name="_lv">階層</param>
    /// <param name="_cid">銷售類別</param>
    /// <param name="_pid">上層編號</param>
    /// <returns></returns>
    private IQueryable<CatalogMenuData.Models.MenuItem> GetMenuList(string _lv, string _cid, string _pid)
    {
        //----- 宣告:資料參數 -----
        MenuRepository _data = new MenuRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----
        //[取得/檢查參數] - Level
        if (!string.IsNullOrWhiteSpace(_lv))
        {
            search.Add("Level", _lv);
        }

        //[取得/檢查參數] - ClassID
        if (!string.IsNullOrWhiteSpace(_cid))
        {
            search.Add("ClassID", _cid);
        }

        //[取得/檢查參數] - ParentID
        if (!string.IsNullOrWhiteSpace(_pid))
        {
            search.Add("ParentID", _pid);
        }

        //----- 原始資料:取得所有資料 -----
        var results = _data.GetList("TW", search, out ErrMsg);

        //release
        _data = null;

        return results;

    }

    #endregion


    #region -- 傳遞參數 --

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    public string thisPage
    {
        get
        {
            return "{0}myProd_Extend/Prod_SetClass.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Server.UrlEncode(Req_DataID));
        }
        set
        {
            this._thisPage = value;
        }
    }
    private string _thisPage;


    /// <summary>
    /// 設定參數 - 上頁Url
    /// </summary>
    public string backPage
    {
        get
        {
            return "{0}Product/Prod_Edit.aspx?Model_No={1}".FormatThis(fn_Param.WebUrl, Server.UrlEncode(Req_DataID));
        }
        set
        {
            this._backPage = value;
        }
    }
    private string _backPage;


    /// <summary>
    /// 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }

    #endregion

}