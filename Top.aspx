<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Top.aspx.cs" Inherits="Top" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        function hideMenu() {
            parent.document.getElementById('DownFrame').cols = '0,*';
        }

        function ShowMenu() {
            parent.document.getElementById('DownFrame').cols = '176,*';
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="Header">
            <div class="SystemLogo">
                <a href="<%=Application["WebUrl"] %>" target="_top">
                    <%=Navi_產品中心%></a>
            </div>
            <!--快速選單-->
            <div class="TopFastLink">
                <ul>
                    <li class="Fast6"><a href="http://pkef.prokits.com.tw/" target="_blank">Prokits Intranet</a></li>
                    <li class="Fast3"><a href="http://pk711.prokits.com.tw/" target="_blank">專案服務區</a></li>
                    <li>&nbsp;</li>
                </ul>
            </div>
            <div class="SysInfoControlCon">
                <div class="MenuFirstControl">
                    <a class="Menu12Hide" href="javascript:hideMenu();" title="隱藏選單">隱藏選單</a> <a class="Menu12Show"
                        href="javascript:ShowMenu();" title="展開選單">展開選單</a>
                </div>
                <div class="FastPageBack">
                    <asp:DropDownList ID="ddl_CorpID" runat="server" Enabled="false" Visible="false">
                        <asp:ListItem>公司別</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddl_Language" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Language_SelectedIndexChanged"
                        Enabled="false" Visible="false">
                    </asp:DropDownList>
                    <a href="Main.aspx" target="mainFrame">
                        <%=Navi_回首頁%></a>&nbsp;|&nbsp;<asp:LinkButton ID="lbtn_Logout" runat="server" CausesValidation="false"
                            CssClass="B" OnClick="lbtn_Logout_Click">登出</asp:LinkButton>&nbsp;|&nbsp;<%=fn_Param.CurrentAccount %> (<%=fn_Param.UserAccountName %>)
                </div>
            </div>
        </div>
    </form>
</body>
</html>
