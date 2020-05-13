using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ExtensionUI;
using PKLib_Method.Methods;
using ProdSampleData.Controllers;

public partial class SampleStat : SecurityIn
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("112", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

            }
        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 匯出
    /// </summary>
    protected void btn_Export_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        int _dataCnt = 0;
        string _errMsg = "";
        this.pl_Msg.Visible = false;

        //----- 原始資料:條件篩選 -----
        //[取得/檢查參數] - sDate
        string sDate = this.tb_SDate.Text;
        search.Add("StartDate", sDate);

        //[取得/檢查參數] - eDate
        string eDate = this.tb_EDate.Text;
        search.Add("EndDate", eDate);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetProdSample_List(search, 0, 999999, out _dataCnt, out _errMsg)
            .Select(dt =>
                    new
                    {
                        公司別 = dt.Company_Name,
                        檢驗類別 = dt.Check_Name,
                        負責人 = dt.Assign_Name,
                        實際完成 = dt.Date_Actual,
                        廠商 = dt.Cust_Name,
                        產品描述 = dt.Description1,
                        比對產品 = dt.Description2
                    });

        if (query.Count() == 0)
        {
            this.pl_Msg.Visible = true;
            query = null;
            return;
        }


        DataTable DT = fn_CustomUI.LINQToDataTable(query);

        query = null;

        //匯出Excel
        fn_CustomUI.ExportExcel(
            DT
            , "{0}-新品取樣登記.xlsx".FormatThis(DateTime.Now.ToShortDateString().ToDateString("yyyyMMdd")));
    }
}