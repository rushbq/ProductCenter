<%@ Page Title="外驗查核表|核准" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Approved.aspx.cs" Inherits="myProdCheck_Approved" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Page_SearchUrl%>"><i class="material-icons left">arrow_back</i><span class="hide-on-small-only">返回列表</span></a></li>
                    </ul>
                    <span class="brand-logo center">查檢表核准</span>
                    <ul class="right">
                        <li>
                            <asp:PlaceHolder ID="ph_btn" runat="server">
                                <asp:LinkButton ID="lbtn_No" runat="server" CssClass="btn-large waves-effect waves-light grey" OnClick="lbtn_No_Click">不同意<i class="material-icons left">clear</i></asp:LinkButton>
                                <asp:LinkButton ID="lbtn_Yes" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Yes_Click">同意<i class="material-icons left">done</i></asp:LinkButton>
                            </asp:PlaceHolder>
                        </li>
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
            <asp:PlaceHolder ID="ph_OK" runat="server" Visible="false">
                <div class="card-panel green darken-1 white-text">
                    <h4><i class="material-icons right">done</i>已核准成功<small>
                        <asp:Literal ID="lt_MailTime" runat="server"></asp:Literal>
                    </small></h4>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_Data" runat="server">
                <!-- ERP採購單資料 Start -->
                <div id="erp" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>ERP採購單資料</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s6">
                                <label>品號</label>
                                <p class="flow-text red-text text-darken-1">
                                    <b>
                                        <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                                    </b>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>廠商</label>
                                <p class="flow-text orange-text text-darken-1">
                                    <strong>
                                        <asp:Literal ID="lt_Vendor" runat="server"></asp:Literal></strong>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s6">
                                <label>單別/單號</label>
                                <p class="green-text text-darken-2">
                                    <asp:Literal ID="lt_ErpID" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>採購日期</label>
                                <p>
                                    <asp:Literal ID="lt_BuyDate" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s6">
                                <label>公司別</label>
                                <p>
                                    <asp:Literal ID="lt_Corp" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>採購數量</label>
                                <p>
                                    <asp:Literal ID="lt_BuyCnt" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- ERP採購單資料 End -->


                <!-- 查檢表欄位 Start -->
                <div id="base" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>查檢表欄位</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s6">
                                <label>狀態</label>
                                <p>
                                    <asp:Literal ID="lt_Status" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>檢驗數量</label>
                                <p class="flow-text blue-text text-darken-1">
                                    <b>
                                        <asp:Literal ID="lt_CheckTotal" runat="server"></asp:Literal>
                                    </b>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s6">
                                <label>預計驗貨日</label>
                                <p>
                                    <asp:Literal ID="lt_Date_Est" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>實際驗貨日</label>
                                <p>
                                    <asp:Literal ID="lt_Date_Act" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <label>備註</label>
                                <p>
                                    <asp:Literal ID="lt_Remark" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 查檢表欄位 End -->

                <!-- 檢驗報表 Start -->
                <div id="report" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>檢驗報表</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-blue">
                                    <h6>查檢表</h6>
                                    <div>
                                        <asp:ListView ID="lv_Files_Check" runat="server" ItemPlaceholderID="ph_Items">
                                            <LayoutTemplate>
                                                <table class="bordered striped">
                                                    <tbody>
                                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                    </tbody>
                                                </table>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <a href="<%#Application["RefUrl"] %><%#UploadFolder %><%#Eval("AttachFile") %>" target="_blank"><%#Eval("AttachFile_Name") %></a>
                                                    </td>
                                                    <td style="width: 20%">
                                                        <%#Eval("Create_Time")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <div class="center-align grey-text text-lighten-1">
                                                    <i class="material-icons flow-text">info_outline</i>
                                                    <span class="flow-text">尚未上傳</span>
                                                </div>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </div>
                                </blockquote>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-green">
                                    <h6>其他附件</h6>
                                    <div>
                                        <asp:ListView ID="lv_Files_Other" runat="server" ItemPlaceholderID="ph_Items">
                                            <LayoutTemplate>
                                                <table class="bordered striped">
                                                    <tbody>
                                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                    </tbody>
                                                </table>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td>
                                                        <a href="<%#Application["RefUrl"] %><%#UploadFolder %><%#Eval("AttachFile") %>" target="_blank"><%#Eval("AttachFile_Name") %></a>
                                                    </td>
                                                    <td style="width: 20%">
                                                        <%#Eval("Create_Time")%>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <div class="center-align grey-text text-lighten-1">
                                                    <i class="material-icons flow-text">info_outline</i>
                                                    <span class="flow-text">尚未上傳</span>
                                                </div>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </div>
                                </blockquote>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 檢驗報表 End -->


            </asp:PlaceHolder>

        </div>
    </div>

    <!-- Body End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

