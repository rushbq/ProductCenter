using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI;
using System.Collections;

namespace ExtensionMethods
{
    public static class fn_Extensions
    {
        #region "一般功能"
        /// <summary>
        /// 簡化string.format
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatThis(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 取得Right字串
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="length">取得長度</param>
        /// <returns>string</returns>
        /// <example>
        /// string str = "12345";
        /// str = str.Right(3);  //345
        /// </example>
        public static string Right(this string inputValue, int length)
        {
            length = Math.Max(length, 0);

            if (inputValue.Length > length)
            {
                return inputValue.Substring(inputValue.Length - length, length);
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// 取得Left字串
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="length">取得長度</param>
        /// <returns>string</returns>
        /// <example>
        /// string str = "12345";
        /// str = str.Left(3);  //123
        /// </example>
        public static string Left(this string inputValue, int length)
        {
            length = Math.Max(length, 0);

            if (inputValue.Length > length)
            {
                return inputValue.Substring(0, length);
            }
            else
            {
                return inputValue;
            }
        }

        /// <summary>
        /// 取得各參數串的值
        /// </summary>
        /// <param name="str">String to process</param>
        /// <param name="OuterSeparator">Separator for each "NameValue"</param>
        /// <param name="NameValueSeparator">Separator for Name/Value splitting</param>
        /// <returns></returns>
        /// <example>
        /// string para = "param1=value1;param2=value2";
        /// NameValueCollection nv = para.ToNameValueCollection(';', '=');
        /// foreach (var item in nv)
        /// {
        ///     Response.Write(item + "<BR>");
        /// }
        /// </example>
        public static NameValueCollection ToNameValueCollection(this String inputValue, Char OuterSeparator, Char NameValueSeparator)
        {
            NameValueCollection nvText = null;
            inputValue = inputValue.TrimEnd(OuterSeparator);
            if (!String.IsNullOrEmpty(inputValue))
            {
                String[] arrStrings = inputValue.TrimEnd(OuterSeparator).Split(OuterSeparator);

                foreach (String s in arrStrings)
                {
                    Int32 posSep = s.IndexOf(NameValueSeparator);
                    String name = s.Substring(0, posSep);
                    String value = s.Substring(posSep + 1);
                    if (nvText == null)
                        nvText = new NameValueCollection();
                    nvText.Add(name, value);
                }
            }
            return nvText;
        }

        /// <summary>
        /// 檢查格式 - 日期
        /// </summary>
        /// <param name="inputValue">日期</param>
        /// <returns>bool</returns>
        /// <example>
        /// string someDate = "2010/1/5";
        /// bool isDate = nonDate.IsDate();
        /// </example>
        public static bool IsDate(this string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                DateTime dt;
                return (DateTime.TryParse(inputValue, out dt));
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查格式 - 網址
        /// </summary>
        /// <param name="inputValue">網址字串</param>
        /// <returns>bool</returns>
        public static bool IsUrl(this string inputValue)
        {
            return Regex.IsMatch(inputValue, "http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&amp;=]*)?");
        }

        /// <summary>
        /// 檢查格式 - 座標
        /// </summary>
        /// <param name="Lat">座標-Lat字串</param>
        /// <param name="Lng">座標-Lng字串</param>
        /// <returns>Boolean</returns>
        public static bool IsLatLng(string Lat, string Lng)
        {
            if (IsNumeric(Lat) & IsNumeric(Lng))
            {
                if (Math.Abs(Convert.ToDouble(Lat)) >= 0 & Math.Abs(Convert.ToDouble(Lat)) < 180 & Math.Abs(Convert.ToDouble(Lng)) >= 0 & Math.Abs(Convert.ToDouble(Lng)) < 180)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 檢查格式 - 數字
        /// </summary>
        /// <param name="Expression">輸入值</param>
        /// <returns>bool</returns>
        /// <see cref="http://support.microsoft.com/kb/329488/zh-tw"/>
        public static bool IsNumeric(this object Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;
            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;
            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        /// <summary>
        /// 檢查格式 - EMail
        /// </summary>
        /// <param name="inputValue">Email</param>
        /// <returns>bool</returns>
        public static bool IsEmail(this string inputValue)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(inputValue,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// 日期格式化
        /// </summary>
        /// <param name="inputValue">日期字串</param>
        /// <param name="formatValue">要輸出的格式</param>
        /// <returns>string</returns>
        public static string ToDateString(this string inputValue, string formatValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return "";
            }
            else
            {
                return String.Format("{0:" + formatValue + "}", Convert.ToDateTime(inputValue));
            }

        }

        /// <summary>
        /// 日期格式化 - ERP
        /// </summary>
        /// <param name="inputValue">日期字串</param>
        /// <param name="formatValue">日期間隔符號</param>
        /// <returns>string</returns>
        /// <example>原始日期:20101215</example>
        public static string ToDateString_ERP(this string inputValue, string formatValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return "";
            }
            else
            {
                return String.Format("{1}{0}{2}{0}{3}"
                    , formatValue
                    , inputValue.Substring(0, 4)
                    , inputValue.Substring(4, 2)
                    , inputValue.Substring(6, 2));
            }

        }

        /// <summary>
        /// 產生隨機英數字
        /// </summary>
        /// <param name="VcodeNum">顯示幾碼</param>
        /// <returns>string</returns>
        public static string RndNum(int VcodeNum)
        {
            string Vchar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了
            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数
            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);
                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }

        /// <summary>
        /// 判斷字串內是否包含某字詞
        /// </summary>
        /// <param name="inputValue">輸入字串</param>
        /// <param name="strPool">要判斷的字詞</param>
        /// <param name="splitSymbol">Array的分割符號</param>
        /// <param name="splitNum">分割符號的數量</param>
        /// <returns></returns>
        /// <example>
        ///     string strTmp = ".jpg||.png||.pdf||.bmp";
        ///     Response.Write(fn_Extensions.CheckStrWord(src, strTmp, "|", 2));        
        /// </example>
        public static bool CheckStrWord(string inputValue, string strPool, string splitSymbol, int splitNum)
        {
            string[] strAry = Regex.Split(strPool, @"\" + splitSymbol + "{" + splitNum + "}");
            foreach (string item in strAry)
            {
                if (inputValue.IndexOf(item.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region "字串驗証"

        //================================= 字串 =================================
        public enum InputType
        {
            英文,
            數字,
            小寫英文,
            小寫英文混數字,
            小寫英文開頭混數字,
            大寫英文,
            大寫英文混數字,
            大寫英文開頭混數字
        }

        /// <summary>
        /// 驗証 - 輸入類型(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="InputType">輸入類型</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_輸入限制(string value, InputType InputType, string minLength, string maxLength
            , out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //判斷輸入限制種類 - InputType
                switch (InputType)
                {
                    case InputType.數字:

                        return IsNumeric(value);

                    case InputType.英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                & System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                                & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.小寫英文開頭混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (i == 0)
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 97 | System.Char.Parse(value.Substring(i, 1)) > 122)
                                    & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                                {
                                    return false;
                                }
                            }
                        }

                        break;

                    case InputType.大寫英文:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.大寫英文混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                            {
                                return false;
                            }
                        }

                        break;

                    case InputType.大寫英文開頭混數字:
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (i == 0)
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if ((System.Char.Parse(value.Substring(i, 1)) < 65 | System.Char.Parse(value.Substring(i, 1)) > 90)
                                    & (System.Char.Parse(value.Substring(i, 1)) < 48 | System.Char.Parse(value.Substring(i, 1)) > 57))
                                {
                                    return false;
                                }
                            }
                        }

                        break;
                }

                //檢查字數是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (value.Length < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "字數小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查字數是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (value.Length > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "字數大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 輸入字數(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_字數(string value, string minLength, string maxLength, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查字數是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (value.Length < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "字數小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查字數是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (value.Length > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "字數大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 驗証 - 輸入字數(byte)(文字)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minLength">最少字元數</param>
        /// <param name="maxLength">最大字元數</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool String_資料長度Byte(string value, string minLength, string maxLength, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                double valueByteLength = System.Text.Encoding.Default.GetBytes(value).Length;
                //檢查資料長度(Byte)是不是小於 minLength
                if (IsNumeric(minLength))
                {
                    if (valueByteLength < Math.Floor(Convert.ToDouble(minLength)))
                    {
                        ErrMsg = "資料長度(Byte)小於 minLength：" + Math.Floor(Convert.ToDouble(minLength));
                        return false;
                    }
                }
                //檢查資料長度(Byte)是不是大於 maxLength
                if (IsNumeric(maxLength))
                {
                    if (valueByteLength > Math.Floor(Convert.ToDouble(maxLength)))
                    {
                        ErrMsg = "資料長度(Byte)大於 maxLength：" + maxLength;
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        //================================ 日期時間 ==============================
        /// <summary>
        /// 驗証 - 日期
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minDate">最小日期</param>
        /// <param name="maxDate">最大日期</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool DateTime_日期(string value, string minDate, string maxDate, out string ErrMsg)
        {
            try
            {
                DateTime dtResult;
                ErrMsg = "";
                value = value.Trim();
                minDate = minDate.Trim();
                maxDate = maxDate.Trim();
                //檢查是不是時間
                if (DateTime.TryParse(value, out dtResult) == false | string.IsNullOrEmpty(value))
                {
                    ErrMsg = "不是日期資料";
                    return false;
                }
                //檢查是不是小於 minDate
                if (DateTime.TryParse(minDate, out dtResult) & !string.IsNullOrEmpty(minDate))
                {
                    if (Convert.ToDateTime(value) < Convert.ToDateTime(minDate))
                    {
                        ErrMsg = "小於 minDate：" + string.Format(minDate, "yyyy-MM-dd");
                        return false;
                    }
                }
                //檢查是不是小於 maxDate
                if (DateTime.TryParse(maxDate, out dtResult) & !string.IsNullOrEmpty(maxDate))
                {
                    if (Convert.ToDateTime(value) > Convert.ToDateTime(maxDate))
                    {
                        ErrMsg = "大於 maxDate：" + string.Format(maxDate, "yyyy-MM-dd");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 時間
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minDateTime">最小時間</param>
        /// <param name="maxDateTime">最大時間</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool DateTime_時間(string value, string minDateTime, string maxDateTime, out string ErrMsg)
        {
            try
            {
                DateTime dtResult;
                ErrMsg = "";
                value = value.Trim();
                minDateTime = minDateTime.Trim();
                maxDateTime = maxDateTime.Trim();
                //檢查是不是時間
                if (DateTime.TryParse(value, out dtResult) == false | string.IsNullOrEmpty(value))
                {
                    ErrMsg = "不是時間資料";
                    return false;
                }
                //檢查是不是小於 minDateTime
                if (DateTime.TryParse(minDateTime, out dtResult) & !string.IsNullOrEmpty(minDateTime))
                {
                    if (Convert.ToDateTime(value) < Convert.ToDateTime(minDateTime))
                    {
                        ErrMsg = "小於 minDateTime：" + string.Format(minDateTime, "yyyy-MM-dd HH:mm:ss.fff");
                        return false;
                    }
                }
                //檢查是不是小於 maxDateTime
                if (DateTime.TryParse(maxDateTime, out dtResult) & !string.IsNullOrEmpty(maxDateTime))
                {
                    if (Convert.ToDateTime(value) > Convert.ToDateTime(maxDateTime))
                    {
                        ErrMsg = "大於 maxDateTime：" + string.Format(maxDateTime, "yyyy-MM-dd HH:mm:ss.fff");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        //================================= 數值 =================================
        /// <summary>
        /// 驗証 - 數字(正整數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_正整數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) <= 0)
                {
                    ErrMsg = "小於或等於 0";
                    return false;
                }
                //檢查是不是整數
                if (Convert.ToDouble(value) != Math.Floor(Convert.ToDouble(value)))
                {
                    ErrMsg = "正數非正整數";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(負整數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_負整數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) >= 0)
                {
                    ErrMsg = "大於或等於 0";
                    return false;
                }
                //檢查是不是整數
                if (Convert.ToDouble(value) != Math.Floor(Convert.ToDouble(value)))
                {
                    ErrMsg = "負數非負整數";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(正數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_正數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) <= 0)
                {
                    ErrMsg = "小於或等於 0";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }

        /// <summary>
        /// 驗証 - 數字(負數)
        /// </summary>
        /// <param name="value">要驗証的值</param>
        /// <param name="minValue">最小數值</param>
        /// <param name="maxValue">最大數值</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>Boolean</returns>
        public static bool Num_負數(string value, string minValue, string maxValue, out string ErrMsg)
        {
            try
            {
                value = value.Trim();
                ErrMsg = "";

                //檢查是不是數值
                if (IsNumeric(value) == false)
                {
                    ErrMsg = "不是數值";
                    return false;
                }
                //檢查是不是大於零
                if (Convert.ToDouble(value) >= 0)
                {
                    ErrMsg = "大於或等於 0";
                    return false;
                }
                //檢查是不是小於 minValue
                if (IsNumeric(minValue))
                {
                    if (Convert.ToDouble(value) < Convert.ToDouble(minValue))
                    {
                        ErrMsg = "小於 minValue：" + minValue;
                        return false;
                    }
                }
                //檢查是不是大於 maxValue
                if (IsNumeric(maxValue))
                {
                    if (Convert.ToDouble(value) > Convert.ToDouble(maxValue))
                    {
                        ErrMsg = "大於 maxValue：" + maxValue;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception：" + ex.Message.ToString();
                return false;

            }
        }


        #endregion

        #region "常用功能"
        /// <summary>
        /// 使用HttpWebRequest取得網頁資料
        /// </summary>
        /// <param name="url">網址</param>
        /// <returns>string</returns>
        public static string WebRequest_GET(string url)
        {
            try
            {
                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                //IIS為AD驗証時加入此段 Start
                req.UseDefaultCredentials = true;
                req.PreAuthenticate = true;
                req.Credentials = CredentialCache.DefaultCredentials;
                //IIS為AD驗証時加入此段 End

                req.Method = "GET";
                using (WebResponse wr = req.GetResponse())
                {
                    using (StreamReader myStreamReader = new StreamReader(wr.GetResponseStream(), myEncoding))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }


        /// <summary>
        /// 使用HttpWebRequest POST取得網頁資料
        /// </summary>
        /// <param name="url">網址</param>
        /// <param name="param">參數 (a=123&b=456)</param>
        /// <returns></returns>
        public static string WebRequest_POST(string url, string param)
        {
            try
            {
                byte[] postData = Encoding.ASCII.GetBytes(param);

                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postData.Length;

                // 寫入 Post Body Message 資料流
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(postData, 0, postData.Length);
                }

                // 取得回應資料
                string result = "";
                using (HttpWebResponse wr = req.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(wr.GetResponseStream(), myEncoding))
                    {
                        result = sr.ReadToEnd();
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 使用FileStream取得資料
        /// </summary>
        /// <param name="path">磁碟路徑</param>
        /// <returns>string</returns>
        public static string IORequest_GET(string path)
        {
            try
            {
                if (false == System.IO.File.Exists(path)) return "";
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader sw = new StreamReader(fs, System.Text.Encoding.UTF8))
                    {
                        return sw.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region "IO"

        private static object _thisLock = new object();
        /// <summary>
        /// 產生Html檔 - (產品訊息)
        /// </summary>
        /// <param name="myPath">檔案存放路徑</param>
        /// <param name="myFileName">檔案名稱</param>
        /// <param name="myTemplate">HTML模版完整路徑</param>
        /// <param name="myHtml">自訂的Html的Body內容</param>
        /// <param name="ProcMsg">處理訊息回傳</param>
        /// <returns></returns>
        public static bool Generate_Html(string myPath, string myFileName, string myTemplate, StringBuilder myHtml, out string ProcMsg)
        {
            try
            {
                //[判斷] - Folder是否存在, 不存在則建立
                fn_Extensions.CheckFolder(myPath);

                //[判斷] - Html 模版
                if (false == File.Exists(myTemplate))
                {
                    ProcMsg = "Html 模版不存在";
                    return false;
                }

                //[宣告] - 編碼方式
                Encoding myCode = Encoding.UTF8;

                //[資料取得] - Html模版內容
                string GetHtmlTemplate = "";
                using (StreamReader sr = new StreamReader(myTemplate, myCode))
                {
                    GetHtmlTemplate = sr.ReadToEnd();
                }

                /*
                 * [資料寫入] - 產生新的檔案
                 *  - 路徑
                 *  - true:append原本內容 ; false:覆蓋原本的內容
                 *  - 編碼
                 */
                lock (_thisLock)
                {
                    using (StreamWriter sw = new StreamWriter(myPath + myFileName, false, myCode))
                    {
                        //置換內容, 將#Container# 置換為自訂的內容
                        sw.Write(GetHtmlTemplate.Replace("#Container#", myHtml.ToString()));
                    }
                }

                //回傳訊息
                ProcMsg = "OK";
                return true;

            }
            catch (Exception ex)
            {
                ProcMsg = ex.Message.ToString();
                return false;
            }

        }

        /// <summary>
        /// IO - 判斷目標資料夾是否存在
        /// </summary>
        /// <param name="folder">目標資料夾</param>
        /// <returns>bool</returns>
        public static bool CheckFolder(string folder)
        {
            try
            {
                DirectoryInfo CheckFolder = new DirectoryInfo(folder);
                if (CheckFolder.Exists == false)
                    CheckFolder.Create();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region -- 產品 --
        ///// <summary>
        ///// 產品類別 - 產生XML
        ///// </summary>
        ///// <param name="ErrMsg">錯誤訊息</param>
        ///// <returns>bool</returns>
        //public static bool XmlProdClass(out string ErrMsg)
        //{
        //    ErrMsg = "";
        //    try
        //    {
        //        //[取得資料] - 產品類別選單
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            StringBuilder SBSql = new StringBuilder();
        //            SBSql.AppendLine("SELECT RTRIM(Class_ID) AS Class_ID ");
        //            SBSql.AppendLine("  , Class_Name_zh_TW, Class_Name_en_US, Class_Name_zh_CN, Display, Display_PKWeb, Sort ");
        //            SBSql.AppendLine(" FROM Prod_Class ");
        //            SBSql.AppendLine(" WHERE (LEFT(RTRIM(Class_ID),1) = '2') ");
        //            SBSql.AppendLine(" ORDER BY Sort, Class_ID ");
        //            cmd.CommandText = SBSql.ToString();
        //            cmd.Parameters.Clear();
        //            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
        //            {
        //                //判斷是否有資料
        //                if (DT.Rows.Count == 0)
        //                {
        //                    return false;
        //                }
        //                //判斷資料夾是否存在
        //                string folder = System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"] + @"Xml_Data\";
        //                if (fn_Extensions.CheckFolder(folder) == false)
        //                {
        //                    ErrMsg = "XML資料夾產生失敗，檔案未建立!";
        //                    return false;
        //                }

        //                //[XML] - 根目錄
        //                XElement ProdClass = new XElement("ProdClass");
        //                //[XML] - 各節點
        //                for (int i = 0; i < DT.Rows.Count; i++)
        //                {
        //                    ProdClass.Add(
        //                        new XElement("Class", new XAttribute("ID", DT.Rows[i]["Class_ID"].ToString()),
        //                            new XElement("Name_zh_TW", new XCData(DT.Rows[i]["Class_Name_zh_TW"].ToString())),
        //                            new XElement("Name_en_US", new XCData(DT.Rows[i]["Class_Name_en_US"].ToString())),
        //                            new XElement("Name_zh_CN", new XCData(DT.Rows[i]["Class_Name_zh_CN"].ToString())),
        //                            new XElement("Display", DT.Rows[i]["Display"].ToString()),
        //                            new XElement("Display_PKWeb", DT.Rows[i]["Display_PKWeb"].ToString()),
        //                            new XElement("Sort", DT.Rows[i]["Sort"].ToString())
        //                            )
        //                        );
        //                }
        //                //[XML] -  產生XML檔案
        //                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), ProdClass);
        //                xdoc.Save(folder + @"ProdClass.xml");
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrMsg = ex.Message.ToString();
        //        return false;
        //    }

        //}

        /// <summary>
        /// 產品類別 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool ProdClassMenu(DropDownList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 產品類別選單
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT RTRIM(Class_ID) AS ID, Class_Name_{0} AS Label".FormatThis(fn_Language.Param_Lang));
                    SBSql.AppendLine(" FROM Prod_Class ");
                    SBSql.AppendLine(" WHERE (LEFT(RTRIM(Class_ID),1) = '2') AND (Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Sort, Class_ID ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //判斷是否有資料
                        if (DT.Rows.Count == 0)
                        {
                            return false;
                        }

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            string ID = DT.Rows[row]["ID"].ToString();
                            string Label = DT.Rows[row]["Label"].ToString();

                            setMenu.Items.Add(new ListItem(
                                ID + " - " + Label
                                , ID));
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
        /// 產品類別 - 取得類別值
        /// </summary>
        /// <param name="ClassID">類別ID</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>string</returns>
        //public static string GetProdClassMenu(string ClassID, out string ErrMsg)
        //{
        //    ErrMsg = "";
        //    try
        //    {
        //        //取得Xml
        //        string XmlResult = fn_Extensions.WebRequest_GET(
        //            System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/ProdClass.xml");
        //        //將Xml字串轉成byte
        //        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
        //        //讀取Xml
        //        using (XmlReader reader = XmlTextReader.Create(stream))
        //        {
        //            //使用XElement載入Xml
        //            XElement XmlDoc = XElement.Load(reader);

        //            var Results = from result in XmlDoc.Elements("Class")
        //                          where result.Attribute("ID").Value.Equals(ClassID)
        //                          select new
        //                          {
        //                              ID = result.Attribute("ID").Value,
        //                              Name = result.Element("Name_zh_TW").Value
        //                          };

        //            if (Results.Count() == 0)
        //            {
        //                return "";
        //            }
        //            else
        //            {
        //                return Results.ElementAt(0).ID + " - " + Results.ElementAt(0).Name;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrMsg = ex.Message.ToString();
        //        return "";
        //    }
        //}

        /// <summary>
        /// 產品規格輸入方式 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool ProdSpecTypeMenu(DropDownList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //取得Xml
                string XmlResult = fn_Extensions.WebRequest_GET(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/ProdSpecType.xml");
                //將Xml字串轉成byte
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
                //讀取Xml
                using (XmlReader reader = XmlTextReader.Create(stream))
                {
                    //使用XElement載入Xml
                    XElement XmlDoc = XElement.Load(reader);

                    var Results = from result in XmlDoc.Elements("SpecType")
                                  orderby Convert.ToInt16(result.Element("Sort").Value) ascending
                                  select new
                                  {
                                      ID = result.Attribute("ID").Value,
                                      Name = result.Element("Name").Value,
                                      Desc = result.Element("Desc").Value
                                  };

                    foreach (var result in Results)
                    {
                        setMenu.Items.Add(new ListItem(
                            result.Name
                            , result.ID));
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
        /// ICON圖例 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="inputID">傳入的編號</param>
        /// <param name="inputSubID">傳入的明細編號</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool OptIconMenu(RadioButtonList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 符號資料庫
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Icon.IconName_zh_TW AS IconName, Icon_Pics.Pic_File AS FileName, Icon_Pics.Pic_ID AS Pic_ID, Icon.Icon_Type ");
                    SBSql.AppendLine(" FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                    SBSql.AppendLine(" WHERE (Icon.Icon_Type IN ('Product','Public')) AND (Icon.Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Icon.Sort, Icon_Pics.Sort, Icon.CID ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            string filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Icons/" + DT.Rows[row]["FileName"].ToString();
                            string iconName = DT.Rows[row]["IconName"].ToString();

                            setMenu.Items.Add(new ListItem(
                                "<img src=\"" + filePath + "\" width=\"30\" class=\"tooltip\" iconName=\"" + iconName + "\">"
                                , DT.Rows[row]["Pic_ID"].ToString()));
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
        /// ICON圖例 - 取得資料
        /// </summary>
        /// <param name="inputID">傳入的編號</param>
        /// <returns>圖片字串</returns>
        public static string GetOptIcon(string inputID)
        {
            try
            {
                string ErrMsg;

                //[取得資料] - 符號資料庫
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Icon.IconName_zh_TW AS IconName, Icon_Pics.Pic_File AS FileName, Icon.Icon_Type ");
                    SBSql.AppendLine(" FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                    SBSql.AppendLine(" WHERE (Icon.Icon_Type IN ('Product','Public')) AND (Icon.Display = 'Y') AND (Icon_Pics.Pic_ID = @inputID) ");
                    SBSql.AppendLine(" ORDER BY Icon.Sort, Icon_Pics.Sort, Icon.CID ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("inputID", inputID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return "";
                        }
                        //輸出項目
                        StringBuilder tmp = new StringBuilder();
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            tmp.Append(string.Format("<img src=\"{0}\" style=\"border:1px solid #efefef;\" width=\"35\" alt=\"{1}\" />&nbsp;"
                                , System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Icons/" + DT.Rows[row]["FileName"].ToString()
                                , DT.Rows[row]["IconName"].ToString()));
                        }

                        return tmp.ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// ICON圖例 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="inputID">傳入的編號</param>
        /// <param name="inputSubID">傳入的明細編號</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool SpecIconMenu(RadioButtonList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 符號資料庫
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Icon.IconName_zh_TW AS IconName, Icon_Pics.Pic_File AS FileName, Icon_Pics.Pic_ID AS Pic_ID, Icon.Icon_Type ");
                    SBSql.AppendLine(" FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                    SBSql.AppendLine(" WHERE (Icon.Icon_Type IN ('Product','Public')) AND (Icon.Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Icon.Sort, Icon_Pics.Sort, Icon.CID ");
                    cmd.CommandText = SBSql.ToString();
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            string filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Icons/" + DT.Rows[row]["FileName"].ToString();
                            string iconName = DT.Rows[row]["IconName"].ToString();

                            setMenu.Items.Add(new ListItem(
                                "<img src=\"" + filePath + "\" width=\"30\" class=\"tooltip\" iconName=\"" + iconName + "\">"
                                , DT.Rows[row]["Pic_ID"].ToString()));
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
        /// ICON圖例 - 取得資料
        /// </summary>
        /// <param name="inputID">傳入的編號</param>
        /// <returns>圖片字串</returns>
        public static string GetSpecIcon(string inputID)
        {
            try
            {
                string ErrMsg;

                //[取得資料] - 符號資料庫
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Icon.IconName_zh_TW AS IconName, Icon_Pics.Pic_File AS FileName, Icon.Icon_Type ");
                    SBSql.AppendLine(" FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                    SBSql.AppendLine(" WHERE (Icon.Icon_Type IN ('Product','Public')) AND (Icon.Display = 'Y') AND (Icon_Pics.Pic_ID = @inputID) ");
                    SBSql.AppendLine(" ORDER BY Icon.Sort, Icon_Pics.Sort, Icon.CID ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("inputID", inputID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return "";
                        }
                        //輸出項目
                        StringBuilder tmp = new StringBuilder();
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            tmp.Append(string.Format("<img src=\"{0}\" style=\"border:1px solid #efefef;\" width=\"35\" alt=\"{1}\" />&nbsp;"
                                , System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Icons/" + DT.Rows[row]["FileName"].ToString()
                                , DT.Rows[row]["IconName"].ToString()));
                        }

                        return tmp.ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 取得Vol (for DropDownList)
        /// </summary>
        /// <param name="setMenu">控制項</param>
        /// <param name="inputValue">輸入值</param>
        /// <param name="showRoot">是否顯示索引文字</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns></returns>
        public static bool Get_Vol(DropDownList setMenu, string inputValue, bool showRoot, out string ErrMsg)
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

                    // ↓↓ SQL查詢組成 ↓↓
                    SBSql.AppendLine(" SELECT Tbl.Catelog_Vol AS ID, Tbl.Catelog_Vol AS Label FROM ( ");
                    SBSql.AppendLine("     SELECT Catelog_Vol ");
                    SBSql.AppendLine("     FROM Prod_Item WITH(NOLOCK) ");
                    SBSql.AppendLine("     WHERE (Catelog_Vol <> '') ");
                    SBSql.AppendLine("     GROUP BY Catelog_Vol ");

                    //SBSql.AppendLine("     UNION ALL ");

                    //SBSql.AppendLine("     SELECT Catelog_Vol ");
                    //SBSql.AppendLine("     FROM Prod_ItemSZ WITH(NOLOCK) ");
                    //SBSql.AppendLine("     WHERE (Catelog_Vol <> '') ");
                    //SBSql.AppendLine("     GROUP BY Catelog_Vol ");
                    SBSql.AppendLine(" ) AS Tbl ");
                    SBSql.AppendLine(" GROUP BY Tbl.Catelog_Vol ");
                    SBSql.AppendLine(" ORDER BY Tbl.Catelog_Vol ");
                    cmd.CommandText = SBSql.ToString();
                    // ↑↑ SQL查詢組成 ↑↑

                    // SQL查詢執行
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
                            setMenu.Items.Insert(0, new ListItem("-- 不限 --", ""));
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

        #region -- 認証 --
        /// <summary>
        /// 認証類別 - 產生XML
        /// </summary>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool XmlCertClass(out string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                //判斷資料夾是否存在
                string folder = System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"] + @"Xml_Data\";
                if (fn_Extensions.CheckFolder(folder) == false)
                {
                    ErrMsg = "XML資料夾產生失敗，檔案未建立!";
                    return false;
                }

                //[XML] - 產生XML內容
                XElement Xele = new XElement("Certs",
                        new XElement("Cert", new XAttribute("ID", 1),
                            new XElement("Name", "OEM"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 2),
                            new XElement("Name", "RoHs"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 3),
                            new XElement("Name", "Pb Free"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 4),
                            new XElement("Name", "EMC"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 5),
                            new XElement("Name", "LVD"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 6),
                            new XElement("Name", "GS"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 7),
                            new XElement("Name", "UL"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 8),
                            new XElement("Name", "VDE"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 9),
                            new XElement("Name", "CB"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 10),
                            new XElement("Name", "ETL"),
                            new XElement("Sort", 100)
                        ),
                        new XElement("Cert", new XAttribute("ID", 11),
                            new XElement("Name", "IP"),
                            new XElement("Sort", 200)
                        ),
                        new XElement("Cert", new XAttribute("ID", 12),
                            new XElement("Name", "CE"),
                            new XElement("Sort", 300)
                        ),
                        new XElement("Cert", new XAttribute("ID", 999),
                            new XElement("Name", "其他"),
                            new XElement("Sort", 999)
                        )

                    );

                //[XML] -  產生XML檔案
                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), Xele);
                xdoc.Save(folder + @"Certification.xml");

                return true;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// 認證類別 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool CertMenu(DropDownList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //取得Xml
                string XmlResult = fn_Extensions.WebRequest_GET(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/Certification.xml");
                //將Xml字串轉成byte
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
                //讀取Xml
                using (XmlReader reader = XmlTextReader.Create(stream))
                {
                    //使用XElement載入Xml
                    XElement XmlDoc = XElement.Load(reader);

                    var Results = from result in XmlDoc.Elements("Cert")
                                  orderby result.Element("Sort").Value, result.Element("Name").Value ascending
                                  select new
                                  {
                                      ID = result.Attribute("ID").Value,
                                      Name = result.Element("Name").Value
                                  };

                    foreach (var result in Results)
                    {
                        setMenu.Items.Add(new ListItem(result.Name, result.Name));
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
        /// ICON圖例 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="inputID">傳入的編號</param>
        /// <param name="inputSubID">傳入的明細編號</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool CertIconMenu(CheckBoxList setMenu, string inputID, string inputSubID, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //[取得資料] - 符號資料庫
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Icon.IconName_zh_TW AS IconName, Icon_Pics.Pic_File AS FileName, Icon_Pics.Pic_ID AS Pic_ID, Icon.Icon_Type ");
                    SBSql.AppendLine("  , Rel.Pic_ID AS Rel_ID ");
                    SBSql.AppendLine(" FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                    SBSql.AppendLine("  LEFT JOIN Icon_Rel_Certification Rel ON Rel.Pic_ID = Icon_Pics.Pic_ID AND Rel.Cert_ID = @inputID AND Rel.Detail_ID = @inputSubID ");
                    SBSql.AppendLine(" WHERE (Icon.Icon_Type IN ('Certification','Public')) AND (Icon.Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Icon.Sort, Icon_Pics.Sort, Icon.CID ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("inputID", inputID);
                    cmd.Parameters.AddWithValue("inputSubID", inputSubID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        //新增選單項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            string filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Icons/" + DT.Rows[row]["FileName"].ToString();
                            string iconName = DT.Rows[row]["IconName"].ToString();

                            setMenu.Items.Add(new ListItem(
                                "<img src=\"" + filePath + "\" width=\"35\" class=\"tooltip\" iconName=\"" + iconName + "\">"
                                , DT.Rows[row]["Pic_ID"].ToString()));
                        }

                        //判斷是否有已選取的項目
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            for (int col = 0; col < setMenu.Items.Count; col++)
                            {
                                if (setMenu.Items[col].Value.Equals(DT.Rows[row]["Rel_ID"].ToString()))
                                {
                                    setMenu.Items[col].Selected = true;
                                }
                            }
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
        /// ICON圖例 - 取得資料
        /// </summary>
        /// <param name="inputID">傳入的編號</param>
        /// <param name="inputSubID">傳入的明細編號</param>
        /// <returns>圖片字串</returns>
        public static string GetCertIcon(string inputID, string inputSubID)
        {
            try
            {
                string ErrMsg;

                //[取得資料] - 符號資料庫
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Icon.IconName_zh_TW AS IconName, Icon_Pics.Pic_File AS FileName, Icon.Icon_Type ");
                    SBSql.AppendLine(" FROM Icon INNER JOIN Icon_Pics ON Icon.Icon_ID = Icon_Pics.Icon_ID ");
                    SBSql.AppendLine("  INNER JOIN Icon_Rel_Certification Rel ON Rel.Pic_ID = Icon_Pics.Pic_ID AND Rel.Cert_ID = @inputID AND Rel.Detail_ID = @inputSubID ");
                    SBSql.AppendLine(" WHERE (Icon.Icon_Type IN ('Certification','Public')) AND (Icon.Display = 'Y') ");
                    SBSql.AppendLine(" ORDER BY Icon.Sort, Icon_Pics.Sort, Icon.CID ");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("inputID", inputID);
                    cmd.Parameters.AddWithValue("inputSubID", inputSubID);
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count == 0)
                        {
                            return "";
                        }
                        //輸出項目
                        StringBuilder tmp = new StringBuilder();
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            tmp.Append(string.Format("<img src=\"{0}\" style=\"border:1px solid #efefef;\" width=\"35\" alt=\"{1}\" />&nbsp;"
                                , System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Icons/" + DT.Rows[row]["FileName"].ToString()
                                , DT.Rows[row]["IconName"].ToString()));
                        }

                        return tmp.ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region -- 語系 --
        /// <summary>
        /// 語系 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool LangMenu(DropDownList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //取得Xml
                string XmlResult = fn_Extensions.WebRequest_GET(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/Language.xml");
                //將Xml字串轉成byte
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
                //讀取Xml
                using (XmlReader reader = XmlTextReader.Create(stream))
                {
                    //使用XElement載入Xml
                    XElement XmlDoc = XElement.Load(reader);

                    var Results = from result in XmlDoc.Elements("Language")
                                  orderby result.Element("Sort").Value ascending
                                  select new
                                  {
                                      Value = result.Element("Value").Value,
                                      Name = result.Element("Name").Value
                                  };
                    setMenu.Items.Add(new ListItem("-- 語系 --", ""));
                    foreach (var result in Results)
                    {
                        setMenu.Items.Add(new ListItem(result.Name, result.Value));
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
        /// 語系 - 取得選單值
        /// </summary>
        /// <param name="ObjID">傳入ID</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>string</returns>
        public static string GetLangValue(string ObjID, out string ErrMsg)
        {
            ErrMsg = "";
            try
            {
                //取得Xml
                string XmlResult = fn_Extensions.WebRequest_GET(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/Language.xml");
                //將Xml字串轉成byte
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
                //讀取Xml
                using (XmlReader reader = XmlTextReader.Create(stream))
                {
                    //使用XElement載入Xml
                    XElement XmlDoc = XElement.Load(reader);

                    var Results = from result in XmlDoc.Elements("Language")
                                  where result.Element("Value").Value.Equals(ObjID)
                                  select new
                                  {
                                      Value = result.Element("Value").Value,
                                      Name = result.Element("Name").Value
                                  };

                    if (Results.Count() == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return Results.ElementAt(0).Name;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
                return "";
            }
        }
        #endregion

        #region -- 權限 --
        ///// <summary>
        ///// 權限 - 使用者權限
        ///// </summary>
        ///// <param name="GUID">AD GUID</param>
        ///// <param name="ErrMsg">錯誤訊息</param>
        ///// <returns>bool</returns>
        //public static bool XmlAuthProfile(string GUID, out string ErrMsg)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(GUID))
        //        {
        //            ErrMsg = "編號空白!";
        //            return false;
        //        }
        //        //[取得資料] - 使用者權限
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            StringBuilder SBSql = new StringBuilder();
        //            //[SQL] - 所有使用者
        //            SBSql.AppendLine("SELECT [Guid], Display, Account_Name FROM User_Profile WHERE (User_Profile.Guid = @Param_GUID) ");
        //            cmd.CommandText = SBSql.ToString();
        //            cmd.Parameters.Clear();
        //            cmd.Parameters.AddWithValue("Param_GUID", GUID);
        //            using (DataTable DT = dbConClass.LookupDT(cmd, dbConClass.DBS.PKSYS, out ErrMsg))
        //            {
        //                //判斷是否有資料
        //                if (DT.Rows.Count == 0)
        //                {
        //                    return false;
        //                }

        //                //暫存參數
        //                string Param_GUID = DT.Rows[0]["Guid"].ToString();
        //                string Param_Account = DT.Rows[0]["Account_Name"].ToString();
        //                string Param_Display = DT.Rows[0]["Display"].ToString();

        //                //判斷資料夾是否存在
        //                string folder = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"Data_File\Authorization\";
        //                if (fn_Extensions.CheckFolder(folder) == false)
        //                {
        //                    ErrMsg = "XML資料夾產生失敗，檔案未建立!";
        //                    return false;
        //                }

        //                //[XML] - 根目錄
        //                XElement DataNode = new XElement("Users");
        //                //[XML] - 節點 (第1層)
        //                XElement xLv1 =
        //                   new XElement("User",
        //                       new XAttribute("Guid", Param_GUID),
        //                       new XAttribute("Display", Param_Display)
        //                       );

        //                //[SQL] - 各使用者的權限關聯
        //                SBSql.Clear();
        //                SBSql.AppendLine(" SELECT Rel.Prog_ID ");
        //                SBSql.AppendLine(" FROM PKSYS.dbo.User_Profile INNER JOIN User_Profile_Rel_Program Rel ON User_Profile.Guid = Rel.Guid ");
        //                SBSql.AppendLine(" WHERE (User_Profile.Guid = @Param_GUID) ");
        //                cmd.CommandText = SBSql.ToString();
        //                cmd.Parameters.Clear();
        //                cmd.Parameters.AddWithValue("Param_GUID", Param_GUID);
        //                using (DataTable DT_Node = dbConClass.LookupDT(cmd, out ErrMsg))
        //                {
        //                    if (DT_Node.Rows.Count > 0)
        //                    {
        //                        for (int idx = 0; idx < DT_Node.Rows.Count; idx++)
        //                        {
        //                            //[XML] - 節點 (第2層)
        //                            XElement xLv2 =
        //                               new XElement("ProgID", DT_Node.Rows[idx]["Prog_ID"].ToString());

        //                            xLv1.Add(xLv2);
        //                        }
        //                    }
        //                }

        //                //[XML] - 新增節點
        //                DataNode.Add(xLv1);

        //                //[XML] -  產生XML檔案
        //                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), DataNode);

        //                xdoc.Save(folder + @"User_Profile_" + Param_Account + ".xml");

        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrMsg = ex.Message.ToString();
        //        return false;
        //    }

        //}

        ///// <summary>
        ///// 權限 - 群組權限
        ///// </summary>
        ///// <param name="ErrMsg">錯誤訊息</param>
        ///// <returns>bool</returns>
        //public static bool XmlAuthGroup(out string ErrMsg)
        //{
        //    try
        //    {
        //        //[取得資料] - 群組權限
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            StringBuilder SBSql = new StringBuilder();
        //            //[SQL] - 所有群組
        //            SBSql.AppendLine("SELECT [Guid], Display FROM PKSYS.dbo.User_Group ");
        //            SBSql.AppendLine(" WHERE EXISTS (SELECT [Guid] FROM User_Group_Rel_Program Rel WHERE (Rel.Guid = User_Group.Guid))");
        //            cmd.CommandText = SBSql.ToString();
        //            cmd.Parameters.Clear();
        //            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
        //            {
        //                //判斷是否有資料
        //                if (DT.Rows.Count == 0)
        //                {
        //                    return false;
        //                }

        //                //判斷資料夾是否存在
        //                string folder = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"Data_File\Authorization\";
        //                if (fn_Extensions.CheckFolder(folder) == false)
        //                {
        //                    ErrMsg = "XML資料夾產生失敗，檔案未建立!";
        //                    return false;
        //                }

        //                //[XML] - 根目錄
        //                XElement DataNode = new XElement("Groups");
        //                //[XML] - 各節點
        //                for (int i = 0; i < DT.Rows.Count; i++)
        //                {
        //                    //[SQL] - 各群組的權限關聯
        //                    SBSql.Clear();
        //                    SBSql.AppendLine(" SELECT Rel.Prog_ID ");
        //                    SBSql.AppendLine(" FROM PKSYS.dbo.User_Group INNER JOIN User_Group_Rel_Program Rel ON User_Group.Guid = Rel.Guid ");
        //                    SBSql.AppendLine(" WHERE (User_Group.Guid = @Param_GUID) ");
        //                    cmd.CommandText = SBSql.ToString();
        //                    cmd.Parameters.Clear();
        //                    cmd.Parameters.AddWithValue("Param_GUID", DT.Rows[i]["Guid"].ToString());
        //                    using (DataTable DT_Node = dbConClass.LookupDT(cmd, out ErrMsg))
        //                    {
        //                        if (DT_Node.Rows.Count > 0)
        //                        {
        //                            //[XML] - 節點 (第1層)
        //                            XElement xLv1 =
        //                               new XElement("Group",
        //                                   new XAttribute("Guid", DT.Rows[i]["Guid"].ToString()),
        //                                   new XAttribute("Display", DT.Rows[i]["Display"].ToString())
        //                                   );


        //                            for (int idx = 0; idx < DT_Node.Rows.Count; idx++)
        //                            {
        //                                //[XML] - 節點 (第2層)
        //                                XElement xLv2 =
        //                                   new XElement("ProgID", DT_Node.Rows[idx]["Prog_ID"].ToString());

        //                                xLv1.Add(xLv2);
        //                            }

        //                            //[XML] - 新增節點
        //                            DataNode.Add(xLv1);
        //                        }
        //                    }
        //                }
        //                //[XML] -  產生XML檔案
        //                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), DataNode);

        //                xdoc.Save(folder + @"User_Group.xml");

        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrMsg = ex.Message.ToString();
        //        return false;
        //    }

        //}
        #endregion

        #region -- 圖片 --
        /// <summary>
        /// 圖片資料庫 - 圖片類別,產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool ProdPicClassMenu(DropDownList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                //取得Xml
                string XmlResult = fn_Extensions.WebRequest_GET(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"] + @"Xml_Data/ProdPicClass.xml");
                //將Xml字串轉成byte
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
                //讀取Xml
                using (XmlReader reader = XmlTextReader.Create(stream))
                {
                    //使用XElement載入Xml
                    XElement XmlDoc = XElement.Load(reader);

                    var Results = from result in XmlDoc.Elements("Class")
                                  where !result.Attribute("ID").Value.Equals("8")
                                  orderby Convert.ToInt16(result.Element("Sort").Value) ascending
                                  select new
                                  {
                                      ID = result.Attribute("ID").Value,
                                      Name = result.Element("Name").Value
                                  };

                    foreach (var result in Results)
                    {
                        setMenu.Items.Add(new ListItem(
                            result.Name
                            , result.ID));
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

        #region -- 符號 --
        /// <summary>
        /// 符號使用者 - 產生選單
        /// </summary>
        /// <param name="setMenu">傳入的元素</param>
        /// <param name="ErrMsg">錯誤訊息</param>
        /// <returns>bool</returns>
        public static bool IconUseMenu(DropDownList setMenu, out string ErrMsg)
        {
            ErrMsg = "";
            setMenu.Items.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    StringBuilder SBSql = new StringBuilder();
                    SBSql.AppendLine("SELECT Param_Name, Param_Value ");
                    SBSql.AppendLine(" FROM Param_Public ");
                    SBSql.AppendLine(" WHERE (Param_Kind = '符號使用者')");
                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.Clear();
                    using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
                    {
                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            setMenu.Items.Add(new ListItem(
                                DT.Rows[row]["Param_Name"].ToString()
                                , DT.Rows[row]["Param_Value"].ToString()));
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

        #region "其他"
        /// <summary>
        /// jQuery - BlockUI
        /// </summary>
        /// <param name="validGroup">驗証控制項的群組名稱</param>
        /// <param name="inputTxt">輸入的Html</param>
        /// <returns>string</returns>
        public static string BlockJs(string validGroup, string inputTxt)
        {
            StringBuilder BlockJs = new StringBuilder();
            BlockJs.Append("if(Page_ClientValidate_AllPass('" + validGroup + "')) {");
            BlockJs.Append("$(function () {");
            BlockJs.Append(" $.blockUI({");
            BlockJs.Append("  message: '" + inputTxt + "',");
            BlockJs.Append("  css: {");
            BlockJs.Append("   width: '250px',");
            BlockJs.Append("   border: 'none',");
            BlockJs.Append("   padding: '5px',");
            BlockJs.Append("   backgroundColor: '#000',");
            BlockJs.Append("   '-webkit-border-radius': '10px',");
            BlockJs.Append("   '-moz-border-radius': '10px',");
            BlockJs.Append("   opacity: .8,");
            BlockJs.Append("   color: '#fff'");
            BlockJs.Append("  }");
            BlockJs.Append(" });");
            BlockJs.Append("});");
            BlockJs.Append("}");
            return BlockJs.ToString();
        }

        /// <summary>
        /// Javascript - Alert & Location
        /// </summary>
        /// <param name="alertMessage">警示訊息</param>
        /// <param name="hrefUrl">導向頁面或JS語法</param>
        /// <remarks>
        /// 使用JS語法須加入 script: 以判斷
        /// </remarks>
        public static void JsAlert(string alertMessage, string hrefUrl)
        {
            try
            {
                StringBuilder sbJs = new StringBuilder();
                //警示訊息
                if (false == string.IsNullOrEmpty(alertMessage))
                {
                    sbJs.Append(string.Format("alert('{0}');", alertMessage));
                }
                //判斷是頁面還是語法
                if (false == string.IsNullOrEmpty(hrefUrl))
                {
                    if (hrefUrl.IndexOf("script:") != -1)
                    {
                        sbJs.Append(hrefUrl.Replace("script:", ""));
                    }
                    else
                    {
                        sbJs.Append(string.Format("location.href=\'{0}\';", hrefUrl));
                    }
                }
                ScriptManager.RegisterClientScriptBlock((Page)HttpContext.Current.Handler, typeof(string), "js", sbJs.ToString(), true);
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// SQL參數組合 - Where IN
        /// </summary>
        /// <param name="listSrc">來源資料(List)</param>
        /// <param name="paramName">參數名稱</param>
        /// <returns>參數字串</returns>
        public static string GetSQLParam(List<string> listSrc, string paramName)
        {
            if (listSrc.Count == 0)
            {
                return "";
            }

            //組合參數字串
            ArrayList aryParam = new ArrayList();
            for (int row = 0; row < listSrc.Count; row++)
            {
                aryParam.Add(string.Format("@{0}{1}", paramName, row));
            }
            //回傳以 , 為分隔符號的字串
            return string.Join(",", aryParam.ToArray());
        }
        #endregion

    }

}