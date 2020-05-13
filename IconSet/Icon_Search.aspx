<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Icon_Search.aspx.cs" Inherits="Icon_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
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
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'elastic',
                nextEffect: 'elastic'
            });

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_Keyword").val("");
                $("select#ddl_Icon_Type")[0].selectedIndex = 0;
            });

        });

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>符號資料庫</a>&gt;<span>符號資料列表</span>
    </div>
    <div class="h2Head">
        <h2>
            符號資料列表</h2>
    </div>
    <!-- Basic Sift -->
    <div class="Sift">
        <ul>
            <li>符號使用者：
                <asp:DropDownList ID="ddl_Icon_Type" runat="server">
                </asp:DropDownList>
            </li>
            <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" ToolTip="名稱關鍵字"></asp:TextBox>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                |
                <input type="button" value="資料新增" onclick="location.href='Icon_Edit.aspx';" class="btnBlock colorBlue" />
                <asp:Button ID="btn_Save" runat="server" Text="儲存本頁設定" ValidationGroup="List" OnClick="btn_Save_Click"
                    CssClass="btnBlock colorRed" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound"
        OnItemCommand="lvDataList_ItemCommand">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="60px">
                        自訂編號
                    </td>
                    <td width="300px">
                        名稱
                    </td>
                    <td>
                        圖片
                    </td>
                    <td width="120px">
                        符號使用者
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
                <td align="center">
                    <%#Eval("CID")%>
                </td>
                <td align="left">
                    <div style="padding-bottom: 4px">
                        <span class="styleGraylight">(繁中)</span>&nbsp;
                        <%#Eval("IconName_zh_TW").ToString()%>
                    </div>
                    <div style="padding-bottom: 4px">
                        <span class="styleGraylight">(英文)</span>&nbsp;
                        <%#Eval("IconName_en_US").ToString()%>
                    </div>
                    <div>
                        <span class="styleGraylight">(簡中)</span>&nbsp;
                        <%#Eval("IconName_zh_CN").ToString()%>
                    </div>
                </td>
                <td align="left">
                    <asp:Literal ID="lt_Pics" runat="server"></asp:Literal>
                </td>
                <td align="center" class="styleEarth">
                    <%#Eval("Icon_TypeName").ToString()%>
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
                                        Text='<%# Eval("Sort")%>'></asp:TextBox>
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
                    <a class="Edit" href="Icon_Edit.aspx?Icon_ID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("Icon_ID").ToString()))%>">
                        修改</a>
                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                        OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                    <asp:HiddenField ID="hf_Icon_ID" runat="server" Value='<%# Eval("Icon_ID")%>' />
                    <asp:HiddenField ID="hf_Icon_Type" runat="server" Value='<%# Eval("Icon_Type")%>' />
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
