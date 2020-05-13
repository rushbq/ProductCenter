<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProdPic_Search.aspx.cs" Inherits="ProdPic_Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- catcomplete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" type="text/css" />
    <script src="../js/catcomplete/catcomplete.js" type="text/javascript"></script>
    <%-- catcomplete End --%>
    <%-- smallipop Start --%>
    <link href="../js/smallipop/css/contrib/animate.min.css" rel="stylesheet" type="text/css" />
    <link href="../js/smallipop/css/jquery.smallipop.min.css" rel="stylesheet"
        type="text/css" />
    <script src="../js/smallipop/jquery.smallipop.min.js" type="text/javascript"></script>
    <script src="../js/smallipop/modernizr.min.js" type="text/javascript"></script>
    <%-- smallipop End --%>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            /* Autocomplete - 群組分類(品號) */
            $("#tb_Model_No").catcomplete({
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
                }
            });

            /* smallipop, tour效果 */
            $('#runTour').click(function () {
                $('.tipTour').smallipop('tour');
            });
            $('.tipTour').smallipop({
                theme: 'black',
                cssAnimations: {
                    enabled: true,
                    show: 'animated flipInX',
                    hide: 'animated flipOutX'
                }
            });

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_Model_No").val("");
                $("select#ddl_Class_ID")[0].selectedIndex = 0;
            });

        });

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">系統首頁</a>&gt;<a>圖片資料庫</a>&gt;<span>資料列表</span>
        </div>
        <div class="h2Head">
            <h2>資料列表</h2>
        </div>
        <!-- Basic Sift -->
        <div class="Sift">
            <!--條件篩選區-->
            <ul>
                <li>品號：<asp:TextBox ID="tb_Model_No" runat="server" MaxLength="40" CssClass="tipTour"
                    ToolTip="品號關鍵字查詢<br>可輸入關鍵字(ex:1PK)，就會出現相關的品號選單" data-smallipop-tour-index="1"></asp:TextBox>
                </li>
                <li>銷售類別：
                <asp:DropDownList ID="ddl_Class_ID" runat="server">
                </asp:DropDownList>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                    <input type="button" id="runTour" value="?" title="導覽" class="btnBlock colorDark" />
                </li>
            </ul>
        </div>
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead" style="white-space: nowrap;">
                        <td width="140px">品號<span class="tipTour" data-smallipop-tour-index="2">&nbsp;<span class="smallipopHint">點選「品號」可進入編輯頁</span></span>
                        </td>
                        <td width="80px">主要出貨地
                        </td>
                        <td width="100px">條碼
                        </td>
                        <td>產品圖<span class="tipTour" data-smallipop-tour-index="3">&nbsp;<span class="smallipopHint">若圖片已上傳，<br />
                            點下連結可進入檢視頁</span></span>
                        </td>
                        <td>產品輔圖
                        </td>
                        <td>DM
                        </td>
                        <td>彩盒
                        </td>
                        <td>彩標
                        </td>
                        <td>貼紙
                        </td>
                        <td>卡片
                        </td>
                        <td>說明書
                        </td>
                        <td>Pounch袋
                        </td>
                        <td>本體設計
                        </td>
                        <td>袖套
                        </td>
                        <td>吊卡
                        </td>
                        <td>尺寸示意圖
                        </td>
                        <td>其他
                        </td>
                        <td>品保
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server" style="white-space: nowrap;">
                    <td align="left">
                        <div class="JQ-ui-state-default">
                            <asp:Literal ID="lt_Edit" runat="server"></asp:Literal>
                        </div>
                        <div class="L2Info styleGreen" style="padding-top: 3px;">
                            <%#Eval("Class_Name")%>
                        </div>
                    </td>
                    <td align="center">
                        <%#Eval("Ship_From")%>
                    </td>
                    <td align="center">
                        <%#Eval("BarCode")%>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View1" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View2" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View3" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View4" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View5" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View6" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View7" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <a href="../Product/Prod_Files_View.aspx?Model_No=<%#Eval("Model_No") %>">點我前往</a>
                        <asp:Literal ID="lt_View8" runat="server" Visible="false"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View9" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View10" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View11" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View12" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View13" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View99" runat="server"></asp:Literal>
                    </td>
                    <td align="center">
                        <asp:Literal ID="lt_View14" runat="server"></asp:Literal>
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
