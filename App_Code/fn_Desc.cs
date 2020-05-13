using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 各功能的中英文描述轉換
/// </summary>
public class fn_Desc
{
    /// <summary>
    /// 產品類
    /// </summary>
    public class Prod {
        /// <summary>
        /// 產品Logo
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string Logo(string inputValue) {
            switch (inputValue) { 
                case "1":
                    return "PK Logo";
                case "2":
                    return "無Logo";
                case "3":
                    return "客戶Logo";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 主/配件
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string Accessories(string inputValue)
        {
            switch (inputValue)
            {
                case "1":
                    return "主件";
                case "2":
                    return "配件";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 品規輸入方式
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string InputType(string inputValue)
        {
            switch (inputValue.ToUpper())
            {
                case "SINGLESELECT":
                    return "單選";

                case "MULTISELECT":
                    return "複選";

                case "INT":
                    return "數值";

                case "DEVIATIONINT":
                    return "誤差值";

                case "BETWEENINT":
                    return "介於值";

                case "GREATERSMALL":
                    return "大小值";

                case "INTGREATERSMALL":
                    return "誤差大小值";

                case "RATIO":
                    return "比例值";

                case "MULTITYPE":
                    return "文字多行";

                case "SINGLETYPE":
                    return "文字單行";

                default:
                    return "";
            }
        }
    }

    /// <summary>
    /// 共用類
    /// </summary>
    public class PubAll {
        /// <summary>
        /// 是否
        /// </summary>
        /// <param name="inputValue">輸入值</param>
        /// <returns>string</returns>
        public static string YesNo(string inputValue)
        {
            switch (inputValue.ToUpper())
            {
                case "Y":
                    return "是";
                case "N":
                    return "否";
                default:
                    return "";
            }
        }
    }

    #region --描述說明--
    /// <summary>
    /// 描述回傳 - 是否
    /// </summary>
    /// <param name="inputValue">輸入文字(Y/N)</param>
    /// <returns></returns>
    public static string Desc_YesNo_zhTW(string inputValue)
    {
        //檢查 - 是否為空白字串
        if (string.IsNullOrEmpty(inputValue))
            return "";

        switch (inputValue.ToUpper())
        {
            case "Y":
                return "是";

            case "N":
                return "否";

            default:
                return "";
        }
    }

    /// <summary>
    /// 描述回傳 - Yes No
    /// </summary>
    /// <param name="inputValue">輸入文字(Y/N)</param>
    /// <returns></returns>
    public static string Desc_YesNo_enUS(string inputValue)
    {
        //檢查 - 是否為空白字串
        if (string.IsNullOrEmpty(inputValue))
            return "";

        switch (inputValue.ToUpper())
        {
            case "Y":
                return "Yes";

            case "N":
                return "No";

            default:
                return "";
        }
    }
    #endregion
    
}