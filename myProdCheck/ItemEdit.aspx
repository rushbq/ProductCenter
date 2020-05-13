<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ItemEdit.aspx.cs" Inherits="myProdCheck_ItemEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>myProdCheck/ItemSearch.aspx"><i class="material-icons left">arrow_back</i>返回列表</a></li>
                    </ul>
                    <span class="brand-logo center">上傳品號附件 (<%=Req_DataID %>)</span>
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

                <div class="card-panel orange darken-1 white-text">
                    <asp:Literal ID="lt_UploadMessage" runat="server"><i class="material-icons left">info</i>檔案上傳可一次上傳多筆</asp:Literal>
                </div>

                <div class="card grey">
                    <div class="card-content white-text">
                        <h5>附件上傳</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s8">
                                <div class="file-field input-field">
                                    <div class="btn">
                                        <span>File</span>
                                        <asp:FileUpload ID="file_Upload" runat="server" AllowMultiple="true" />
                                    </div>
                                    <div class="file-path-wrapper">
                                        <input class="file-path validate" type="text" placeholder="上傳一個或多個檔案">
                                    </div>
                                </div>
                            </div>
                            <div class="input-field col s4">
                                <asp:LinkButton ID="lbtn_AddFiles" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="AddFiles" OnClick="lbtn_AddFiles_Click">開始上傳</asp:LinkButton>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col s12">
                                <asp:ListView ID="lv_Files" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_Files_ItemCommand">
                                    <LayoutTemplate>
                                        <table class="bordered striped">
                                            <thead>
                                                <tr>
                                                    <th>檔案</th>
                                                    <th>建立時間</th>
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
                                            <td>
                                                <a href="<%#Application["RefUrl"] %><%#UploadFolder %><%#Eval("AttachFile") %>" target="_blank"><%#Eval("AttachFile_Name") %></a>
                                            </td>
                                            <td>
                                                <%#Eval("Create_Time")%>
                                            </td>
                                            <td class="center-align">
                                                <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('是否確定刪除?')" ValidationGroup="ListAttach"><i class="material-icons">clear</i></asp:LinkButton>
                                                <asp:HiddenField ID="hf_AttachID" runat="server" Value='<%#Eval("AttachID") %>' />
                                                <asp:HiddenField ID="hf_FileName" runat="server" Value='<%#Eval("AttachFile") %>' />
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

