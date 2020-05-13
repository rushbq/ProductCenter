<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Create_Xml.aspx.cs" Inherits="Create_Xml" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="產品類別XML" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" Text="認証類別XML" OnClick="Button2_Click" />
       <%-- <asp:Button ID="Button3" runat="server" Text="認証圖片XML" OnClick="Button3_Click" />--%>
        <asp:Button ID="Button4" runat="server" Text="產生Clyde的權限管理" OnClick="Button4_Click" />
        <asp:Button ID="Button5" runat="server" Text="產品欄位設定類別XML" OnClick="Button5_Click" />
    </div>
    </form>
</body>
</html>
