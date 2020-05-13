<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_BOM_DtlView.aspx.cs"
    Inherits="Prod_BOM_DtlView" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">
                <%=Navi_系統首頁%></a>&gt;<a><%=Navi_產品資料庫%></a>&gt;<span><%=Navi_產品資料%></span>
        </div>
        <div class="h2Head">
            <h2>
                <div style="float: left">
                    <asp:Label ID="lb_CateName" runat="server"></asp:Label>,
                <asp:Label ID="lb_ModelNo" runat="server"></asp:Label>,
                <asp:Label ID="lb_SpecInfo" runat="server"></asp:Label>
                    | 組合明細
                </div>
                <div style="float: right; padding-right: 5px;">
                    <a class="btn btn-info" href="Prod_DtlView.aspx?Model_No=<%= Server.UrlEncode(Param_ModelNo) %>">回上一層</a>
                    <asp:PlaceHolder ID="ph_Url" runat="server" Visible="false">
                        <a class="btn btn-warning" href="Prod_BOM_DtlEdit.aspx?Model_No=<%=Server.UrlEncode(Param_ModelNo) %>&CateID=<%=Param_CateID %>&SpecClassID=<%=Param_SpecClassID %>&SpecID=<%=Param_SpecID %>">回編輯頁</a>
                    </asp:PlaceHolder>
                </div>
            </h2>
        </div>
        <div class="SysTab3">
            <ul>
                <li><a href="Prod_View.aspx?Model_No=<%=Server.UrlEncode(Param_ModelNo) %>">主檔資料</a></li>
                <li class="TabAc">
                    <a href="Prod_BOM_DtlView.aspx?Model_No=<%=Server.UrlEncode(Param_ModelNo) %>&CateID=<%=Param_CateID %>&SpecClassID=<%=Param_SpecClassID %>&SpecID=<%=Param_SpecID %>">規格明細 -&gt; 組合明細</a>
                </li>
            </ul>
        </div>
        <table class="TableModify">
            <tr>
                <td class="TableModifyTd">
                    <div class="table-responsive">
                        <table class="table table-bordered table-striped">
                            <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
                        </table>
                    </div>
                </td>
            </tr>

        </table>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="N"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
