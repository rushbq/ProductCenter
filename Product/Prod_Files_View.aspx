<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_Files_View.aspx.cs" Inherits="Prod_Files_View" %>

<%@ Import Namespace="ExtensionMethods" %>
<%@ Register Src="Ascx_TabMenu_View.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.js"></script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">
                <%=Navi_系統首頁%></a>&gt;<a><%=Navi_產品資料庫%></a>&gt;<span><%=Navi_產品資料%></span>
        </div>
        <div class="h2Head">
            <h2>
                <%=Navi_產品資料%> -
                <span class="styleRed B"><%=Param_ModelNo %></span>
            </h2>
        </div>
        <div class="SysTab3">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <!-- Data List -->
        <div class="text-right" style="padding: 5px 0 5px 0">
            <a href="https://pkef.prokits.com.tw/?t=dwfiles" class="btn btn-info" target="_blank">前往維護頁</a>
        </div>
        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <tr class="tdHead">
                            <td>下載對象</td>
                            <td>檔案</td>
                            <td>最後更新</td>
                            <td>&nbsp;</td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server">
                        <td align="center"><%#Eval("TargetName") %></td>
                        <td align="left">
                            <span class="label label-danger"><%#Eval("Class_Name") %></span>
                            <span class="label label-warning"><%#Eval("LangName") %></span>
                            <span class="label label-info"><%#Eval("FileTypeName") %></span>
                            <h5><strong><%#Eval("DisplayName") %></strong></h5>
                        </td>
                        <td align="center">
                            <%#Eval("MtTime").ToString().ToDateString("yyyy-MM-dd HH:mm") %>
                        </td>
                        <td align="center">
                            <a href="<%=Application["WebUrl"] %>Ashx_FtpFileDownload.ashx?dwFolder=ProdFiles/<%#Eval("Class_ID") %>&realFile=<%#Eval("FileName") %>&dwFileName=<%#Eval("DisplayName") %>" class="btn btn-success">&nbsp;<i class="fa fa-cloud-download"></i>&nbsp;</a>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div style="padding: 30px 0;" class="text-center text-danger">
                        <h3>尚未新增檔案...</h3>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <asp:Panel ID="pl_Page" runat="server" CssClass="PagesArea" Visible="false">
            <div class="PageControlCon">
                <div class="PageControl">
                    <asp:Literal ID="lt_Page_Link" runat="server" EnableViewState="False"></asp:Literal>
                    <span class="PageSet">轉頁至
                    <asp:DropDownList ID="ddl_Page_List" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Page_List_SelectedIndexChanged">
                    </asp:DropDownList>
                        /
                    <asp:Literal ID="lt_TotalPage" runat="server" EnableViewState="False"></asp:Literal>
                        頁</span>
                </div>
                <div class="PageAccount">
                    <asp:Literal ID="lt_Page_DataCntInfo" runat="server" EnableViewState="False"></asp:Literal>
                </div>
            </div>
        </asp:Panel>

        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
