using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;
using ProdSampleData.Controllers;
using ProdSampleData.Models;

public partial class mySample_SampleView : SecurityIn
{
    public string ErrMsg;
    public bool adminAuth = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("146", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }
                //管理權限
                adminAuth = fn_CheckAuth.CheckAuth_User("147", out ErrMsg);
                this.ph_ModalMessage.Visible = !adminAuth;
                this.ph_ModalData.Visible = adminAuth;

                //判斷編號是否為空
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Message.Visible = true;
                    this.ph_Data.Visible = false;

                    return;
                }

                this.ph_Message.Visible = false;
                this.ph_Data.Visible = true;

                //Get Class
                Get_ClassList(myClass.status, this.NewStatus);

                //顯示資料
                LookupData();
                LookupData_Prod();
                LookupData_Rel();
                LookupData_Files();
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----
        search.Add("DataID", Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var data = _data.GetOne_ProdSample(search, out ErrMsg);


        //----- 資料整理:繫結 ----- 
        this.lvDataList.DataSource = data;
        this.lvDataList.DataBind();


        //----- 資料整理:提取部份資料 -----
        foreach (var item in data)
        {
            //Modal-變更狀態的欄位
            this.m_SerialNo.Text = item.SerialNo;
            this.m_NowStatus.Text = item.Status_Name;
            this.NewStatus.SelectedValue = item.Cls_Status.ToString();

            //維護資訊
            this.lt_Creater.Text = item.Create_Name;
            this.lt_CreateTime.Text = item.Create_Time;
            this.lt_Updater.Text = item.Update_Name;
            this.lt_UpdateTime.Text = item.Update_Time;

            //檢測結果
            Description3.Text = item.Description3;
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            ////供應商代號
            //int Sup_UID = Convert.ToInt32(DataBinder.Eval(dataItem.DataItem, "Cust_ID"));

            //if (Sup_UID != 0)
            //{
            //    //供應商關聯列表
            //    Literal lt_SupplierList = ((Literal)e.Item.FindControl("lt_SupplierList"));

            //    //----- 宣告:資料參數 -----
            //    SupplierRepository _data = new SupplierRepository();

            //    //----- 原始資料:取得所有資料 -----
            //    var query = _data.GetRelList(Sup_UID.ToString());
            //    StringBuilder html = new StringBuilder();

            //    html.Append("<table class=\"bordered striped\">");
            //    html.Append("<thead>");
            //    html.Append("  <tr><th>公司別</th><th>代號</th><th>名稱</th></tr>");
            //    html.Append("</thead>");
            //    html.Append("<tbody>");
            //    foreach (var item in query)
            //    {
            //        html.Append("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>".FormatThis(
            //            item.Corp_Name
            //            , item.ERP_SupID
            //            , item.ERP_SupName
            //            ));
            //    }
            //    html.Append("</tbody>");
            //    html.Append("</table>");

            //    //release
            //    query = null;

            //    //output html
            //    lt_SupplierList.Text = html.ToString();
            //}

        }
    }


    /// <summary>
    /// 取得類別資料 
    /// </summary>
    /// <param name="cls">類別參數</param>
    /// <param name="ddl">下拉選單object</param>
    /// <param name="rootName">第一選項顯示名稱</param>
    private void Get_ClassList(myClass cls, DropDownList ddl)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls);


        //----- 資料整理 -----
        ddl.Items.Clear();

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        query = null;
    }

    #endregion

    #region -- 資料顯示:檔案附件 --

    /// <summary>
    /// 顯示檔案附件
    /// </summary>
    private void LookupData_Files()
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _dataList = new ProdSampleRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetFileList(Req_DataID);


        //----- 資料整理:繫結 ----- 
        this.lv_Attachment.DataSource = query;
        this.lv_Attachment.DataBind();

        //Release
        query = null;
    }

    #endregion

    #region -- 資料顯示:產品品號 --

    /// <summary>
    /// 顯示產品品號關聯
    /// </summary>
    private void LookupData_Prod()
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _dataList = new ProdSampleRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetRelModelList(Req_DataID);

        //----- 資料整理:繫結 ----- 
        this.lv_Prod.DataSource = query;
        this.lv_Prod.DataBind();

        //Release
        query = null;
    }

    #endregion

    #region -- 資料顯示:樣品關聯 --

    /// <summary>
    /// 顯示樣品關聯
    /// </summary>
    private void LookupData_Rel()
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _dataList = new ProdSampleRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();
        int dataCnt = 0;

        //----- 原始資料:條件篩選 -----
        search.Add("RelID", Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetProdSample_List(search, 0, 99999, out dataCnt, out ErrMsg);

        //----- 資料整理:繫結 ----- 
        this.lv_Sample.DataSource = query;
        this.lv_Sample.DataBind();

        //Release
        query = null;
    }

    #endregion

    /// <summary>
    /// 變更狀態
    /// </summary>
    protected void lbtn_ChangeStatus_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();

        //----- 設定:資料欄位 -----
        var data = new ProdSample
        {
            SP_ID = new Guid(Req_DataID),
            Cls_Status = Convert.ToInt16(this.NewStatus.SelectedValue)
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update_Status(data))
        {
            Response.Write("壞掉了....");
            return;
        }
        else
        {
            Response.Redirect(PageUrl);
        }
    }


    #region -- 參數設定 --
    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}ProdSample/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], Req_DataID);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    /// <summary>
    /// 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}mySample/SampleView.aspx?DataID={1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - 修改頁Url
    /// </summary>
    private string _Page_EditUrl;
    public string Page_EditUrl
    {
        get
        {
            return "{0}mySample/SampleEdit.aspx?DataID={1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._Page_EditUrl = value;
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
            string tempUrl = CustomExtension.getCookie("ProdSample");

            return string.IsNullOrWhiteSpace(tempUrl) ? fn_Param.WebUrl + "mySample/SampleList.aspx" : Server.UrlDecode(tempUrl);
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

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

    #endregion

}