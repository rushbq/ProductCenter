<%@ Page Title="認證資料清單" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="Cert_Search.aspx.cs" Inherits="Cert_Search" %>


<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">產品中心</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">認證資料庫</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section red-text text-darken-2">認證資料清單
                    </h5>
                </div>
            </div>
            <div class="right menu">
                <div class="ui right pointing red basic label">點此新增資料</div>
                <a class="item" href="Cert_Edit.aspx">
                    <i class="plus icon"></i>
                    <span class="mobile hidden">新增資料</span>
                </a>
                <asp:LinkButton ID="lbtn_Excel" runat="server" CssClass="item" OnClick="lbtn_Excel_Click"><i class="file excel icon"></i><span class="mobile hidden">匯出</span></asp:LinkButton>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <!-- Search Start -->
        <div class="ui orange attached segment">
            <div class="ui small form">
                <div class="fields">
                    <div class="five wide field">
                        <label>品號</label>
                        <div class="ui fluid search ac-ModelNo">
                            <div class="ui right labeled input">
                                <asp:TextBox ID="tb_Model_No" runat="server" CssClass="prompt" MaxLength="40"></asp:TextBox>
                                <asp:Panel ID="lb_ModelNo" runat="server" CssClass="ui label">品號/品名關鍵字</asp:Panel>
                            </div>
                        </div>
                    </div>

                    <div class="four wide field">
                        <label>銷售類別</label>
                        <asp:DropDownList ID="ddl_Class_ID" runat="server" CssClass="fluid">
                        </asp:DropDownList>
                    </div>
                    <div class="four wide field">
                        <label>目錄</label>
                        <div class="ui labeled input">
                            <div class="ui label">
                                Vol.
                            </div>
                            <asp:TextBox ID="tb_Vol" runat="server" autocomplete="off" MaxLength="6"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="fields">
                    <div class="five wide field">
                        <label>證書關鍵字</label>
                        <asp:TextBox ID="tb_Keyword" runat="server" autocomplete="off" placeholder="查詢:認證指令, 證書編號" MaxLength="40"></asp:TextBox>
                    </div>
                    <div class="four wide field">
                        <label>自有認證</label>
                        <asp:DropDownList ID="ddl_SelfCert" runat="server" CssClass="fluid">
                            <asp:ListItem Value="">-- 所有資料 --</asp:ListItem>
                            <asp:ListItem Value="Y">是</asp:ListItem>
                            <asp:ListItem Value="N">否</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="seven wide inline field">
                        <label>認證選項:</label>
                        <asp:CheckBox ID="cb_IsExpired" runat="server" Text="已過期認證" />&nbsp;
                        <asp:CheckBox ID="cb_IsCE" runat="server" Text="自我宣告" />&nbsp;
                        <asp:CheckBox ID="cb_IsCheck" runat="server" Text="自我檢測" />
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
                            <a href="Cert_Edit.aspx" class="ui basic green button">新增資料</a>
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- Empty Content End -->

        <!-- List Content Start -->
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <div class="ui green attached segment">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                    <LayoutTemplate>
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="center aligned">品號</th>
                                    <th class="center aligned">類別</th>
                                    <th class="center aligned">廠商料號</th>
                                    <th class="center aligned">主要出貨地</th>
                                    <th class="center aligned">廠商</th>
                                    <th class="center aligned">自有認證</th>
                                    <th class="center aligned">目錄</th>
                                    <th class="center aligned">頁次</th>
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
                                <a href="Cert_View.aspx?id=<%# Eval("Cert_ID")%>">
                                    <b class="red-text text-darken-2">
                                        <%#Eval("Model_No")%>
                                    </b>
                                </a>
                            </td>
                            <td class="center aligned green-text text-darken-2">
                                <strong><%#Eval("Class_Name")%></strong>
                            </td>
                            <td class="center aligned">
                                <%#Eval("Supplier_ItemNo")%>
                            </td>
                            <td class="center aligned">
                                <%#Eval("Ship_From")%>
                            </td>
                            <td class="center aligned">
                                <%#Eval("Supplier")%>
                            </td>
                            <td class="center aligned">
                                <%# fn_Desc.Desc_YesNo_zhTW(Eval("Self_Cert").ToString())%>
                            </td>
                            <td class="center aligned">
                                <%#Eval("Vol")%>
                            </td>
                            <td class="center aligned">
                                <%#Eval("Page")%>
                            </td>
                            <td class="left aligned collapsing">
                                <asp:PlaceHolder ID="ph_Edit" runat="server">
                                    <a class="ui small teal basic icon button" href="Cert_Edit.aspx?id=<%# Eval("Cert_ID")%>" title="編輯"><i class="pencil icon"></i></a>
                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="ui small orange basic icon button" OnClientClick="return confirm('是否確定刪除!?')"><i class="trash alternate icon"></i></asp:LinkButton>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="ph_View" runat="server">
                                    <a class="ui small grey basic icon button" href="Cert_View.aspx?id=<%# Eval("Cert_ID")%>" title="看明細">
                                        <i class="file alternate icon"></i>
                                    </a>
                                </asp:PlaceHolder>
                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Cert_ID") %>' />
                                <asp:HiddenField ID="hf_ModelNo" runat="server" Value='<%#Eval("Model_No") %>' />
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
            $('select').dropdown();
        });
    </script>
    <%-- Search UI Start --%>
    <script>
        /* 品號 (使用category) */
        $('.ac-ModelNo').search({
            type: 'category',
            minCharacters: 1,
            searchFields: [
                'title',
                'description'
            ]
            , onSelect: function (result, response) {
                $("#MainContent_lb_ModelNo").text(result.description);

            }
            , apiSettings: {
                url: '<%=fn_Param.WebUrl%>Ajax_Data/GetData_Prod_v1.ashx?q={query}',
                onResponse: function (ajaxResp) {
                    //宣告空陣列
                    var response = {
                        results: {}
                    }
                    ;
                    // translate API response to work with search
                    /*
                      取得遠端資料後處理
                      .results = 物件名稱
                      item.Category = 要群組化的欄位
                      maxResults = 查詢回傳筆數

                    */
                    $.each(ajaxResp.results, function (index, item) {
                        var
                          categoryContent = item.Category || 'Unknown',
                          maxResults = 20
                        ;
                        if (index >= maxResults) {
                            return false;
                        }
                        // create new categoryContent category
                        if (response.results[categoryContent] === undefined) {
                            response.results[categoryContent] = {
                                name: categoryContent,
                                results: []
                            };
                        }

                        //重組回傳結果(指定顯示欄位)
                        response.results[categoryContent].results.push({
                            title: item.ID,
                            description: item.Label
                        });
                    });
                    return response;
                }
            }

        });
    </script>
    <%-- Search UI End --%>
</asp:Content>

