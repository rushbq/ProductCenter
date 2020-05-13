<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProdPic_Figure_View.aspx.cs"
    Inherits="ProdPic_Figure_View" %>

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
        <tr class="ModifyHead" id="PicList">
            <td colspan="4">
                圖片資料<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead" style="width: 100px">
                輔圖-1
            </td>
            <td class="TableModifyTd" style="width: 360px" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic01" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime01" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl01" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead" style="width: 100px">
                說明文字-1
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt01_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt01_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt01_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-2
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic02" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime02" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl02" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-2
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt02_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt02_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt02_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-3
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic03" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime03" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl03" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-3
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt03_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt03_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt03_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-4
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic04" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime04" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl04" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-4
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt04_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt04_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt04_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-5
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic05" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime05" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl05" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-5
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt05_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt05_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt05_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-6
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic06" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime06" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl06" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-6
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt06_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt06_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt06_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-7
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic07" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime07" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl07" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-7
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt07_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt07_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt07_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                輔圖-8
            </td>
            <td class="TableModifyTd" valign="top">
                <div style="padding: 5px 0px 5px 0px;">
                    <div>
                        <div style="float: left; width: 180px; text-align: center;">
                            <asp:Literal ID="lt_Pic08" runat="server"></asp:Literal>
                        </div>
                        <div style="float: left; text-align: left;">
                            <div class="styleGraylight" style="padding-top: 10px;">
                                <asp:Literal ID="lt_PicUpdTime08" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div style="clear: both; padding-top: 10px;">
                        <asp:Literal ID="lt_PicUrl08" runat="server"></asp:Literal>
                    </div>
                </div>
            </td>
            <td class="TableModifyTdHead">
                說明文字-8
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;<asp:Literal ID="lt_Txt08_zh_TW" runat="server"></asp:Literal></div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;<asp:Literal ID="lt_Txt08_en_US" runat="server"></asp:Literal></div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;<asp:Literal ID="lt_Txt08_zh_CN" runat="server"></asp:Literal></div>
            </td>
        </tr>
        <!-- 圖片資料 End -->
    </table>
    <div class="SubmitAreaS">
        <a href="<%=Session["BackListUrl"] %>" class="btnBlock colorGray">返回列表</a>
    </div>
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
        ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
