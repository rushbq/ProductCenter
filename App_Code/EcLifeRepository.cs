using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using PKLib_Method.Methods;
using EcLifeData.Models;

namespace EcLifeData.Controllers
{
    /// <summary>
    /// 查詢參數
    /// </summary>
    public enum mySearch : int
    {
        DataID = 1,
        Keyword = 2
    }

    public class EcLifeRepository
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
        public IQueryable<Product> GetDataList()
        {
            return GetDataList(null);
        }


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="search">查詢參數</param>
        /// <returns></returns>
        public IQueryable<Product> GetDataList(Dictionary<int, string> search)
        {
            //----- 宣告 -----
            List<Product> dataList = new List<Product>();
            StringBuilder sql = new StringBuilder();

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine("SELECT Base.*");
                sql.AppendLine("   , Prod.BarCode");

                //商品特色, 商品介紹(Video<pkweb>+特性+應用), 商品規格, 包裝附件(產品中心)
                sql.AppendLine("   , ISNULL(Info.Info5, '') AS Desc_Classics, Info.Info2 AS Desc_Feature, Info.Info4 AS Desc_Standards, Info.Info3 AS Append_Feature");
                sql.AppendLine("   , Prod.Pub_Individual_Packing_zh_TW AS Desc_Introduce");

                //ERP價格(市價Price_Sale = MB055, 網路價Price_Spical = MB047=>0)
                sql.AppendLine("   , 1 AS Price_Cost"); //成本不提供(預設給 1,良興才能處理)
                sql.AppendLine("   , ISNULL(INVMB.MB055, 0) Price_Sale, 1 AS Price_Spical"); //網路價固定為1 #15-20200923-0002

                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Create_Who)) AS Create_Name ");
                sql.AppendLine("   , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.Update_Who)) AS Update_Name ");
                sql.AppendLine(" FROM EcLife_Product Base");
                sql.AppendLine("  LEFT JOIN Prod_Item Prod ON Base.ModelNo = Prod.Model_No");
                sql.AppendLine("  LEFT JOIN Prod_Info Info ON Base.ModelNo = Info.Model_No AND UPPER(Info.Lang) = 'ZH-TW'");

                //關聯ERP (prokit2)
                sql.AppendLine("  LEFT JOIN prokit2.dbo.INVMB WITH(NOLOCK) ON RTRIM(INVMB.MB001) COLLATE CHINESE_TAIWAN_STROKE_CI_AS = Base.ModelNo");
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
                                    sql.Append(" AND (UPPER(Base.ModelNo) = UPPER(@DataID))");

                                    cmd.Parameters.AddWithValue("DataID", item.Value);
                                }

                                break;

                            case (int)mySearch.Keyword:
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    sql.Append(" AND (");
                                    sql.Append("    (UPPER(Base.ModelNo) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.ProductName) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.SPName) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.MType) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append("    OR (UPPER(Base.ProductMemo) LIKE '%' + UPPER(@Keyword) + '%')");
                                    sql.Append(" )");

                                    cmd.Parameters.AddWithValue("Keyword", item.Value);
                                }

                                break;

                        }
                    }
                }
                #endregion

                sql.AppendLine(" ORDER BY ModelNo");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();

                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Product
                        {
                            ModelNo = item.Field<string>("ModelNo"),
                            HouseNo = item.Field<string>("HouseNo"),
                            HouseName = item.Field<string>("HouseName"),
                            LargeNo = item.Field<string>("LargeNo"),
                            LargeName = item.Field<string>("LargeName"),
                            MediumNo = item.Field<string>("MediumNo"),
                            MediumName = item.Field<string>("MediumName"),
                            ProductName = item.Field<string>("ProductName"),
                            SPName = item.Field<string>("SPName"),
                            MType = item.Field<string>("MType"),
                            ProductMemo = item.Field<string>("ProductMemo"),

                            //商品特色, 商品介紹, 商品規格, 包裝附件(產品中心)
                            Desc_Classics = item.Field<string>("Desc_Classics"),
                            Desc_Feature = item.Field<string>("Append_Feature") + item.Field<string>("Desc_Feature"),
                            Desc_Standards = item.Field<string>("Desc_Standards"),
                            Desc_Introduce = string.IsNullOrEmpty(item.Field<string>("Desc_Introduce")) ? "-" : item.Field<string>("Desc_Introduce"),

                            Desc_Services = item.Field<string>("Desc_Services"),
                            PicUrl = item.Field<string>("PicUrl"),

                            MaxBuy = item.Field<int>("MaxBuy"),
                            MaxBuyTot = item.Field<int>("MaxBuyTot"),

                            Price_Cost = 1,
                            Price_Sale = Math.Round(item.Field<Decimal>("Price_Sale"), 0),
                            //Price_Spical = Math.Round(item.Field<Decimal>("Price_Spical"), 0),
                            Price_Spical = 1,

                            Create_Time = item.Field<DateTime?>("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Update_Time = item.Field<DateTime?>("Update_Time").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            Create_Who = item.Field<string>("Create_Who"),
                            Update_Who = item.Field<string>("Update_Who"),
                            Create_Name = item.Field<string>("Create_Name"),
                            Update_Name = item.Field<string>("Update_Name"),

                            SyncStatus = item.Field<string>("SyncStatus"),
                            IsSync = item.Field<string>("IsSync"),

                            BarCode = item.Field<string>("BarCode") //Prod_Item.BarCode

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
        /// 取得產品關鍵字
        /// </summary>
        /// <param name="modelNo"></param>
        /// <returns></returns>
        public IQueryable<Tags> GetTags(string modelNo)
        {
            //----- 宣告 -----
            List<Tags> dataList = new List<Tags>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Tags.Tag_ID, Tags.Tag_Name, Rel.Model_No");
                sql.AppendLine(" FROM Prod_Tags Tags WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN Prod_Rel_Tags Rel WITH(NOLOCK) ON Tags.Tag_ID = Rel.Tag_ID");
                sql.AppendLine(" WHERE (UPPER(Rel.Model_No) = UPPER(@ModelNo))");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKWeb, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Tags
                        {
                            ModelNo = item.Field<string>("Model_No"),
                            TagID = item.Field<int>("Tag_ID"),
                            TagName = item.Field<string>("Tag_Name")
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
        /// 取得Log
        /// </summary>
        /// <param name="modelNo"></param>
        /// <returns></returns>
        public IQueryable<Log> GetLog(string modelNo, string type)
        {
            //----- 宣告 -----
            List<Log> dataList = new List<Log>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.LogName, Base.LogValue, Base.LogTime");
                sql.AppendLine("  , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WHERE (Guid = Base.LogWho)) AS LogWho");
                sql.AppendLine(" FROM EcLife_Log Base WITH(NOLOCK)");
                sql.AppendLine(" WHERE (UPPER(Base.ModelNo) = UPPER(@ModelNo)) AND (Base.LogType = @LogType)");
                sql.AppendLine(" ORDER BY Base.LogTime DESC");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", modelNo);
                cmd.Parameters.AddWithValue("LogType", type);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new Log
                        {
                            LogName = item.Field<string>("LogName"),
                            LogValue = item.Field<string>("LogValue"),
                            LogTime = item.Field<DateTime?>("LogTime").ToString().ToDateString("yyyy/MM/dd HH:mm"),
                            LogWho = item.Field<string>("LogWho")
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
        /// 取得API值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetApiValue(string type)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT DataValue");
                sql.AppendLine(" FROM EcLife_ApiData WITH(NOLOCK)");
                sql.AppendLine(" WHERE (UPPER(DataType) = UPPER(@DataType))");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataType", type);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return DT.Rows[0]["DataValue"].ToString();
                    }
                }

            }
        }


        /// <summary>
        /// 取得官網影片連結字串
        /// </summary>
        /// <param name="modelNo">品號</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public string GetPKWeb_Video(string modelNo, string lang)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Base.PV_Uri AS Url");
                sql.AppendLine(" FROM PV_Group GP WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN PV_Group_Rel_ModelNo Rel WITH(NOLOCK) ON GP.Group_ID = Rel.Group_ID");
                sql.AppendLine("    INNER JOIN PV Base WITH(NOLOCK) ON GP.Group_ID = Base.Group_ID");
                sql.AppendLine(" WHERE (Base.LangCode = @lang) AND (Rel.Model_No = @modelNo)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("modelNo", modelNo);
                cmd.Parameters.AddWithValue("lang", lang);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKWeb, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return "<iframe src=\"{0}\" frameborder=\"0\" allowfullscreen></iframe>".FormatThis(
                                DT.Rows[0]["Url"].ToString()
                            );
                        //return "<iframe allowfullscreen=\"\" frameborder=\"0\" height=\"434\" scr=\"{0}\" width=\"772\"></iframe>".FormatThis(
                        //    DT.Rows[0]["Url"].ToString()
                        //    );
                       
                    }
                }

            }
        }
        #endregion


        #region -----// Create //-----

        /// <summary>
        /// 建立基本資料
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create(Product instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" INSERT INTO EcLife_Product( ");
                sql.AppendLine("  ModelNo, ProductName, PicUrl");
                sql.AppendLine("  , SyncStatus, IsSync");
                sql.AppendLine("  , Create_Who, Create_Time");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT RTRIM(Model_No) AS ModelNo ");
                sql.AppendLine("  , ('Pro&#39;sKit寶工' + Model_Name_zh_TW) AS ModelName");
                sql.AppendLine("  , @PicUrl");
                sql.AppendLine("  , @SyncStatus, 'N'");
                sql.AppendLine("  , @Create_Who, GETDATE()");
                sql.AppendLine(" FROM Prod_Item");
                sql.AppendLine(" WHERE (UPPER(RTRIM(Model_No)) = @ModelNo)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", instance.ModelNo);
                cmd.Parameters.AddWithValue("PicUrl", instance.PicUrl);
                cmd.Parameters.AddWithValue("SyncStatus", instance.SyncStatus);
                cmd.Parameters.AddWithValue("Create_Who", instance.Create_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// 建立TAG關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Tags(Tags instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                if (instance.TagID.Equals(0))
                {
                    //判斷名稱是否已存在
                    sql.AppendLine("IF NOT EXISTS (SELECT * FROM Prod_Tags WHERE (Tag_Name = @Tag_Name))");
                    sql.AppendLine(" BEGIN ");

                    //新增Tag
                    sql.AppendLine(" DECLARE @New_ID AS INT ");
                    sql.AppendLine(" SET @New_ID = (SELECT ISNULL(MAX(Tag_ID), 0) + 1 FROM Prod_Tags) ");
                    sql.AppendLine(" INSERT INTO Prod_Tags( ");
                    sql.AppendLine("  Tag_ID, Tag_Name");
                    sql.AppendLine(" ) VALUES ( ");
                    sql.AppendLine("  @New_ID, @Tag_Name");
                    sql.AppendLine(" ); ");

                    //新增關聯
                    sql.AppendLine(" INSERT INTO Prod_Rel_Tags( ");
                    sql.AppendLine("  Tag_ID, Model_No");
                    sql.AppendLine(" ) VALUES ( ");
                    sql.AppendLine("  @New_ID, @ModelNo");
                    sql.AppendLine(" ); ");

                    sql.AppendLine(" END ");

                    cmd.Parameters.AddWithValue("Tag_Name", instance.TagName);
                }
                else
                {
                    //判斷ID是否已存在
                    sql.AppendLine("IF NOT EXISTS (SELECT * FROM Prod_Rel_Tags WHERE (Tag_ID = @Tag_ID) AND (Model_No = @ModelNo))");
                    sql.AppendLine(" BEGIN ");

                    //新增其他Tag關聯
                    sql.AppendLine(" INSERT INTO Prod_Rel_Tags( ");
                    sql.AppendLine("  Tag_ID, Model_No");
                    sql.AppendLine(" ) VALUES ( ");
                    sql.AppendLine("  @Tag_ID, @ModelNo");
                    sql.AppendLine(" ); ");

                    sql.AppendLine(" END ");

                    cmd.Parameters.AddWithValue("Tag_ID", instance.TagID);
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", instance.ModelNo);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.PKWeb, out ErrMsg);
            }
        }

        /// <summary>
        /// 建立LOG
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Create_Log(Log instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DECLARE @NewID AS VARCHAR(38)");
                sql.AppendLine(" SET @NewID = (SELECT CONVERT(VARCHAR(38), NEWID()))");
                sql.AppendLine(" INSERT INTO EcLife_Log( ");
                sql.AppendLine("  LogID, ModelNo, LogType, LogName, LogValue, LogWho, LogTime");
                sql.AppendLine(" ) VALUES (");
                sql.AppendLine("  @NewID, @ModelNo, @LogType, @LogName, @LogValue, @LogWho, GETDATE()");
                sql.AppendLine(" )");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ModelNo", instance.ModelNo);
                cmd.Parameters.AddWithValue("LogType", instance.LogType);
                cmd.Parameters.AddWithValue("LogName", instance.LogName);
                cmd.Parameters.AddWithValue("LogValue", instance.LogValue);
                cmd.Parameters.AddWithValue("LogWho", instance.LogWho);

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
        public bool Update(Product instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE EcLife_Product SET ");
                sql.AppendLine("  ProductName = @ProductName, SPName = @SPName, MType = @MType, ProductMemo = @ProductMemo");

                #region >> 分類欄位判斷 <<

                if (!string.IsNullOrEmpty(instance.HouseNo))
                {
                    sql.AppendLine(", HouseNo = @HouseNo, HouseName = @HouseName");

                    cmd.Parameters.AddWithValue("HouseNo", instance.HouseNo);
                    cmd.Parameters.AddWithValue("HouseName", instance.HouseName);
                }

                if (!string.IsNullOrEmpty(instance.LargeNo))
                {
                    sql.AppendLine(", LargeNo = @LargeNo, LargeName = @LargeName");

                    cmd.Parameters.AddWithValue("LargeNo", instance.LargeNo);
                    cmd.Parameters.AddWithValue("LargeName", instance.LargeName);
                }

                if (!string.IsNullOrEmpty(instance.MediumNo))
                {
                    sql.AppendLine(", MediumNo = @MediumNo, MediumName = @MediumName");

                    cmd.Parameters.AddWithValue("MediumNo", instance.MediumNo);
                    cmd.Parameters.AddWithValue("MediumName", instance.MediumName);
                }

                #endregion

                if (!string.IsNullOrEmpty(instance.PicUrl))
                {
                    sql.AppendLine(", PicUrl = @PicUrl");

                    cmd.Parameters.AddWithValue("PicUrl", instance.PicUrl);
                }

                sql.AppendLine("  , Desc_Services = @Desc_Services");
                sql.AppendLine("  , MaxBuy = @MaxBuy, MaxBuyTot = @MaxBuyTot");
                sql.AppendLine("  , Update_Who = @Update_Who, Update_Time = GETDATE()");
                sql.AppendLine("  , SyncStatus = @SyncStatus, IsSync = 'N'");
                sql.AppendLine(" WHERE (ModelNo = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.ModelNo);
                cmd.Parameters.AddWithValue("ProductName", instance.ProductName);
                cmd.Parameters.AddWithValue("SPName", instance.SPName);
                cmd.Parameters.AddWithValue("MType", instance.MType);
                cmd.Parameters.AddWithValue("ProductMemo", instance.ProductMemo);

                cmd.Parameters.AddWithValue("Desc_Services", instance.Desc_Services);

                cmd.Parameters.AddWithValue("MaxBuy", instance.MaxBuy);
                cmd.Parameters.AddWithValue("MaxBuyTot", instance.MaxBuyTot);
                cmd.Parameters.AddWithValue("SyncStatus", instance.SyncStatus);

                cmd.Parameters.AddWithValue("Update_Who", instance.Update_Who);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        /// <summary>
        /// 更新狀態
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Update_Status(Product instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE EcLife_Product SET ");
                sql.AppendLine("  SyncStatus = @SyncStatus, IsSync = @IsSync");
                sql.AppendLine(" WHERE (ModelNo = @DataID)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.ModelNo);
                cmd.Parameters.AddWithValue("SyncStatus", instance.SyncStatus);
                cmd.Parameters.AddWithValue("IsSync", instance.IsSync);


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }


        /// <summary>
        /// 更新API參考值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update_ApiData(string type, string value)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" UPDATE EcLife_ApiData SET DataValue = @Value");
                sql.AppendLine(" WHERE (DataType = @DataType)");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataType", type);
                cmd.Parameters.AddWithValue("Value", value.Trim());


                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }

        #endregion


        #region -----// Delete //-----

        /// <summary>
        /// 刪除品號關聯
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Delete_Tag(Tags instance)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" DELETE FROM Prod_Rel_Tags");
                sql.AppendLine(" WHERE (Tag_ID = @DataID) AND (Model_No = @Model_No)");

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("DataID", instance.TagID);
                cmd.Parameters.AddWithValue("Model_No", instance.ModelNo);

                return dbConClass.ExecuteSql(cmd, dbConClass.DBS.PKWeb, out ErrMsg);
            }
        }


        #endregion

    }
}