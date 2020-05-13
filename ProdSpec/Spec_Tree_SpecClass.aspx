<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Tree_SpecClass.aspx.cs"
    Inherits="Spec_Rel_SpecClass" %>

<%@ Register Src="Ascx_QuickMenu.ascx" TagName="Ascx_QuickMenu" TagPrefix="uc1" %>
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
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $("#TreeView").treeview({
                collapsed: false,
                animated: "medium",
                control: "#sidetreecontrol"
            });
        });
    </script>
</head>
<body class="MenuSecondPage">
    <form id="form1" runat="server">
    <div id="sidetreecontrol" class="MenuSecControl">
        <a href="?#">收合</a> | <a href="?#">展開</a> | <a href="#" onclick="location.reload();">
            重整</a>
    </div>
    <hr class="MenuSecondHr" />
    <div class="MenuSecond">
        <asp:Literal ID="lt_TreeView" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
