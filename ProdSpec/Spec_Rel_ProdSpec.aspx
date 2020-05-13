<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Rel_ProdSpec.aspx.cs"
    Inherits="Spec_Rel_ProdSpec" %>

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
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
    <script src="../js/public.js" type="text/javascript"></script>
    <script type="text/javascript">
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
                    $("#val_ModelNo").val(ui.item.value);

                    //自動新增項目
                    Add_Item();
                }
            });

            /* Autocomplete - 規格欄位 */
            $("#tb_SpecName").autocomplete('../AC_Spec.aspx');
            $("#tb_SpecName").result(
                function (event, data, formatted) {
                    $("#tb_SpecID").val(data[1]);
                    $("#lb_SpecID").text(data[0]);
                }
	        );

            /* Tooltip - 使用Html */
            $("#tb_SpecName").poshytip({
                className: 'tip-darkgray',
                bgImageFrameSize: 9,
                offsetX: -10,
                offsetY: 10,
                fade: false,
                content: $('#tip1').html()
            });
            /* Tooltip - 使用Html */
            $("#tb_ModelNo").poshytip({
                className: 'tip-darkgray',
                bgImageFrameSize: 9,
                offsetX: -10,
                offsetY: 10,
                fade: false,
                content: $('#tip2').html()
            });

            //Click事件 - 規格欄位
            $('#tb_SpecName').click(function () {
                $(this).select();
            });
            //Click事件 - 品號
            $('#tb_ModelNo').click(function () {
                $(this).select();
            });

        });

    </script>
    <script type="text/javascript">
        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_Item() {
            var valModelNo = $("#val_ModelNo");
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjVal = valModelNo.val();
            if (ObjVal == "") {
                alert('品號空白!');
                return;
            }
            var NewItem = '<li id="li_' + ObjId + '">';
            NewItem += '    <table>';
            NewItem += '        <tr>';
            NewItem += '            <td class="FEItemIn">';
            NewItem += '                 <input type="text" class="Item_Val" maxlength="40" style="width:200px" value="' + ObjVal + '" readonly />';
            NewItem += '            </td>';
            NewItem += '            <td class="FEItemControl">';
            NewItem += '                <a href="javascript:Delete_Item(\'' + ObjId + '\');">刪除</a>';
            NewItem += '            </td>';
            NewItem += '        </tr>';
            NewItem += '    </table>';
            NewItem += '</li>';
            $("#ul_Item_List").append(NewItem);
        }

        /* 刪除項目 */
        function Delete_Item(TarObj) {
            $("#li_" + TarObj).remove();
        }

        function Delete_AllItem() {
            $("#ul_Item_List li").each(
               function (i, elm) {
                   $(elm).remove();
               });
        }
        /* 時間function */
        Date.prototype.Format = function (fmt) { //author: meizz
            var o = {
                "M+": this.getMonth() + 1,                 //月份
                "d+": this.getDate(),                    //日
                "h+": this.getHours(),                   //小時
                "m+": this.getMinutes(),                 //分
                "s+": this.getSeconds(),                 //秒
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度
                "S": this.getMilliseconds()             //毫秒
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }

    /* 取得各項目欄位值
    分隔符號 : ||||
    */
    function Get_Item() {
        var Item_Val = $("#tb_Item_Val");
        Item_Val.val("");
        $("#ul_Item_List li .Item_Val").each(
                function (i, elm) {
                    var OldCont = Item_Val.val();
                    if (OldCont == '') {
                        Item_Val.val($(elm).val());
                    } else {
                        Item_Val.val(OldCont + '||||' + $(elm).val());
                    }
                }
            );
    }
    //----- 動態欄位 End -----
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>功能關聯</span>
    </div>
    <div class="h2Head">
        <h2>
            規格欄位 ←關聯→ 產品主檔</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <div class="Sift">
        <ul>
            <li>規格欄位：
                <asp:TextBox ID="tb_SpecName" runat="server" Width="250px">
                </asp:TextBox>
                <asp:TextBox ID="tb_SpecID" runat="server" Style="display: none">
                </asp:TextBox>
                <asp:Button ID="btn_Search" runat="server" Text="選擇" OnClick="btn_Search_Click" Width="90px"
                    CssClass="btnBlock colorGray" />
                <asp:Label ID="lb_SpecID" runat="server" CssClass="styleBlue"></asp:Label>
            </li>
        </ul>
        <asp:PlaceHolder ID="ph_Search" runat="server">
            <ul>
                <li>關聯品號：
                    <asp:TextBox ID="tb_ModelNo" runat="server" MaxLength="40" Width="250px">
                    </asp:TextBox>
                    <asp:TextBox ID="val_ModelNo" runat="server" Style="display: none">
                    </asp:TextBox>
                    <input type="button" class="Add_Item btnBlock colorBlue" onclick="Add_Item();" value="新增"
                        style="width: 90px" />
                    |
                    <asp:Button ID="btn_Save" runat="server" Text="儲存關聯" OnClick="btn_Save_Click" OnClientClick="Get_Item();"
                        Width="90px" CssClass="tooltip btnBlock colorRed" ToolTip="設定完成後，請按下儲存鈕。" />
                </li>
            </ul>
        </asp:PlaceHolder>
    </div>
    <div id="tip1" style="display: none">
        How to use?<br />
        1. 輸入關鍵字:規格代號 (ex:006)<br />
        2. 輸入關鍵字:規格名稱 (ex:材質)
    </div>
    <div id="tip2" style="display: none">
        How to use?<br />
        1. 輸入品號關鍵字,不限大小寫 (ex:1PK)<br />
        2. 至少輸入 2 個字元
    </div>
    <div>
        <!-- 動態新增品號 -->
        <asp:TextBox ID="tb_Item_Val" runat="server" ToolTip="項目欄位值組合" Style="display: none;">
        </asp:TextBox>
        <ul id="ul_Item_List" class="FEditCon_NoArrow">
            <asp:Literal ID="lt_Items" runat="server"></asp:Literal>
        </ul>
    </div>
    <!-- 備註說明 -->
    <div class="ListIllusArea">
        <div class="JQ-ui-state-highlight">
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>針對品號，設定該品號特有的規格欄位</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>此為關聯性設定，若要維護規格欄位，需前往「<a href="Spec_Search.aspx">規格欄位</a>」</div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span>這功能設定為一對多模式，即規格欄位(一) 對 品號(多)</div>
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
