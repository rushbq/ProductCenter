<%@ WebHandler Language="C#" Class="GetUserList" %>
using System;
using System.Web;
using System.Linq;
using PKLib_Data.Models;
using PKLib_Data.Controllers;
using PKLib_Data.Assets;
using Newtonsoft.Json;

public class GetUserList : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //[接收參數] 查詢字串
        string search_Area = context.Request["area"] ?? "";

        //----- 宣告:資料參數 -----
        UsersRepository _data = new UsersRepository();


        //----- 原始資料:取得所有資料 -----
        IQueryable<UserTree> query;

        //判斷是否有區域限制
        Common.DeptArea myArea;
        switch (search_Area.ToUpper())
        {
            case "TW":
                myArea = Common.DeptArea.TW;
                break;

            case "SH":
                myArea = Common.DeptArea.SH;
                break;

            case "SZ":
                myArea = Common.DeptArea.SZ;
                break;

            default:
                myArea = Common.DeptArea.ALL;
                break;
        }

        //載入資料
        query = _data.GetUserTree(myArea);


        //----- 資料整理:顯示欄位 -----
        /*
         * 使用jsTree 樹狀選單元件
         * 將資料整理成 jsTree json格式
         */
        var data = query
            .Select(fld =>
             new
             {
                 id = fld.MenuID,
                 parentID = fld.ParentID,
                 label = fld.MenuName,
                 open = fld.IsOpen,
                 chkDisabled = fld.chkDisabled
             });


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