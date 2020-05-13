<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SampleView.aspx.cs" Inherits="mySample_SampleView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>產品檢驗登記</title>
    <link href="<%=Application["CDN_Url"] %>plugin/google-icon/material-icons.css?family=Material+Icons" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/css/materialize.min.css" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/css/style.css?v=20161123" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="content">
            <!-- Sub Header Start -->
            <div class="grey lighten-3">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l12">
                            <h5 class="breadcrumbs-title">登記明細</h5>
                            <ol class="breadcrumb">
                                <li><a href="<%=Page_SearchUrl %>">產品檢驗登記</a></li>
                                <li class="active">登記明細</li>
                            </ol>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Sub Header End -->
            <!-- Body Content Start -->
            <div class="container">
                <div class="section">
                    <asp:PlaceHolder ID="ph_Message" runat="server">
                        <div class="card-panel red darken-3">
                            <i class="material-icons flow-text white-text">error_outline</i>
                            <span class="flow-text white-text">找不到相關資料</span>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="ph_Data" runat="server">
                        <div class="row">
                            <div class="col s12 m9 l10">
                                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                                    <LayoutTemplate>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <!-- // 基本資料 // -->
                                        <div id="base" class="scrollspy">
                                            <ul class="collection with-header">
                                                <li class="collection-header grey">
                                                    <a href="<%=Page_EditUrl %>#base" class="white-text">
                                                        <h5><i class="material-icons right">keyboard_arrow_right</i>基本資料</h5>
                                                    </a>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">樣品編號</span>
                                                        <span class="secondary-content flow-text"><b><%#Eval("SerialNo") %></b></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">負責人</span>
                                                        <span class="secondary-content grey-text text-darken-3"><%#Eval("Assign_Name") %></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">公司別</span>
                                                        <span class="secondary-content"><span class="label label-default"><%#Eval("Company_Name") %></span></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">來源</span>
                                                        <span class="secondary-content"><span class="label label-success"><%#Eval("Source_Name") %></span></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">檢驗類別</span>
                                                        <span class="secondary-content"><span class="label label-info"><%#Eval("Check_Name") %></span></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">目前狀態</span>
                                                        <span class="secondary-content"><span class="label label-danger"><%#Eval("Status_Name") %></span></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">寶工品號</span>
                                                        <span class="secondary-content grey-text text-darken-3"><%#Eval("Model_No") %></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">管理備註</span>
                                                        <p><%#Eval("Remark").ToString().Replace("\n","<br/>") %></p>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>


                                        <!-- // 產品建議 // -->
                                        <div id="advice" class="scrollspy">
                                            <ul class="collection with-header">
                                                <li class="collection-header grey">
                                                    <a href="<%=Page_EditUrl %>#advice" class="white-text">
                                                        <h5><i class="material-icons right">keyboard_arrow_right</i>產品建議</h5>
                                                    </a>
                                                </li>
                                                <li class="collection-item">
                                                    <div class="row">
                                                        <div class="col s12">
                                                            <asp:TextBox ID="Description4" runat="server" TextMode="MultiLine" CssClass="ckeditor" Enabled="false" Text='<%#Eval("Description4") %>'></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>

                                        <!-- // 最終決議 // -->
                                        <div id="finalcfm" class="scrollspy">
                                            <ul class="collection with-header">
                                                <li class="collection-header grey">
                                                    <a href="<%=Page_EditUrl %>#finalcfm" class="white-text">
                                                        <h5><i class="material-icons right">keyboard_arrow_right</i>最終決議</h5>
                                                    </a>
                                                </li>
                                                <li class="collection-item">
                                                    <div class="row">
                                                        <div class="col s12">
                                                            <asp:TextBox ID="Description5" runat="server" TextMode="MultiLine" CssClass="ckeditor" Enabled="false" Text='<%#Eval("Description5") %>'></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>

                                        <!-- // 廠商資訊 // -->
                                        <div id="cust" class="scrollspy">
                                            <ul class="collection with-header">
                                                <li class="collection-header grey">
                                                    <a href="<%=Page_EditUrl %>#cust" class="white-text">
                                                        <h5><i class="material-icons right">keyboard_arrow_right</i>廠商資訊</h5>
                                                    </a>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">廠商名稱</span>
                                                        <span class="secondary-content"><b><%#Eval("Cust_Name") %></b></span>
                                                    </div>
                                                    <%--<div>
                                                        <asp:Literal ID="lt_SupplierList" runat="server"></asp:Literal>
                                                    </div>--%>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">數量</span>
                                                        <span class="secondary-content grey-text text-darken-3"><%#Eval("Qty") %></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">產品</span>
                                                        <p><%#Eval("Cust_ModelNo") %></p>
                                                        <p><%#Eval("Description1") %></p>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">比對產品/特殊測試要求</span>
                                                        <p><%#Eval("Description2").ToString().Replace("\n","<br/>") %></p>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>


                                        <!-- // 時間資訊 // -->
                                        <div id="time" class="scrollspy">
                                            <ul class="collection with-header">
                                                <li class="collection-header grey">
                                                    <a href="<%=Page_EditUrl %>#base" class="white-text">
                                                        <h5><i class="material-icons right">keyboard_arrow_right</i>時間資訊</h5>
                                                    </a>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">來樣日</span>
                                                        <span class="secondary-content grey-text text-darken-3"><%#Eval("Date_Come") %></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">預計完成日</span>
                                                        <span class="secondary-content grey-text text-darken-3"><%#Eval("Date_Est") %></span>
                                                    </div>
                                                </li>
                                                <li class="collection-item">
                                                    <div>
                                                        <span class="title grey-text text-darken-1">實際完成日</span>
                                                        <span class="secondary-content grey-text text-darken-3"><%#Eval("Date_Actual") %></span>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="section">
                                            <div class="card-panel red darken-3">
                                                <i class="material-icons flow-text white-text">error_outline</i>
                                                <span class="flow-text white-text">找不到資料</span>
                                            </div>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>


                                <!-- // 檔案附件 // -->
                                <div id="attachment" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_EditUrl %>#attachment" class="white-text">
                                                <h5><i class="material-icons right">keyboard_arrow_right</i>檔案附件</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <asp:ListView ID="lv_Attachment" runat="server" ItemPlaceholderID="ph_Items">
                                                <LayoutTemplate>
                                                    <table class="bordered striped">
                                                        <thead>
                                                            <tr>
                                                                <th>檔案</th>
                                                                <th>建立時間</th>
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
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 檢測結果 // -->
                                <div id="chkresult" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_EditUrl %>#chkresult" class="white-text">
                                                <h5><i class="material-icons right">keyboard_arrow_right</i>檢測結果</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="col s12">
                                                    <asp:TextBox ID="Description3" runat="server" TextMode="MultiLine" CssClass="ckeditor" Enabled="false"></asp:TextBox>
                                                </div>
                                            </div>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 樣品資料關聯 // -->
                                <div id="dataRel" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_EditUrl %>#dataRel" class="white-text">
                                                <h5><i class="material-icons right">keyboard_arrow_right</i>樣品資料關聯</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <asp:ListView ID="lv_Sample" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2">
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
                                                    <td>
                                                        <a href="<%#Application["WebUrl"] %>mySample/SampleView.aspx?DataID=<%#Eval("SP_ID") %>"><%#Eval("SerialNo") %></a>
                                                    </td>
                                                </ItemTemplate>
                                                <EmptyItemTemplate>
                                                    <td>&nbsp;</td>
                                                </EmptyItemTemplate>
                                                <EmptyDataTemplate>
                                                    <div class="center-align grey-text text-lighten-1">
                                                        <i class="material-icons flow-text">info_outline</i>
                                                        <span class="flow-text">尚未加入關聯</span>
                                                    </div>

                                                </EmptyDataTemplate>
                                            </asp:ListView>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 品號關聯 // -->
                                <div id="modelRel" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_EditUrl %>#modelRel" class="white-text">
                                                <h5><i class="material-icons right">keyboard_arrow_right</i>品號關聯</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <asp:ListView ID="lv_Prod" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="4">
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
                                                    <td>
                                                        <%#Eval("Model_No") %>
                                                    </td>
                                                </ItemTemplate>
                                                <EmptyItemTemplate>
                                                    <td>&nbsp;</td>
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
                                        <li><a href="#base">基本資料</a></li>
                                        <li><a href="#advice">產品建議</a></li>
                                        <li><a href="#finalcfm">最終決議</a></li>
                                        <li><a href="#cust">廠商資訊</a></li>
                                        <li><a href="#time">時間資訊</a></li>
                                        <li><a href="#attachment">檔案附件</a></li>
                                        <li><a href="#chkresult">檢測結果</a></li>
                                        <li><a href="#dataRel">樣品資料關聯</a></li>
                                        <li><a href="#modelRel">品號關聯</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>

                    </asp:PlaceHolder>
                </div>

                <!-- // Hidden buttons // -->
                <div class="fixed-action-btn toolbar">
                    <a class="btn-floating btn-large red">
                        <i class="large material-icons">menu</i>
                    </a>
                    <ul>
                        <li class="waves-effect waves-light"><a href="<%=Application["WebUrl"] %>mySample/SampleEdit.aspx?DataID=<%=Req_DataID %>"><i class="material-icons">mode_edit</i>編輯資料</a></li>
                        <li class="waves-effect waves-light"><a class="modal-trigger" href="#status-modal"><i class="material-icons">autorenew</i>變更狀態</a></li>
                        <li class="waves-effect waves-light grey"><a href="<%=Page_SearchUrl %>"><i class="material-icons">list</i>回列表</a></li>
                    </ul>
                </div>

                <!-- // Status Modal Structure // -->
                <div id="status-modal" class="modal modal-fixed-footer">
                    <div class="modal-content">
                        <h4>
                            <asp:Literal ID="m_SerialNo" runat="server"></asp:Literal></h4>
                        <div class="divider"></div>
                        <asp:PlaceHolder ID="ph_ModalMessage" runat="server">
                            <div class="card-panel red darken-3">
                                <i class="material-icons flow-text white-text">error_outline</i>
                                <span class="flow-text white-text">您無使用權限</span>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_ModalData" runat="server">
                            <p>
                                目前狀態為：
                            <span class="label label-danger">
                                <asp:Literal ID="m_NowStatus" runat="server"></asp:Literal></span>
                            </p>
                            <div class="row">
                                <div class="col s6">
                                    欲變更狀態為：
                                <div class="input-field inline">
                                    <asp:DropDownList ID="NewStatus" runat="server">
                                    </asp:DropDownList>
                                </div>
                                </div>
                                <div class="input-field col s6">
                                    <asp:LinkButton ID="lbtn_ChangeStatus" runat="server" CssClass="btn waves-effect waves-light blue" OnClick="lbtn_ChangeStatus_Click" ValidationGroup="Status">確定</asp:LinkButton>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                    <div class="modal-footer">
                        <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat">Close</a>
                    </div>
                </div>
            </div>
            <!-- Body Content End -->
        </div>


        <asp:PlaceHolder runat="server">
            <script src="<%=Application["CDN_Url"] %>plugin/jQuery/jquery.min.js"></script>
            <script src="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/js/materialize.min.js"></script>
            <script>
                (function ($) {
                    $(function () {
                        //scrollSpy
                        $('.scrollspy').scrollSpy();

                        //pushpin
                        $('.table-Nav').pushpin({
                            top: 97
                        });

                        //載入選單
                        $('select').material_select();

                        //Modal
                        $('.modal').modal();

                    }); // end of document ready
                })(jQuery); // end of jQuery name space
            </script>
            <%-- ckeditor Start --%>
            <script src="//cdn.ckeditor.com/4.9.1/full/ckeditor.js"></script>
            <script>
                CKEDITOR.inline('Description3', {
                    customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
                });
                CKEDITOR.inline('lvDataList_Description4_0', {
                    customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
                });
                CKEDITOR.inline('lvDataList_Description5_0', {
                    customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
                });


            </script>
            <%-- ckeditor End --%>
        </asp:PlaceHolder>
    </form>
</body>
</html>
