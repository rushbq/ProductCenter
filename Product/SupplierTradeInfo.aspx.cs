using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SupplierData.Controllers;

public partial class Product_SupplierHistory : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("108", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //判斷品號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    return;
                }

                //Get Data
                LookupData();


            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _dataList = new SupplierRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetTradeLog(Req_DataID, Req_SupID, Req_Company);


        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();

        //Release
        query = null;
    }



    /// <summary>
    /// 資料編號
    /// </summary>
    private string _Req_DataID;
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["modelNo"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }


    private string _Req_SupID;
    public string Req_SupID
    {
        get
        {
            String data = Request.QueryString["supID"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_SupID = value;
        }
    }

    private string _Req_Company;
    public string Req_Company
    {
        get
        {
            String data = Request.QueryString["company"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Company = value;
        }
    }
}