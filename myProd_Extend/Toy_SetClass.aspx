<%@ Page Title="科學玩具分類設定" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="Toy_SetClass.aspx.cs" Inherits="myProd_Extend_Toy_SetClass" %>

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
                    <div class="section">產品資料庫</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section red-text text-darken-2">科學玩具分類設定－<asp:Literal ID="lt_nav" runat="server"></asp:Literal>
                    </h5>
                </div>
            </div>
            <div class="right menu">
                <a href="<%=backPage %>" class="item"><i class="undo icon"></i><span class="mobile hidden">返回</span></a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                <div class="ui negative message">
                    <div class="header">
                        請注意!!
                    </div>
                    <ul>
                        <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                    </ul>
                </div>
            </asp:PlaceHolder>

            <!-- 分類 Section Start -->
            <div class="ui segments">
                <div class="ui green segment">
                    <h5 class="ui header">分類設定</h5>
                </div>
                <div class="ui segment">
                    <div class="ui small form">
                        <div class="fields">
                            <div class="five wide field">
                                <label>選擇分類</label>
                                <asp:DropDownList ID="ddl_Class" runat="server" CssClass="fluid">
                                </asp:DropDownList>
                            </div>
                            <div class="three wide field">
                                <label>&nbsp;</label>
                                <asp:LinkButton ID="btn_Add" runat="server" CssClass="ui small green icon button" OnClick="btn_Add_Click" ToolTip="加入"><i class="plus icon"></i></asp:LinkButton>

                            </div>
                        </div>
                    </div>
                    <div class="ui divider"></div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                        <LayoutTemplate>
                            <table class="ui celled selectable compact table">
                                <thead>
                                    <tr>
                                        <th class="grey-bg lighten-3" colspan="2">分類名稱</th>
                                        <th class="grey-bg lighten-3"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="right aligned collapsing">
                                    <span class="ui basic label"><%#Eval("ID") %></span>
                                </td>
                                <td>
                                    <strong><%#Eval("Label_TW") %></strong> / 
                                    <strong><%#Eval("Label_EN") %></strong> / 
                                    <strong><%#Eval("Label_CN") %></strong>
                                </td>
                                <td class="left aligned collapsing">
                                    <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small orange basic icon button" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('確定刪除?')" ToolTip="移除"><i class="trash alternate icon"></i></asp:LinkButton>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("ID") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="ui placeholder segment">
                                <div class="ui icon header">
                                    <i class="blind icon"></i>
                                    分類未設定
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>
            <!-- 分類 Section End -->
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init dropdown
            $('select').dropdown();


        });
    </script>

</asp:Content>

