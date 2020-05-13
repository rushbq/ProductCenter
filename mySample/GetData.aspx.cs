using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;
using ProdSampleData.Controllers;

public partial class Google_GetData : System.Web.UI.Page
{
    /// <summary>
    /// 案件數(依類別)
    /// </summary>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetCountbyClass(string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();
            
            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT COUNT(Base.SeqNo) AS GroupCnt, Cls.Class_ID AS GroupID, Cls.Class_Name AS GroupName");
            sbSQL.Append(" FROM Sample_List Base WITH(NOLOCK)");
            sbSQL.Append("  INNER JOIN Sample_Class Cls WITH(NOLOCK) ON Base.Cls_Check = Cls.Class_ID");
            sbSQL.Append(" WHERE (Cls.Display = 'Y')");
          
            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }

            sbSQL.Append(" GROUP BY Cls.Class_ID, Cls.Class_Name");
            sbSQL.Append(" ORDER BY GroupCnt");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                int val = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組名稱
                    cat = dr[2].ToString();
                    //群組數值
                    val = Convert.ToInt32(dr[0]);
                    dataList.Add(new Data(cat, val));
                }
                return dataList;
            }
        }

    }


    /// <summary>
    /// 案件數(依公司別)
    /// </summary>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetCountbyCompany(string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT COUNT(Base.SeqNo) AS GroupCnt, Base.Company AS GroupID");
            sbSQL.Append(" , (CASE Base.Company WHEN 'TWS' THEN '台灣' ELSE '上海' END) AS GroupName");
            sbSQL.Append(" FROM Sample_List Base WITH(NOLOCK)");
            sbSQL.Append(" WHERE (1=1)");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }

            sbSQL.Append(" GROUP BY Base.Company");
            sbSQL.Append(" ORDER BY GroupCnt");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                int val = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組名稱
                    cat = dr[2].ToString();
                    //群組數值
                    val = Convert.ToInt32(dr[0]);
                    dataList.Add(new Data(cat, val));
                }
                return dataList;
            }
        }

    }


    /// <summary>
    /// --案件數統計(依負責人)
    /// </summary>
    /// <param name="StartDate">查詢日期-開始日</param>
    /// <param name="EndDate">查詢日期-結束日</param>
    /// <returns></returns>
    [WebMethod]
    public static List<Data> GetCountByWho(string StartDate, string EndDate)
    {
        string ErrMsg;

        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - SQL Statement
            sbSQL.Append(" SELECT");
            sbSQL.Append("  COUNT(Base.SeqNo) AS GroupCnt, Base.Assign_Who AS GroupID, Prof.Display_Name AS GroupName");
            sbSQL.Append(" FROM Sample_List Base WITH(NOLOCK)");
            sbSQL.Append("  INNER JOIN PKSYS.dbo.User_Profile Prof WITH(NOLOCK) ON Base.Assign_Who = Prof.Account_Name");
            sbSQL.Append(" WHERE (1=1)");

            //[查詢條件] - 開始日期
            if (false == string.IsNullOrEmpty(StartDate))
            {
                sbSQL.Append(" AND (Base.Create_Time >= @StartDate) ");
                cmd.Parameters.AddWithValue("StartDate", string.Format("{0} 00:00:00", StartDate));
            }
            //[查詢條件] - 結束日期
            if (false == string.IsNullOrEmpty(EndDate))
            {
                sbSQL.Append(" AND (Base.Create_Time <= @EndDate) ");
                cmd.Parameters.AddWithValue("EndDate", string.Format("{0} 23:59:59", EndDate));
            }

            sbSQL.Append(" GROUP BY Base.Assign_Who, Prof.Display_Name");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();


            //取得資料
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                List<Data> dataList = new List<Data>();
                string cat = "";
                int val = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    //群組名稱
                    cat = dr[2].ToString();
                    //群組數值
                    val = Convert.ToInt32(dr[0]);
                    dataList.Add(new Data(cat, val));

                }
                return dataList;
            }
        }

    }

   
}

public class Data
{
    public string ColumnName = "";
    public double Value = 0;
    public double Value1 = 0;

    public Data(string columnName, double value)
    {
        ColumnName = columnName;
        Value = value;
    }

    public Data(string columnName, double value, double value1)
    {
        ColumnName = columnName;
        Value = value;
        Value1 = value1;
    }
}
