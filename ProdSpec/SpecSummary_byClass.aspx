<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecSummary_byClass.aspx.cs"
    Inherits="SpecSummary_byClass" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- superTables Start --%>
    <link href="../js/superTables/superTables.min.css" rel="stylesheet" type="text/css" />
    <script src="../js/superTables/superTables.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/superTables/jquery.superTable.js"></script>
    <%-- superTables End --%>
    <script src="../js/public.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //fancybox - 編輯鈕
            $(".EditBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'none', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });

            //Click事件 - 取得條件資料
            $("input#btn_Search").click(function () {
                //取得欄位輸入值
                var ClassID = $("select#ddl_SpecClass").val();
                if (ClassID == '') {
                    alert('請選擇主要類別!');
                    return false;
                }
                //取得比對類別值
                Get_Item();
                //取得比對類別
                var CpClassID = $("#tb_Item_Val").val();
                if (CpClassID == '') {
                    alert('至少需有一項比對類別!');
                    return false;
                }

                //判斷搜尋條件
                var reqUrl = 'SpecSummary_byClass_Ajax.aspx';
                //取得資料
                Get_Data(ClassID, CpClassID, reqUrl);
            });
        });
    </script>
    <script type="text/javascript">
        //----- Script Functions -----
        //superTables - 凍結窗格
        function freezeTable() {
            $("#lv_List").toSuperTable({
                width: "98%",
                height: "450px",
                fixedCols: 2,
                headerRows: 2,
                cssSkin: "sDefault"
            }).find("tr:odd").addClass("altRow");
        }

        //取得資料 - Ajax
        function Get_Data(ClassID, CpClassID, reqUrl) {
            var showResult = $("div#ResultHtml");   //顯示執行結果
            var divloading = $("div#Loading");      //Loading
            var btn_Search = $("input#btn_Search"); //查詢鈕
            var divCpClass = $("div#CpClass");      //比對類別欄

            //Lock查詢按鈕
            btn_Search.attr("disabled", "disabled");
            //清空Html
            showResult.html('');
            //顯示Loading
            divloading.show();
            //隱藏比對類別欄
            divCpClass.hide();

            var request = $.ajax({
                url: reqUrl + '?' + new Date().getTime(),
                data: { SpecClass: ClassID, CpSpecClass: CpClassID },
                type: "POST",
                dataType: "html"
            });

            request.done(function (response) {
                //載入回傳的Html
                showResult.html(response);
                //隱藏Loading
                divloading.hide();
                //顯示查詢按鈕
                btn_Search.removeAttr("disabled");
                //呼叫 - 凍結視窗
                freezeTable();
                //載入在Ajax頁面中的Click事件
                Custom_Click();

            });

            request.fail(function (jqXHR, textStatus) {
                //alert("Request failed: " + textStatus);
                showResult.html('<div class="styleRed">資料載入發生錯誤，請重新整理頁面。</div>');
                //隱藏Loading
                divloading.hide();
                //顯示查詢按鈕
                btn_Search.removeAttr("disabled");
                //顯示比對類別欄
                divCpClass.show();
            });

            if (request != null) {
                request.onreadystatechange = null;
                request.abort = null;
                request = null;
            }
        }

        //Ajax回傳頁上的Click
        function Custom_Click() {
            //Click事件 - 加入關聯
            $(".AddRel").click(function () {
                var thisID = $(this);
                var SpecID = thisID.attr("SpecID");    //規格編號
                var ClassID = thisID.attr("ClassID");  //分類編號

                $.ajax({
                    url: 'SpecSummary_byClass_Action.aspx' + '?' + new Date().getTime(),
                    data: {
                        ValidCode: '<%=ValidCode %>',
                        SpecID: SpecID,
                        ClassID: ClassID,
                        Type: 'AddRel'
                    },
                    type: "POST",
                    dataType: "html"
                }).done(function (html) {
                    if (html.indexOf("OK") != -1) {
                        thisID.removeClass("AddRel").addClass("styleGraylight").text("已設定, 請重新查詢");
                    } else {
                        thisID.text(html);
                    }

                }).fail(function (jqXHR, textStatus) {
                    thisID.text("設定失敗...");
                });

            });

            //Click事件 - 移除關聯
            $(".RemoveRel").click(function () {
                var thisID = $(this);
                var SpecID = thisID.attr("SpecID");    //規格編號
                var ClassID = thisID.attr("ClassID");  //分類編號

                $.ajax({
                    url: 'SpecSummary_byClass_Action.aspx' + '?' + new Date().getTime(),
                    data: {
                        ValidCode: '<%=ValidCode %>',
                        SpecID: SpecID,
                        ClassID: ClassID,
                        Type: 'RemoveRel'
                    },
                    type: "POST",
                    dataType: "html"
                }).done(function (html) {
                    if (html.indexOf("OK") != -1) {
                        thisID.removeClass("RemoveRel").addClass("styleGraylight").text("已移除, 請重新查詢");
                    } else {
                        thisID.text(html);
                    }

                }).fail(function (jqXHR, textStatus) {
                    thisID.text("移除失敗...");
                });

            });
        }
    </script>
    <script type="text/javascript">
        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_Item() {
            //判斷是否已選擇
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjVal = $("select#ddl_CpSpecClass").val();
            if (ObjVal == "") {
                alert('請選擇比對分類!');
                return;
            }
            //判斷動態欄位是否隱藏
            if (!$("div#CpClass").is(":visible")) {
                $("div#CpClass").show();
            }
            //警示-超過 n 個項目
            var limitNum = 5;
            if ($("#ul_Item_List li").length >= limitNum) {
                alert('[警示訊息]\n若比對項目超過 ' + limitNum + ' 項，有可能會讓電腦變慢!');
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
        分隔符號 : |
        */
        function Get_Item() {
            //清空欄位值
            var Item_Val = $("#tb_Item_Val");
            Item_Val.val("");
            $("#ul_Item_List li .Item_Val").each(
                function (i, elm) {
                    var OldCont = Item_Val.val();
                    if (OldCont == '') {
                        Item_Val.val($(elm).val());
                    } else {
                        Item_Val.val(OldCont + '|' + $(elm).val());
                    }
                }
            );
        }
        //----- 動態欄位 End -----
    </script>
    <style type="text/css">
        .altRow
        {
            background-color: #efefef;
        }
    </style>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格維護</a>&gt;<span>by 類別</span>
    </div>
    <div class="h2Head">
        <h2>
            規格維護表 -by 類別</h2>
    </div>
    <!-- 功能區 Start -->
    <div class="Sift">
        <ul>
            <li class="tooltip" title="要對照的主要類別">主要分類：
                <asp:DropDownListGP ID="ddl_SpecClass" runat="server">
                </asp:DropDownListGP>
            </li>
            <li><a href="SpecSummary_byClass_Excel.aspx" target="_blank">
                <img src="../images/System/ico_excel2.png" width="20" border="0" />
                匯出所有分類</a> </li>
        </ul>
        <ul>
            <li class="tooltip" title="要對照的比對類別<BR>按下「加入」可多筆新增">比對分類：
                <asp:DropDownListGP ID="ddl_CpSpecClass" runat="server">
                </asp:DropDownListGP>
                <input type="button" class="Add_Item btnBlock colorBlue" onclick="Add_Item();" value="加入比對項目">
                <input type="button" id="btn_Search" value="查詢" class="btnBlock colorGray">
            </li>
        </ul>
    </div>
    <div id="CpClass" style="display: none">
        <div style="padding: 5px 0px 0px 20px">
            <span class="SiftLight">(重複加入的項目將會自動過濾)</span> <a class="SysLink" onclick="$('#CpClass').hide();"
                style="cursor: pointer">隱藏項目清單</a>
        </div>
        <!-- 動態新增 -->
        <asp:TextBox ID="tb_Item_Val" runat="server" ToolTip="項目欄位值組合" Style="display: none;"></asp:TextBox>
        <ul id="ul_Item_List" class="FEditCon_NoArrow">
            <asp:Literal ID="lt_Items" runat="server"></asp:Literal>
        </ul>
    </div>
    <!-- 功能區 End -->
    <!-- 內容區 Start -->
    <div>
        <div id="ResultHtml">
        </div>
        <div id="Loading" style="display: none; padding: 10px 0px 0px 20px;">
            <img src="../images/loadingAnimation.gif" /></div>
    </div>
    <!-- 內容區 End -->
    </form>
</body>
</html>
