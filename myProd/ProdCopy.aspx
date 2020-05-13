<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ProdCopy.aspx.cs" Inherits="myProd_ProdCopy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">產品複製功能</h5>
                    <ol class="breadcrumb">
                        <li><a href="#!">產品資料庫</a></li>
                        <li class="active">其他設定</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="row">
        <div class="col s12">
            <asp:PlaceHolder ID="ph_Data" runat="server">
                <!-- Base Start -->
                <div class="card grey">
                    <div class="card-content white-text">
                        <h5>功能設定</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <!-- Block Content Start -->
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-blue">
                                    <h6>來源品號&nbsp;(資料複製來源)</h6>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <asp:TextBox ID="src_ModelNo" runat="server" data-target="MainContent_hf_src_ModelNo" CssClass="AC-ModelNo"></asp:TextBox>
                                            <label for="src_ModelNo">來源品號</label>
                                            <asp:HiddenField ID="hf_src_ModelNo" runat="server" />
                                            <div class="grey-text text-darken-2">(輸入關鍵字,出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>)</div>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-green">
                                    <h6>目標品號&nbsp;(將會複製來源品號的資料)</h6>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <asp:TextBox ID="tar_ModelNo" runat="server" data-target="MainContent_hf_tar_ModelNo" CssClass="AC-ModelNo"></asp:TextBox>
                                            <label for="tar_ModelNo">目標品號</label>
                                            <asp:HiddenField ID="hf_tar_ModelNo" runat="server" />
                                            <div class="grey-text text-darken-2">(輸入關鍵字,出現選單後, <u class="pink-text text-lighten-2">選擇你要的項目</u>)</div>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-blue">
                                    <h6>複製選項&nbsp;(可複選)</h6>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <asp:CheckBoxList ID="cbl_Option" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="1" Selected="True">基本資料</asp:ListItem>
                                                <asp:ListItem Value="2" Selected="True">規格明細</asp:ListItem>
                                                <asp:ListItem Value="3" Selected="True">PDF匯出設定(by Item)</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>
                        <div class="card-panel red darken-1 white-text">
                            <p>
                                <i class="material-icons right">announcement</i>
                                按下「開始複製」後，<u>目標品號</u> <span class="flow-text">相關欄位的資料將會被清空</span>(依勾選的項目)，並複製<u>來源品號</u>的資料，清空後的資料 <span class="flow-text">無法救回</span>，請確認無誤後再按。
                            </p>
                        </div>
                        <div class="row">
                            <div class="col s12 right-align">
                                <asp:LinkButton ID="lbtn_Copy" runat="server" CssClass="btn waves-effect waves-light blue" OnClick="lbtn_Copy_Click" OnClientClick="return confirm('確定要開始複製了嗎?')">開始複製</asp:LinkButton>
                            </div>
                        </div>
                        <!-- Block Content End -->
                    </div>
                </div>
                <!-- Base End -->

                <!-- 異動記錄 Start -->
                <div class="card grey">
                    <div class="card-content white-text">
                        <h5>異動記錄 (最近10筆)</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <asp:ListView ID="lv_Log" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table class="bordered striped">
                                    <thead>
                                        <tr>
                                            <th>事件</th>
                                            <th>異動人</th>
                                            <th>異動時間</th>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <asp:PlaceHolder ID="ph_Items" runat="server" />

                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%#Eval("LogDesc") %></td>
                                    <td><%#Eval("LogWho") %></td>
                                    <td><%#Eval("LogTime").ToString() %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </div>
                <!-- 異動記錄 End -->
            </asp:PlaceHolder>

        </div>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.css" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.js"></script>
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.js"></script>
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
</asp:Content>

