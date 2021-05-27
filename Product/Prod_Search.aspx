<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_Search.aspx.cs" Inherits="Prod_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
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
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <%-- Scroll Top 按扭 Start --%>
    <link href="<%=Application["WebUrl"] %>js/scrollTop/scrollTop.css" rel="stylesheet"
        type="text/css" />
    <script src="<%=Application["WebUrl"] %>js/scrollTop/scrollTop.js" type="text/javascript"></script>
    <%-- Scroll Top 按扭 End --%>
    <%-- lazyload Start --%>
    <script src="../js/lazyload/jquery.lazyload.min.js" type="text/javascript"></script>
    <%-- lazyload End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //lazyload - 延遲讀取網頁圖片
            jQuery("img.lazy").lazyload({
                effect: "fadeIn"
            });

            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'elastic',
                nextEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                }
            });

            //fancybox - 編輯鈕
            $(".EditBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });

            /* Autocomplete - 群組分類(品號) */
            $(".Ac-Prod").catcomplete({
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
                    $(this).val(ui.item.value);

                    //trigger Search click
                    $("#btn_Search").trigger("click");
                }
            });

            /* Autocomplete - 群組分類(目錄頁次) */
            $("#tb_VoltoPage").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_VoltoPage.aspx",
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
                    $(this).val(ui.item.value);
                }
            });

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                location.href = 'Prod_Search.aspx';
            });

        });
    </script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript">
        $(function () {
            //tooltip
            $("#tb_Model_No").tooltip({
                html: true,
                title: '<p align="left">How to use?<br/>1. 輸入品號關鍵字,不限大小寫<br />2. 至少輸入 2 個字元</p>',
                trigger: 'focus',
                placement: 'right'
            });

            //Click事件 - 觸發catecomplete
            $('#tb_VoltoPage').click(function () {
                var getVol = ($('#ddl_Vol').val() == '') ? " " : $('#ddl_Vol').val();
                $(this).catcomplete("search", getVol);
            });

            /* 日期選擇器 */
            $("#tb_StartDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#tb_EndDate").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#tb_EndDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#tb_StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });
        });
    </script>
    <%-- bootstrap End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">
                <%=Navi_系統首頁%></a>&gt;<a><%=Navi_產品資料庫%></a>&gt;<span><%=Navi_產品列表%></span>
        </div>
        <div class="h2Head">
            <h2>
                <%=Navi_產品列表%></h2>
        </div>
        <!-- Search Start -->
        <div class="Sift">
            <ul>
                <li>關鍵字：<asp:TextBox ID="tb_Model_No" runat="server" MaxLength="40" Width="180px" CssClass="Ac-Prod styleBlack" placeholder="輸入關鍵字:品號或品名"></asp:TextBox>
                </li>
                <li>指定品號：
                    <asp:TextBox ID="tb_CurrModelNo" runat="server" MaxLength="40" Width="180px" CssClass="Ac-Prod styleBlack" placeholder="輸入品號"></asp:TextBox>
                </li>
                <li>銷售類別：
                <asp:DropDownList ID="ddl_Class_ID" runat="server" CssClass="styleBlack">
                </asp:DropDownList>
                </li>
                <li>主要出貨地：
                    <asp:DropDownList ID="ddl_ShipFrom" runat="server" CssClass="styleBlack">
                        <asp:ListItem Value="">-- 不限 --</asp:ListItem>
                        <asp:ListItem Value="TW">TW</asp:ListItem>
                        <asp:ListItem Value="SH">SH</asp:ListItem>
                    </asp:DropDownList>
                </li>
            </ul>
            <ul>
                <li>上市日期：
                    <asp:TextBox ID="tb_StartDate" runat="server" Style="text-align: center" Width="90px" CssClass="styleBlack" autocomplete="off"></asp:TextBox>&nbsp;
                    ~
                    <asp:TextBox ID="tb_EndDate" runat="server" Style="text-align: center" Width="90px" CssClass="styleBlack" autocomplete="off"></asp:TextBox>

                    <asp:RegularExpressionValidator ID="rev_tb_StartDate" runat="server" ErrorMessage="「開始日期」格式錯誤."
                        ControlToValidate="tb_StartDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                        Display="Dynamic" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                    <asp:RegularExpressionValidator ID="rev_tb_EndDate" runat="server" ErrorMessage="「結束日期」格式錯誤."
                        ControlToValidate="tb_EndDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                        Display="Dynamic" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                </li>
                <li>目錄：
                    <asp:DropDownList ID="ddl_Vol" runat="server" CssClass="styleBlack">
                    </asp:DropDownList>
                    &nbsp;
                    頁次：
                    <asp:TextBox ID="tb_VoltoPage" runat="server" MaxLength="8" CssClass="styleBlack" Width="90px" autocomplete="off"></asp:TextBox>
                    &nbsp;
                    條碼：
                    <asp:TextBox ID="tb_Barcode" runat="server" MaxLength="50" CssClass="styleBlack" Width="130px" autocomplete="off"></asp:TextBox>
                </li>
                <li style="background:none">
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btn btn-success" />
                    <input type="button" id="clear_form" value="重置" title="重置篩選條件" class="btn btn-default" />
                </li>
            </ul>
        </div>
        <!-- Search End -->
        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <tr class="tdHead">
                            <td>品號/品名
                            </td>
                            <td width="180px">條碼/目錄/頁次
                            </td>
                            <td width="80px">上市日期
                            </td>
                            <td width="80px">停售日期
                            </td>
                            <td width="100px">其他連結
                            </td>
                            <td width="80px">功能選項
                            </td>
                            <td width="140px">PDF匯出
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server" style="white-space: nowrap;">
                        <td align="center">
                            <table class="List2" width="100%">
                                <tbody>
                                    <tr>
                                        <asp:Literal ID="lt_Photo" runat="server"></asp:Literal>
                                        <td class="L2Info">
                                            <div class="L2MainHead">
                                                <a href="Prod_View.aspx?Model_No=<%# HttpUtility.UrlEncode(Eval("Model_No").ToString()) %>"
                                                    class="L2MainHead" title="查看明細">
                                                    <%#Eval("Model_No")%></a>
                                            </div>
                                            <div class="L2Info styleGreen" title="銷售類別" style="padding-top: 3px;">
                                                <%#Eval("Class_Name")%>
                                            </div>
                                            <div class="L2Info" style="padding-top: 3px;">
                                                <%#Eval("Model_Name")%>
                                            </div>
                                            <div class="styleGraylight" style="padding-top: 3px;">
                                                主要出貨地：<span class="styleEarth">
                                                    <%#Eval("Ship_From")%></span>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td align="center">
                            <table class="TableS1" width="98%">
                                <tbody>
                                    <tr>
                                        <td class="TableS1TdHead" style="width: 50px;">條碼
                                        </td>
                                        <td>
                                            <%#Eval("BarCode")%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TableS1TdHead">目錄
                                        </td>
                                        <td>
                                            <%#Eval("Catelog_Vol")%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="TableS1TdHead">頁次
                                        </td>
                                        <td>
                                            <%#Eval("Page")%>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td align="center">
                            <%#Eval("Date_Of_Listing").ToString().ToDateString_ERP("-")%>
                        </td>
                        <td align="left">
                            <div><small class="styleGraylight">(官網)</small><br />&nbsp;<%#Eval("Stop_Offer_Date").ToString().ToDateString("yyyy-MM-dd")%></div>
                            <div title="來源:寶工EDM" style="padding-top:5px;"><small class="styleGraylight">(內部發佈)</small><br />&nbsp;<%#Eval("StopDate").ToString().ToDateString("yyyy-MM-dd")%></div>
                        </td>
                        <td align="center">
                            <a href="http://ef.prokits.com.tw/employee/pdf/sipdb1_pdf.asp?item_id=<%# Server.UrlEncode(Eval("Model_No").ToString())%>"
                                class="BtnFour" target="_blank">舊版SIP</a><br />
                            <a href="http://ef.prokits.com.tw/employee/complaingrid2.asp?s_case_desc=<%# Server.UrlEncode(Eval("Model_No").ToString())%>"
                                class="BtnFour" target="_blank">品質異常</a><br />
                            <a href="http://ef.prokits.com.tw/employee/Login.asp?RedirUrl=<%# Server.UrlEncode("http://ef.prokits.com.tw/employee/complain2/complaim2Grid_v2.asp?s_item_no=" + Eval("Model_No").ToString())%>"
                                class="BtnFour" target="_blank">客訴記錄</a><br />
                            <a href="<%=Application["WebUrl"]%>myProdNews/Search.aspx?Keyword=<%#Server.UrlEncode(Eval("Model_No").ToString()) %>" class="BtnFour" target="_self">產品訊息</a>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <a class="btn btn-primary" href="Prod_Edit.aspx?Model_No=<%# Server.UrlEncode(Eval("Model_No").ToString())%>">修改</a>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_SOP" runat="server">
                                <div style="margin-top: 10px;">
                                    <a class="btn btn-default EditBox" href="Prod_SOP_Upload.aspx?Model_No=<%# Server.UrlEncode(Eval("Model_No").ToString())%>"
                                        title="SOP上傳">SOP上傳</a>
                                </div>
                            </asp:PlaceHolder>
                        </td>
                        <td valign="top">
                            <div>
                                <a href="<%#GetPDFUrl("outside","zh-TW",Eval("Model_No").ToString()) %>" class="btn btn-danger" target="_blank">繁中</a>

                                <a href="<%#GetPDFUrl("outside","en-US",Eval("Model_No").ToString()) %>" class="btn btn-danger" target="_blank">英文</a>

                                <a href="<%#GetPDFUrl("outside","zh-CN",Eval("Model_No").ToString()) %>" class="btn btn-danger" target="_blank">簡中</a>
                            </div>
                            <div style="margin-top: 10px;">
                                <a href="<%#GetPDFUrl("inside","zh-TW",Eval("Model_No").ToString()) %>" class="btn btn-danger" target="_blank">SIP</a>
                                <a href="<%#GetViewUrl("inside","zh-TW",Eval("Model_No").ToString()) %>" class="btn btn-info" target="_blank">SIP列印</a>
                            </div>
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
                        <asp:Literal ID="lt_Page_DataCntInfo" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div>
            <!-- Scroll Top 按扭 Start -->
            <a href="#" class="scrollup">Scroll</a>
            <!-- Scroll Top 按扭 End -->
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
