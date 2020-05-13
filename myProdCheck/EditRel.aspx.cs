using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProdCheckData.Controllers;
using ProdCheckData.Models;
using PKLib_Method.Methods;

public partial class myProdCheck_EditRel : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //載入基本資料
                LookupData();
            }
           

        }
        catch (Exception)
        {

            throw;
        }
        
    }

    #region -- 資料取得 --


    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            this.ph_ErrMessage.Visible = true;
            this.ph_Data.Visible = false;
            this.lt_ShowMsg.Text = "無法取得資料";
            return;
        }

        //Get Data
        string corp = query.Corp_UID.ToString();
        string vendor = query.Vendor.ToString();
        string modelNo = query.ModelNo;
        this.lt_Title.Text = query.FirstID + " - " + query.SecondID;


        //Rel Data
        GetPurData(corp, vendor, modelNo);
    }

    /// <summary>
    /// 取得採購單資料(未關聯)
    /// </summary>
    private void GetPurData(string corp, string vendor, string modelNo)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:取得資料 -----
        var query = _data.GetPurRelData(corp, vendor, modelNo, Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_List.DataSource = query;
        this.lv_List.DataBind();

        //Release
        query = null;
    }

    protected void lv_List_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得必要的資料
            string firstID = ((HiddenField)e.Item.FindControl("hf_FirstID")).Value;
            string secondID = ((HiddenField)e.Item.FindControl("hf_SecondID")).Value;


            //----- 宣告:資料參數 -----
            ProdCheckRepository _data = new ProdCheckRepository();


            //----- 設定:資料欄位 -----
            var data = new RelData
            {
                DataID = Req_DataID,
                FirstID = firstID,
                SecondID = secondID
            };

            //----- 方法:新增資料 -----
            if (false == _data.Create_Rel(data))
            {
                this.ph_ErrMessage.Visible = true;
                this.lt_ShowMsg.Text = "關聯新增失敗, 請聯絡系統管理員.";
                return;
            }
            else
            {
                //更新Url
                string thisUrl = "{0}myProdCheck/EditRel.aspx?DataID={1}".FormatThis(Application["WebUrl"], Req_DataID);

                //導向
                Response.Redirect(thisUrl);
            }
        }
    }

    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 取得參數 - DataID
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