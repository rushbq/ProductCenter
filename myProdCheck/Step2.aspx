<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Step2.aspx.cs" Inherits="myProdCheck_Step2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>myProdCheck/Step1.aspx"><i class="material-icons left">arrow_back</i>回上一步</a></li>
                    </ul>
                    <span class="brand-logo center">建立查檢表</span>
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
                <div class="card grey">
                    <div class="card-content white-text">
                        <h5>Step2 - 查詢採購單</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12 grey lighten-4">
                                <i class="material-icons">flag</i>&nbsp;
                                目前的資料庫別為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_CorpName" runat="server"></asp:Literal></b></span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-blue">
                                    <h6>1. 依採購單號查詢</h6>
                                    <div class="row">
                                        <div class="input-field col s3">
                                            <label>
                                                採購單別&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_FirstID" runat="server" ErrorMessage="請填寫「採購單別」" ControlToValidate="tb_FirstID" CssClass="red-text" Display="Dynamic" ValidationGroup="s1"></asp:RequiredFieldValidator></label>
                                            <asp:TextBox ID="tb_FirstID" runat="server" MaxLength="8" placeholder="輸入完整的採購單別"></asp:TextBox>
                                        </div>
                                        <div class="input-field col s6">
                                            <label>
                                                採購單號&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_SecondID" runat="server" ErrorMessage="請填寫「採購單號」" ControlToValidate="tb_SecondID" CssClass="red-text" Display="Dynamic" ValidationGroup="s1"></asp:RequiredFieldValidator></label>
                                            <asp:TextBox ID="tb_SecondID" runat="server" MaxLength="20" placeholder="輸入完整的採購單號"></asp:TextBox>
                                        </div>
                                        <div class="col s3">
                                            <asp:LinkButton ID="lbtn_Search1" runat="server" ValidationGroup="s1" CssClass="btn waves-effect waves-light blue darken-1" OnClick="lbtn_Search1_Click">查詢</asp:LinkButton>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-green">
                                    <h6>2. 依廠商查詢</h6>
                                    <div class="row">
                                        <div class="col s2">
                                            <label>採購年度</label>
                                            <asp:DropDownList ID="ddl_Year" runat="server">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="input-field col s4">
                                            <label>
                                                廠商&nbsp;<asp:RequiredFieldValidator ID="rfv_Cust_ID_Val" runat="server" ErrorMessage="請選擇正確的「廠商」" ControlToValidate="Cust_ID_Val" CssClass="red-text" Display="Dynamic" ValidationGroup="s2"></asp:RequiredFieldValidator></label>

                                            <asp:TextBox ID="Cust_ID_Val" runat="server" Style="display: none"></asp:TextBox>
                                            <asp:TextBox ID="Cust_Name" runat="server" CssClass="AC-Customer" data-target="MainContent_Cust_ID_Val" placeholder="輸入代號或名稱關鍵字"></asp:TextBox>
                                            <div class="grey-text text-darken-2">(輸入關鍵字, 出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>)</div>

                                        </div>
                                        <div class="input-field col s3">
                                            <label>
                                                品號&nbsp;<asp:RequiredFieldValidator ID="rfv_ModelNo_Val" runat="server" ErrorMessage="請選擇正確的「品號」" ControlToValidate="ModelNo_Val" CssClass="red-text" Display="Dynamic" ValidationGroup="s2"></asp:RequiredFieldValidator></label>

                                            <asp:TextBox ID="ModelNo_Val" runat="server" Style="display: none"></asp:TextBox>
                                            <asp:TextBox ID="ModelNo" runat="server" CssClass="AC-ModelNo" data-target="MainContent_ModelNo_Val" placeholder="輸入品號或品名關鍵字"></asp:TextBox>
                                            <div class="grey-text text-darken-2">(輸入關鍵字,出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>)</div>
                                        </div>

                                        <div class="col s3">
                                            <asp:LinkButton ID="lbtn_Search2" runat="server" ValidationGroup="s2" CssClass="btn waves-effect waves-light green darken-1" OnClick="lbtn_Search2_Click">查詢</asp:LinkButton>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>

                        <div class="section row">
                            <div class="col s12">
                                <label>注意事項</label>
                                <div>
                                    <ul class="collection">
                                        <li class="collection-item"><i class="material-icons left">info</i>系統提供兩種方式查詢採購單</li>
                                        <li class="collection-item"><i class="material-icons left">info</i>兩種查詢方式的欄位皆為必填</li>
                                    </ul>
                                </div>
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
    <script>
        $(function () {
            //載入選單
            $('select').material_select();
        });
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
    <%-- Catcomplete End --%>
</asp:Content>

