using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using ClosedXML.Excel;
using CustomController;
using ExtensionMethods;

namespace ExtensionUI
{
    /// <summary>
    /// 自訂常用的UI
    /// 換網站時注意DB名前置詞
    /// </summary>
    public class fn_CustomUI
    {
        #region -- EXCEL匯出 --
        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="fileName"></param>
        /// <remarks>
        /// 使用元件:ClosedXML
        /// </remarks>
        /// <seealso cref="https://closedxml.codeplex.com/wikipage?title=Showcase&referringTitle=Documentation"/>
        public static void ExportExcel(DataTable DT, string fileName)
        {
            //宣告
            XLWorkbook wbook = new XLWorkbook();

            //-- 工作表設定 Start --
            var ws = wbook.Worksheets.Add(DT, "PKDataList");

            //鎖定工作表, 並設定密碼
            ws.Protect("iLoveProkits25")    //Set Password
                .SetFormatCells(true)   // Cell Formatting
                .SetInsertColumns() // Inserting Columns
                .SetDeleteColumns() // Deleting Columns
                .SetDeleteRows();   // Deleting Rows

            //細項設定
            ws.Tables.FirstOrDefault().ShowAutoFilter = false;  //停用自動篩選
            ws.Style.Font.FontName = "Microsoft JhengHei";  //字型名稱
            ws.Style.Font.FontSize = 10;

            //修改標題列
            var header = ws.FirstRowUsed(false);
            //header.Style.Fill.BackgroundColor = XLColor.Green;
            //header.Style.Font.FontColor = XLColor.Yellow;
            header.Style.Font.FontSize = 12;
            header.Style.Font.Bold = true;
            header.Height = 22;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //-- 工作表設定 End --

            //Http Response & Request
            var resp = HttpContext.Current.Response;
            var req = HttpContext.Current.Request;
            HttpResponse httpResponse = resp;
            httpResponse.Clear();
            // 編碼
            httpResponse.ContentEncoding = Encoding.UTF8;
            // 設定網頁ContentType
            httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // 匯出檔名
            var browser = req.Browser.Browser;
            var exportFileName = browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase)
                ? fileName
                : HttpUtility.UrlEncode(fileName, Encoding.UTF8);

            resp.AddHeader(
                "Content-Disposition",
                string.Format("attachment;filename={0}", exportFileName));

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wbook.SaveAs(memoryStream);
                memoryStream.WriteTo(httpResponse.OutputStream);
                memoryStream.Close();
            }

            httpResponse.End();
        }

        /// <summary>
        /// Linq查詢結果轉Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// 此方法僅可接受IEnumerable<T>泛型物件
        /// DataTable dt = LINQToDataTable(query);
        /// </remarks>
        public static DataTable LINQToDataTable<T>(IEnumerable<T> query)
        {
            //宣告一個datatable
            DataTable tbl = new DataTable();
            //宣告一個propertyinfo為陣列的物件，此物件需要import reflection才可以使用
            //使用 ParameterInfo 的執行個體來取得有關參數的資料型別、預設值等資訊

            PropertyInfo[] props = null;
            //使用型別為T的item物件跑query的內容
            foreach (T item in query)
            {
                if (props == null) //尚未初始化
                {
                    //宣告一型別為T的t物件接收item.GetType()所回傳的物件
                    Type t = item.GetType();
                    //props接收t.GetProperties();所回傳型別為props的陣列物件
                    props = t.GetProperties();
                    //使用propertyinfo物件針對propertyinfo陣列的物件跑迴圈
                    foreach (PropertyInfo pi in props)
                    {
                        //將pi.PropertyType所回傳的物件指給型別為Type的coltype物件
                        Type colType = pi.PropertyType;
                        //針對Nullable<>特別處理
                        if (colType.IsGenericType
                            && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            colType = colType.GetGenericArguments()[0];
                        //建立欄位
                        tbl.Columns.Add(pi.Name, colType);
                    }
                }
                //宣告一個datarow物件
                DataRow row = tbl.NewRow();
                //同樣利用PropertyInfo跑迴圈取得props的內容，並將內容放進剛所宣告的datarow中
                //接著在將該datarow加到datatable (tb1) 當中
                foreach (PropertyInfo pi in props)
                    row[pi.Name] = pi.GetValue(item, null) ?? DBNull.Value;
                tbl.Rows.Add(row);
            }
            //回傳tb1的datatable物件
            return tbl;
        }
        #endregion

        #region -- 產生選單 --
        /// <summary>
        /// 取得部門 (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Get_DeptList(DropDownListGP setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            return Get_DeptList(setMenu, inputValue, showRoot, null, out ErrMsg);
        }

        /// <summary>
        /// 取得部門 (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <param name="AreaCode">區域別</param>
        /// <returns></returns>
        /// /// <remarks>
        /// DB = PKSYS
        /// </remarks>
        public static bool Get_DeptList(DropDownListGP setMenu, string inputValue, bool showRoot, List<string> AreaCode, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料] - 部門資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Shipping.SName, User_Dept.DeptID, User_Dept.DeptName ");
                    SBSql.AppendLine("  , ROW_NUMBER() OVER(PARTITION BY Shipping.SID ORDER BY Shipping.Sort, User_Dept.Sort ASC) AS GP_Rank");
                    SBSql.AppendLine(" FROM User_Dept WITH (NOLOCK) ");
                    SBSql.AppendLine("   INNER JOIN Shipping WITH (NOLOCK) ON User_Dept.Area = Shipping.SID ");
                    SBSql.AppendLine(" WHERE (User_Dept.Display = 'Y') ");

                    //判斷是否有區域別條件, 組合SQL字串
                    if (AreaCode != null && AreaCode.Count > 0)
                    {
                        SBSql.Append(" AND (User_Dept.Area IN ({0})) ".FormatThis(
                                fn_Extensions.GetSQLParam(AreaCode, "Area")
                            ));
                    }

                    SBSql.AppendLine(" ORDER BY Shipping.Sort, User_Dept.Sort ");
                    cmd.CommandText = SBSql.ToString();

                    //判斷是否有區域別條件, 組合參數串
                    if (AreaCode != null && AreaCode.Count > 0)
                    {
                        for (int row = 0; row < AreaCode.Count; row++)
                        {
                            cmd.Parameters.AddWithValue("Area{0}".FormatThis(row), AreaCode[row].ToString());
                        }
                    }
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
                    using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            //判斷GP_Rank, 若為第一項，則輸出群組名稱
                            if (DT.Rows[row]["GP_Rank"].ToString().Equals("1"))
                            {
                                setMenu.AddItemGroup(DT.Rows[row]["SName"].ToString());

                            }

                            setMenu.Items.Add(new ListItem(DT.Rows[row]["DeptName"].ToString()
                                         , DT.Rows[row]["DeptID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇部門 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得產品檔案分類 (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Get_ProdFileClass(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    SBSql.AppendLine(" SELECT Cls.Class_ID AS ID, Cls.Class_Name AS Label ");
                    SBSql.AppendLine(" FROM Prod_FilesClass Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y') AND (UPPER(Cls.LangCode) = 'ZH-TW') ");
                    SBSql.AppendLine(" ORDER BY Cls.Sort, Cls.Class_ID");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇類別 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 取得語系 (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Get_LangList(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    SBSql.AppendLine(" SELECT Cls.LangCode AS ID, Cls.LangName AS Label ");
                    SBSql.AppendLine(" FROM Param_Language Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y')");
                    SBSql.AppendLine(" ORDER BY Cls.Sort");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇語系 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 檔案下載語系類別 (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Get_LangTypeList(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";
            setMenu.Items.Clear();

            try
            {
                //[取得資料]
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();

                    SBSql.AppendLine(" SELECT Cls.Class_ID AS ID, Cls.Class_Name AS Label ");
                    SBSql.AppendLine(" FROM Prod_FileLangType Cls WITH (NOLOCK) ");
                    SBSql.AppendLine(" WHERE (Cls.Display = 'Y')");
                    SBSql.AppendLine(" ORDER BY Cls.Sort");
                    cmd.CommandText = SBSql.ToString();

                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(DT.Rows[row]["Label"].ToString()
                                         , DT.Rows[row]["ID"].ToString()));
                        }
                        //判斷是否有已選取的項目
                        if (false == string.IsNullOrEmpty(inputValue))
                        {
                            setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                        }
                        //判斷是否要顯示索引文字
                        if (showRoot)
                        {
                            setMenu.Items.Insert(0, new ListItem("-- 選擇語系 --", ""));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }
        #endregion

        #region -- 狀態 --
        /// <summary>
        /// 一般狀態
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Get_PubDisp(RadioButtonList setMenu, string inputValue, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";

            try
            {
                var queryList = fn_CustomUI.Get_PubDisp(true)
                    .OrderBy(el => el.Sort);
                //新增選單項目
                foreach (var item in queryList)
                {
                    setMenu.Items.Add(new ListItem(item.Name, item.ID));
                }

                //判斷是否有已選取的項目
                if (false == string.IsNullOrEmpty(inputValue))
                {
                    setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 一般狀態List
        /// </summary>
        /// <returns></returns>
        public static List<PubDisplay> Get_PubDisp(bool showIcon)
        {
            List<PubDisplay> list_Disp = new List<PubDisplay>();
            list_Disp.Add(new PubDisplay("Y", "{0}顯示中".FormatThis(showIcon ? "<i class=\"fa fa-check\"></i>&nbsp;" : ""), 1));
            list_Disp.Add(new PubDisplay("N", "{0}不顯示".FormatThis(showIcon ? "<i class=\"fa fa-ban\"></i>&nbsp;" : ""), 2));

            return list_Disp;
        }


        /// <summary>
        /// 下載對象
        /// </summary>
        /// <param name="setMenu"></param>
        /// <param name="inputValue"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool Get_Target(RadioButtonList setMenu, string inputValue, out string ErrMsg)
        {
            //清除參數
            ErrMsg = "";

            try
            {
                var queryList = fn_CustomUI.Get_Target()
                    .OrderBy(el => el.Sort);
                //新增選單項目
                foreach (var item in queryList)
                {
                    setMenu.Items.Add(new ListItem(item.Name, item.ID));
                }

                //判斷是否有已選取的項目
                if (false == string.IsNullOrEmpty(inputValue))
                {
                    setMenu.SelectedIndex = setMenu.Items.IndexOf(setMenu.Items.FindByValue(inputValue.ToString().Trim()));
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }
        /// <summary>
        /// 下載對象List
        /// </summary>
        /// <returns></returns>
        public static List<PubDisplay> Get_Target()
        {
            List<PubDisplay> list_Disp = new List<PubDisplay>();
            list_Disp.Add(new PubDisplay("0", "Everyone&nbsp;&nbsp;", 1));
            list_Disp.Add(new PubDisplay("1", "會員&nbsp;&nbsp;", 2));
            list_Disp.Add(new PubDisplay("2", "經銷商&nbsp;&nbsp;", 3));

            return list_Disp;
        }



        /// <summary>
        /// 狀態參數
        /// </summary>
        public class PubDisplay
        {
            /// <summary>
            /// [參數] - ID
            /// </summary>
            private string _ID;
            public string ID
            {
                get { return this._ID; }
                set { this._ID = value; }
            }

            /// <summary>
            /// [參數] - Name
            /// </summary>
            private string _Name;
            public string Name
            {
                get { return this._Name; }
                set { this._Name = value; }
            }

            /// <summary>
            /// [參數] - Sort
            /// </summary>
            private int _Sort;
            public int Sort
            {
                get { return this._Sort; }
                set { this._Sort = value; }
            }

            /// <summary>
            /// 設定參數值
            /// </summary>
            /// <param name="ID">編號</param>
            /// <param name="Name">名稱</param>
            /// <param name="Sort">排序</param>
            public PubDisplay(string ID, string Name, int Sort)
            {
                this._ID = ID;
                this._Name = Name;
                this._Sort = Sort;
            }
        }
        #endregion


    }
}
