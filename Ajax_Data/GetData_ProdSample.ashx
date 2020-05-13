<%@ WebHandler Language="C#" Class="GetData_ProdSample" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProdSampleData.Models;
using ProdSampleData.Controllers;
using PKLib_Method.Methods;

public class GetData_ProdSample : IHttpHandler
{

    /// <summary>
    /// 取得取樣資料(Ajax)
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string searchVal = context.Request["keyword"];
        string nowID = context.Request["id"];
        Guid myID = new Guid();

        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        int dataCnt = 0;
        string ErrMsg;

        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(searchVal))
        {
            search.Add("Keyword", searchVal);
        }
        if (!string.IsNullOrEmpty(nowID))
        {
            myID = new Guid(nowID);
        }

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetProdSample_List(search, 0, 100, out dataCnt, out ErrMsg);


        //----- 資料整理:顯示筆數 -----
        var data = query
            .Where(fld => !fld.SP_ID.Equals(myID))
            .Select(fld =>
             new
             {
                 ID = fld.SP_ID,
                 Label = "({0}) {1} {2}".FormatThis(fld.SerialNo, fld.Company_Name, fld.Cust_Name + fld.Cust_ModelNo)
             })
            .Take(100);


        //----- 資料整理:序列化 ----- 
        string jdata = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

        /*
         * [回傳格式] - Json
         * data：資料
         */

        //輸出Json
        context.Response.ContentType = "application/json";
        context.Response.Write(jdata);


    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}