<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="EditRel.aspx.cs" Inherits="myProdCheck_EditRel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>myProdCheck/Edit.aspx?DataID=<%=Req_DataID %>"><i class="material-icons left">arrow_back</i>返回編輯頁</a></li>
                    </ul>
                    <span class="brand-logo center">採購單關聯設定</span>
                    <ul class="right">
                    </ul>
                </div>
            </div>
        </nav>
    </div>
    <!-- Top Nav End -->
    <!-- Body Start -->
    <div class="row">
        <div class="col s12">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                <div class="card-panel red darken-1 white-text">
                    <h4><i class="material-icons right">error_outline</i>糟糕了!!...發生了一點小問題</h4>
                    <p>若持續看到此訊息, 請回報 <strong class="flow-text">詳細操作狀況</strong>, 以便抓蟲<i class="material-icons">bug_report</i>。</p>
                    <p>
                        <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                    </p>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ph_Data" runat="server">
                <div class="card grey">
                    <div class="card-content white-text">
                        <h5>新增關聯, 主單號 (<asp:Literal ID="lt_Title" runat="server"></asp:Literal>)</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12">
                                <asp:ListView ID="lv_List" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_List_ItemCommand">
                                    <LayoutTemplate>
                                        <table class="bordered striped">
                                            <thead>
                                                <tr>
                                                    <th>單別/單號</th>
                                                    <th>採購日</th>
                                                    <th>廠商</th>
                                                    <th>品號</th>
                                                    <th>採購數量</th>
                                                    <th>&nbsp</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><strong><%#Eval("FirstID") %> - <%#Eval("SecondID") %></strong></td>
                                            <td><%#Eval("BuyDate") %></td>
                                            <td><%#Eval("CustName") %></td>
                                            <td><%#Eval("ModelNo") %></td>
                                            <td><%#fn_stringFormat.C_format(Eval("BuyQty").ToString()) %></td>
                                            <td>
                                                <asp:LinkButton ID="lbtn_Select" runat="server" CssClass="btn-flat waves-effect waves-red"><i class="material-icons">add</i></asp:LinkButton>
                                                
                                                <asp:HiddenField ID="hf_FirstID" runat="server" Value='<%#Eval("FirstID") %>' />
                                                <asp:HiddenField ID="hf_SecondID" runat="server" Value='<%#Eval("SecondID") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="section">
                                            <div class="card-panel grey darken-1">
                                                <i class="material-icons flow-text white-text">error_outline</i>
                                                <span class="flow-text white-text">查無其他可設定關聯的採購單</span>
                                            </div>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>

    <!-- Body End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

