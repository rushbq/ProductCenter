using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;

public partial class myProdCheck_Step2 : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("520", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{0}Unauthorized.aspx?ErrMsg={1}", Application["WebUrl"], Server.UrlEncode(ErrMsg)), true);
                    return;
                }

                //Check Null
                if (string.IsNullOrEmpty(Req_Corp))
                {
                    Go_Step1();
                    return;
                }


                //[取得資料] - 公司別
                GetCorpData(Req_Corp);

                //Reset Year 
                int currYear = DateTime.Now.Year;
                this.ddl_Year.Items.Clear();
                for (int y = currYear; y >= currYear - 1; y--)
                {
                    this.ddl_Year.Items.Add(new ListItem(y.ToString(), y.ToString()));
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    #region -- 資料取得 --

    /// <summary>
    /// 取得公司別資料
    /// </summary>
    private void GetCorpData(string corpUID)
    {
        //----- 宣告:資料參數 -----
        ParamsRepository _data = new ParamsRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        if (!string.IsNullOrEmpty(corpUID))
        {
            search.Add((int)Common.mySearch.DataID, corpUID);

        }


        //----- 原始資料:取得資料 -----
        var query = _data.GetCorpList(search).Take(1).FirstOrDefault();


        //Check Null
        if (query == null)
        {
            Go_Step1();
            return;
        }

        //Print Data
        this.lt_CorpName.Text = query.Corp_Name;
    }

    #endregion


    #region -- 其他功能 --

    /// <summary>
    /// 回Step1
    /// </summary>
    private void Go_Step1()
    {
        this.ph_Data.Visible = false;
        this.ph_ErrMessage.Visible = true;
        this.lt_ShowMsg.Text = "<a href=\"{0}myProdCheck/Step1.aspx\" class=\"btn waves-effect waves-light light-blue\">請選擇正確的公司別, 點此回上一步重新選擇</a>".FormatThis(Application["WebUrl"]);
    }


    protected void lbtn_Search1_Click(object sender, EventArgs e)
    {
        Response.Redirect("{0}myProdCheck/Step3.aspx?corp={1}&fid={2}&sid={3}".FormatThis(
           Application["WebUrl"]
           , Req_Corp
           , Server.UrlEncode(this.tb_FirstID.Text)
           , Server.UrlEncode(this.tb_SecondID.Text)
           ));
    }

    protected void lbtn_Search2_Click(object sender, EventArgs e)
    {
        Response.Redirect("{0}myProdCheck/Step3.aspx?corp={1}&year={2}&vendor={3}&modelno={4}".FormatThis(
                  Application["WebUrl"]
                  , Req_Corp
                  , Server.UrlEncode(this.ddl_Year.SelectedValue)
                  , Server.UrlEncode(this.Cust_ID_Val.Text)
                  , Server.UrlEncode(this.ModelNo_Val.Text)
                  ));
    }

    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得參數 - Corp
    /// </summary>
    private string _Req_Corp;
    public string Req_Corp
    {
        get
        {
            String data = Request.QueryString["Corp"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_Corp = value;
        }
    }

    #endregion

}