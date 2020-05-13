<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Rel_ProdItem.aspx.cs"
    Inherits="Spec_Rel_ProdItem" %>

<%@ Register Src="Ascx_QuickMenu.ascx" TagName="Ascx_QuickMenu" TagPrefix="uc1" %>
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
    <%-- catcomplete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/catcomplete/catcomplete.js" type="text/javascript"></script>
    <%-- catcomplete End --%>
    <%-- autocomplete Start --%>
    <link href="../js/autocomplete/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%-- autocomplete End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            /* Autocomplete - 群組分類(品號) */
            $("#tb_ModelNo").catcomplete({
                minLength: 2,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_Model_No_Json.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    category: item.category
                                }
                            }));
                        }
                    });
                },
                select: function (event, ui) {
                    $(this).addClass("input01");
                    $("#val_ModelNo").val(ui.item.value);
                }
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
            });

            //Click事件 - 項目選擇
            $('input[id*="cb_Item"]').click(function () {
                //取得上層 - TR
                var this_tID = $(this).parent().parent("tr");
                //判斷是否選取
                if ($(this).attr("checked")) {
                    //變更Css
                    this_tID.addClass("TrOrange");
                } else {
                    //變更Css
                    this_tID.removeClass("TrOrange");
                }
            });

            //判斷選項是否已勾選(pageload)
            check_Select();
        });

        //判斷選項是否已勾選
        function check_Select() {
            //巡覽Checkbox - 判斷選項是否已勾選
            $('input[id*="cb_Item"]').each(function () {
                //取得上層 - TR
                var this_tID = $(this).parent().parent("tr");
                //判斷是否選取
                if ($(this).attr("checked")) {
                    //變更Css
                    this_tID.addClass("TrOrange");
                } else {
                    //變更Css
                    this_tID.removeClass("TrOrange");
                }
            });
        }

        //全選
        function selectAll(invoker) {
            var inputElements = document.getElementsByTagName('input');

            for (var i = 0; i < inputElements.length; i++) {
                var myElement = inputElements[i];
                if (myElement.type === "checkbox") {
                    myElement.checked = invoker.checked;
                }
            }
            //判斷選項是否已勾選
            check_Select();
        }

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>產品關聯</span>
    </div>
    <div class="h2Head">
        <h2>
            產品主檔 ←關聯→ 規格欄位</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div class="Sift">
        <ul>
            <li>目前品號：
                <asp:TextBox ID="tb_ModelNo" runat="server" MaxLength="40" Width="200px">
                </asp:TextBox>
                <asp:TextBox ID="val_ModelNo" runat="server" Style="display: none">
                </asp:TextBox>&nbsp;
                <asp:RequiredFieldValidator ID="rfv_val_ModelNo" runat="server" ErrorMessage="-&gt; 請選擇正確的「品號」!"
                    ForeColor="Red" ControlToValidate="val_ModelNo" Display="Dynamic" ValidationGroup="Select">
                </asp:RequiredFieldValidator>
                <asp:Button ID="btn_ModelNo" runat="server" Text="選擇品號" OnClick="btn_ModelNo_Click"
                    ValidationGroup="Select" CssClass="btnBlock colorGray" />
            </li>
        </ul>
        <asp:PlaceHolder ID="ph_Search" runat="server">
            <ul>
                <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" ToolTip="關鍵字查詢"></asp:TextBox>
                </li>
                <li>選單單頭：<asp:TextBox ID="tb_GID" runat="server" MaxLength="5" Width="180px"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                    |
                    <asp:Button ID="btn_Save" runat="server" Text="此頁勾選加入關聯" ValidationGroup="List" OnClick="btn_Save_Click"
                        CssClass="btnBlock colorRed" />
                </li>
            </ul>
        </asp:PlaceHolder>
    </div>
    <asp:PlaceHolder ID="ph_List" runat="server">
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead">
                        <td width="60px">
                            <asp:CheckBox ID="cbSelectAll" runat="server" Text="ALL" OnClick="selectAll(this)" />
                        </td>
                        <td>
                            規格欄位
                        </td>
                        <td width="140px">
                            輸入方式
                        </td>
                        <td width="60px">
                            是否必填
                        </td>
                        <td width="100px">
                            選單單頭
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server">
                    <td align="center">
                        <asp:CheckBox ID="cb_Item" runat="server" />
                        <asp:HiddenField ID="hf_SpecID" runat="server" Value='<%# Eval("SpecID")%>' />
                        <asp:HiddenField ID="hf_CheckedID" runat="server" />
                    </td>
                    <td valign="top">
                        <table class="List2">
                            <tbody>
                                <tr>
                                    <td class="L2Info">
                                        <div class="styleGreen B">
                                            <%# Eval("SpecID")%></div>
                                        <div class="L2MainHead" style="padding-top: 3px">
                                            <%# Eval("SpecName_zh_TW")%>
                                        </div>
                                        <div class="L2Info styleGraylight" style="padding-left: 5px">
                                            <%# Eval("SpecDESC")%>
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td align="center">
                        <%# fn_Desc.Prod.InputType(Eval("SpecType").ToString())%>
                    </td>
                    <td align="center">
                        <%# fn_Desc.PubAll.YesNo(Eval("IsRequired").ToString())%>
                    </td>
                    <td align="center">
                        <a href="SpecOption_Search.aspx?GID=<%# HttpUtility.UrlEncode(Eval("OptionGID").ToString())%>">
                            <%# Eval("OptionGID")%></a>
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
    </asp:PlaceHolder>
    <!-- 備註說明 -->
    <div class="ListIllusArea">
        <div class="JQ-ui-state-highlight">
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>針對品號，設定該品號特有的規格欄位</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>此為關聯性設定，若要維護規格欄位，需前往「<a href="Spec_Search.aspx">規格欄位</a>」</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>這功能設定為一對多模式，即品號(一) 對 規格欄位(多)</div>
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
</a>