<%@ Page Title="外驗查核表|清單" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="myProdCheck_Search" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <ol class="breadcrumb">
                        <li><a>外驗查檢表</a></li>
                        <li class="active">查檢表清單</li>
                    </ol>
                </div>
            </div>
        </div>

        <!-- Add Button -->
        <div style="position: relative;">
            <div class="fixed-action-btn" style="position: absolute; display: inline-block; top: -43px; right: 24px;">
                <a class="btn-floating btn-large red" href="<%=Application["WebUrl"] %>myProdCheck/Step1.aspx">
                    <i class="large material-icons">add</i>
                </a>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <div class="card-panel grey lighten-5">
            <div class="row">
                <div class="input-field col s3">
                    <asp:DropDownList ID="filter_Status" runat="server" CssClass="select-control">
                    </asp:DropDownList>
                </div>
                <div class="col s8">
                    <div class="input-field inline">
                        <i class="material-icons prefix">today</i>
                        <asp:TextBox ID="filter_sDate" runat="server" CssClass="datepicker"></asp:TextBox>
                        <label for="MainContent_filter_sDate">實際驗貨日-起始</label>
                    </div>
                    <div class="input-field inline">
                        <i class="material-icons prefix">today</i>
                        <asp:TextBox ID="filter_eDate" runat="server" CssClass="datepicker"></asp:TextBox>
                        <label for="MainContent_filter_eDate">實際驗貨日-訖止</label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col s4">
                    <asp:DropDownList ID="filter_CorpID" runat="server" CssClass="select-control">
                        <asp:ListItem Value="">所有公司別</asp:ListItem>
                        <asp:ListItem Value="1">台灣</asp:ListItem>
                        <asp:ListItem Value="2">上海</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col s4">
                    <asp:DropDownList ID="filter_Who" runat="server" CssClass="select-control">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col s8">
                    <asp:TextBox ID="filter_Keyword" runat="server" placeholder="關鍵字查詢:品號, 廠商, 單號" autocomplete="off"></asp:TextBox>
                </div>
                <div class="col s4 right-align">
                    <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                    <asp:LinkButton ID="lbtn_Reset" runat="server" class="btn waves-effect waves-light grey" ToolTip="重置條件" OnClick="lbtn_Reset_Click"><i class="material-icons">refresh</i></asp:LinkButton>

                    <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                </div>
            </div>
        </div>


        <div class="row">
            <div class="col s12">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                    <LayoutTemplate>
                        <table class="bordered striped">
                            <thead>
                                <tr>
                                    <th style="text-align: center; width: 100px;">預計驗貨日</th>
                                    <th>產品</th>
                                    <th>單號/廠商</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </table>
                        <div class="right-align">
                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <div class="card-panel <%#Eval("IsFinished").Equals("Y")?"grey lighten-1":"cyan darken-1" %> white-text center-align">
                                    <span class="flow-text"><strong><%#Eval("Est_CheckDay").ToString().ToDateString("MM/dd") %></strong></span><br />
                                    <%#Eval("Est_CheckDay").ToString().ToDateString("yyyy") %>
                                </div>
                            </td>
                            <td>
                                <p><span class="red-text <%#Eval("IsFinished").Equals("Y")?"text-lighten-2":"text-darken-1" %> flow-text"><strong><%#Eval("ModelNo") %></strong></span></p>
                                <p><%#Eval("ModelName") %></p>
                            </td>
                            <td>
                                <p><span class="flow-text"><strong><%#Eval("VendorName") %></strong></span> (<%#Eval("Vendor") %>)</p>
                                <p><strong class="green-text text-darken-2"><%#Eval("FirstID") %> - <%#Eval("SecondID") %></strong></p>

                            </td>
                            <td>
                                <p>
                                    <a class="waves-effect waves-light btn white grey-text text-darken-3" href="<%=Application["WebUrl"] %>myProdCheck/Edit.aspx?DataID=<%#Eval("Data_ID") %>"><i class="material-icons left">edit</i>詳細</a>
                                    <!-- Modal Trigger 1-->
                                    <a class="waves-effect waves-light btn teal modal-trigger" data-source="<%=Application["WebUrl"] %>myProdCheck/ViewDwReport.aspx?DataID=<%#Eval("Data_ID") %>&ModelNo=<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>" href="#remoteModal" title="下載查檢表"><i class="material-icons left">cloud_download</i>下載</a>

                                    <asp:PlaceHolder ID="ph_Approve" runat="server">
                                        <a class="waves-effect waves-light btn red darken-1" href="<%=Application["WebUrl"] %>myProdCheck/Approved.aspx?DataID=<%#Eval("Data_ID") %>"><i class="material-icons left">assignment_turned_in</i>核准</a>
                                    </asp:PlaceHolder>
                                </p>
                                <asp:PlaceHolder ID="ph_Edit" runat="server">
                                    <a class="waves-effect waves-light btn green" href="<%=Application["WebUrl"] %>myProdCheck/EditUpload.aspx?DataID=<%#Eval("Data_ID") %>" title="上傳報告"><i class="material-icons left">cloud_upload</i>上傳</a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="ph_ShowRpt" runat="server">
                                    <!-- Modal Trigger 2-->
                                    <a class="waves-effect waves-light btn teal lighten-1 modal-trigger" data-source="<%=Application["WebUrl"] %>myProdCheck/ViewUpReport.aspx?DataID=<%#Eval("Data_ID") %>&ModelNo=<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>" href="#remoteModal" title="查看報告"><i class="material-icons left">pageview</i>報告</a>
                                    <a class="waves-effect waves-light btn orange lighten-1 " href="<%=Application["WebUrl"] %>myProdCheck/Mail.aspx?DataID=<%#Eval("Data_ID") %>" title="發信"><i class="material-icons left">mail</i>發信</a>
                                </asp:PlaceHolder>

                                <asp:LinkButton ID="lbtn_SetLock" runat="server" CssClass="btn waves-effect waves-light grey darken-1" OnClientClick="return confirm('是否將此筆資料設為隱藏??')" CommandName="Lock"><i class="material-icons left">block</i>隱藏</asp:LinkButton>

                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <div>
                                    <asp:Label ID="lb_finish" runat="server" Text="已結案" CssClass="label label-default"></asp:Label>
                                    <span class="label label-default"><%#Eval("SeqNo") %></span>
                                    <span class="label label-success"><%#Eval("Corp_Name") %></span>
                                    <span class="label label-danger"><%#Eval("StatusName") %></span>
                                    <asp:Label ID="lb_isRel" runat="server" Text="有關聯單號" CssClass="label label-warning"></asp:Label>
                                    <asp:Label ID="lb_isLock" runat="server" Text="已設為隱藏" CssClass="label label-info"></asp:Label>
                                    <asp:Label ID="lb_isApproved" runat="server" Text="主管已核准" CssClass="label label-success"></asp:Label>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="section">
                            <div class="card-panel grey darken-1">
                                <i class="material-icons flow-text white-text">error_outline</i>
                                <span class="flow-text white-text">找不到資料, 快按「小紅點」新增資料吧!<i class="material-icons right">arrow_upward</i></span>
                            </div>
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>
    <!-- Body Content End -->


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
            /* Remote Modal Start */
            $(".modal-trigger").on("click", function () {
                var url = $(this).attr("data-source");

                $("#remoteModal .modal-content #myHtml").attr("src", url);
                //$.get(url, function (result) {
                //    $("#remoteModal .modal-content").html(result);
                //});
            });

            $('.showModal').modal();
            /* Remote Modal End */

            //載入選單
            $('.select-control').material_select();

            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 5, // Creates a dropdown of 15 years to control year
                format: 'yyyy-mm-dd',

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });

            //[搜尋][查詢鈕] - 觸發關鍵字快查
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });

            //[搜尋][Enter鍵] - 觸發關鍵字快查
            $("#MainContent_filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_KeySearch").trigger("click");

                    e.preventDefault();
                }
            });

        });
    </script>
</asp:Content>

