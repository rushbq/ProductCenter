<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_DtlView.aspx.cs" Inherits="Prod_DtlView" %>

<%@ Register Src="Ascx_TabMenu_View.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //資料展開/收合
            $(".DTtoggle").click(function () {
                //取得此物件的rel屬性值 (#xx)
                var _this = $(this).attr("rel");
                var _img = $(this).attr("imgrel");
                //判斷指定元素是否隱藏
                if ($(_this).css("display") == "none") {
                    $(this).attr("title", "收合");
                    $(_this).show();
                    $(_img).attr("src", "../images/icon_top.png");
                } else {
                    $(this).attr("title", "展開");
                    $(_this).hide();
                    $(_img).attr("src", "../images/icon_down.png");
                }
                return false;
            });

            //導向連結
            $("input[type=button].goUrl").click(function () {
                location.href = $(this).attr('rel');
            });
        });
    </script>
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
                    <%=Navi_產品資料%> -
                    <asp:Label ID="lb_Model_No" runat="server" CssClass="styleRed B"><%=Param_ModelNo %></asp:Label>
                </div>
                <div style="float: right; padding-right: 5px;">
                    <asp:PlaceHolder ID="ph_Url" runat="server" Visible="false">
                        <input type="button" class="goUrl btnBlock colorDark" value="返回編輯頁" rel="Prod_DtlEdit.aspx?Model_No=<%=Server.UrlEncode(Param_ModelNo) %>" />
                    </asp:PlaceHolder>
                </div>
            </h2>
        </div>
        <div class="SysTab">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <table class="TableModify" style="line-height: 22px;">
            <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
        </table>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
