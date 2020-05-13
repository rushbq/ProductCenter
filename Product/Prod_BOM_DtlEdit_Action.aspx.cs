using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using LogRecord;
using ExtensionMethods;

public partial class Prod_BOM_DtlEdit_Action : SecurityIn
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
                    Response.Write("設定失敗, 驗証碼有誤!");
                    return;
                }
                if (!Request.Form["ValidCode"].Equals(ValidCode))
                {
                    Response.Write("設定失敗, 驗証碼有誤!");
                    return;
                }

                string ErrMsg;
                //[檢查&取得參數] - 來源類型
                if (Request.Form["Type"] == null)
                {
                    Response.Write("來源參數錯誤!");
                    return;
                }
                string type = fn_stringFormat.Filter_Html(Request.Form["Type"].ToString());
                string ModelNo = Request.Form["ModelNo"].ToString();
                string CateID = Request.Form["CateID"].ToString();
                string SpecClassID = Request.Form["SpecClassID"].ToString();
                string SpecID = Request.Form["SpecID"].ToString();
                string BOMSpecID = Request.Form["BOMSpecID"].ToString();
                string RowID = Request.Form["RowID"].ToString();

                //判斷來源類型
                switch (type.ToLower())
                {
                    case "remove":
                        if (false == RemoveItems(ModelNo, CateID, SpecClassID, SpecID, BOMSpecID, RowID, out ErrMsg))
                        {
                            Response.Write(ErrMsg);
                        }
                        else
                        {
                            //寫入Log
                            fn_Log.Log_Rec("BOM產品規格"
                                , ModelNo
                                , "移除BOM規格明細,品號:{0}, 規格分類:{1}, 規格類別:{2}, 規格編號:{3}, BOM規格編號:{4}, RowID:{5}"
                                .FormatThis(ModelNo, CateID, SpecClassID, SpecID, BOMSpecID, RowID)
                                , fn_Param.CurrentAccount.ToString());

                            //回傳OK, Ajax判斷成功
                            Response.Write("OK");
                        }
                        break;

                    case "removeall":
                        if (false == RemoveItems(ModelNo, CateID, SpecClassID, SpecID, out ErrMsg))
                        {
                            Response.Write(ErrMsg);
                        }
                        else
                        {
                            //寫入Log
                            fn_Log.Log_Rec("BOM產品規格"
                                , ModelNo
                                , "移除全部BOM規格明細,品號:{0}, 規格分類:{1}, 規格類別:{2}, 規格編號:{3}"
                                .FormatThis(ModelNo, CateID, SpecClassID, SpecID)
                                , fn_Param.CurrentAccount.ToString());

                            //回傳OK, Ajax判斷成功
                            Response.Write("OK");
                        }
                        break;

                    default:
                        Response.Write("無代誌...");
                        break;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
                return;
            }

        }
    }

    /// <summary>
    /// 移除規格明細值(所有)
    /// </summary>
    /// <param name="ModelNo">品號</param>
    /// <param name="CateID">規格分類</param>
    /// <param name="SpecClassID">規格類別</param>
    /// <param name="SpecID">規格編號</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool RemoveItems(string ModelNo, string CateID, string SpecClassID, string SpecID
        , out string ErrMsg)
    {
        try
        {
            if (string.IsNullOrEmpty(ModelNo) || string.IsNullOrEmpty(CateID) || string.IsNullOrEmpty(SpecClassID) || string.IsNullOrEmpty(SpecID))
            {
                ErrMsg = "參數傳遞錯誤!";
                return false;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Prod_BOMSpec_List ");
                SBSql.AppendLine(" WHERE (Model_No = @Model_No) AND (CateID = @CateID) AND (SpecClassID = @SpecClassID) AND (SpecID = @SpecID)");
                cmd.Parameters.AddWithValue("Model_No", ModelNo.Trim());
                cmd.Parameters.AddWithValue("CateID", CateID.Trim());
                cmd.Parameters.AddWithValue("SpecClassID", SpecClassID.Trim());
                cmd.Parameters.AddWithValue("SpecID", SpecID.Trim());
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    ErrMsg = "設定失敗, 請重新設定," + ErrMsg;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 移除規格明細值(單一項目)
    /// </summary>
    /// <param name="ModelNo">品號</param>
    /// <param name="CateID">規格分類</param>
    /// <param name="SpecClassID">規格類別</param>
    /// <param name="SpecID">規格編號</param>
    /// <param name="BOMSpecID">BOM規格編號</param>
    /// <param name="RowID">RowID</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool RemoveItems(string ModelNo, string CateID, string SpecClassID, string SpecID, string BOMSpecID, string RowID
        , out string ErrMsg)
    {
        try
        {
            if (string.IsNullOrEmpty(ModelNo) || string.IsNullOrEmpty(CateID) || string.IsNullOrEmpty(SpecClassID) || string.IsNullOrEmpty(SpecID)
                || string.IsNullOrEmpty(BOMSpecID) || string.IsNullOrEmpty(RowID))
            {
                ErrMsg = "參數傳遞錯誤!";
                return false;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Prod_BOMSpec_List ");
                SBSql.AppendLine(" WHERE (Model_No = @Model_No) AND (CateID = @CateID) AND (SpecClassID = @SpecClassID) AND (SpecID = @SpecID)");
                SBSql.AppendLine("  AND (BOM_SpecID = @BOM_SpecID) AND (RowID = @RowID)");
                cmd.Parameters.AddWithValue("Model_No", ModelNo.Trim());
                cmd.Parameters.AddWithValue("CateID", CateID.Trim());
                cmd.Parameters.AddWithValue("SpecClassID", SpecClassID.Trim());
                cmd.Parameters.AddWithValue("SpecID", SpecID.Trim());
                cmd.Parameters.AddWithValue("BOM_SpecID", BOMSpecID.Trim());
                cmd.Parameters.AddWithValue("RowID", RowID.Trim());
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    ErrMsg = "設定失敗, 請重新設定," + ErrMsg;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
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