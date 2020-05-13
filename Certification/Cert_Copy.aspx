<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cert_Copy.aspx.cs" Inherits="Cert_Copy" %>

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
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- fancybox helpers Start --%>
    <script src="../js/fancybox/helpers/jquery.fancybox-thumbs.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-thumbs.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <script type="text/javascript">
        $(function () {
            //Click事件 - 圖片選擇
            $('input[id*="cb_CertItem"]').click(function () {
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

            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'none',
                nextEffect: 'none',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    overlay: {
                        opacity: 0.8,
                        css: {
                            'background-color': '#000'
                        }
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    },
                    buttons: {}
                }
            });

            //fancybox - 檢視
            $(".ViewBox").fancybox({
                type: 'iframe',
                width: '80%',
                height: '90%',
                fitToView: false,
                autoSize: false,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });

            /* Autocomplete - 群組分類(來源品號) */
            $("#tb_ModelNo_from").catcomplete({
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
                    $(this).val(ui.item.value);
                    $("#val_ModelNo_from").val(ui.item.value);
                    $("#rev_ModelNo_from").show();
                }
            });

            /* Autocomplete - 群組分類(目標品號) */
            $("#tb_ModelNo_to").catcomplete({
                minLength: 2,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_Model_No_Json.aspx",
                        data: {
                            q: request.term,
                            CurrModelNo: $("#tb_ModelNo_from").val()
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
                    $(this).val(ui.item.value);
                    $("#val_ModelNo_to").val(ui.item.value);
                    $("#rev_ModelNo_to").show();
                    Add_Item();
                }
            });

        });

        //判斷選項是否已勾選
        function check_Select() {
            //巡覽Checkbox - 判斷選項是否已勾選
            $('input[id*="cb_CertItem"]').each(function () {
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

        //清除Autocomplete結果欄位
        function clear_Field(objName) {
            switch (objName) {
                case 'Step1':
                    $("#tb_ModelNo_from").val('');
                    $("#tb_ModelNo_from").removeClass("input01");
                    $("#val_ModelNo_from").val('');
                    $("#rev_ModelNo_from").hide();
                    break;

                case 'Step3':
                    $("#tb_ModelNo_to").val('');
                    $("#tb_ModelNo_to").removeClass("input01");
                    $("#val_ModelNo_to").val('');
                    $("#rev_ModelNo_to").hide();
                    break;

                default:

            }
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

        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_Item() {
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjVal = $("#val_ModelNo_to").val();
            if (ObjVal == "") {
                alert('目標品號空白!');
                return;
            } else {
                $("#tb_ModelNo_to").val('');
                $("#tb_ModelNo_to").removeClass("input01");
                $("#tb_ModelNo_to").focus();
                //$("#val_ModelNo_to").val('');
                $("#rev_ModelNo_to").hide();
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
        <a href="../Main.aspx">系統首頁</a>&gt;<a>認證資料庫</a>&gt;<span>證書複製</span>
    </div>
    <div class="h2Head">
        <h2>
            證書複製</h2>
    </div>
    <table class="TableModify">
        <!-- Step 1 Start -->
        <asp:PlaceHolder ID="ph_Step1" runat="server">
            <tr class="ModifyHead">
                <td colspan="2">
                    Step 1：選擇來源品號<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead" style="width: 10%">
                    來源品號
                </td>
                <td class="TableModifyTd">
                    <asp:TextBox ID="tb_ModelNo_from" runat="server" MaxLength="40" Width="200px"></asp:TextBox>
                    <asp:TextBox ID="val_ModelNo_from" runat="server" Style="display: none"></asp:TextBox>&nbsp;
                    <img id="rev_ModelNo_from" src="../images/delete.png" onclick="clear_Field('Step1');"
                        title="清除" alt="清除" style="display: none; cursor: pointer;" />
                    <asp:RequiredFieldValidator ID="rfv_val_ModelNo_from" runat="server" ErrorMessage="-&gt; 請選擇正確的「品號」!"
                        ForeColor="Red" ControlToValidate="val_ModelNo_from" Display="Dynamic" ValidationGroup="Step1"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr id="tr_Step1" runat="server">
                <td class="SubmitAreaS" colspan="2">
                    <asp:Button ID="btn_Step1_Next" runat="server" Text="下一步" ValidationGroup="Step1"
                        OnClick="btn_Step1_Next_Click" Width="90px" CssClass="btnBlock colorBlue" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <!-- Step 1 End -->
        <!-- Step 2 Start -->
        <asp:PlaceHolder ID="ph_Step2" runat="server" Visible="false">
            <tr class="ModifyHead">
                <td colspan="2">
                    Step 2：選擇要複製的證書<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTd" colspan="2" width="100%">
                    <div class="styleRed B">
                        * 證書編號為空白的資料，將會略過不進行複製。</div>
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="List1" width="100%">
                                <tr class="tdHead">
                                    <td width="60px">
                                        <asp:CheckBox ID="cbSelectAll" runat="server" Text="ALL" OnClick="selectAll(this)" />
                                    </td>
                                    <td width="130px">
                                        證書類別
                                    </td>
                                    <td width="120px">
                                        證書編號
                                    </td>
                                    <td>
                                        認證指令
                                    </td>
                                    <td width="90px">
                                        發證日期
                                    </td>
                                    <td width="90px">
                                        有效日期
                                    </td>
                                    <td width="70px">
                                        證書
                                    </td>
                                    <td width="70px">
                                        Test<br />
                                        Report
                                    </td>
                                    <td width="80px">
                                        自我宣告
                                    </td>
                                    <td width="80px">
                                        自我檢測
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="trItem" runat="server">
                                <td align="center">
                                    <asp:CheckBox ID="cb_CertItem" runat="server" />
                                </td>
                                <td class="L2MainHead">
                                    <div>
                                        <asp:Literal ID="lt_CertType" runat="server"></asp:Literal></div>
                                    <div style="padding-top: 5px">
                                        <asp:Literal ID="lt_Icon" runat="server"></asp:Literal></div>
                                </td>
                                <td align="center">
                                    <%#Eval("Cert_No")%>
                                </td>
                                <td align="center">
                                    <%#Eval("Cert_Cmd")%>
                                </td>
                                <td align="center">
                                    <%# String.Format("{0:yyyy-MM-dd}", Eval("Cert_ApproveDate"))%>
                                </td>
                                <td align="center">
                                    <%# String.Format("{0:yyyy-MM-dd}", Eval("Cert_ValidDate"))%>
                                </td>
                                <td align="center">
                                    <asp:Literal ID="lt_CertFile" runat="server"></asp:Literal>
                                </td>
                                <td align="center">
                                    <asp:Literal ID="lt_FileTestReport" runat="server"></asp:Literal>
                                </td>
                                <td align="left">
                                    <div style="padding-bottom: 4px">
                                        <span class="styleGraylight">(繁中)</span>&nbsp;
                                        <asp:Literal ID="lt_FileCE" runat="server"></asp:Literal>
                                    </div>
                                    <div style="padding-bottom: 4px">
                                        <span class="styleGraylight">(英文)</span>&nbsp;
                                        <asp:Literal ID="lt_FileCE_enUS" runat="server"></asp:Literal>
                                    </div>
                                    <div>
                                        <span class="styleGraylight">(簡中)</span>&nbsp;
                                        <asp:Literal ID="lt_FileCE_zhCN" runat="server"></asp:Literal>
                                    </div>
                                </td>
                                <td align="left">
                                    <div style="padding-bottom: 4px">
                                        <span class="styleGraylight">(繁中)</span>&nbsp;
                                        <asp:Literal ID="lt_FileCheck" runat="server"></asp:Literal>
                                    </div>
                                    <div style="padding-bottom: 4px">
                                        <span class="styleGraylight">(英文)</span>&nbsp;
                                        <asp:Literal ID="lt_FileCheck_enUS" runat="server"></asp:Literal>
                                    </div>
                                    <div>
                                        <span class="styleGraylight">(簡中)</span>&nbsp;
                                        <asp:Literal ID="lt_FileCheck_zhCN" runat="server"></asp:Literal>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="styleBlue B" style="padding-top: 5px">
                                * 此品號尚未新增資料.....<a href="<%=Application["WebUrl"] %>Certification/Cert_Search.aspx?func=Certification&Model_No=<%=HttpUtility.UrlEncode(this.val_ModelNo_from.Text) %>">前往新增</a></div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </td>
            </tr>
            <%--<tr id="tr_Step2" runat="server">
                <td class="SubmitAreaS" colspan="2">
                    <asp:Button ID="btn_Step2_Prev" runat="server" Text="上一步" ValidationGroup="Step2"
                        OnClick="btn_Step2_Prev_Click" CausesValidation="false" />
                    <asp:Button ID="btn_Step2_Next" runat="server" Text="下一步" ValidationGroup="Step2"
                        OnClick="btn_Step2_Next_Click" />
                </td>
            </tr>--%>
        </asp:PlaceHolder>
        <!-- Step 2 End -->
        <!-- Step 3 Start -->
        <asp:PlaceHolder ID="ph_Step3" runat="server" Visible="false">
            <tr class="ModifyHead">
                <td colspan="2">
                    Step 3：選擇目標品號<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    目標品號
                </td>
                <td class="TableModifyTd">
                    <!-- Autocomplete 品號 -->
                    <asp:TextBox ID="tb_ModelNo_to" runat="server" MaxLength="40" Width="200px"></asp:TextBox>
                    <asp:TextBox ID="val_ModelNo_to" runat="server" Style="display: none"></asp:TextBox>&nbsp;
                    <img id="rev_ModelNo_to" src="../images/delete.png" onclick="clear_Field('Step3');"
                        title="清除" alt="清除" style="display: none; cursor: pointer;" />
                    <!-- 動態新增品號 -->
                    <input type="button" class="Add_Item btnBlock colorGray" onclick="Add_Item();" value="新增項目" />
                    <asp:TextBox ID="tb_Item_Val" runat="server" ToolTip="項目欄位值組合" Style="display: none;"></asp:TextBox>
                    <hr />
                    <ul id="ul_Item_List" class="FEditCon_NoArrow">
                        <asp:Literal ID="lt_Items" runat="server"></asp:Literal>
                    </ul>
                </td>
            </tr>
            <tr id="tr_Step3" runat="server">
                <td class="SubmitAreaS" colspan="2">
                    <asp:Button ID="btn_Step3_Prev" runat="server" Text="上一步" ValidationGroup="Step3"
                        OnClick="btn_Step3_Prev_Click" CausesValidation="false" Width="90px" CssClass="btnBlock colorGreen" />
                    <asp:Button ID="btn_Step3_Next" runat="server" Text="開始複製" ValidationGroup="Step3"
                        OnClick="btn_Step3_Next_Click" Width="90px" CssClass="btnBlock colorBlue" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <!-- Step 3 End -->
        <asp:PlaceHolder ID="ph_Message" runat="server" Visible="false">
            <tr class="ModifyHead">
                <td colspan="2">
                    複製結果<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    說明
                </td>
                <td class="TableModifyTd">
                    <asp:Label ID="lb_Status" runat="server"></asp:Label>
                    <asp:Panel ID="pl_Result" runat="server">
                        <div class="styleGraylight" style="padding-top: 5px">
                            複製完成，是否要繼續複製其他資料，<asp:LinkButton ID="lbtn_Yes" runat="server" OnClick="lbtn_Yes_Click">是</asp:LinkButton>
                            /
                            <asp:LinkButton ID="lbtn_No" runat="server" OnClick="lbtn_No_Click">否</asp:LinkButton>。
                        </div>
                        <div class="styleBlue" style="padding-top: 5px">
                            <asp:Literal ID="lt_ViewUrl" runat="server"></asp:Literal></div>
                    </asp:Panel>
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
    </form>
</body>
</html>
