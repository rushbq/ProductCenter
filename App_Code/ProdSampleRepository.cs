using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using PKLib_Method.Methods;
using ProdSampleData.Models;

namespace ProdSampleData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        Company = 3,
        Source = 4,
        Check = 5,
        Status = 6,
        DateType = 7,  //要放在sDate, eDate之前
        StartDate = 8,
        EndDate = 9,
        RelID = 10
    }


    /// <summary>
    /// 類別參數
    /// </summary>
    /// <remarks>
    /// 1:來源 / 2:檢驗類別 / 3:狀態
    /// </remarks>
    public enum myClass : int
    {
        source = 1,
        check = 2,
        status = 3
    }

    public class ProdSampleRepository
    {
        public string ErrMsg;


        #region -----// Read //-----
        /// <summary>
        /// [新品取樣] 指定資料
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ProdSample> GetOne_ProdSample(Dictionary<string, string> search, out string ErrMsg)
        {
            int dataCnt = 0;
            return GetProdSample_List(search, 0, 1, out dataCnt, out ErrMsg);
        }


        /// <summary>
        /// [新品取樣] 資料清單
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="startRow">StartRow</param>
        /// <param name="endRow">RecordsPerPage</param>
        /// <param name="DataCnt">傳址參數(資料總筆數)</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<ProdSample> GetProdSample_List(Dictionary<string, string> search, int startRow, int endRow
            , out int DataCnt, out string ErrMsg)
        {
            ErrMsg = "";

            try
            {
                /* 開始/結束筆數計算 */
                int cntStartRow = startRow + 1;
                int cntEndRow = startRow + endRow;

                //----- 宣告 -----
                List<ProdSample> dataList = new List<ProdSample>(); //資料容器
                List<SqlParameter> sqlParamList = new List<SqlParameter>(); //SQL參數容器
                List<SqlParameter> subParamList = new List<SqlParameter>(); //SQL參數取得
                StringBuilder sql = new StringBuilder(); //SQL語法容器
                StringBuilder subSql = new StringBuilder(); //條件SQL取得
                DataCnt = 0;    //資料總數

                //取得SQL語法
                subSql = SQL_ProdSample(search);
                //取得SQL參數集合
                subParamList = Params_ProdSample(search);


                #region >> 資料筆數SQL查詢 <<
                using (SqlCommand cmdCnt = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.Clear();
                    sql.AppendLine(" SELECT COUNT(TbAll.SP_ID) AS TotalCnt FROM (");

                    //子查詢SQL
                    sql.Append(subSql);

                    sql.AppendLine(" ) AS TbAll");

                    //----- SQL 執行 -----
                    cmdCnt.CommandText = sql.ToString();
                    cmdCnt.Parameters.Clear();
                    sqlParamList.Clear();
                    //cmd.CommandTimeout = 60;   //單位:秒

                    //----- SQL 固定參數 -----
                    //sqlParamList.Add(new SqlParameter("@CC_Type", CCType));

                    //----- SQL 條件參數 -----
                    //加入篩選後的參數
                    sqlParamList.AddRange(subParamList);

                    //加入參數陣列
                    cmdCnt.Parameters.AddRange(sqlParamList.ToArray());

                    //Execute
                    using (DataTable DTCnt = dbConClass.LookupDT(cmdCnt, out ErrMsg))
                    {
                        //資料總筆數
                        if (DTCnt.Rows.Count > 0)
                        {
                            DataCnt = Convert.ToInt32(DTCnt.Rows[0]["TotalCnt"]);
                        }
                    }

                    //*** 在SqlParameterCollection同個循環內不可有重複的SqlParam,必須清除才能繼續使用. ***
                    cmdCnt.Parameters.Clear();
                }
                #endregion


                #region >> 主要資料SQL查詢 <<

                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    sql.Clear();
                    sql.AppendLine(" SELECT TbAll.* FROM (");

                    //子查詢SQL
                    sql.Append(subSql);

                    sql.AppendLine(" ) AS TbAll");

                    sql.AppendLine(" WHERE (TbAll.RowIdx >= @startRow) AND (TbAll.RowIdx <= @endRow)");
                    sql.AppendLine(" ORDER BY TbAll.RowIdx");

                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.Clear();
                    sqlParamList.Clear();
                    //cmd.CommandTimeout = 60;   //單位:秒

                    //----- SQL 固定參數 -----
                    sqlParamList.Add(new SqlParameter("@startRow", cntStartRow));
                    sqlParamList.Add(new SqlParameter("@endRow", cntEndRow));

                    //----- SQL 條件參數 -----
                    //加入篩選後的參數
                    sqlParamList.AddRange(subParamList);

                    //加入參數陣列
                    cmd.Parameters.AddRange(sqlParamList.ToArray());

                    //Execute
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new ProdSample
                            {
                                SeqNo = item.Field<int>("SeqNo"),
                                SP_ID = item.Field<Guid>("SP_ID"),

                                Qty = item.Field<int?>("Qty"),
                                Cls_Source = item.Field<int?>("Cls_Source"),
                                Cls_Check = item.Field<int?>("Cls_Check"),
                                Cls_Status = item.Field<int?>("Cls_Status"),

                                Date_Come = item.Field<DateTime?>("Date_Come").ToString().ToDateString("yyyy/MM/dd"),
                                Date_Est = item.Field<DateTime?>("Date_Est").ToString().ToDateString("yyyy/MM/dd"),
                                Date_Actual = item.Field<DateTime?>("Date_Actual").ToString().ToDateString("yyyy/MM/dd"),
                                Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                                Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),

                                //樣品編號:TWS-1602-001
                                SerialNo = (item.Field<int>("SerialID")) == null ? "資料建立中" :
                                 "{0}-{1}-{2}".FormatThis(
                                    item.Field<string>("Company")
                                    , item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyMM")
                                    , ("00" + item.Field<int>("SerialID")).Right(3)
                                ),
                                Company = item.Field<string>("Company"),
                                Assign_Who = item.Field<string>("Assign_Who"),
                                Model_No = item.Field<string>("Model_No"),
                                Sup_Corp = item.Field<int?>("Sup_Corp"),
                                Sup_ErpID = item.Field<string>("Sup_ErpID"),
                                Cust_ModelNo = item.Field<string>("Cust_ModelNo"),
                                Cust_Newguy = item.Field<string>("Cust_Newguy"),
                                Description1 = string.IsNullOrEmpty(item.Field<string>("Description1")) ? "" : item.Field<string>("Description1"),
                                Description2 = string.IsNullOrEmpty(item.Field<string>("Description2")) ? "" : item.Field<string>("Description2"),
                                Description3 = string.IsNullOrEmpty(item.Field<string>("Description3")) ? "" : item.Field<string>("Description3"),
                                Description4 = string.IsNullOrEmpty(item.Field<string>("Description4")) ? "" : item.Field<string>("Description4"),
                                Description5 = string.IsNullOrEmpty(item.Field<string>("Description5")) ? "" : item.Field<string>("Description5"),
                                Remark = string.IsNullOrEmpty(item.Field<string>("Remark")) ? "" : item.Field<string>("Remark"),
                                Create_Who = item.Field<string>("Create_Who"),
                                Update_Who = item.Field<string>("Update_Who"),
                                Company_Name = item.Field<string>("myCompany"),
                                Assign_Name = item.Field<string>("AssignName"),
                                Source_Name = item.Field<string>("mySource"),
                                Check_Name = item.Field<string>("myCheck"),
                                Status_Name = item.Field<string>("myStatus"),
                                Cust_Name = item.Field<string>("CustName"),
                                Create_Name = item.Field<string>("Create_Name"),
                                Update_Name = item.Field<string>("Update_Name")
                            };


                            //將項目加入至集合
                            dataList.Add(data);

                        }
                    }

                    //回傳集合
                    return dataList.AsQueryable();
                }

                #endregion

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// [新品取樣] 取得SQL查詢
        /// ** TSQL查詢條件寫在此 **
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="fieldLang">欄位語系(ex:zh_TW)</param>
        /// <returns></returns>
        private StringBuilder SQL_ProdSample(Dictionary<string, string> search)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(" SELECT Base.* ");
            sql.AppendLine("   , (CASE Base.Company WHEN 'TWS' THEN '台灣' ELSE '上海' END) AS myCompany");
            sql.AppendLine("   , ClsSrc.Class_Name AS mySource, ClsChk.Class_Name AS myCheck, ClsSt.Class_Name AS myStatus");
            sql.AppendLine("   , Prof.Display_Name AS AssignName");
            sql.AppendLine("   , ISNULL(RTRIM(Sup.MA002 + ' (' + Sup.MA001 + ')'), Base.Cust_Newguy) AS CustName");
            sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name ");
            sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name ");
            sql.AppendLine("   , ROW_NUMBER() OVER(ORDER BY Base.Create_Time DESC) AS RowIdx");
            sql.AppendLine(" FROM Sample_List Base");
            sql.AppendLine("  LEFT JOIN Sample_Class ClsSrc ON Base.Cls_Source = ClsSrc.Class_ID");
            sql.AppendLine("  LEFT JOIN Sample_Class ClsChk ON Base.Cls_Check = ClsChk.Class_ID");
            sql.AppendLine("  LEFT JOIN Sample_Class ClsSt ON Base.Cls_Status = ClsSt.Class_ID");
            sql.AppendLine("  LEFT JOIN PKSYS.dbo.User_Profile Prof WITH(NOLOCK) ON Base.Assign_Who = Prof.Account_Name");
            sql.AppendLine("  LEFT JOIN PKSYS.dbo.Param_Corp Corp ON Corp.Corp_UID = Base.Sup_Corp");
            sql.AppendLine("  LEFT JOIN PKSYS.dbo.Supplier_ERPData Sup ON Sup.MA001 = Base.Sup_ErpID AND Corp.Corp_ID = RTRIM(Sup.COMPANY)");
            sql.AppendLine(" WHERE (1 = 1) ");

            /* Search */
            #region >> filter <<

            if (search != null)
            {
                //過濾空值
                var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));
                string filterDateType = "Base.Create_Time";

                //查詢內容
                foreach (var item in thisSearch)
                {
                    switch (item.Key)
                    {
                        case "DataID":
                            sql.Append(" AND (Base.SP_ID = @DataID)");

                            break;

                        case "Keyword":
                            sql.Append(" AND (");
                            sql.Append("    (UPPER(RTRIM(Base.Model_No)) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR (UPPER(RTRIM(Base.Cust_ModelNo)) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR (UPPER(Base.Cust_Newguy) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR (UPPER(Base.Description1) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append("    OR ((Base.Company + RIGHT(CONVERT(VARCHAR(6), Base.Create_Time, 112), 4) + RIGHT('00' + CAST(SerialID AS VARCHAR), 3)) LIKE '%' + REPLACE(UPPER(@Keyword), '-', '') + '%')");
                            sql.Append("    OR (UPPER(Sup.MA002) LIKE '%' + UPPER(@Keyword) + '%')");
                            sql.Append(" )");

                            break;

                        case "Company":
                            sql.Append(" AND (Base.Company = @Company)");

                            break;

                        case "Source":
                            sql.Append(" AND (Base.Cls_Source = @Source)");

                            break;

                        case "Check":
                            sql.Append(" AND (Base.Cls_Check = @Check)");

                            break;

                        case "Status":
                            sql.Append(" AND (Base.Cls_Status = @Status)");

                            break;

                        case "AssignWho":
                            sql.Append(" AND (Base.Assign_Who = @AssignWho)");

                            break;

                        case "DateType":
                            switch (item.Value)
                            {
                                case "1":
                                    filterDateType = "Base.Date_Come";
                                    break;

                                case "2":
                                    filterDateType = "Base.Date_Est";
                                    break;

                                case "3":
                                    filterDateType = "Base.Date_Actual";
                                    break;

                                default:
                                    filterDateType = "Base.Create_Time";
                                    break;
                            }

                            break;

                        case "StartDate":
                            sql.Append(" AND ({0} >= @sDate)".FormatThis(filterDateType));
                            break;
                        case "EndDate":
                            sql.Append(" AND ({0} <= @eDate)".FormatThis(filterDateType));
                            break;


                        case "RelID":
                            //樣品資料關聯
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                sql.Append("AND (Base.SP_ID IN (");
                                sql.Append(" SELECT Rel_ID FROM Sample_Rel_ID");
                                sql.Append(" WHERE (SP_ID = @RelID)");
                                sql.Append("))");
                            }

                            break;
                    }
                }
            }
            #endregion

            return sql;
        }


        /// <summary>
        /// [新品取樣] 取得條件參數
        /// ** SQL參數設定寫在此 **
        /// </summary>
        /// <param name="search">search集合</param>
        /// <returns></returns>
        private List<SqlParameter> Params_ProdSample(Dictionary<string, string> search)
        {
            //declare
            List<SqlParameter> sqlParamList = new List<SqlParameter>();

            //get values
            if (search != null)
            {
                //過濾空值
                var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                //查詢內容
                foreach (var item in thisSearch)
                {
                    switch (item.Key)
                    {
                        case "DataID":
                            sqlParamList.Add(new SqlParameter("@DataID", item.Value));

                            break;

                        case "Keyword":
                            sqlParamList.Add(new SqlParameter("@Keyword", item.Value));

                            break;

                        case "Company":
                            sqlParamList.Add(new SqlParameter("@Company", item.Value));

                            break;

                        case "Source":
                            sqlParamList.Add(new SqlParameter("@Source", item.Value));

                            break;

                        case "Check":
                            sqlParamList.Add(new SqlParameter("@Check", item.Value));

                            break;

                        case "Status":
                            sqlParamList.Add(new SqlParameter("@Status", item.Value));

                            break;

                        case "AssignWho":
                            sqlParamList.Add(new SqlParameter("@AssignWho", item.Value));

                            break;

                        case "StartDate":
                            sqlParamList.Add(new SqlParameter("@sDate", item.Value + " 00:00:00"));
                            break;
                        case "EndDate":
                            sqlParamList.Add(new SqlParameter("@eDate", item.Value + " 23:59:59"));
                            break;


                        case "RelID":
                            //樣品資料關聯
                            sqlParamList.Add(new SqlParameter("@RelID", item.Value));

                            break;
                    }
                }
            }


            return sqlParamList;
        }

        
        /// <summary>
        /// 取得類別
        /// </summary>
        /// <param name="cls">type</param>
        /// <returns></returns>
        public IQueryable<SampleClass> GetClassList(myClass cls)
        {
            //----- 宣告 -----
            List<SampleClass> dataList = new List<SampleClass>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData_Status(cls))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var data = new SampleClass
                    {
                        ID = item.Field<int>("ID"),
                        Label = item.Field<string>("Label")
                    };

                    //將項目加入至集合
                    dataList.Add(data);

                }
            }

            //回傳集合
            return dataList.AsQueryable();
        }


        /// <summary>
        /// 取得品號關聯
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<RelModelNo> GetRelModelList(string dataID)
        {
            //----- 宣告 -----
            List<RelModelNo> dataList = new List<RelModelNo>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT SP_ID AS ID, Model_No AS Label");
                sql.AppendLine(" FROM Sample_Rel_ModelNo WITH(NOLOCK)");
                sql.AppendLine(" WHERE (SP_ID = @DataID)");
                sql.AppendLine(" ORDER BY Model_No");


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
                        var data = new RelModelNo
                        {
                            SP_ID = item.Field<Guid>("ID"),
                            Model_No = item.Field<string>("Label")
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
        /// 取得檔案附件
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<SampleFiles> GetFileList(string dataID)
        {
            //----- 宣告 -----
            List<SampleFiles> dataList = new List<SampleFiles>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT AttachID, SP_ID, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                sql.AppendLine(" FROM Sample_Attachment WITH(NOLOCK)");
                sql.AppendLine(" WHERE (SP_ID = @DataID)");
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
                        var data = new SampleFiles
                        {
                            AttachID = item.Field<int>("AttachID"),
                            SP_ID = item.Field<Guid>("SP_ID"),
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
        public bool Create(ProdSample instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("DECLARE @NewID AS INT ");
                sql.AppendLine("SET @NewID = (SELECT ISNULL(MAX(SerialID), 0) + 1 FROM Sample_List WHERE (Company = @Company) AND (CONVERT(VARCHAR(6), Create_Time,112) = CONVERT(VARCHAR(6), GETDATE(),112))) ");
                sql.AppendLine(" INSERT INTO Sample_List( ");
                sql.AppendLine("  SP_ID, Company, SerialID");
                sql.AppendLine("  , Cls_Status, Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @SP_ID, @Company, @NewID");
                sql.AppendLine("  , @Cls_Status, @Create_Who, GETDATE()");
                sql.AppendLine(" );");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("SP_ID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Company", instance.Company);
                cmd.Parameters.AddWithValue("Cls_Status", instance.Cls_Status);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// 建立品號關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_RelByModelNo(RelModelNo instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" IF (SELECT COUNT(*) FROM Sample_Rel_ModelNo WHERE (SP_ID = @DataID) AND (Model_No = @Model_No)) = 0");
                sql.AppendLine(" INSERT INTO Sample_Rel_ModelNo( ");
                sql.AppendLine("  SP_ID, Model_No, Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @DataID, @Model_No, @Create_Who, GETDATE()");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 建立樣品關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_RelBySample(RelSampleID instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" IF (SELECT COUNT(*) FROM Sample_Rel_ID WHERE (SP_ID = @DataID) AND (Rel_ID = @Rel_ID)) = 0");
                sql.AppendLine(" INSERT INTO Sample_Rel_ID( ");
                sql.AppendLine("  SP_ID, Rel_ID, Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @DataID, @Rel_ID, @Create_Who, GETDATE()");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Rel_ID", instance.Rel_ID);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 建立檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Attachment(List<SampleFiles> instance)
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
                    sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(AttachID), 0) + 1 FROM Sample_Attachment);");
                    sql.AppendLine(" INSERT INTO Sample_Attachment( ");
                    sql.AppendLine("  AttachID, SP_ID, AttachFile, AttachFile_Name, Create_Who, Create_Time");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @NewID, @DataID, @AttachFile_{0}, @AttachFile_Name_{0}, @Create_Who, GETDATE()".FormatThis(row));
                    sql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AttachFile_{0}".FormatThis(row), instance[row].AttachFile);
                    cmd.Parameters.AddWithValue("AttachFile_Name_{0}".FormatThis(row), instance[row].AttachFile_Name);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance[0].SP_ID);
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
        public bool Update(ProdSample instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Sample_List SET ");
                sql.AppendLine("  Assign_Who = @Assign_Who, Model_No = @Model_No");
                sql.AppendLine("  , Sup_Corp = @Sup_Corp, Sup_ErpID = @Sup_ErpID, Cust_ModelNo = @Cust_ModelNo, Cust_Newguy = @Cust_Newguy, Qty = @Qty");
                sql.AppendLine("  , Cls_Source = @Cls_Source, Cls_Check = @Cls_Check");
                sql.AppendLine("  , Date_Come = @Date_Come, Date_Est = @Date_Est, Date_Actual = @Date_Actual");
                sql.AppendLine("  , Description1 = @Description1, Description2 = @Description2, Remark = @Remark");
                sql.AppendLine("  , Description3 = @Description3, Description4 = @Description4, Description5 = @Description5");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (SP_ID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Assign_Who", instance.Assign_Who);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);     //Nullable
                cmd.Parameters.AddWithValue("Sup_Corp", instance.Sup_Corp);       //Nullable
                cmd.Parameters.AddWithValue("Sup_ErpID", instance.Sup_ErpID);       //Nullable
                cmd.Parameters.AddWithValue("Cust_ModelNo", instance.Cust_ModelNo);     //Nullable
                cmd.Parameters.AddWithValue("Cust_Newguy", instance.Cust_Newguy);       //Nullable
                cmd.Parameters.AddWithValue("Qty", instance.Qty);
                cmd.Parameters.AddWithValue("Cls_Source", instance.Cls_Source);
                cmd.Parameters.AddWithValue("Cls_Check", instance.Cls_Check);
                cmd.Parameters.AddWithValue("Date_Come", string.IsNullOrEmpty(instance.Date_Come) ? DBNull.Value : (object)instance.Date_Come.ToDateString("yyyy-MM-dd"));  //Nullable
                cmd.Parameters.AddWithValue("Date_Est", string.IsNullOrEmpty(instance.Date_Est) ? DBNull.Value : (object)instance.Date_Est.ToDateString("yyyy-MM-dd"));     //Nullable
                cmd.Parameters.AddWithValue("Date_Actual", string.IsNullOrEmpty(instance.Date_Actual) ? DBNull.Value : (object)instance.Date_Actual.ToDateString("yyyy-MM-dd"));    //Nullable
                cmd.Parameters.AddWithValue("Description1", instance.Description1); //Nullable
                cmd.Parameters.AddWithValue("Description2", instance.Description2); //Nullable
                cmd.Parameters.AddWithValue("Description3", instance.Description3);
                cmd.Parameters.AddWithValue("Description4", instance.Description4);
                cmd.Parameters.AddWithValue("Description5", instance.Description5);
                cmd.Parameters.AddWithValue("Remark", instance.Remark);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// 更新狀態
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Status(ProdSample instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Sample_List SET ");
                sql.AppendLine("  Cls_Status = @Cls_Status");
                sql.AppendLine(" WHERE (SP_ID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Cls_Status", instance.Cls_Status);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// 刪除所有資料
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public bool Delete(string dataID)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Sample_Rel_ID WHERE (SP_ID = @DataID);");
                sql.AppendLine(" DELETE FROM Sample_Rel_ModelNo WHERE (SP_ID = @DataID);");
                sql.AppendLine(" DELETE FROM Sample_Attachment WHERE (SP_ID = @DataID);");
                sql.AppendLine(" DELETE FROM Sample_List WHERE (SP_ID = @DataID);");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 刪除檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_SampleFiles(SampleFiles instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Sample_Attachment");
                sql.AppendLine(" WHERE (SP_ID = @DataID) AND (AttachID = @AttachID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("AttachID", instance.AttachID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 刪除品號關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_RelByModelNo(RelModelNo instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Sample_Rel_ModelNo");
                sql.AppendLine(" WHERE (SP_ID = @DataID) AND (Model_No = @Model_No)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        /// <summary>
        /// 刪除樣品關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_RelBySample(RelSampleID instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Sample_Rel_ID");
                sql.AppendLine(" WHERE (SP_ID = @DataID) AND (Rel_ID = @Rel_ID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
                cmd.Parameters.AddWithValue("Rel_ID", instance.Rel_ID);

                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }
        }

        #endregion


        #region -- 取得原始資料 --

        /// <summary>
        /// 取得類別資料
        /// </summary>
        /// <param name="cls">類別參數</param>
        /// <returns></returns>
        private DataTable LookupRawData_Status(myClass cls)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Class_ID AS ID, Class_Name AS Label");
                sql.AppendLine(" FROM Sample_Class WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Class_Type = @Class) AND (Display = 'Y')");
                sql.AppendLine(" ORDER BY Sort");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Class", cls);


                //----- 回傳資料 -----
                return dbConClass.LookupDT(cmd, out ErrMsg);
            }
        }

        #endregion

    }
}