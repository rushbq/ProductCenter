<%@ Page Title="" Language="C#" MasterPageFile="~/SiteBox.master" AutoEventWireup="true" CodeFile="ViewUpReport.aspx.cs" Inherits="myProdCheck_ViewUpReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/css/materialize.min.css" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/css/style.css?v=20170309" rel="stylesheet" />
    <style>
        /* 先移除padding-left, 未來改版再移除此段 */
        #main, footer {
            padding-left: 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="card grey">
        <div class="card-content white-text">
            <h5>查看報告 <i class="material-icons prefix">keyboard_arrow_right</i> <%=Req_ModelNo %></h5>
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
                                        <td style="width: 30%">
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
                                        <td style="width: 30%">
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
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

