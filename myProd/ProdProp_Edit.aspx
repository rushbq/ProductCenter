<%@ Page Title="貨號屬性編輯" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ProdProp_Edit.aspx.cs" Inherits="ProdProp_Edit" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<%@ Import Namespace="Resources" %>
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
                    <h5 class="active section red-text text-darken-2">貨號屬性編輯
                    </h5>
                </div>
            </div>
            <div class="right menu">
                <a class="anchor" id="top"></a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui grid">
            <div class="row">
                <!-- Left Body Content Start -->
                <div id="myStickyBody" class="thirteen wide column">
                    <div class="ui attached segment grey-bg lighten-5">
                        <!-- Section-基本資料 Start -->
                        <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                            <div class="ui negative message">
                                <div class="header">
                                    Oops...
                                </div>
                                <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>
                        <div class="ui segments">
                            <div class="ui green segment">
                                <h5 class="ui header"><a class="anchor" id="baseData"></a>基本資料</h5>
                            </div>
                            <div class="ui form segment">
                                <div class="fields">
                                    <div class="six wide field">
                                        <div class="ui fluid grey card">
                                            <div class="content">
                                                <div class="right floated meta">貨號</div>
                                                <span class="header orange-text text-darken-4">
                                                    <asp:Literal ID="lt_ItemNo" runat="server"></asp:Literal></span>
                                            </div>
                                            <div class="image">
                                                <asp:Literal ID="lt_Photo" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="ten wide field">
                                        <div class="two fields">
                                            <div class="field">
                                                <label>主要品號</label>
                                                <asp:Label ID="lb_ModelNo" runat="server" CssClass="ui large label"></asp:Label>
                                            </div>
                                            <div class="field">
                                                <label>新品類別</label>
                                                <asp:DropDownList ID="ddl_NewProdClass" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="two fields">
                                            <div class="field">
                                                <label>主要出貨地</label>
                                                <asp:Label ID="lb_ShipFrom" runat="server" CssClass="ui large label"></asp:Label>
                                            </div>
                                            <div class="field">
                                                <label>銷售類別</label>
                                                <asp:DropDownList ID="ddl_SaleClass" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="two fields">
                                            <div class="field">
                                                <label>上市日</label>
                                                <div class="ui left icon input datepicker">
                                                    <asp:TextBox ID="tb_OnlineDate" runat="server" autocomplete="off"></asp:TextBox>
                                                    <i class="calendar alternate outline icon"></i>
                                                </div>
                                            </div>
                                            <div class="field">
                                                <label>採購/生管類別</label>
                                                <asp:DropDownList ID="ddl_PurClass" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="two fields">
                                            <div class="field">
                                                <label>停售日</label>
                                                <div class="ui left icon input datepicker">
                                                    <asp:TextBox ID="tb_StopDate" runat="server" autocomplete="off"></asp:TextBox>
                                                    <i class="calendar alternate outline icon"></i>
                                                </div>
                                            </div>
                                            <div class="field">
                                                <label>倉管類別</label>
                                                <asp:DropDownList ID="ddl_WareHouseClass" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="two fields">
                                            <div class="field">
                                                <label>CCC COOD</label>
                                                <asp:TextBox ID="tb_CCCode" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="field">
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <!-- Section-基本資料 End -->

                        <!-- Section-關聯品號 Start -->
                        <div class="ui segments">
                            <div class="ui blue segment">
                                <h5 class="ui header"><a class="anchor" id="relData"></a>關聯品號</h5>
                            </div>
                            <div class="ui segment">
                                <asp:ListView ID="lv_Models" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <div class="ui left labeled button" tabindex="0">
                                            <div class="ui basic label">
                                                <%#Eval("ModelNo") %>
                                            </div>
                                            <a href="<%#fn_Param.WebUrl %>Product/Prod_View.aspx?Model_No=<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>" target="_blank" title="看產品" class="ui icon button">
                                                <i class="fork icon"></i>
                                            </a>
                                        </div>
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                        <!-- Section-關聯品號 End -->

                        <!-- Section-維護資訊 Start -->
                        <div class="ui segments">
                            <div class="ui grey segment">
                                <h5 class="ui header"><a class="anchor" id="infoData"></a>維護資訊</h5>
                            </div>
                            <div class="ui segment">
                                <table class="ui celled small four column table">
                                    <thead>
                                        <tr>
                                            <th colspan="2" class="center aligned">建立</th>
                                            <th colspan="2" class="center aligned">最後更新</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr class="center aligned">
                                            <td>
                                                <asp:Literal ID="info_Creater" runat="server">資料建立中...</asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="info_CreateTime" runat="server">資料建立中...</asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="info_Updater" runat="server"></asp:Literal>
                                            </td>
                                            <td>
                                                <asp:Literal ID="info_UpdateTime" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <!-- Section-維護資訊 End -->
                    </div>

                </div>
                <!-- Left Body Content End -->

                <!-- Right Navi Menu Start -->
                <div class="three wide column">
                    <div class="ui sticky">
                        <div id="fastjump" class="ui secondary vertical pointing fluid text menu">
                            <div class="header item">快速跳轉<i class="dropdown icon"></i></div>
                            <a href="#baseData" class="item">基本資料</a>
                            <a href="#relData" class="item">關聯品號</a>
                            <a href="#top" class="item"><i class="angle double up icon"></i>到頂端</a>
                        </div>

                        <div class="ui vertical text menu">

                            <div class="header item">功能按鈕</div>
                            <div class="item">
                                <a href="<%:Page_SearchUrl %>" class="ui small button"><i class="undo icon"></i>返回列表</a>
                            </div>
                            <div class="item">
                                <button id="doSave" type="button" class="ui green small button"><i class="save icon"></i>資料存檔</button>
                                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_doSave_Click" Style="display: none;" />
                                <asp:HiddenField ID="hf_DataID" runat="server" />
                                <asp:HiddenField ID="hf_ItemNo" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Right Navi Menu End -->
            </div>
        </div>

    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //[觸發][Save按鈕]
            $("#doSave").click(function () {
                //lock button
                $(this).addClass('loading').addClass('disabled');
                //lock page
                $("#myStickyBody").addClass("loading");

                //觸發ServerControll Button
                $("#MainContent_btn_doSave").trigger("click");
            });

            //init dropdown list
            $('select').dropdown();

        });
    </script>

    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Param.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Param.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Param.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js"></script>
    <script>
        $(function () {
            //載入datepicker (date)
            $('.datepicker').calendar(calendarOpts_Range_unLimit);
        });
    </script>
    <%-- 日期選擇器 End --%>

    <%-- 快速選單 --%>
    <script src="<%=fn_Param.WebUrl %>javascript/sticky.js"></script>
</asp:Content>

