using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Data.Assets;
using PKLib_Data.Controllers;
using PKLib_Method.Methods;
using ProdCheckData.Models;

/*
   外驗查檢表
 */
namespace ProdCheckData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        Status = 3,
        Vendor = 4,
        FirstID = 5,
        SecondID = 6,
        StartDate = 7,
        EndDate = 8,
        IsLock = 9,
        CorpID = 10,
        UpdateWho = 11
    }


    public class ProdCheckRepository
    {
        public string ErrMsg;

        #region -----// Read //-----

        /// <summary>
        /// 資料列表 - 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ProdCheck> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ProdCheck> dataList = new List<ProdCheck>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(search))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new ProdCheck
                    {
                        SeqNo = item.Field<int>("SeqNo"),
                        Data_ID = item.Field<Guid>("Data_ID"),
                        Corp_UID = item.Field<int>("Corp_UID"),
                        Corp_Name = item.Field<string>("Corp_Name"),
                        FirstID = item.Field<string>("FID"),
                        SecondID = item.Field<string>("SID"),
                        ModelNo = item.Field<string>("ModelNo"),
                        ModelName = item.Field<string>("ModelName"),
                        ShipFrom = item.Field<string>("Ship_From"),
                        QC_Category = item.Field<string>("Pub_QC_Category"),
                        Vendor = item.Field<string>("Vendor"),
                        VendorName = item.Field<string>("VendorName"),
                        VendorAddress = item.Field<string>("VendorAddress"),
                        Est_CheckDay = item.Field<DateTime?>("Est_CheckDay").ToString().ToDateString("yyyy/MM/dd"),
                        Act_CheckDay = item.Field<DateTime?>("Act_CheckDay").ToString().ToDateString("yyyy/MM/dd"),
                        IsFinished = item.Field<string>("IsFinished"),
                        Status = item.Field<int>("Status"),
                        StatusName = item.Field<string>("StatusName"),
                        IsRel = (item.Field<int>("IsRel")) > 0 ? "Y" : "N",
                        IsReported = (item.Field<int>("IsReported")) > 0 ? "Y" : "N",
                        IsLock = item.Field<string>("IsLock"),
                        Remark = item.Field<string>("Remark") ?? "",

                        SubNo_TW = item.Field<string>("SubNo_TW"),
                        SubNo_SH = item.Field<string>("SubNo_SH"),
                        SubNo_SZ = item.Field<string>("SubNo_SZ"),
                        ProdNotes = item.Field<string>("ProdNotes"),

                        Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Mail_Time = item.Field<DateTime?>("Mail_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Approved_Time = item.Field<DateTime?>("Approved_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                        Create_Name = item.Field<string>("Create_Name"),
                        Update_Name = item.Field<string>("Update_Name"),
                        Mail_Name = item.Field<string>("Mail_Name"),
                        Approved_Name = item.Field<string>("Approved_Name")

                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }

        /// <summary>
        /// 取得ERP採購單資料 - 建立時使用
        /// </summary>
        /// <param name="corp">公司別 UID</param>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ERP_PurData> GetPurData(string corp, Dictionary<int, string> search)
        {
            //取得資料庫名
            string dbName = GetDBName(corp);

            //----- 宣告 -----
            List<ERP_PurData> dataList = new List<ERP_PurData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(Base.TC001) FirstID, RTRIM(Base.TC002) SecondID, Base.TC003 BuyDate");
                sql.AppendLine("  , RTRIM(Cust.MA001) CustID, RTRIM(Cust.MA002) CustName");
                sql.AppendLine("  , RTRIM(Sub.TD004) ModelNo, SUM(Sub.TD008) BuyQty");
                sql.AppendLine("  , (SELECT COUNT(*) FROM Prod_Check chk WHERE (chk.FID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC001) AND (chk.SID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC002) AND (chk.ModelNo COLLATE Chinese_Taiwan_Stroke_BIN = Sub.TD004)) AS RelCnt");
                sql.AppendLine(" FROM [{0}].dbo.PURTC Base WITH(NOLOCK)".FormatThis(dbName));
                sql.AppendLine("  INNER JOIN [{0}].dbo.PURMA Cust WITH(NOLOCK) ON Base.TC004 = Cust.MA001".FormatThis(dbName));
                sql.AppendLine("  INNER JOIN [{0}].dbo.PURTD Sub WITH(NOLOCK) ON Base.TC001 = Sub.TD001 AND Base.TC002 = Sub.TD002".FormatThis(dbName));
                sql.AppendLine(" WHERE (Base.TC014 = 'Y') ");
                //AND (UPPER(Sub.TD016) = 'Y') --結案碼

                /* Search */
                #region >> filter <<

                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                //ModelNo
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Sub.TD004 = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Vendor:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Cust.MA001 = @Vendor)");

                                    cmd.Parameters.AddWithValue("Vendor", item.Value);
                                }

                                break;

                            case (int)mySearch.FirstID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.TC001 = @FirstID)");

                                    cmd.Parameters.AddWithValue("FirstID", item.Value);
                                }

                                break;

                            case (int)mySearch.SecondID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.TC002 = @SecondID)");

                                    cmd.Parameters.AddWithValue("SecondID", item.Value);
                                }

                                break;

                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //傳入格式為yyyyMMdd
                                    sql.Append(" AND (Base.TC003 >= @sDate) ");

                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;

                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    //傳入格式為yyyyMMdd
                                    sql.Append(" AND (Base.TC003 <= @eDate)");

                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;

                        }
                    }
                }
                #endregion


                sql.AppendLine(" GROUP BY Base.TC001, Base.TC002, Base.TC003, Cust.MA001, Cust.MA002, Sub.TD004");
                sql.AppendLine(" ORDER BY Sub.TD004, Base.TC003 DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ERP_PurData
                        {
                            FirstID = item.Field<string>("FirstID"),
                            SecondID = item.Field<string>("SecondID"),
                            BuyDate = item.Field<string>("BuyDate"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            ModelNo = item.Field<string>("ModelNo"),
                            BuyQty = item.Field<Decimal>("BuyQty"),
                            RelCnt = item.Field<Int32>("RelCnt")
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
        /// 取得品號列表 - 品號附件維護
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<ProdItems> GetProdList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<ProdItems> dataList = new List<ProdItems>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT");
                sql.AppendLine("  RTRIM(Prod.Model_No) ModelNo, RTRIM(Model_Name_zh_TW) AS ModelName");
                sql.AppendLine("  , (SELECT COUNT(*) FROM Prod_Check_RefDoc Ref WHERE (Ref.ModelNo = Prod.Model_No)) RefCnt");
                sql.AppendLine(" FROM Prod_Item Prod");
                sql.AppendLine(" WHERE (LEFT(Prod.Model_No, 1) <> '0')");

                /* Search */
                #region >> filter <<

                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Prod.Model_No)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Prod.Model_Name_zh_TW)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Prod.Model_Name_zh_CN)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Prod.Model_Name_en_US)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY Prod.Model_No");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ProdItems
                        {
                            ModelNo = item.Field<string>("ModelNo"),
                            ModelName = item.Field<string>("ModelName"),
                            AttachCnt = item.Field<int>("RefCnt")
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
        /// 取得採購單未關聯資料 (排除主檔的單號) - 採購單關聯設定
        /// </summary>
        /// <param name="corp"></param>
        /// <param name="vendor"></param>
        /// <param name="modelNo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// SQL資料取去年至今年
        /// </remarks>
        public IQueryable<ERP_PurData> GetPurRelData(string corp, string vendor, string modelNo, string id)
        {
            //取得資料庫名
            string dbName = GetDBName(corp);

            //----- 宣告 -----
            List<ERP_PurData> dataList = new List<ERP_PurData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(Base.TC001) FirstID, RTRIM(Base.TC002) SecondID, Base.TC003 BuyDate");
                sql.AppendLine("  , RTRIM(Cust.MA001) CustID, RTRIM(Cust.MA002) CustName");
                sql.AppendLine("  , RTRIM(Sub.TD004) ModelNo, SUM(Sub.TD008) BuyQty");
                sql.AppendLine(" FROM {0}.dbo.PURTC Base WITH(NOLOCK)".FormatThis(dbName));
                sql.AppendLine("  INNER JOIN {0}.dbo.PURMA Cust WITH(NOLOCK) ON Base.TC004 = Cust.MA001".FormatThis(dbName));
                sql.AppendLine("  INNER JOIN {0}.dbo.PURTD Sub WITH(NOLOCK) ON Base.TC001 = Sub.TD001 AND Base.TC002 = Sub.TD002".FormatThis(dbName));
                sql.AppendLine(" WHERE (Base.TC014 = 'Y')"); //AND (UPPER(Sub.TD016) = 'Y')
                sql.AppendLine("  AND (Cust.MA001 = @Vendor) AND (Sub.TD004 = @ModelNo)");
                sql.AppendLine("  AND (Base.TC003 >= (CAST(YEAR(GETDATE())-1 AS VARCHAR(4)) + '0101')) AND (Base.TC003 <= (CAST(YEAR(GETDATE()) AS VARCHAR(4)) + '1231'))");
                sql.AppendLine("  AND NOT EXISTS (");
                sql.AppendLine("   SELECT Data_ID FROM Prod_Check WHERE FID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC001 AND SID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC002 AND Data_ID = @DataID");
                sql.AppendLine("  )");
                sql.AppendLine("  AND NOT EXISTS (");
                sql.AppendLine("   SELECT Data_ID FROM Prod_Check_Rel WHERE FID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC001 AND SID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC002 AND Data_ID = @DataID");
                sql.AppendLine("  )");
                sql.AppendLine(" GROUP BY Base.TC001, Base.TC002, Base.TC003, Cust.MA001, Cust.MA002, Sub.TD004");
                sql.AppendLine(" ORDER BY Sub.TD004, Base.TC003 DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Vendor", vendor);
                cmd.Parameters.AddWithValue("ModelNo", modelNo);
                cmd.Parameters.AddWithValue("DataID", id);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ERP_PurData
                        {
                            FirstID = item.Field<string>("FirstID"),
                            SecondID = item.Field<string>("SecondID"),
                            BuyDate = item.Field<string>("BuyDate"),
                            CustID = item.Field<string>("CustID"),
                            CustName = item.Field<string>("CustName"),
                            ModelNo = item.Field<string>("ModelNo"),
                            BuyQty = item.Field<Decimal>("BuyQty")
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
        /// 取得類別
        /// </summary>
        /// <param name="cls">class</param>
        /// <returns></returns>
        public IQueryable<CheckClass> GetClassList(string cls)
        {
            //----- 宣告 -----
            List<CheckClass> dataList = new List<CheckClass>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Class_ID AS ID, Class_Name AS Label");
                sql.AppendLine(" FROM Prod_Check_Class WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Display = 'Y')");

                if (!string.IsNullOrEmpty(cls))
                {
                    sql.Append(" AND (Class_ID = @Class)");

                    cmd.Parameters.AddWithValue("Class", cls);
                }

                sql.AppendLine(" ORDER BY Sort");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new CheckClass
                        {
                            ID = item.Field<int>("ID"),
                            Label = item.Field<string>("Label")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得更新人員
        /// </summary>
        /// <returns></returns>
        public IQueryable<UpdateWho> GetUpdWhoList()
        {
            //----- 宣告 -----
            List<UpdateWho> dataList = new List<UpdateWho>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Prof.Guid AS ID, (Prof.Account_Name + ' (' + Prof.Display_Name + ')') AS Label");
                sql.AppendLine(" FROM Prod_Check Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.User_Profile Prof WITH(NOLOCK) ON Base.Update_Who = Prof.Guid");
                sql.AppendLine(" GROUP BY Prof.Guid, Prof.DeptID, Prof.Account_Name, Prof.Display_Name");
                sql.AppendLine(" ORDER BY Prof.DeptID, Prof.Account_Name");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new UpdateWho
                        {
                            ID = item.Field<string>("ID"),
                            Label = item.Field<string>("Label")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得關聯資料 - 主檔編輯頁
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public IQueryable<RelData> GetRelList(string id)
        {
            //----- 宣告 -----
            List<RelData> dataList = new List<RelData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT FID, SID");
                sql.AppendLine(" FROM Prod_Check_Rel WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");
                sql.AppendLine(" ORDER BY FID, SID");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", id);

                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RelData
                        {
                            FirstID = item.Field<string>("FID"),
                            SecondID = item.Field<string>("SID")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得資料庫名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetDBName(string id)
        {
            //----- 宣告:資料參數 -----
            ParamsRepository _data = new ParamsRepository();
            Dictionary<int, string> search = new Dictionary<int, string>();

            //----- 原始資料:條件篩選 -----
            search.Add((int)Common.mySearch.DataID, id);

            //----- 原始資料:取得資料 -----
            var query = _data.GetCorpList(search).Take(1).FirstOrDefault();


            //Check Null
            if (query == null)
            {
                return "prokit2";
            }
            else
            {
                return query.DB_Name;
            }
        }


        /// <summary>
        /// 取得檢驗數量加總 - 主檔編輯頁
        /// </summary>
        /// <param name="corp"></param>
        /// <param name="dataID"></param>
        /// <param name="modelNo"></param>
        /// <returns></returns>
        public int GetTotalCnt(string corp, string dataID, string modelNo)
        {
            //取得資料庫名
            string dbName = GetDBName(corp);

            //----- 宣告 -----
            List<ERP_PurData> dataList = new List<ERP_PurData>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT ISNULL(SUM(Tbl.BuyCnt), 0) AS TotalCnt FROM (");
                sql.AppendLine(" SELECT Sub.TD008 BuyCnt");
                sql.AppendLine(" FROM [{0}].dbo.PURTC Base WITH(NOLOCK)".FormatThis(dbName));
                sql.AppendLine("  INNER JOIN [{0}].dbo.PURTD Sub WITH(NOLOCK) ON Base.TC001 = Sub.TD001 AND Base.TC002 = Sub.TD002".FormatThis(dbName));
                sql.AppendLine(" WHERE (Base.TC014 = 'Y') AND (Sub.TD004 = @ModelNo)"); // AND (UPPER(Sub.TD016) = 'Y')
                sql.AppendLine("  AND EXISTS (");
                sql.AppendLine("   SELECT Data_ID FROM Prod_Check WHERE FID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC001 AND SID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC002 AND Data_ID = @DataID");
                sql.AppendLine("  )");

                sql.AppendLine(" UNION ALL");

                sql.AppendLine(" SELECT Sub.TD008 BuyCnt");
                sql.AppendLine(" FROM [{0}].dbo.PURTC Base WITH(NOLOCK)".FormatThis(dbName));
                sql.AppendLine("  INNER JOIN [{0}].dbo.PURTD Sub WITH(NOLOCK) ON Base.TC001 = Sub.TD001 AND Base.TC002 = Sub.TD002".FormatThis(dbName));
                sql.AppendLine(" WHERE (Base.TC014 = 'Y') AND (Sub.TD004 = @ModelNo)"); // AND (UPPER(Sub.TD016) = 'Y')
                sql.AppendLine("  AND EXISTS (");
                sql.AppendLine("   SELECT Data_ID FROM Prod_Check_Rel WHERE FID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC001 AND SID COLLATE Chinese_Taiwan_Stroke_BIN = Base.TC002 AND Data_ID = @DataID");
                sql.AppendLine("  )");
                sql.AppendLine(" ) AS Tbl");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);
                cmd.Parameters.AddWithValue("DataID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return 0;
                    }

                    return Convert.ToInt32(DT.Rows[0]["TotalCnt"]);
                }

            }
        }


        /// <summary>
        /// 取得ERP檢驗項目
        /// </summary>
        /// <param name="shipFrom"></param>
        /// <param name="id"></param>
        /// <param name="QC_Category"></param>
        /// <returns></returns>
        public IQueryable<CheckItems> GetCheckItems(string shipFrom, string id, string QC_Category)
        {
            //----- 宣告 -----
            List<CheckItems> dataList = new List<CheckItems>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.ME002 chkType, Sub.MG005 Lv, Sub.MG006 Spec, Sub.MG007 Remark");
                sql.AppendLine(" FROM ERP_QMSME Base WITH(NOLOCK)");
                sql.AppendLine("  INNER JOIN ERP_QMSMG Sub WITH(NOLOCK) ON Base.ME001 = Sub.MG003 AND Base.DBS = Sub.DBS");
                sql.AppendLine(" WHERE (Base.DBS = @ShipFrom) AND (Sub.MG002 = @id) AND (Sub.MG001 = @QC_Category)");
                sql.AppendLine(" ORDER BY Sub.MG003");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ShipFrom", shipFrom);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Parameters.AddWithValue("QC_Category", QC_Category);

                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new CheckItems
                        {
                            chkType = item.Field<string>("chkType"),
                            Lv = item.Field<string>("Lv"),
                            Spec = item.Field<string>("Spec"),
                            Remark = item.Field<string>("Remark")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得檢驗報告檔案
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IQueryable<CheckFiles> GetFileList(string dataID, string type)
        {
            //----- 宣告 -----
            List<CheckFiles> dataList = new List<CheckFiles>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Data_ID, AttachID, AttachType, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                sql.AppendLine(" FROM Prod_Check_Report WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Data_ID = @DataID) AND (AttachType = @type)");
                sql.AppendLine(" ORDER BY Create_Time");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("type", type);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new CheckFiles
                        {
                            Data_ID = item.Field<Guid>("Data_ID"),
                            AttachID = item.Field<int>("AttachID"),
                            AttachFile = item.Field<string>("AttachFile"),
                            AttachFile_Name = item.Field<string>("AttachFile_Name"),
                            Create_Who = item.Field<string>("Create_Who"),
                            Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")
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
        /// 取得品號附件
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<ProdItemsFiles> GetItemFileList(string dataID)
        {
            //----- 宣告 -----
            List<ProdItemsFiles> dataList = new List<ProdItemsFiles>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT AttachID, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                sql.AppendLine(" FROM Prod_Check_RefDoc WITH(NOLOCK)");
                sql.AppendLine(" WHERE (ModelNo = @DataID)");
                sql.AppendLine(" ORDER BY Create_Time");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new ProdItemsFiles
                        {
                            AttachID = item.Field<int>("AttachID"),
                            AttachFile = item.Field<string>("AttachFile"),
                            AttachFile_Name = item.Field<string>("AttachFile_Name"),
                            Create_Who = item.Field<string>("Create_Who"),
                            Create_Time = item.Field<DateTime>("Create_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")
                        };

                        //將項目加入至集合
                        dataList.Add(data);

                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        #endregion


        #region -----// Create //-----

        /// <summary>
        /// 建立基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create(ProdCheck instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO Prod_Check( ");
                sql.AppendLine("  Data_ID, Corp_UID, FID, SID");
                sql.AppendLine("  , ModelNo, Vendor, Est_CheckDay");
                sql.AppendLine("  , IsFinished, Status");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @Data_ID, @Corp_UID, @FID, @SID");
                sql.AppendLine("  , @ModelNo, @Vendor, GETDATE()");
                sql.AppendLine("  , 'N', 1");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Data_ID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Corp_UID", instance.Corp_UID);
                cmd.Parameters.AddWithValue("FID", instance.FirstID);
                cmd.Parameters.AddWithValue("SID", instance.SecondID);
                cmd.Parameters.AddWithValue("ModelNo", instance.ModelNo);
                cmd.Parameters.AddWithValue("Vendor", instance.Vendor);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 建立關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Rel(RelData instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" IF (SELECT COUNT(*) FROM Prod_Check_Rel WHERE (Data_ID = @DataID) AND (FID = @FID) AND (SID = @SID)) = 0");
                sql.AppendLine(" INSERT INTO Prod_Check_Rel( ");
                sql.AppendLine("  Data_ID, FID, SID");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @DataID, @FID, @SID");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.DataID);
                cmd.Parameters.AddWithValue("FID", instance.FirstID);
                cmd.Parameters.AddWithValue("SID", instance.SecondID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 建立檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Attachment(List<CheckFiles> instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DECLARE @NewID AS INT ");

                for (int row = 0; row < instance.Count; row++)
                {
                    sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(AttachID), 0) + 1 FROM Prod_Check_Report);");
                    sql.AppendLine(" INSERT INTO Prod_Check_Report( ");
                    sql.AppendLine("  Data_ID, AttachID, AttachType, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @DataID, @NewID, @AttachType, @AttachFile_{0}, @AttachFile_Name_{0}, @Create_Who, GETDATE()".FormatThis(row));
                    sql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AttachFile_{0}".FormatThis(row), instance[row].AttachFile);
                    cmd.Parameters.AddWithValue("AttachFile_Name_{0}".FormatThis(row), instance[row].AttachFile_Name);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance[0].Data_ID);
                cmd.Parameters.AddWithValue("AttachType", instance[0].AttachType);
                cmd.Parameters.AddWithValue("Create_Who", instance[0].Create_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 建立品號附件
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_ItemAttachment(List<ProdItemsFiles> instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DECLARE @NewID AS INT ");

                for (int row = 0; row < instance.Count; row++)
                {
                    sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(AttachID), 0) + 1 FROM Prod_Check_RefDoc);");
                    sql.AppendLine(" INSERT INTO Prod_Check_RefDoc( ");
                    sql.AppendLine("  ModelNo, AttachID, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @DataID, @NewID, @AttachFile_{0}, @AttachFile_Name_{0}, @Create_Who, GETDATE()".FormatThis(row));
                    sql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AttachFile_{0}".FormatThis(row), instance[row].AttachFile);
                    cmd.Parameters.AddWithValue("AttachFile_Name_{0}".FormatThis(row), instance[row].AttachFile_Name);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance[0].ModelNo);
                cmd.Parameters.AddWithValue("Create_Who", instance[0].Create_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion


        #region -----// Update //-----

        /// <summary>
        /// 更新基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update(ProdCheck instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_Check SET ");
                sql.AppendLine("  Est_CheckDay = @Est_CheckDay, Act_CheckDay = @Act_CheckDay");
                sql.AppendLine("  , Status = @Status, Remark = @Remark");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Est_CheckDay", string.IsNullOrEmpty(instance.Est_CheckDay) ? DBNull.Value : (object)instance.Est_CheckDay.ToDateString("yyyy-MM-dd"));  //Nullable
                cmd.Parameters.AddWithValue("Act_CheckDay", string.IsNullOrEmpty(instance.Act_CheckDay) ? DBNull.Value : (object)instance.Act_CheckDay.ToDateString("yyyy-MM-dd"));     //Nullable
                cmd.Parameters.AddWithValue("Status", instance.Status);
                cmd.Parameters.AddWithValue("Remark", instance.Remark);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 設為結案
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Finish(ProdCheck instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_Check SET IsFinished = 'Y', Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 設為隱藏
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Lock(ProdCheck instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_Check SET IsLock = 'Y', Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 已發送郵件
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_MailSent(ProdCheck instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_Check SET Mail_Who = @Mail_Who, Mail_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Mail_Who", instance.Mail_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// 核准
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Approve(ProdCheck instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_Check SET Approved_Who = @Approved_Who, Approved_Time = GETDATE()");
                sql.AppendLine(" WHERE (Data_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("Approved_Who", instance.Approved_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// 刪除檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_Files(CheckFiles instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Prod_Check_Report");
                sql.AppendLine(" WHERE (Data_ID = @DataID) AND (AttachID = @AttachID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.Data_ID);
                cmd.Parameters.AddWithValue("AttachID", instance.AttachID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除品號附件
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_ItemFiles(ProdItemsFiles instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Prod_Check_RefDoc");
                sql.AppendLine(" WHERE (AttachID = @AttachID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("AttachID", instance.AttachID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_RelData(RelData instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Prod_Check_Rel");
                sql.AppendLine(" WHERE (Data_ID = @DataID) AND (FID = @FID) AND (SID = @SID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.DataID);
                cmd.Parameters.AddWithValue("FID", instance.FirstID);
                cmd.Parameters.AddWithValue("SID", instance.SecondID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="search">查詢</param>
        /// <returns></returns>
        private DataTable LookupRawData(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT Tbl.* FROM (");
                sql.AppendLine(" SELECT Base.SeqNo, Base.Data_ID, Base.Corp_UID");
                sql.AppendLine("   , Base.FID, Base.SID, Base.Est_CheckDay, Base.Act_CheckDay");
                sql.AppendLine("   , Base.IsFinished, Base.Status, Base.Remark, Base.IsLock");
                sql.AppendLine("   , Base.Create_Time, Base.Update_Time, Base.Mail_Time, Base.Approved_Time");
                sql.AppendLine("   , Cls.Class_Name StatusName, Prod.Ship_From, Prod.Pub_QC_Category");
                sql.AppendLine("   , Corp.Corp_Name");
                sql.AppendLine("   , RTRIM(Prod.Model_No) ModelNo, Prod.Model_Name_zh_TW ModelName");
                sql.AppendLine("   , Prod.Substitute_Model_No_TW SubNo_TW, Prod.Substitute_Model_No_SH SubNo_SH, Prod.Substitute_Model_No_SZ SubNo_SZ, Prod.Pub_Notes ProdNotes");
                sql.AppendLine("   , RTRIM(Sup.MA001) Vendor, RTRIM(Sup.MA002) VendorName, RTRIM(Sup.MA014) VendorAddress");
                sql.AppendLine("   , (SELECT COUNT(*) FROM Prod_Check_Rel Rel WHERE Rel.Data_ID = Base.Data_ID) AS IsRel");
                sql.AppendLine("   , (SELECT COUNT(*) FROM Prod_Check_Report Rpt WHERE Rpt.Data_ID = Base.Data_ID) AS IsReported");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name ");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name ");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Mail_Who)) AS Mail_Name ");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Approved_Who)) AS Approved_Name ");
                sql.AppendLine(" FROM Prod_Check Base");
                sql.AppendLine("  INNER JOIN Prod_Check_Class Cls ON Base.Status = Cls.Class_ID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Param_Corp Corp ON Base.Corp_UID = Corp.Corp_UID");
                sql.AppendLine("  INNER JOIN PKSYS.dbo.Supplier_ERPData Sup ON Base.Vendor = Sup.MA001 AND Sup.COMPANY = Corp.Corp_ID");
                sql.AppendLine("  INNER JOIN Prod_Item Prod ON Base.ModelNo = Prod.Model_No");
                sql.AppendLine(" WHERE (1 = 1) ");


                /* Search */
                #region >> filter <<

                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            #region - 篩選條件 -

                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Data_ID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(RTRIM(Prod.Model_No)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Sup.MA001)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(RTRIM(Sup.MA002)) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.FID) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.SID) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;


                            case (int)mySearch.Status:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Cls.Class_ID = @Status)");

                                    cmd.Parameters.AddWithValue("Status", item.Value);
                                }

                                break;


                            case (int)mySearch.IsLock:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.IsLock = @IsLock)");

                                    cmd.Parameters.AddWithValue("IsLock", item.Value);
                                }

                                break;


                            case (int)mySearch.StartDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Act_CheckDay >= @sDate) ");

                                    cmd.Parameters.AddWithValue("sDate", item.Value);
                                }

                                break;


                            case (int)mySearch.EndDate:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Act_CheckDay <= @eDate)");

                                    cmd.Parameters.AddWithValue("eDate", item.Value);
                                }

                                break;


                            case (int)mySearch.CorpID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Corp_UID = @Corp_UID)");

                                    cmd.Parameters.AddWithValue("Corp_UID", item.Value);
                                }

                                break;


                            case (int)mySearch.UpdateWho:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.Create_Who = @Create_Who)");

                                    cmd.Parameters.AddWithValue("Create_Who", item.Value);
                                }

                                break;


                            #endregion

                        }
                    }
                }
                #endregion

                sql.AppendLine(") AS Tbl ");
                sql.AppendLine(" ORDER BY Tbl.IsFinished ASC, Tbl.Create_Time DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();


                //----- 回傳資料 -----
                return dbConClass.LookupDT(cmd, out ErrMsg);
            }
        }


        #endregion
    }
}