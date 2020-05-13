<%@ Page Title="" Language="C#" MasterPageFile="~/SiteBox.master" AutoEventWireup="true" CodeFile="ViewDwReport.aspx.cs" Inherits="myProdCheck_ViewDwReport" EnableEventValidation="false" %>

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
            <h5>下載查檢表 <i class="material-icons prefix">keyboard_arrow_right</i> <%=Req_ModelNo %></h5>
        </div>
        <div class="card-content grey lighten-5">
            <div class="row">
                <div class="col s12">
                    <blockquote class="color-blue">
                        <h6>查檢表</h6>
                        <div>
                            <a href="<%=Application["WebUrl"] %>myHandler/GetFile_CheckView.ashx?dataID=<%=Req_DataID %>&modelNo=<%=Server.UrlEncode(Req_ModelNo) %>" class="btn waves-effect waves-light blue darken-1" target="_blank">點我下載PDF</a>
                        </div>
                    </blockquote>
                </div>
            </div>
            <div class="row">
                <div class="col s12">
                    <blockquote class="color-green">
                        <h6>其他相關附件</h6>
                        <asp:ListView ID="lv_Files" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <div class="collection">
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </div>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <a href="<%#Application["RefUrl"] %><%#UploadFolder %><%#Eval("AttachFile") %>" class="collection-item" target="_blank"><i class="material-icons right">attach_file</i><%#Eval("AttachFile_Name") %></a>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="center-align grey-text text-lighten-1">
                                    <i class="material-icons flow-text">info_outline</i>
                                    <span class="flow-text">無其他附件</span>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </blockquote>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

