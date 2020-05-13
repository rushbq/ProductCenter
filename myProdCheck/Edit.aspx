<%@ Page Title="外驗查核表|編輯" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="myProdCheck_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Page_SearchUrl%>"><i class="material-icons left">arrow_back</i>返回列表</a></li>
                    </ul>
                    <span class="brand-logo center">編輯查檢表：<asp:Literal ID="lt_Title" runat="server"></asp:Literal></span>
                    <ul class="right">
                        <asp:PlaceHolder ID="ph_Lock" runat="server" Visible="false">
                            <li class="grey-text text-lighten-1"><i class="material-icons right">lock_outline</i>已結案</li>
                        </asp:PlaceHolder>
                        <!-- Dropdown Trigger -->
                        <li><a class="dropdown-button" href="#!" data-activates="tableNav">區塊選擇<i class="material-icons right">more_vert</i></a></li>
                    </ul>
                </div>

                <ul id="tableNav" class="dropdown-content">
                    <li><a href="#erp">採購單資料</a></li>
                    <li><a href="#base">查檢表欄位</a></li>
                    <li><a href="#prodOther">產品其他資料</a></li>
                    <li><a href="#report">檢驗報表</a></li>
                    <li><a href="#prodPic">產品圖片</a></li>
                    <li><a href="#dataRel">採購單關聯</a></li>
                </ul>
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
                <!-- ERP採購單資料 Start -->
                <div id="erp" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <div class="left">
                            <h5>ERP採購單資料</h5>
                        </div>
                        <div class="right">
                            <a href="<%=GetPDFUrl("inside","zh-TW",Param_ModelNo) %>" class="btn" target="_blank">SIP</a>
                            <a href="<%=GetViewUrl("inside","zh-TW",Param_ModelNo) %>" class="btn" target="_blank">SIP列印</a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s6">
                                <label>品號</label>
                                <p class="flow-text red-text text-darken-1">
                                    <b>
                                        <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                                    </b>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>廠商</label>
                                <p class="flow-text orange-text text-darken-1">
                                    <strong>
                                        <asp:Literal ID="lt_Vendor" runat="server"></asp:Literal></strong>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s6">
                                <label>單別/單號</label>
                                <p class="green-text text-darken-2">
                                    <asp:Literal ID="lt_ErpID" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>採購日期</label>
                                <p>
                                    <asp:Literal ID="lt_BuyDate" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s6">
                                <label>公司別</label>
                                <p>
                                    <asp:Literal ID="lt_Corp" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>採購數量</label>
                                <p>
                                    <asp:Literal ID="lt_BuyCnt" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- ERP採購單資料 End -->


                <!-- 查檢表欄位 Start -->
                <div id="base" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <div class="left">
                            <h5>查檢表欄位</h5>
                        </div>
                        <div class="right">
                            <asp:LinkButton ID="lbtn_Save" runat="server" CssClass="btn waves-effect waves-light blue" OnClick="lbtn_Save_Click">資料存檔</asp:LinkButton>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="input-field col s6">
                                <asp:DropDownList ID="ddl_Status" runat="server">
                                </asp:DropDownList>
                                <label for="MainContent_ddl_Status">狀態</label>
                            </div>
                            <div class="col s6">
                                <label>檢驗數量</label>
                                <p class="flow-text blue-text text-darken-1">
                                    <b>
                                        <asp:Literal ID="lt_CheckTotal" runat="server"></asp:Literal>
                                    </b>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="input-field col s6">
                                <i class="material-icons prefix">today</i>
                                <asp:TextBox ID="Date_Est" runat="server" CssClass="datepicker"></asp:TextBox>
                                <label for="MainContent_Date_Est">預計驗貨日</label>
                            </div>
                            <div class="input-field col s6">
                                <i class="material-icons prefix">today</i>
                                <asp:TextBox ID="Date_Act" runat="server" CssClass="datepicker"></asp:TextBox>
                                <label for="MainContent_Date_Act">實際驗貨日</label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="input-field col s12">
                                <asp:TextBox ID="Remark" runat="server" CssClass="materialize-textarea" MaxLength="200" length="200" TextMode="MultiLine"></asp:TextBox>
                                <label for="MainContent_Remark">備註</label>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 查檢表欄位 End -->


                <!-- 產品其他資料 Start -->
                <div id="prodOther" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>產品其他資料</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12">
                                <label>替代品號</label>
                                <ul class="collection">
                                    <li class="collection-item">&nbsp;
                                            <asp:Literal ID="lt_Substitute_Model_No_TW" runat="server"></asp:Literal>
                                        <span class="right">(TW)</span>
                                    </li>
                                    <li class="collection-item">&nbsp;
                                            <asp:Literal ID="lt_Substitute_Model_No_SH" runat="server"></asp:Literal>
                                        <span class="right">(SH)</span>
                                    </li>
                                    <li class="collection-item">&nbsp;
                                            <asp:Literal ID="lt_Substitute_Model_No_SZ" runat="server"></asp:Literal>
                                        <span class="right">(SZ)</span>
                                    </li>
                                </ul>

                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <label>產品備註</label>
                                <p>
                                    <asp:Literal ID="lt_Pub_Notes" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 產品其他資料 End -->


                <!-- 檢驗報表 Start -->
                <div id="report" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <div class="left">
                            <h5>檢驗報表</h5>
                        </div>
                        <div class="right">
                            <asp:PlaceHolder ID="ph_UploadBtn" runat="server">
                                <a href="<%=Application["WebUrl"] %>myProdCheck/EditUpload.aspx?DataID=<%=Req_DataID %>" class="btn waves-effect waves-light green">上傳報告</a>
                            </asp:PlaceHolder>
                        </div>
                        <div class="clearfix"></div>
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
                    </div>
                </div>
                <!-- 檢驗報表 End -->


                <!-- 產品圖片 Start -->
                <div id="prodPic" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>產品圖片</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <asp:Literal ID="lt_PhotoContainer" runat="server"></asp:Literal>
                    </div>
                </div>
                <!-- 產品圖片 End -->


                <!-- 採購單關聯 Start -->
                <div id="dataRel" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <div class="left">
                            <h5>採購單關聯</h5>
                        </div>
                        <div class="right">
                            <asp:PlaceHolder ID="ph_RelBtn" runat="server">
                                <a href="<%=Application["WebUrl"] %>myProdCheck/EditRel.aspx?DataID=<%=Req_DataID %>" class="btn waves-effect waves-light green">新增關聯</a>
                            </asp:PlaceHolder>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="card-content grey lighten-5">
                        <asp:ListView ID="lv_RelData" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_RelData_ItemCommand">
                            <LayoutTemplate>
                                <table class="bordered striped">
                                    <thead>
                                        <tr>
                                            <th>單別</th>
                                            <th>單號</th>
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
                                        <%#Eval("FirstID") %>
                                    </td>
                                    <td>
                                        <%#Eval("SecondID") %>
                                    </td>
                                    <td class="center-align">
                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" ValidationGroup="DataRel" OnClientClick="return confirm('確定要刪除嗎?')"><i class="material-icons">clear</i></asp:LinkButton>

                                        <asp:HiddenField ID="hf_Fid" runat="server" Value='<%#Eval("FirstID") %>' />
                                        <asp:HiddenField ID="hf_Sid" runat="server" Value='<%#Eval("SecondID") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="center-align grey-text text-lighten-1">
                                    <i class="material-icons flow-text">info_outline</i>
                                    <span class="flow-text">尚未設定關聯</span>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
                <!-- 採購單關聯 End -->


                <!-- 維護資訊 Start -->
                <div class="card grey">
                    <div class="card-content white-text">
                        <h5>維護資訊</h5>
                    </div>
                    <div class="card-content grey lighten-5">
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
                    </div>
                </div>
                <!-- 維護資訊 End -->
            </asp:PlaceHolder>

            <!-- // 浮動按鈕 // -->
            <div class="fixed-action-btn toolbar">
                <a class="btn-floating btn-large red">
                    <i class="large material-icons">menu</i>
                </a>
                <ul>
                    <li class="waves-effect waves-light teal lighten-1">
                        <a href="#remoteModal" class="waves-effect waves-light btn teal lighten-1 modal-trigger" data-source="<%=Application["WebUrl"] %>myProdCheck/ViewDwReport.aspx?DataID=<%=Req_DataID %>&ModelNo=<%=Server.UrlEncode(this.lt_ModelNo.Text)%>"><i class="material-icons">cloud_download</i>&nbsp;下載查檢表</a>
                    </li>
                    <li class="waves-effect waves-light grey darken-1">
                        <asp:LinkButton ID="lbtn_Lock" runat="server" OnClick="lbtn_Lock_Click" OnClientClick="return confirm('是否確定隱藏??')"><i class="material-icons">block</i>&nbsp;設為隱藏</asp:LinkButton>
                    </li>
                    <asp:PlaceHolder ID="ph_JobBtns" runat="server">
                        <li class="waves-effect waves-light">
                            <asp:LinkButton ID="lbtn_Finish" runat="server" OnClick="lbtn_Finish_Click" OnClientClick="return confirm('是否確定結案??\n結案後資料將無法修改，請確認!')"><i class="material-icons">offline_pin</i>&nbsp;設為結案</asp:LinkButton>
                        </li>
                    </asp:PlaceHolder>
                </ul>
            </div>
        </div>
    </div>

    <!-- Body End -->

    <!-- Modal Structure Start -->
    <div id="remoteModal" class="showModal modal modal-fixed-footer">
        <div class="modal-content">
            <iframe id="myHtml" style="width: 100%; height: 100%; border: none;"></iframe>
        </div>
        <div class="modal-footer">
            <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat">Close</a>
        </div>
    </div>

    <!-- Modal Structure End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script src="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 2, // Creates a dropdown of 15 years to control year
                format: 'yyyy-mm-dd'
            });

            //載入選單
            $('select').material_select();

            //scrollSpy
            $('.scrollspy').scrollSpy();

            /* Remote Modal Start */
            $(".modal-trigger").on("click", function () {
                var url = $(this).attr("data-source");

                $("#remoteModal .modal-content #myHtml").attr("src", url);

                //$.get(url, function (result) {
                //    var myResult = result
                //    $("#remoteModal .modal-content").html(result);
                //});
            });

            $('.showModal').modal();
            /* Remote Modal End */


            //產品圖片Tab, Click Event
            $(".tab > a").on("click", function () {
                //取得目標物件
                var targetID = $(this).attr("target-id");

                //取得參數
                var dataID = $(this).attr("data-id");
                var modelNo = '<%=Param_ModelNo%>';

                //取得目標容器
                var container = $("#" + targetID);

                //填入Ajax Html
                var url = "<%=Application["WebUrl"]%>Ajax_Data/GetHTML_ProdPic.ashx?modelNo=" + modelNo + "&clsID=" + dataID;

                $(container).load(url, function (response, status, xhr) {
                    if (status == "error") {
                        var msg = "Sorry but there was an error: ";
                        alert(msg + xhr.status + " " + xhr.statusText);
                    } else {
                        $('.materialboxed').materialbox();
                    }
                });

            });

        });
    </script>
</asp:Content>

