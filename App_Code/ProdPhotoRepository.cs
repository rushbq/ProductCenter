using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Method.Methods;
using ProdPhotoData.Models;

/*
   產品圖片
 */
namespace ProdPhotoData.Controllers
{
    public class ProdPhotoRepository
    {
        public string ErrMsg;

        #region -----// Read //-----


        /// <summary>
        /// 取得產品圖片(依類別自動判斷)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="picClass"></param>
        /// <returns></returns>
        public IQueryable<PhotoItem> GetPhotos(string id, string picClass)
        {
            switch (picClass)
            {
                case "1":
                case "2":
                    return GetPhotoItems(id, picClass);

                default:
                    return GetPhotoGroup(id, picClass);
            }
        }

        /// <summary>
        /// 取得產品圖片(主圖/輔圖)
        /// </summary>
        /// <param name="id">品號</param>
        /// <param name="picClass">1 or 2</param>
        /// <returns>
        /// picClass = 1 -> 產品主圖
        /// picClass = 2 -> 產品輔圖
        /// </returns>
        private IQueryable<PhotoItem> GetPhotoItems(string id, string picClass)
        {
            //----- 宣告 -----
            List<PhotoItem> dataList = new List<PhotoItem>();
            StringBuilder sql = new StringBuilder();


            //----- 取得欄位參考 -----
            var refCol = GetRefCols(picClass);

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(Model_No) ModelNo");

                foreach (var item in refCol)
                {
                    sql.Append(" ,{0}".FormatThis(item.ColID));
                }

                sql.AppendLine(" FROM {0} WITH(NOLOCK)".FormatThis(picClass.Equals("1") ? "ProdPic_Photo" : "ProdPic_Figure"));
                sql.AppendLine(" WHERE (UPPER(RTRIM(Model_No)) = UPPER(@ID)) ");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ID", id);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();


                    //資料迴圈
                    foreach (var item in query)
                    {
                        foreach (var subItem in refCol)
                        {
                            //加入項目
                            var data = new PhotoItem
                            {
                                ColID = subItem.ColID,
                                ColName = subItem.ColName,
                                ColValue = item.Field<string>(subItem.ColID)
                            };


                            //將項目加入至集合
                            dataList.Add(data);
                        }

                    }
                }

                //回傳集合
                return dataList.AsQueryable();
            }
        }


        /// <summary>
        /// 取得圖片欄位參考(僅限class=1,2)
        /// </summary>
        /// <param name="picClass"></param>
        /// <returns></returns>
        private IQueryable<PhotoItem> GetRefCols(string picClass)
        {
            //----- 宣告 -----
            List<PhotoItem> dataList = new List<PhotoItem>();
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Param_RelColumn, Param_Name");
                sql.AppendLine(" FROM ProdPic_Param WITH(NOLOCK)");
                sql.AppendLine(" WHERE (Pic_Class = @Class) AND (Display = 'Y')");
                sql.AppendLine(" ORDER BY Sort");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("Class", picClass);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();

                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new PhotoItem
                        {
                            ColID = item.Field<string>("Param_RelColumn"),
                            ColName = item.Field<string>("Param_Name")
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
        /// 取得產品圖片(其他類別)
        /// </summary>
        /// <param name="id">品號</param>
        /// <param name="picClass">其他</param>
        /// <returns></returns>
        private IQueryable<PhotoItem> GetPhotoGroup(string id, string picClass)
        {
            //----- 宣告 -----
            List<PhotoItem> dataList = new List<PhotoItem>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT RTRIM(Model_No) ModelNo, Lang, Pic_ID, Pic_OrgFile, Pic_File, Pic_Desc");
                sql.AppendLine(" FROM ProdPic_Group WITH(NOLOCK)");
                sql.AppendLine(" WHERE (UPPER(RTRIM(Model_No)) = UPPER(@ID)) AND (Pic_Class = @Cls) ");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("ID", id);
                cmd.Parameters.AddWithValue("Cls", picClass);


                //----- 資料取得 -----
                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    //LinQ 查詢
                    var query = DT.AsEnumerable();


                    //資料迴圈
                    foreach (var item in query)
                    {
                        //加入項目
                        var data = new PhotoItem
                        {
                            ColID = item.Field<int>("Pic_ID").ToString(),
                            ColName = "{0} {1}".FormatThis(
                                item.Field<string>("Pic_OrgFile")
                                , string.IsNullOrEmpty(item.Field<string>("Lang")) ? "" : "(" + item.Field<string>("Lang") + ")"
                            ),
                            ColValue = item.Field<string>("Pic_File")
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
        /// 取得圖片大類
        /// </summary>
        /// <returns></returns>
        public IQueryable<PhotoClass> GetPhotoClass()
        {
            //----- 宣告 -----
            List<PhotoClass> dataList = new List<PhotoClass>();
            StringBuilder sql = new StringBuilder();


            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Class_ID, ClassName");
                sql.AppendLine(" FROM ProdPic_Class");
                sql.AppendLine(" WHERE (Display = 'Y')");
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
                        var data = new PhotoClass
                        {
                            ID = item.Field<int>("Class_ID"),
                            Label = item.Field<string>("ClassName")
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


    }
}
