using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using LogRecord;
using ExtensionMethods;

public partial class Prod_BOM_DtlEdit_Ajax : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[驗證] - MD5是否相同
                if (Request.Form["ValidCode"] == null)
                {
                    Response.Write("驗證失敗!");
                    return;
                }
                if (!Request.Form["ValidCode"].Equals(ValidCode))
                {
                    Response.Write("驗證失敗!");
                    return;
                }

                //[取得callback資料]
                if (string.IsNullOrEmpty(Request["dataVal"]) || string.IsNullOrEmpty(Request["ModelNo"]))
                {
                    Response.Write("error");
                    return;
                }
                //[取得參數值]
                string Param_Type = Request.Form["Type"].ToString();

                switch (Param_Type.ToUpper())
                {
                    case "INSERT":
                        AddData();
                        break;

                    case "UPDATE":
                        EditData();
                        break;

                    default:
                        Response.Write("error:錯誤的操作!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Response.Write("error:" + ex.Message.ToString());
                return;
            }
        }
    }

    /// <summary>
    /// 新增資料
    /// </summary>
    private void AddData()
    {
        string ErrMsg;

        //[SQL] - 資料新增/更新
        using (SqlCommand cmd = new SqlCommand())
        {
            //[清除參數]
            cmd.Parameters.Clear();

            //宣告
            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" Declare @New_ID AS INT, @Row_ID AS INT ");
            SBSql.AppendLine(" SET @Row_ID = (SELECT ISNULL(MAX(RowID), 0) + 1 FROM Prod_BOMSpec_List WHERE ( ");
            SBSql.AppendLine("     (Model_No = @Model_No) ");
            SBSql.AppendLine("     AND (CateID = @CateID) ");
            SBSql.AppendLine("     AND (SpecClassID = @SpecClassID) ");
            SBSql.AppendLine("     AND (SpecID = @SpecID) ");
            SBSql.AppendLine("     AND (BOM_SpecID = @BOM_SpecID) ");
            SBSql.AppendLine(" )) ");

            //反序列化
            List<SpecData> sData = JsonConvert.DeserializeObject<List<SpecData>>(Param_dataVal);

            for (int row = 0; row < sData.Count; row++)
            {
                //處理多值
                string[] aryData = Regex.Split(sData[row].Val, @"\|{4}");

                //Insert
                for (int col = 0; col < aryData.Length; col++)
                {
                    //開始新增
                    SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Spec_ListID), 0) + 1 FROM Prod_BOMSpec_List WHERE (Model_No = @Model_No)) ");

                    SBSql.AppendLine(" INSERT INTO Prod_BOMSpec_List (");
                    SBSql.AppendLine("  Spec_ListID, Model_No, CateID, SpecClassID, SpecID, BOM_SpecID, RowID, ListSymbol, ListValue, Sort ");
                    SBSql.AppendLine(" ) VALUES ( ");
                    SBSql.AppendLine(string.Format(
                        "@New_ID, @Model_No, @CateID, @SpecClassID, @SpecID, @BOM_SpecID, @Row_ID, '{0}', N'{1}', @Row_ID*10"
                        , sData[row].Symbol
                        , aryData[col].ToString()
                        ));
                    SBSql.AppendLine(" ); ");
                }
            }
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
            cmd.Parameters.AddWithValue("CateID", Param_CateID);
            cmd.Parameters.AddWithValue("SpecClassID", Param_SpecClassID);
            cmd.Parameters.AddWithValue("SpecID", Param_SpecID);
            cmd.Parameters.AddWithValue("BOM_SpecID", Param_BOMSpecID);
            if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
            {
                Response.Write("error:" + ErrMsg);
                return;
            }
            else
            {
                //寫入Log
                fn_Log.Log_Rec("BOM產品規格"
                    , Param_ModelNo
                    , "新增BOM規格明細,品號:{0}, 規格分類:{1}, 規格類別:{2}, 規格編號:{3}, BOM規格編號:{4}"
                    .FormatThis(Param_ModelNo, Param_CateID, Param_SpecClassID, Param_SpecID, Param_BOMSpecID)
                    , fn_Param.CurrentAccount.ToString());

                //Response
                Response.Write("OK");
            }

        }
    }

    /// <summary>
    /// 更新資料
    /// </summary>
    private void EditData()
    {
        string ErrMsg;

        //[SQL] - 資料新增/更新
        using (SqlCommand cmd = new SqlCommand())
        {
            //[清除參數]
            cmd.Parameters.Clear();

            StringBuilder SBSql = new StringBuilder();
            SBSql.AppendLine(" Declare @New_ID AS INT ");

            //反序列化
            List<SpecData> sData = JsonConvert.DeserializeObject<List<SpecData>>(Param_dataVal);

            for (int row = 0; row < sData.Count; row++)
            {
                //判斷是否有DataID
                if (string.IsNullOrEmpty(sData[row].DataID))
                {
                    if (sData[row].Kind.ToUpper().Equals("SINGLESELECT")
                        || sData[row].Kind.ToUpper().Equals("MULTISELECT"))
                    {
                        //[刪除] - 單選/複選的項目
                        SBSql.AppendLine(" DELETE FROM Prod_BOMSpec_List ");
                        SBSql.AppendLine(" WHERE (Model_No = @Model_No) AND (CateID = @CateID) AND (SpecClassID = @SpecClassID) AND (SpecID = @SpecID)");
                        SBSql.AppendLine("  AND (BOM_SpecID = @BOM_SpecID) AND (RowID = @Row_ID)");
                    }

                    //處理多值
                    string[] aryData = Regex.Split(sData[row].Val, @"\|{4}");

                    //Insert
                    for (int col = 0; col < aryData.Length; col++)
                    {
                        //開始新增
                        SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Spec_ListID), 0) + 1 FROM Prod_BOMSpec_List WHERE (Model_No = @Model_No)) ");

                        SBSql.AppendLine(" INSERT INTO Prod_BOMSpec_List (");
                        SBSql.AppendLine("  Spec_ListID, Model_No, CateID, SpecClassID, SpecID, BOM_SpecID, RowID, ListSymbol, ListValue, Sort ");
                        SBSql.AppendLine(" ) VALUES ( ");
                        SBSql.AppendLine(string.Format(
                            "@New_ID, @Model_No, @CateID, @SpecClassID, @SpecID, @BOM_SpecID, @Row_ID, '{0}', N'{1}', {2}"
                            , sData[row].Symbol
                            , aryData[col].ToString()
                            , Convert.ToInt16(sData[row].Sort)
                            ));
                        SBSql.AppendLine(" ); ");
                    }
                }
                else
                {
                    //處理多值
                    string[] aryID = Regex.Split(sData[row].DataID, @"\|{4}");
                    string[] aryData = Regex.Split(sData[row].Val, @"\|{4}");

                    //Update
                    for (int col = 0; col < aryID.Length; col++)
                    {
                        SBSql.AppendLine(" UPDATE Prod_BOMSpec_List SET ");
                        SBSql.AppendLine(" ListSymbol = '{0}', ListValue = N'{1}', Sort = {2}"
                            .FormatThis(sData[row].Symbol
                            , aryData[col].ToString()
                            , Convert.ToInt16(sData[row].Sort)));
                        SBSql.AppendLine(" WHERE (Spec_ListID = {0}) AND (Model_No = @Model_No); "
                            .FormatThis(aryID[col]
                            ));
                    }
                }

            }
            cmd.CommandText = SBSql.ToString();
            cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
            cmd.Parameters.AddWithValue("CateID", Param_CateID);
            cmd.Parameters.AddWithValue("SpecClassID", Param_SpecClassID);
            cmd.Parameters.AddWithValue("SpecID", Param_SpecID);
            cmd.Parameters.AddWithValue("BOM_SpecID", Param_BOMSpecID);
            cmd.Parameters.AddWithValue("Row_ID", Param_RowID);
            if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
            {
                Response.Write("error:" + ErrMsg);
                return;
            }
            else
            {
                //寫入Log
                fn_Log.Log_Rec("BOM產品規格"
                      , Param_ModelNo
                      , "修改BOM規格明細,品號:{0}, 規格分類:{1}, 規格類別:{2}, 規格編號:{3}, BOM規格編號:{4}, RowID:{5}"
                      .FormatThis(Param_ModelNo, Param_CateID, Param_SpecClassID, Param_SpecID, Param_BOMSpecID, Param_RowID)
                      , fn_Param.CurrentAccount.ToString());

                //Response
                Response.Write("OK");
            }
        }
    }

    #region -- 參數設定 --

    /// <summary>
    /// 取得欄位參數
    /// </summary>
    public class SpecData
    {
        public string SpecID { get; set; }
        public string Kind { get; set; }
        public string OptionGid { get; set; }
        public string DataID { get; set; }
        public string Val { get; set; }
        public string Symbol { get; set; }
        public string Sort { get; set; }
    }

    /// <summary>
    /// 取得欄位值
    /// </summary>
    private string _Param_dataVal;
    public string Param_dataVal
    {
        get { return Request.Form["dataVal"].ToString(); }
        private set { this._Param_dataVal = value; }
    }

    /// <summary>
    /// 取得品號
    /// </summary>
    private string _Param_ModelNo;
    public string Param_ModelNo
    {
        get { return Request.Form["ModelNo"].ToString(); }
        private set { this._Param_ModelNo = value; }
    }

    /// <summary>
    /// 取得CateID
    /// </summary>
    private string _Param_CateID;
    public string Param_CateID
    {
        get { return Request.Form["CateID"].ToString(); }
        private set { this._Param_CateID = value; }
    }

    /// <summary>
    /// 取得SpecClassID
    /// </summary>
    private string _Param_SpecClassID;
    public string Param_SpecClassID
    {
        get { return Request.Form["SpecClassID"].ToString(); }
        private set { this._Param_SpecClassID = value; }
    }

    /// <summary>
    /// 取得SpecID
    /// </summary>
    private string _Param_SpecID;
    public string Param_SpecID
    {
        get { return Request.Form["SpecID"].ToString(); }
        private set { this._Param_SpecID = value; }
    }

    /// <summary>
    /// 取得BOMSpecID
    /// </summary>
    private string _Param_BOMSpecID;
    public string Param_BOMSpecID
    {
        get { return Request.Form["BOMSpecID"].ToString(); }
        private set { this._Param_BOMSpecID = value; }
    }

    /// <summary>
    /// 取得RowID
    /// </summary>
    private string _Param_RowID;
    public string Param_RowID
    {
        get { return Request.Form["RowID"].ToString(); }
        private set { this._Param_RowID = value; }
    }

    /// <summary>
    /// 產生MD5驗証碼
    /// SessionID + 登入帳號 + 自訂字串
    /// </summary>
    private string _ValidCode;
    public string ValidCode
    {
        get { return Cryptograph.MD5(Session.SessionID + fn_Param.CurrentAccount + System.Web.Configuration.WebConfigurationManager.AppSettings["ValidCode_Pwd"], 32); }
        private set { this._ValidCode = value; }
    }

    #endregion
}