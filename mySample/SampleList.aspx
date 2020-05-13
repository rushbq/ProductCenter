<%@ Page Title="產品檢驗登記" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="SampleList.aspx.cs" Inherits="mySample_Search" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">品管研發</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">產品檢驗登記</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section red-text text-darken-2">登記清單
                    </h5>
                </div>
            </div>
            <div class="right menu">
                <a class="item" href="<%=fn_Param.WebUrl%>mySample/SampleEdit.aspx">
                    <i class="plus icon"></i>
                    <span class="mobile hidden">新增</span>
                </a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <!-- Search Start -->
        <div class="ui orange attached segment">
            <div class="ui small form">
                <div class="four fields">
                    <div class="field">
                        <label>公司別</label>
                        <asp:DropDownList ID="filter_Company" runat="server" CssClass="semui fluid">
                            <asp:ListItem Value="">選擇公司別</asp:ListItem>
                            <asp:ListItem Value="TWS">台灣</asp:ListItem>
                            <asp:ListItem Value="SHS">上海</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="field">
                        <label>來源</label>
                        <asp:DropDownList ID="filter_Source" runat="server" CssClass="semui fluid">
                        </asp:DropDownList>
                    </div>
                    <div class="field">
                        <label>檢驗類別</label>
                        <asp:DropDownList ID="filter_Check" runat="server" CssClass="semui fluid">
                        </asp:DropDownList>
                    </div>
                    <div class="field">
                        <label>狀態</label>
                        <asp:DropDownList ID="filter_Status" runat="server" CssClass="semui fluid">
                        </asp:DropDownList>
                    </div>

                </div>
                <div class="fields">
                    <div class="three wide field">
                        <label>日期區間</label>
                        <asp:DropDownList ID="filter_DateType" runat="server" CssClass="semui fluid">
                            <asp:ListItem Value="1">來樣日</asp:ListItem>
                            <asp:ListItem Value="2">預計完成日</asp:ListItem>
                            <asp:ListItem Value="3">實際完成日</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="five wide field">
                        <div class="two fields">
                            <div class="field">
                                <label>&nbsp;</label>
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_sDate" runat="server" placeholder="開始日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate outline icon"></i>
                                </div>
                            </div>
                            <div class="field">
                                <label>&nbsp;</label>
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_eDate" runat="server" placeholder="結束日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate icon"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="three wide field">
                        <label>負責人</label>
                        <asp:DropDownListGP ID="filter_Who" runat="server">
                        </asp:DropDownListGP>
                    </div>
                    <div class="five wide field">
                        <label>關鍵字查詢</label>
                        <asp:TextBox ID="filter_Keyword" runat="server" autocomplete="off" placeholder="查詢：取樣編號, 寶工品號, 廠商品號, 廠商名稱, 廠商產品描述" MaxLength="20"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="ui two column grid">
                <div class="column">
                    <a href="<%=thisPage %>" class="ui small button"><i class="refresh icon"></i>重置條件</a>
                </div>
                <div class="column right aligned">
                    <button type="button" id="doSearch" class="ui blue small button"><i class="search icon"></i>查詢</button>
                    <asp:Button ID="btn_Search" runat="server" Text="Button" OnClick="btn_Search_Click" Style="display: none" />
                </div>
            </div>

        </div>
        <!-- Search End -->

        <!-- Empty Content Start -->
        <asp:PlaceHolder ID="ph_EmptyData" runat="server" Visible="false">
            <div class="ui placeholder segment">
                <div class="ui two column stackable center aligned grid">
                    <div class="ui vertical divider">Or</div>
                    <div class="middle aligned row">
                        <div class="column">
                            <div class="ui icon header">
                                <i class="search icon"></i>
                                目前條件查無資料，請重新查詢。
                            </div>
                        </div>
                        <div class="column">
                            <div class="ui icon header">
                                <i class="plus icon"></i>
                                建立新資料
                            </div>
                            <a href="<%=fn_Param.WebUrl%>mySample/SampleEdit.aspx" class="ui basic green button">新增</a>
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- Empty Content End -->

        <!-- List Content Start -->
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <div class="ui green attached segment">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                    <LayoutTemplate>
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="center aligned">公司別</th>
                                    <th class="center aligned">狀態</th>
                                    <th class="center aligned">樣品編號</th>
                                    <th>廠商名稱</th>
                                    <th>廠商品號</th>
                                    <th>寶工品號</th>
                                    <th>產品描述</th>
                                    <th class="center aligned">負責人</th>
                                    <th class="center aligned">來樣日</th>
                                    <th class="center aligned">預計完成</th>
                                    <th class="center aligned">實際完成</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="center aligned collapsing">
                                <div class="ui grey basic fluid label"><%#Eval("Company_Name") %></div>
                            </td>
                            <td class="center aligned collapsing">
                                <div class="ui orange basic fluid label"><%#Eval("Status_Name") %></div>
                            </td>
                            <td class="center aligned collapsing">
                                <b class="blue-text text-darken-2"><%#Eval("SerialNo") %></b>
                            </td>
                            <td>
                                <b class="green-text text-darken-2"><%#Eval("Cust_Name") %></b>
                            </td>
                            <td>
                                <%#Eval("Cust_ModelNo") %>
                            </td>
                            <td>
                                <b><%#Eval("Model_No") %></b>
                            </td>
                            <td>
                                <%#Eval("Description1") %>
                            </td>
                            <td class="center aligned collapsing">
                                <span class="grey-text text-darken-2"><%#Eval("Assign_Name") %></span>
                            </td>
                            <td class="center aligned collapsing">
                                <span class="grey-text text-darken-3">
                                    <%#Eval("Date_Come").ToString().ToDateString("yyyy/MM/dd") %>
                                </span>
                            </td>
                            <td class="center aligned collapsing">
                                <span class="grey-text text-darken-3">
                                    <%#Eval("Date_Est").ToString().ToDateString("yyyy/MM/dd") %>
                                </span>
                            </td>
                            <td class="center aligned collapsing">
                                <span class="grey-text text-darken-3">
                                    <%#Eval("Date_Actual").ToString().ToDateString("yyyy/MM/dd") %>
                                </span>
                            </td>
                            <td class="left aligned collapsing">
                                <a class="ui small grey basic icon button" href="<%=FuncPath() %>SampleView.aspx?DataID=<%#Server.UrlEncode(Eval("SP_ID").ToString()) %>" title="查看">
                                    <i class="file alternate icon"></i>
                                </a>
                                <a class="ui small teal basic icon button" href="<%=FuncPath() %>SampleEdit.aspx?DataID=<%#Server.UrlEncode(Eval("SP_ID").ToString()) %>" title="編輯">
                                    <i class="pencil icon"></i>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
            </div>
            <!-- List Pagination Start -->
            <div class="ui mini bottom attached segment grey-bg lighten-4">
                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
            </div>
            <!-- List Pagination End -->
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
            //[搜尋][查詢鈕] - 觸發查詢
            $("#doSearch").click(function () {
                //觸發查詢按鈕
                $("#MainContent_btn_Search").trigger("click");
            });

            //init dropdown list
            $('select.semui').dropdown();
        });
    </script>
    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Param.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Param.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Param.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').calendar(calendarOpts_Range);
        });
    </script>
    <%-- 日期選擇器 End --%>
</asp:Content>

