<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test_PDF.aspx.cs" Inherits="Product_Test_PDF" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btn1" runat="server" Text="Get-PDF-HTML" OnClick="Button1_Click" />
            <asp:Button ID="btn2" runat="server" Text="Convert PDF" OnClick="Button2_Click" />
        </div>
        <hr />
        <div>
            <asp:TextBox ID="TextBox1" runat="server" TextMode="MultiLine" Rows="30" Columns="200"></asp:TextBox>
        </div>
    </form>
</body>
</html>
