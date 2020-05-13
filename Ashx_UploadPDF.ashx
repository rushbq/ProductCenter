<%@ WebHandler Language="C#" Class="UploadPDF" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

/// <summary>
/// 20171023 移除不使用
/// </summary>
/// <remarks>
/// 1) 使用前，記得調整 webconfig 的上傳限制
/// 上傳大小(預設值 4096KB<4 MB>, 上限 2097151KB<2 GB>, 目前 100 MB, 單位KB, 預設90秒)
/// <httpRuntime maxRequestLength="102400" executionTimeout="1200" requestValidationMode="2.0"/>
/// 2) Client端須安裝Flash Player
/// </remarks>
public class UploadPDF : IHttpHandler
{
    //檔案大小上限(KB)
    int _fileMaxSize = 40960;
    //可上傳之檔案類型
    string[] _fileAgreeType = {
	        ".pdf"
        };
    
    //亂數
    Random rnd = new Random();
        
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        //由於uploadify的flash是採用utf-8的編碼方式，所以上傳頁面也要用utf-8編碼，才能正常上傳中文檔名的文件
        context.Request.ContentEncoding = Encoding.GetEncoding("UTF-8");
        context.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
        context.Response.Charset = "UTF-8";

        //開始上傳
        string result = "";
        if (IsPostFile())
        {
            result = SaveRequestFiles();
        }
        context.Response.Write(result);
    }

    //判斷是否有需上傳的檔案
    public static bool IsPostFile()
    {
        if (HttpContext.Current.Request.Files.Count == 0)
        {
            return false;
        }

        for (int i = 0; i <= HttpContext.Current.Request.Files.Count; i++)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Files[i].FileName))
            {
                return true;
            }
        }
        return false;
    }

    //檢查檔案格式是否符合要求
    private bool CheckFileExt(string _fileExt)
    {
        bool fileAllow = false;
        //旗標
        for (int i = 0; i <= _fileAgreeType.Length - 1; i++)
        {
            if (_fileExt == _fileAgreeType[i])
            {
                fileAllow = true;
                break; // TODO: might not be correct. Was : Exit For
            }
        }
        if (fileAllow)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //
    //檢查檔案大小是否超過限制
    private bool CheckFileSize(int _fileSize)
    {
        if ((_fileSize / 1024) > _fileMaxSize)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //儲存上傳的檔案
    public string SaveRequestFiles()
    {
        string result = "";
        int fCount = HttpContext.Current.Request.Files.Count;
        for (int i = 0; i <= fCount - 1; i++)
        {
            //取得檔案資訊
            System.IO.FileInfo file = new System.IO.FileInfo(HttpContext.Current.Request.Files[i].FileName);
            //取得檔案名稱
            string fileName = file.Name;
            //取得檔案附檔名
            string fileExtension = file.Extension.ToLower();
            //取得檔案類型
            string fileType = HttpContext.Current.Request.Files[i].ContentType.ToLower();
            //取得檔案大小
            int fileSize = HttpContext.Current.Request.Files[i].ContentLength;
            //設定新日期檔案名稱
            string tmpFileName = "";

            //檢查檔案大小
            if (CheckFileSize(fileSize))
            {
                //檢查檔案格式
                if (CheckFileExt(fileExtension))
                {
                    //取得檔案真實路徑
                    //string UploadDir = HttpContext.Current.Server.MapPath(Param_FilePath);

                    //建立新檔案完整名稱(日期_SessionID + 附檔名)
                    tmpFileName = string.Format("{0}{1}_{2}{3}"
                        , String.Format("{0:yyMMddHHmmssfff}", DateTime.Now)
                        , rnd.Next(0, 99)
                        , Param_SessionID
                        , fileExtension);

                    //建立檔案檢查路徑
                    string fileToCheck = Param_FilePath + tmpFileName;

                    //檢查目錄是否存在，不存在則建立目錄
                    if (!System.IO.Directory.Exists(Param_FilePath))
                    {
                        System.IO.Directory.CreateDirectory(Param_FilePath);
                    }
                    //儲存檔案
                    HttpContext.Current.Request.Files[i].SaveAs(Param_FilePath + tmpFileName);
                    //回傳檔案名稱
                    result = tmpFileName;
                }
                else
                {
                    //result = "檔案格式不符合"
                    result = "999";
                }
            }
            else
            {
                //result = "檔案大小超過限制"
                result = "998";
            }
        }
        return result;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// 資料夾名稱
    /// </summary>
    private string _Param_FolderName;
    public string Param_FolderName
    {
        get
        {
            return (string.IsNullOrEmpty(HttpContext.Current.Request["folder"])) ? "" : HttpContext.Current.Request["folder"].ToString();
        }
        private set
        {
            this._Param_FolderName = value;
        }
    }

    /// <summary>
    /// 檔案存放位置
    /// </summary>
    private string _Param_FilePath;
    public string Param_FilePath
    {
        get
        {
            return string.Format(@"{0}{1}\", System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"], Param_FolderName);
        }
        private set
        {
            this._Param_FilePath = value;
        }
    }

    /// <summary>
    /// SessionID
    /// </summary>
    private string _Param_SessionID;
    public string Param_SessionID
    {
        get
        {
            return (string.IsNullOrEmpty(HttpContext.Current.Request["sessionID"])) ? "" : HttpContext.Current.Request["sessionID"].ToString();
        }
        private set
        {
            this._Param_SessionID = value;
        }
    }
}