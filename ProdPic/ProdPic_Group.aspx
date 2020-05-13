<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProdPic_Group.aspx.cs" Inherits="ProdPic_Group" %>

<%@ Import Namespace="ExtensionMethods" %>
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
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- 多筆上傳 Start --%>
    <script src="../js/multiFile/jquery.MultiFile.pack.js" type="text/javascript"></script>
    <%-- 多筆上傳 End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox - 說明
            $("#helpMe").fancybox({
                type: 'iframe',
                width: 500,
                height: 400,
                fitToView: false,
                autoSize: false,
                closeClick: false,
                openEffect: 'fade', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });

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


            //多筆上傳
            $('#fu_Pic').MultiFile({
                STRING: {
                    remove: '<img src="../images/trashcan.png" alt="x" border="0" width="14" />' //移除圖示
                },
                accept: '<%=FileExtLimit %>' //副檔名限制
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
            <tr class="ModifyHead">
                <td colspan="4">圖片資料<em class="TableModifyTitleIcon"></em> <span class="Font12 stylePurple">(副檔名上傳限制：<%=FileExtLimit.Replace("|",", ")%>)</span>&nbsp;
                <span class="Font12 styleRed">(size=
                    <%=Param_Width%>*<%=Param_Height %>
                    px)</span>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead">
                    <em>(*)</em>圖片上傳
                </td>
                <td class="TableModifyTd" colspan="2" valign="top">
                    <table>
                        <tr>
                            <asp:PlaceHolder ID="ph_Lang" runat="server" Visible="false">
                                <td width="100" valign="top">
                                    <asp:DropDownList ID="ddl_Lang" runat="server">
                                    </asp:DropDownList>
                                    <br />
                                    <asp:RequiredFieldValidator ID="rfv_ddl_Lang" runat="server" ErrorMessage="-&gt; 請選擇「語系」"
                                        ForeColor="Red" Display="Dynamic" ControlToValidate="ddl_Lang" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                </td>
                            </asp:PlaceHolder>
                            <td valign="top">
                                <asp:FileUpload ID="fu_Pic" runat="server" />
                                <asp:RequiredFieldValidator ID="rfv_fu_Pic" runat="server" ErrorMessage="-&gt; 請選擇要上傳的圖片"
                                    ForeColor="Red" Display="Dynamic" ControlToValidate="fu_Pic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="TableModifyTd" valign="top">
                    <div>
                        <asp:Button ID="btn_Upload" runat="server" Text="上傳圖片" ValidationGroup="Add" OnClick="btn_Upload_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />
                        <input type="button" id="helpMe" value="?" style="cursor: help" title="說明" href="../Help.aspx?func=ProdPic_Group" />
                        <asp:Button ID="btn_SaveSort" runat="server" Text="儲存設定" OnClick="btn_SaveSort_Click"
                            ValidationGroup="List" Width="90px" CssClass="btnBlock colorRed" />
                        <asp:Button ID="btn_DelAll" runat="server" Text="全部刪除" OnClick="btn_DelAll_Click"
                            CausesValidation="false" OnClientClick="return confirm('是否確定刪除!?\n資料及圖檔將一併刪除!')"
                            Width="90px" CssClass="btnBlock colorRed" />
                        <a href="<%=Session["BackListUrl"] %>" class="btnBlock colorGray">返回列表</a>
                    </div>
                    <div class="styleBlue">
                        * 圖片可多張上傳，使用說明請點按「?」按鈕。<br />
                        * 圖片顯示順序，會以「排序」設定為依據。
                    </div>
                    <asp:ValidationSummary ID="vs_Add" runat="server" ShowMessageBox="true" ShowSummary="false"
                        ValidationGroup="Add" />
                    <asp:ValidationSummary ID="vs_List" runat="server" ShowMessageBox="true" ShowSummary="false"
                        ValidationGroup="List" />
                </td>
            </tr>
            <tr id="PicList">
                <td class="TableModifyTd" colspan="4">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
                        OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="List1" width="100%">
                                <tr class="tdHead">
                                    <td width="60px">編號
                                    </td>
                                    <td>圖片
                                    </td>
                                    <td width="100px" id="tdHeadLang" runat="server">語系
                                    </td>
                                    <td width="120px">更新日期
                                    </td>
                                    <td width="100px">排序
                                    </td>
                                    <td width="180px">功能選項
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="trItem" runat="server">
                                <td align="center">
                                    <%#Eval("Pic_ID") %>
                                </td>
                                <td valign="top">
                                    <table class="List2">
                                        <tbody>
                                            <tr>
                                                <%#PicUrl(Eval("Pic_File").ToString(), Eval("Pic_OrgFile").ToString())%>
                                                <td class="L2Info styleGraylight">
                                                    <asp:PlaceHolder ID="ph_Txt" runat="server" Visible="false">
                                                        <div>
                                                            <asp:TextBox ID="tb_PicDesc" runat="server" Text='<%# Eval("Pic_Desc")%>' TextMode="MultiLine"
                                                                Width="90%" Height="95px" CssClass="input01"></asp:TextBox>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <div style="padding-top: 5px">
                                                        <%#DealerPicUrl(Eval("Pic_File").ToString())%>
                                                    </div>
                                                    <div style="padding-top: 5px">
                                                        <%#Eval("Pic_OrgFile").ToString()%>
                                                    </div>
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
                                    <asp:TextBox ID="tb_Sort" runat="server" Width="40px" Style="text-align: center;"
                                        Text='<%# Eval("Sort")%>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="排序不可為空白"
                                        ControlToValidate="tb_Sort" Display="Dynamic" ForeColor="Red" ValidationGroup="List"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字！"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ForeColor="Red" ValidationGroup="List"></asp:RangeValidator>
                                </td>
                                <td align="center">
                                    <a class="btnBlock colorGray" href="<%#Get_MaintainUrl(Eval("Pic_ID").ToString()) %>">圖檔維護</a>
                                    <br />
                                    <a href="<%#Application["WebUrl"] %>FileDownload.ashx?OrgiName=<%#Server.UrlEncode(Eval("Pic_OrgFile").ToString()) %>&FilePath=<%#Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + Eval("Pic_File").ToString())) %>"
                                        class="BtnTwo">下載</a>
                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                                        OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                                    <asp:Literal ID="lt_PicID" runat="server" Text='<%#Eval("Pic_ID") %>' Visible="false"></asp:Literal>
                                </td>
                            </tr>
                        </ItemTemplate>
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
