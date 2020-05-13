<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecCategory_Rel.aspx.cs"
    Inherits="SpecCategory_Rel" %>

<%@ Register Src="Ascx_QuickMenu.ascx" TagName="Ascx_QuickMenu" TagPrefix="uc1" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- autocomplete Start --%>
    <link href="../js/autocomplete/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%-- autocomplete End --%>
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $("#TreeView").treeview({
                collapsed: true,
                animated: "fast",
                control: "#sidetreecontrol"
            });

            /* Autocomplete - 規格分類 */
            $("#tb_SpecCateName").autocomplete('../AC_SpecCategory.aspx');
            $("#tb_SpecCateName").result(
                function (event, data, formatted) {
                    $("#tb_SpecCateID").val(data[1]);
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

            //Checkbox階層式全選
            $("input[id*='cb_']").click(function () {
                //取得目前元素的ID
                var thisID = $(this).attr("id");

                //判斷目前元素是否勾選
                if ($(this).attr("checked")) {
                    //檢查每個元素
                    $("input[id*='" + thisID + "']").each(function () {
                        $(this).attr("checked", true);  //將其下每一層CheckBox勾選

                        //[取得定義] - 屬性rel (上層編號)
                        var thisLevel = $(this).attr("rel");

                        //[取得定義] - 上層編號
                        var thisUpID = $("#" + thisLevel);

                        //[判斷定義] - 判斷屬性rel是否有定義
                        if (typeof (thisLevel) != "undefined") {
                            //拆解字串
                            var arythisLevel = thisLevel.split("_");
                            for (var i = 0; i < arythisLevel.length - 1; i++) {
                                //判斷上層項目是否勾選
                                if (typeof (thisUpID.attr("checked")) == "undefined") {
                                    thisUpID.attr("checked", true);
                                }
                                //再上層Rel
                                thisLevel = thisUpID.attr("rel");
                                //再上層編號
                                thisUpID = $("#" + thisLevel);
                            }
                        }
                    });
                }
                else {
                    $("input[id*='" + thisID + "']").each(function () {
                        $(this).attr("checked", false);
                    });
                }
            });
        });

        //取得Checkbox已勾選的值
        function GetCbxValue() {
            var tmpID = '';
            $("input[id*='cb_']:checkbox:checked").each(function (i) {
                if (this.value != '') {
                    if (tmpID != '') {
                        tmpID += ',';
                    }
                    tmpID += this.value;
                }
            });
            //將勾選值填入(分類項目不列入)
            $("#hf_RelID").val(tmpID);
            //觸發click事件
            $('#btn_GetRelID').trigger('click');
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>規格分類關聯設定</span>
    </div>
    <div class="h2Head">
        <h2>
            規格分類關聯設定</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div id="sidetreecontrol" class="MenuSecControl" style="padding-top: 5px">
        <a href="?#">收合</a> | <a href="?#">展開</a> | <a href="#" onclick="location.reload();">
            重整</a> |
        <input type="button" onclick="GetCbxValue()" value="儲存勾選" class="btnBlock colorRed" />
        <asp:Button ID="btn_GetRelID" runat="server" Text="Button" OnClick="btn_GetRelID_Click"
            ValidationGroup="Save" Style="display: none" />
        <asp:HiddenField ID="hf_RelID" runat="server" />
        | <span class="styleGreen">搜尋規格分類：</span><asp:TextBox ID="tb_SpecCateName" runat="server"
            Width="200px" CssClass="tooltip_html"></asp:TextBox>
        <asp:TextBox ID="tb_SpecCateID" runat="server" ValidationGroup="Search" Style="display: none"></asp:TextBox>
        <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" ValidationGroup="Search"
            class="btnBlock colorGray" />
        <input type="button" value="返回分類列表" onclick="location.href='SpecCategory_Search.aspx?func=Spec';"
            class="btnBlock colorGreen" />
        <asp:RequiredFieldValidator ID="rfv_tb_SpecCateID" runat="server" ErrorMessage="請選擇正確的「規格分類」！"
            ControlToValidate="tb_SpecCateID" ForeColor="Red" Display="Dynamic" ValidationGroup="Search"></asp:RequiredFieldValidator>
    </div>
    <div id="tip1" style="display: none">
        How to use?<br />
        1. 輸入關鍵字:規格分類名稱 (ex:手鉗)<br />
        2. 按下空白鍵
    </div>
    <hr class="MenuSecondHr" />
    <div class="MenuSecond">
        <asp:Literal ID="lt_TreeView" runat="server"></asp:Literal>
    </div>
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="N"
        ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
