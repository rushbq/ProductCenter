<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_BOM_Dtl_WinBox.aspx.cs"
    Inherits="Prod_BOM_Dtl_WinBox" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
        <%if(Param_IsMulti == "Y"){ %>
            //Click事件 - Checkbox全選
            $("#clickAll").click(function () {
                if ($("#clickAll").attr("checked")) {
                    $("input[id*='cb_ID']").each(function () {
                        $(this).attr("checked", true);
                    });
                }
                else {
                    $("input[id*='cb_ID']'").each(function () {
                        $(this).attr("checked", false);
                    });
                }
            });

            <% }else{ %>
        
            //Click事件 - Checkbox, 限定只能勾選一項, 勾選後清除其他已勾選
            $("#clickAll").hide();
            $('input[id*="cb_ID"]').click(function () {
                var cb_Items = $('input[id*="cb_ID"]');
                //清除所有已勾選
                for (var i = 0; i < cb_Items.length; i++) {
                    cb_Items[i].checked = false;
                }
                //勾選此項目
                $(this).attr("checked", true);
            });
            <% } %>

            //判斷是否有已選取的值
            reCheck_Items();

        });

        //判斷是否有已選取的值，勾選已選的Checkbox
        function reCheck_Items(){
            //取得母頁已選取的值
            var getInputVal = parent.document.getElementById('<%=Param_FiledName %>').value;
            var spInputVal = getInputVal.split("||||");
            var cb_Items = $('input[id*="cb_ID"]');
            for (var row = 0; row < cb_Items.length; row++) {
                for (var col = 0; col < spInputVal.length; col++) {
                    //分解Checkbox Value
                    var aryVal = cb_Items[row].value.split("｜");
                    if (aryVal[0] == spInputVal[col]) {
                        cb_Items[row].checked = true;
                        break;
                    };
                }
            }
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Sift">
        <ul>
            <li>
                <asp:Button ID="btn_Add" runat="server" Text="加入勾選項目" OnClick="btn_Add_Click" CssClass="btnBlock colorRed" />|
                <input type="button" value="關閉視窗" onclick="parent.$.fancybox.close();" style="width: 90px"
                    class="btnBlock colorGray" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="60px">
                        <input id="clickAll" type="checkbox" title="全選">
                    </td>
                    <td>
                        選單代號/名稱
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server" style="white-space: nowrap;">
                <td align="center">
                    <asp:CheckBox ID="cb_ID" runat="server" />
                </td>
                <td valign="top">
                    <table class="List2">
                        <tbody>
                            <tr>
                                <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                                <td class="L2Info">
                                    <span class="styleGreen B">
                                        <%# Eval("Spec_OptionValue")%></span> - <span class="L2MainHead">
                                            <%# Eval("Spec_OptionName_zh_TW")%>
                                        </span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>
            <div style="padding: 120px 0px 120px 0px; text-align: center">
                <span style="color: #FD590B; font-size: 12px">查無任何符合資料！</span>
            </div>
        </EmptyDataTemplate>
    </asp:ListView>
    <div style="padding-bottom: 30px">
    </div>
    </form>
</body>
</html>
