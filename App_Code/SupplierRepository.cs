using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using PKLib_Method.Methods;
using SupplierData.Models;

namespace SupplierData.Controllers
{
    public class SupplierRepository
    {
        public string ErrMsg;


        #region -----// Read //-----

        /// <summary>
        /// 取得供應商採購記錄
        /// </summary>
        /// <param name="modelNo">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Supplier> GetDataList(string modelNo)
        {
            //----- 宣告 -----
            List<Supplier> dataList = new List<Supplier>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT * FROM (");
                sql.AppendLine("    --TW");
                sql.AppendLine("    SELECT DISTINCT");
                sql.AppendLine("      SUBSTRING(TC003,1,4) AS SetYear");
                sql.AppendLine("      , TC004 AS SupID, MA002 AS SupName");
                sql.AppendLine("      , 'TW' AS Company");
                sql.AppendLine("      , tbPrice.Currency, ROUND(ISNULL(tbPrice.Price, 0), 0) LastPrice, tbPrice.ValidDate");
                sql.AppendLine("    FROM [prokit2].[dbo].[PURTD] WITH (NOLOCK)");
                sql.AppendLine("        LEFT JOIN [prokit2].[dbo].[PURTC] WITH (NOLOCK) ON TC001 = TD001 AND TC002 = TD002");
                sql.AppendLine("        LEFT JOIN [prokit2].[dbo].[PURMA] WITH (NOLOCK) ON TC004 = MA001");
                sql.AppendLine("        --/* 排序, 取得第一筆 Start */");
                sql.AppendLine("        LEFT JOIN (");
                sql.AppendLine("            SELECT TOP 100 PERCENT");
                sql.AppendLine("                myPURMB.MB002, MB003 AS Currency, MB011 AS Price, MB014 AS ValidDate");
                sql.AppendLine("                , RANK() OVER (");
                sql.AppendLine("                     PARTITION BY myPURMB.MB002");
                sql.AppendLine("                     ORDER BY myPURMB.MB014 DESC");
                sql.AppendLine("                  ) AS myTbSeq");
                sql.AppendLine("            FROM [prokit2].[dbo].[PURMB] AS myPURMB WITH (NOLOCK)");
                sql.AppendLine("            WHERE (UPPER(myPURMB.MB001) = UPPER(@ModelNo))");
                sql.AppendLine("            ORDER BY myPURMB.MB014 DESC");
                sql.AppendLine("        ) AS tbPrice");
                sql.AppendLine("        ON PURTC.TC004 = tbPrice.MB002 AND tbPrice.myTbSeq = 1");
                sql.AppendLine("        --/* 排序, 取得第一筆 End */");

                sql.AppendLine("    -- Filter:TC014單頭確認 /TD016單身結案碼 自動結案");
                sql.AppendLine("    WHERE (TC014 = 'Y') AND (TD016 = 'Y') ");
                sql.AppendLine("        AND (UPPER(TD004) = UPPER(@ModelNo))");

                sql.AppendLine("    UNION ALL");

                sql.AppendLine("    --SH");
                sql.AppendLine("    SELECT DISTINCT");
                sql.AppendLine("      SUBSTRING(TC003,1,4) AS SetYear");
                sql.AppendLine("      , TC004 AS SupID, MA002 AS SupName");
                sql.AppendLine("      , 'SH' AS Company");
                sql.AppendLine("      , tbPrice.Currency, ROUND(ISNULL(tbPrice.Price, 0), 2) LastPrice, tbPrice.ValidDate");
                sql.AppendLine("    FROM [SHPK2].[dbo].[PURTD] WITH (NOLOCK)");
                sql.AppendLine("        LEFT JOIN [SHPK2].[dbo].[PURTC] WITH (NOLOCK) ON TC001 = TD001 AND TC002 = TD002");
                sql.AppendLine("        LEFT JOIN [SHPK2].[dbo].[PURMA] WITH (NOLOCK) ON TC004 = MA001");

                sql.AppendLine("        --/* 排序, 取得第一筆 Start */");
                sql.AppendLine("        LEFT JOIN (");
                sql.AppendLine("            SELECT TOP 100 PERCENT");
                sql.AppendLine("                myPURMB.MB002, MB003 AS Currency, MB011 AS Price, MB014 AS ValidDate");
                sql.AppendLine("                , RANK() OVER (");
                sql.AppendLine("                     PARTITION BY myPURMB.MB002");
                sql.AppendLine("                     ORDER BY myPURMB.MB014 DESC");
                sql.AppendLine("                  ) AS myTbSeq");
                sql.AppendLine("            FROM [SHPK2].[dbo].[PURMB] AS myPURMB WITH (NOLOCK)");
                sql.AppendLine("            WHERE (UPPER(myPURMB.MB001) = UPPER(@ModelNo))");
                sql.AppendLine("            ORDER BY myPURMB.MB014 DESC");
                sql.AppendLine("        ) AS tbPrice");
                sql.AppendLine("        ON PURTC.TC004 = tbPrice.MB002 AND tbPrice.myTbSeq = 1");
                sql.AppendLine("        --/* 排序, 取得第一筆 End */");

                sql.AppendLine("    -- Filter:TC014單頭確認 /TD016單身結案碼 自動結案");
                sql.AppendLine("    WHERE (TC014 = 'Y') AND (TD016 = 'Y') ");
                sql.AppendLine("        AND (UPPER(TD004) = UPPER(@ModelNo))");

                sql.AppendLine(") AS Tbl");
                sql.AppendLine("ORDER BY Tbl.SetYear DESC, Tbl.SupID");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Supplier
                        {
                            SetYear = item.Field<string>("SetYear"),
                            SupID = item.Field<string>("SupID"),
                            SupName = item.Field<string>("SupName"),
                            Company = item.Field<string>("Company"),
                            Currency = item.Field<string>("Currency"),
                            LastPrice = item.Field<Decimal>("LastPrice"),
                            ValidDate = item.Field<string>("ValidDate")
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }

        }


        /// <summary>
        /// 取得廠商交易記錄
        /// </summary>
        /// <returns></returns>
        public IQueryable<Supplier_TradeLog> GetTradeLog(string modelNo, string supID, string company)
        {
            //----- 宣告 -----
            List<Supplier_TradeLog> dataList = new List<Supplier_TradeLog>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT [MB001] AS ModelNo");
                sql.AppendLine("      ,[MB003] AS Currency");
                sql.AppendLine("      ,[MB004] AS Unit");
                sql.AppendLine("      ,[MB005] AS FirstTradeDate");
                sql.AppendLine("      ,[MB007] AS SupModelNo");
                sql.AppendLine("      ,[MB008] AS CheckDate");
                sql.AppendLine("      ,[MB009] AS LastInvDate");
                sql.AppendLine("      ,[MB010] AS IsSpQty");
                sql.AppendLine("      ,[MB011] AS BuyPrice");
                sql.AppendLine("      ,[MB012] AS Remark");
                sql.AppendLine("      ,[MB013] AS IsTax");
                sql.AppendLine("      ,[MB014] AS ValidDate");
                sql.AppendLine("      ,[MB015] AS InValidDate");
                sql.AppendLine("  FROM {0}.[dbo].[PURMB] WITH(NOLOCK)".FormatThis(GetDBName(company)));
                sql.AppendLine("WHERE (UPPER(MB001) = UPPER(@ModelNo)) AND (MB002 = @SupID)");
                sql.AppendLine("ORDER BY [MB014] DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);
                cmd.Parameters.AddWithValue("SupID", supID);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Supplier_TradeLog
                        {
                            ModelNo = item.Field<string>("ModelNo"),
                            Currency = item.Field<string>("Currency"),
                            Unit = item.Field<string>("Unit"),
                            FirstTradeDate = item.Field<string>("FirstTradeDate"),
                            SupModelNo = item.Field<string>("SupModelNo"),
                            CheckDate = item.Field<string>("CheckDate"),
                            LastInvDate = item.Field<string>("LastInvDate"),
                            IsSpQty = item.Field<string>("IsSpQty"),
                            BuyPrice = item.Field<Decimal>("BuyPrice"),
                            Remark = item.Field<string>("Remark"),
                            IsTax = item.Field<string>("IsTax"),
                            ValidDate = item.Field<string>("ValidDate"),
                            InValidDate = item.Field<string>("InValidDate")
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        /// <summary>
        /// 取得廠商分量計價
        /// </summary>
        /// <returns></returns>
        public IQueryable<Supplier_SpQtyInfo> GetSpQtyInfo(string modelNo, string supID, string company, string validDate)
        {
            //----- 宣告 -----
            List<Supplier_SpQtyInfo> dataList = new List<Supplier_SpQtyInfo>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT MC003 AS Currency, MC005 AS spQty, ISNULL(MC006, 0) AS spPrice");
                sql.AppendLine(" FROM {0}.[dbo].[PURMC] WITH(NOLOCK)".FormatThis(GetDBName(company)));
                sql.AppendLine(" WHERE (UPPER(MC001) = UPPER(@ModelNo)) AND (MC002 = @SupID) AND (MC008 = @ValidDate)");
                sql.AppendLine(" ORDER BY MC005");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);
                cmd.Parameters.AddWithValue("SupID", supID);
                cmd.Parameters.AddWithValue("ValidDate", validDate);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Supplier_SpQtyInfo
                        {
                            Currency = item.Field<string>("Currency"),
                            spQty = Convert.ToInt16(item.Field<Decimal>("spQty")),
                            spPrice = item.Field<Decimal>("spPrice")
                        };

                        //將項目加入至集合
                        dataList.Add(data);
                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        /// <summary>
        /// 回傳對應的DB NAME
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        private string GetDBName(string company)
        {
            switch (company.ToUpper())
            {
                case "TW":
                    return "prokit2";

                case "SH":
                    return "SHPK2";

                default:
                    return "prokit2";

            }
        }


        #endregion



    }
}