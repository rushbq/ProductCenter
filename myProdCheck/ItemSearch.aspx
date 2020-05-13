<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ItemSearch.aspx.cs" Inherits="myProdCheck_Search" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <div class="card">
                        <asp:TextBox ID="filter_Keyword" runat="server" placeholder="查詢品號" autocomplete="off" Style="display: block; font-size: 16px; font-weight: 300; width: 90%; height: 45px; margin: 0; padding: 0 0px 0 10px; border: 0;"></asp:TextBox>
                        <i id="trigger-keySearch" class="material-icons" style="position: absolute; top: 10px; right: 10px; cursor: pointer;">search</i>
                        <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                    </div>
                    <ol class="breadcrumb">
                        <li><a>外驗查檢表</a></li>
                        <li class="active">品號附件維護</li>
                    </ol>
                </div>
            </div>
        </div>

    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <div class="row">
            <div class="col s12">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                    <LayoutTemplate>
                        <table class="bordered striped">
                            <thead>
                                <tr>
                                    <th style="text-align: center; width: 250px;">品號</th>
                                    <th>品名</th>
                                    <th style="text-align:center">已上傳</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </table>
                        <div class="right-align">
                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <span class="flow-text red-text text-darken-1"><strong><%#Eval("ModelNo")%></strong></span>
                            </td>
                            <td>
                                <p><%#Eval("ModelName") %></p>
                            </td>
                            <td style="text-align:center">
                                <strong><%#Eval("AttachCnt") %></strong>
                            </td>
                            <td style="text-align:center">
                                <a class="waves-effect waves-light btn white grey-text text-darken-3" href="<%=Application["WebUrl"] %>myProdCheck/ItemEdit.aspx?DataID=<%#HttpUtility.UrlEncode(Eval("ModelNo").ToString()) %>"><i class="material-icons left">edit</i>詳細資料</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="section">
                            <div class="card-panel grey darken-1">
                                <i class="material-icons flow-text white-text">error_outline</i>
                                <span class="flow-text white-text">找不到資料</span>
                            </div>
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>
    <!-- Body Content End -->

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //[搜尋][查詢鈕] - 觸發關鍵字快查
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });

            //[搜尋][Enter鍵] - 觸發關鍵字快查
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

