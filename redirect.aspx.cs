﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Data.Controllers;

public partial class redirect : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                string ErrMsg;

                //----- 宣告:資料參數 -----
                ParamsRepository resp = new ParamsRepository();

                try
                {
                    //建立點擊資料
                    //1:ProductCenter, 2:PKHome, 3:PKEF, 4:PKReport
                    if (!resp.Create_ClickInfo(1, Convert.ToInt32(Req_MenuID), fn_Param.CurrentUser, out ErrMsg))
                    {
                        Response.Write("---- Log記錄失敗 ----");
                    }

                    //Redirect
                    Response.Redirect(Req_Url);


                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    resp = null;
                }


            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public string Req_MenuID
    {
        get
        {
            String _data = Request.QueryString["menuID"] == null ? "0" : Request.QueryString["menuID"];
            return _data;
        }
        set
        {
            this._Req_MenuID = value;
        }
    }
    private string _Req_MenuID;


    public string Req_Url
    {
        get
        {
            String _data = Request.QueryString["url"] == null ? fn_Param.WebUrl : Request.QueryString["url"];
            return _data;
        }
        set
        {
            this._Req_Url = value;
        }
    }
    private string _Req_Url;
}