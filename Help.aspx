<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
        管理後台</title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <!-- 認證資料庫 -->
    <asp:Panel ID="pl_Cert" runat="server" Visible="false" HorizontalAlign="Center">
        <div class="styleReddark B Font13" style="padding: 5px 0px 10px 0px">
            [品號搜尋使用方式]</div>
        <div>
            <img src="<%=Application["File_WebUrl"] %>QA/Certification/01.jpg" />
        </div>
        <div class="styleBlue Font15" style="height: 30px">
            ↓↓↓↓↓↓↓↓
        </div>
        <div>
            <img src="<%=Application["File_WebUrl"] %>QA/Certification/02.jpg" />
        </div>
    </asp:Panel>
    <!-- 產品圖片(ProdPic_Group) -->
    <asp:Panel ID="pl_PicGroup" runat="server" Visible="false" HorizontalAlign="Center">
        <div class="styleReddark B Font13" style="padding: 5px 0px 10px 0px">
            [多筆上傳使用方式]</div>
        <div>
            <img src="<%=Application["File_WebUrl"] %>QA/PicGroup/01.jpg" width="450" />
        </div>
        <div class="styleBlue Font15" style="height: 30px">
            ↓↓↓↓↓↓↓↓
        </div>
        <div>
            <img src="<%=Application["File_WebUrl"] %>QA/PicGroup/02.jpg" />
        </div>
        <div class="styleBlue Font15" style="height: 30px">
            ↓↓↓↓↓↓↓↓
        </div>
        <div>
            <img src="<%=Application["File_WebUrl"] %>QA/PicGroup/03.jpg" width="450" />
        </div>
        <div class="styleBlue Font15" style="height: 30px">
            ↓↓↓↓↓↓↓↓
        </div>
        <div>
            <img src="<%=Application["File_WebUrl"] %>QA/PicGroup/04.jpg" />
        </div>
    </asp:Panel>
    <!-- 其他 -->
    <asp:Panel ID="pl_uc" runat="server" Visible="false">
        <div style="text-align: center">
            <img src="images/under_construction.png" />
        </div>
    </asp:Panel>
    <div style="text-align: center; padding-top: 20px;">
        <input type="button" value="關閉視窗" onclick="parent.jQuery.fancybox.close();" />
    </div>
    </form>
</body>
</html>
