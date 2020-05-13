using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace ExtensionIO
{
    /// <summary>
    /// 自訂的IO處理
    /// </summary>
    public class fn_CustomIO
    {
        /// <summary>
        /// 壓縮資料夾至指定目錄 (.zip)
        /// </summary>
        /// <param name="sourceFolder">來源資料夾(UNC或本機實體路徑)</param>
        /// <param name="targetFolder">目標資料夾(UNC或本機實體路徑)</param>
        /// <param name="targetFileName">目標檔案名稱</param>
        /// <returns></returns>
        /// <example>
        /// sourceFolder = @"\\PKRC9\PKResource\ProductPic\MT-8100\1"
        /// targetFolder = @"\\PKRC9\PKResource\ProductPic_Zip\"
        /// targetFileName = "MT-8100_1.zip"
        /// </example>
        public static bool exec_ZipFiles(string sourceFolder, string targetFolder, string targetFileName)
        {
            try
            {
                //目標檔案
                string targetFile = targetFolder + targetFileName;

                //判斷來源資料夾是否存在
                DirectoryInfo CheckFolder = new DirectoryInfo(sourceFolder);
                if (CheckFolder.Exists == false)
                {
                    //來源不存在,刪除目前檔案
                    if (fn_CustomIO.CheckFile(targetFile))
                    {
                        IOManage.DelFile(targetFolder, targetFileName);
                    }

                    return false;
                }

                //判斷來源資料夾內是否還有檔案
                if (CheckFolder.GetFiles().Count() == 0)
                {
                    //沒檔案了，刪除目標壓縮檔(舊的壓縮檔)
                    IOManage.DelFile(targetFolder, targetFileName);
                    return true;
                }

                //判斷目標資料夾是否存在
                if (fn_CustomIO.CheckFolder(targetFolder))
                {
                    //若目標壓縮檔存在,則刪除壓縮檔,之後再建立新的壓縮檔
                    if (fn_CustomIO.CheckFile(targetFile))
                    {
                        IOManage.DelFile(targetFolder, targetFileName);
                    }

                    //壓縮檔案(來源資料夾, 目標檔案名稱, 壓縮最佳化, 壓縮時不含目錄名稱)
                    ZipFile.CreateFromDirectory(sourceFolder, targetFile, CompressionLevel.Optimal, false);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

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

        /// <summary>
        /// IO - 判斷目標檔案是否存在
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool CheckFile(string filepath)
        {
            try
            {
                return File.Exists(filepath);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }


    public static class IOManage
    {
        //副檔名
        private static string _FileExtend;
        public static string FileExtend
        {
            get;
            private set;
        }
        //檔案名稱 - 原始檔名
        private static string _FileFullName;
        public static string FileFullName
        {
            get;
            private set;
        }
        //檔案名稱 - 系統命名
        private static string _FileNewName;
        public static string FileNewName
        {
            get;
            private set;
        }
        //檔案真實路徑
        private static string _FileRealName;
        private static string FileRealName
        {
            get;
            set;
        }
        //處理訊息
        private static string _Message;
        public static string Message
        {
            get;
            private set;
        }

        private static int idx = 0;

        /// <summary>
        /// 取得相關檔案名稱
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        public static void GetFileName(HttpPostedFile hpFile)
        {
            try
            {
                if (hpFile.ContentLength != 0)
                {
                    //[IO] - 檔案真實路徑
                    FileRealName = hpFile.FileName;
                    //[IO] - 取得副檔名(.xxx)
                    FileExtend = Path.GetExtension(FileRealName);
                    //[IO] - 檔案重新命名
                    idx += 1;
                    FileNewName = String.Format("{0:yyyyMMddHHmmssfff}", DateTime.Now) + Convert.ToString(idx) + FileExtend;
                    //[IO] - 取得完整檔名
                    FileFullName = Path.GetFileName(FileRealName);

                    Message = "OK";
                }
                else
                {
                    FileExtend = null;
                    FileFullName = null;
                    FileNewName = null;
                    FileRealName = null;
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - GetFileName";
            }
        }

        /// <summary>
        /// 儲存檔案
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                {
                    //判斷資料夾是否存在
                    if (fn_CustomIO.CheckFolder(FileFolder))
                    {
                        hpFile.SaveAs(FileFolder + newFileName);
                        Message = "OK";
                    }
                    else
                    {
                        Message = "資料夾無法建立，檔案上傳失敗。";
                    }
                }
                else
                {
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - Save";
            }

        }

        /// <summary>
        /// 儲存檔案, 使用縮圖
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        /// <param name="intWidth">指定寬度</param>
        /// <param name="intHeight">指定高度</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName, int intWidth, int intHeight)
        {
            Save(hpFile, FileFolder, newFileName, intWidth, intHeight, "");
        }

        /// <summary>
        /// 儲存檔案, 使用縮圖
        /// </summary>
        /// <param name="hpFile">FileUpload</param>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="newFileName">檔案名稱</param>
        /// <param name="intWidth">指定寬度</param>
        /// <param name="intHeight">指定高度</param>
        /// <param name="waterImg">浮水印路徑</param>
        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName, int intWidth, int intHeight
            , string waterImg)
        {
            try
            {
                if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                {
                    string fileUrl = FileFolder + newFileName;

                    //判斷資料夾是否存在
                    if (fn_CustomIO.CheckFolder(FileFolder))
                    {
                        //儲存原始圖檔
                        hpFile.SaveAs(fileUrl);

                        //產生縮圖並覆蓋原始圖檔
                        renderThumb(fileUrl, fileUrl, intWidth, intHeight, waterImg);

                        Message = "OK";
                    }
                    else
                    {
                        Message = "資料夾無法建立，檔案上傳失敗。";
                    }
                }
                else
                {
                    Message = "";
                }
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - Save";
            }

        }


        public static void Save(HttpPostedFile hpFile, string FileFolder, string newFileName, int intWidth, int intHeight
           , string waterImg, bool isResize) {
               try
               {
                   if (string.IsNullOrEmpty(newFileName) == false || hpFile.ContentLength != 0)
                   {
                       string fileUrl = FileFolder + newFileName;

                       //判斷資料夾是否存在
                       if (fn_CustomIO.CheckFolder(FileFolder))
                       {
                           //儲存原始圖檔
                           hpFile.SaveAs(fileUrl);

                           //產生縮圖並覆蓋原始圖檔
                           if (isResize && !string.IsNullOrEmpty(waterImg))
                           {
                               renderThumb(fileUrl, fileUrl, intWidth, intHeight, waterImg);
                           }

                           Message = "OK";
                       }
                       else
                       {
                           Message = "資料夾無法建立，檔案上傳失敗。";
                       }
                   }
                   else
                   {
                       Message = "";
                   }
               }
               catch (Exception)
               {
                   Message = "系統發生錯誤 - Save";
               }
        }

        /// <summary>
        /// 產生並儲存縮圖
        /// </summary>
        /// <param name="inputImg">來源圖檔路徑(磁碟路徑)</param>
        /// <param name="outputImg">輸出圖檔路徑(磁碟路徑)</param>
        /// <param name="w">寬</param>
        /// <param name="h">高</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static void renderThumb(string inputImg, string outputImg, int w, int h)
        {
            renderThumb(inputImg, outputImg, w, h, "");
        }

        /// <summary>
        /// 有浮水印的圖
        /// </summary>
        /// <param name="inputImg">來源圖檔路徑(磁碟路徑)</param>
        /// <param name="outputImg">輸出圖檔路徑(磁碟路徑)</param>
        /// <param name="w">寬</param>
        /// <param name="h">高</param>
        /// <param name="waterImg">浮水印路徑</param>
        private static void renderThumb(string inputImg, string outputImg, int w, int h
            , string waterImg)
        {
            //取得原始圖檔
            //Image image = new Bitmap(inputImg);
            Bitmap image = (Bitmap)Bitmap.FromFile(inputImg);
            
            //原始圖格式
            ImageFormat thisFormat = image.RawFormat;

            //取得圖檔寬高
            int width = 0;
            int height = 0;
            width = image.Width;
            height = image.Height;

            //重新設定寬高 (等比例)
            if (!(width < w & height < h))
            {
                if (width > height)
                {
                    h = w * height / width;
                }
                else
                {
                    w = h * width / height;
                }
            }
            else
            {
                h = height;
                w = width;
            }

            //產生縮圖
            Bitmap imageOutput = new Bitmap(image, w, h);
         
            //取得新縮圖
            Graphics graphic = Graphics.FromImage(imageOutput);

            //graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //浮水印圖檔附加至新的縮圖
            if (!string.IsNullOrEmpty(waterImg))
            {
                //取得浮水印
                Image watermarkImage = Image.FromFile(waterImg);
                //設定DPI
                imageOutput.SetResolution(graphic.DpiX, graphic.DpiY);

                //開始畫圖
                graphic.DrawImage(watermarkImage    //浮水印圖檔
                   , new Rectangle(
                       0 //imageOutput.Width - watermarkImage.Width    //浮水印 x 座標
                       , 0 //imageOutput.Height - watermarkImage.Height    //浮水印 y 座標
                       , imageOutput.Width, imageOutput.Height)
                   , 0, 0, imageOutput.Width, imageOutput.Height, GraphicsUnit.Pixel);
            }


            //釋放資源(先釋放)
            image.Dispose();

            //輸出縮圖
            imageOutput.Save(outputImg, thisFormat);

            //釋放資源
            imageOutput.Dispose();
            graphic.Dispose();
        }


        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="FileFolder">資料夾路徑</param>
        /// <param name="oldFileName">檔案名稱</param>
        public static void DelFile(string FileFolder, string oldFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(FileFolder) || string.IsNullOrEmpty(oldFileName))
                {
                    Message = "傳入參數空白";
                    return;
                }
                FileInfo FileDelete = new FileInfo(FileFolder + oldFileName);
                if (FileDelete.Exists)
                    FileDelete.Delete();

                Message = "OK";
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - DelFile";
            }
        }

        /// <summary>
        /// 刪除資料夾
        /// </summary>
        /// <param name="FileFolder">資料夾名稱</param>
        public static void DelFolder(string FileFolder)
        {
            try
            {
                if (string.IsNullOrEmpty(FileFolder))
                {
                    Message = "傳入參數空白";
                    return;
                }

                string[] strTemp = null;
                int idx = 0;
                // 刪除檔案
                strTemp = Directory.GetFiles(FileFolder);
                for (idx = 0; idx < strTemp.Length; idx++)
                {
                    if (File.Exists(strTemp[idx]))
                        File.Delete(strTemp[idx]);
                }
                // 刪除子目錄
                strTemp = Directory.GetDirectories(FileFolder);
                for (idx = 0; idx < strTemp.Length; idx++)
                {
                    //呼叫 DelFolder
                    DelFolder(strTemp[idx]);
                }
                // 刪除該目錄
                System.IO.Directory.Delete(FileFolder);

                Message = "OK";
            }
            catch (Exception)
            {
                Message = "系統發生錯誤 - DelFolder";
            }
        }
    }

}
