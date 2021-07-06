<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_InfoEdit.aspx.cs" Inherits="Prod_InfoEdit"
    ValidateRequest="false" %>

<%@ Register Src="Ascx_TabMenu.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
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
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- ckEditor Start --%>
    <script src="../js/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script>
        //允許所有的html tag
        CKEDITOR.config.allowedContent = true;
    </script>
    <%-- ckEditor End --%>
    <style>
        /* 強迫設定 ckeditor 高度*/
        .cke_contents {
            height: 400px !important;
        }
    </style>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">
                <%=Navi_系統首頁%></a>&gt;<a><%=Navi_產品資料庫%></a>&gt;<span><%=Navi_產品資料%></span>
        </div>
        <div class="h2Head">
            <h2>產品資訊 -
            <asp:Label ID="lb_Model_No" runat="server" CssClass="styleRed B"><%=Param_ModelNo %></asp:Label>
            </h2>
        </div>
        <div class="SysTab">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
            <%--<div style="float: right">
                <a href="<%=Param_DealerWebUrl %>Product/ProdPreview.aspx?LangType=<%=Param_InfoLang %>&ModelNo=<%=Server.UrlEncode(Param_ModelNo) %>"
                    class="btnBlock colorDark" target="_blank">頁面預覽</a>
            </div>--%>
        </div>
        <table class="TableModify">
            <%--            <!-- // 規格符號 Start // -->
            <tr class="ModifyHead">
                <td>規格符號 (for 深圳)<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <asp:ListView ID="lvIconList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                            GroupItemCount="5" OnItemDataBound="lvIconList_ItemDataBound" Visible="false">
                            <LayoutTemplate>
                                <table class="List1" width="100%">
                                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                                </table>
                            </LayoutTemplate>
                            <GroupTemplate>
                                <tr>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tr>
                            </GroupTemplate>
                            <ItemTemplate>
                                <td align="center" width="20%">
                                    <%#SpecPicUrl(Eval("Pic_File").ToString())%>
                                    <div style="padding-top: 5px;">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Value="Y"><span class="styleBlue">顯示</span>&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="N">隱藏</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <asp:HiddenField ID="hf_PicID" runat="server" Value='<%# Eval("Pic_ID").ToString()%>' />
                                </td>
                            </ItemTemplate>
                            <EmptyItemTemplate>
                                <td></td>
                            </EmptyItemTemplate>
                        </asp:ListView>
                    </td>
                </tr>
            </tbody>
            <!-- // 規格符號 End // -->
            <!-- // 認證符號 Start // -->
            <tr class="ModifyHead">
                <td>認證符號 (for 深圳)<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <asp:ListView ID="lvCertIconList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                            GroupItemCount="8" OnItemDataBound="lvCertIconList_ItemDataBound" Visible="false">
                            <LayoutTemplate>
                                <table class="List1" width="100%">
                                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                                </table>
                            </LayoutTemplate>
                            <GroupTemplate>
                                <tr>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tr>
                            </GroupTemplate>
                            <ItemTemplate>
                                <td align="center" valign="bottom" width="12.5%">
                                    <%#CertPicUrl(Eval("Pic_File").ToString())%>
                                    <div style="padding-top: 5px;">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Value="Y"><span class="styleBlue">顯示</span>&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="N">隱藏</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>

                                    <asp:HiddenField ID="hf_PicID" runat="server" Value='<%# Eval("Pic_ID").ToString()%>' />
                                </td>
                            </ItemTemplate>
                            <EmptyItemTemplate>
                                <td></td>
                            </EmptyItemTemplate>
                        </asp:ListView>
                    </td>
                </tr>
            </tbody>
            <!-- // 認證符號 End // -->--%>
            <!-- // 產品簡述SEO Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        網站描述(SEO) <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="Button2" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info9" runat="server" TextMode="MultiLine" Width="100%" Height="100"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 產品簡述SEO End // -->
            <!-- // 產品簡述 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        產品簡述(官網產品簡述) <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="Button1" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info5" runat="server" TextMode="MultiLine" Width="100%" Height="100"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 產品簡述 End // -->
            <!-- // 描述 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        描述(Description) <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="btn_Save1" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info1" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 描述 End // -->
            <!-- // 特性 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        特性(Features) <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="btn_Save2" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info2" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 特性 Start // -->
            <!-- // 應用 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        應用(Applications) <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="btn_Save3" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info3" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 應用 Start // -->
            <!-- // 規格 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        規格(Specification) <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="btn_Save4" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info4" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 規格 Start // -->
            <!-- // 產品展示簡述 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        產品展示簡述 <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                        <asp:Button ID="btn_Save6" runat="server" Text="資料儲存" ValidationGroup="Add" OnClick="btn_Save_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info6" runat="server" TextMode="MultiLine" Width="100%" Height="200"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 產品展示簡述 End // -->
            <!-- // 社群分享標題 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        社群分享標題 <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info7" runat="server" TextMode="MultiLine" Width="100%" MaxLength="80"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 社群分享標題 End // -->
            <!-- // 社群分享簡述 Start // -->
            <tr class="ModifyHead">
                <td>
                    <div style="float: left">
                        社群分享簡述 <em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right">
                    </div>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Info8" runat="server" TextMode="MultiLine" Width="100%" Height="200"></asp:TextBox>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- // 社群分享簡述 End // -->
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
