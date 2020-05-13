<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecCategory_Edit.aspx.cs"
    Inherits="SpecCategory_Edit" %>

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
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                資料編輯<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead" style="width: 120px">
                <em>(*)</em> 編號
            </td>
            <td class="TableModifyTd styleBlue">
                <asp:Literal ID="lt_CateID" runat="server">系統自動編號</asp:Literal>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>規格分類名稱
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;
                    <asp:TextBox ID="tb_CateName_zh_TW" runat="server" Width="400px" MaxLength="50" ValidationGroup="GPAdd"></asp:TextBox>
                </div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;
                    <asp:TextBox ID="tb_CateName_en_US" runat="server" Width="400px" MaxLength="100"
                        ValidationGroup="GPAdd"></asp:TextBox>
                </div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;
                    <asp:TextBox ID="tb_CateName_zh_CN" runat="server" Width="400px" MaxLength="50" ValidationGroup="GPAdd"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfv_tb_CateName_zh_TW" runat="server" Display="Dynamic"
                    ControlToValidate="tb_CateName_zh_TW" ErrorMessage="-&gt; 請輸入「規格分類名稱」" ForeColor="Red"
                    ValidationGroup="GPAdd"></asp:RequiredFieldValidator>
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
        <tr id="TrRel" runat="server" visible="false">
            <td class="TableModifyTdHead">
                關聯設定
            </td>
            <td class="TableModifyTd">
                <a href="SpecCategory_Rel.aspx?CateID=<%=Server.UrlEncode(Param_CateID) %>&CateName=<%=Server.UrlEncode(this.tb_CateName_zh_TW.Text.Trim()) %>"
                    target="mainFrame">關聯設定</a>
            </td>
        </tr>
    </table>
    <div class="SubmitArea">
        <asp:Button ID="btn_Edit" runat="server" Text="新增" OnClick="btn_Edit_Click" ValidationGroup="GPAdd"
            Width="90px" CssClass="btnBlock colorBlue" />
        <input onclick="parent.$.fancybox.close();" type="button" value="關閉視窗" style="width: 90px"
            class="btnBlock colorGray" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
            ShowMessageBox="true" ValidationGroup="GPAdd" />
    </div>
    <div class="ListIllus">
        <span>標明<em class="Must">(*)</em>的項目請務必填寫。</span>
    </div>
    </form>
</body>
</html>
