using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using ExtensionMethods;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;

public partial class Prod_BOM_Dtl_WinBox : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //[初始化]
            string ErrMsg;

            //[權限判斷] - 品規編輯
            if (fn_CheckAuth.CheckAuth_User("121", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("無使用權限！", "script:parent.$.fancybox.close()");
                return;
            }

            //[取得/檢查參數] - 單/複選
            if (fn_Extensions.String_字數(Param_IsMulti, "1", "1", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }
            //[取得/檢查參數] - 選單單頭
            if (fn_Extensions.String_字數(Param_OptGID, "5", "5", out ErrMsg) == false)
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", "script:parent.$.fancybox.close()");
                return;
            }

            //[帶出資料]
            LookupDataList();
        }
    }

    #region -- 資料取得 --
    /// <summary>
    /// 副程式 - 取得資料列表
    /// </summary>
    private void LookupDataList()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string ErrMsg;
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Spec_OptionValue, Spec_OptionName_zh_TW, Spec_OptionPic, Spec_OptionFile ");
                SBSql.AppendLine(" FROM Prod_BOMSpec_Option ");
                SBSql.AppendLine(" WHERE (OptionGID = @OptionGID) AND (Display = 'Y') ");
                SBSql.AppendLine(" ORDER BY Sort, Spec_OptionValue ");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("OptionGID", Param_OptGID);
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //DataBind            
                    this.lvDataList.DataSource = DT.DefaultView;
                    this.lvDataList.DataBind();
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 取得資料列表！", "");
        }
    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //新增屬性 - Checkbox Value
            string OptValue = DataBinder.Eval(dataItem.DataItem, "Spec_OptionValue").ToString();
            string OptName = DataBinder.Eval(dataItem.DataItem, "Spec_OptionName_zh_TW").ToString();
            CheckBox cb_ID = (CheckBox)e.Item.FindControl("cb_ID");
            cb_ID.InputAttributes["value"] = OptValue + "｜" + OptName;

            //[判斷參數] - 顯示圖片
            string Pic = DataBinder.Eval(dataItem.DataItem, "Spec_OptionPic").ToString();
            string PicFile = DataBinder.Eval(dataItem.DataItem, "Spec_OptionFile").ToString();
            Literal lt_Pic = (Literal)e.Item.FindControl("lt_Pic");

            if (string.IsNullOrEmpty(Pic) || Pic.Equals("0"))
            {
                //沒有Icon, 顯示自行上傳的圖片
                lt_Pic.Text = PicUrl(PicFile, "File");
            }
            else
            {
                //顯示Icon
                lt_Pic.Text = PicUrl(Pic, "Icon");
            }
        }
    }

    #endregion

    #region -- 按鈕區 --
    protected void btn_Add_Click(object sender, EventArgs e)
    {
        try
        {
            //編號組合 (Array)
            ArrayList aryIds = new ArrayList();
            ArrayList aryNames = new ArrayList();
            for (int row = 0; row < lvDataList.Items.Count; row++)
            {
                //判斷選項是否有勾選
                CheckBox cb_ID = (CheckBox)lvDataList.Items[row].FindControl("cb_ID");
                if (cb_ID.Checked)
                {
                    //分解Checkbox Value
                    string[] aryVal = Regex.Split(cb_ID.InputAttributes["value"], @"\｜{1}");
                    aryIds.Add(aryVal[0]);
                    aryNames.Add(aryVal[1]);
                }
            }
            string Get_Desc;
            string Get_Css;
            if (aryIds.Count == 0)
            {
                Get_Desc = "請先選擇項目";
                Get_Css = "selectBox styleGreen";
            }
            else
            {
                Get_Desc = "已選取 " + aryIds.Count + " 個項目：" + string.Join(", ", aryNames.ToArray());
                Get_Css = "selectBox styleBlue";
            }

            //回傳至母頁
            string js = string.Format(
                "parent.$('#{0}').val('{2}');" +
                "parent.$('#{1}').text('{3}');" +
                "parent.$('#{1}').removeClass().addClass('{4}');" +
                "parent.$('#addBtn_{5}').trigger('click');" +
                "parent.$.fancybox.close();"
                , Param_Mode.ToUpper().Equals("INSERT") ? "{0}_newval".FormatThis(Param_SpecID) : "{0}_{1}_val".FormatThis(Param_SpecID, Param_RowID)
                , "url_{0}_{1}".FormatThis(Param_SpecID, Param_RowID)
                , string.Join("||||", aryIds.ToArray())
                , Get_Desc
                , Get_Css
                , Param_SpecID
             );

            ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", js, true);
            return;
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 加入項目！", "");
            return;
        }
    }


    #endregion

    #region -- 自訂功能 --
    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">檔名</param>
    /// <param name="PicType">顯示類型</param>
    /// <returns></returns>
    private string PicUrl(string PicName, string PicType)
    {
        if (string.IsNullOrEmpty(PicName))
        {
            return "";
        }

        switch (PicType.ToUpper())
        {
            case "ICON":
                return "<td class=\"L2Img\" style=\"width: 50px\" align=\"left\">{0}</td>".FormatThis(fn_Extensions.GetOptIcon(PicName));

            case "FILE":
                return "<td class=\"L2Img\" style=\"width: 50px\" align=\"left\"><img src=\"{0}\" width=\"50px\" /></td>".FormatThis(Param_WebFolder + PicName);

            default:

                return "";
        }
    }
    #endregion

    #region -- 參數設定 --
    /// <summary>
    /// 單選或複選
    /// </summary>
    private string _Param_IsMulti;
    public string Param_IsMulti
    {
        get
        {
            string IsMulti = Request.QueryString["IsMulti"] == null ? "" : Request.QueryString["IsMulti"].ToString().ToUpper();

            return this._Param_IsMulti != null ? this._Param_IsMulti : IsMulti;
        }
        private set
        {
            this._Param_IsMulti = value;
        }
    }

    /// <summary>
    /// 選單單頭編號
    /// </summary>
    private string _Param_OptGID;
    public string Param_OptGID
    {
        get
        {
            string OptGID = Request.QueryString["OptionGID"] == null ? "" : Request.QueryString["OptionGID"].ToString().ToUpper();

            return this._Param_OptGID != null ? this._Param_OptGID : OptGID;
        }
        private set
        {
            this._Param_OptGID = value;
        }
    }

    /// <summary>
    /// BOM規格編號
    /// </summary>
    private string _Param_SpecID;
    public string Param_SpecID
    {
        get
        {
            string SpecID = Request.QueryString["SpecID"] == null ? "" : Request.QueryString["SpecID"].ToString().ToUpper();

            return this._Param_SpecID != null ? this._Param_SpecID : SpecID;
        }
        private set
        {
            this._Param_SpecID = value;
        }
    }

    /// <summary>
    /// RowID
    /// </summary>
    private string _Param_RowID;
    public string Param_RowID
    {
        get
        {
            return Request.QueryString["RowID"] == null ? "" : Request.QueryString["RowID"].ToString();
        }
        private set
        {
            this._Param_RowID = value;
        }
    }

    /// <summary>
    /// Mode
    /// </summary>
    private string _Param_Mode;
    public string Param_Mode
    {
        get
        {
            return Request.QueryString["Mode"] == null ? "" : Request.QueryString["Mode"].ToString();
        }
        private set
        {
            this._Param_Mode = value;
        }
    }

    /// <summary>
    /// 母頁欄位名稱
    /// </summary>
    private string _Param_FiledName;
    public string Param_FiledName
    {
        get
        {
            return Param_Mode.ToUpper().Equals("INSERT") ? "{0}_newval".FormatThis(Param_SpecID) : "{0}_{1}_val".FormatThis(Param_SpecID, Param_RowID);
        }
        private set
        {
            this._Param_FiledName = value;
        }
    }

    /// <summary>
    /// 取得圖片連結
    /// </summary>
    /// <param name="PicName">檔名</param>
    /// <returns></returns>
    private string PicUrl(string PicName)
    {
        return string.Format(
            "<td class=\"L2Img\" style=\"width: 50px\" align=\"left\">{0}</td>"
            , fn_Extensions.GetOptIcon(PicName));
    }

    /// <summary>
    /// 參數 - Web資料夾路徑
    /// </summary>
    private string _Param_WebFolder;
    public string Param_WebFolder
    {
        get
        {
            return Application["File_WebUrl"] + @"ProductSpec/";
        }
        set
        {
            this._Param_WebFolder = value;
        }
    }
    #endregion
}