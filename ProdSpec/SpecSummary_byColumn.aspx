<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SpecSummary_byColumn.aspx.cs"
    Inherits="SpecSummary_byColumn" %>

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
    <%-- autocomplete Start --%>
    <link href="../js/autocomplete/jquery.autocomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/autocomplete/jquery.autocomplete.js" type="text/javascript"></script>
    <%-- autocomplete End --%>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- superTables Start --%>
    <link href="../js/superTables/superTables.min.css" rel="stylesheet" type="text/css" />
    <script src="../js/superTables/superTables.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/superTables/jquery.superTable.js"></script>
    <%-- superTables End --%>
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

            /* Autocomplete - 規格欄位 */
            $("#tb_SpecName").autocomplete('../AC_Spec.aspx');
            $("#tb_SpecName").result(
                function (event, data, formatted) {
                    $("#tb_SpecID").val(data[1]);
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

            //Click事件 - 取得條件資料
            $("input#btn_Search").click(function () {
                //取得欄位輸入值
                var SpecID = $("input#tb_SpecID").val();
                if (SpecID == '') {
                    alert('請選擇正確的規格欄位!');
                    return false;
                }

                //判斷搜尋條件
                var reqUrl = 'SpecSummary_byColumn_Ajax.aspx';
                //取得資料
                Get_Data(SpecID, reqUrl);
            });
        });
    </script>
    <script type="text/javascript">
        //----- Script Functions -----
        //superTables - 凍結窗格
        function freezeTable() {
            $("#lv_List").toSuperTable({
                width: "98%",
                height: "400px",
                fixedCols: 1,
                headerRows: 2,
                cssSkin: "sDefault"
            }).find("tr:odd").addClass("altRow");
        }

        //取得資料 - Ajax
        function Get_Data(SpecID, reqUrl) {
            var showResult = $("div#ResultHtml");   //顯示執行結果
            var divloading = $("div#Loading");      //Loading
            var btn_Search = $("input#btn_Search"); //查詢鈕

            //Lock查詢按鈕
            btn_Search.attr("disabled", "disabled");
            //清空Html
            showResult.html('');
            //顯示Loading
            divloading.show();

            var request = $.ajax({
                url: reqUrl + '?' + new Date().getTime(),
                data: { SpecID: SpecID },
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
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格維護</a>&gt;<span>by 欄位</span>
    </div>
    <div class="h2Head">
        <h2>
            規格維護表 -by 欄位</h2>
    </div>
    <!-- 功能區 Start -->
    <div class="Sift">
        <ul>
            <li>搜尋規格欄位：
                <asp:TextBox ID="tb_SpecName" runat="server" Width="250px" CssClass="tooltip_html"></asp:TextBox>
                <asp:TextBox ID="tb_SpecID" runat="server" Style="display: none"></asp:TextBox>
            </li>
            <li>
                <input type="button" id="btn_Search" value="查詢" class="btnBlock colorGray">
            </li>
        </ul>
    </div>
    <div id="tip1" style="display: none">
        How to use?<br />
        1. 輸入關鍵字:規格代號 (ex:006)<br />
        2. 輸入關鍵字:規格名稱 (ex:材質)
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
