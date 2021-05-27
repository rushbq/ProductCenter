<%@ Page Title="貨號屬性" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ProdProp_Search.aspx.cs" Inherits="ProdProp_Search" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">產品資料庫</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section red-text text-darken-2">貨號屬性
                    </h5>
                </div>
            </div>

        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <!-- Advance Search Start -->
        <div class="ui orange attached segment">
            <div class="ui small form">
                <div class="fields">
                    <div class="four wide field">
                        <label>品號關鍵字(Like %)</label>
                        <asp:TextBox ID="filter_ModelNo" runat="server" MaxLength="20" autocomplete="off" placeholder="品號關鍵字"></asp:TextBox>
                    </div>
                    <div class="four wide field">
                        <label>貨號關鍵字(Like %)</label>
                        <asp:TextBox ID="filter_ItemNo" runat="server" MaxLength="20" autocomplete="off" placeholder="貨號關鍵字"></asp:TextBox>
                    </div>
                    <div class="four wide field" style="text-align: right;">
                        <label>&nbsp;</label>
                        <a href="<%=thisPage %>" class="ui small button"><i class="refresh icon"></i>重置</a>
                        <button type="button" id="doSearch" class="ui blue small button"><i class="search icon"></i>查詢</button>
                        <asp:Button ID="btn_Search" runat="server" Text="search" OnClick="btn_Search_Click" Style="display: none" />
                    </div>
                </div>
            </div>
        </div>
        <!-- Advance Search End -->

        <!-- Empty Content Start -->
        <asp:PlaceHolder ID="ph_EmptyData" runat="server">
            <div class="ui placeholder segment">
                <div class="ui icon header">
                    <i class="search icon"></i>
                    目前條件查無資料，請重新查詢。
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- Empty Content End -->

        <!-- List Content Start -->
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3">&nbsp;</th>
                                    <th class="grey-bg lighten-3 center aligned">貨號</th>
                                    <th class="grey-bg lighten-3 center aligned">主要出貨地</th>
                                    <th class="grey-bg lighten-3 center aligned">上市日</th>
                                    <th class="grey-bg lighten-3 center aligned">停售日</th>
                                    <th class="grey-bg lighten-3 center aligned">新品類別</th>
                                    <th class="grey-bg lighten-3 center aligned">銷售類別</th>
                                    <th class="grey-bg lighten-3 center aligned">採購/生管類別</th>
                                    <th class="grey-bg lighten-3 center aligned">倉管類別</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </table>
                    </div>

                    <!-- List Pagination Start -->
                    <div class="ui mini bottom attached segment grey-bg lighten-4">
                        <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                    </div>
                    <!-- List Pagination End -->
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="left aligned collapsing">
                            <!-- edit -->
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <a class="ui small teal basic icon button" href="<%=FuncPath() %>ProdProp_Edit.aspx?id=<%#Eval("Data_ID") %>&itemNo=<%#Server.UrlEncode(Eval("Item_No").ToString()) %>" title="編輯">
                                    <i class="pencil icon"></i>
                                </a>
                            </asp:PlaceHolder>
                            <a class="ui small grey basic icon button btn-OpenDetail" data-id="<%#Eval("Item_No") %>" data-val="<%#Eval("Item_No") %>" title="關聯品號">
                                <i class="tags icon"></i>
                            </a>
                        </td>
                        <td class="center aligned orange-text text-darken-4">
                            <h5><%#Eval("Item_No") %></h5>
                        </td>
                        <td class="center aligned">
                            <%#Eval("ShipFrom") %>
                        </td>
                        <td class="center aligned">
                            <!-- 上市日 -->
                            <%#Eval("OnlineDate").ToString().ToDateString_ERP("/") %>
                        </td>
                        <td class="center aligned">
                            <!-- 停售日 -->
                            <%#Eval("StopDate").ToString().ToDateString_ERP("/") %>
                        </td>
                        <td class="center aligned">
                            <!-- 新品類別 -->
                            <%#Eval("NewProdClass") %>
                        </td>
                        <td class="center aligned">
                            <!-- 銷售類別 -->
                            <%#Eval("SaleClassName") %>
                        </td>
                        <td class="center aligned">
                            <!-- 採購/生管類別 -->
                            <%#Eval("PurClassName") %>
                        </td>
                        <td class="center aligned">
                            <!-- 倉管類別 -->
                            <%#Eval("WareHouseClassName") %>
                        </td>
                    </tr>
                    <%-- 帶出處理人員名單 --%>
                    <tr id="tar-Detail-<%#Eval("Item_No") %>" class="grey-bg lighten-5" style="display: none;">
                        <td class="right aligned"></td>
                        <td colspan="7">
                            <div class="Detail-<%#Eval("Item_No") %>">
                                <div class="ui icon message">
                                    <i class="notched circle loading icon"></i>
                                    <div class="content">
                                        <div class="header">
                                            資料擷取,請稍候....
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td class="center aligned">
                            <a class="ui small grey button btn-CloseDetail" data-id="<%#Eval("Item_No") %>">CLOSE</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </asp:PlaceHolder>
        <!-- List Content End -->

    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init dropdown list
            $('select').dropdown();


            $(".myContentBody").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#doSearch").trigger("click");
                    //避免觸發submit
                    e.preventDefault();
                }
            });

            //[搜尋][查詢鈕] - 觸發查詢
            $("#doSearch").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });


        });
    </script>
    <%-- 處理人員名單 Start --%>
    <script>
        $(function () {
            //按鈕 - 開明細
            $(".btn-OpenDetail").click(function () {
                var id = $(this).attr("data-id");
                var val = $(this).attr("data-val");

                boxDetail(id, val, true);
            });

            //按鈕 - 關明細
            $('.btn-CloseDetail').click(function () {
                var id = $(this).attr("data-id");
                var val = $(this).attr("data-val");

                boxDetail(id, val, false);
            });

            //FUNCTION - 明細開關
            function boxDetail(id, val, isOpen) {
                var myBox = $("#tar-Detail-" + id);

                if (isOpen) {
                    myBox.show();

                    loadDetail(id, val);

                } else {
                    myBox.hide();
                }
            }

            //Ajax - 讀取明細
            function loadDetail(id, val) {
                //取得目標容器
                var container = $(".Detail-" + id);

                //取得輸入值
                var _dataVal = val;

                //填入Ajax Html
                var url = "<%=fn_Param.WebUrl%>myProd/GetHtml_ModelList.ashx?id=" + _dataVal;
                container.load(url);
            }

        });
    </script>
    <%-- 處理人員名單 End --%>
</asp:Content>

