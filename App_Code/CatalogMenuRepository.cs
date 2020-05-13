using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PKLib_Method.Methods;
using CatalogMenuData.Models;

namespace CatalogMenuData.Controllers
{
    public class MenuRepository
    {
        #region -----// Read //-----

        /// <summary>
        /// 選得分類選單
        /// </summary>
        /// <param name="lang">語系(tw/en/cn)</param>
        /// <param name="search">查詢參數</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public IQueryable<MenuItem> GetList(string lang, Dictionary<string, string> search, out string ErrMsg)
        {
            //----- 宣告 -----
            List<MenuItem> dataList = new List<MenuItem>();
            StringBuilder sql = new StringBuilder();
            string langCode = fn_Language.Get_DBLangCode(lang);

            //----- 資料取得 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT Menu.Menu_ID AS ID, Menu.MenuName_{0} AS Label".FormatThis(langCode));
                sql.AppendLine(" , Menu.Parent_ID, Menu.Menu_Level, Menu.Class_ID");
                sql.AppendLine(" FROM [PKCatalog].dbo.Catalog_Menu Menu");
                sql.AppendLine(" WHERE (Display = 'Y')");

                /* Search */
                #region >> filter <<

                if (search != null)
                {
                    //過濾空值
                    var thisSearch = search.Where(fld => !string.IsNullOrWhiteSpace(fld.Value));

                    //查詢內容
                    foreach (var item in thisSearch)
                    {
                        switch (item.Key)
                        {
                            case "Level":
                                switch (item.Value)
                                {
                                    case "1":
                                        sql.Append(" AND (Menu_Level = 1)");
                                        break;

                                    case "2":
                                        sql.Append(" AND (Menu_Level = 2)");
                                        break;

                                    default:
                                        sql.Append(" AND (Menu_Level = 3)");
                                        break;
                                }
                                break;


                            case "ClassID":
                                sql.Append(" AND (Class_ID = @Class_ID)");

                                cmd.Parameters.AddWithValue("Class_ID", item.Value);

                                break;


                            case "ParentID":
                                sql.Append(" AND (Parent_ID = @Parent_ID)");

                                cmd.Parameters.AddWithValue("Parent_ID", item.Value);

                                break;


                            case "Keyword":
                                sql.Append(" AND (");
                                sql.Append("    (UPPER(Menu.Menu_ID) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("    OR (UPPER(RTRIM(Menu.MenuName_zh_TW)) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("    OR (UPPER(RTRIM(Menu.MenuName_zh_CN)) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append("    OR (UPPER(RTRIM(Menu.MenuName_en_US)) LIKE '%' + UPPER(@Keyword) + '%')");
                                sql.Append(" )");

                                cmd.Parameters.AddWithValue("Keyword", item.Value);
                                break;
                        }
                    }
                }
                #endregion

                //order by
                sql.AppendLine(" ORDER BY Menu.Sort, Menu.Menu_ID");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                //cmd.CommandTimeout = 60;   //單位:秒

                using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                {
                    if (DT != null)
                    {
                        //LinQ 查詢
                        var query = DT.AsEnumerable();

                        //資料迴圈
                        foreach (var item in query)
                        {
                            //加入項目
                            var data = new MenuItem
                            {
                                ID = item.Field<int>("ID"),
                                Label = item.Field<string>("Label"),
                                Parent_ID = item.Field<int>("Parent_ID"),
                                Menu_Level = item.Field<int>("Menu_Level"),
                                Class_ID = item.Field<int>("Class_ID")
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


        #endregion
    }
}