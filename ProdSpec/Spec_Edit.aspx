<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Edit.aspx.cs" Inherits="Spec_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            /* --- 符號表 Start --- */
            //符號表顯示大圖 
            $(".tooltip").poshytip({
                className: 'tip-darkgray',
                bgImageFrameSize: 9,
                offsetX: -10,
                offsetY: 10,
                content: function () {
                    var fileSrc = $(this).attr('src');
                    var iconName = $(this).attr('iconName');
                    var html = '<div>' + iconName + '</div>';
                    html += '<div><img src="' + fileSrc + '" width="150"></div>';
                    return html;
                }
            });
            /* --- 符號表 End --- */
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                <asp:Literal ID="lt_Head" runat="server" Text="資料編輯"></asp:Literal><em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead" style="width: 120px">
                <em>(*)</em> 代號
            </td>
            <td class="TableModifyTd styleBlue">
                <asp:Literal ID="lt_SpecID" runat="server">系統自動編號</asp:Literal>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>規格欄位名稱
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;
                    <asp:TextBox ID="tb_SpecName_zh_TW" runat="server" Width="400px" MaxLength="50" ValidationGroup="GPAdd"></asp:TextBox>
                </div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;
                    <asp:TextBox ID="tb_SpecName_en_US" runat="server" Width="400px" MaxLength="100"
                        ValidationGroup="GPAdd"></asp:TextBox>
                </div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;
                    <asp:TextBox ID="tb_SpecName_zh_CN" runat="server" Width="400px" MaxLength="50" ValidationGroup="GPAdd"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfv_tb_SpecName" runat="server" Display="Dynamic"
                    ControlToValidate="tb_SpecName_zh_TW" ErrorMessage="-&gt; 請輸入「規格欄位名稱」" ForeColor="Red"
                    ValidationGroup="GPAdd"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>輸入方式
            </td>
            <td class="TableModifyTd">
                <asp:DropDownList ID="ddl_SpecType" runat="server">
                </asp:DropDownList>
                <asp:Label ID="lb_SpecType_Warning" runat="server" CssClass="SiftLight"></asp:Label>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>是否必填
            </td>
            <td class="TableModifyTd">
                <asp:RadioButtonList ID="rbl_IsRequired" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Y">是</asp:ListItem>
                    <asp:ListItem Value="N" Selected="True">否</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                分類關聯
            </td>
            <td class="TableModifyTd">
                <asp:Panel ID="pl_SetClass" runat="server">
                    <asp:DropDownListGP ID="ddl_SpecClass" runat="server">
                    </asp:DropDownListGP>
                </asp:Panel>
                <asp:Panel ID="pl_SetMoreClass" runat="server" Visible="false">
                    <a href="#" onclick="parent.location.href='Spec_Frame_SpecClass.aspx';">前往設定</a>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                使用選單單頭
            </td>
            <td class="TableModifyTd">
                <asp:DropDownList ID="ddl_OptionGID" runat="server">
                </asp:DropDownList>
                <asp:Label ID="lb_SetMsg" runat="server" Text="(確認選單單頭是否正確)" CssClass="SiftLight"
                    Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                規格欄位說明
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_SpecDESC" runat="server" Width="400px" MaxLength="50"></asp:TextBox><br />
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                符號
            </td>
            <td class="TableModifyTd">
                <asp:RadioButtonList ID="rbl_Icon" runat="server" RepeatColumns="6" RepeatLayout="Table"
                    RepeatDirection="Horizontal" CssClass="List2" Width="65%">
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                是否顯示
            </td>
            <td class="TableModifyTd">
                <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Y" Selected="True">顯示</asp:ListItem>
                    <asp:ListItem Value="N">隱藏</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                排序
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Sort" runat="server" Width="50px" MaxLength="3" Style="text-align: center;">999</asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="-&gt; 請輸入「排序」"
                    Display="Dynamic" ControlToValidate="tb_Sort" ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                    Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                    ForeColor="Red"></asp:RangeValidator>
            </td>
        </tr>
    </table>
    <div class="SubmitArea">
        <asp:Button ID="btn_Edit" runat="server" Text="新增" OnClick="btn_Edit_Click" ValidationGroup="GPAdd"
            Width="90px" CssClass="btnBlock colorBlue" />
        <asp:Button ID="btn_Del" runat="server" Text="刪除本筆" OnClick="btn_Del_Click" ValidationGroup="GPAdd"
            OnClientClick="return confirm('是否確定刪除!?')" Enabled="false" Width="90px" CssClass="btnBlock colorRed" />
        <input onclick="parent.$.fancybox.close();" type="button" value="關閉視窗" style="width: 90px"
            class="btnBlock colorGray" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
            ShowMessageBox="true" ValidationGroup="GPAdd" />
    </div>
    <div class="ListIllus">
        <span>標明<em class="Must">(*)</em>的規格欄位請務必填寫。</span>
    </div>
    </form>
</body>
</html>
