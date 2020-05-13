<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Map.aspx.cs" Inherits="Spec_Map" %>

<%@ Register Src="Ascx_QuickMenu.ascx" TagName="Ascx_QuickMenu" TagPrefix="uc1" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
    <%-- fancybox Start --%>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <%-- fancybox Start --%>
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $("#TreeView").treeview({
                collapsed: false,
                animated: "fast",
                control: "#sidetreecontrol"
            });

            //Tooltip
            $(".tooltip").poshytip({
                className: 'tip-darkgray',
                bgImageFrameSize: 9,
                offsetX: -10,
                offsetY: 10

            });

            /* fancybox */
            $(".EditBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>規格總覽</span>
    </div>
    <div class="h2Head">
        <h2>
            規格總覽</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div id="sidetreecontrol" class="MenuSecControl" style="padding-top: 5px">
        <a href="?#">收合</a> | <a href="?#">展開</a> | <a href="#" onclick="location.reload();">
            重整</a> |
        <asp:DropDownList ID="ddl_Class" runat="server" OnSelectedIndexChanged="ddl_Class_SelectedIndexChanged"
            AutoPostBack="true">
        </asp:DropDownList>
    </div>
    <hr class="MenuSecondHr" />
    <div class="MenuSecond">
        <asp:Literal ID="lt_TreeView" runat="server"></asp:Literal>
    </div>
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="N"
        ShowTop="Y" ShowBottom="Y" />
    <!-- 備註說明 -->
    <div class="ListIllusArea">
        <div class="JQ-ui-state-highlight">
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>以樹狀模式，顯示目前的規格設定架構</div>
        </div>
    </div>
    </form>
</body>
</html>
