using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

/// <summary>
/// AES 加解密
/// </summary>
public class Cryptograph
{
    #region -- AES --
    private static readonly String strAesKey = "8AFB8F5039EE9D3BEA527F07DD54CA60";  //設定密鑰

    /// <summary>
    /// AES - 加密函式
    /// </summary>
    /// <param name="EncryptString">欲加密字串</param>
    /// <returns></returns>
    public static string Encrypt(string EncryptString)
    {
        try
        {
            //密碼轉譯一定都是用byte[] 所以把string都換成byte[]
            byte[] byte_secretContent = Encoding.UTF8.GetBytes(EncryptString);
            byte[] byte_pwd = Encoding.UTF8.GetBytes(strAesKey);

            //加解密函數的key通常都會有固定的長度 而使用者輸入的key長度不定 因此用hash過後的值當做key
            MD5CryptoServiceProvider provider_MD5 = new MD5CryptoServiceProvider();
            byte[] byte_pwdMD5 = provider_MD5.ComputeHash(byte_pwd);

            //產生加密實體 如果要用其他不同的加解密演算法就改這裡(ex:3DES)
            RijndaelManaged provider_AES = new RijndaelManaged();
            ICryptoTransform encrypt_AES = provider_AES.CreateEncryptor(byte_pwdMD5, byte_pwdMD5);

            //output就是加密過後的結果
            byte[] output = encrypt_AES.TransformFinalBlock(byte_secretContent, 0, byte_secretContent.Length);

            return Convert.ToBase64String(output);
        }
        catch (Exception)
        {

            return "";
        }

    }

    /// <summary>
    /// AES - 解密函式
    /// </summary>
    /// <param name="DecryptString">欲解密字串</param>
    /// <returns></returns>
    public static string Decrypt(string DecryptString)
    {
        try
        {
            byte[] byte_ciphertext = Convert.FromBase64String(DecryptString);
            //密碼轉譯一定都是用byte[] 所以把string都換成byte[]
            byte[] byte_pwd = Encoding.UTF8.GetBytes(strAesKey);

            //加解密函數的key通常都會有固定的長度 而使用者輸入的key長度不定 因此用hash過後的值當做key
            MD5CryptoServiceProvider provider_MD5 = new MD5CryptoServiceProvider();
            byte[] byte_pwdMD5 = provider_MD5.ComputeHash(byte_pwd);

            //產生解密實體
            RijndaelManaged provider_AES = new RijndaelManaged();
            ICryptoTransform decrypt_AES = provider_AES.CreateDecryptor(byte_pwdMD5, byte_pwdMD5);

            //string_secretContent就是解密後的明文
            byte[] byte_secretContent = decrypt_AES.TransformFinalBlock(byte_ciphertext, 0, byte_ciphertext.Length);
            string string_secretContent = Encoding.UTF8.GetString(byte_secretContent);

            return string_secretContent;
        }
        catch (Exception)
        {
            return "";
        }

    }
    #endregion

    #region -- MD5 --
    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="InputString">輸入字串</param>
    /// <returns>string</returns>
    public static string MD5(string InputString)
    {
        //MD5計算Function,取自MSDN
        // 建立一個MD5物件
        System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

        // 將input轉換成MD5，並且以Bytes傳回，由於ComputeHash只接受Bytes型別參數，所以要先轉型別為Bytes
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(InputString));

        // 建立一個StringBuilder物件
        StringBuilder sBuilder = new StringBuilder();

        // 將Bytes轉型別為String，並且以16進位存放
        int i = 0;
        for (i = 0; i <= data.Length - 1; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        //傳回
        return sBuilder.ToString();
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="InputString">輸入字串</param>
    /// <param name="code">16/32位元</param>
    /// <returns>string</returns>
    public static string MD5(string InputString, int code)
    {
        if (code == 16)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(InputString, "MD5").ToUpper().Substring(8, 16);
        }
        if (code == 32)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(InputString, "MD5").ToUpper();
        }
        return "00000000000000000000000000000000";
    }

    /// <summary>
    /// MD5 - 加密 (需搭配DES)
    /// </summary>
    /// <param name="pToDecrypt">輸入字串</param>
    /// <param name="sKey">DES Key(8個英文字) </param>
    public static string MD5Encrypt(string pToEncrypt, string sKey)
    {
        try
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);

            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            MemoryStream ms = new MemoryStream();

            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            ret.ToString();

            return ret.ToString();
        }
        catch (Exception)
        {
            return "";
        }

    }

    /// <summary>
    /// MD5 - 解密 (需搭配DES)
    /// </summary>
    /// <param name="pToDecrypt">輸入字串</param>
    /// <param name="sKey">DES Key(8個英文字) </param>
    /// <returns></returns>
    public static string MD5Decrypt(string pToDecrypt, string sKey)
    {
        try
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];

            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));

                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);

            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            MemoryStream ms = new MemoryStream();

            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        catch (Exception)
        {
            return "";
        }

    }

    #endregion
}