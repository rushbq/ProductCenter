<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProdPic_Figure.aspx.cs" Inherits="ProdPic_Figure" %>

<%@ Register Src="Ascx_ProdPicClass.ascx" TagName="Ascx_ProdPicClass" TagPrefix="uc1" %>
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
            <a href="../Main.aspx">系統首頁</a>&gt;<a>圖片資料庫</a>&gt;<span>圖片資料維護</span>
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
                <td colspan="4">圖片資料<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTdHead" style="width: 100px">輔圖-1
                    </td>
                    <td class="TableModifyTd" style="width: 360px" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic01" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic01") %>">圖檔維護</a>

                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime01" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName01" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl01" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead" style="width: 100px">說明文字-1
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt01_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt01_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt01_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-2
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic02" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic02") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime02" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName02" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl02" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-2
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt02_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt02_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt02_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-3
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic03" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic03") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime03" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName03" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl03" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-3
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt03_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt03_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt03_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-4
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic04" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic04") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime04" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName04" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl04" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-4
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt04_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt04_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt04_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-5
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic05" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic05") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime05" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName05" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl05" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-5
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt05_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt05_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt05_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-6
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic06" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic06") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime06" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName06" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl06" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-6
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt06_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt06_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt06_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-7
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic07" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic07") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime07" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName07" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl07" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-7
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt07_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt07_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt07_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">輔圖-8
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic08" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic08") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime08" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName08" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl08" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">說明文字-8
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;<asp:TextBox ID="tb_Txt08_zh_TW" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;<asp:TextBox ID="tb_Txt08_en_US" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;<asp:TextBox ID="tb_Txt08_zh_CN" runat="server"
                                MaxLength="100" Width="300px"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- 圖片資料 End -->
        </table>
        <div class="SubmitArea">
            <asp:HiddenField ID="hf_PicID" runat="server" />
            <asp:Button ID="btn_Save" runat="server" Text="儲存" OnClick="btn_Save_Click" CssClass="btnBlock colorBlue" />
            <asp:Button ID="btn_DelAll" runat="server" Text="全部刪除" OnClick="btn_DelAll_Click"
                CausesValidation="false" CssClass="btnBlock colorRed" />
            <a href="<%=Session["BackListUrl"] %>" class="btnBlock colorGray">返回列表</a>
        </div>
        <table class="TableModify">
            <!-- 維護資訊 Start -->
            <tr class="ModifyHead">
                <td colspan="2">維護資訊<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead" style="width: 120px">維護資訊
                </td>
                <td class="TableModifyTd">
                    <table cellpadding="3" border="0">
                        <tr>
                            <td align="right" width="100px">建立者：
                            </td>
                            <td class="styleGreen" width="200px">
                                <asp:Literal ID="lt_Create_Who" runat="server" Text="新增資料中"></asp:Literal>
                            </td>
                            <td align="right" width="100px">建立時間：
                            </td>
                            <td class="styleGreen" width="250px">
                                <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">最後修改者：
                            </td>
                            <td class="styleGreen">
                                <asp:Literal ID="lt_Update_Who" runat="server"></asp:Literal>
                            </td>
                            <td align="right">最後修改時間：
                            </td>
                            <td class="styleGreen">
                                <asp:Literal ID="lt_Update_Time" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <!-- 維護資訊 End -->
        </table>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
