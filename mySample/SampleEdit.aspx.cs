using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using CustomController;
using MailMethods;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;
using ProdSampleData.Controllers;
using ProdSampleData.Models;

public partial class mySample_SampleEdit : SecurityIn
{
    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(
        fn_FTP.myFtp_Username
        , fn_FTP.myFtp_Password
        , fn_FTP.myFtp_ServerUrl);

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

                //Get Class
                Get_ClassList(myClass.source, this.Source, "來源");
                Get_ClassList(myClass.check, this.Check, "類別");
                Get_ClassList(myClass.status, this.NewStatus, "");

                //Get Users
                Get_UserList(this.AssignWho, true);

                //[參數判斷] - 判斷是否有資料編號
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_FirstChoose.Visible = true;
                    this.ph_Data.Visible = false;
                    this.ph_Buttons.Visible = false;
                }
                else
                {
                    this.ph_FirstChoose.Visible = false;
                    this.ph_Data.Visible = true;
                    this.ph_Buttons.Visible = true;

                    //依權限判斷欄位是否鎖定
                    this.Remark.Enabled = adminAuth;


                    //載入資料
                    LookupData();
                    LookupData_Files();
                    LookupData_Prod();
                    LookupData_Rel();
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
    /// 取得類別資料 
    /// </summary>
    /// <param name="cls">類別參數</param>
    /// <param name="ddl">下拉選單object</param>
    /// <param name="rootName">第一選項顯示名稱</param>
    private void Get_ClassList(myClass cls, DropDownList ddl, string rootName)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetClassList(cls);


        //----- 資料整理 -----
        ddl.Items.Clear();

        if (!string.IsNullOrEmpty(rootName))
        {
            ddl.Items.Add(new ListItem("選擇" + rootName, ""));
        }

        foreach (var item in query)
        {
            ddl.Items.Add(new ListItem(item.Label, item.ID.ToString()));
        }

        query = null;
    }

    /// <summary>
    /// 取得人員列表
    /// </summary>
    /// <param name="menu">選單object</param>
    /// <param name="showRoot">是否顯示root</param>
    private void Get_UserList(DropDownListGP menu, bool showRoot)
    {
        //----- 宣告:資料參數 -----
        UsersRepository _users = new UsersRepository();

        //----- 原始資料:取得所有資料 -----
        Dictionary<int, string> deptID = new Dictionary<int, string>();
        deptID.Add(1, "140");
        deptID.Add(2, "170");
        deptID.Add(3, "270");
        deptID.Add(4, "316");
        deptID.Add(5, "240");

        var query = _users.GetUsers(null, deptID);

        //index 0
        if (showRoot)
        {
            menu.Items.Add(new ListItem("選擇負責人", ""));
        }

        //Item list
        foreach (var item in query)
        {
            //判斷GP_Rank, 若為第一項, 則輸出群組名稱
            if (item.GP_Rank.Equals(1))
            {
                menu.AddItemGroup(item.DeptName);
            }

            //Item Name  
            menu.Items.Add(new ListItem("{0} ({1})".FormatThis(item.ProfID, item.ProfName), item.ProfID));
        }

        //Release
        query = null;
    }


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
        var query = _data.GetOne_ProdSample(search, out ErrMsg);

        //----- 資料整理:繫結 ----- 
        foreach (var item in query)
        {
            this.lt_SerialNo.Text = item.SerialNo;
            this.lt_Status.Text = item.Status_Name;

            if (item.Cls_Source != null)
            {
                this.Source.Items.RemoveAt(0);
                this.Source.Items.Insert(0, new ListItem("({0})".FormatThis(item.Source_Name), item.Cls_Source.ToString()));
            }

            if (item.Cls_Check != null)
            {
                this.Check.Items.RemoveAt(0);
                this.Check.Items.Insert(0, new ListItem("({0})".FormatThis(item.Check_Name), item.Cls_Check.ToString()));
            }

            if (item.Assign_Who != null)
            {
                this.AssignWho.Items.RemoveAt(0);
                this.AssignWho.Items.Insert(0, new ListItem("{0}".FormatThis(item.Assign_Name), item.Assign_Who.ToString()));
            }

            this.ModelNo.Text = item.Model_No;
            this.hf_ModelNo.Value = item.Model_No;

            this.Date_Come.Text = item.Date_Come;
            this.Date_Est.Text = item.Date_Est;
            this.Date_Actual.Text = item.Date_Actual;
            this.Remark.Text = item.Remark;

            this.Cust_Name.Text = string.IsNullOrWhiteSpace(item.Sup_ErpID) ? "" : item.Cust_Name;
            this.Cust_ID_Val.Text = item.Sup_ErpID;
            this.Cust_Corp.Text = item.Sup_Corp.ToString();
            this.Cust_Newguy.Text = item.Cust_Newguy;
            this.Cust_ModelNo.Text = item.Cust_ModelNo;
            this.Qty.Text = item.Qty.ToString();
            this.Description1.Text = item.Description1;
            this.Description2.Text = item.Description2;
            this.Description3.Text = item.Description3;
            this.Description4.Text = item.Description4;
            this.Description5.Text = item.Description5;

            //Modal-變更狀態的欄位
            this.m_SerialNo.Text = item.SerialNo;
            this.m_NowStatus.Text = item.Status_Name;
            this.NewStatus.SelectedValue = item.Cls_Status.ToString();

            //subject
            tb_MailSubject.Text = "{0} {1} {2} {3}".FormatThis(item.SerialNo, item.Cust_Name, item.Cust_ModelNo, item.Description1);

            //維護資訊
            this.lt_Creater.Text = item.Create_Name;
            this.lt_CreateTime.Text = item.Create_Time;
            this.lt_Updater.Text = item.Update_Name;
            this.lt_UpdateTime.Text = item.Update_Time;
        }
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

    protected void lv_Attachment_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
            string Get_FileName = ((HiddenField)e.Item.FindControl("hf_FileName")).Value;


            //----- 宣告:資料參數 -----
            ProdSampleRepository _data = new ProdSampleRepository();


            //----- 設定:資料欄位 -----
            var data = new SampleFiles
            {
                SP_ID = new Guid(Req_DataID),
                AttachID = Convert.ToInt32(Get_DataID)
            };

            //----- 方法:刪除資料 -----
            if (false == _data.Delete_SampleFiles(data))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //刪除檔案
                _ftp.FTP_DelFile(UploadFolder, Get_FileName);

                //導向本頁
                Response.Redirect(PageUrl + "#attachment");
            }

        }
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

    protected void lv_Prod_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            //----- 宣告:資料參數 -----
            ProdSampleRepository _data = new ProdSampleRepository();


            //----- 設定:資料欄位 -----
            var data = new RelModelNo
            {
                SP_ID = new Guid(Req_DataID),
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
                Response.Redirect(PageUrl + "#modelRel");
            }

        }
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

    protected void lv_Sample_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;

            //----- 宣告:資料參數 -----
            ProdSampleRepository _data = new ProdSampleRepository();


            //----- 設定:資料欄位 -----
            var data = new RelSampleID
            {
                SP_ID = new Guid(Req_DataID),
                Rel_ID = new Guid(Get_DataID)
            };

            //----- 方法:刪除資料 -----
            if (false == _data.Delete_RelBySample(data))
            {
                this.ph_Message.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(PageUrl + "#dataRel");
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
            throw new Exception("系統發生錯誤 - 存檔");
        }

    }

    protected void btn_Choose_TW_Click(object sender, EventArgs e)
    {
        Add_Data("TWS");
    }
    protected void btn_Choose_SH_Click(object sender, EventArgs e)
    {
        Add_Data("SHS");
    }
    //protected void btn_Choose_SZ_Click(object sender, EventArgs e)
    //{
    //    Add_Data("SZS");
    //}

    /// <summary>
    /// 資料自動新增
    /// </summary>
    private void Add_Data(string src)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();


        //----- 設定:資料欄位 -----
        //產生Guid
        string guid = CustomExtension.GetGuid();

        var data = new ProdSample
          {
              SP_ID = new Guid(guid),
              Company = src.ToUpper(),
              Cls_Status = 8, //評估中
              Create_Who = fn_Param.CurrentUser.ToString()
          };

        //----- 方法:新增資料 -----
        if (false == _data.Create(data))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //更新本頁Url
            string thisUrl = "{0}/mySample/SampleEdit.aspx?DataID={1}".FormatThis(Application["WebUrl"], guid);

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
        ProdSampleRepository _data = new ProdSampleRepository();


        //----- 設定:資料欄位 -----
        var data = new ProdSample
        {
            SP_ID = new Guid(Req_DataID),
            Assign_Who = this.AssignWho.SelectedValue,
            Model_No = this.hf_ModelNo.Value,
            Sup_ErpID = this.Cust_ID_Val.Text,
            Sup_Corp = string.IsNullOrEmpty(Cust_Corp.Text) ? 0 : Convert.ToInt32(Cust_Corp.Text),
            Cust_ModelNo = this.Cust_ModelNo.Text,
            Cust_Newguy = this.Cust_Newguy.Text,
            Qty = string.IsNullOrEmpty(this.Qty.Text) ? 1 : Convert.ToInt32(this.Qty.Text),
            Cls_Source = Convert.ToInt32(this.Source.SelectedValue),
            Cls_Check = Convert.ToInt32(this.Check.SelectedValue),
            Date_Come = this.Date_Come.Text,
            Date_Est = this.Date_Est.Text,
            Date_Actual = this.Date_Actual.Text,
            Description1 = this.Description1.Text,
            Description2 = this.Description2.Text,
            Description3 = this.Description3.Text,
            Description4 = this.Description4.Text,
            Description5 = this.Description5.Text,
            Remark = this.Remark.Text,
            Update_Who = fn_Param.CurrentUser.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Update(data))
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
    /// 附件上傳
    /// </summary>
    protected void lbtn_AddFiles_Click(object sender, EventArgs e)
    {
        #region -- 檔案處理 --

        //宣告
        List<IOTempParam> ITempList = new List<IOTempParam>();
        Random rnd = new Random();

        int GetFileCnt = 0;

        //取得上傳檔案集合
        HttpFileCollection hfc = Request.Files;

        //--- 限制上傳數量 ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > 0)
            {
                GetFileCnt++;
            }
        }
        if (GetFileCnt > FileCountLimit)
        {
            //[提示]
            this.lt_UploadMessage.Text = "單次上傳超出限制, 每次上傳僅限 {0} 個檔案.".FormatThis(FileCountLimit);
            return;
        }


        //--- 檔案檢查 ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > FileSizeLimit)
            {
                //[提示]
                this.lt_UploadMessage.Text = "部份檔案大小超出限制, 每個檔案大小限制為 {0} MB".FormatThis(FileSizeLimit / 1024000);
                return;
            }

            if (hpf.ContentLength > 0)
            {
                //取得原始檔名
                string OrgFileName = Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = Path.GetExtension(OrgFileName).ToLower();
                if (false == CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                {
                    //[提示]
                    this.lt_UploadMessage.Text = "部份檔案副檔名不符規定, 僅可上傳副檔名為 {0}".FormatThis(FileExtLimit.Replace("|", ", "));
                    return;
                }
            }
        }


        //--- 檔案暫存List ---
        for (int idx = 0; idx <= hfc.Count - 1; idx++)
        {
            //取得個別檔案
            HttpPostedFile hpf = hfc[idx];

            if (hpf.ContentLength > 0)
            {
                //取得原始檔名
                string OrgFileName = Path.GetFileName(hpf.FileName);
                //取得副檔名
                string FileExt = Path.GetExtension(OrgFileName).ToLower();

                //設定檔名, 重新命名
                string myFullFile = String.Format(@"{0:yyMMddHHmmssfff}{1}{2}"
                    , DateTime.Now
                    , rnd.Next(0, 99)
                    , FileExt);


                //判斷副檔名, 未符合規格的檔案不上傳
                if (CustomExtension.CheckStrWord(FileExt, FileExtLimit, "|", 1))
                {
                    //設定暫存-檔案
                    ITempList.Add(new IOTempParam(myFullFile, OrgFileName, hpf));
                }
            }
        }

        #endregion


        #region -- 儲存檔案 --

        if (ITempList.Count > 0)
        {
            int errCnt = 0;
            //判斷資料夾, 不存在則建立
            _ftp.FTP_CheckFolder(UploadFolder);

            //暫存檔案List
            for (int row = 0; row < ITempList.Count; row++)
            {
                //取得個別檔案
                HttpPostedFile hpf = ITempList[row].Param_hpf;

                //執行上傳
                if (false == _ftp.FTP_doUpload(hpf, UploadFolder, ITempList[row].Param_FileName))
                {
                    errCnt++;
                }
            }

            if (errCnt > 0)
            {
                this.lt_UploadMessage.Text = "檔案上傳失敗, 失敗筆數為 {0} 筆, 請重新整理後再上傳!".FormatThis(errCnt);
                return;
            }
        }

        #endregion


        #region --資料處理--

        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();
        List<SampleFiles> dataList = new List<SampleFiles>();

        //----- 設定:資料欄位 -----
        for (int row = 0; row < ITempList.Count; row++)
        {
            var data = new SampleFiles
            {
                SP_ID = new Guid(Req_DataID),
                AttachFile = ITempList[row].Param_FileName,
                AttachFile_Name = ITempList[row].Param_OrgFileName,
                Create_Who = fn_Param.CurrentUser.ToString()
            };

            dataList.Add(data);
        }

        //----- 方法:更新資料 -----
        if (false == _data.Create_Attachment(dataList))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl + "#attachment");
        }
        #endregion

    }

    /// <summary>
    /// 加入產品品號關聯
    /// </summary>
    protected void lbtn_AddProd_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();


        //----- 設定:資料欄位 -----
        var data = new RelModelNo
        {
            SP_ID = new Guid(Req_DataID),
            Model_No = this.Rel_ModelNo_Val.Text,
            Create_Who = fn_Param.CurrentUser.ToString()
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
            Response.Redirect(PageUrl + "#modelRel");
        }
    }

    /// <summary>
    /// 加入樣品關聯
    /// </summary>
    protected void lbtn_AddRel_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();


        //----- 設定:資料欄位 -----
        var data = new RelSampleID
        {
            SP_ID = new Guid(Req_DataID),
            Rel_ID = new Guid(this.Rel_SampleID_Val.Text),
            Create_Who = fn_Param.CurrentUser.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Create_RelBySample(data))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //導向本頁
            Response.Redirect(PageUrl + "#dataRel");
        }
    }

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


    /// <summary>
    /// 刪除資料
    /// </summary>
    protected void lbtn_DelData_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();

        //----- 方法:更新資料 -----
        if (false == _data.Delete(Req_DataID))
        {
            this.ph_Message.Visible = true;
            return;
        }
        else
        {
            //刪除整個Folder檔案
            _ftp.FTP_DelFolder(UploadFolder);

            //導向至列表頁
            Response.Redirect(Page_SearchUrl);
        }
    }

    #endregion -- 資料編輯 End --


    #region -- 發通知信 --

    protected void btn_Setting_Click(object sender, EventArgs e)
    {
        try
        {
            /*
             * Step1:將勾選清單存入 x_MailList
             * Step2:自動Update x_MailList.MailAddress = PKSYS.User_Profile.Email
             * Step3:取得Mail清單，開始發信
             */

            string _subject = tb_MailSubject.Text.Trim();
            if (string.IsNullOrWhiteSpace(_subject))
            {
                CustomExtension.AlertMsg("主旨不得為空白", "");
                return;
            }

            //建立Mail清單
            if (!Create_MailList())
            {
                CustomExtension.AlertMsg("Mail清單建立失敗", "");
                return;
            }

            //取得Mail清單，開始發信
            List<string> mailList = Get_MailList();

            #region -- 發信 --

            //[設定參數] - 建立者
            fn_Mail.Create_Who = "System";

            //[設定參數] - 來源程式/功能
            fn_Mail.FromFunc = "ProductCenter, 產品檢驗登記";

            //[設定參數] - 寄件人
            fn_Mail.Sender = System.Web.Configuration.WebConfigurationManager.AppSettings["SysMail_Sender"];

            //[設定參數] - 寄件人顯示名稱
            fn_Mail.SenderName = "Pro'sKit Mail System";

            //[設定參數] - 收件人
            if (mailList == null)
            {
                CustomExtension.AlertMsg("Mail無法取得收件人清單", "");
                return;
            }
            fn_Mail.Reciever = mailList;

            //[設定參數] - 轉寄人群組
            fn_Mail.CC = null;

            //[設定參數] - 密件轉寄人群組
            fn_Mail.BCC = null;

            //[設定參數] - 郵件主旨
            fn_Mail.Subject = _subject;

            //[設定參數] - 郵件內容
            fn_Mail.MailBody = Get_MailBody();

            //[設定參數] - 指定檔案 - 路徑
            fn_Mail.FilePath = "";

            //[設定參數] - 指定檔案 - 檔名
            fn_Mail.FileName = "";

            //發送郵件
            fn_Mail.SendMail();

            //[判斷參數] - 寄件是否成功
            if (!fn_Mail.MessageCode.Equals(200))
            {
                CustomExtension.AlertMsg("發送Mail時發生了一點小問題, 錯誤代碼:{0}".FormatThis(fn_Mail.MessageCode), "");
                return;
            }
            else
            {
                //導向本頁
                CustomExtension.AlertMsg("發信完成", PageUrl);
            }

            #endregion


        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// 建立Mail清單
    /// </summary>
    /// <returns></returns>
    private bool Create_MailList()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" DECLARE @NewID AS INT ");
            sql.AppendLine(" DELETE FROM Sample_MailList WHERE (Data_ID = @DataID)");

            //[取得參數值] - 編號組合(工號)
            string[] strAry = Regex.Split(this.tb_Values_User.Text, @"\,{1}");
            var query = from el in strAry
                        select new
                        {
                            Val = el.ToString().Trim()
                        };

            //[SQL] - 資料新增
            foreach (var item in query)
            {
                //-- 產生序號 --
                sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(MailID), 0) + 1 FROM Sample_MailList WHERE (Data_ID = @DataID));");

                //新增資料
                sql.AppendLine(" INSERT INTO Sample_MailList( ");
                sql.AppendLine("  Data_ID, MailID, MailTo, MailAddress, Create_Who, Create_Time");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT @DataID, @NewID, N'{0}', Email, @Create_Who, GETDATE()".FormatThis(item.Val));
                sql.AppendLine(" FROM PKSYS.dbo.User_Profile ");
                sql.AppendLine(" WHERE (Email IS NOT NULL) AND (Email <> '') AND (Guid = N'{0}'); ".FormatThis(item.Val));
            }


            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);
            cmd.Parameters.AddWithValue("Create_Who", fn_Param.CurrentUser.ToString());

            return dbConClass.ExecuteSql(cmd, out ErrMsg);
        }
    }


    /// <summary>
    /// 取得Mail清單
    /// </summary>
    /// <returns>List</returns>
    private List<string> Get_MailList()
    {
        //----- 宣告 -----
        StringBuilder sql = new StringBuilder();
        List<string> GetEmail = new List<string>();

        //----- 資料查詢 -----
        using (SqlCommand cmd = new SqlCommand())
        {
            //----- SQL 查詢語法 -----
            sql.AppendLine(" SELECT MailAddress");
            sql.AppendLine(" FROM Sample_MailList WITH (NOLOCK)");
            sql.AppendLine(" WHERE (Data_ID = @DataID)");


            //----- SQL 執行 -----
            cmd.CommandText = sql.ToString();
            cmd.Parameters.AddWithValue("DataID", Req_DataID);

            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                for (int row = 0; row < DT.Rows.Count; row++)
                {
                    GetEmail.Add(DT.Rows[row]["MailAddress"].ToString());
                }
            }
        }

        //return
        return GetEmail;
    }


    /// <summary>
    /// 取得寄信內容
    /// </summary>
    /// <returns></returns>
    private StringBuilder Get_MailBody()
    {
        //Html Url(CDN)
        string myHtmlUrl = "{0}ProductCenter/Prod_Sample/Mail.html".FormatThis(fn_Param.CDNUrl);

        //宣告
        StringBuilder html = new StringBuilder();

        //取得html內容
        html.Append(CustomExtension.WebRequest_byGET(myHtmlUrl).ToString());

        //----- 宣告:資料參數 -----
        ProdSampleRepository _data = new ProdSampleRepository();
        Dictionary<string, string> search = new Dictionary<string, string>();

        //----- 原始資料:條件篩選 -----
        search.Add("DataID", Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetOne_ProdSample(search, out ErrMsg).FirstOrDefault();

        //----- 資料整理:填入資料 ----- 
        html.Replace("#SP_ID#", query.SerialNo);
        html.Replace("#CustName#", query.Cust_Name);
        html.Replace("#CustModel#", query.Cust_ModelNo);
        html.Replace("#ModelNo#", query.Model_No);
        html.Replace("#ProdDesc#", query.Description1);

        string url = "{0}mySample/SampleEdit.aspx?DataID={1}".FormatThis(fn_Param.WebUrl, Req_DataID);
        html.Replace("#ViewUrl#"
            , "<a href=\"{0}\" target=\"_blank\">{0}</a>".FormatThis(
                url
            ));
        html.Replace("#今年#", DateTime.Now.Year.ToString());

        string dotLine = "<div style=\"border-top:1px dashed #cccccc;height: 1px;overflow:hidden\"></div>";
        html.Replace("#htmlDesc1#", string.IsNullOrWhiteSpace(query.Description3) ? "" : "<h4>{0}</h4>{1}".FormatThis("【檢測結果】", query.Description3 + dotLine));
        html.Replace("#htmlDesc2#", string.IsNullOrWhiteSpace(query.Description4) ? "" : "<h4>{0}</h4>{1}".FormatThis("【產品建議】", query.Description4 + dotLine));
        html.Replace("#htmlDesc3#", string.IsNullOrWhiteSpace(query.Description5) ? "" : "<h4>{0}</h4>{1}".FormatThis("【最終決議】", query.Description5));


        _data = null;
        query = null;

        //return
        return html;
    }


    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return "{0}mySample/SampleEdit.aspx?DataID={1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }


    /// <summary>
    /// 設定參數 - 檢視頁Url
    /// </summary>
    private string _Page_ViewUrl;
    public string Page_ViewUrl
    {
        get
        {
            return "{0}mySample/SampleView.aspx?DataID={1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._Page_ViewUrl = value;
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

    #endregion


    #region -- 上傳參數 --
    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return "jpg|png|pdf|doc|docx|xls|xlsx|txt|rar|zip";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 限制上傳的檔案大小(1MB = 1024000), 50MB
    /// </summary>
    private int _FileSizeLimit;
    public int FileSizeLimit
    {
        get
        {
            return 51200000;
        }
        set
        {
            this._FileSizeLimit = value;
        }
    }

    /// <summary>
    /// 限制上傳檔案數
    /// </summary>
    private int _FileCountLimit;
    public int FileCountLimit
    {
        get
        {
            return 5;
        }
        set
        {
            this._FileCountLimit = value;
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
            return "{0}ProdSample/{1}/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"], Req_DataID);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class IOTempParam
    {
        /// <summary>
        /// [參數] - 檔名
        /// </summary>
        private string _Param_FileName;
        public string Param_FileName
        {
            get { return this._Param_FileName; }
            set { this._Param_FileName = value; }
        }

        /// <summary>
        /// [參數] -原始檔名
        /// </summary>
        private string _Param_OrgFileName;
        public string Param_OrgFileName
        {
            get { return this._Param_OrgFileName; }
            set { this._Param_OrgFileName = value; }
        }


        private HttpPostedFile _Param_hpf;
        public HttpPostedFile Param_hpf
        {
            get { return this._Param_hpf; }
            set { this._Param_hpf = value; }
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="Param_FileName">系統檔名</param>
        /// <param name="Param_OrgFileName">原始檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">檔案類別</param>
        public IOTempParam(string Param_FileName, string Param_OrgFileName, HttpPostedFile Param_hpf)
        {
            this._Param_FileName = Param_FileName;
            this._Param_OrgFileName = Param_OrgFileName;
            this._Param_hpf = Param_hpf;
        }

    }
    #endregion

}