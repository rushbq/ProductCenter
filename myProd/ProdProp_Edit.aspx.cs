using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using ProdAttributeData.Models;
using ProdAttributeData.Controllers;
using PKLib_Method.Methods;
using System.Data;

public partial class ProdProp_Edit : SecurityIn
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            #region --權限--
            //[權限判斷] Start
            if (fn_CheckAuth.CheckAuth_User("160", out ErrMsg) == false)
            {
                Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                return;
            }

            //[權限判斷] End
            #endregion

            if (!IsPostBack)
            {
                //無資料,自動新增
                if (string.IsNullOrWhiteSpace(Req_DataID))
                {
                    Add_Data(Req_ItemNo);
                    return;
                }

                //取得選單(銷售=2, 生管採購=3, 倉管=4 ; 新品=A)
                Get_ClassList("A", ddl_NewProdClass, "請選擇", "");
                Get_ClassList("2", ddl_SaleClass, "請選擇", "");
                Get_ClassList("3", ddl_PurClass, "請選擇", "");
                Get_ClassList("4", ddl_WareHouseClass, "請選擇", "");

                //載入資料
                LookupData();

            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料顯示:基本資料 --

    /// <summary>
    /// 取得基本資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        ProdItemPropRespository _data = new ProdItemPropRespository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----
        search.Add("DataID", Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetOne_ProdAttr(search, out ErrMsg);

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            CustomExtension.AlertMsg("無法取得資料,即將返回列表頁.", Page_SearchUrl);
            return;
        }
        if (query.Rows.Count == 0)
        {
            CustomExtension.AlertMsg("無法取得資料,請檢查ERP品號資料(如:主要出貨地).", Page_SearchUrl);
            return;
        }

        #region >> 欄位填寫 <<

        //--- 填入基本資料 ---
        string _modelNo = query.Rows[0]["ModelNo"].ToString();
        string _itemNo = query.Rows[0]["Item_No"].ToString();

        lb_ModelNo.Text = _modelNo;
        lt_ItemNo.Text = _itemNo;
        hf_ItemNo.Value = _itemNo;
        lt_Photo.Text = "<img src=\"{0}EcImg/500/{1}/\" alt=\"{1}\" />".FormatThis(fn_Param.ApiUrl, _modelNo); //Api Pic Url
        lb_ShipFrom.Text = query.Rows[0]["ShipFrom"].ToString();
        tb_OnlineDate.Text = query.Rows[0]["OnlineDate"].ToString().ToDateString_ERP("/");
        tb_StopDate.Text = query.Rows[0]["StopDate"].ToString().ToDateString_ERP("/");
        tb_CCCode.Text = query.Rows[0]["CCCode"].ToString();

        ddl_NewProdClass.SelectedValue = query.Rows[0]["NewProdClass"].ToString();
        ddl_SaleClass.SelectedValue = query.Rows[0]["SaleClass"].ToString();
        ddl_PurClass.SelectedValue = query.Rows[0]["PurClass"].ToString();
        ddl_WareHouseClass.SelectedValue = query.Rows[0]["WareHouseClass"].ToString();

        hf_DataID.Value = query.Rows[0]["Data_ID"].ToString();

        #endregion

        //維護資訊
        info_Creater.Text = query.Rows[0]["Create_Name"].ToString();
        info_CreateTime.Text = query.Rows[0]["Create_Time"].ToString();
        info_Updater.Text = query.Rows[0]["Update_Name"].ToString();
        info_UpdateTime.Text = query.Rows[0]["Update_Time"].ToString();

        //-- 載入其他資料 --
        LookupData_Models(_itemNo);


        //Release
        _data = null;

    }


    /// <summary>
    /// 取得類別
    /// </summary>
    /// <param name="clsType">銷售=2, 生管採購=3, 倉管=4</param>
    /// <param name="ddl"></param>
    /// <param name="rootName"></param>
    /// <param name="inputValue"></param>
    private void Get_ClassList(string clsType, DropDownList ddl, string rootName, string inputValue)
    {
        //----- 宣告:資料參數 -----
        ProdItemPropRespository _data = new ProdItemPropRespository();

        //----- 原始資料:取得所有資料 -----
        DataTable query = _data.Get_ErpClass(clsType, out ErrMsg);

        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem(rootName, ""));
        }

        for (int row = 0; row < query.Rows.Count; row++)
        {
            ddl.Items.Add(new ListItem(
                query.Rows[row]["id"].ToString() + " - " + query.Rows[row]["label"].ToString()
                , query.Rows[row]["id"].ToString()
                ));
        }

        //被選擇值
        if (!string.IsNullOrWhiteSpace(inputValue))
        {
            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(inputValue));
        }

        query = null;
    }


    #endregion


    #region -- 資料編輯:基本資料 --

    //Save
    protected void btn_doSave_Click(object sender, EventArgs e)
    {
        Edit_Data();
    }

    /// <summary>
    /// 資料新增
    /// </summary>
    private void Add_Data(string _itemNo)
    {
        //----- 宣告:資料參數 -----
        ProdItemPropRespository _data = new ProdItemPropRespository();

        try
        {
            //----- 方法:新增資料 -----
            string _id = _data.Check_ProdAttr(_itemNo, out ErrMsg);

            if (string.IsNullOrWhiteSpace(_id))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "<b>資料新增失敗</b><p>{0}</p><p>{1}</p>".FormatThis("遇到無法排除的錯誤，請聯絡系統管理員。", ErrMsg);

                CustomExtension.AlertMsg("新增失敗", "");
                return;
            }

            //更新本頁Url
            string thisUrl = "{0}ProdProp_Edit.aspx?id={1}".FormatThis(FuncPath(), _id);

            //Redirect
            Response.Redirect(thisUrl);
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }

    /// <summary>
    /// 需求資料:資料修改
    /// </summary>
    private void Edit_Data()
    {
        //----- 宣告:資料參數 -----
        ProdItemPropRespository _data = new ProdItemPropRespository();

        try
        {
            //----- 設定:資料欄位 -----
            string _id = hf_DataID.Value;
            var data = new ItemProp
            {
                OnlineDate = tb_OnlineDate.Text.ToDateString("yyyyMMdd"),
                StopDate = tb_StopDate.Text.ToDateString("yyyyMMdd"),
                NewProdClass = ddl_NewProdClass.SelectedValue,
                SaleClass = ddl_SaleClass.SelectedValue,
                PurClass = ddl_PurClass.SelectedValue,
                WareHouseClass = ddl_WareHouseClass.SelectedValue,
                CCCode = tb_CCCode.Text
            };


            //----- 方法:修改資料 -----
            if (!_data.Update_ProdAttr(_id, data, out ErrMsg))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "<b>資料修改失敗</b><p>{0}</p><p>{1}</p>".FormatThis("遇到無法排除的錯誤，請聯絡系統管理員。", ErrMsg);

                CustomExtension.AlertMsg("修改失敗", "");
                return;
            }


            //導向本頁
            CustomExtension.AlertMsg("修改成功", thisPage);
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _data = null;
        }

    }

    #endregion


    #region -- 資料顯示:關聯品號 --

    /// <summary>
    /// 顯示關聯品號
    /// </summary>
    private void LookupData_Models(string _itemNo)
    {
        //----- 宣告:資料參數 -----
        ProdItemPropRespository _data = new ProdItemPropRespository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.Get_Models(_itemNo, out ErrMsg);

        //----- 資料整理:繫結 ----- 
        lv_Models.DataSource = query;
        lv_Models.DataBind();

        //Release
        query = null;
        _data = null;
    }

    #endregion


    #region -- 按鈕事件 --

    #endregion


    #region -- 網址參數 --
    /// <summary>
    /// 取得此功能的前置路徑
    /// </summary>
    /// <returns></returns>
    public string FuncPath()
    {
        return "{0}myProd/".FormatThis(fn_Param.WebUrl);
    }

    #endregion


    #region -- 傳遞參數 --
    /// <summary>
    /// 取得傳遞參數 - 貨號
    /// </summary>
    private string _Req_ItemNo;
    public string Req_ItemNo
    {
        get
        {
            String data = Request.QueryString["itemNo"];

            return data;
        }
        set
        {
            _Req_ItemNo = value;
        }
    }


    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String DataID = Request.QueryString["id"];

            return DataID;
        }
        set
        {
            _Req_DataID = value;
        }
    }

    /// <summary>
    /// 本頁網址
    /// </summary>
    private string _thisPage;
    public string thisPage
    {
        get
        {
            return "{0}ProdProp_Edit.aspx?id={1}".FormatThis(FuncPath(), Req_DataID);
        }
        set
        {
            _thisPage = value;
        }
    }


    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            string tempUrl = CustomExtension.getCookie("ProdProp");

            return string.IsNullOrWhiteSpace(tempUrl) ? FuncPath() + "ProdProp_Search.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            _Page_SearchUrl = value;
        }
    }

    #endregion

}