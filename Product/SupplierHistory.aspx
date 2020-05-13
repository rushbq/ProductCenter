<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="SupplierHistory.aspx.cs" Inherits="Product_SupplierHistory" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>Product/Prod_Edit.aspx?Model_No=<%=Req_DataID %>"><i class="material-icons left">arrow_back</i>Back</a></li>
                    </ul>
                </div>
                <div class="nav-content">
                    <span class="nav-title right flow-text"><strong><%=Req_DataID %></strong></span>
                </div>
            </div>
        </nav>
    </div>
    <!-- Top Nav End -->
    <!-- Body Start -->
    <div class="row">
        <div class="col s12 m12 l12">
            <div class="card grey">
                <div class="card-content white-text">
                    <h5>庫存資訊</h5>
                </div>
                <div class="card-content grey lighten-5">
                    <table class="responsive-table centered striped">
                        <thead>
                            <tr>
                                <th id="invType">倉別</th>
                                <th id="invNum">庫存</th>
                                <th id="invSafe">安全存量</th>
                                <th id="invPreOut">預計銷</th>
                                <th id="invPreIn">預計進</th>
                                <th id="stockNum">可用量</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th>台灣(01倉)</th>
                                <td>
                                    <asp:Literal ID="lt_INV_Num_TW" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_INV_Safe_TW" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_INV_PreOut_TW" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_INV_PreIn_TW" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Stock_TW" runat="server">-</asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <th>上海(12倉)</th>
                                <td>
                                    <asp:Literal ID="lt_INV_Num_SH" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_INV_Safe_SH" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_INV_PreOut_SH" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_INV_PreIn_SH" runat="server">-</asp:Literal>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Stock_SH" runat="server">-</asp:Literal>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

        </div>
    </div>
    <div class="row">
        <div class="col s12 m12 l12">
            <div class="card grey">
                <div class="card-content white-text">
                    <h5>供應商採購記錄</h5>
                </div>
                <div class="card-content grey lighten-5">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table class="bordered centered highlight">
                                <thead>
                                    <tr>
                                        <th>年份</th>
                                        <th>編號</th>
                                        <th>公司別</th>
                                        <th>簡稱</th>
                                        <th>幣別</th>
                                        <th>最近採購單價</th>
                                        <th>最新生效日</th>
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
                                <td><%#Eval("SetYear") %></td>
                                <td><%#Eval("SupID") %></td>
                                <td><%#Eval("Company") %></td>
                                <td><%#Eval("SupName") %></td>
                                <td><%#Eval("Currency") %></td>
                                <td><%#Eval("LastPrice") %></td>
                                <td><%#Eval("ValidDate") %></td>
                                <td>
                                    <a href="http://ef.prokits.com.tw/employee/supplierv/supplier_base.asp?supcode=<%#Eval("SupID") %>&fg=supsch" target="_blank" class="waves-effect waves-green btn-flat" title="供應商資料"><i class="material-icons">open_in_new</i></a>
                                    <a href="<%=Application["WebUrl"] %>Product/SupplierTradeInfo.aspx?modelNo=<%#Req_DataID %>&supID=<%#Eval("SupID") %>&company=<%#Eval("Company") %>" class="waves-effect waves-green btn-flat" title="交易記錄"><i class="material-icons">more_vert</i></a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="section">
                                <div class="card-panel grey darken-1">
                                    <i class="material-icons flow-text white-text">error_outline</i>
                                    <span class="flow-text white-text">找不到資料!<i class="material-icons right">arrow_upward</i></span>
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>

                </div>
            </div>

        </div>
    </div>
    <!-- Body End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>
