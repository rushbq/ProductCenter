using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using LogRecord;

public partial class ProdPic_myPic_Maintain : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg = "";

            if (!IsPostBack)
            {
                //[判斷 & 取得參數] - Cls 圖片類別
                if (fn_Extensions.Num_正整數(Param_Class, "1", "999999999", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 圖片類別！", "script:parent.$.fancybox.close();");
                }
                //[判斷 & 取得參數] - Model_No 品號
                if (fn_Extensions.String_字數(Param_ModelNo, "1", "40", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 品號！", "script:parent.$.fancybox.close();");
                }
                //[判斷 & 取得參數] - Col 圖片欄位
                if (fn_Extensions.String_字數(Param_Column, "1", "10", out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("參數傳遞錯誤 - 圖片欄位！", "script:parent.$.fancybox.close();");
                }

                //帶出資料 - 圖片
                GetData_Pic();
                //帶出資料 - 關聯性
                GetData_Rel();

                //顯示暫存, 不可關聯的品號
                if (Session["PicRelItems"] != null)
                {
                    this.pl_unRel.Visible = true;
                    this.lt_unRel.Text = Session["PicRelItems"].ToString();

                    //程除Session (PicRelItems)
                    Session.Remove("PicRelItems");
                }

            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤！", "");
        }
    }
    

    #region -- 資料讀取 --
    /// <summary>
    /// 取得資料 - 圖片
    /// </summary>
    private void GetData_Pic()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                string ErrMsg = "";
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //判斷圖片類別
                switch (Param_Class)
                {
                    case "1":
                        //[SQL] - 資料查詢, 產品圖
                        SBSql.AppendLine(" SELECT Pic_ID, '' AS Lang ");
                        //[SQL] - 取得檔案
                        SBSql.AppendLine(" , {0} AS PicFile, {0}_OrgFile AS PicOrgFile".FormatThis(Param_Column));
                        //[SQL] - 取得關聯ID
                        SBSql.AppendLine(" , (");
                        SBSql.AppendLine("      SELECT Rel_ID FROM ProdPic_Rel ");
                        SBSql.AppendLine("      WHERE (Model_No = @Param_ModelNo) AND (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) ");
                        SBSql.AppendLine("   ) AS RelID ");
                        SBSql.AppendLine(" FROM ProdPic_Photo ");
                        SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) ");

                        break;

                    case "2":
                        //[SQL] - 資料查詢, 產品輔圖
                        SBSql.AppendLine(" SELECT Pic_ID, '' AS Lang ");
                        //[SQL] - 取得檔案
                        SBSql.AppendLine(" , {0} AS PicFile, {0}_OrgFile AS PicOrgFile".FormatThis(Param_Column));
                        //[SQL] - 取得關聯ID
                        SBSql.AppendLine(" , (");
                        SBSql.AppendLine("      SELECT Rel_ID FROM ProdPic_Rel ");
                        SBSql.AppendLine("      WHERE (Model_No = @Param_ModelNo) AND (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) ");
                        SBSql.AppendLine("   ) AS RelID ");
                        SBSql.AppendLine(" FROM ProdPic_Figure ");
                        SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) ");

                        break;

                    default:
                        //[SQL] - 資料查詢, 其他
                        SBSql.AppendLine(" SELECT Pic_ID, Lang ");
                        //[SQL] - 取得檔案
                        SBSql.AppendLine(" , Pic_File AS PicFile, Pic_OrgFile AS PicOrgFile");
                        //[SQL] - 取得關聯ID
                        SBSql.AppendLine(" , (");
                        SBSql.AppendLine("      SELECT Rel_ID FROM ProdPic_Rel ");
                        SBSql.AppendLine("      WHERE (Model_No = @Param_ModelNo) AND (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) ");
                        SBSql.AppendLine("   ) AS RelID ");
                        SBSql.AppendLine(" FROM ProdPic_Group ");
                        SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) AND (Pic_Class = @Pic_Class) AND (Pic_ID = @Pic_Column) ");

                        break;
                }

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
                cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //判斷是否有資料, 控制相關按鈕
                        this.hf_PicID.Value = "";
                        this.hf_RelID.Value = "";
                        this.tb_ModelNo.Enabled = false;
                        this.lbtn_AddRel.Enabled = false;
                    }
                    else
                    {
                        //判斷是否有資料, 控制相關按鈕
                        this.hf_PicID.Value = DT.Rows[0]["Pic_ID"].ToString();
                        this.hf_RelID.Value = DT.Rows[0]["RelID"].ToString();
                        this.hf_Lang.Value = DT.Rows[0]["Lang"].ToString();
                        string PicFile = DT.Rows[0]["PicFile"].ToString();
                        string PicOrgFile = DT.Rows[0]["PicOrgFile"].ToString();

                        if (false == string.IsNullOrEmpty(PicFile))
                        {
                            //[按鈕] - 刪除
                            this.btn_Del.Visible = true;
                            //this.btn_DelAll.Visible = true;

                            //填入圖片資料
                            this.lt_Pic.Text = PicUrl(PicFile, PicOrgFile);
                            //this.lt_PicName.Text = PicOrgFile;

                            //暫存檔名
                            this.hf_Pic.Value = PicFile;
                            this.hf_Pic500.Value = "{0}{1}".FormatThis(ThumbFrontFileName_500, PicFile);
                            this.hf_Pic1000.Value = "{0}{1}".FormatThis(ThumbFrontFileName_1000, PicFile);
                            this.hf_OrgPic.Value = PicOrgFile;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
        }

    }

    /// <summary>
    /// 取得資料 - 關聯性
    /// </summary>
    private void GetData_Rel()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg = "";

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Rel_ID, Pic_Class, Pic_Column, RTRIM(Model_No) AS Model_No ");
                SBSql.AppendLine(" FROM ProdPic_Rel ");
                SBSql.AppendLine(" WHERE (Model_No <> @Param_ModelNo) AND (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column)");
                SBSql.AppendLine("  AND (Rel_ID IN (");
                SBSql.AppendLine("      SELECT Rel_ID FROM ProdPic_Rel ");
                SBSql.AppendLine("      WHERE (Model_No = @Param_ModelNo) AND (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) ");
                SBSql.AppendLine("  ))");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
                cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
                cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();

                    //將關聯品號暫存, 更新時會使用
                    ArrayList aryRel = new ArrayList();
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        aryRel.Add(DT.Rows[row]["Model_No"].ToString());
                    }
                    this.hf_RelModel.Value = string.Join("|", aryRel.ToArray());
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 讀取資料！", "");
        }
    }

    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();

                //[取得參數] - 編號
                string Get_ModelNo = ((HiddenField)e.Item.FindControl("hf_Model_No")).Value;
                string Get_RelID = ((HiddenField)e.Item.FindControl("hf_Rel_ID")).Value;

                //[SQL] - 刪除關聯資料(只刪除關聯, 保留檔案)
                SBSql.AppendLine(" DELETE FROM ProdPic_Rel");
                SBSql.AppendLine(" WHERE (Model_No = @Param_ModelNo) AND (Rel_ID = @Param_RelID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Param_ModelNo", Get_ModelNo);
                cmd.Parameters.AddWithValue("Param_RelID", Get_RelID);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("關聯刪除失敗！", "");
                }
                else
                {
                    //頁面跳轉
                    fn_Extensions.JsAlert("", PageUrl);
                }

            }
        }
    }

    #endregion


    #region -- 資料編輯 --

    /// <summary>
    /// 自動上傳
    /// </summary>
    protected void lbtn_Save1_Click(object sender, EventArgs e)
    {
        #region --檔案處理--

        //副檔名檢查參數
        int errExt = 0;

        //[IO] - 暫存檔案名稱
        List<TempParam> ITempList = new List<TempParam>();

        //主圖
        HttpPostedFile hpFile = this.fu_Pic.PostedFile;
        if (hpFile != null)
        {
            if (hpFile.ContentLength > 0)
            {
                //[IO] - 取得檔案名稱
                IOManage.GetFileName(hpFile);

                //判斷副檔名，未符合規格的檔案不上傳
                if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 2))
                {
                    string picName = IOManage.FileNewName;
                    string fullName = IOManage.FileFullName;

                    //暫存檔案資訊
                    ITempList.Add(new TempParam(picName, IOManage.FileFullName, this.hf_Pic.Value, hpFile, "P1", true));

                    ITempList.Add(new TempParam(ThumbFrontFileName_500 + picName, fullName, this.hf_Pic500.Value, hpFile, "P2", true));
                    ITempList.Add(new TempParam(ThumbFrontFileName_1000 + picName, fullName, this.hf_Pic1000.Value, hpFile, "P3", true));
                }
                else
                {
                    errExt++;
                }

            }
        }

        //未符合檔案規格的警示訊息
        if (errExt > 0)
        {
            fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
            return;
        }

        #endregion


        //判斷是否有資料
        if (string.IsNullOrEmpty(this.hf_PicID.Value))
        {
            //Insert
            Add_Data(ITempList);
        }
        else
        {
            //Update
            Edit_Data(ITempList);
        }
    }


    /// <summary>
    /// 手動上傳
    /// </summary>
    protected void lbtn_Save2_Click(object sender, EventArgs e)
    {
        //將Logo樣式重置為0, 避免自動浮水印
        this.rbl_LogoType.SelectedIndex = 0;


        #region --檔案處理--

        //副檔名檢查參數
        int errExt = 0;

        //[IO] - 暫存檔案名稱
        List<TempParam> ITempList = new List<TempParam>();
        string picMain = this.hf_Pic.Value;

        //主圖
        HttpPostedFile hpFile = this.fu_Pic.PostedFile;
        if (hpFile != null)
        {
            if (hpFile.ContentLength > 0)
            {
                //[IO] - 取得檔案名稱
                IOManage.GetFileName(hpFile);

                //判斷副檔名，未符合規格的檔案不上傳
                if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 2))
                {
                    //取得主圖檔名
                    picMain = IOManage.FileNewName;

                    //暫存檔案資訊
                    ITempList.Add(new TempParam(picMain, IOManage.FileFullName, this.hf_Pic.Value, hpFile, "P1", true));

                }
                else
                {
                    errExt++;
                }
            }
        }

        //500圖
        HttpPostedFile hpFile500 = this.file_500.PostedFile;
        if (hpFile500 != null)
        {
            if (hpFile500.ContentLength > 0)
            {
                //[IO] - 取得檔案名稱
                IOManage.GetFileName(hpFile500);

                //判斷副檔名，未符合規格的檔案不上傳
                if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 2))
                {
                    //設定檔名
                    string myFileName = ThumbFrontFileName_500 + picMain;

                    //暫存檔案資訊
                    ITempList.Add(new TempParam(myFileName, IOManage.FileFullName, this.hf_Pic500.Value, hpFile500, "P2", true));

                }
                else
                {
                    errExt++;
                }
            }
        }

        //1000圖
        HttpPostedFile hpFile1000 = this.file_1000.PostedFile;
        if (hpFile1000 != null)
        {
            if (hpFile1000.ContentLength > 0)
            {
                //[IO] - 取得檔案名稱
                IOManage.GetFileName(hpFile1000);

                //判斷副檔名，未符合規格的檔案不上傳
                if (fn_Extensions.CheckStrWord(IOManage.FileExtend, FileExtLimit, "|", 2))
                {
                    //設定檔名
                    string myFileName = ThumbFrontFileName_1000 + picMain;

                    //暫存檔案資訊
                    ITempList.Add(new TempParam(myFileName, IOManage.FileFullName, this.hf_Pic1000.Value, hpFile1000, "P3", true));

                }
                else
                {
                    errExt++;
                }
            }
        }

        //未符合檔案規格的警示訊息
        if (errExt > 0)
        {
            fn_Extensions.JsAlert("上傳內容含有不正確的副檔名\\n請重新挑選!!", "");
            return;
        }

        #endregion


        //判斷是否有資料
        if (string.IsNullOrEmpty(this.hf_PicID.Value))
        {
            //Insert
            Add_Data(ITempList);
        }
        else
        {
            //Update
            Edit_Data(ITempList);
        }
    }


    /// <summary>
    /// 資料新增
    /// </summary>
    /// <param name="PicTmp">圖片暫存參數</param>
    private void Add_Data(List<TempParam> PicTmp)
    {
        string ErrMsg;
        string picMain = "", picName = "", pic500 = "", pic1000 = "";

        //取得圖片參數
        var queryPic = from el in PicTmp
                       select new
                       {
                           NewPic = el.Param_Pic,
                           OrgPicName = el.Param_OrgPic,
                           PicKind = el.Param_FileKind
                       };
        foreach (var item in queryPic)
        {
            if (item.PicKind.Equals("P1"))
            {
                picMain = item.NewPic;
                picName = item.OrgPicName;
            }

            if (item.PicKind.Equals("P2"))
            {
                pic500 = item.NewPic;
            }

            if (item.PicKind.Equals("P3"))
            {
                pic1000 = item.NewPic;
            }
        }

        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();


            //判斷圖片類別
            switch (Param_Class)
            {
                case "1":
                    //[SQL] - 資料新增, 產品圖
                    SBSql.AppendLine("IF (SELECT COUNT(*) FROM ProdPic_Photo WHERE (Model_No = @Param_ModelNo)) = 0 ");
                    SBSql.AppendLine(" BEGIN ");

                    SBSql.AppendLine(" Declare @New_ID AS INT SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Photo) ");
                    SBSql.AppendLine(" INSERT INTO ProdPic_Photo( ");
                    SBSql.AppendLine("  Pic_ID, Model_No, Create_Who, Create_Time");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , {0}, {0}_OrgFile, {0}_UpdTime".FormatThis(Param_Column));
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Param_ModelNo, @Param_CreateWho, GETDATE()");
                    //[SQL] - 圖片參數欄位
                    SBSql.AppendLine("  , @Param_Pic, @Param_OrgPic, GETDATE()");
                    SBSql.AppendLine(" )");

                    SBSql.AppendLine(" END ");

                    break;

                case "2":
                    //[SQL] - 資料新增, 產品輔圖
                    SBSql.AppendLine("IF (SELECT COUNT(*) FROM ProdPic_Figure WHERE (Model_No = @Param_ModelNo)) = 0 ");
                    SBSql.AppendLine(" BEGIN ");

                    SBSql.AppendLine(" Declare @New_ID AS INT SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Figure) ");
                    SBSql.AppendLine(" INSERT INTO ProdPic_Figure( ");
                    SBSql.AppendLine("  Pic_ID, Model_No, Create_Who, Create_Time");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , {0}, {0}_OrgFile, {0}_UpdTime".FormatThis(Param_Column));
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Param_ModelNo, @Param_CreateWho, GETDATE()");
                    //[SQL] - 圖片參數欄位
                    SBSql.AppendLine("  , @Param_Pic, @Param_OrgPic, GETDATE()");
                    SBSql.AppendLine(" )");

                    SBSql.AppendLine(" END ");

                    break;

                default:
                    //[SQL] - 資料新增, 其他
                    SBSql.AppendLine(" Declare @New_ID AS INT SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Group WHERE (RTRIM(Model_No) = @Param_ModelNo)) ");
                    SBSql.AppendLine(" INSERT INTO ProdPic_Group( ");
                    SBSql.AppendLine("  Pic_ID, Pic_Class, Lang, Model_No, Create_Who, Create_Time");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , Pic_File, Pic_OrgFile");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine("  @New_ID, @Param_Class, @Param_Lang, @Param_ModelNo, @Param_CreateWho, GETDATE()");
                    //[SQL] - 圖片參數欄位
                    SBSql.AppendLine("  , @Param_Pic, @Param_OrgPic");
                    SBSql.AppendLine(" )");

                    cmd.Parameters.AddWithValue("Param_Class", Param_Class);
                    cmd.Parameters.AddWithValue("Param_Lang", this.hf_Lang.Value);

                    break;
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
            cmd.Parameters.AddWithValue("Param_CreateWho", fn_Param.CurrentAccount.ToString());
            cmd.Parameters.AddWithValue("Param_Pic", picMain);
            cmd.Parameters.AddWithValue("Param_OrgPic", picName);
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料新增失敗！", "");
            }
            else
            {
                //寫入Log
                fn_Log.Log_Rec("圖片資料庫"
                    , Param_ModelNo
                    , "新增圖檔,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                    , fn_Param.CurrentAccount.ToString());


                //取得圖片指定長寬
                int[] mySizeDef = GetImgSize("Main");
                int[] mySize500 = GetImgSize("500");
                int[] mySize1000 = GetImgSize("1000");

                //LogoType
                string getLogo = this.rbl_LogoType.SelectedValue;

                //[IO] - 儲存檔案
                for (int row = 0; row < PicTmp.Count; row++)
                {
                    HttpPostedFile hpf = PicTmp[row].Param_hpf;

                    string fileKind = PicTmp[row].Param_FileKind;
                    int width, height;
                    string waterImg;
                    bool isResize = PicTmp[row].IsResize;

                    if (hpf.ContentLength > 0)
                    {
                        switch (fileKind)
                        {
                            case "P2":
                                width = mySize500[0];
                                height = mySize500[1];
                                waterImg = getLogo.Equals("0") ? "" : @"{0}{1}\logo_500.png".FormatThis(WaterImg_FileFolder, LogoType(getLogo));

                                break;

                            case "P3":
                                width = mySize1000[0];
                                height = mySize1000[1];
                                waterImg = getLogo.Equals("0") ? "" : @"{0}{1}\logo_1000.png".FormatThis(WaterImg_FileFolder, LogoType(getLogo));

                                break;

                            default:
                                width = mySizeDef[0];
                                height = mySizeDef[1];
                                waterImg = "";

                                break;
                        }

                        //Save file
                        IOManage.Save(hpf, Param_FileFolder, PicTmp[row].Param_Pic, width, height, waterImg, isResize);
                    }
                }

                if (PicTmp.Count > 0)
                {
                    //更新圖片集Zip
                    Update_ZipFiles(Param_ModelNo, Param_Class);
                }

                //redirect
                fn_Extensions.JsAlert("資料新增成功！", PageUrl);
            }
        }
    }

    /// <summary>
    /// 資料更新
    /// </summary>
    /// <param name="PicTmp">圖片暫存參數</param>
    private void Edit_Data(List<TempParam> PicTmp)
    {
        string ErrMsg;
        string picMain = this.hf_Pic.Value, picName = this.hf_OrgPic.Value, pic500 = "", pic1000 = "";

        //取得圖片參數
        var queryPic = from el in PicTmp
                       select new
                       {
                           NewPic = el.Param_Pic,
                           OrgPicName = el.Param_OrgPic,
                           PicKind = el.Param_FileKind
                       };
        foreach (var item in queryPic)
        {
            if (item.PicKind.Equals("P1"))
            {
                picMain = item.NewPic;
                picName = item.OrgPicName;
            }

            if (item.PicKind.Equals("P2"))
            {
                pic500 = item.NewPic;
            }

            if (item.PicKind.Equals("P3"))
            {
                pic1000 = item.NewPic;
            }
        }

        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //判斷圖片類別
            switch (Param_Class)
            {
                case "1":
                    //[SQL] - 資料更新, 產品圖
                    SBSql.AppendLine(" UPDATE ProdPic_Photo ");
                    SBSql.AppendLine(" SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , {0} = @Param_Pic, {0}_OrgFile = @Param_OrgPic, {0}_UpdTime = GETDATE()".FormatThis(Param_Column));
                    SBSql.AppendLine(" WHERE (Pic_ID = @Param_PicID) AND (Model_No = @Param_ModelNo)");

                    break;

                case "2":
                    //[SQL] - 資料更新, 產品輔圖
                    SBSql.AppendLine(" UPDATE ProdPic_Figure ");
                    SBSql.AppendLine(" SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , {0} = @Param_Pic, {0}_OrgFile = @Param_OrgPic, {0}_UpdTime = GETDATE()".FormatThis(Param_Column));
                    SBSql.AppendLine(" WHERE (Pic_ID = @Param_PicID) AND (Model_No = @Param_ModelNo)");

                    break;

                default:
                    //[SQL] - 資料更新, 其他
                    SBSql.AppendLine(" UPDATE ProdPic_Group ");
                    SBSql.AppendLine(" SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE(), Lang = @Param_Lang ");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , Pic_File = @Param_Pic, Pic_OrgFile = @Param_OrgPic");
                    SBSql.AppendLine(" WHERE (Pic_ID = @Param_PicID) AND (Pic_Class = @Param_Class) AND (Model_No = @Param_ModelNo)");

                    cmd.Parameters.AddWithValue("Param_Class", Param_Class);
                    cmd.Parameters.AddWithValue("Param_Lang", this.hf_Lang.Value);
                    break;
            }

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_PicID", this.hf_PicID.Value);
            cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
            cmd.Parameters.AddWithValue("Param_Pic", picMain);
            cmd.Parameters.AddWithValue("Param_OrgPic", picName);
            cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("資料更新失敗！", "");
            }
            else
            {
                //取得圖片指定長寬
                int[] mySizeDef = GetImgSize("Main");
                int[] mySize500 = GetImgSize("500");
                int[] mySize1000 = GetImgSize("1000");

                //LogoType
                string getLogo = this.rbl_LogoType.SelectedValue;

                //[IO] - 儲存檔案
                for (int row = 0; row < PicTmp.Count; row++)
                {
                    //刪除原本的檔案
                    IOManage.DelFile(Param_FileFolder, PicTmp[row].Param_CurrentPic);


                    //儲存新的檔案
                    HttpPostedFile hpf = PicTmp[row].Param_hpf;

                    string fileKind = PicTmp[row].Param_FileKind;
                    int width, height;
                    string waterImg;
                    bool isResize = PicTmp[row].IsResize;

                    if (hpf.ContentLength > 0)
                    {
                        switch (fileKind)
                        {
                            case "P2":
                                width = mySize500[0];
                                height = mySize500[1];
                                waterImg = getLogo.Equals("0") ? "" : @"{0}{1}\logo_500.png".FormatThis(WaterImg_FileFolder, LogoType(getLogo));

                                break;

                            case "P3":
                                width = mySize1000[0];
                                height = mySize1000[1];
                                waterImg = getLogo.Equals("0") ? "" : @"{0}{1}\logo_1000.png".FormatThis(WaterImg_FileFolder, LogoType(getLogo));

                                break;

                            default:
                                width = mySizeDef[0];
                                height = mySizeDef[1];
                                waterImg = "";

                                break;
                        }

                        //Save file
                        IOManage.Save(hpf, Param_FileFolder, PicTmp[row].Param_Pic, width, height, waterImg, isResize);
                    }
                }

                if (PicTmp.Count > 0)
                {
                    //更新圖片集Zip
                    Update_ZipFiles(Param_ModelNo, Param_Class);
                }


                //[其他處理]
                if (PicTmp.Count > 0)
                {
                    //[取得參數] - 關聯品號集合
                    if (!string.IsNullOrEmpty(this.hf_RelModel.Value))
                    {
                        string[] strAry_Prods = Regex.Split(this.hf_RelModel.Value, @"\|{1}");

                        //更新關聯品號
                        if (false == CopyPic(strAry_Prods, picMain, picName, out ErrMsg))
                        {
                            fn_Extensions.JsAlert("關聯資料已更新，但檔案處理失敗！", "");
                            return;
                        }
                    }

                    //更新圖片集Zip
                    Update_ZipFiles(Param_ModelNo, Param_Class);
                }

                //寫入Log
                fn_Log.Log_Rec("圖片資料庫"
                    , Param_ModelNo
                    , "修改圖檔,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                    , fn_Param.CurrentAccount.ToString());

                //redirect
                fn_Extensions.JsAlert("資料更新成功！", PageUrl);
            }
        }
    }


    /// <summary>
    /// 刪除 - 目前品號
    /// </summary>
    protected void btn_Del_Click(object sender, EventArgs e)
    {
        try
        {
            DelItem();
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 刪除！", "");
        }
    }


    /// <summary>
    /// 刪除目前品號的圖及關聯性
    /// </summary>
    private void DelItem()
    {
        string ErrMsg;
        using (SqlCommand cmd = new SqlCommand())
        {
            //宣告
            StringBuilder SBSql = new StringBuilder();

            //[SQL] - 清除參數設定
            cmd.Parameters.Clear();

            //判斷圖片類別
            switch (Param_Class)
            {
                case "1":
                    //[SQL] - 資料更新, 產品圖
                    SBSql.AppendLine(" UPDATE ProdPic_Photo SET ");
                    SBSql.AppendLine("  Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , {0} = '', {0}_OrgFile = '', {0}_UpdTime = NULL".FormatThis(Param_Column));
                    SBSql.AppendLine(" WHERE (Pic_ID = @Param_PicID) AND (Model_No = @Param_ModelNo); ");

                    break;

                case "2":
                    //[SQL] - 資料更新, 產品輔圖
                    SBSql.AppendLine(" UPDATE ProdPic_Figure SET ");
                    SBSql.AppendLine("  Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                    //[SQL] - 圖片欄位
                    SBSql.AppendLine("  , {0} = '', {0}_OrgFile = '', {0}_UpdTime = NULL".FormatThis(Param_Column));
                    SBSql.AppendLine(" WHERE (Pic_ID = @Param_PicID) AND (Model_No = @Param_ModelNo); ");

                    break;

                default:
                    //[SQL] - 資料更新, 其他
                    SBSql.AppendLine(" DELETE FROM ProdPic_Group WHERE (Pic_ID = @Param_PicID) AND (Pic_Class = @Pic_Class) AND (RTRIM(Model_No) = @Param_ModelNo); ");

                    break;
            }

            //[SQL] - 刪除關聯 (目前品號)
            SBSql.AppendLine(" DELETE FROM ProdPic_Rel WHERE (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) AND (Model_No = @Param_ModelNo)");

            //[SQL] - Command
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Param_PicID", this.hf_PicID.Value);
            cmd.Parameters.AddWithValue("Param_ModelNo", Param_ModelNo);
            cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
            cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
            cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
            if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("刪除失敗！", "");
            }
            else
            {
                //寫入Log
                fn_Log.Log_Rec("圖片資料庫"
                    , Param_ModelNo
                    , "刪除圖及關聯性,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                    , fn_Param.CurrentAccount.ToString());

                //刪除檔案
                IOManage.DelFile(Param_FileFolder, this.hf_Pic.Value);
                //刪除檔案(縮圖)
                IOManage.DelFile(Param_FileFolder, this.hf_Pic500.Value);
                IOManage.DelFile(Param_FileFolder, this.hf_Pic1000.Value);

                //更新圖片集Zip
                Update_ZipFiles(Param_ModelNo, Param_Class);

                fn_Extensions.JsAlert("刪除成功！", PageUrl);
            }
        }
    }


    /// <summary>
    /// 儲存關聯
    /// 新增目標品號關聯
    /// </summary>
    /// 
    protected void lbtn_AddRel_Click(object sender, EventArgs e)
    {
        try
        {
            string ErrMsg;

            #region >> 欄位檢查 <<
            StringBuilder SBAlert = new StringBuilder();

            //[參數檢查] - 品號
            string ProdIDs = this.tb_GroupVal_IDs.Text;
            if (string.IsNullOrEmpty(ProdIDs))
            {
                SBAlert.Append("未輸入「品號」\\n");
            }

            //[參數檢查] - 主要圖片
            string GetPic = this.hf_Pic.Value;
            if (string.IsNullOrEmpty(GetPic))
            {
                SBAlert.Append("請先上傳「圖檔」\\n");
            }

            //[JS] - 判斷是否有警示訊息
            if (string.IsNullOrEmpty(SBAlert.ToString()) == false)
            {
                fn_Extensions.JsAlert(SBAlert.ToString(), "");
                return;
            }

            //[取得參數] - 資料集合
            string[] strAry_Prods = Regex.Split(ProdIDs, @"\|{1}");
            List<string> strList_Prods = strAry_Prods.ToList();

            #endregion

            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                List<string> Model_inRel = new List<string>();  //有被關聯的品號

                #region >> 列出已被關聯的目標品號 <<

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 列出已被關聯的目標品號, 不會被Insert
                SBSql.AppendLine(" SELECT RTRIM(Model_No) AS ModelNo ");
                SBSql.AppendLine(" FROM ProdPic_Rel ");
                SBSql.AppendLine(" WHERE (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) AND (Rel_ID <> @Rel_ID) ");
                SBSql.AppendLine("  AND RTRIM(Model_No) IN ({0}) ".FormatThis(GetSQLParam(strAry_Prods.ToList<string>(), "ParamTmp")));
                for (int row = 0; row < strAry_Prods.Length; row++)
                {
                    //代入參數 - 目標品號
                    cmd.Parameters.AddWithValue("ParamTmp" + row, strAry_Prods[row]);
                }
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
                cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
                cmd.Parameters.AddWithValue("Rel_ID", this.hf_RelID.Value);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        //暫存不會被Insert的品號
                        StringBuilder unRelHtml = new StringBuilder();
                        unRelHtml.AppendLine("<ul>");

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            string Get_Model = DT.Rows[row]["ModelNo"].ToString();
                            unRelHtml.AppendLine("<li>{0}</li>".FormatThis(Get_Model));

                            if (strList_Prods.Contains(Get_Model))
                            {
                                //有被關聯
                                Model_inRel.Add(Get_Model);
                                //清掉有被關聯的品號
                                strList_Prods.Remove(Get_Model);
                            }
                        }

                        unRelHtml.AppendLine("</ul>");

                        //暫存至Session (PicRelItems)
                        Session["PicRelItems"] = unRelHtml;

                    }

                }

                #endregion

                //判斷有沒有要新增的關聯品號
                if (strList_Prods.Count == 0)
                {
                    Response.Redirect(PageUrl);
                    return;
                }

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                SBSql.Clear();

                //[SQL] - 判斷來源品號是否有建立關聯
                SBSql.AppendLine(" DECLARE @RelID AS INT, @RelCnt AS INT, @New_ID AS INT ");
                SBSql.AppendLine(" SET @RelID = (SELECT Rel_ID FROM ProdPic_Rel WHERE (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) AND (Model_No = @Src_ModelNo)) ");
                SBSql.AppendLine(" SET @RelCnt = (SELECT COUNT(Rel_ID) FROM ProdPic_Rel WHERE (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) AND (Model_No = @Src_ModelNo)) ");
                SBSql.AppendLine(" IF @RelCnt = 0 ");
                SBSql.AppendLine("  BEGIN ");
                SBSql.AppendLine("     SET @New_ID = (SELECT ISNULL(MAX(Rel_ID), 0) + 1 FROM ProdPic_Rel) ");
                SBSql.AppendLine("     INSERT INTO ProdPic_Rel( ");
                SBSql.AppendLine("         Rel_ID, Pic_Class, Pic_Column, Model_No ");
                SBSql.AppendLine("     ) VALUES ( ");
                SBSql.AppendLine("         @New_ID, @Pic_Class, @Pic_Column, @Src_ModelNo ");
                SBSql.AppendLine("     ) ");
                SBSql.AppendLine("     SET @RelID = @New_ID ");
                SBSql.AppendLine("  END ");

                //[SQL] - 判斷目標品號是否已建立關聯, 不存在則Insert
                //若目標品號已被其他品號關聯，則不會Insert
                for (int row = 0; row < strList_Prods.Count; row++)
                {
                    SBSql.AppendLine(" IF( ");
                    SBSql.AppendLine("    SELECT COUNT(*)");
                    SBSql.AppendLine("    FROM ProdPic_Rel");
                    SBSql.AppendLine("    WHERE (Pic_Class = @Pic_Class) AND (Pic_Column = @Pic_Column) AND (Model_No = @ModelNo_{0})".FormatThis(row));
                    SBSql.AppendLine(" ) = 0 ");
                    SBSql.AppendLine(" BEGIN ");
                    //[SQL] - 新增關聯
                    SBSql.AppendLine("  INSERT INTO ProdPic_Rel(Rel_ID, Pic_Class, Pic_Column, Model_No) ");
                    SBSql.AppendLine("  VALUES (@RelID, @Pic_Class, @Pic_Column, @ModelNo_{0});".FormatThis(row));

                    cmd.Parameters.AddWithValue("ModelNo_{0}".FormatThis(row), strList_Prods[row].ToString());
                    SBSql.AppendLine(" END; ");
                }

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
                cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
                cmd.Parameters.AddWithValue("Src_ModelNo", Param_ModelNo);
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("關聯設定失敗！", "");
                    return;
                }
                else
                {
                    if (false == CopyPic(strList_Prods.ToArray(), this.hf_Pic.Value, hf_OrgPic.Value, out ErrMsg))
                    {
                        fn_Extensions.JsAlert("關聯已設定，但檔案處理失敗！", "");
                        return;
                    }
                    else
                    {
                        //寫入Log
                        fn_Log.Log_Rec("圖片資料庫"
                            , Param_ModelNo
                            , "設定關聯,品號:{0}, 圖片類別:{1}".FormatThis(Param_ModelNo, Param_Class)
                            , fn_Param.CurrentAccount.ToString());

                        fn_Extensions.JsAlert("關聯設定成功！", PageUrl);
                        return;
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 儲存關聯！", "");
        }
    }


    #endregion


    #region -- 自訂功能 --
    /// <summary>
    /// 圖檔處理
    /// Del目標品號圖片 , Update目標品號圖片欄位, Copy來源品號圖片至目標品號
    /// </summary>
    /// <param name="strAry_Prods">品號</param>
    /// <param name="srcFileName">來源檔案名稱(真實檔名)</param>
    /// <param name="srcOldFileName">來源檔案名稱(上傳檔名)</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool CopyPic(string[] strAry_Prods, string srcFileName, string srcOrgFileName, out string ErrMsg)
    {
        try
        {
            if (strAry_Prods.Length == 0)
            {
                ErrMsg = "";
                return true;
            }

            //參數宣告
            StringBuilder SBSql = new StringBuilder();
            //List<string> Temp_PicID = new List<string>();
            List<string> Temp_PicFile = new List<string>();
            List<string> Temp_ModelNo = new List<string>();

            #region >> 刪除目標品號圖片 <<
            //[SQL] - 取得圖片檔名, PicID
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                cmd.Parameters.Clear();

                //判斷圖片類別
                switch (Param_Class)
                {
                    case "1":
                        //[SQL] - 資料查詢, 產品圖
                        SBSql.AppendLine(" SELECT Pic_ID, RTRIM(Model_No) AS ModelNo, {0} AS PicFile ".FormatThis(Param_Column));
                        SBSql.AppendLine(" FROM ProdPic_Photo ");
                        SBSql.AppendLine(" WHERE RTRIM(Model_No) IN ({0}) ".FormatThis(GetSQLParam(strAry_Prods.ToList<string>(), "ParamTmp")));

                        break;

                    case "2":
                        //[SQL] - 資料查詢, 產品輔圖
                        SBSql.AppendLine(" SELECT Pic_ID, RTRIM(Model_No) AS ModelNo, {0} AS PicFile ".FormatThis(Param_Column));
                        SBSql.AppendLine(" FROM ProdPic_Figure ");
                        SBSql.AppendLine(" WHERE RTRIM(Model_No) IN ({0}) ".FormatThis(GetSQLParam(strAry_Prods.ToList<string>(), "ParamTmp")));

                        break;

                    default:
                        //[SQL] - 資料查詢, 其他
                        SBSql.AppendLine(" SELECT Pic_ID, RTRIM(Model_No) AS ModelNo, Pic_File AS PicFile ");
                        SBSql.AppendLine(" FROM ProdPic_Group ");
                        SBSql.AppendLine(" WHERE RTRIM(Model_No) IN ({0}) AND (Pic_Class = @Pic_Class) AND (Pic_ID = @Pic_Column) ".FormatThis(GetSQLParam(strAry_Prods.ToList<string>(), "ParamTmp")));

                        cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
                        cmd.Parameters.AddWithValue("Pic_Class", Param_Class);

                        break;
                }

                for (int row = 0; row < strAry_Prods.Length; row++)
                {
                    //代入參數 - 目標品號
                    cmd.Parameters.AddWithValue("ParamTmp" + row, strAry_Prods[row]);
                }
                cmd.CommandText = SBSql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        //Temp_PicID.Add(DT.Rows[row]["Pic_ID"].ToString());      //暫存PicID
                        Temp_PicFile.Add(DT.Rows[row]["PicFile"].ToString());   //暫存圖片檔名
                        Temp_ModelNo.Add(DT.Rows[row]["ModelNo"].ToString());   //暫存ModelNo
                    }
                }
            }
            #endregion

            #region >> Update目標品號圖片欄位 <<
            using (SqlCommand cmd = new SqlCommand())
            {
                //清除參數
                SBSql.Clear();
                cmd.Parameters.Clear();

                //判斷資料是否已新增
                SBSql.AppendLine(" Declare @New_ID AS INT ");
                for (int row = 0; row < strAry_Prods.Length; row++)
                {
                    //判斷圖片類別
                    switch (Param_Class)
                    {
                        case "1":
                            //[SQL] - 資料更新, 產品圖
                            SBSql.AppendLine(" IF( ");
                            SBSql.AppendLine("    SELECT COUNT(*)");
                            SBSql.AppendLine("    FROM ProdPic_Photo");
                            SBSql.AppendLine("    WHERE (Model_No = @ModelNo_{0})".FormatThis(row));
                            SBSql.AppendLine(" ) = 0 ");
                            SBSql.AppendLine("  BEGIN ");
                            //----- Insert Start -----
                            SBSql.AppendLine("    SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Photo) ");
                            SBSql.AppendLine("    INSERT INTO ProdPic_Photo(Pic_ID, Model_No");
                            SBSql.AppendLine("     , {0}, {0}_OrgFile, {0}_UpdTime".FormatThis(Param_Column));
                            SBSql.AppendLine("     , Create_Who, Create_Time");
                            SBSql.AppendLine("    ) VALUES ( ");
                            SBSql.AppendLine("     @New_ID, @ModelNo_{0}".FormatThis(row));
                            SBSql.AppendLine("     , @Param_Pic, @Param_OrgPic, GETDATE()");
                            SBSql.AppendLine("     , @Param_UpdateWho, GETDATE() ) ");
                            //----- Insert End -----
                            SBSql.AppendLine("  END ");
                            SBSql.AppendLine(" ELSE ");
                            SBSql.AppendLine("  BEGIN ");
                            //----- Update Start -----
                            SBSql.AppendLine("    UPDATE ProdPic_Photo ");
                            SBSql.AppendLine("    SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                            SBSql.AppendLine("     , {0} = @Param_Pic, {0}_OrgFile = @Param_OrgPic, {0}_UpdTime = GETDATE()".FormatThis(Param_Column));
                            SBSql.AppendLine("    WHERE (Model_No = @ModelNo_{0}); ".FormatThis(row));
                            //----- Update End -----
                            SBSql.AppendLine("  END ");

                            break;

                        case "2":
                            //[SQL] - 資料更新, 產品輔圖
                            SBSql.AppendLine(" IF( ");
                            SBSql.AppendLine("    SELECT COUNT(*)");
                            SBSql.AppendLine("    FROM ProdPic_Figure");
                            SBSql.AppendLine("    WHERE (Model_No = @ModelNo_{0})".FormatThis(row));
                            SBSql.AppendLine(" ) = 0 ");
                            SBSql.AppendLine("  BEGIN ");
                            //----- Insert Start -----
                            SBSql.AppendLine("    SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Figure) ");
                            SBSql.AppendLine("    INSERT INTO ProdPic_Figure(Pic_ID, Model_No");
                            SBSql.AppendLine("     , {0}, {0}_OrgFile, {0}_UpdTime".FormatThis(Param_Column));
                            SBSql.AppendLine("     , Create_Who, Create_Time");
                            SBSql.AppendLine("    ) VALUES ( ");
                            SBSql.AppendLine("     @New_ID, @ModelNo_{0}".FormatThis(row));
                            SBSql.AppendLine("     , @Param_Pic, @Param_OrgPic, GETDATE()");
                            SBSql.AppendLine("     , @Param_UpdateWho, GETDATE() ) ");
                            //----- Insert End -----
                            SBSql.AppendLine("  END ");
                            SBSql.AppendLine(" ELSE ");
                            SBSql.AppendLine("  BEGIN ");
                            //----- Update Start -----
                            SBSql.AppendLine("    UPDATE ProdPic_Figure ");
                            SBSql.AppendLine("    SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE() ");
                            SBSql.AppendLine("     , {0} = @Param_Pic, {0}_OrgFile = @Param_OrgPic, {0}_UpdTime = GETDATE()".FormatThis(Param_Column));
                            SBSql.AppendLine("    WHERE (Model_No = @ModelNo_{0}); ".FormatThis(row));
                            //----- Update End -----
                            SBSql.AppendLine("  END ");

                            break;

                        default:
                            //[SQL] - 資料更新, 其他
                            SBSql.AppendLine(" IF( ");
                            SBSql.AppendLine("    SELECT COUNT(*)");
                            SBSql.AppendLine("    FROM ProdPic_Group");
                            SBSql.AppendLine("    WHERE (Model_No = @ModelNo_{0}) AND (Pic_Class = @Pic_Class) AND (Pic_ID = @Pic_Column)".FormatThis(row));
                            SBSql.AppendLine(" ) = 0 ");
                            SBSql.AppendLine("  BEGIN ");
                            //----- Insert Start -----
                            //SBSql.AppendLine("    SET @New_ID = (SELECT ISNULL(MAX(Pic_ID), 0) + 1 FROM ProdPic_Group WHERE (RTRIM(Model_No) = @ModelNo_{0})) ".FormatThis(row));
                            SBSql.AppendLine("    INSERT INTO ProdPic_Group(Pic_ID, Pic_Class, Lang, Model_No, Create_Who, Create_Time");
                            SBSql.AppendLine("     , Pic_File, Pic_OrgFile");
                            SBSql.AppendLine("    ) VALUES ( ");
                            SBSql.AppendLine("     @Pic_Column, @Pic_Class, @Param_Lang, @ModelNo_{0}, @Param_UpdateWho, GETDATE()".FormatThis(row));
                            SBSql.AppendLine("     , @Param_Pic, @Param_OrgPic");
                            SBSql.AppendLine("    ) ");
                            //----- Insert End -----
                            SBSql.AppendLine("  END ");
                            SBSql.AppendLine(" ELSE ");
                            SBSql.AppendLine("  BEGIN ");
                            //----- Update Start -----
                            SBSql.AppendLine("    UPDATE ProdPic_Group ");
                            SBSql.AppendLine("    SET Update_Who = @Param_UpdateWho, Update_Time = GETDATE(), Lang = @Param_Lang ");
                            SBSql.AppendLine("     , Pic_File = @Param_Pic, Pic_OrgFile = @Param_OrgPic");
                            SBSql.AppendLine("    WHERE (Model_No = @ModelNo_{0}) AND (Pic_Class = @Pic_Class) AND (Pic_ID = @Pic_Column) ; ".FormatThis(row));
                            //----- Update End -----
                            SBSql.AppendLine("  END ");

                            break;
                    }

                    //代入參數 - 目標品號
                    cmd.Parameters.AddWithValue("ModelNo_" + row, strAry_Prods[row]);
                }
                cmd.Parameters.AddWithValue("Param_Lang", this.hf_Lang.Value);
                cmd.Parameters.AddWithValue("Pic_Class", Param_Class);
                cmd.Parameters.AddWithValue("Pic_Column", Param_Column);
                cmd.Parameters.AddWithValue("Param_Pic", srcFileName);
                cmd.Parameters.AddWithValue("Param_OrgPic", srcOrgFileName);
                cmd.Parameters.AddWithValue("Param_UpdateWho", fn_Param.CurrentAccount.ToString());
                cmd.CommandText = SBSql.ToString();
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    return false;
                }
            }
            #endregion

            #region >> Copy來源品號圖片至目標品號 <<
            //刪除「目標品號」原本的檔案
            if (Temp_PicFile.Count > 0)
            {
                for (int row = 0; row < Temp_PicFile.Count; row++)
                {
                    string fileName = Temp_PicFile[row];

                    //[IO] - 刪除舊檔案(檔案路徑, 舊檔案名稱)
                    IOManage.DelFile(
                        Application["File_DiskUrl"] + @"ProductPic\{0}\{1}\".FormatThis(Temp_ModelNo[row].ToString(), Param_Class)
                        , fileName);

                    IOManage.DelFile(
                        Application["File_DiskUrl"] + @"ProductPic\{0}\{1}\".FormatThis(Temp_ModelNo[row].ToString(), Param_Class)
                        , ThumbFrontFileName_500 + fileName);
                    IOManage.DelFile(
                        Application["File_DiskUrl"] + @"ProductPic\{0}\{1}\".FormatThis(Temp_ModelNo[row].ToString(), Param_Class)
                        , ThumbFrontFileName_1000 + fileName);
                }

            }

            //來源檔案名稱
            string filename = srcFileName;
            string fullPath = Param_FileFolder;

            //取得完整檔案路徑
            string FullFileName = System.IO.Path.Combine(fullPath, filename);

            //「目標品號」Loop
            for (int row = 0; row < strAry_Prods.Length; row++)
            {
                //各品號資料夾
                string targetPath = Application["File_DiskUrl"] + @"ProductPic\{0}\{1}".FormatThis(strAry_Prods[row].ToString(), Param_Class);
                string targetFile = System.IO.Path.Combine(targetPath, filename);
                //判斷目標資料夾是否存在
                if (!System.IO.Directory.Exists(targetPath))
                {
                    System.IO.Directory.CreateDirectory(targetPath);
                }
                //[IO] - 複製檔案
                System.IO.File.Copy(FullFileName, targetFile, true);

                //更新圖片集Zip
                Update_ZipFiles(strAry_Prods[row].ToString(), Param_Class);
            }

            //縮圖500
            string thumbFile = ThumbFrontFileName_500 + filename;
            FullFileName = System.IO.Path.Combine(fullPath, thumbFile);
            for (int row = 0; row < strAry_Prods.Length; row++)
            {
                //各品號資料夾
                string targetPath = Application["File_DiskUrl"] + @"ProductPic\{0}\{1}".FormatThis(strAry_Prods[row].ToString(), Param_Class);
                string targetFile = System.IO.Path.Combine(targetPath, thumbFile);
                //判斷目標資料夾是否存在
                if (!System.IO.Directory.Exists(targetPath))
                {
                    System.IO.Directory.CreateDirectory(targetPath);
                }

                if (System.IO.File.Exists(FullFileName))
                {
                    //[IO] - 複製檔案
                    System.IO.File.Copy(FullFileName, targetFile, true);
                }


            }

            //縮圖1000
            string thumbFile_1000 = ThumbFrontFileName_1000 + filename;
            FullFileName = System.IO.Path.Combine(fullPath, thumbFile_1000);
            for (int row = 0; row < strAry_Prods.Length; row++)
            {
                //各品號資料夾
                string targetPath = Application["File_DiskUrl"] + @"ProductPic\{0}\{1}".FormatThis(strAry_Prods[row].ToString(), Param_Class);
                string targetFile = System.IO.Path.Combine(targetPath, thumbFile_1000);
                //判斷目標資料夾是否存在
                if (!System.IO.Directory.Exists(targetPath))
                {
                    System.IO.Directory.CreateDirectory(targetPath);
                }

                if (System.IO.File.Exists(FullFileName))
                {
                    //[IO] - 複製檔案
                    System.IO.File.Copy(FullFileName, targetFile, true);
                }


            }
            #endregion

            return true;
        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">真實檔名</param>
    /// <param name="OrgPicName">原始檔名</param>
    /// <returns>bool</returns>
    private string PicUrl(string PicName, string OrgPicName)
    {
        try
        {
            //判斷空值
            if (string.IsNullOrEmpty(PicName))
            {
                return "";
            }

            //宣告
            string preView = "";
            //Web資料夾路徑
            string WebFolder = Application["File_WebUrl"] + @"ProductPic/";

            //判斷是否為圖片
            string strFileExt = ".jpg||.png||.gif";
            if (fn_Extensions.CheckStrWord(PicName, strFileExt, "|", 2))
            {
                //圖片預覽(Server資料夾/ProductPic/型號/圖片類別/圖片)
                preView = string.Format(
                     "<img src=\"{0}\" class=\"materialboxed\" alt=\"{1}\">"
                    , WebFolder + Server.UrlEncode(Param_ModelNo) + "/" + Param_Class + "/" + PicName
                    , OrgPicName);
            }
            else
            {
                //非圖片，顯示下載連結
                preView = string.Format(
                   "<div style=\"padding:20px 0 10px 0\"><a href=\"{3}/FileDownload.ashx?OrgiName={2}&FilePath={1}\">&gt;&gt; {0} &lt;&lt;</a></div>"
                   , "點我下載"
                   , Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + PicName))
                   , Server.UrlEncode(OrgPicName)
                   , Application["WebUrl"]
                   );
            }

            //輸出Html
            return preView;

        }
        catch (Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 回傳對應的Logo樣式資料夾
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string LogoType(string type)
    {
        switch (type)
        {
            case "1":
                return "PKWeb";

            case "2":
                return "Robot";

            default:
                return "Science";

        }
    }

    /// <summary>
    /// SQL參數組合 - Where IN
    /// </summary>
    /// <param name="listSrc">來源資料(List)</param>
    /// <param name="paramName">參數名稱</param>
    /// <returns>參數字串</returns>
    string GetSQLParam(List<string> listSrc, string paramName)
    {
        if (listSrc.Count == 0)
        {
            return "";
        }

        //組合參數字串
        ArrayList aryParam = new ArrayList();
        for (int row = 0; row < listSrc.Count; row++)
        {
            aryParam.Add(string.Format("@{0}{1}", paramName, row));
        }
        //回傳以 , 為分隔符號的字串
        return string.Join(",", aryParam.ToArray());
    }

    /// <summary>
    /// 更新圖片集壓縮檔
    /// </summary>
    /// <param name="ModelNo">品號</param>
    /// <param name="ClassID">類別代號</param>
    private bool Update_ZipFiles(string ModelNo, string ClassID)
    {
        //來源資料夾(產品圖CID = 1)
        string sourceFolder = @"{0}{1}\{2}\{3}".FormatThis(Application["File_DiskUrl"], "ProductPic", ModelNo, ClassID);
        //目標資料夾
        string targetFolder = @"{0}{1}\".FormatThis(Application["File_DiskUrl"], "ProductPic_Zip");
        //目標檔案
        string targetFileName = "{0}_gallery{1}.zip".FormatThis(ModelNo, ClassID);

        return fn_CustomIO.exec_ZipFiles(sourceFolder, targetFolder, targetFileName);
    }
    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 上一頁網址
    /// </summary>
    private string _LastUrl;
    public string LastUrl
    {
        get
        {
            return Request.QueryString["rt"].ToString();
        }
        set
        {
            this._LastUrl = value;
        }
    }


    /// <summary>
    /// [參數] - 本頁路徑
    /// </summary>
    private string _PageUrl;
    public string PageUrl
    {
        get
        {
            return this._PageUrl != null ? this._PageUrl : "{0}ProdPic/myPic_Maintain.aspx?ModelNo={1}&Cls={2}&Col={3}&rt={4}"
                .FormatThis(
                    Application["WebUrl"]
                    , Server.UrlEncode(Param_ModelNo)
                    , Server.UrlEncode(Param_Class)
                    , Server.UrlEncode(Param_Column)
                    , Server.UrlEncode(LastUrl));
        }
        set
        {
            this._PageUrl = value;
        }
    }

    /// <summary>
    /// [參數] - 品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get
        {
            return Request.QueryString["ModelNo"] == null ? "" : Request.QueryString["ModelNo"].Trim().ToUpper();
        }
        set
        {
            this._Param_ModelNo = value;
        }
    }

    /// <summary>
    /// [參數] - 圖片類別
    /// </summary>
    private string _Param_Class;
    public string Param_Class
    {
        get
        {
            return Request.QueryString["Cls"] == null ? "" : Request.QueryString["Cls"].ToString();
        }
        set
        {
            this._Param_Class = value;
        }
    }

    /// <summary>
    /// [參數] - 圖片欄位 / 圖片ID
    /// </summary>
    private string _Param_Column;
    public string Param_Column
    {
        get
        {
            return Request.QueryString["Col"] == null ? "" : Request.QueryString["Col"].ToString();
        }
        set
        {
            this._Param_Column = value;
        }
    }

    #endregion


    #region -- 上傳參數 --


    /// <summary>
    /// 縮圖檔名
    /// </summary>
    private string _ThumbFrontFileName_500;
    public string ThumbFrontFileName_500
    {
        get
        {
            return "500x500_";
        }
        set
        {
            this._ThumbFrontFileName_500 = value;
        }
    }

    private string _ThumbFrontFileName_1000;
    public string ThumbFrontFileName_1000
    {
        get
        {
            return "1000x1000_";
        }
        set
        {
            this._ThumbFrontFileName_1000 = value;
        }
    }

    /// <summary>
    /// [參數] - 資料夾路徑
    /// </summary>
    private string _Param_FileFolder;
    public string Param_FileFolder
    {
        get
        {
            return this._Param_FileFolder != null ? this._Param_FileFolder : Application["File_DiskUrl"] + @"ProductPic\{0}\{1}\"
                .FormatThis(Server.HtmlEncode(Param_ModelNo), Param_Class);
        }
        set
        {
            this._Param_FileFolder = value;
        }
    }

    /// <summary>
    /// [參數] - 浮水印資料夾路徑
    /// </summary>
    private string _WaterImg_FileFolder;
    public string WaterImg_FileFolder
    {
        get
        {
            return Application["File_DiskUrl"] + @"WaterImg\";
        }
        set
        {
            this._WaterImg_FileFolder = value;
        }
    }


    /// <summary>
    /// 限制上傳的副檔名
    /// </summary>
    private string _FileExtLimit;
    public string FileExtLimit
    {
        get
        {
            return ".jpg||.png||.gif||.pdf";
        }
        set
        {
            this._FileExtLimit = value;
        }
    }

    /// <summary>
    /// 圖片設定寬度
    /// </summary>
    private int _Param_Width;
    public int Param_Width
    {
        get
        {
            return 1600;
        }
        set
        {
            this._Param_Width = value;
        }
    }
    /// <summary>
    /// 圖片設定高度
    /// </summary>
    private int _Param_Height;
    public int Param_Height
    {
        get
        {
            return 1600;
        }
        set
        {
            this._Param_Height = value;
        }
    }

    /// <summary>
    /// 圖片Size
    /// </summary>
    /// <param name="sizeType"></param>
    /// <returns></returns>
    private int[] GetImgSize(string sizeType)
    {
        switch (sizeType.ToLower())
        {
            case "500":
                return new int[] { 500, 500 };

            case "1000":
                return new int[] { 1000, 1000 };

            default:
                return new int[] { 1600, 1600 };
        }
    }


    #endregion

    /// <summary>
    /// 暫存參數
    /// </summary>
    public class TempParam
    {
        /// <summary>
        /// [參數] - 圖片檔名
        /// </summary>
        private string _Param_Pic;
        public string Param_Pic
        {
            get { return this._Param_Pic; }
            set { this._Param_Pic = value; }
        }

        /// <summary>
        /// [參數] - 圖片原始名稱
        /// </summary>
        private string _Param_OrgPic;
        public string Param_OrgPic
        {
            get { return this._Param_OrgPic; }
            set { this._Param_OrgPic = value; }
        }

        /// <summary>
        /// [參數] - 目前圖片檔名
        /// </summary>
        private string _Param_CurrentPic;
        public string Param_CurrentPic
        {
            get { return this._Param_CurrentPic; }
            set { this._Param_CurrentPic = value; }
        }

        /// <summary>
        /// [參數] - 圖片類別
        /// </summary>
        private string _Param_FileKind;
        public string Param_FileKind
        {
            get { return this._Param_FileKind; }
            set { this._Param_FileKind = value; }
        }



        private bool _IsResize;
        public bool IsResize
        {
            get { return this._IsResize; }
            set { this._IsResize = value; }
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
        /// <param name="Param_Pic">圖片檔名</param>
        /// <param name="Param_OrgPic">圖片原始名稱</param>
        /// <param name="Param_CurrentPic">目前圖片檔名</param>
        /// <param name="Param_hpf">上傳檔案</param>
        /// <param name="Param_FileKind">分類</param>
        /// <param name="IsResize">是否要重設大小</param>
        public TempParam(string Param_Pic, string Param_OrgPic, string Param_CurrentPic
                , HttpPostedFile Param_hpf, string Param_FileKind, bool IsResize)
        {
            this._Param_Pic = Param_Pic;
            this._Param_OrgPic = Param_OrgPic;
            this._Param_CurrentPic = Param_CurrentPic;
            this._Param_hpf = Param_hpf;
            this._Param_FileKind = Param_FileKind;
            this._IsResize = IsResize;
        }
    }


}