using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using ExtensionMethods;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// 權限處理
/// </summary>
/// <remarks>
/// 判斷個人權限是否存在
///   -否:前往群組權限
///   -是:
///     1.判斷帳戶是否停用
///     2.判斷權限編號是否正常設定
/// </remarks>
public class fn_CheckAuth
{
    #region -- 權限檢查 --
    /// <summary>
    /// 權限檢查
    /// </summary>
    /// <param name="authProgID">欲判斷的權限編號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    /// <remarks>
    /// 先判斷是否有個人權限, 若沒有才檢查群組權限
    /// </remarks>
    public static bool CheckAuth_User(string authProgID, out string ErrMsg)
    {
        try
        {
            //取得個人Guid
            string tmpGuid = fn_Param.CurrentUser.ToString();
            if (string.IsNullOrEmpty(tmpGuid))
            {
                ErrMsg = "無法取得個人參數，請聯絡系統管理員!";
                return false;
            }
            //取得個人帳號
            string tmpAccount = fn_Param.CurrentAccount.ToString();
            if (string.IsNullOrEmpty(tmpAccount))
            {
                ErrMsg = "無法取得個人參數，請聯絡系統管理員!";
                return false;
            }

            //判斷是否有個人權限
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Guid, Prog_ID ");
                sbSQL.AppendLine(" FROM User_Profile_Rel_Program WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Prog_ID = @Prog_ID) AND (Guid = @Guid) ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.AddWithValue("Prog_ID", authProgID);
                cmd.Parameters.AddWithValue("Guid", tmpGuid);

                //取得資料
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        
                        return false;
                    }
                    else
                    {
                        ErrMsg = "";
                        return true;

                    }
                }
            }

        }
        catch (Exception)
        {
            ErrMsg = "權限判斷發生錯誤，請聯絡系統管理員!";
            return false;
        }
    }

    #endregion

}