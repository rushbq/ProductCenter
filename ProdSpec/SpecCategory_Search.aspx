<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecCategory_Search.aspx.cs"
    Inherits="SpecCategory_Search" %>

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
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
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

            /* Autocomplete - 規格欄位 */
            $("#tb_SpecName").autocomplete('../AC_Spec.aspx');
            $("#tb_SpecName").result(
                function (event, data, formatted) {
                    $("#tb_SpecID").val(data[1]);
                }
	        );

            /* Tooltip - 使用Html */
            $(".tooltip_html").poshytip({
                className: 'tip-darkgray',
                bgImageFrameSize: 9,
                offsetX: -10,
                offsetY: 10,
                fade: false,
                content: $('#tip1').html()
            });

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("select#ddl_SpecClass")[0].selectedIndex = 0;
                $("#tb_SpecName").val('');
                $("#tb_SpecID").val('');
                $("#tb_Keyword").val('');
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>規格分類</span>
    </div>
    <div class="h2Head">
        <h2>
            規格分類列表</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div class="Sift">
        <ul>
            <li>已關聯小分類：
                <asp:DropDownListGP ID="ddl_SpecClass" runat="server">
                </asp:DropDownListGP>
            </li>
            <li>已關聯規格：
                <asp:TextBox ID="tb_SpecName" runat="server" Width="250px" CssClass="tooltip_html"></asp:TextBox>
                <asp:TextBox ID="tb_SpecID" runat="server" Style="display: none"></asp:TextBox>
            </li>
        </ul>
        <ul>
            <li>規格分類關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50"></asp:TextBox></li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                |
                <input type="button" class="EditBox btnBlock colorBlue" value="資料新增" title="規格分類 - 新增"
                    href="SpecCategory_Edit.aspx" />
                <asp:Button ID="btn_Save" runat="server" Text="儲存本頁設定" ValidationGroup="List" OnClick="btn_Save_Click"
                    CssClass="btnBlock colorRed" />
            </li>
        </ul>
    </div>
    <div id="tip1" style="display: none">
        How to use?<br />
        1. 輸入關鍵字:規格代號 (ex:006)<br />
        2. 輸入關鍵字:規格名稱 (ex:材質)
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
        OnItemDataBound="lvDataList_ItemDataBound">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="60px">
                        編號
                    </td>
                    <td>
                        規格分類
                    </td>
                    <td width="120px">
                        是否顯示
                    </td>
                    <td width="80px">
                        排序
                    </td>
                    <td width="240px">
                        功能選項
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center">
                    <%# Eval("CateID")%>
                </td>
                <td align="center">
                    <span class="L2MainHead">
                        <%# Eval("CateName_zh_TW")%>
                    </span>
                </td>
                <td align="center">
                    <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="Y">顯示&nbsp;</asp:ListItem>
                        <asp:ListItem Value="N">隱藏</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td align="center" class="styleBlue">
                    <asp:TextBox ID="tb_Sort" runat="server" Width="40px" Style="text-align: center;"
                        Text='<%# Eval("Sort")%>'></asp:TextBox><br />
                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="排序不可為空白"
                        ControlToValidate="tb_Sort" Display="Dynamic" ForeColor="Red" ValidationGroup="List"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字！"
                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                        ForeColor="Red" ValidationGroup="List"></asp:RangeValidator>
                </td>
                <td align="center">
                    <a class="Edit EditBox" title="規格分類 - 修改" href="SpecCategory_Edit.aspx?CateID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("CateID").ToString()))%>">
                        修改</a>
                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                        OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                    <a class="BtnFour" href="SpecCategory_Rel.aspx?CateID=<%# Server.UrlEncode(Eval("CateID").ToString())%>&CateName=<%# Server.UrlEncode(Eval("CateName_zh_TW").ToString())%>">
                        關聯設定</a>
                    <asp:HiddenField ID="hf_CateID" runat="server" Value='<%# Eval("CateID")%>' />
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
                <span class="JQ-ui-icon ui-icon-info"></span>設定產品明細資料，各規格欄位的小分類(與1.分類設定中的類別不同)</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>維護完成後，需設定關聯性(屬於哪一個規格欄位)</div>
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
