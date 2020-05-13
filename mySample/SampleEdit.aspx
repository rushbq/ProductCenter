<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SampleEdit.aspx.cs" Inherits="mySample_SampleEdit" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>產品檢驗登記</title>
    <link href="<%=Application["CDN_Url"] %>plugin/google-icon/material-icons.css?family=Material+Icons" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/css/materialize.min.css" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/css/style.css?v=20161123" rel="stylesheet" />
    <style>
        .collection {
            overflow: visible;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="content">
            <!-- Sub Header Start -->
            <div class="grey lighten-3">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l12">
                            <h5 class="breadcrumbs-title">登記資料編輯</h5>
                            <ol class="breadcrumb">
                                <li><a href="<%=Page_SearchUrl %>">產品檢驗登記</a></li>
                                <li class="active">登記資料編輯</li>
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

                    <asp:PlaceHolder ID="ph_FirstChoose" runat="server">
                        <!-- // 第一次新增資料時的選項 // -->
                        <div class="row">
                            <div class="col s12">
                                <h5>請先選擇公司別</h5>
                                <div>
                                    <asp:LinkButton ID="btn_Choose_TW" runat="server" CssClass="btn btn-large waves-effect waves-light green" OnClick="btn_Choose_TW_Click">台灣</asp:LinkButton>
                                    &nbsp;
                                    <asp:LinkButton ID="btn_Choose_SH" runat="server" CssClass="btn btn-large waves-effect waves-light green" OnClick="btn_Choose_SH_Click">上海</asp:LinkButton>
                                   
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="ph_Data" runat="server">
                        <div class="row">
                            <div class="col s12 m9 l10">
                                <!-- // 基本資料 // -->
                                <div id="base" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <div class="left">
                                                <a href="<%=Page_ViewUrl %>#base" class="white-text">
                                                    <h5><i class="material-icons left">arrow_back</i>基本資料</h5>
                                                </a>
                                            </div>
                                            <div class="right">
                                                <a class="btn waves-effect waves-light blue trigger-Save">存檔</a>
                                            </div>
                                            <div class="clearfix"></div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">樣品編號</span>
                                                <span class="secondary-content flow-text"><b>
                                                    <asp:Literal ID="lt_SerialNo" runat="server"></asp:Literal></b></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">目前狀態</span>
                                                <span class="secondary-content"><span class="label label-danger">
                                                    <asp:Literal ID="lt_Status" runat="server"></asp:Literal></span></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="input-field col s6">
                                                    <asp:DropDownList ID="Source" runat="server">
                                                    </asp:DropDownList>
                                                    <label for="Source">
                                                        來源 *
                                                        &nbsp;<asp:RequiredFieldValidator ID="rfv_Source" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="Source" CssClass="red-text" Display="Dynamic"></asp:RequiredFieldValidator></label>
                                                </div>
                                                <div class="input-field col s6">
                                                    <asp:DropDownList ID="Check" runat="server">
                                                    </asp:DropDownList>
                                                    <label for="Check">
                                                        檢驗類別 *
                                                        &nbsp;<asp:RequiredFieldValidator ID="rfv_Check" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="Check" CssClass="red-text" Display="Dynamic"></asp:RequiredFieldValidator></label>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="input-field col s6">
                                                    <asp:DropDownListGP ID="AssignWho" runat="server">
                                                    </asp:DropDownListGP>
                                                    <label for="AssignWho">
                                                        負責人 *
                                                        &nbsp;<asp:RequiredFieldValidator ID="rfv_AssignWho" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="AssignWho" CssClass="red-text" Display="Dynamic"></asp:RequiredFieldValidator>
                                                    </label>
                                                </div>
                                                <div class="input-field col s6">
                                                    <asp:TextBox ID="ModelNo" runat="server" data-target="hf_ModelNo" CssClass="AC-ModelNo"></asp:TextBox>
                                                    <label for="ModelNo">寶工品號</label>
                                                    <asp:HiddenField ID="hf_ModelNo" runat="server" />
                                                    <div class="grey-text text-darken-2">(輸入關鍵字,出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>)</div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="input-field col s4">
                                                    <i class="material-icons prefix">today</i>
                                                    <asp:TextBox ID="Date_Come" runat="server" CssClass="datepicker"></asp:TextBox>
                                                    <label for="Date_Come">來樣日</label>
                                                </div>
                                                <div class="input-field col s4">
                                                    <i class="material-icons prefix">today</i>
                                                    <asp:TextBox ID="Date_Est" runat="server" CssClass="datepicker"></asp:TextBox>
                                                    <label for="Date_Est">預計完成日</label>
                                                </div>
                                                <div class="input-field col s4">
                                                    <i class="material-icons prefix">today</i>
                                                    <asp:TextBox ID="Date_Actual" runat="server" CssClass="datepicker"></asp:TextBox>
                                                    <label for="Date_Actual">實際完成日</label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="input-field col s12">
                                                    <asp:TextBox ID="Remark" runat="server" CssClass="materialize-textarea" MaxLength="200" length="200" TextMode="MultiLine"></asp:TextBox>
                                                    <label for="Remark">管理備註</label>
                                                </div>
                                            </div>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 產品建議 // -->
                                <div id="advice" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <div class="left">
                                                <a href="<%=Page_ViewUrl %>#advice" class="white-text">
                                                    <h5><i class="material-icons left">arrow_back</i>產品建議</h5>
                                                </a>
                                            </div>
                                            <div class="right">
                                                <a class="btn waves-effect waves-light blue trigger-Save">存檔</a>
                                            </div>
                                            <div class="clearfix"></div>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="col s12">
                                                    <asp:TextBox ID="Description4" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                                                </div>
                                            </div>
                                        </li>
                                    </ul>
                                </div>

                                <!-- // 最終決議 // -->
                                <div id="finalcfm" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <div class="left">
                                                <a href="<%=Page_ViewUrl %>#finalcfm" class="white-text">
                                                    <h5><i class="material-icons left">arrow_back</i>最終決議</h5>
                                                </a>
                                            </div>
                                            <div class="right">
                                                <a class="btn waves-effect waves-light blue trigger-Save">存檔</a>
                                            </div>
                                            <div class="clearfix"></div>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="col s12">
                                                    <asp:TextBox ID="Description5" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                                                </div>
                                            </div>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 廠商資訊 // -->
                                <div id="cust" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <div class="left">
                                                <a href="<%=Page_ViewUrl %>#cust" class="white-text">
                                                    <h5><i class="material-icons left">arrow_back</i>廠商資訊</h5>
                                                </a>
                                            </div>
                                            <div class="right">
                                                <a class="btn waves-effect waves-light blue trigger-Save right">存檔</a>
                                            </div>
                                            <div class="clearfix"></div>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="input-field col s10">
                                                    <asp:TextBox ID="Cust_Name" runat="server" CssClass="AC-Customer" data-target="Cust_ID_Val" data-corp="Cust_Corp"></asp:TextBox>
                                                    <asp:TextBox ID="Cust_ID_Val" runat="server" Style="display: none"></asp:TextBox>
                                                    <asp:TextBox ID="Cust_Corp" runat="server" Style="display: none"></asp:TextBox>
                                                    <label for="Cust_Name">廠商名稱(ERP)</label>
                                                    <div class="grey-text text-darken-2">(輸入關鍵字, 出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>, 若無資料請前往ERP設定)</div>
                                                </div>
                                                <div class="input-field col s2">
                                                     <a class="btn waves-effect waves-light grey clearCust">清除</a>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="input-field col s12">
                                                    <asp:TextBox ID="Cust_Newguy" runat="server"></asp:TextBox>
                                                    <label for="Cust_Newguy">新廠商</label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="input-field col s6">
                                                    <asp:TextBox ID="Cust_ModelNo" runat="server"></asp:TextBox>
                                                    <label for="Cust_ModelNo">廠商品號</label>
                                                </div>
                                                <div class="input-field col s6">
                                                    <asp:TextBox ID="Qty" runat="server"></asp:TextBox>
                                                    <label for="Qty">數量</label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="input-field col s12">
                                                    <asp:TextBox ID="Description1" runat="server" MaxLength="100" length="100"></asp:TextBox>
                                                    <label for="Description1">產品描述</label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="input-field col s12">
                                                    <asp:TextBox ID="Description2" runat="server" CssClass="materialize-textarea" MaxLength="200" length="200" TextMode="MultiLine"></asp:TextBox>
                                                    <label for="Description2">比對產品/特殊測試要求</label>
                                                </div>
                                            </div>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 檔案附件 // -->
                                <div id="attachment" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_ViewUrl %>#attachment" class="white-text">
                                                <h5><i class="material-icons left">arrow_back</i>檔案附件</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
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
                                            <div class="red-text">
                                                <asp:Literal ID="lt_UploadMessage" runat="server"></asp:Literal>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <asp:ListView ID="lv_Attachment" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_Attachment_ItemCommand">
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
                                                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("AttachID") %>' />
                                                            <asp:HiddenField ID="hf_FileName" runat="server" Value='<%#Eval("AttachFile") %>' />
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
                                            <div class="left">
                                                <a href="<%=Page_ViewUrl %>#chkresult" class="white-text">
                                                    <h5><i class="material-icons left">arrow_back</i>檢測結果</h5>
                                                </a>
                                            </div>
                                            <div class="right">
                                                <a class="btn waves-effect waves-light blue trigger-Save">存檔</a>
                                            </div>
                                            <div class="clearfix"></div>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="col s12">
                                                    <asp:TextBox ID="Description3" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                                                </div>
                                            </div>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 樣品資料關聯 // -->
                                <div id="dataRel" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_ViewUrl %>#dataRel" class="white-text">
                                                <h5><i class="material-icons left">arrow_back</i>樣品資料關聯</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="input-field col s8">
                                                    <asp:TextBox ID="Rel_SampleID" runat="server" CssClass="AC-ProdSample" data-target="Rel_SampleID_Val" ValidationGroup="AddRel"></asp:TextBox>
                                                    <asp:TextBox ID="Rel_SampleID_Val" runat="server" ValidationGroup="AddRel" Style="display: none"></asp:TextBox>
                                                    <label for="Rel_SampleID">
                                                        樣品編號
                                                        <asp:RequiredFieldValidator ID="rfv_Rel_SampleID_Val" runat="server" ErrorMessage="請填寫「樣品編號」" ControlToValidate="Rel_SampleID_Val" Display="Dynamic" ValidationGroup="AddRel" CssClass="red-text"></asp:RequiredFieldValidator>
                                                    </label>
                                                    <div class="grey-text text-darken-2">(輸入關鍵字, 出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>, 並按下<span class="flow-text"><b class="orange-text text-darken-3">加入關聯</b></span>)</div>
                                                </div>
                                                <div class="input-field col s4">
                                                    <asp:LinkButton ID="lbtn_AddRel" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="AddRel" OnClick="lbtn_AddRel_Click">加入關聯</asp:LinkButton>
                                                </div>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <asp:ListView ID="lv_Sample" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2" OnItemCommand="lv_Sample_ItemCommand">
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
                                                    <td style="width: 35%">
                                                        <a href="<%#Application["WebUrl"] %>mySample/SampleEdit.aspx?DataID=<%#Eval("SP_ID") %>"><%#Eval("SerialNo") %></a>
                                                    </td>
                                                    <td style="width: 15%" class="center-align">
                                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('是否確定刪除關聯?')" ValidationGroup="ListRel"><i class="material-icons">clear</i></asp:LinkButton>
                                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("SP_ID") %>' />
                                                    </td>
                                                </ItemTemplate>
                                                <EmptyItemTemplate>
                                                    <td style="width: 35%">&nbsp;</td>
                                                    <td style="width: 15%">&nbsp;</td>
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
                                            <a href="<%=Page_ViewUrl %>#modelRel" class="white-text">
                                                <h5><i class="material-icons left">arrow_back</i>品號關聯</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="input-field col s8">
                                                    <asp:TextBox ID="Rel_ModelNo" runat="server" CssClass="AC-ModelNo" data-target="Rel_ModelNo_Val" ValidationGroup="AddProd"></asp:TextBox>
                                                    <asp:TextBox ID="Rel_ModelNo_Val" runat="server" ValidationGroup="AddProd" Style="display: none"></asp:TextBox>
                                                    <label for="Rel_ModelNo">
                                                        品號
                                                        <asp:RequiredFieldValidator ID="rfv_Rel_ModelNo_Val" runat="server" ErrorMessage="請填寫「品號」" ControlToValidate="Rel_ModelNo_Val" Display="Dynamic" ValidationGroup="AddProd" CssClass="red-text"></asp:RequiredFieldValidator>
                                                    </label>
                                                </div>
                                                <div class="input-field col s4">
                                                    <asp:LinkButton ID="lbtn_AddProd" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="AddProd" OnClick="lbtn_AddProd_Click">加入關聯</asp:LinkButton>
                                                </div>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <asp:ListView ID="lv_Prod" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2" OnItemCommand="lv_Prod_ItemCommand">
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
                                                    <td style="width: 35%">
                                                        <%#Eval("Model_No") %>
                                                    </td>
                                                    <td style="width: 15%" class="center-align">
                                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('是否確定刪除關聯?')" ValidationGroup="ListProd"><i class="material-icons">clear</i></asp:LinkButton>
                                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Model_No") %>' />
                                                    </td>
                                                </ItemTemplate>
                                                <EmptyItemTemplate>
                                                    <td style="width: 35%">&nbsp;</td>
                                                    <td style="width: 15%">&nbsp;</td>
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


                                <!-- // 發信通知 // -->
                                <div id="email" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <div class="left">
                                                <h5 class="white-text">發信通知</h5>
                                            </div>
                                            <div class="right">
                                                <a id="doSet" class="btn waves-effect waves-light blue">確認發信</a>
                                            </div>
                                            <div class="clearfix"></div>
                                        </li>
                                        <li class="collection-item">
                                            <div class="row">
                                                <div class="input-field col s12">
                                                    <asp:TextBox ID="tb_MailSubject" runat="server" MaxLength="150" placeholder="請填寫郵件主旨"></asp:TextBox>
                                                    <label for="tb_MailSubject">主旨</label>
                                                </div>
                                            </div>
                                            <div id="userList" class="ztree"></div>
                                        </li>
                                    </ul>

                                    <div style="display: none" class="serversidecontroller">
                                        <asp:Button ID="btn_Setting" runat="server" Text="Set" OnClick="btn_Setting_Click" />
                                        <asp:TextBox ID="tb_Values_User" runat="server"></asp:TextBox>
                                    </div>
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
                                        <li><a href="#attachment">檔案附件</a></li>
                                        <li><a href="#chkresult">檢測結果</a></li>
                                        <li><a href="#dataRel">樣品資料關聯</a></li>
                                        <li><a href="#modelRel">品號關聯</a></li>
                                        <li><a href="#email">發信通知</a></li>
                                    </ul>
                                </div>
                            </div>

                            <!-- // Hidden buttons // -->
                            <div class="SrvSide-Buttons" style="display: none;">
                                <asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="btn_Save_Click" />
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" HeaderText="尚有資料未填寫" />
                            </div>
                        </div>

                    </asp:PlaceHolder>
                </div>

                <!-- // Bottom buttons // -->
                <div class="fixed-action-btn toolbar">
                    <a class="btn-floating btn-large red">
                        <i class="large material-icons">menu</i>
                    </a>
                    <ul>
                        <asp:PlaceHolder ID="ph_Buttons" runat="server">
                            <li class="waves-effect waves-light">
                                <asp:LinkButton ID="lbtn_DelData" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('是否確定刪除資料?\n注意:刪除後無法復原!')" OnClick="lbtn_DelData_Click" ValidationGroup="DelData"><i class="material-icons">delete_forever</i>刪除資料</asp:LinkButton>
                            </li>
                            <li class="waves-effect waves-light"><a class="modal-trigger" href="#status-modal"><i class="material-icons">autorenew</i>變更狀態</a></li>
                        </asp:PlaceHolder>
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
            <script src="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
            <script>
                (function ($) {
                    $(function () {
                        //載入datepicker
                        $('.datepicker').pickadate({
                            selectMonths: true, // Creates a dropdown to control month
                            selectYears: 5, // Creates a dropdown of 15 years to control year
                            format: 'yyyy-mm-dd',

                            closeOnSelect: false // Close upon selecting a date(此版本無作用)
                        }).on('change', function () {
                            $(this).next().find('.picker__close').click();
                        });

                        //載入選單
                        $('select').material_select();

                        //Modal
                        $('.modal').modal();

                        //scrollSpy
                        $('.scrollspy').scrollSpy();

                        //pushpin
                        $('.table-Nav').pushpin({
                            top: 97
                        });


                        //trigger Save
                        $(".trigger-Save").click(function () {
                            $("#btn_Save").trigger("click");
                        });


                        //clear 
                        $(".clearCust").click(function () {
                            $("#Cust_Name").val('');
                            $("#Cust_ID_Val").val('');
                            $("#Cust_Corp").val('');
                        });

                    }); // end of document ready
                })(jQuery); // end of jQuery name space
            </script>

            <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.css" rel="stylesheet" />
            <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.css" rel="stylesheet" />
            <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.js"></script>
            <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.js"></script>
            <%-- Catcomplete Start --%>
            <script>
                /* Autocomplete 品號關聯 */
                $(".AC-ModelNo").catcomplete({
                    minLength: 1,  //至少要輸入 n 個字元
                    source: function (request, response) {
                        $.ajax({
                            url: "<%=Application["WebUrl"]%>Ajax_Data/GetData_Prod.ashx",
                            data: {
                                keyword: request.term
                            },
                            type: "POST",
                            dataType: "json",
                            success: function (data) {
                                if (data != null) {
                                    response($.map(data, function (item) {
                                        return {
                                            id: item.ID,
                                            label: item.Label,
                                            category: item.Category
                                        }
                                    }));
                                }
                            }
                        });
                    },
                    select: function (event, ui) {
                        //目前欄位
                        $(this).val(ui.item.value);

                        //實際欄位-儲存值
                        var targetID = $(this).attr("data-target");
                        $("#" + targetID).val(ui.item.id);

                        event.preventDefault();
                    }
                });
            </script>
            <script>
                /* Autocomplete 樣品關聯 */
                $(".AC-ProdSample").autocomplete({
                    minLength: 1,  //至少要輸入 n 個字元
                    source: function (request, response) {
                        $.ajax({
                            url: "<%=Application["WebUrl"]%>Ajax_Data/GetData_ProdSample.ashx",
                            data: {
                                keyword: request.term,
                                id: '<%=Req_DataID%>'
                            },
                            type: "POST",
                            dataType: "json",
                            success: function (data) {
                                if (data != null) {
                                    response($.map(data, function (item) {
                                        return {
                                            id: item.ID,
                                            label: item.Label
                                        }
                                    }));
                                }
                            }
                        });
                    },
                    select: function (event, ui) {
                        //目前欄位
                        $(this).val(ui.item.value);

                        //實際欄位-儲存值
                        var targetID = $(this).attr("data-target");
                        $("#" + targetID).val(ui.item.id);

                        event.preventDefault();
                    }
                });
            </script>
            <script>
                /* Autocomplete 供應商 */
                $(".AC-Customer").autocomplete({
                    minLength: 1,  //至少要輸入 n 個字元
                    source: function (request, response) {
                        $.ajax({
                            url: "<%=Application["WebUrl"]%>Ajax_Data/GetData_erpSupplier.ashx",
                            data: {
                                keyword: request.term
                            },
                            type: "POST",
                            dataType: "json",
                            success: function (data) {
                                if (data != null) {
                                    response($.map(data, function (item) {
                                        return {
                                            id: item.ID,
                                            label: item.Label,
                                            corp: item.CorpID
                                        }
                                    }));
                                }
                            }
                        });
                    },
                    select: function (event, ui) {
                        //目前欄位
                        $(this).val(ui.item.value);

                        //實際欄位-儲存值
                        var targetID = $(this).attr("data-target");
                        var corp = $(this).attr("data-corp");
                        $("#" + targetID).val(ui.item.id);
                        $("#" + corp).val(ui.item.corp);

                        event.preventDefault();
                    }
                });
            </script>
            <%-- Catcomplete End --%>

            <%-- ckeditor Start --%>
            <script src="//cdn.ckeditor.com/4.9.1/full/ckeditor.js"></script>
            <script>
                CKEDITOR.replace('Description3', {
                    customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
                });
                CKEDITOR.replace('Description4', {
                    customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
                });
                CKEDITOR.replace('Description5', {
                    customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
                });
            </script>
            <%-- ckeditor End --%>

            <asp:PlaceHolder ID="ph_treeJS" runat="server">
                <link rel="stylesheet" href="<%=fn_Param.WebUrl %>plugins/zTree/css/style.min.css" />
                <script src="<%=fn_Param.WebUrl %>plugins/zTree/jquery.ztree.core-3.5.min.js"></script>
                <script src="<%=fn_Param.WebUrl %>plugins/zTree/jquery.ztree.excheck-3.5.min.js"></script>
                <script>
                    //--- zTree 設定 Start ---
                    var setting = {
                        view: {
                            dblClickExpand: false   //已使用onclick展開,故將雙擊展開關閉                    
                        },
                        callback: {
                            onClick: MMonClick
                        },
                        check: {
                            enable: true
                        },
                        data: {
                            simpleData: {
                                enable: true
                            }
                        }
                    };

                    //Event - onClick
                    function MMonClick(e, treeId, treeNode) {
                        var zTree = $.fn.zTree.getZTreeObj(treeId);
                        zTree.expandNode(treeNode);
                    }
                    //--- zTree 設定 End ---
                </script>
                <script>
                    $(function () {
                        /*
                            取得人員List
                        */
                        var jqxhr = $.post("<%=fn_Param.WebUrl%>Ajax_Data/GetUserList.ashx", {
                            area: 'ALL'
                        })
                          .done(function (data) {
                              //載入選單
                              $.fn.zTree.init($("#userList"), setting, data)
                          })
                          .fail(function () {
                              alert("人員選單載入失敗");
                          });


                        /*
                            取得已勾選的項目ID
                        */
                        $("#doSet").on("click", function () {
                            //confirm
                            var r = confirm("確定要發信??\n「確定」:開始發信\n「取消」:繼續編輯");
                            if (r == false) {
                                return false;
                            }

                            var myTreeName_User = "userList";
                            var valAry = [];
                            var valAry_User = [];

                            //宣告tree物件
                            var treeObj_User = $.fn.zTree.getZTreeObj(myTreeName_User);

                            //取得節點array
                            var nodes_User = treeObj_User.getCheckedNodes(true);


                            //將id丟入陣列
                            for (var row = 0; row < nodes_User.length; row++) {
                                //只取開頭為'v_'的值
                                var myval = nodes_User[row].id;
                                if (myval.substring(0, 2) == "v_") {
                                    valAry_User.push(myval.replace("v_", ""));
                                }
                            }

                            //Check null
                            if (valAry_User == "") {
                                alert('你未做任何勾選，請確認!')
                                return false;
                            }

                            //將陣列組成以','分隔的字串，並填入欄位
                            $("#tb_Values_User").val(valAry_User.join(","));

                            //觸發設定 click
                            $("#btn_Setting").trigger("click");

                        });
                    });
                </script>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </form>
</body>
</html>
