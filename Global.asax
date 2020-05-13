<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // 應用程式啟動時執行的程式碼
        Application["Web_Name"] = System.Web.Configuration.WebConfigurationManager.AppSettings["Web_Name"];
        Application["DiskUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"];
        Application["WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["WebUrl"];
        Application["File_DiskUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_DiskUrl"];
        Application["File_WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["File_WebUrl"];
        Application["DesKey"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DesKey"];
        Application["RefUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"];
        Application["CDN_Url"] = System.Web.Configuration.WebConfigurationManager.AppSettings["CDN_Url"];
        Application["API_WebUrl"] = System.Web.Configuration.WebConfigurationManager.AppSettings["API_WebUrl"];
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  應用程式關閉時執行的程式碼

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // 發生未處理錯誤時執行的程式碼

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // 啟動新工作階段時執行的程式碼

    }

    void Session_End(object sender, EventArgs e) 
    {
        // 工作階段結束時執行的程式碼。 
        // 注意: 只有在 Web.config 檔將 sessionstate 模式設定為 InProc 時，
        // 才會引發 Session_End 事件。如果將工作階段模式設定為 StateServer 
        // 或 SQLServer，就不會引發這個事件。

    }
    
    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        //[判斷參數] - 判斷Cookies是否存在
        if ((Request.Cookies["ProductCenter_Lang"] != null))
        {
            switch (Request.Cookies["ProductCenter_Lang"].Value.ToString())
            {
                case "zh-TW":
                    System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo("zh-TW");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;

                case "zh-CN":
                    currentInfo = new System.Globalization.CultureInfo("zh-CN");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;

                case "zh-US":
                    currentInfo = new System.Globalization.CultureInfo("en-US");
                    System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
                    break;
            }
        }
        else
        {
            //[判斷參數] - 新增預設語系(繁中)
            Response.Cookies.Add(new HttpCookie("ProductCenter_Lang", "zh-TW"));
            Response.Cookies["ProductCenter_Lang"].Expires = DateTime.Now.AddYears(1);
            System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo("zh-TW");
            System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
        }

    }
</script>
