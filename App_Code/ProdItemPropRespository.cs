using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using PKLib_Method.Methods;
using ProdAttributeData.Models;


namespace ProdAttributeData.Controllers
{
    public class ProdItemPropRespository
    {
        public string ErrMsg;


        #region -----// Read //-----

        /// <summary>
        /// [貨號屬性] 貨號資料清單:取得不分頁的清單
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dbs">TW/SH</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public DataTable GetOne_ProdAttr(Dictionary<string, string> search, out string ErrMsg)
        {
            int DataCnt = 0;
            return Get_ProdAttrList(search, 0, 0, false, out DataCnt, out ErrMsg);
        }

        /// <summary>
        /// [貨號屬性] 貨號資料清單
        /// </summary>
        /// <param name="search">search集合</param>
        /// <param name="dbs">TW/SH</param>
        /// <param name="startRow">StartRow(從0開始)</param>
        /// <param name="endRow">RecordsPerPage</param>
        /// <param name="doPaging">是否分頁</param>
        /// <param name="DataCnt">傳址參數(資料總筆數)</param>
        /// <param name="ErrMsg"></param>
        /// <returns>DataTable</returns>
        public DataTable Get_ProdAttrList(Dictionary<string, string> search
            , int startRow, int endRow, bool doPaging
            , out int DataCnt, out string ErrMsg)
        {
            ErrMsg = "";
            string AllErrMsg = "";

            try
            {
                /* 開始/結束筆數計算 */
                int cntStartRow = startRow + 1;
                int cntEndRow = startRow + endRow;

                //----- 宣告 -----
                StringBuilder sql = new StringBuilder(); //SQL語法容器
                List<SqlParameter> sqlParamList = new List<SqlParameter>(); //SQL參數容器
                List<SqlParameter> sqlParamList_Cnt = new List<SqlParameter>(); //SQL參數容器
                DataCnt = 0;    //資料總數

                #region >> 資料筆數SQL查詢 <<
                using (SqlCommand cmdCnt = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string mainSql = @"
                    SELECT COUNT(*) AS TotalCnt
                    FROM [PKSYS].dbo.ModelNo_Rel_ItemNo Base
                     INNER JOIN Prod_Item Prod ON Base.Model_No = Prod.Model_No
                     INNER JOIN Prod_Class Cls ON Prod.Class_ID = Cls.Class_ID
                     LEFT JOIN Prod_ItemNo_Prop Attr ON Base.Item_No = Attr.Item_No
                    WHERE (1=1)";

                    //append
                    sql.Append(mainSql);


                    #region >> 條件組合 <<

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
                                    //指定資料編號
                                    sql.Append(" AND (Attr.Data_ID = @DataID)");

                                    sqlParamList_Cnt.Add(new SqlParameter("@DataID", item.Value));

                                    break;

                                case "ModelNo":
                                    //品號
                                    sql.Append(" AND (Base.Item_No IN (");
                                    sql.Append(" SELECT Item_No");
                                    sql.Append(" FROM Prod_item");
                                    sql.Append(" WHERE (UPPER(Model_No) LIKE UPPER(@ModelNo) + '%')");
                                    sql.Append("))");

                                    sqlParamList_Cnt.Add(new SqlParameter("@ModelNo", item.Value));

                                    break;

                                case "ItemNo":
                                    //貨號
                                    sql.Append(" AND (UPPER(Base.Item_No) LIKE UPPER(@ItemNo) + '%')");
                                    sqlParamList_Cnt.Add(new SqlParameter("@ItemNo", item.Value));

                                    break;
                            }
                        }
                    }
                    #endregion


                    //----- SQL 執行 -----
                    cmdCnt.CommandText = sql.ToString();
                    cmdCnt.Parameters.Clear();


                    //----- SQL 參數陣列 -----
                    cmdCnt.Parameters.AddRange(sqlParamList_Cnt.ToArray());

                    //Execute
                    using (DataTable DTCnt = dbConClass.LookupDT(cmdCnt, out ErrMsg))
                    {
                        //資料總筆數
                        if (DTCnt.Rows.Count > 0)
                        {
                            DataCnt = Convert.ToInt32(DTCnt.Rows[0]["TotalCnt"]);
                        }
                    }
                    AllErrMsg += ErrMsg;

                    //*** 在SqlParameterCollection同個循環內不可有重複的SqlParam,必須清除才能繼續使用. ***
                    cmdCnt.Parameters.Clear();
                }
                #endregion


                #region >> 主要資料SQL查詢 <<
                sql.Clear();

                //----- 資料取得 -----
                using (SqlCommand cmd = new SqlCommand())
                {
                    //----- SQL 查詢語法 -----
                    string mainSql = @"
                    SELECT TbAll.*
                    FROM (
                        SELECT Attr.Data_ID, RTRIM(Base.Model_No) AS ModelNo
	                     , RTRIM(Base.Item_No) AS Item_No, Attr.ShipFrom, Attr.OnlineDate, Attr.StopDate 
	                     , Attr.NewProdClass, Attr.SaleClass, Attr.PurClass, Attr.WareHouseClass, Attr.CCCode
	                     , Cls1.MA003 AS SaleClassName
	                     , Cls2.MA003 AS PurClassName
	                     , Cls3.MA003 AS WareHouseClassName
	                     , Attr.Create_Time, Attr.Update_Time
	                     , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE ([Guid] = Attr.Create_Who)) AS Create_Name
	                     , (SELECT Account_Name + ' (' + Display_Name + ')' FROM PKSYS.dbo.User_Profile WITH(NOLOCK) WHERE ([Guid] = Attr.Update_Who)) AS Update_Name
	                     , ROW_NUMBER() OVER (ORDER BY Prod.Class_ID, Base.Item_No) AS RowIdx
	                    FROM [PKSYS].dbo.ModelNo_Rel_ItemNo Base
	                     INNER JOIN Prod_Item Prod ON Base.Model_No = Prod.Model_No
	                     INNER JOIN Prod_Class Cls ON Prod.Class_ID = Cls.Class_ID
	                     LEFT JOIN Prod_ItemNo_Prop Attr ON Base.Item_No = Attr.Item_No
	                     LEFT JOIN [prokit2].dbo.INVMA Cls1 ON Attr.SaleClass COLLATE Chinese_Taiwan_Stroke_BIN = Cls1.MA002
	                     LEFT JOIN [prokit2].dbo.INVMA Cls2 ON Attr.PurClass COLLATE Chinese_Taiwan_Stroke_BIN = Cls2.MA002
	                     LEFT JOIN [prokit2].dbo.INVMA Cls3 ON Attr.WareHouseClass COLLATE Chinese_Taiwan_Stroke_BIN = Cls3.MA002
                    WHERE (1=1)";

                    //append sql
                    sql.Append(mainSql);

                    #region >> 條件組合 <<

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
                                    //指定資料編號
                                    sql.Append(" AND (Attr.Data_ID = @DataID)");

                                    sqlParamList.Add(new SqlParameter("@DataID", item.Value));

                                    break;

                                case "ModelNo":
                                    //品號
                                    sql.Append(" AND (Base.Item_No IN (");
                                    sql.Append(" SELECT Item_No");
                                    sql.Append(" FROM Prod_item");
                                    sql.Append(" WHERE (UPPER(Model_No) LIKE UPPER(@ModelNo) + '%')");
                                    sql.Append("))");

                                    sqlParamList.Add(new SqlParameter("@ModelNo", item.Value));

                                    break;

                                case "ItemNo":
                                    //貨號
                                    sql.Append(" AND (UPPER(Base.Item_No) LIKE UPPER(@ItemNo) + '%')");

                                    sqlParamList.Add(new SqlParameter("@ItemNo", item.Value));

                                    break;
                            }
                        }
                    }
                    #endregion

                    //Sql尾段
                    sql.AppendLine(") AS TbAll");


                    //是否分頁
                    if (doPaging)
                    {
                        sql.AppendLine(" WHERE (TbAll.RowIdx >= @startRow) AND (TbAll.RowIdx <= @endRow)");

                        sqlParamList.Add(new SqlParameter("@startRow", cntStartRow));
                        sqlParamList.Add(new SqlParameter("@endRow", cntEndRow));

                    }
                    sql.AppendLine(" ORDER BY TbAll.RowIdx");


                    //----- SQL 執行 -----
                    cmd.CommandText = sql.ToString();
                    cmd.Parameters.Clear();

                    //----- SQL 參數陣列 -----
                    cmd.Parameters.AddRange(sqlParamList.ToArray());

                    //Execute
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //return err
                        if (!string.IsNullOrWhiteSpace(AllErrMsg)) ErrMsg = AllErrMsg;

                        return DT;
                    }

                }

                #endregion

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString() + "_Error:_" + ErrMsg);
            }
        }


        /// <summary>
        /// [貨號屬性] ERP的分類選項
        /// </summary>
        /// <param name="_type">2,3,4</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// MB006 [品號分類二][銷售]2
        /// MB007 [品號分類三][生管採購]3
        /// MB008 [品號分類四][倉管]4
        /// </remarks>
        public DataTable Get_ErpClass(string _type, out string ErrMsg)
        {
            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = "";

                //----- SQL 查詢語法 -----
                switch (_type)
                {
                    case "2":
                    case "3":
                    case "4":
                        sql = @"
                            SELECT RTRIM(MA002) AS id, RTRIM(MA003) AS label
                            FROM [prokit2].dbo.INVMA WITH(NOLOCK)
                            WHERE (MA001 = @type)
                            ORDER BY MA002";
                        break;

                    default:
                        sql = @"
                            SELECT Class_ID AS id, Class_Name AS label
                            FROM Prod_ItemNo_ParamClass WITH(NOLOCK)
                            WHERE (Class_Type = @type)
                            ORDER BY Sort";

                        break;
                }


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("type", _type);

                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    return DT;
                }

            }
        }


        /// <summary>
        /// [貨號屬性] 取得對應的品號
        /// </summary>
        /// <param name="_ItemNo">貨號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// Product/Prod_View.aspx?Model_No=103-132C
        /// </remarks>
        public DataTable Get_Models(string _ItemNo, out string ErrMsg)
        {
            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                SELECT RTRIM(Model_No) AS ModelNo
                FROM Prod_Item
                WHERE (Item_No = @Item_No)";

                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Item_No", _ItemNo);

                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    return DT;
                }

            }
        }

        #endregion



        #region -----// Create //-----
        /// <summary>
        /// [貨號屬性] 貨號資料:無編號時自動新增,並回傳編號
        /// </summary>
        /// <param name="_ItemNo">貨號</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        public string Check_ProdAttr(string _ItemNo, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //OpcsRemk_Order -> OpcsRemk_Order
                string sql = @"
                    DECLARE @DataID AS VARCHAR(38)
                    IF (SELECT COUNT(*) FROM [ProductCenter].dbo.Prod_ItemNo_Prop WHERE (Item_No = @ItemNo)) > 0
                     BEGIN
                       --Exists
                       SET @DataID = (SELECT Data_ID FROM [ProductCenter].dbo.Prod_ItemNo_Prop WHERE (Item_No = @ItemNo))
                     END

                    ELSE

                     BEGIN
                       SET @DataID = NEWID()
	                    --Insert
	                    INSERT INTO [ProductCenter].dbo.Prod_ItemNo_Prop (
	                    Data_ID
	                    , Item_No, OnlineDate, ShipFrom
	                    , SaleClass, PurClass, WareHouseClass, CCCode
	                    , StopDate, Create_Who
	                    )
	                    SELECT TOP 1 @DataID
	                    , Tbl.Item_No, Tbl.OnlineDate, Tbl.ShipFrom
	                    , Tbl.SaleClass, Tbl.PurClass, Tbl.WareHouseClass, Tbl.CCCode
	                    , ISNULL(CONVERT(VARCHAR, Prod.Stop_Offer_Date, 112), '') AS StopDate
	                    , @Who AS Who
	                    FROM (
		                    SELECT
		                    Base.Item_No
		                    , Base.Model_No
		                    , ErpTW.MB218 AS OnlineDate
		                    , ErpTW.MB219 AS ShipFrom
		                    , ErpTW.MB006 AS SaleClass
		                    , ErpTW.MB007 AS PurClass
		                    , ErpTW.MB008 AS WareHouseClass
		                    , ErpTW.MB213 AS CCCode
		                    FROM [PKSYS].dbo.ModelNo_Rel_ItemNo Base
		                    INNER JOIN [prokit2].dbo.INVMB ErpTW ON Base.Model_No COLLATE Chinese_Taiwan_Stroke_BIN = ErpTW.MB001 AND ErpTW.MB219 IN ('TW','CSH')
		                    WHERE (ErpTW.MB001 = @ItemNo)
		                    UNION ALL
		                    SELECT
		                    Base.Item_No
		                    , Base.Model_No
		                    , ErpSH.MB218 AS OnlineDate
		                    , ErpSH.MB219 AS ShipFrom
		                    , ErpSH.MB006 AS SaleClass
		                    , ErpSH.MB007 AS PurClass
		                    , ErpSH.MB008 AS WareHouseClass
		                    , ErpSH.MB213 AS CCCode
		                    FROM [PKSYS].dbo.ModelNo_Rel_ItemNo Base
		                    INNER JOIN [SHPK2].dbo.INVMB ErpSH ON Base.Model_No COLLATE Chinese_Taiwan_Stroke_BIN = ErpSH.MB001 AND ErpSH.MB219 IN ('SH')
		                    WHERE (ErpSH.MB001 = @ItemNo)
	                    ) AS Tbl
	                    LEFT JOIN [ProductCenter].dbo.Prod_Item Prod ON Tbl.Model_No = Prod.Model_No

                     END

                    SELECT @DataID AS ShowDataID";


                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("ItemNo", _ItemNo);
                cmd.Parameters.AddWithValue("Who", fn_Param.CurrentUser);

                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return DT.Rows[0]["ShowDataID"].ToString();
                    }
                }
            }
        }


        #endregion


        #region -----// Update //-----

        /// <summary>
        /// [貨號屬性] 貨號資料Update
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool Update_ProdAttr(string _id, ItemProp inst, out string ErrMsg)
        {
            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                string sql = @"
                UPDATE Prod_ItemNo_Prop
                SET OnlineDate = @OnlineDate, StopDate = @StopDate
                 , NewProdClass = @NewProdClass, SaleClass = @SaleClass, PurClass = @PurClass
                 , WareHouseClass = @WareHouseClass, CCCode = @CCCode
                 , Update_Who = @Who, Update_Time = GETDATE()
                WHERE Data_ID = @id";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("OnlineDate", inst.OnlineDate);
                cmd.Parameters.AddWithValue("StopDate", inst.StopDate);
                cmd.Parameters.AddWithValue("NewProdClass", inst.NewProdClass);
                cmd.Parameters.AddWithValue("SaleClass", inst.SaleClass);
                cmd.Parameters.AddWithValue("PurClass", inst.PurClass);
                cmd.Parameters.AddWithValue("WareHouseClass", inst.WareHouseClass);
                cmd.Parameters.AddWithValue("CCCode", inst.CCCode);
                cmd.Parameters.AddWithValue("Who", fn_Param.CurrentUser);
                cmd.Parameters.AddWithValue("id", _id);

                //execute
                return dbConClass.ExecuteSql(cmd, out ErrMsg);
            }

        }


        #endregion


        #region -----// Others //-----

        /// <summary>
        /// 依代號取得資料庫實體名稱
        /// </summary>
        /// <param name="dbs">TW/SH/SZ</param>
        /// <returns></returns>
        private string GetDBName(string dbs)
        {
            switch (dbs.ToUpper())
            {
                case "SH":
                    return "SHPK2";

                default:
                    return "prokit2";
            }
        }


        #endregion


    }
}