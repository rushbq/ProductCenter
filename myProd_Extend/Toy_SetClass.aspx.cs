using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;

public partial class myProd_Extend_Toy_SetClass : SecurityIn
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

    private void LookupData()
    {
        //取出資料
        string modelNo = Req_DataID;

        //填入資料
        lt_nav.Text = modelNo;

        //表頭
        Page.Title = "[{0}] {1}".FormatThis(modelNo, Page.Title);

        //建立一階選單
        CreateMenu(ddl_Class);

        //載入分類清單
        LookupData_ClassMenu();

    }


    /// <summary>
    /// 取得已關聯分類清單
    /// </summary>
    private void LookupData_ClassMenu()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT Base.Class_ID AS ID");
            sql.AppendLine(" , Base.Class_Name_zh_TW AS Label_TW, Base.Class_Name_zh_CN AS Label_CN, Base.Class_Name_en_US AS Label_EN");
            sql.AppendLine(" FROM ProdToy_Class Base");
            sql.AppendLine("  INNER JOIN ProdToy_Class_Rel_ModelNo Rel ON Base.Class_ID = Rel.Class_ID");
            sql.AppendLine(" WHERE (UPPER(Rel.Model_No) = UPPER(@ID))");

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


    #endregion


    #region -- 資料編輯 --

    /// <summary>
    /// 新增分類
    /// </summary>
    protected void btn_Add_Click(object sender, EventArgs e)
    {
        //取得輸入值
        string _modelNo = Req_DataID;
        string _clsID = ddl_Class.SelectedValue;

        //Check null
        if (string.IsNullOrWhiteSpace(_clsID))
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
            sql.AppendLine(" SELECT Model_No");
            sql.AppendLine(" FROM ProdToy_Class_Rel_ModelNo");
            sql.AppendLine(" WHERE (UPPER(Model_No) = UPPER(@Model_No)) AND (Class_ID = @Class_ID)");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Model_No", _modelNo);
            cmd.Parameters.AddWithValue("Class_ID", _clsID);

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
            sql.AppendLine(" INSERT INTO ProdToy_Class_Rel_ModelNo( ");
            sql.AppendLine("  Model_No, Class_ID");
            sql.AppendLine(" ) VALUES (");
            sql.AppendLine("  @Model_No, @Class_ID");
            sql.AppendLine(" )");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("Model_No", _modelNo);
            cmd.Parameters.AddWithValue("Class_ID", _clsID);

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
            sql.AppendLine(" DELETE FROM ProdToy_Class_Rel_ModelNo");
            sql.AppendLine(" WHERE (Class_ID = @dataID)");

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

    /// <summary>
    /// 建立選單
    /// </summary>
    /// <param name="drp">控制項</param>
    private void CreateMenu(DropDownList drp)
    {
        //清空選項
        drp.Items.Clear();

        //取得資料
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料取得 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT Class_ID AS ID, Class_Name_zh_TW AS Label");
            sql.AppendLine(" FROM ProdToy_Class");
            sql.AppendLine(" ORDER BY Sort, Class_ID");

            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();

            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT != null)
                {
                    //LinQ 查詢
                    var _data = DT.AsEnumerable()
                        .Select(fld => new
                        {
                            ID = fld.Field<string>("ID"),
                            Label = fld.Field<string>("Label")
                        });

                    //建立子項
                    foreach (var item in _data)
                    {
                        drp.Items.Add(new ListItem("{0} - {1}".FormatThis(item.ID, item.Label), item.ID.ToString()));
                    }
                }
            }
        }


        //建立root item
        drp.Items.Insert(0, new ListItem("請選擇", ""));
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
            return "{0}myProd_Extend/Toy_SetClass.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Server.UrlEncode(Req_DataID));
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