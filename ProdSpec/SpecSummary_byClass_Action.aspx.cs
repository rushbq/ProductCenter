using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;

public partial class ProdSpec_SpecSummary_byClass_Action : SecurityIn
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
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
            string ClassID = Request.Form["ClassID"].ToString();
            string SpecID = Request.Form["SpecID"].ToString();

            //判斷來源類型
            switch (type.ToLower())
            {
                case "addrel":
                    if (false == AddRel(ClassID, SpecID, out ErrMsg))
                    {
                        Response.Write(ErrMsg);
                    }
                    else
                    {
                        //回傳OK, Ajax判斷成功
                        Response.Write("OK");
                    }
                    break;

                case "removerel":
                    if (false == RemoveRel(ClassID, SpecID, out ErrMsg))
                    {
                        Response.Write(ErrMsg);
                    }
                    else
                    {
                        //回傳OK, Ajax判斷成功
                        Response.Write("OK");
                    }
                    break;

                default:
                    Response.Write("無代誌...");
                    break;
            }
        }
    }

    /// <summary>
    /// 新增分類關聯
    /// </summary>
    /// <param name="ClassID">分類編號</param>
    /// <param name="SpecID">規格編號</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool AddRel(string ClassID, string SpecID, out string ErrMsg)
    {
        try
        {
            if (string.IsNullOrEmpty(ClassID) || string.IsNullOrEmpty(SpecID))
            {
                ErrMsg = "參數傳遞錯誤!";
                return false;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" IF (SELECT COUNT(*) ");
                SBSql.AppendLine("  FROM Prod_SpecClass_Rel_Spec");
                SBSql.AppendLine("  WHERE (SpecClassID = @ClassID) AND (SpecID = @SpecID) ");
                SBSql.AppendLine(" ) = 0 ");
                SBSql.AppendLine(" BEGIN ");
                SBSql.AppendLine("  INSERT INTO Prod_SpecClass_Rel_Spec (SpecClassID, SpecID) ");
                SBSql.AppendLine("  VALUES (@ClassID, @SpecID) ");
                SBSql.AppendLine(" END ");
                cmd.Parameters.AddWithValue("ClassID", ClassID.Trim());
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
    /// 移除分類關聯
    /// </summary>
    /// <param name="ClassID">分類編號</param>
    /// <param name="SpecID">規格編號</param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    private bool RemoveRel(string ClassID, string SpecID, out string ErrMsg)
    {
        try
        {
            if (string.IsNullOrEmpty(ClassID) || string.IsNullOrEmpty(SpecID))
            {
                ErrMsg = "參數傳遞錯誤!";
                return false;
            }
          
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                StringBuilder SBSql = new StringBuilder();
                SBSql.AppendLine(" DELETE FROM Prod_SpecClass_Rel_Spec ");
                SBSql.AppendLine(" WHERE (SpecClassID = @ClassID) AND (SpecID = @SpecID) ");
                cmd.Parameters.AddWithValue("ClassID", ClassID.Trim());
                cmd.Parameters.AddWithValue("SpecID", SpecID.Trim());
                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                if (dbConClass.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    ErrMsg = "移除失敗, 請重新設定," + ErrMsg;
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