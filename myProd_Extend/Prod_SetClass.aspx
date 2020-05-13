<%@ Page Title="電子目錄分類設定" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="Prod_SetClass.aspx.cs" Inherits="myProd_Extend_Prod_SetClass" %>

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
                    <h5 class="active section red-text text-darken-2">電子目錄分類設定－<asp:Literal ID="lt_nav" runat="server"></asp:Literal>
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

            <!-- 款式 Section Start -->
            <div class="ui segments">
                <div class="ui blue segment">
                    <h5 class="ui header">款式設定</h5>
                </div>
                <div class="ui small form segment">
                    <div class="fields">
                        <div class="five wide field">
                            <asp:DropDownList ID="ddl_Style" runat="server">
                                <asp:ListItem Value="">請選擇款式</asp:ListItem>
                                <asp:ListItem Value="1">專業款</asp:ListItem>
                                <asp:ListItem Value="2">進階款</asp:ListItem>
                                <asp:ListItem Value="3">經典款</asp:ListItem>
                                <asp:ListItem Value="99">其他</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="eleven wide field">
                            <asp:Button ID="btn_SaveStyle" runat="server" Text="儲存款式" CssClass="ui small blue button" OnClick="btn_SaveStyle_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <!-- 款式 Section End -->
            <!-- 分類 Section Start -->
            <div class="ui segments">
                <div class="ui green segment">
                    <h5 class="ui header">分類階層設定</h5>
                </div>
                <div class="ui segment">
                    <div class="ui small form">
                        <div class="fields">
                            <div class="three wide field">
                                <label>一階分類</label>
                                <asp:DropDownList ID="ddl_Lv1" runat="server" CssClass="fluid" AutoPostBack="true" OnSelectedIndexChanged="ddl_Lv1_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="five wide field">
                                <label>二階分類</label>
                                <asp:DropDownList ID="ddl_Lv2" runat="server" CssClass="fluid" AutoPostBack="true" OnSelectedIndexChanged="ddl_Lv2_SelectedIndexChanged">
                                    <asp:ListItem Value="">請先選擇一階分類</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="three wide field">
                                <label>三階分類</label>
                                <asp:DropDownList ID="ddl_Lv3" runat="server" CssClass="fluid">
                                    <asp:ListItem Value="">請先選擇二階分類</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="two wide field">
                                <label>排序方式</label>
                                <asp:DropDownList ID="ddl_sort" runat="server" CssClass="fluid">
                                    <asp:ListItem Value="1">品號</asp:ListItem>
                                    <asp:ListItem Value="2">成本價</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="three wide field">
                                <label>&nbsp;</label>
                                <a href="<%=thisPage %>" class="ui small grey icon button"><i class="refresh icon"></i></a>
                                <asp:LinkButton ID="btn_Add" runat="server" CssClass="ui small green icon button" OnClick="btn_Add_Click" ToolTip="加入"><i class="plus icon"></i></asp:LinkButton>

                                <asp:HiddenField ID="hf_classID" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="ui divider"></div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                        <LayoutTemplate>
                            <div class="ui green attached segment">
                                <table class="ui celled selectable compact small table">
                                    <thead>
                                        <tr>
                                            <th class="grey-bg lighten-3" colspan="2">一階</th>
                                            <th class="grey-bg lighten-3" colspan="2">二階</th>
                                            <th class="grey-bg lighten-3" colspan="2">三階</th>
                                            <th class="grey-bg lighten-3 collapsing">排序方式</th>
                                            <th class="grey-bg lighten-3"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="right aligned collapsing">
                                    <span class="ui basic label"><%#Eval("Menu_Lv1") %></span>
                                </td>
                                <td>
                                    <strong><%#Eval("MenuNameLv1") %></strong>
                                </td>
                                <td class="right aligned collapsing">
                                    <span class="ui basic label"><%#Eval("Menu_Lv2") %></span>
                                </td>
                                <td>
                                    <strong><%#Eval("MenuNameLv2") %></strong>
                                </td>
                                <td class="right aligned collapsing">
                                    <span class="ui basic label"><%#Eval("Menu_Lv3") %></span>
                                </td>
                                <td>
                                    <strong><%#Eval("MenuNameLv3") %></strong>
                                </td>
                                <td>
                                    <%#getTypeName(Eval("SortType").ToString()) %>
                                </td>
                                <td class="left aligned collapsing">
                                    <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small orange basic icon button" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('確定刪除?')" ToolTip="移除"><i class="trash alternate icon"></i></asp:LinkButton>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Rel_ID") %>' />
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
    <%--    <div class="ui info message">
        <div class="header">
            功能說明
        </div>
        <ul class="list">
            <li class="red-text text-darken-2">
                <h3>*** 對應表匯入 "千萬不可" 與BBC匯入同時進行，否則會有不可預期的後果!!!***</h3>
            </li>
            <li>匯入格式請下載範本。</li>
            <li>商城及客戶的欄位請謹慎填寫。</li>
            <li>匯入時會先將舊資料清空，然後再匯入新資料。</li>
        </ul>
    </div>--%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init dropdown
            $('select').dropdown();


        });
    </script>

</asp:Content>

