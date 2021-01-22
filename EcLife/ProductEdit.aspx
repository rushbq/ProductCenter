<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ProductEdit.aspx.cs" Inherits="EcLife_ProductEdit" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>Product/Prod_Edit.aspx?Model_No=<%=Req_DataID %>"><i class="material-icons left">arrow_back</i>Back</a></li>
                    </ul>
                    <span class="brand-logo center">良興商品維護</span>
                    <ul class="right">
                        <!-- Dropdown Trigger -->
                        <li><a class="dropdown-button" href="#!" data-activates="tableNav">區塊選擇<i class="material-icons right">more_vert</i></a></li>
                    </ul>
                </div>
                <ul id="tableNav" class="dropdown-content">
                    <li><a href="#base">商品資料</a></li>
                    <li><a href="#setTags">關鍵字設定</a></li>
                    <li><a href="#pic">商品圖異動</a></li>
                    <li><a href="#stock">庫存異動</a></li>
                    <li><a href="#log">異動記錄</a></li>
                </ul>
            </div>
        </nav>
    </div>
    <!-- Top Nav End -->
    <!-- Body Start -->
    <div class="row">
        <div class="col s12">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server">
                <div class="card-panel red darken-1 white-text">
                    <h4><i class="material-icons right">error_outline</i>糟糕了!!...發生了一點小問題</h4>
                    <p>若持續看到此訊息, 請回報 <strong class="flow-text">詳細操作狀況</strong>, 以便抓蟲<i class="material-icons">bug_report</i>。</p>
                    <p>
                        <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                    </p>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ph_Data" runat="server">
                <!-- 商品資料 Start -->
                <div id="base" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <div class="left">
                            <h5>商品資料</h5>
                        </div>
                        <div class="right">
                            <asp:LinkButton ID="lbtn_Save" runat="server" CssClass="btn waves-effect waves-light blue" ValidationGroup="base" OnClick="lbtn_Save_Click">資料存檔</asp:LinkButton>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" HeaderText="尚有資料未填寫" ValidationGroup="base" />
                        </div>
                        <div class="clearfix"></div>
                    </div>

                    <!-- // Messages Start // -->
                    <asp:PlaceHolder ID="ph_Msg1" runat="server">
                        <div class="card-content yellow darken-4 white-text">
                            <h5>請先將「必填欄位」的資料填齊</h5>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_eclife_Msg1" runat="server">
                        <div class="card-content cyan darken-1 white-text">
                            <div class="left">
                                <h5>商品資料未提報, 請先按下「同步」鈕</h5>
                            </div>
                            <div class="right">
                                <asp:LinkButton ID="lbtn_ProdNew" runat="server" CssClass="btn waves-effect waves-light green" OnClick="lbtn_ProdNew_Click">同步-商品提報</asp:LinkButton>
                            </div>
                            <div class="clearfix"></div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_eclife_Msg2" runat="server">
                        <div class="card-content cyan darken-1 white-text">
                            <div class="left">
                                <h5>資料異動後未同步, 請按下「同步」鈕</h5>
                            </div>
                            <div class="right">
                                <asp:LinkButton ID="lbtn_ProdUpdate" runat="server" CssClass="btn waves-effect waves-light green" OnClick="lbtn_ProdUpdate_Click">同步-商品異動</asp:LinkButton>
                            </div>
                            <div class="clearfix"></div>
                        </div>
                    </asp:PlaceHolder>
                    <!-- // Messages End // -->

                    <div class="card-content grey lighten-5">
                        <!-- Block Content Start -->
                        <div class="row">
                            <div class="col s6">
                                <label>品號 (廠商貨號)</label>
                                <div class="red-text text-darken-2 flow-text">
                                    <b><%=Req_DataID %></b>
                                </div>
                            </div>
                            <div class="col s6">
                                <label>商品狀態</label>
                                <div class="green-text text-darken-2 flow-text">
                                    <strong>
                                        <asp:Literal ID="lt_ProdStatus" runat="server">審核中</asp:Literal>
                                        <asp:HiddenField ID="hf_ProdStatus" runat="server" />
                                    </strong>
                                </div>
                            </div>
                        </div>

                        <!-- // 分類設定 // -->
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-blue">
                                    <h6>分類設定</h6>
                                    <div class="row">
                                        <div class="input-field col s4">
                                            <asp:DropDownList ID="ddl_HouseNo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_HouseNo_SelectedIndexChanged">
                                                <asp:ListItem Value="">請選擇大分類</asp:ListItem>
                                            </asp:DropDownList>
                                            <label>
                                                大分類 *&nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_HouseNo" runat="server" ErrorMessage="請填寫「大分類」" ControlToValidate="ddl_HouseNo" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator></label>
                                            <span class="grey-text">已設選項:</span>
                                            <b>
                                                <asp:Label ID="lb_Cls1" runat="server" CssClass="orange-text text-darken-3"></asp:Label></b>
                                        </div>
                                        <div class="input-field col s4">
                                            <asp:DropDownList ID="ddl_LargeNo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_LargeNo_SelectedIndexChanged">
                                                <asp:ListItem Value="">請先選擇大分類</asp:ListItem>
                                            </asp:DropDownList>
                                            <label>
                                                中分類 *&nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_LargeNo" runat="server" ErrorMessage="請填寫「中分類」" ControlToValidate="ddl_LargeNo" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator></label>
                                            <span class="grey-text">已設選項:</span>
                                            <b>
                                                <asp:Label ID="lb_Cls2" runat="server" CssClass="orange-text text-darken-3"></asp:Label></b>
                                        </div>
                                        <div class="input-field col s4">
                                            <asp:DropDownList ID="ddl_MediumNo" runat="server">
                                                <asp:ListItem Value="">請先選擇中分類</asp:ListItem>
                                            </asp:DropDownList>
                                            <label>
                                                小分類 *&nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_MediumNo" runat="server" ErrorMessage="請填寫「小分類」" ControlToValidate="ddl_MediumNo" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator></label>
                                            <span class="grey-text">已設選項:</span>
                                            <b>
                                                <asp:Label ID="lb_Cls3" runat="server" CssClass="orange-text text-darken-3"></asp:Label></b>
                                        </div>
                                    </div>
                                    <small class="grey-text text-darken-2">分類不正確嗎?&nbsp;<asp:LinkButton ID="lbtn_ClsReset" runat="server" OnClick="lbtn_ClsReset_Click">按我重新取得</asp:LinkButton>
                                    </small>
                                </blockquote>
                            </div>
                        </div>

                        <!-- // 基本設定 // -->
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-green">
                                    <h6>基本設定</h6>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <label>
                                                良興品名 *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_ProdName" runat="server" ErrorMessage="請填寫「良興品名」" ControlToValidate="tb_ProductName" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator></label>
                                            <asp:TextBox ID="tb_ProductName" runat="server" MaxLength="50" length="50" placeholder="產品名稱"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <label>
                                                簡稱 (消費者發票品名) *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_ShortName" runat="server" ErrorMessage="請填寫「簡稱」" ControlToValidate="tb_SPName" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator></label>
                                            <asp:TextBox ID="tb_SPName" runat="server" MaxLength="20" length="20" placeholder="輸入簡稱(英文20字, 中文10字)"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s6">
                                            <label>條碼</label>
                                            <asp:TextBox ID="tb_Barcode" runat="server" MaxLength="20" length="20" disabled placehoder="產品中心的條碼"></asp:TextBox>
                                        </div>
                                        <div class="input-field col s6">
                                            <label>推薦說明</label>
                                            <asp:TextBox ID="tb_ProductMemo" runat="server" MaxLength="12" length="12" placeholder="產品的附加說明,最多12字"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s6">
                                            <label>
                                                一次可購量 *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_MaxBuy" runat="server" ErrorMessage="請填寫「一次可購量」" ControlToValidate="tb_MaxBuy" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cv_tb_MaxBuy" runat="server" ControlToValidate="tb_MaxBuy" ErrorMessage="請輸入數字" Operator="DataTypeCheck" Type="Integer" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:CompareValidator>
                                            </label>
                                            <asp:TextBox ID="tb_MaxBuy" runat="server">1</asp:TextBox>
                                        </div>
                                        <div class="input-field col s6">
                                            <label>
                                                當日總訂量 *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_MaxBuy_Tot" runat="server" ErrorMessage="請填寫「當日總訂量」" ControlToValidate="tb_MaxBuy_Tot" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="cv_tb_MaxBuy_Tot" runat="server" ControlToValidate="tb_MaxBuy_Tot" ErrorMessage="請輸入數字" Operator="DataTypeCheck" Type="Integer" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:CompareValidator>
                                            </label>
                                            <asp:TextBox ID="tb_MaxBuy_Tot" runat="server">1</asp:TextBox>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>

                        <!-- // 價格顯示 // -->
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-blue">
                                    <h6>價格&nbsp;<small>(僅能在新增時同步)</small></h6>
                                    <div class="row">
                                        <div class="col s6">
                                            <label>
                                                市價&nbsp;(ERP:台灣NTD網路價)<%--MB055--%>
                                            </label>
                                            <p>
                                                <asp:Literal ID="lt_Price_Sale" runat="server"></asp:Literal>
                                            </p>
                                        </div>
                                        <div class="col s6">
                                            <label>
                                                網路價&nbsp;(ERP:台灣NTD網路價)<%--MB055--%>
                                            </label>
                                            <p>
                                                <asp:Literal ID="lt_Price_Spical" runat="server"></asp:Literal>
                                            </p>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>

                        <!-- // 商品詳細說明 // -->
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-green">
                                    <h6>商品詳細說明 (刮號內為良興欄位)</h6>
                                    <div class="row">
                                        <div class="col s4">
                                            <label>產品簡述 (商品特色)</label>
                                            <div>
                                                <a href="#modal1" class="modal-trigger waves-effect waves-light btn light-blue darken-1"><i class="material-icons">pageview</i></a>
                                                <a href="<%=Application["WebUrl"] %>Product/Prod_InfoEdit.aspx?Lang=zh-TW&Model_No=<%=Req_DataID%>" class="waves-effect waves-light btn amber darken-1"><i class="material-icons">mode_edit</i></a>
                                            </div>

                                            <!-- Modal Structure -->
                                            <div id="modal1" class="modal modal-fixed-footer">
                                                <div class="modal-content">
                                                    <h4>產品簡述 (商品特色)</h4>
                                                    <div>
                                                        <asp:Literal ID="lt_Desc_Classics" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="modal-footer">
                                                    <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat ">Close</a>
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col s4">
                                            <label>特性 (商品介紹)</label>
                                            <div>
                                                <a href="#modal2" class="modal-trigger waves-effect waves-light btn light-blue darken-1"><i class="material-icons">pageview</i></a>
                                                <a href="<%=Application["WebUrl"] %>Product/Prod_InfoEdit.aspx?Lang=zh-TW&Model_No=<%=Req_DataID%>" class="waves-effect waves-light btn amber darken-1"><i class="material-icons">mode_edit</i></a>
                                            </div>

                                            <!-- Modal Structure -->
                                            <div id="modal2" class="modal modal-fixed-footer">
                                                <div class="modal-content">
                                                    <h4>特性 (商品介紹)</h4>
                                                    <div>
                                                        <asp:Literal ID="lt_Desc_Feature" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="modal-footer">
                                                    <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat ">Close</a>
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col s4">
                                            <label>規格 (商品規格)</label>
                                            <div>
                                                <a href="#modal3" class="modal-trigger waves-effect waves-light btn light-blue darken-1"><i class="material-icons">pageview</i></a>
                                                <a href="<%=Application["WebUrl"] %>Product/Prod_InfoEdit.aspx?Lang=zh-TW&Model_No=<%=Req_DataID%>" class="waves-effect waves-light btn amber darken-1"><i class="material-icons">mode_edit</i></a>
                                            </div>

                                            <!-- Modal Structure -->
                                            <div id="modal3" class="modal modal-fixed-footer">
                                                <div class="modal-content">
                                                    <h4>規格 (商品規格)</h4>
                                                    <div>
                                                        <asp:Literal ID="lt_Desc_Standards" runat="server"></asp:Literal>
                                                    </div>
                                                </div>
                                                <div class="modal-footer">
                                                    <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat ">Close</a>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col s12">
                                            <label>包裝附件</label>
                                            <p>
                                                <asp:Literal ID="lt_Desc_Introduce" runat="server"></asp:Literal>
                                            </p>
                                            <!-- from ERP INVMB.MB209 -->
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <label for="MainContent_tb_Desc_Services">保固說明 *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Desc_Services" runat="server" ErrorMessage="請填寫「保固說明」" ControlToValidate="tb_Desc_Services" CssClass="red-text" Display="Dynamic" ValidationGroup="base"></asp:RequiredFieldValidator></label>
                                            <asp:TextBox ID="tb_Desc_Services" runat="server" CssClass="materialize-textarea" TextMode="MultiLine" MaxLength="100" length="100"></asp:TextBox>
                                        </div>
                                    </div>
                                </blockquote>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col s12 right-align">
                                <asp:LinkButton ID="lbtn_SaveBottom" runat="server" CssClass="btn waves-effect waves-light blue" ValidationGroup="base" OnClick="lbtn_Save_Click">資料存檔</asp:LinkButton>
                            </div>
                        </div>
                        <!-- Block Content End -->
                    </div>
                </div>
                <!-- 商品資料 End -->

                <!-- 關鍵字設定 Start -->
                <div id="setTags" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>關鍵字設定 <small>(設定完成後, 必須重新同步)</small><small class="cyan-text text-accent-2">&nbsp;** 請確認官網商品已新增 **</small></h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="input-field col s8">
                                <asp:TextBox ID="Rel_Tag" runat="server" ValidationGroup="AddTag"></asp:TextBox>

                                <asp:TextBox ID="tb_TagName" runat="server" Style="display: none"></asp:TextBox>
                                <asp:TextBox ID="tb_TagID" runat="server" Style="display: none">0</asp:TextBox>

                                <label for="Rel_Tag">
                                    關鍵字<asp:RequiredFieldValidator ID="rfv_Rel_Tag" runat="server" ErrorMessage="請填寫「關鍵字」" ControlToValidate="Rel_Tag" Display="Dynamic" ValidationGroup="AddTag" CssClass="red-text"></asp:RequiredFieldValidator>
                                </label>
                                <div class="grey-text text-darken-2">(輸入關鍵字, 出現的選單為<u class="pink-text text-lighten-2">曾經填寫過的項目</u>, 新的關鍵字則<u class="pink-text text-lighten-2">直接填寫</u>, 完畢後請按下<span class="flow-text"><b class="orange-text text-darken-3">加入</b></span>)</div>
                            </div>
                            <div class="input-field col s4">
                                <asp:LinkButton ID="lbtn_AddTag" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="AddTag" OnClick="lbtn_AddTag_Click">加入</asp:LinkButton>
                            </div>
                        </div>
                        <div>
                            <asp:ListView ID="lv_Tags" runat="server" ItemPlaceholderID="ph_Items" GroupPlaceholderID="ph_Group" GroupItemCount="2" OnItemCommand="lv_Tags_ItemCommand">
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
                                        <%#Eval("TagName") %>
                                    </td>
                                    <td style="width: 15%" class="center-align">
                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('確定刪除?')" ValidationGroup="ListProd"><i class="material-icons">clear</i></asp:LinkButton>
                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("TagID") %>' />
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td style="width: 35%">&nbsp;</td>
                                    <td style="width: 15%">&nbsp;</td>
                                </EmptyItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="center-align grey-text text-lighten-1">
                                        <i class="material-icons flow-text">info_outline</i>
                                        <span class="flow-text">尚未加入關鍵字</span>
                                    </div>

                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
                <!-- 關鍵字設定 End -->

                <!-- 商品圖異動 Start -->
                <div id="pic" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>商品圖異動 <small>(選擇圖片或自行填入網址)</small></h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="input-field col m7 s6">
                                <label>
                                    商品圖Url路徑 (500*500) &nbsp;<asp:RequiredFieldValidator ID="rfv_tb_PicUrl" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="tb_PicUrl" CssClass="red-text" Display="Dynamic" ValidationGroup="Pic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rev_tb_PicUrl" runat="server" ErrorMessage="網址格式輸入不正確" ControlToValidate="tb_PicUrl" CssClass="red-text" Display="Dynamic" ValidationGroup="Pic" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"></asp:RegularExpressionValidator>
                                </label>
                                <asp:TextBox ID="tb_PicUrl" runat="server" placeholder="選擇圖片或自行填入網址"></asp:TextBox>
                            </div>
                            <div class="input-field col m5 s6">
                                <a href="#modal-Img" class="modal-trigger waves-effect waves-light btn light-blue darken-1"><i class="material-icons">pageview</i></a>

                                <asp:LinkButton ID="lbtn_ProdImg" runat="server" CssClass="btn waves-effect waves-light green" OnClick="lbtn_ProdImg_Click">同步-商品圖異動</asp:LinkButton>
                            </div>
                        </div>
                        <!-- Modal Structure -->
                        <div id="modal-Img" class="modal modal-fixed-footer">
                            <div class="modal-content">
                                <h4>選擇圖片</h4>

                                <asp:Literal ID="lt_Pics" runat="server"></asp:Literal>

                            </div>
                            <div class="modal-footer">
                                <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat ">取消</a>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 商品圖異動 End -->

                <!-- 庫存異動 Start -->
                <div id="stock" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>庫存異動</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="input-field col m7 s6">
                                <label>
                                    庫存量異動 *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Stock" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="tb_Stock" CssClass="red-text" Display="Dynamic" ValidationGroup="stock"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="cv_tb_Stock" runat="server" ControlToValidate="tb_Stock" ErrorMessage="請輸入數字" Operator="DataTypeCheck" Type="Integer" CssClass="red-text" Display="Dynamic" ValidationGroup="stock"></asp:CompareValidator>
                                </label>
                                <asp:TextBox ID="tb_Stock" runat="server" placeholder="請填入數字"></asp:TextBox>
                            </div>
                            <div class="input-field col m5 s6">
                                <asp:LinkButton ID="lbtn_ProdStock" runat="server" CssClass="btn waves-effect waves-light green" OnClick="lbtn_ProdStock_Click">同步-庫存量異動</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 庫存異動 End -->

                <!-- 異動記錄 Start -->
                <div id="log" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>異動記錄</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12">
                                <ul class="tabs">
                                    <li class="tab col s3"><a class="active" href="#log_base">商品資料</a></li>
                                    <li class="tab col s3"><a href="#log_pic">商品圖異動</a></li>
                                    <li class="tab col s3"><a href="#log_stock">庫存異動</a></li>
                                    <li class="tab col s3"><a href="#log_api">API同步</a></li>
                                </ul>
                            </div>
                            <div id="log_base" class="col s12">
                                <table class="centered striped">
                                    <thead>
                                        <tr>
                                            <th>事件</th>
                                            <th>動作</th>
                                            <th>異動人</th>
                                            <th>異動時間</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Literal ID="lt_logBase" runat="server"></asp:Literal>
                                    </tbody>
                                </table>
                            </div>
                            <div id="log_pic" class="col s12">
                                <table class="centered striped">
                                    <thead>
                                        <tr>
                                            <th>事件</th>
                                            <th>動作</th>
                                            <th>異動人</th>
                                            <th>異動時間</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Literal ID="lt_logPic" runat="server"></asp:Literal>
                                    </tbody>
                                </table>
                            </div>
                            <div id="log_stock" class="col s12">
                                <table class="centered striped">
                                    <thead>
                                        <tr>
                                            <th>事件</th>
                                            <th>動作</th>
                                            <th>異動人</th>
                                            <th>異動時間</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Literal ID="lt_logStock" runat="server"></asp:Literal>
                                    </tbody>
                                </table>
                            </div>
                            <div id="log_api" class="col s12">
                                <table class="centered striped">
                                    <thead>
                                        <tr>
                                            <th>事件</th>
                                            <th>動作</th>
                                            <th>異動人</th>
                                            <th>異動時間</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Literal ID="lt_logApi" runat="server"></asp:Literal>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                    </div>
                </div>
                <!-- 異動記錄 End -->
            </asp:PlaceHolder>

        </div>
    </div>

    <!-- Body End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">

    <script>
        $(document).on('focus', ':input', function () {
            $(this).attr('autocomplete', 'off');
        });

        (function ($) {
            $(function () {
                //載入選單
                $('select').material_select();

                //scrollSpy
                $('.scrollspy').scrollSpy();


                //Modal init
                $('.modal').modal();

            }); // end of document ready
        })(jQuery); // end of jQuery name space


        //傳送選擇的圖片
        function sendPicUrl(url) {
            $("#MainContent_tb_PicUrl").val(url);
            $('#modal-Img').modal('close');
        }
    </script>
    <%-- Autocompelete Tag Start --%>
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/jquery-ui.min.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/jquery-ui.min.js"></script>
    <script>
        $("#MainContent_Rel_Tag").autocomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ProdTags.aspx",
                    data: {
                        q: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    value: item.label,
                                    id: item.id
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
                $("#MainContent_tb_TagName").val(ui.item.value);
                $("#MainContent_tb_TagID").val(ui.item.id);

                event.preventDefault();
            }
        });
    </script>
    <%-- Autocompelete Tag End --%>
</asp:Content>
