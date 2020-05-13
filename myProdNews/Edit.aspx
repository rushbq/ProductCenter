<%@ Page Title="產品訊息" Language="C#" MasterPageFile="~/SiteMaster_v1.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="myProdNews_Edit" ValidateRequest="false" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">編輯產品訊息<asp:Literal ID="lt_CorpName" runat="server"></asp:Literal></h5>
                    <ol class="breadcrumb">
                        <li><a>產品資料庫</a></li>
                        <li><a href="<%=fn_Param.WebUrl %>myProdNews/Search.aspx">產品訊息</a></li>
                        <li class="active">編輯產品訊息</li>
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
                                            <asp:DropDownList ID="ddl_Class" runat="server">
                                                <asp:ListItem Value="1">產品設計變更</asp:ListItem>
                                                <asp:ListItem Value="2">產品淘汰</asp:ListItem>
                                            </asp:DropDownList>
                                            <label for="MainContent_ddl_Class">類別</label>
                                            <asp:RequiredFieldValidator ID="rfv_ddl_Class" runat="server" ErrorMessage="必填欄位請填寫" ControlToValidate="ddl_Class" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="input-field col s6">
                                            <asp:DropDownList ID="ddl_Lang" runat="server">
                                                <asp:ListItem Value="zh-TW">繁體中文</asp:ListItem>
                                                <asp:ListItem Value="zh-CN">簡體中文</asp:ListItem>
                                            </asp:DropDownList>
                                            <label for="MainContent_ddl_Lang">語系</label>
                                            <asp:RequiredFieldValidator ID="rfv_ddl_Lang" runat="server" ErrorMessage="必填欄位請填寫" ControlToValidate="ddl_Lang" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s6">
                                            <asp:TextBox ID="tb_Subject" runat="server" MaxLength="80" data-length="80"></asp:TextBox>
                                            <label for="MainContent_tb_Subject">主旨*&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Subject" runat="server" ErrorMessage="必填欄位請填寫" ControlToValidate="tb_Subject" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator></label>
                                        </div>
                                        <div class="col s6">
                                            <label>發送對象*&nbsp;<asp:CustomValidator ID="cv_cbl_Target" runat="server" Display="Dynamic" ClientValidationFunction="Check_Target" CssClass="red-text" ValidationGroup="Add" ErrorMessage="必填欄位請填寫"></asp:CustomValidator></label>
                                            <asp:CheckBoxList ID="cbl_Target" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="TW">台灣</asp:ListItem>
                                                <asp:ListItem Value="SH">上海</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s6">
                                            <label>時間點*&nbsp;</label>
                                            <asp:RadioButtonList ID="rbl_TimingType" runat="server" CssClass="myRadio" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="2" Selected="True">庫存用完後變更&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="3">即日起&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="1">開始日&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="99">其他&nbsp;&nbsp;</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                        <div class="input-field col s6 ">
                                            <i class="material-icons prefix">today</i>
                                            <asp:TextBox ID="tb_TimingDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                            <label for="MainContent_tb_TimingDate">
                                                開始日&nbsp;<asp:RegularExpressionValidator ID="rev_tb_TimingDate" runat="server"
                                                    ControlToValidate="tb_TimingDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                                                    Display="Dynamic" ValidationGroup="Add" ErrorMessage="格式錯誤"> </asp:RegularExpressionValidator></label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <asp:TextBox ID="tb_Desc2" runat="server" TextMode="MultiLine" CssClass="materialize-textarea" MaxLength="500" data-length="500"></asp:TextBox>
                                            <label for="MainContent_tb_Desc2">配合事項</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s12">
                                            <label for="MainContent_tb_Desc1">說明內文</label>
                                            <asp:TextBox ID="tb_Desc1" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>

                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s12 right-align">
                                            <a class="btn waves-effect waves-light blue trigger-Save"><i class="material-icons left">save</i>存檔</a>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s12">
                                        </div>
                                    </div>
                                    <div class="card-panel orange darken-1 white-text">
                                        <i class="material-icons left">info</i>提醒您!! 在進行其他功能操作前， ↑↑上方內容若有變動↑↑，請記得按下「<span class="flow-text">存檔</span>」鈕。
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
                                    <div class="row">
                                        <div class="input-field col s8">
                                            <asp:TextBox ID="Rel_ModelNo" runat="server" CssClass="AC-ModelNo" data-target="MainContent_Rel_ModelNo_Val" ValidationGroup="AddProd"></asp:TextBox>
                                            <asp:TextBox ID="Rel_ModelNo_Val" runat="server" ValidationGroup="AddProd" Style="display: none"></asp:TextBox>
                                            <label for="MainContent_Rel_ModelNo">
                                                品號&nbsp;<asp:RequiredFieldValidator ID="rfv_Rel_ModelNo_Val" runat="server" ErrorMessage="請選擇正確的品號" ControlToValidate="Rel_ModelNo_Val" Display="Dynamic" ValidationGroup="AddProd" CssClass="red-text"></asp:RequiredFieldValidator>
                                            </label>
                                            <div class="grey-text text-darken-2">(輸入關鍵字, 出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>, 並按下<span class="flow-text pink-text text-lighten-1">加入關聯</span>)</div>
                                        </div>
                                        <div class="input-field col s4">
                                            <asp:LinkButton ID="lbtn_AddProd" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="AddProd" OnClick="lbtn_AddProd_Click"><i class="material-icons right">expand_more</i>加入關聯</asp:LinkButton>
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


                        <!-- // 附件檔案 // -->
                        <div id="attachments" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <div class="left">
                                        <h5 class="white-text">附件檔案</h5>
                                    </div>
                                    <div class="right">
                                        <asp:PlaceHolder ID="ph_UploadBtn" runat="server">
                                            <a href="<%=fn_Param.WebUrl %>myProdNews/EditUpload.aspx?DataID=<%=Req_DataID %>" class="btn waves-effect waves-light green"><i class="material-icons right">keyboard_arrow_right</i>附件維護</a>
                                        </asp:PlaceHolder>
                                    </div>
                                    <div class="clearfix"></div>
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
                                    <div class="row">
                                        <div class="input-field col s8">
                                            <asp:TextBox ID="Rel_SubModelNo" runat="server" CssClass="AC-ModelNo" data-target="MainContent_Rel_SubModelNo_Val" ValidationGroup="AddSubProd"></asp:TextBox>
                                            <asp:TextBox ID="Rel_SubModelNo_Val" runat="server" ValidationGroup="AddSubProd" Style="display: none"></asp:TextBox>
                                            <label for="MainContent_Rel_ModelNo">
                                                品號&nbsp;<asp:RequiredFieldValidator ID="rfv_Rel_SubModelNo_Val" runat="server" ErrorMessage="請選擇正確的品號" ControlToValidate="Rel_SubModelNo_Val" Display="Dynamic" ValidationGroup="AddSubProd" CssClass="red-text"></asp:RequiredFieldValidator>
                                            </label>
                                            <div class="grey-text text-darken-2">(輸入關鍵字, 出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>, 並按下<span class="flow-text pink-text text-lighten-1">加入關聯</span>)</div>
                                        </div>
                                        <div class="input-field col s4">
                                            <asp:LinkButton ID="lbtn_AddSubProd" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="AddSubProd" OnClick="lbtn_AddSubProd_Click"><i class="material-icons right">expand_more</i>加入關聯</asp:LinkButton>
                                        </div>
                                    </div>
                                </li>
                                <li class="collection-item">
                                    <asp:ListView ID="lv_SubProd" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2" OnItemCommand="lv_SubProd_ItemCommand">
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

                    <!-- // Hidden buttons // -->
                    <div class="SrvSide-Buttons" style="display: none;">
                        <asp:HiddenField ID="hf_DataID" runat="server" />
                        <asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" />
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" HeaderText="資料填寫不完整，請重新確認!" ValidationGroup="Add" />
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
    <script src="<%=fn_Param.CDNUrl %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
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

                //載入datepicker
                $('.datepicker').pickadate({
                    selectMonths: true, // Creates a dropdown to control month
                    selectYears: 5, // Creates a dropdown of 15 years to control year
                    format: 'yyyy-mm-dd'
                }).on('change', function () {
                    $(this).next().find('.picker__close').click();
                });


                //trigger Save
                $(".trigger-Save").click(function () {
                    $("#MainContent_btn_Save").trigger("click");
                });


                //取值
                $(".myRadio input[type='radio']").change(function () {
                    var val = this.value;
                    if (val != "1") {
                        $("#MainContent_tb_TimingDate").val('');
                    }
                });


            }); // end of document ready
        })(jQuery); // end of jQuery name space


        //Check 發送對象是否勾選(搭配 CustomValidator )
        function Check_Target(sender, args) {
            var flagNum = 0;
            var optList = document.getElementById("MainContent_cbl_Target");
            var inArr = optList.getElementsByTagName('input');
            for (var i = 0; i < inArr.length; i++) {
                if (inArr[i].type == "checkbox") {
                    if (inArr[i].checked == true) {
                        flagNum += 1;
                    }
                }
            }
            if (flagNum == 0) {
                args.IsValid = false;
            }
            else {
                args.IsValid = true;
            }
        }
    </script>

    <link href="<%=fn_Param.CDNUrl %>plugin/jqueryUI-1.12.1/jquery-ui.min.css" rel="stylesheet" />
    <link href="<%=fn_Param.CDNUrl %>plugin/jqueryUI-1.12.1/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="<%=fn_Param.CDNUrl %>plugin/jqueryUI-1.12.1/jquery-ui.min.js"></script>
    <script src="<%=fn_Param.CDNUrl %>plugin/jqueryUI-1.12.1/catcomplete/catcomplete.js"></script>
    <%-- Catcomplete Start --%>
    <script>
        /* Autocomplete 品號關聯 */
        $(".AC-ModelNo").catcomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=fn_Param.WebUrl %>Ajax_Data/GetData_Prod.ashx",
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

    <%-- ckeditor Start --%>
    <script src="//cdn.ckeditor.com/4.9.1/full/ckeditor.js"></script>
    <script>
        CKEDITOR.replace('MainContent_tb_Desc1', {
            customConfig: '<%=fn_Param.CDNUrl%>plugin/ckeditor/config_v4.9.1.js?v=0328'
        });
    </script>
    <%-- ckeditor End --%>
</asp:Content>

