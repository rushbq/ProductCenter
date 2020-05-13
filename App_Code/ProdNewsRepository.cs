using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Method.Methods;
using ProdNewsData.Models;


namespace ProdNewsData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2,
        Status = 3,
        UserID = 4
    }


    /// <summary>
    /// 產品中心-產品訊息
    /// </summary>
    public class ProdNewsRepository
    {
        public string ErrMsg;


        #region -----// Read //-----

        /// <summary>
        /// 取得所有資料(傳入預設參數)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 預設值為(null)
        /// </remarks>
        public IQueryable<Items> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Items> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Items> dataList = new List<Items>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT Base.NewsID, Cls.ClassID, Cls.ClassName_zh_TW AS ClassName, Base.Lang");
                sql.AppendLine(" , Base.Subject, Base.TimingType, Base.TimingDate, Base.TimingDesc");
                sql.AppendLine(" , Base.Desc1, Base.Desc2, Base.IsMail, Base.IsClose");
                sql.AppendLine(" , ISNULL(Base.BPM_Sno, '') BPM_Sno, ISNULL(Base.BPM_Oid, '') BPM_Oid, ISNULL(Base.BPM_FormNo, '') BPM_FormNo");
                sql.AppendLine(" , Base.Create_Time, Base.Update_Time, Base.Send_Time");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE (Account_Name = Base.Create_Who)) AS Create_Name");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE (Account_Name = Base.Update_Who)) AS Update_Name");
                sql.AppendLine(" , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE (Account_Name = Base.Send_Who)) AS Send_Name");
                //BPM WorkItemID
                sql.AppendLine(" , (CASE WHEN BPM_Sno IS NOT NULL THEN");
                sql.AppendLine("     ISNULL((");
                sql.AppendLine("      SELECT TOP 1 WorkItem.OID AS WorkItemID");
                sql.AppendLine("	  FROM EFGP.dbo.ProcessInstance");
                sql.AppendLine("	  INNER JOIN EFGP.dbo.WorkItem ON ProcessInstance.contextOID = WorkItem.contextOID");
                sql.AppendLine("	  LEFT JOIN EFGP.dbo.WorkAssignment ON WorkItem.OID = WorkAssignment.workItemOID");
                sql.AppendLine("	  LEFT JOIN EFGP.dbo.Users WU ON WorkItem.performerOID = WU.OID");
                sql.AppendLine("	  LEFT JOIN EFGP.dbo.Users AU ON WorkAssignment.assigneeOID = AU.OID");
                sql.AppendLine("	  WHERE ProcessInstance.serialNumber COLLATE Chinese_Taiwan_Stroke_BIN = Base.BPM_Sno");
                sql.AppendLine("	   AND (WU.id = @UserID OR AU.id = @UserID)");
                sql.AppendLine("	  ORDER BY WorkItem.createdTime DESC");
                sql.AppendLine("     ),'')");
                sql.AppendLine("   ELSE '' END");
                sql.AppendLine(" ) AS BPM_WorkItemID");
                sql.AppendLine(" FROM Prod_News Base");
                sql.AppendLine("  INNER JOIN Prod_News_Class Cls ON Base.ClassID = Cls.ClassID");
                sql.AppendLine(" WHERE (1=1) ");

                /* Search */
                #region >> filter <<

                if (search != null)
                {
                    foreach (var item in search)
                    {
                        switch (item.Key)
                        {
                            case (int)mySearch.DataID:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.NewsID = @DataID)");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("  (UPPER(Base.Subject) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.BPM_Sno) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (UPPER(Base.BPM_FormNo) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("  OR (Base.NewsID IN (");
                                    sql.Append("    SELECT NewsID FROM Prod_News_Models ");
                                    sql.Append("    WHERE (UPPER(Model_No) LIKE '%' + UPPER(@Keyword) + '%') ");
                                    sql.Append("  ))");
                                    sql.Append("  OR (Base.NewsID IN (");
                                    sql.Append("    SELECT NewsID FROM Prod_News_SubModels ");
                                    sql.Append("    WHERE (UPPER(Model_No) LIKE '%' + UPPER(@Keyword) + '%') ");
                                    sql.Append("  ))");
                                    sql.Append(" )");


                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                            case (int)mySearch.Status:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (Base.ClassID = @status)");

                                    cmd.Parameters.AddWithValue("status", item.Value);
                                }

                                break;


                            case (int)mySearch.UserID:
                                //使用者工號(用來查詢WorkItemID)
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    cmd.Parameters.AddWithValue("UserID", item.Value);
                                }

                                break;
                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY Base.NewsID DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();

                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Items
                        {
                            NewsID = item.Field<Int32>("NewsID"),
                            ClassID = item.Field<Int32>("ClassID"),
                            ClassName = item.Field<string>("ClassName"),
                            Lang = item.Field<string>("Lang"),
                            Subject = item.Field<string>("Subject"),
                            TimingType = item.Field<Int16?>("TimingType"),
                            TimingDate = item.Field<DateTime?>("TimingDate").ToString().ToDateString("yyyy/MM/dd"),
                            TimingDesc = item.Field<string>("TimingDesc"),
                            Desc1 = item.Field<string>("Desc1"),
                            Desc2 = item.Field<string>("Desc2"),
                            IsMail = item.Field<string>("IsMail"),
                            IsClose = item.Field<string>("IsClose"),
                            BPM_Sno = item.Field<string>("BPM_Sno"),
                            BPM_Oid = item.Field<string>("BPM_Oid"),
                            BPM_WorkItemID = item.Field<string>("BPM_WorkItemID"),
                            BPM_FormNo = item.Field<string>("BPM_FormNo"),                            

                            Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Send_Time = item.Field<DateTime?>("Send_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),

                            Create_Name = item.Field<string>("Create_Name"),
                            Update_Name = item.Field<string>("Update_Name"),
                            Send_Name = item.Field<string>("Send_Name")

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
        /// 取得發送對象
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<RelTarget> GetRelTarget(string dataID)
        {
            //----- 宣告 -----
            List<RelTarget> dataList = new List<RelTarget>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT TargetID");
                sql.AppendLine(" FROM Prod_News_Target");
                sql.AppendLine(" WHERE (NewsID = @DataID) ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RelTarget
                        {
                            TargetID = item.Field<string>("TargetID")
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
        /// 取得關聯品號
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<RelModelNo> GetRelModelList(string dataID)
        {
            //----- 宣告 -----
            List<RelModelNo> dataList = new List<RelModelNo>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT NewsID, Model_No");
                sql.AppendLine(" FROM Prod_News_Models");
                sql.AppendLine(" WHERE (NewsID = @DataID) ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RelModelNo
                        {
                            NewsID = item.Field<int>("NewsID"),
                            Model_No = item.Field<string>("Model_No")
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
        /// 取得關聯替代品號
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public IQueryable<RelModelNo> GetRelSubModelList(string dataID)
        {
            //----- 宣告 -----
            List<RelModelNo> dataList = new List<RelModelNo>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT NewsID, Model_No");
                sql.AppendLine(" FROM Prod_News_SubModels");
                sql.AppendLine(" WHERE (NewsID = @DataID) ");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);

                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new RelModelNo
                        {
                            NewsID = item.Field<int>("NewsID"),
                            Model_No = item.Field<string>("Model_No")
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
        /// 取得附件檔案
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IQueryable<AttachFiles> GetFileList(string dataID, string type)
        {
            //----- 宣告 -----
            List<AttachFiles> dataList = new List<AttachFiles>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT NewsID, AttID, AttType, AttOrgName, AttName, AttDesc, Create_Who, Create_Time");
                sql.AppendLine(" FROM Prod_News_Attachment WITH(NOLOCK)");
                sql.AppendLine(" WHERE (NewsID = @DataID) AND (AttType = @type)");
                sql.AppendLine(" ORDER BY Create_Time");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", dataID);
                cmd.Parameters.AddWithValue("type", type);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new AttachFiles
                        {
                            NewsID = item.Field<Int32>("NewsID"),
                            AttID = item.Field<Int32>("AttID"),
                            AttachFile = item.Field<string>("AttName"),
                            AttachFile_Name = item.Field<string>("AttOrgName"),
                            AttDesc = item.Field<string>("AttDesc"),
                            Create_Who = item.Field<string>("Create_Who").ToString(),
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
        public Int32 Create(Items instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //[SQL] - 取得最新編號
                Int32 New_ID;
                sql.AppendLine(" SELECT (ISNULL(MAX(NewsID), 0) + 1) AS New_ID FROM Prod_News ");
                cmd.CommandText = sql.ToString();
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    New_ID = Convert.ToInt32(DT.Rows[0]["New_ID"]);
                }

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();
                sql.Clear();


                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO Prod_News( ");
                sql.AppendLine("  NewsID, ClassID, Lang, Subject");
                sql.AppendLine("  , TimingType, TimingDate, TimingDesc");
                sql.AppendLine("  , Desc1, Desc2, IsMail, IsClose");
                sql.AppendLine("  , BPM_Sno, BPM_Oid, BPM_FormNo");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @DataID, @ClassID, @Lang, @Subject");
                sql.AppendLine("  , @TimingType, @TimingDate, @TimingDesc");
                sql.AppendLine("  , @Desc1, @Desc2, 'N', 'N'");
                sql.AppendLine("  , @BPM_Sno, @BPM_Oid, @BPM_FormNo");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" );");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", New_ID);
                cmd.Parameters.AddWithValue("ClassID", instance.ClassID);
                cmd.Parameters.AddWithValue("Lang", instance.Lang);
                cmd.Parameters.AddWithValue("Subject", instance.Subject);
                cmd.Parameters.AddWithValue("TimingType", instance.TimingType);
                cmd.Parameters.AddWithValue("TimingDate", string.IsNullOrEmpty(instance.TimingDate) ? DBNull.Value : (object)instance.TimingDate.ToDateString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("TimingDesc", instance.TimingDesc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Desc1", instance.Desc1 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Desc2", instance.Desc2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("BPM_Sno", instance.BPM_Sno ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("BPM_Oid", instance.BPM_Oid ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("BPM_FormNo", instance.BPM_FormNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);


                if (!dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg))
                {
                    return 0;
                }
                else
                {
                    return New_ID;
                }
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
                sql.AppendLine(" IF (SELECT COUNT(*) FROM Prod_News_Models WHERE (NewsID = @DataID) AND (Model_No = @Model_No)) = 0");
                sql.AppendLine(" INSERT INTO Prod_News_Models( ");
                sql.AppendLine("  NewsID, Model_No");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @DataID, @Model_No");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.NewsID);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }
        }


        /// <summary>
        /// 建立替代品號關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_RelBySubModelNo(RelModelNo instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" IF (SELECT COUNT(*) FROM Prod_News_SubModels WHERE (NewsID = @DataID) AND (Model_No = @Model_No)) = 0");
                sql.AppendLine(" INSERT INTO Prod_News_SubModels( ");
                sql.AppendLine("  NewsID, Model_No");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @DataID, @Model_No");
                sql.AppendLine(" )");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.NewsID);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }
        }


        /// <summary>
        /// 建立檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Attachment(List<AttachFiles> instance)
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
                    sql.AppendLine(" SET @NewID = (SELECT ISNULL(MAX(AttID), 0) + 1 FROM Prod_News_Attachment);");
                    sql.AppendLine(" INSERT INTO Prod_News_Attachment( ");
                    sql.AppendLine("  AttID, NewsID, AttType, AttName, AttOrgName, AttDesc");
                    sql.AppendLine(" , Create_Who, Create_Time");
                    sql.AppendLine(" ) VALUES (");
                    sql.AppendLine("  @NewID, @DataID, @AttType, @AttachFile_{0}, @AttachFile_Name_{0}, @AttDesc_{0}".FormatThis(row));
                    sql.AppendLine(" , @Create_Who, GETDATE()");
                    sql.AppendLine(" );");

                    cmd.Parameters.AddWithValue("AttachFile_{0}".FormatThis(row), instance[row].AttachFile);
                    cmd.Parameters.AddWithValue("AttachFile_Name_{0}".FormatThis(row), instance[row].AttachFile_Name);
                    cmd.Parameters.AddWithValue("AttDesc_{0}".FormatThis(row), instance[row].AttDesc);
                }

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance[0].NewsID);
                cmd.Parameters.AddWithValue("AttType", instance[0].AttType);
                cmd.Parameters.AddWithValue("Create_Who", instance[0].Create_Who);


                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }
        }

        #endregion


        #region -----// Update //-----

        /// <summary>
        /// 更新基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update(Items instance, out string ErrMsg)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_News SET ");
                sql.AppendLine("  ClassID = @ClassID, Lang = @Lang, Subject = @Subject");
                sql.AppendLine("  , TimingType = @TimingType, TimingDate = @TimingDate, TimingDesc = @TimingDesc");
                sql.AppendLine("  , Desc1 = @Desc1, Desc2 = @Desc2");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine(" WHERE (NewsID = @DataID);");

                //[SQL] - 清除發送對象, Prod_News_Target
                sql.AppendLine(" DELETE FROM Prod_News_Target WHERE (NewsID = @DataID);");

                int idx = 0;
                foreach (var val in instance.SendTarget)
                {
                    idx++;
                    sql.AppendLine(" INSERT INTO Prod_News_Target(NewsID, TargetID) ");
                    sql.AppendLine(" VALUES (@DataID, @TargetID_{0});".FormatThis(idx));

                    cmd.Parameters.AddWithValue("TargetID_" + idx, val);
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.NewsID);
                cmd.Parameters.AddWithValue("ClassID", instance.ClassID);
                cmd.Parameters.AddWithValue("Lang", instance.Lang);
                cmd.Parameters.AddWithValue("Subject", instance.Subject);
                cmd.Parameters.AddWithValue("TimingType", instance.TimingType);
                cmd.Parameters.AddWithValue("TimingDate", string.IsNullOrEmpty(instance.TimingDate) ? (object)DBNull.Value : instance.TimingDate);
                cmd.Parameters.AddWithValue("TimingDesc", instance.TimingDesc);
                cmd.Parameters.AddWithValue("Desc1", instance.Desc1);
                cmd.Parameters.AddWithValue("Desc2", instance.Desc2);
                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }

        }


        /// <summary>
        /// 更新檔案描述
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_FileDesc(AttachFiles instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_News_Attachment SET ");
                sql.AppendLine("  AttDesc = @AttDesc");
                sql.AppendLine(" WHERE (AttID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.AttID);
                cmd.Parameters.AddWithValue("AttDesc", instance.AttDesc);


                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }

        }



        /// <summary>
        /// 更新狀態
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Status(Items instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE Prod_News SET ");
                sql.AppendLine(" IsMail = @IsMail, IsClose = @IsClose");
                sql.AppendLine(" WHERE (NewsID = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.NewsID);
                cmd.Parameters.AddWithValue("IsMail", instance.IsMail);
                cmd.Parameters.AddWithValue("IsClose", instance.IsClose);


                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }

        }
        #endregion


        #region -----// Delete //-----

        ///// <summary>
        ///// 刪除所有資料
        ///// </summary>
        ///// <param name="dataID"></param>
        ///// <returns></returns>
        //public bool Delete(string dataID)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" DELETE FROM Sample_Rel_ID WHERE (SP_ID = @DataID);");
        //        sql.AppendLine(" DELETE FROM Sample_Rel_ModelNo WHERE (SP_ID = @DataID);");
        //        sql.AppendLine(" DELETE FROM Sample_Attachment WHERE (SP_ID = @DataID);");
        //        sql.AppendLine(" DELETE FROM Sample_List WHERE (SP_ID = @DataID);");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", dataID);

        //        return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
        //    }
        //}

        ///// <summary>
        ///// 刪除檔案資料
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <returns></returns>
        //public bool Delete_SampleFiles(SampleFiles instance)
        //{
        //    //----- 宣告 -----
        //    StringBuilder sql = new StringBuilder();

        //    //----- 資料查詢 -----
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        //----- SQL 查詢語法 -----
        //        sql.AppendLine(" DELETE FROM Sample_Attachment");
        //        sql.AppendLine(" WHERE (SP_ID = @DataID) AND (AttachID = @AttachID)");

        //        //----- SQL 執行 -----
        //        cmd.CommandText = sql.ToString();
        //        cmd.Parameters.AddWithValue("DataID", instance.SP_ID);
        //        cmd.Parameters.AddWithValue("AttachID", instance.AttachID);

        //        return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
        //    }
        //}


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
                sql.AppendLine(" DELETE FROM Prod_News_Models");
                sql.AppendLine(" WHERE (NewsID = @DataID) AND (Model_No = @Model_No)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.NewsID);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除替代品號關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_RelBySubModelNo(RelModelNo instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Prod_News_SubModels");
                sql.AppendLine(" WHERE (NewsID = @DataID) AND (Model_No = @Model_No)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.NewsID);
                cmd.Parameters.AddWithValue("Model_No", instance.Model_No);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }
        }


        /// <summary>
        /// 刪除檔案資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_Files(AttachFiles instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Prod_News_Attachment");
                sql.AppendLine(" WHERE (AttID = @DataID)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.AttID);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.Product, out ErrMsg);
            }
        }


        #endregion
    }
}
