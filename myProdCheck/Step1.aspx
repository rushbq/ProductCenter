<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Step1.aspx.cs" Inherits="myProdCheck_Step1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>myProdCheck/Search.aspx"><i class="material-icons left">arrow_back</i>返回列表</a></li>
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
                        <h5>Step1 - 選擇公司別</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="section row">
                            <div class="col s6">
                                <a href="<%=Application["WebUrl"] %>myProdCheck/Step2.aspx?corp=1" class="btn waves-effect waves-light green btn-large flow-text" style="display: block"><b>台灣</b><i class="material-icons right">keyboard_arrow_right</i></a>
                            </div>
                            <div class="col s6">
                                <a href="<%=Application["WebUrl"] %>myProdCheck/Step2.aspx?corp=3" class="btn waves-effect waves-light blue btn-large flow-text" style="display: block"><b>上海</b><i class="material-icons right">keyboard_arrow_right</i></a>
                            </div>
                        </div>
                        <div class="section row">
                            <div class="col s12">
                                <label>注意事項</label>
                                <div>
                                    <ul class="collection">
                                        <li class="collection-item"><i class="material-icons left">info</i>請正確的選擇ERP資料庫別</li>
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

