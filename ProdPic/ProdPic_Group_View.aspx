<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProdPic_Group_View.aspx.cs"
    Inherits="ProdPic_Group_View" %>

<%@ Import Namespace="ExtensionMethods" %>
<%@ Register Src="Ascx_ProdPicClass_View.ascx" TagName="Ascx_ProdPicClass" TagPrefix="uc1" %>
<%@ Register Src="Ascx_ProdData.ascx" TagName="Ascx_ProdData" TagPrefix="uc2" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
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
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'none',
                nextEffect: 'none',
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

            //Click事件 - 一般欄位click後全選
            $("input[type=text]").click(function () {
                $(this).select();
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>圖片資料庫</a>&gt;<span>圖片資料檢視</span>
    </div>
    <div class="h2Head">
        <h2>
            <%=Param_ModelNo %></h2>
    </div>
    <div class="SysTab">
        <uc1:Ascx_ProdPicClass ID="Ascx_ProdPicClass1" runat="server" />
    </div>
    <div>
        <!-- 基本資料 Start -->
        <uc2:Ascx_ProdData ID="Ascx_ProdData1" runat="server" />
        <!-- 基本資料 End -->
    </div>
    <table class="TableModify">
        <!-- 圖片資料 Start -->
        <tr class="ModifyHead">
            <td colspan="4">
                圖片資料<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr id="PicList">
            <td class="TableModifyTd" colspan="4">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                    <LayoutTemplate>
                        <table class="List1" width="100%">
                            <tr class="tdHead">
                                <td>
                                    圖片
                                </td>
                                <td width="100px" id="tdHeadLang" runat="server">
                                    語系
                                </td>
                                <td width="120px">
                                    更新日期
                                </td>
                                <td width="100px">
                                    排序
                                </td>
                                <td width="100px">
                                    功能選項
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="trItem" runat="server">
                            <td valign="top">
                                <table class="List2" width="100%">
                                    <tbody>
                                        <tr>
                                            <%#PicUrl(Eval("Pic_File").ToString(), Eval("Pic_OrgFile").ToString())%>
                                            <td class="L2Info styleGraylight">
                                                <div>
                                                    <%# Eval("Pic_Desc").ToString().Replace("\r\n","<BR>")%>
                                                </div>
                                                <div style="padding-top: 5px">
                                                    <%#DealerPicUrl(Eval("Pic_File").ToString())%></div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                            <td align="center" class="styleBlue" id="td_Lang" runat="server">
                                <asp:Literal ID="lt_Lang" runat="server"></asp:Literal>
                            </td>
                            <td align="center">
                                <%# Eval("LastTime").ToString().ToDateString("yyyy-MM-dd HH:mm")%>
                            </td>
                            <td align="center">
                                <%# Eval("Sort")%>
                            </td>
                            <td align="center">
                                <a href="<%#Application["WebUrl"] %>FileDownload.ashx?OrgiName=<%#Server.UrlEncode(Eval("Pic_OrgFile").ToString()) %>&FilePath=<%#Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + Eval("Pic_File").ToString())) %>"
                                    class="BtnTwo">下載</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div style="padding: 5px 5px 5px 5px" class="styleRed">
                            目前尚無資料...</div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </td>
        </tr>
        <!-- 圖片資料 End -->
    </table>
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
        ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
