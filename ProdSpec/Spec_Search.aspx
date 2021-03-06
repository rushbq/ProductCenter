﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Search.aspx.cs" Inherits="Spec_Search" %>

<%@ Register Src="Ascx_QuickMenu.ascx" TagName="Ascx_QuickMenu" TagPrefix="uc1" %>
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
    <%-- autocomplete Start --%>
    <link href="../js/autocomplete/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%-- autocomplete End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            //fancybox - 編輯鈕
            $(".EditBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });

            //AutoComplete - 欄位群組
            $("#tb_GID").autocomplete('../AC_SpecOption_Group.aspx');
            $("#tb_GID").result(
                function (event, data, formatted) {
                    $("#tb_GID").val(data[1]);
                }
	        );

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_Keyword").val("");
                $("#tb_GID").val("");
                $("select#ddl_SpecClass")[0].selectedIndex = 0;
                $("select#ddl_IsRel")[0].selectedIndex = 0;
            });

            //隱藏/顯示
            $("img.DTtoggle").click(function () {
                //取得此物件的rel屬性值 (#xx)
                var _this = $(this).attr("rel");
                //判斷指定元素是否隱藏
                if ($(_this).css("display") == "none") {
                    $(_this).slideDown();
                    $(this).attr("src", "../images/icon_top.png").attr("title", "隱藏");
                } else {
                    $(_this).slideUp();
                    $(this).attr("src", "../images/icon_down.png").attr("title", "查看關聯");
                }
                return false;
            });

        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>規格欄位</span>
    </div>
    <div class="h2Head">
        <h2>
            規格欄位列表</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div class="Sift">
        <ul>
            <li>已關聯分類：
                <asp:DropDownListGP ID="ddl_SpecClass" runat="server">
                </asp:DropDownListGP>
            </li>
            <li>是否關聯選單單頭：
                <asp:DropDownList ID="ddl_IsRel" runat="server">
                    <asp:ListItem Value="">-- 所有資料 --</asp:ListItem>
                    <asp:ListItem Value="Y">已關聯</asp:ListItem>
                    <asp:ListItem Value="N">未關聯</asp:ListItem>
                </asp:DropDownList>
            </li>
        </ul>
        <ul>
            <li>選單單頭關鍵字：<asp:TextBox ID="tb_GID" runat="server" MaxLength="5" Width="180px"></asp:TextBox>
            </li>
            <li>欄位關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" ToolTip="欄位名稱/代號查詢"></asp:TextBox>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                |
                <input type="button" class="EditBox btnBlock colorBlue" value="資料新增" title="規格欄位 - 新增"
                    href="Spec_Edit.aspx" />
                <asp:Button ID="btn_Save" runat="server" Text="儲存本頁設定" ValidationGroup="List" OnClick="btn_Save_Click"
                    CssClass="btnBlock colorRed" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
        OnItemDataBound="lvDataList_ItemDataBound">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td>
                        欄位代號/名稱
                    </td>
                    <td width="100px">
                        輸入方式
                    </td>
                    <td width="60px">
                        是否必填
                    </td>
                    <td width="140px">
                        選單單頭關聯
                    </td>
                    <td width="180px">
                        排序 & 顯示
                    </td>
                    <td width="140px">
                        功能選項
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td valign="top">
                    <div class="L2Info">
                        <span class="styleGreen B">
                            <%# Eval("SpecID")%></span> - <span class="L2MainHead">
                                <%# Eval("SpecName_zh_TW")%>
                            </span>
                        <asp:Literal ID="lt_toggleImg" runat="server"></asp:Literal>
                    </div>
                    <div id="dv<%# Eval("SpecID")%>" style="padding-left: 8px; display: none;">
                        <asp:Literal ID="lt_ClassNavi" runat="server"></asp:Literal>
                    </div>
                </td>
                <td align="center">
                    <%# fn_Desc.Prod.InputType(Eval("SpecType").ToString())%>
                </td>
                <td align="center">
                    <%# fn_Desc.PubAll.YesNo(Eval("IsRequired").ToString())%>
                </td>
                <td align="center">
                    <a href="SpecOption_Search.aspx?GID=<%# HttpUtility.UrlEncode(Eval("OptionGID").ToString())%>">
                        <%# Eval("OptionGP_Name")%></a>
                </td>
                <td align="center">
                    <table class="TableS1" width="98%">
                        <tbody>
                            <tr>
                                <td class="TableS1TdHead" style="width: 50px;">
                                    顯示
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal"
                                        RepeatLayout="Flow">
                                        <asp:ListItem Value="Y">顯示&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="N">隱藏</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="TableS1TdHead">
                                    排序
                                </td>
                                <td>
                                    <asp:TextBox ID="tb_Sort" runat="server" Width="40px" Style="text-align: center;"
                                        Text='<%# Eval("Sort")%>'></asp:TextBox><br />
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="排序不可為空白"
                                        ControlToValidate="tb_Sort" Display="Dynamic" ForeColor="Red" ValidationGroup="List"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字！"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ForeColor="Red" ValidationGroup="List"></asp:RangeValidator>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td align="center">
                    <a class="Edit EditBox" href="Spec_Edit.aspx?SpecID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("SpecID").ToString()))%>">
                        修改</a>
                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                        OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                    <asp:HiddenField ID="hf_SpecID" runat="server" Value='<%# Eval("SpecID")%>' />
                </td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>
            <div style="padding: 120px 0px 120px 0px; text-align: center">
                <span style="color: #FD590B; font-size: 12px">未新增或無任何符合資料！</span>
            </div>
        </EmptyDataTemplate>
    </asp:ListView>
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
                <asp:Literal ID="lt_Page_DataCntInfo" runat="server" EnableViewState="False"></asp:Literal></div>
        </div>
    </asp:Panel>
    <!-- 備註說明 -->
    <div class="ListIllusArea">
        <div class="JQ-ui-state-highlight">
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>設定產品明細資料，可使用的規格欄位</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>設定完成後，再前往「<a href="Spec_Rel_SpecClass.aspx">分類關聯</a>」設定關聯性</div>
        </div>
    </div>
    </form>
</body>
</html>
<script language="javascript" type="text/javascript">
    function EnterClick(e) {
        // 這一行讓 ie 的判斷方式和 Firefox 一樣。
        if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;

        if (e.which == 13) {
            // Submit按鈕
            __doPostBack('btn_Search', '');
            return false;
        }
    }

    document.onkeypress = EnterClick;
</script>
