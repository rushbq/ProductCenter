<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_DtlEdit.aspx.cs" Inherits="Prod_DtlEdit" %>

<%@ Register Src="Ascx_TabMenu.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
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
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <script type="text/javascript">
        $(function () {
            //fancybox - 開窗
            $(".selectBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });

            //導向連結
            $("input[type=button].goUrl").click(function () {
                location.href = $(this).attr('rel');
            });

            //資料展開/收合
            $(".DTtoggle").click(function () {
                //取得此物件的rel屬性值 (#xx)
                var _this = $(this).attr("rel");
                var _img = $(this).attr("imgrel");
                //判斷指定元素是否隱藏
                if ($(_this).css("display") == "none") {
                    $(this).attr("title", "收合");
                    $(_this).show();
                    $(_img).attr("src", "../images/icon_top.png");
                } else {
                    $(this).attr("title", "展開");
                    $(_this).hide();
                    $(_img).attr("src", "../images/icon_down.png");
                }
                return false;
            });

            //快速連結, 滑至指定目標
            $(".scrollme").click(function () {
                //取得元素
                var _thisID = $(this).attr("href");

                //滑動至指定ID
                $('html, body').animate({
                    scrollTop: $(_thisID).offset().top
                }, 600);
            });

            //Click事件 - 儲存
            $(".doSave").click(function () {
                //取得目前按的按鈕，是第幾區
                var relVal = $(this).attr("rel");

                //Block畫面
                BlockPage('<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>');
                //Ajax處理 - 儲存資料(品號, 處理頁)
                SaveData('<%=Param_ModelNo %>', 'Prod_DtlEdit_Ajax.aspx', relVal);
            });

            //Click事件 - 一般欄位click
            $("input[type=text]").click(function () {
                $(this).select();
            });

            //Click事件 - 清除選項值 (資料刪除)
            $(".Remove").click(function () {
                if (confirm("確認清除此規格的明細值!?") == false) {
                    return;
                }
                //[取得參數]
                var thisID = $(this);
                var SpecID = thisID.attr("SpecID");         //規格編號
                var SpecType = thisID.attr("SpecType");     //規格型態
                var SpecClass = '<%=Param_SpecClass %>';    //規格類別
                var ModelNo = '<%=Param_ModelNo %>';        //品號
                var CateID = thisID.attr("CateID");         //規格類別編號

                $.ajax({
                    url: 'Prod_DtlEdit_Action.aspx' + '?' + new Date().getTime(),
                    data: {
                        ValidCode: '<%=ValidCode %>',
                        SpecID: SpecID,
                        SpecClass: SpecClass,
                        ModelNo: ModelNo,
                        CateID: CateID,
                        Type: 'Remove'
                    },
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    dataType: "html"

                }).done(function (html) {
                    if (html.indexOf("OK") != -1) {
                        //隱藏清除鈕
                        thisID.hide();
                        /*
                        判斷規格型態
                        SINGLESELECT / MULTISELECT ,為選單值的清除方式
                        DEVIATIONINT / BETWEENINT / INTGREATERSMALL / RATIO ,為前後兩欄式
                        */
                        switch (SpecType.toUpperCase()) {
                            case "SINGLESELECT":
                            case "MULTISELECT":
                                //變更連結文字
                                $("#url_" + CateID + SpecID).removeClass().addClass('selectBox styleGreen').text('請先選擇項目');
                                //清除欄位值
                                $("#val_" + CateID + SpecID).val('');
                                break;

                            case "DEVIATIONINT":
                            case "BETWEENINT":
                            case "INTGREATERSMALL":
                            case "RATIO":
                                //清除欄位值
                                var val_min = $("#val_min_" + CateID + SpecID);
                                var val_max = $("#val_max_" + CateID + SpecID);
                                val_min.val('');
                                val_min.attr("dataid", "");
                                val_max.val('');
                                val_max.attr("dataid", "");

                                break;

                            default:
                                //清除欄位值
                                $("#val_" + CateID + SpecID).val('').attr("dataid", "");
                                break;
                        }

                    } else {
                        alert(html);
                    }

                }).fail(function (jqXHR, textStatus) {
                    alert('清除失敗!' + textStatus);
                });

            });
        });

        //BlockUI - 鎖定畫面
        function BlockPage(inputTxt) {
            $.blockUI({
                message: inputTxt,
                css: {
                    width: '250px',
                    border: 'none',
                    padding: '5px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .8,
                    color: '#fff'
                }
            });
        }

        //BlockUI - 解鎖畫面
        function unBlockPage(inputTxt) {
            $.unblockUI({
                onUnblock: function () { alert(inputTxt); }
            });
        }
    </script>
    <script type="text/javascript">
        //Ajax 處理 - 資料儲存
        function SaveData(ModelNo, reqUrl, relVal) {
            /* 取得各欄位元素 Start */
            //宣告陣列
            var aryVal = [];
            //放值的欄位
            var valEle = $("[id*=val_]");

            //[判斷] - 是否有必填欄位
            var err = 0;
            for (var row = 0; row < valEle.length; row++) {
                var _thisID = $("#" + valEle[row].id);
                //判斷必填欄位
                if (_thisID.attr("required") == 'Y' && valEle[row].value == '') {
                    err++;
                }
            }
            if (err != 0) {
                alert('(*)為必填欄位, 請輸入必填欄位!');
                return;
            }

            //[取值]
            for (var row = 0; row < valEle.length; row++) {
                var _thisID = $("#" + valEle[row].id);
                var inputID = valEle[row].id;   //欄位編號
                var getSpecID = _thisID.attr("specid");     //SpecID
                var getKind = _thisID.attr("kind");     //輸入方式
                var getVal = _thisID.attr("value");     //輸入值
                var getOptGid = _thisID.attr("optgid");     //選單單頭編號
                var getDataID = _thisID.attr("dataid");     //DB欄位值ID
                var getCateID = _thisID.attr("cateid");     //規格類別ID
                var getSymbol = $("select#symbol_" + getCateID + getSpecID);

                //將值塞入陣列(Json), 值不為空值才處理
                if (getVal != '') {
                    //判斷是否有符號
                    var symbolValue = '';
                    if (typeof (getSymbol) != "undefined") { symbolValue = getSymbol.val(); }

                    //新增陣列
                    aryVal.push({
                        inputID: inputID,
                        SpecID: getSpecID,
                        Kind: getKind,
                        OptionGid: getOptGid,
                        DataID: getDataID,
                        Symbol: symbolValue,
                        CateID: getCateID,
                        Val: getVal
                    });

                }
            }
            /* 取得各欄位元素 End */

            //[Ajax] - 前置處理
            var showResult = $("div#Result");   //顯示執行結果
            var divloading = $(".Loading");  //Loading
            var btn_Save = $("input.btn_Save"); //送出鈕

            //Lock按鈕
            btn_Save.attr("disabled", "disabled");
            //清空Html
            showResult.html('');
            //顯示Loading
            divloading.show();
            //目前時間
            var getNow = new Date();

            //[Ajax] - 資料處理
            var request = $.ajax({
                url: reqUrl + '?' + getNow.getTime(),
                data: {
                    dataVal: JSON.stringify(aryVal),
                    ModelNo: '<%=Param_ModelNo %>',
                    SpecClass: '<%=Param_SpecClass %>',
                    ValidCode: '<%=ValidCode %>'
                },
                type: "POST",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "html"
            });

            request.done(function (response) {
                //隱藏Loading
                divloading.hide();
                //顯示按鈕
                btn_Save.removeAttr("disabled");
                if (response.indexOf("error") != -1) {
                    //解鎖畫面
                    unBlockPage('資料儲存失敗！' + response);
                    //載入回傳的Html
                    showResult.html(response);
                } else {
                    //載入回傳的Html
                    /*
                    unBlockPage('資料儲存成功');
                    var h = getNow.getHours();
                    var m = getNow.getMinutes();
                    var s = getNow.getSeconds();
                    m = checkTime(m);
                    s = checkTime(s);

                    showResult.html('資料儲存成功...' + h + ":" + m + ":" + s);
                    */

                    alert('資料儲存成功！');
                    history.go(0);
                }
            });

            request.fail(function (jqXHR, textStatus) {
                //alert("Request failed: " + textStatus);
                showResult.html('<div class="styleRed">資料載入發生錯誤，請重新整理頁面。</div>');
                //隱藏Loading
                divloading.hide();
                //顯示按鈕
                btn_Save.removeAttr("disabled");
                //解鎖畫面
                unBlockPage('資料儲存失敗，請聯絡系統管理人員！');
            });

            if (request != null) {
                request.onreadystatechange = null;
                request.abort = null;
                request = null;
            }
        }

        //判斷時間格式, 不滿10 補0
        function checkTime(i) {
            if (i < 10) {
                i = "0" + i;
            }
            return i;
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">
                <%=Navi_系統首頁%></a>&gt;<a><%=Navi_產品資料庫%></a>&gt;<span><%=Navi_產品資料%></span>
        </div>
        <div class="h2Head">
            <h2>
                <div style="float: left">
                    <%=Navi_產品資料%>
                -
                <asp:Label ID="lb_Model_No" runat="server" CssClass="styleRed B"><%=Param_ModelNo %></asp:Label>
                </div>
                <div style="float: right; padding-right: 5px;">
                    <input type="button" class="goUrl btnBlock colorDark" value="切換至檢視頁" rel="Prod_DtlView.aspx?Model_No=<%=Server.UrlEncode(Param_ModelNo) %>" />
                </div>
            </h2>
        </div>
        <div class="SysTab">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <table class="TableModify">
            <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
        </table>
        <div id="Result" class="styleBluelight B" style="height: 40px;">
        </div>
        <div class="ListIllusArea">
            <div class="JQ-ui-state-default">
                <span id="notice" class="JQ-ui-icon ui-icon-info"></span>&nbsp;<span class="B Font13">注意事項：</span><br />
                <table class="List1" width="100%">
                    <tr class="TrGray">
                        <td>
                            <ul style="list-style-type: decimal">
                                <li class="styleBlue">如何輸入上標文字：請輸入符號「<span class="styleRed">^</span>」作為區隔，系統會自動判別並轉換。(ex:
                                10<sup>3</sup>, 請輸入<span class="styleRed">10^3</span>)</li>
                            </ul>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="Y" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
