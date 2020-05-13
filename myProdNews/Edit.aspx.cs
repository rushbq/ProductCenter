using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PKLib_Method.Methods;
using ProdNewsData.Controllers;
using ProdNewsData.Models;

public partial class myProdNews_Edit : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("132", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("{1}Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg), fn_Param.WebUrl), true);
                    return;
                }


                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_Data.Visible = false;
                    //自動新增資料
                    Add_Data();
                }
                else
                {
                    this.ph_Data.Visible = true;


                    //載入資料
                    LookupData();
                    LookupData_Prod();
                    LookupData_SubProd();
                    LookupData_Files("3", this.lv_Files_Mail);
                    LookupData_Files("1", this.lv_Files_Other);
                    LookupData_Files("2", this.lv_Files_BPM);

                }

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
        ProdNewsRepository _data = new ProdNewsRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //目前使用者
        search.Add((int)mySearch.UserID, fn_Param.CurrentAccount);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1);

        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            //判斷是否發信或結案,若為Y則自動跳至檢視頁
            if (item.IsClose.Equals("Y") || item.IsMail.Equals("Y"))
            {
                //Response.Redirect("{0}myProdNews/View.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, item.NewsID));
                //return;
            }

            this.lt_DataID.Text = item.NewsID.ToString();
            this.lt_BPMSno.Text = item.BPM_Sno ?? "---";
            this.lt_BPMFormNo.Text = item.BPM_FormNo ?? "";
            this.ddl_Class.SelectedValue = item.ClassID.ToString();
            this.ddl_Lang.SelectedValue = item.Lang;
            this.tb_Subject.Text = item.Subject;
            this.rbl_TimingType.SelectedValue = item.TimingType.ToString();
            this.tb_TimingDate.Text = item.TimingDate;
            this.tb_Desc1.Text = HttpUtility.HtmlDecode(item.Desc1);
            this.tb_Desc2.Text = item.Desc2;

            //發送對象
            Lookup_Target();


            //維護資訊
            this.lt_Creater.Text = item.Create_Name;
            this.lt_CreateTime.Text = item.Create_Time;
            this.lt_Updater.Text = item.Update_Name;
            this.lt_UpdateTime.Text = item.Update_Time;
            this.lt_Sender.Text = item.Send_Name;
            this.lt_SendTime.Text = item.Send_Time;
        }
    }


    /// <summary>
    /// 發送對象
    /// </summary>
    private void Lookup_Target()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetRelTarget(Req_DataID);

        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            foreach (ListItem cbItem in this.cbl_Target.Items)
            {
                if (item.TargetID.Equals(cbItem.Value))
                {
                    cbItem.Selected = true;
                }
            }
        }
    }


    /// <summary>
    /// 取得檔案資料
    /// </summary>
    /// <param name="type"></param>
    /// <param name="list"></param>
    private void LookupData_Files(string type, ListView list)
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _dataList = new ProdNewsRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetFileList(Req_DataID, type);


        //----- 資料整理:繫結 ----- 
        list.DataSource = query;
        list.DataBind();

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
        ProdNewsRepository _dataList = new ProdNewsRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetRelModelList(Req_DataID);

        //----- 資料整理:繫結 ----- 
        this.lv_Prod.DataSource = query;
        this.lv_Prod.DataBind();

        //Release
        query = null;
    }

    protected void lv_Prod_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            //----- 宣告:資料參數 -----
            ProdNewsRepository _data = new ProdNewsRepository();


            //----- 設定:資料欄位 -----
            var data = new RelModelNo
            {
                NewsID = Convert.ToInt32(Req_DataID),
                Model_No = Get_DataID
            };

            //----- 方法:刪除資料 -----
            if (false == _data.Delete_RelByModelNo(data))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#mainModels");
            }

        }
    }

    #endregion


    #region -- 資料顯示:產品替代品號 --

    /// <summary>
    /// 顯示產品品號關聯
    /// </summary>
    private void LookupData_SubProd()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _dataList = new ProdNewsRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetRelSubModelList(Req_DataID);

        //----- 資料整理:繫結 ----- 
        this.lv_SubProd.DataSource = query;
        this.lv_SubProd.DataBind();

        //Release
        query = null;
    }

    protected void lv_SubProd_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            //----- 宣告:資料參數 -----
            ProdNewsRepository _data = new ProdNewsRepository();


            //----- 設定:資料欄位 -----
            var data = new RelModelNo
            {
                NewsID = Convert.ToInt32(Req_DataID),
                Model_No = Get_DataID
            };

            //----- 方法:刪除資料 -----
            if (false == _data.Delete_RelBySubModelNo(data))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#subModels");
            }

        }
    }

    #endregion


    #region -- 資料編輯 Start --
    /// <summary>
    /// 資料存檔
    /// </summary>
    protected void btn_Save_Click(object sender, EventArgs e)
    {
        try
        {
            Edit_Data();
        }
        catch (Exception)
        {
            //throw new Exception("系統發生錯誤 - 存檔");
            throw;
        }

    }


    /// <summary>
    /// 資料自動新增
    /// </summary>
    private void Add_Data()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();


        //----- 設定:資料欄位 -----
        string myDefDesc = "請不製作E-DM,僅內部週知。\r請同步修正PK網站產品資料/檢視包材規格/說明書/產品圖片。\r請同步修正產品SIP敘述。";

        var data = new Items
        {
            ClassID = 1,
            Lang = "zh-TW",
            Subject = "--系統自動產生主旨--請修改",
            TimingType = 99,
            Desc2 = myDefDesc,
            Create_Who = fn_Param.CurrentAccount.ToString()
        };

        //----- 方法:新增資料 -----
        Int32 dataID = _data.Create(data, out ErrMsg);
        if (dataID.Equals(0))
        {
            this.ph_Message.Visible = true;
            this.ph_Data.Visible = false;
            return;
        }
        else
        {
            //更新本頁Url
            string thisUrl = "{0}myProdNews/Edit.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, dataID);

            //導向本頁
            Response.Redirect(thisUrl);
        }
    }


    /// <summary>
    /// 資料修改
    /// </summary>
    private void Edit_Data()
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();

        //取得對象
        IEnumerable<string> SendTarget = this.cbl_Target.Items
                                        .Cast<ListItem>()
                                        .Where(item => item.Selected)
                                        .Select(item => item.Value);

        //----- 設定:資料欄位 -----
        var data = new Items
        {
            NewsID = Convert.ToInt32(Req_DataID),
            ClassID = Convert.ToInt16(this.ddl_Class.SelectedValue),
            Lang = this.ddl_Lang.SelectedValue,
            Subject = this.tb_Subject.Text,
            TimingType = Convert.ToInt16(this.rbl_TimingType.SelectedValue),
            TimingDate = this.rbl_TimingType.SelectedValue.Equals("1") ? this.tb_TimingDate.Text : "",
            TimingDesc = "",
            Desc1 = HttpUtility.HtmlEncode(this.tb_Desc1.Text),
            Desc2 = this.tb_Desc2.Text,
            SendTarget = SendTarget,
            Update_Who = fn_Param.CurrentAccount.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update(data, out ErrMsg))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl);
        }

    }


    /// <summary>
    /// 加入品號關聯
    /// </summary>
    protected void lbtn_AddProd_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();


        //----- 設定:資料欄位 -----
        var data = new RelModelNo
        {
            NewsID = Convert.ToInt32(Req_DataID),
            Model_No = this.Rel_ModelNo_Val.Text
        };

        //----- 方法:更新資料 -----
        if (false == _data.Create_RelByModelNo(data))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl + "#mainModels");
        }
    }


    /// <summary>
    /// 加入替代品號關聯
    /// </summary>
    protected void lbtn_AddSubProd_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdNewsRepository _data = new ProdNewsRepository();


        //----- 設定:資料欄位 -----
        var data = new RelModelNo
        {
            NewsID = Convert.ToInt32(Req_DataID),
            Model_No = this.Rel_SubModelNo_Val.Text
        };

        //----- 方法:更新資料 -----
        if (false == _data.Create_RelBySubModelNo(data))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl + "#subModels");
        }
    }


    #endregion -- 資料編輯 End --


    #region -- 參數設定 --
    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}myProdNews/Edit.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Req_DataID);
        }
        set
        {
            this._PageUrl = value;
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
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}myProdNews/Search.aspx".FormatThis(fn_Param.WebUrl);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

    /// <summary>
    /// 取得參數 - 資料編號
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
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}Prod_News/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], Req_DataID);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    /// <summary>
    /// BPM上傳目錄
    /// </summary>
    private string _BPM_UploadFolder;
    public string BPM_UploadFolder
    {
        get
        {
            return "EFGP/{0}/".FormatThis(this.lt_BPMFormNo.Text);
        }
        set
        {
            this._BPM_UploadFolder = value;
        }
    }

    #endregion
}