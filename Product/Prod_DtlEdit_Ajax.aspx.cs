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

public partial class Product_Prod_DtlEdit_Ajax : SecurityIn
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
                if (string.IsNullOrEmpty(Request["dataVal"]) || string.IsNullOrEmpty(Request["ModelNo"])
                    || string.IsNullOrEmpty(Request["SpecClass"]))
                {
                    Response.Write("error");
                    return;
                }
                //[取得參數值]
                string Param_dataVal = Request["dataVal"].ToString();
                string Param_ModelNo = Request["ModelNo"].ToString();
                string Param_SpecClass = Request["SpecClass"].ToString();
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

                    int dataIdx = 0;
                    for (int row = 0; row < sData.Count; row++)
                    {
                        //判斷是否為Update
                        if (string.IsNullOrEmpty(sData[row].DataID))
                        {
                            if (sData[row].Kind.ToUpper().Equals("SINGLESELECT")
                                || sData[row].Kind.ToUpper().Equals("MULTISELECT"))
                            {
                                //[刪除] - 單選/複選的所有項目
                                SBSql.AppendLine(" DELETE FROM Prod_Spec_List ");
                                SBSql.AppendLine(" WHERE (SpecClassID = @SpecClassID) AND (Model_No = @Model_No) ");
                                SBSql.AppendLine(string.Format(
                                    " AND (SpecID = '{0}') AND (CateID = {1}) "
                                    , sData[row].SpecID
                                    , Convert.ToInt32(sData[row].CateID)
                                    ));
                            }

                            //處理多值
                            string[] aryData = Regex.Split(sData[row].Val, @"\|{4}");

                            //Insert
                            for (int col = 0; col < aryData.Length; col++)
                            {
                                dataIdx += 1;
                                //開始新增
                                SBSql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Spec_ListID), 0) + 1 FROM Prod_Spec_List WHERE (Model_No = @Model_No)) ");
                                SBSql.AppendLine(" INSERT INTO Prod_Spec_List (");
                                SBSql.AppendLine("  Spec_ListID, Model_No, SpecClassID, SpecID, ListSymbol, ListValue, CateID ");
                                SBSql.AppendLine(" ) VALUES ( ");
                                SBSql.AppendLine(string.Format(
                                    "@New_ID, @Model_No, @SpecClassID, '{0}', '{1}', N'{2}', {3}"
                                    , sData[row].SpecID
                                    , sData[row].Symbol
                                    , aryData[col].ToString()
                                    , Convert.ToInt32(sData[row].CateID)
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
                                SBSql.AppendLine(" UPDATE Prod_Spec_List SET ");
                                SBSql.AppendLine(string.Format(" ListSymbol = '{0}', ListValue = N'{1}'"
                                    , sData[row].Symbol
                                    , aryData[col].ToString()));
                                SBSql.AppendLine(string.Format(
                                    " WHERE (Spec_ListID = {0}) AND (Model_No = @Model_No) AND (CateID = {1}); "
                                    , aryID[col]
                                    , Convert.ToInt32(sData[row].CateID)
                                    ));
                            }
                        }

                    }
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("Model_No", Param_ModelNo);
                    cmd.Parameters.AddWithValue("SpecClassID", Param_SpecClass);
                    if (false == dbConClass.ExecuteSql(cmd, out ErrMsg))
                    {
                        Response.Write("error:" + ErrMsg);
                        return;
                    }
                    else
                    {
                        //寫入Log
                        fn_Log.Log_Rec("產品規格"
                            , Param_ModelNo
                            , "修改產品明細,品號:{0}, 規格類別代號:{1}".FormatThis(Param_ModelNo, Param_SpecClass)
                            , fn_Param.CurrentAccount.ToString());

                        //Response
                        Response.Write("OK");
                    }
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
    /// 取得欄位參數
    /// </summary>
    public class SpecData
    {
        public string inputID { get; set; }
        public string SpecID { get; set; }
        public string Kind { get; set; }
        public string OptionGid { get; set; }
        public string DataID { get; set; }
        public string Val { get; set; }
        public string Symbol { get; set; }
        public string CateID { get; set; }
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
}