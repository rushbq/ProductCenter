<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_BOM_DtlEdit.aspx.cs"
    Inherits="Prod_BOM_DtlEdit" %>

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
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
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

            //快速連結, 滑至指定目標
            $(".scrollme").click(function () {
                //取得元素
                var _thisID = $(this).attr("href");

                //滑動至指定ID
                $('html, body').animate({
                    scrollTop: $(_thisID).offset().top
                }, 600);
            });

            //Click事件 - 新增
            $(".doInsert").click(function () {
                //Block畫面
                BlockPage('<div style=\"text-align:left\">資料新增中....<BR>請不要關閉瀏覽器或點選其他連結!</div>');

                //Ajax處理 - 新增資料(處理頁, BOMSpecID)
                var BOMSpecID = $(this).attr("bomspecid");
                InsertData('Prod_BOM_DtlEdit_Ajax.aspx', BOMSpecID);
            });

            //Click事件 - 儲存
            $(".Save").click(function () {
                //Block畫面
                //BlockPage('<div style=\"text-align:left\">資料儲存中....<BR>請不要關閉瀏覽器或點選其他連結!</div>');

                //Ajax處理 - 儲存資料(處理頁, BOMSpecID, RowID)
                var BOMSpecID = $(this).attr("bomspecid");
                var RowID = $(this).attr("rowid");

                UpdateData('Prod_BOM_DtlEdit_Ajax.aspx', BOMSpecID, RowID, $(this));
            });

            //Click事件 - 全部儲存(觸發每一個Save)
            $(".SaveALL").click(function () {
                $('.Save').each(function (i) {
                    delayedTrigger($(this), 800 * i);
                });

                function delayedTrigger(elem, delay) {
                    setTimeout(function () { $(elem).trigger('click'); }, delay);
                }
            });

            //Click事件 - 清除明細值 (資料刪除)
            $(".Remove").click(function () {
                var BOMSpecID = $(this).attr("bomspecid");     //BOMSpecID
                var RowID = $(this).attr("rowid");         //RowID

                //Ajax處理 - 刪除資料(BOMSpecID, RowID)
                RemoveData(BOMSpecID, RowID);
            });

            //Click事件 - 清除所有明細值 (資料刪除)
            $(".RemoveALL").click(function () {
                //Ajax處理 - 刪除資料
                RemoveALLData();
            });

            //Click事件 - 一般欄位click
            $("input[type=text]").click(function () {
                $(this).select();
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

        //判斷時間格式, 不滿10 補0
        function checkTime(i) {
            if (i < 10) {
                i = "0" + i;
            }
            return i;
        }
    </script>
    <!-- 新增明細 -->
    <script type="text/javascript">
        //Ajax 處理 - 資料新增
        function InsertData(reqUrl, BOMSpecID) {
            /* 取得各欄位元素 Start */
            //宣告陣列
            var aryVal = [];
            //放值的欄位
            var valEle = $("[id*=" + BOMSpecID + "_newval]");

            //[判斷] - 是否有必填欄位
            var err = 0;
            for (var row = 0; row < valEle.length; row++) {
                //取得每個元素的id
                var _thisID = $("#" + valEle[row].id);

                //判斷必填欄位
                if (_thisID.attr("requiredfld") == 'Y' && _thisID.val() == '') {
                    err++;
                }
            }
            if (err != 0) {
                unBlockPage('(*)為必填欄位, 請輸入必填欄位!');
                return;
            }

            //[取值]
            for (var row = 0; row < valEle.length; row++) {
                var _thisID = $("#" + valEle[row].id);  //取得每個元素的id
                var getKind = _thisID.attr("kind");     //輸入方式
                var getVal = _thisID.attr("value");     //輸入值
                var getOptGid = _thisID.attr("optgid");     //選單單頭編號
                var getSymbol = $("select#newsymbol_" + BOMSpecID);    //符號選單

                //將值塞入陣列(Json), 值不為空值才處理
                if (getVal != '') {
                    //判斷是否有符號
                    var symbolValue = '';
                    if (typeof (getSymbol) != "undefined") { symbolValue = getSymbol.val(); }

                    //新增陣列
                    aryVal.push({
                        SpecID: BOMSpecID,
                        Kind: getKind,
                        OptionGid: getOptGid,
                        DataID: 0,
                        Symbol: symbolValue,
                        Val: getVal
                    });

                }
            }
            /* 取得各欄位元素 End */

            //目前時間
            var getNow = new Date();

            //[Ajax] - 資料處理
            var request = $.ajax({
                url: reqUrl + '?' + getNow.getTime(),
                data: {
                    dataVal: JSON.stringify(aryVal),
                    ModelNo: '<%=Param_ModelNo %>',
                    CateID: '<%=Param_CateID %>',
                    SpecClassID: '<%=Param_SpecClassID %>',
                    SpecID: '<%=Param_SpecID %>',
                    BOMSpecID: BOMSpecID,
                    Type: 'Insert',
                    ValidCode: '<%=ValidCode %>'
                },
                type: "POST",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "html"
            });

            request.done(function (response) {
                if (response.indexOf("error") != -1) {
                    //解鎖畫面
                    unBlockPage('資料儲存失敗！' + response);

                } else {
                    //unBlockPage('資料儲存成功');
                    location.replace('<%=PageUrl %>' + "&r=" + getNow.getTime() + "#" + BOMSpecID);
            }
            });

        request.fail(function (jqXHR, textStatus) {
            //alert("Request failed: " + textStatus);
            //解鎖畫面
            unBlockPage('資料儲存失敗，請聯絡系統管理人員！');
        });

        if (request != null) {
            request.onreadystatechange = null;
            request.abort = null;
            request = null;
        }
    }
    </script>
    <!-- 修改明細 -->
    <script type="text/javascript">
        //Ajax 處理 - 資料儲存
        function UpdateData(reqUrl, BOMSpecID, RowID, thisID) {
            //取得元素 - 載入圖
            var loadImg = $("#load_" + BOMSpecID + RowID);
            //取得元素 - 儲存時間
            var saveTime = $("#time_" + BOMSpecID + RowID);
            //隱藏目前儲存鈕
            DispSaveLoad(thisID, loadImg, "saving");

            /* 取得各欄位元素 Start */
            //宣告陣列
            var aryVal = [];
            //放值的欄位
            var valEle = $("[id*=" + BOMSpecID + "_" + RowID + "_val]");

            //[判斷] - 是否有必填欄位
            var err = 0;
            for (var row = 0; row < valEle.length; row++) {
                //取得每個元素的id
                var _thisID = $("#" + valEle[row].id);

                //判斷必填欄位
                if (_thisID.attr("requiredfld") == 'Y' && _thisID.val() == '') {
                    err++;
                }
            }
            if (err != 0) {
                unBlockPage('(*)為必填欄位, 請輸入必填欄位!');
                return;
            }

            //[取值]
            for (var row = 0; row < valEle.length; row++) {
                var _thisID = $("#" + valEle[row].id);  //取得每個元素的id
                var getKind = _thisID.attr("kind");     //輸入方式
                var getVal = _thisID.attr("value");     //輸入值
                var getOptGid = _thisID.attr("optgid");     //選單單頭編號
                var getSymbol = $("select#symbol_" + BOMSpecID + "_" + RowID);    //符號選單
                var getDataID = _thisID.attr("dataid");     //DB欄位值ID
                var getSort = $("#sort_" + BOMSpecID + "_" + RowID).val();    //排序值

                //將值塞入陣列(Json), 值不為空值才處理
                if (getVal != '') {
                    //判斷是否有符號
                    var symbolValue = '';
                    if (typeof (getSymbol) != "undefined") { symbolValue = getSymbol.val(); }

                    //新增陣列
                    aryVal.push({
                        SpecID: BOMSpecID,
                        Kind: getKind,
                        OptionGid: getOptGid,
                        DataID: getDataID,
                        Symbol: symbolValue,
                        Sort: getSort,
                        Val: getVal
                    });
                }
            }
            /* 取得各欄位元素 End */

            //目前時間
            var getNow = new Date();

            //[Ajax] - 資料處理
            var request = $.ajax({
                url: reqUrl + '?' + getNow.getTime(),
                data: {
                    dataVal: JSON.stringify(aryVal),
                    ModelNo: '<%=Param_ModelNo %>',
                    CateID: '<%=Param_CateID %>',
                    SpecClassID: '<%=Param_SpecClassID %>',
                    SpecID: '<%=Param_SpecID %>',
                    BOMSpecID: BOMSpecID,
                    RowID: RowID,
                    Type: 'Update',
                    ValidCode: '<%=ValidCode %>'
                },
                type: "POST",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "html"
            });

            request.done(function (response) {
                if (response.indexOf("error") != -1) {
                    //失敗
                    //unBlockPage('資料儲存失敗！' + response);
                    saveTime.text("error:資料儲存失敗" + h + ":" + m);

                } else {
                    //unBlockPage('資料儲存成功');
                    //location.replace('<%=PageUrl %>' + "&r=" + getNow.getTime() + "#" + BOMSpecID);

                    //顯示目前儲存鈕
                    DispSaveLoad(thisID, loadImg, "done");
                    //顯示儲存成功時間
                    var h = getNow.getHours();
                    var m = getNow.getMinutes();
                    var s = getNow.getSeconds();
                    m = checkTime(m);
                    s = checkTime(s);
                    saveTime.text("...已儲存" + h + ":" + m);
                }
            });

            request.fail(function (jqXHR, textStatus) {
                //錯誤
                //unBlockPage('資料儲存失敗，請聯絡系統管理人員！');
                saveTime.text("fail:資料儲存失敗" + h + ":" + m);
            });

            if (request != null) {
                request.onreadystatechange = null;
                request.abort = null;
                request = null;
            }
        }

        function DispSaveLoad(objBtn, objImg, objType) {
            switch (objType) {
                case "saving":
                    objBtn.hide();
                    objImg.show();
                    break;

                case "done":
                    objBtn.show();
                    objImg.hide();
                    break;

                default:
                    break;
            }
        }
    </script>
    <!-- 刪除明細 -->
    <script type="text/javascript">
        //刪除單一項目
        function RemoveData(BOMSpecID, RowID) {
            if (confirm("是否確認移除此設定值!?") == false) {
                return;
            }
            $.ajax({
                url: 'Prod_BOM_DtlEdit_Action.aspx' + '?' + new Date().getTime(),
                data: {
                    ModelNo: '<%=Param_ModelNo %>',
                    CateID: '<%=Param_CateID %>',
                    SpecClassID: '<%=Param_SpecClassID %>',
                    SpecID: '<%=Param_SpecID %>',
                    BOMSpecID: BOMSpecID,
                    RowID: RowID,
                    ValidCode: '<%=ValidCode %>',
                    Type: 'Remove'
                },
                type: "POST",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "html"

            }).done(function (html) {
                if (html.indexOf("OK") != -1) {
                    //隱藏
                    $("#" + BOMSpecID + "_" + RowID).slideUp("normal", function () { $(this).remove(); });
                } else {
                    alert(html);
                }

            }).fail(function (jqXHR, textStatus) {
                alert('清除失敗!' + textStatus);
            });
        }

        //刪除所有項目
        function RemoveALLData() {
            if (confirm("是否確認移除所有的BOM設定!?") == false) {
                return;
            }

            //Block畫面
            BlockPage('<div style=\"text-align:left\">資料處理中....<BR>請不要關閉瀏覽器或點選其他連結!</div>');

            $.ajax({
                url: 'Prod_BOM_DtlEdit_Action.aspx' + '?' + new Date().getTime(),
                data: {
                    ModelNo: '<%=Param_ModelNo %>',
                    CateID: '<%=Param_CateID %>',
                    SpecClassID: '<%=Param_SpecClassID %>',
                    SpecID: '<%=Param_SpecID %>',
                    BOMSpecID: 0,
                    RowID: 0,
                    ValidCode: '<%=ValidCode %>',
                    Type: 'RemoveALL'
                },
                type: "POST",
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                dataType: "html"

            }).done(function (html) {
                if (html.indexOf("OK") != -1) {
                    unBlockPage('資料處理完成.');
                    location.replace('<%=PageUrl %>');

                } else {
                    unBlockPage('資料處理發生錯誤,' + html);
                }

            }).fail(function (jqXHR, textStatus) {
                unBlockPage('資料處理失敗！' + textStatus);
            });
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
                    <asp:Label ID="lb_CateName" runat="server"></asp:Label>,
                <asp:Label ID="lb_ModelNo" runat="server"></asp:Label>,
                <asp:Label ID="lb_SpecInfo" runat="server"></asp:Label>
                    | 組合明細編輯
                </div>
                <div style="float: right; padding-right: 5px;">
                    <a class="btn btn-info" href="Prod_DtlEdit.aspx?Model_No=<%= Server.UrlEncode(Param_ModelNo) %>">回上一層</a>
                    <a class="btn btn-warning" href="Prod_BOM_DtlView.aspx?Model_No=<%= Server.UrlEncode(Param_ModelNo) %>&CateID=<%=Param_CateID %>&SpecClassID=<%=Param_SpecClassID %>&SpecID=<%=Param_SpecID %>">前往檢視頁</a>
                </div>
            </h2>
        </div>
        <div class="SysTab3">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <table class="TableModify JQ-ui-state-default" style="line-height: 25px;">
            <tr class="ModifyHead">
                <td colspan="5">
                    <div style="float: left;">
                        組合明細<em class="TableModifyTitleIcon"></em>
                    </div>
                    <div style="float: right;">
                        <input type="button" class="SaveALL btnBlock colorBlue" value="全部儲存" />
                        <input type="button" class="RemoveALL btnBlock colorRed" value="全部刪除" />
                    </div>
                </td>
            </tr>
            <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
        </table>
        <div class="ListIllusArea">
            <div class="JQ-ui-state-default">
                <span id="notice" class="JQ-ui-icon ui-icon-info"></span>&nbsp;<span class="B Font13">注意事項：</span><br />
                <table class="List1" width="100%">
                    <tr class="TrGray">
                        <td>
                            <ul style="list-style-type: decimal">
                                <li class="styleBlue">如何輸入上標文字：請輸入符號「<span class="styleRed">^</span>」作為區隔，系統會自動判別並轉換。(ex:
                                10<sup>3</sup>, 請輸入<span class="styleRed">10^3</span>)</li>
                                <li>若只有單一幾筆資料修改，建議使用「<span class="JQ-ui-icon ui-icon-disk"></span>」</li>
                                <li>「全部儲存」：可批次儲存每個項目，請小心使用。</li>
                            </ul>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <!-- Scroll Bar Icon
-->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="N"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
