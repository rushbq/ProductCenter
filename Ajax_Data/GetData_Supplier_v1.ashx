<%@ WebHandler Language="C#" Class="GetData_Supplier" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PKLib_Data.Controllers;
using PKLib_Data.Assets;

public class GetData_Supplier : IHttpHandler
{

    /// <summary>
    /// 供應商資料(Ajax)
    /// 使用Semantic UI的Search UI
    /// </summary>
    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string searchVal = context.Request["q"];


        //----- 宣告:資料參數 -----
        SupplierRepository _data = new SupplierRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(searchVal))
        {
            search.Add((int)Common.mySearch.Keyword, searchVal);
        }


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search);


        //----- 資料整理:顯示筆數 -----
        var results = query
            .Select(fld =>
             new
             {
                 ID = fld.Sup_UID,
                 Label = fld.Sup_Name
             })
            .Take(50);


        var data = new { results };

        //----- 資料整理:序列化 ----- 
        string jdata = JsonConvert.SerializeObject(data, Formatting.None);

        /*
         * [回傳格式] - Json
         * results：資料
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