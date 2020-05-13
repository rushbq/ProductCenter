<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Step3.aspx.cs" Inherits="myProdCheck_Step3" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>myProdCheck/Step2.aspx?corp=<%=Req_Corp %>"><i class="material-icons left">arrow_back</i>回上一步</a></li>
                    </ul>
                    <span class="brand-logo center">建立查檢表</span>
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
                        <h5>Step3 - 選擇品號</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12 grey lighten-4">
                                <i class="material-icons">flag</i>&nbsp;
                                目前的資料庫別為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_CorpName" runat="server"></asp:Literal></b></span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <asp:ListView ID="lv_List" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_List_ItemCommand" OnItemDataBound="lv_List_ItemDataBound">
                                    <LayoutTemplate>
                                        <table class="bordered striped">
                                            <thead>
                                                <tr>
                                                    <th>單別/單號</th>
                                                    <th>採購日</th>
                                                    <th>廠商</th>
                                                    <th style="width: 40px;"></th>
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
                                            <td>
                                                <asp:PlaceHolder ID="ph_showAdded" runat="server">
                                                    <i class="material-icons red-text text-darken-1" title="此品號已建立過查檢表">playlist_add_check</i>
                                                </asp:PlaceHolder>
                                            </td>
                                            <td><%#Eval("ModelNo") %></td>
                                            <td><%#fn_stringFormat.C_format(Eval("BuyQty").ToString()) %></td>
                                            <td>
                                                <asp:LinkButton ID="lbtn_Select" runat="server" CssClass="btn waves-effect waves-light orange darken-1" OnClientClick="return confirm('是否確定產生查檢表?')">SELECT</asp:LinkButton>


                                                <asp:HiddenField ID="hf_FirstID" runat="server" Value='<%#Eval("FirstID") %>' />
                                                <asp:HiddenField ID="hf_SecondID" runat="server" Value='<%#Eval("SecondID") %>' />
                                                <asp:HiddenField ID="hf_ModelNo" runat="server" Value='<%#Eval("ModelNo") %>' />
                                                <asp:HiddenField ID="hf_Vendor" runat="server" Value='<%#Eval("CustID") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="section">
                                            <div class="card-panel grey darken-1">
                                                <i class="material-icons flow-text white-text">error_outline</i>
                                                <span class="flow-text white-text">找不到資料, 請確認篩選條件是否正確.</span>
                                            </div>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                        <div class="section row">
                            <div class="col s12">
                                <label>注意事項</label>
                                <div>
                                    <ul class="collection">
                                        <li class="collection-item"><i class="material-icons left">info</i>若未顯示採購單列表，請確認上一步的條件是否正確</li>
                                        <li class="collection-item"><i class="material-icons left">info</i>按下「SELECT」後，即可產生新的查檢表</li>
                                        <li class="collection-item"><i class="material-icons left">info</i>若出現&nbsp;<i class="material-icons red-text text-darken-1">playlist_add_check</i>&nbsp;, 代表此品號已新增過查檢表</li>
                                    </ul>
                                </div>
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

