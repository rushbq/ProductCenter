<%@ Page Title="產品訊息" Language="C#" MasterPageFile="~/SiteMaster_v1.master" AutoEventWireup="true" CodeFile="View.aspx.cs" Inherits="myProdNews_View" ValidateRequest="false" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">檢視產品訊息<asp:Literal ID="lt_CorpName" runat="server"></asp:Literal></h5>
                    <ol class="breadcrumb">
                        <li><a>產品資料庫</a></li>
                        <li><a href="<%=fn_Param.WebUrl %>myProdNews/Search.aspx">產品訊息</a></li>
                        <li class="active">檢視產品訊息</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <div class="section">
            <asp:PlaceHolder ID="ph_Message" runat="server" Visible="false">
                <div class="card-panel red darken-3">
                    <i class="material-icons flow-text white-text">error_outline</i>
                    <span class="flow-text white-text">資料處理失敗</span>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_Data" runat="server">
                <div class="row">
                    <div class="col s12 m9 l10">
                        <!-- // 內容設定 // -->
                        <div id="base" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">內容設定</h5>
                                </li>
                                <li class="collection-item">
                                    <div class="row">
                                        <div class="col s6">
                                            <label>系統編號</label>
                                            <div class="red-text text-darken-2 center-align">
                                                <b>
                                                    <asp:Literal ID="lt_DataID" runat="server"></asp:Literal></b>
                                            </div>
                                        </div>
                                        <div class="col s6">
                                            <label>BPM流程序號</label>
                                            <div class="green-text text-darken-2 center-align">
                                                <b>
                                                    <asp:Literal ID="lt_BPMSno" runat="server">---</asp:Literal></b>
                                                <asp:Literal ID="lt_BPMFormNo" runat="server" Visible="false"></asp:Literal>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s6">
                                            <asp:DropDownList ID="ddl_Class" runat="server" Enabled="false">
                                                <asp:ListItem Value="1">產品設計變更</asp:ListItem>
                                                <asp:ListItem Value="2">產品淘汰</asp:ListItem>
                                            </asp:DropDownList>
                                            <label for="MainContent_ddl_Class">類別</label>
                                        </div>
                                        <div class="input-field col s6">
                                            <asp:DropDownList ID="ddl_Lang" runat="server" Enabled="false">
                                                <asp:ListItem Value="zh-TW">繁體中文</asp:ListItem>
                                                <asp:ListItem Value="zh-CN">簡體中文</asp:ListItem>
                                            </asp:DropDownList>
                                            <label for="MainContent_ddl_Lang">語系</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s6">
                                            <asp:TextBox ID="tb_Subject" runat="server" MaxLength="80" data-length="80" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div class="col s6">
                                            <label>發送對象*&nbsp;</label>
                                            <asp:CheckBoxList ID="cbl_Target" runat="server" RepeatDirection="Horizontal" Enabled="false">
                                                <asp:ListItem Value="TW">台灣</asp:ListItem>
                                                <asp:ListItem Value="SH">上海</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s6">
                                            <label>時間點*&nbsp;</label>
                                            <asp:RadioButtonList ID="rbl_TimingType" runat="server" CssClass="myRadio" RepeatDirection="Horizontal" Enabled="false">
                                                <asp:ListItem Value="2" Selected="True">庫存用完後變更&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="3">即日起&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="1">開始日&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="99">其他&nbsp;&nbsp;</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <div class="input-field col s6 ">
                                            <i class="material-icons prefix">today</i>
                                            <asp:TextBox ID="tb_TimingDate" runat="server" CssClass="datepicker" Enabled="false"></asp:TextBox>
                                            <label for="MainContent_tb_TimingDate">
                                                開始日</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <asp:TextBox ID="tb_Desc2" runat="server" TextMode="MultiLine" CssClass="materialize-textarea" MaxLength="500" data-length="500" Enabled="false"></asp:TextBox>
                                            <label for="MainContent_tb_Desc1">配合事項</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s12">
                                            <label for="MainContent_tb_Desc1">說明內文</label>
                                            <asp:TextBox ID="tb_Desc1" runat="server" TextMode="MultiLine" CssClass="ckeditor" Enabled="false"></asp:TextBox>

                                        </div>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <!-- // 品號設定 // -->
                        <div id="mainModels" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">品號設定</h5>
                                </li>
                                <li class="collection-item">
                                    <asp:ListView ID="lv_Prod" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2">
                                        <LayoutTemplate>
                                            <table class="bordered striped">
                                                <tbody>
                                                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                                                </tbody>
                                            </table>
                                        </LayoutTemplate>
                                        <GroupTemplate>
                                            <tr>
                                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                                            </tr>
                                        </GroupTemplate>
                                        <ItemTemplate>
                                            <td style="width: 50%">
                                                <%#Eval("Model_No") %>
                                            </td>
                                        </ItemTemplate>
                                        <EmptyItemTemplate>
                                            <td style="width: 50%">&nbsp;</td>
                                        </EmptyItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="center-align grey-text text-lighten-1">
                                                <i class="material-icons flow-text">info_outline</i>
                                                <span class="flow-text">尚未加入產品關聯</span>
                                            </div>

                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </li>
                            </ul>
                        </div>


                        <!-- // 附件檔案 // -->
                        <div id="attachments" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">附件檔案</h5>
                                </li>
                                <li class="collection-item">
                                    <!-- Mail 附圖 -->
                                    <div class="row">
                                        <div class="col s12">
                                            <blockquote class="color-blue">
                                                <h6>Mail附圖</h6>
                                                <div>
                                                    <asp:ListView ID="lv_Files_Mail" runat="server" ItemPlaceholderID="ph_Items">
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
                                                                    <a href="<%#fn_Param.RefUrl %><%#UploadFolder %><%#Eval("AttachFile") %>" target="_blank"><%#Eval("AttachFile_Name") %></a>
                                                                </td>
                                                                <td><%#Eval("AttDesc").ToString().Replace("\r\n","<br/>") %></td>
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

                                    <!-- 其他附件 -->
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
                                                                    <a href="<%#fn_Param.RefUrl %><%#UploadFolder %><%#Eval("AttachFile") %>" target="_blank"><%#Eval("AttachFile_Name") %></a>
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

                                    <!-- BPM表單附件 -->
                                    <div class="row">
                                        <div class="col s12">
                                            <blockquote class="color-red">
                                                <h6>BPM表單附件</h6>
                                                <div>
                                                    <asp:ListView ID="lv_Files_BPM" runat="server" ItemPlaceholderID="ph_Items">
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
                                                                    <a href="<%#fn_Param.RefUrl %><%#BPM_UploadFolder %><%#Eval("AttachFile") %>" target="_blank"><%#Eval("AttachFile_Name") %></a>
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
                                </li>
                            </ul>
                        </div>


                        <!-- // 替代品號 // -->
                        <div id="subModels" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">替代品號</h5>
                                </li>
                                <li class="collection-item">
                                    <asp:ListView ID="lv_SubProd" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2">
                                        <LayoutTemplate>
                                            <table class="bordered striped">
                                                <tbody>
                                                    <asp:PlaceHolder ID="ph_Group" runat="server" />
                                                </tbody>
                                            </table>
                                        </LayoutTemplate>
                                        <GroupTemplate>
                                            <tr>
                                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                                            </tr>
                                        </GroupTemplate>
                                        <ItemTemplate>
                                            <td style="width: 50%">
                                                <%#Eval("Model_No") %>
                                            </td>
                                        </ItemTemplate>
                                        <EmptyItemTemplate>
                                            <td style="width: 50%">&nbsp;</td>
                                        </EmptyItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="center-align grey-text text-lighten-1">
                                                <i class="material-icons flow-text">info_outline</i>
                                                <span class="flow-text">尚未加入替代品號關聯</span>
                                            </div>

                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </li>
                            </ul>
                        </div>


                        <!-- // 維護資訊 // -->
                        <div>
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">維護資訊</h5>
                                </li>
                                <li class="collection-item">
                                    <table class="bordered striped responsive-table">
                                        <tbody>
                                            <tr>
                                                <th>建立資訊
                                                </th>
                                                <td>
                                                    <asp:Literal ID="lt_Creater" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="lt_CreateTime" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>最後更新
                                                </th>
                                                <td>
                                                    <asp:Literal ID="lt_Updater" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="lt_UpdateTime" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Mail發送
                                                </th>
                                                <td>
                                                    <asp:Literal ID="lt_Sender" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="lt_SendTime" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="col hide-on-small-only m3 l2">
                        <!-- // 快速導覽按鈕 // -->
                        <div class="table-Nav">
                            <ul class="table-of-contents">
                                <li><a href="#base">內容設定</a></li>
                                <li><a href="#mainModels">品號設定</a></li>
                                <li><a href="#attachments">附件檔案</a></li>
                                <li><a href="#subModels">替代品號</a></li>
                                <li></li>
                                <li><a href="<%=Page_SearchUrl %>"><i class="material-icons left">list</i>回列表</a></li>
                            </ul>
                        </div>
                    </div>

                </div>

            </asp:PlaceHolder>
        </div>

    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        (function ($) {
            $(function () {
                //載入選單
                $('select').material_select();


                //scrollSpy
                $('.scrollspy').scrollSpy();

                //pushpin
                $('.table-Nav').pushpin({
                    top: 97
                });
            }); // end of document ready
        })(jQuery); // end of jQuery name space
    </script>


    <%-- ckeditor Start --%>
    <script src="//cdn.ckeditor.com/4.9.1/full/ckeditor.js"></script>
    <script>
        CKEDITOR.replace('MainContent_tb_Desc1', {
            customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
        });
    </script>
    <%-- ckeditor End --%>
</asp:Content>

