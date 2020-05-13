using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
                LookupStock();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --

    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        SupplierRepository _dataList = new SupplierRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetDataList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();

        //Release
        query = null;
    }


    /// <summary>
    /// 顯示庫存
    /// </summary>
    private void LookupStock()
    {
        using (DataTable dtStock = GetStock(Req_DataID))
        {
            if (dtStock != null)
            {
                //TW
                var queryTW = dtStock.AsEnumerable()
                    .Where(stock => stock.Field<string>("DBS").ToUpper().Equals("PROKIT2"))
                    .Where(stock => stock.Field<string>("StockType").Equals("01"))
                    .Select(stock =>
                        new
                        {
                            INV_Num = stock.Field<decimal>("INV_Num"),
                            INV_PreOut = stock.Field<decimal>("INV_PreOut"),
                            INV_Safe = stock.Field<decimal>("INV_Safe"),
                            INV_PreIn = stock.Field<decimal>("INV_PreIn"),
                            StockNum = stock.Field<decimal>("StockNum")
                        })
                        .FirstOrDefault();
                if (queryTW != null)
                {
                    this.lt_INV_Num_TW.Text = fn_stringFormat.Money_Format(queryTW.INV_Num.ToString());       //[庫存]
                    this.lt_INV_PreOut_TW.Text = fn_stringFormat.Money_Format(queryTW.INV_PreOut.ToString()); //[預計銷]
                    this.lt_INV_Safe_TW.Text = fn_stringFormat.Money_Format(queryTW.INV_Safe.ToString());     //[安全存量]
                    this.lt_INV_PreIn_TW.Text = fn_stringFormat.Money_Format(queryTW.INV_PreIn.ToString());   //[預計進]
                    this.lt_Stock_TW.Text = fn_stringFormat.Money_Format(queryTW.StockNum.ToString());        //[庫存可用量]
                }

                //SH
                var querySH = dtStock.AsEnumerable()
                    .Where(stock => stock.Field<string>("DBS").ToUpper().Equals("SHPK2"))
                    .Where(stock => stock.Field<string>("StockType").Equals("12"))
                    .Select(stock =>
                        new
                        {
                            INV_Num = stock.Field<decimal>("INV_Num"),
                            INV_PreOut = stock.Field<decimal>("INV_PreOut"),
                            INV_Safe = stock.Field<decimal>("INV_Safe"),
                            INV_PreIn = stock.Field<decimal>("INV_PreIn"),
                            StockNum = stock.Field<decimal>("StockNum")
                        })
                        .FirstOrDefault();
                if (querySH != null)
                {
                    this.lt_INV_Num_SH.Text = fn_stringFormat.Money_Format(querySH.INV_Num.ToString());       //[庫存]
                    this.lt_INV_PreOut_SH.Text = fn_stringFormat.Money_Format(querySH.INV_PreOut.ToString()); //[預計銷]
                    this.lt_INV_Safe_SH.Text = fn_stringFormat.Money_Format(querySH.INV_Safe.ToString());     //[安全存量]
                    this.lt_INV_PreIn_SH.Text = fn_stringFormat.Money_Format(querySH.INV_PreIn.ToString());   //[預計進]
                    this.lt_Stock_SH.Text = fn_stringFormat.Money_Format(querySH.StockNum.ToString());        //[庫存可用量]
                }


            }
        }
    }

    /// <summary>
    /// 取得庫存 (API.WebService)
    /// </summary>
    /// <param name="modelNo">品號</param>
    /// <returns>
    /// DBS / ModelNo / StockType / StockNum
    /// </returns>
    /// <remarks>
    /// [庫別]
    /// - prokit2 = 01
    /// - SHPK2 = 12
    /// </remarks>
    private DataTable GetStock(string modelNo)
    {
        //設定參數 - 庫別
        ArrayList aryStockType = new ArrayList();
        aryStockType.Add("01");
        aryStockType.Add("12");

        //設定參數 - 資料庫別
        ArrayList aryDBS = new ArrayList();
        aryDBS.Add("prokit2");
        aryDBS.Add("SHPK2");

        //設定參數 - 品號
        ArrayList aryModelNo = new ArrayList();
        aryModelNo.Add(modelNo);


        //宣告WebService
        API_GetERPData.ws_GetERPData ws_GetData = new API_GetERPData.ws_GetERPData();

        //取得庫存資料 (DataTable)
        DataTable DT_Stock = ws_GetData.GetStockInfo(aryStockType.ToArray(), aryModelNo.ToArray(), aryDBS.ToArray(), TokenID, out ErrMsg);
        if (DT_Stock == null)
        {
            return null;
        }

        return DT_Stock;
    }

    #endregion


    #region -- 參數設定 --
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

    /// <summary>
    /// 系統通用Token
    /// </summary>
    public string TokenID
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["API_TokenID"];
        }
        set
        {
            this._TokenID = value;
        }
    }
    private string _TokenID;

    #endregion
}