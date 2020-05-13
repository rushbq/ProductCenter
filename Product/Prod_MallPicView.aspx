<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_MallPicView.aspx.cs" Inherits="Prod_MallPic" %>

<%@ Register Src="Ascx_TabMenu_View.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- fancybox helpers Start --%>
    <script src="../js/fancybox/helpers/jquery.fancybox-thumbs.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-thumbs.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <script>
        $(function () {
            //Click事件 - 一般欄位click後全選
            $(".url").click(function () {
                $(this).select();
            });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'elastic',
                nextEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    },
                    buttons: {}
                }
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
            <h2>商城輔圖 -
                <span class="styleRed B"><%=Param_ModelNo %></span>
            </h2>
        </div>
        <div class="SysTab3" style="margin-bottom:10px;">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <div class="row">
            <div class="col-xs-6 text-left">
                <h4>圖片列表</h4>
            </div>
            <div class="col-xs-6 text-right">
                <asp:Literal ID="lt_DownloadBtn" runat="server"></asp:Literal>
            </div>
        </div>
        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <tr class="tdHead">
                            <td width="60%">圖片</td>
                            <td width="40%">說明</td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server">
                        <td align="center">
                            <a href="<%=Param_WebFolder %><%# Eval("PicFile")%>" class="PicGroup" rel="gallery">
                                <img src="<%=Param_WebFolder %><%# Eval("PicFile")%>" class="img-thumbnail img-responsive" alt="" />
                            </a>
                        </td>
                        <td align="center">
                            <div class="form-group">
                                <div class="col-xs-12">
                                    <%# Eval("PicDesc").ToString().Replace("\n","<br>")%>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-xs-12">
                                    <input type="text" class="form-control url" value="<%=Param_WebFolder %><%# Eval("PicFile")%>" readonly="readonly" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div style="margin: 0 auto; text-align: center" class="styleRed">
                        尚未新增圖片...
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>

    </form>
</body>
</html>
