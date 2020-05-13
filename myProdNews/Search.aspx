<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster_v1.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="myProdNews_Search" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">產品訊息查詢</h5>
                    <ol class="breadcrumb">
                        <li><a>產品資料庫</a></li>
                        <li><a href="<%=fn_Param.WebUrl %>myProdNews/Search.aspx">產品訊息</a></li>
                        <li class="active">資料列表</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="row">
        <div class="col s12">
            <div class="card-panel">
                <div class="row">
                    <div class="col s4 m3">
                        <label>類別</label>
                        <asp:DropDownList ID="filter_Status" runat="server" CssClass="select-control">
                            <asp:ListItem Value="">全部資料</asp:ListItem>
                            <asp:ListItem Value="1">產品設計變更</asp:ListItem>
                            <asp:ListItem Value="2">產品淘汰</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="input-field col s4 m5">
                        <asp:TextBox ID="filter_Keyword" runat="server" placeholder="品號 / 主旨 / 流程序號" MaxLength="50" autocomplete="off"></asp:TextBox>
                        <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                    </div>
                    <div class="input-field col s4 m4 right-align">
                        <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                        <asp:PlaceHolder ID="ph_Edit" runat="server">
                            <a href="<%=fn_Param.WebUrl %>myProdNews/Edit.aspx" class="btn waves-effect waves-light red" title="新增"><i class="material-icons">add</i></a>
                        </asp:PlaceHolder>
                        <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                    </div>
                </div>
                <div>
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                        <LayoutTemplate>
                            <%--<asp:Literal ID="lt_TopPager" runat="server"></asp:Literal>--%>
                            <div>
                                <table id="myListTable" class="bordered highlight">
                                    <thead>
                                        <tr>
                                            <th class="center-align">ID</th>
                                            <th class="center-align">發佈日期</th>
                                            <th>主旨/類別</th>
                                            <th class="center-align" title="是否發信">發信<i class="tiny material-icons">email</i></th>
                                            <th class="center-align" title="是否結案">結案<i class="tiny material-icons">assignment_turned_in</i></th>
                                            <th class="center-align">&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </div>
                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="grey-text center-align">
                                    <small><%#Eval("NewsID") %></small>
                                </td>
                                <td class="center-align teal-text text-darken-2">
                                    <small>
                                        <%#Eval("Send_Time").ToString().ToDateString("yyyy/MM/dd<br/>HH:mm") %>
                                    </small>
                                </td>
                                <td>
                                    <strong>
                                        <asp:Literal ID="lt_Subject" runat="server"></asp:Literal></strong>
                                    <span class="new badge grey darken-1" data-badge-caption="<%#Eval("ClassName") %>"></span>
                                    <div title="非流程中的人員只能看代號">
                                        <small>
                                            <asp:Literal ID="lt_BPMSno" runat="server"></asp:Literal>
                                        </small>
                                    </div>
                                </td>
                                <td class="grey-text text-darken-2 center-align"><%#Get_Status(Eval("IsMail").ToString()) %></td>
                                <td class="grey-text text-darken-2 center-align"><%#Get_Status(Eval("IsClose").ToString()) %></td>
                                <td>
                                    <a class="waves-effect waves-light waves-teal btn white grey-text text-darken-2" href="<%:fn_Param.WebUrl %>myProdNews/View.aspx?DataID=<%#Eval("NewsID") %>" title="查看"><i class="material-icons">visibility</i></a>
                                    <asp:PlaceHolder ID="ph_Edit" runat="server">
                                        <a class="waves-effect waves-light btn green" href="<%:fn_Param.WebUrl %>myProdNews/Edit.aspx?DataID=<%#Eval("NewsID") %>" title="編輯"><i class="material-icons">edit</i></a>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="ph_Send" runat="server">
                                        <a class="waves-effect waves-light btn light-blue" target="_blank" href="<%:fn_Param.WebUrl %>myProdNews/Send.aspx?DataID=<%#Eval("NewsID") %>" title="發信"><i class="material-icons">email</i></a>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="ph_Close" runat="server">
                                        <asp:LinkButton ID="lbtn_Close" runat="server" ToolTip="結案" CssClass="waves-effect waves-light btn orange lighten-1" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('是否確定設為結案?')"><i class="material-icons">assignment_turned_in</i></asp:LinkButton>
                                    </asp:PlaceHolder>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("NewsID") %>' />
                                    <asp:HiddenField ID="hf_IsMail" runat="server" Value='<%#Eval("IsMail") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="section">
                                <div class="card-panel grey darken-1">
                                    <i class="material-icons flow-text white-text">error_outline</i>
                                    <span class="flow-text white-text">目前的篩選條件找不到資料，請重新篩選。</span>
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>

        </div>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //載入選單
            $('.select-control').material_select();


            //[搜尋][查詢鈕] - 觸發查詢
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });


            //[搜尋][Enter鍵] - 觸發查詢
            $("#MainContent_filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_KeySearch").trigger("click");

                    e.preventDefault();
                }
            });


        });
    </script>
</asp:Content>

