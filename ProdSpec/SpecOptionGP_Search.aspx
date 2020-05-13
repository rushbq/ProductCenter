<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecOptionGP_Search.aspx.cs"
    Inherits="SpecOptionGP_Search" %>

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

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_Keyword").val("");
                $("select#ddl_IsSet")[0].selectedIndex = 0;
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
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>選單單頭設定</span>
    </div>
    <div class="h2Head">
        <h2>
            選單單頭列表</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div class="Sift">
        <ul>
            <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" ToolTip="選單單頭/名稱查詢"></asp:TextBox>
            </li>
            <li>是否關聯：
                <asp:DropDownList ID="ddl_IsRel" runat="server">
                    <asp:ListItem Value="">-- 所有資料 --</asp:ListItem>
                    <asp:ListItem Value="Y">已關聯</asp:ListItem>
                    <asp:ListItem Value="N">未關聯</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li>選項設定：
                <asp:DropDownList ID="ddl_IsSet" runat="server">
                    <asp:ListItem Value="">-- 所有資料 --</asp:ListItem>
                    <asp:ListItem Value="Y">已設定</asp:ListItem>
                    <asp:ListItem Value="N">未設定</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                |
                <input type="button" class="EditBox btnBlock colorBlue" value="資料新增" title="選單單頭 - 新增"
                    href="SpecOptionGP_Edit.aspx" />
                <asp:Button ID="btn_Save" runat="server" Text="儲存本頁設定" ValidationGroup="List" OnClick="btn_Save_Click"
                    Visible="false" CssClass="btnBlock colorRed" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
        OnItemDataBound="lvDataList_ItemDataBound">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td>
                        選單單頭/名稱
                    </td>
                    <td width="80px">
                        選單設定
                    </td>
                    <td width="180px" style="display: none">
                        排序 & 顯示
                    </td>
                    <td width="250px">
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
                            <%# Eval("OptionGID")%></span> - <span class="L2MainHead">
                                <%# Eval("OptionGName")%>
                            </span>
                        <asp:Literal ID="lt_toggleImg" runat="server"></asp:Literal>
                    </div>
                    <div id="dv<%# Eval("OptionGID")%>" style="padding-left: 8px; display: none;">
                        <asp:Literal ID="lt_ClassNavi" runat="server"></asp:Literal>
                    </div>
                </td>
                <td align="center" class="styleEarth">
                    <asp:Literal ID="lt_SetStatus" runat="server"></asp:Literal>
                </td>
                <td align="center" style="display: none">
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
                    <a class="Edit EditBox" title="選單單頭 - 修改" href="SpecOptionGP_Edit.aspx?OptionGID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("OptionGID").ToString()))%>">
                        修改</a>
                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                        OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                    <a class="BtnFour" href="SpecOption_Search.aspx?GID=<%# Server.UrlEncode(Eval("OptionGID").ToString())%>">
                        設定選單</a>
                    <asp:HiddenField ID="hf_OptionGID" runat="server" Value='<%# Eval("OptionGID")%>' />
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
                <span class="JQ-ui-icon ui-icon-info"></span>設定產品明細資料，各規格可使用的「下拉選單」</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>此設定為下拉選單的單頭，完成後需再前往「<a href="SpecOption_Search.aspx">選單</a>」設定所屬的選單內容</div>
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
