<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="myPic_Maintain.aspx.cs" Inherits="ProdPic_myPic_Maintain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=LastUrl%>"><i class="material-icons left">arrow_back</i>Back</a></li>
                    </ul>
                    <span class="brand-logo center"><%=Param_ModelNo %>&nbsp;&nbsp; 圖檔與關聯維護</span>
                    <ul class="right">
                        <!-- Dropdown Trigger -->
                        <li><a class="dropdown-button" href="#!" data-activates="tableNav">區塊選擇<i class="material-icons right">more_vert</i></a></li>
                    </ul>
                </div>
                <ul id="tableNav" class="dropdown-content">
                    <li><a href="#base">檔案維護</a></li>
                    <li><a href="#rel">關聯設定</a></li>
                </ul>
            </div>
        </nav>
    </div>
    <!-- Top Nav End -->
    <!-- Body Start -->
    <div class="row">
        <div class="col s12">
            <%-- <asp:PlaceHolder ID="ph_ErrMessage" runat="server">
                <div class="card-panel red darken-1 white-text">
                    <h4><i class="material-icons right">error_outline</i>糟糕了!!...發生了一點小問題</h4>
                    <p>若持續看到此訊息, 請回報 <strong class="flow-text">詳細操作狀況</strong>, 以便抓蟲<i class="material-icons">bug_report</i>。</p>
                    <p>
                        <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                    </p>
                </div>
            </asp:PlaceHolder>--%>
            <asp:PlaceHolder ID="ph_Data" runat="server">
                <!-- 基本設定 Start -->
                <div id="base" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>圖檔維護</h5>
                    </div>

                    <div class="card-content grey lighten-5">
                        <!-- Block Content Start -->
                        <div class="row">
                            <div class="col l4 m4 s12">
                                <div class="card">
                                    <div class="card-image">
                                        <asp:Literal ID="lt_Pic" runat="server">尚未上傳...</asp:Literal>
                                    </div>
                                    <div class="card-action center">
                                        <asp:Button ID="btn_Del" runat="server" Text="刪除圖檔" CssClass="btn waves-effect waves-light red" OnClick="btn_Del_Click" OnClientClick="return confirm('確定要刪除嗎?')" Visible="false" />
                                    </div>
                                </div>
                            </div>
                            <div class="col l7 m7 s12">
                                <div>
                                    <div class="row">
                                        <div class="col s12">
                                            <div class="file-field input-field">
                                                <div class="btn">
                                                    <span>原始圖檔</span>
                                                    <asp:FileUpload ID="fu_Pic" runat="server" ValidationGroup="base" />
                                                </div>
                                                <div class="file-path-wrapper">
                                                    <input class="file-path validate" type="text" placeholder="選擇要上傳的檔案">
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                                <ul class="tabs">
                                    <li class="tab col s6"><a class="active" href="#tab1">縮圖自動壓縮</a></li>
                                    <li class="tab col s6"><a href="#tab2">縮圖自行上傳</a></li>
                                </ul>
                                <div id="tab1" class="row">
                                    <div class="col s12">
                                        <blockquote class="color-green">
                                            <h6>上傳後, 系統會自動將圖片壓縮成500x500 / 1000x1000<br />
                                                (請盡量上傳PNG較能保持圖片品質)</h6>
                                            <div class="row section">
                                                <div class="col s12">
                                                    <asp:RadioButtonList ID="rbl_LogoType" runat="server" CssClass="with-gap" RepeatLayout="Flow" RepeatDirection="Vertical">
                                                        <asp:ListItem Value="0" Text='<span class="flow-text">不使用Logo浮水印</span>'></asp:ListItem>
                                                        <asp:ListItem Value="1" Text='&nbsp;<img src="https://cdn.prokits.com.tw/images/ProductCenter/SelectLogo/pk.jpg" alt="img1" width="150" />' Selected="True"></asp:ListItem>
                                                        <asp:ListItem Value="2" Text='&nbsp;<img src="https://cdn.prokits.com.tw/images/ProductCenter/SelectLogo/robot.jpg" alt="img1" width="150" />'></asp:ListItem>
                                                        <asp:ListItem Value="3" Text='&nbsp;<img src="https://cdn.prokits.com.tw/images/ProductCenter/SelectLogo/science.jpg" alt="img1" width="150" />'></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col s12 right-align">
                                                    <asp:LinkButton ID="lbtn_Save1" runat="server" CssClass="btn waves-effect waves-light blue" ValidationGroup="base" OnClick="lbtn_Save1_Click">開始上傳</asp:LinkButton>
                                                </div>
                                            </div>
                                        </blockquote>
                                    </div>
                                </div>

                                <div id="tab2" class="row">
                                    <div class="col s12">
                                        <blockquote class="color-blue">
                                            <h6>自行上傳各種Size的圖片<br />
                                                (請盡量上傳PNG較能保持圖片品質)</h6>
                                            <div class="row">
                                                <div class="col s12">
                                                    <div class="file-field input-field">
                                                        <div class="btn">
                                                            <span>500x500</span>
                                                            <asp:FileUpload ID="file_500" runat="server" />
                                                        </div>
                                                        <div class="file-path-wrapper">
                                                            <input class="file-path validate" type="text" placeholder="選擇要上傳的檔案">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col s12">
                                                    <div class="file-field input-field">
                                                        <div class="btn">
                                                            <span>1000x100</span>
                                                            <asp:FileUpload ID="file_1000" runat="server" />
                                                        </div>
                                                        <div class="file-path-wrapper">
                                                            <input class="file-path validate" type="text" placeholder="選擇要上傳的檔案">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col s12 right-align">
                                                    <asp:LinkButton ID="lbtn_Save2" runat="server" CssClass="btn waves-effect waves-light blue" ValidationGroup="base" OnClick="lbtn_Save2_Click">開始上傳</asp:LinkButton>
                                                </div>
                                            </div>
                                        </blockquote>
                                    </div>
                                </div>
                            </div>
                            <div class="col l1 m1"></div>
                        </div>

                        <!-- Block Content End -->
                    </div>


                    <div class="hidden-field" style="display: none;">
                        <asp:HiddenField ID="hf_Pic" runat="server" />
                        <asp:HiddenField ID="hf_Pic500" runat="server" />
                        <asp:HiddenField ID="hf_Pic1000" runat="server" />
                        <asp:HiddenField ID="hf_OrgPic" runat="server" />
                        <asp:HiddenField ID="hf_PicID" runat="server" />
                        <asp:HiddenField ID="hf_RelID" runat="server" />
                        <asp:HiddenField ID="hf_Lang" runat="server" />
                        <asp:HiddenField ID="hf_RelModel" runat="server" />
                    </div>
                </div>
                <!-- 基本設定 End -->

                <!-- 關聯設定 Start -->
                <div id="rel" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>關聯設定</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="input-field col s12">
                                <label>輸入品號關鍵字</label>
                                <asp:TextBox ID="tb_ModelNo" runat="server" MaxLength="40" placeholder="輸入品號關鍵字" ValidationGroup="Rel"></asp:TextBox>

                                <asp:RequiredFieldValidator ID="rfv_GroupVal_IDs" runat="server" Display="Dynamic" CssClass="red-text"
                                    ErrorMessage="請輸入正確的品號" ControlToValidate="tb_GroupVal_IDs" ValidationGroup="Rel"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <div id="Item_List">
                                    <!-- 動態新增 -->
                                    <ul class="collection">
                                    </ul>
                                    <!-- 動態項目的值集合 -->
                                    <asp:TextBox ID="tb_GroupVal_IDs" runat="server" ToolTip="項目欄位值組合" Style="display: none;"></asp:TextBox>
                                </div>
                                <asp:Panel ID="pl_unRel" runat="server" CssClass="SiftItem" Visible="false">
                                    <span class="SiftLight">(以下品號已存在其他關聯)</span><br />
                                    <asp:Literal ID="lt_unRel" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12 right-align">
                                <asp:LinkButton ID="lbtn_AddRel" runat="server" CssClass="btn waves-effect waves-light green lighten-1" ValidationGroup="Rel" OnClientClick="Get_Item()" OnClick="lbtn_AddRel_Click">儲存關聯</asp:LinkButton>
                            </div>
                        </div>
                        <div>
                            <asp:ListView ID="lvDataList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                                GroupItemCount="3" OnItemCommand="lvDataList_ItemCommand">
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
                                    <td style="width: 20%">
                                        <a href="<%#Application["WebUrl"] %>ProdPic/myPic_Maintain.aspx?ModelNo=<%#Server.UrlEncode(Eval("Model_No").ToString())%>&Cls=<%#Server.UrlEncode(Param_Class)%>&Col=<%#Server.UrlEncode(Eval("Pic_Column").ToString())%>&rt=<%#Server.UrlEncode(LastUrl)%>">
                                            <%#Eval("Model_No").ToString()%></a>
                                    </td>
                                    <td style="width: 13%" class="center-align">
                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('是否確定移除關聯!?\n此品號的圖片將會保留。')" ValidationGroup="ListRel"><i class="material-icons">clear</i></asp:LinkButton>
                                        <asp:HiddenField ID="hf_Model_No" runat="server" Value='<%# Eval("Model_No")%>' />
                                        <asp:HiddenField ID="hf_Rel_ID" runat="server" Value='<%# Eval("Rel_ID")%>' />
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td style="width: 20%">&nbsp;</td>
                                    <td style="width: 13%">&nbsp;</td>
                                </EmptyItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="center-align grey-text text-lighten-1">
                                        <i class="material-icons flow-text">info_outline</i>
                                        <span class="flow-text">尚未加入關聯</span>
                                    </div>

                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
                <!-- 關聯設定 End -->
            </asp:PlaceHolder>

        </div>
    </div>

    <!-- Body End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(document).on('focus', ':input', function () {
            $(this).attr('autocomplete', 'off');
        });

        (function ($) {
            $(function () {
                //scrollSpy
                $('.scrollspy').scrollSpy();


                //tab
                //$('ul.tabs').tabs();

            }); // end of document ready
        })(jQuery); // end of jQuery name space

    </script>

    <%-- jQueryUI Start --%>
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/jquery-ui.min.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/jquery-ui.min.js"></script>
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/catcomplete/catcomplete.js"></script>
    <%-- jQueryUI End --%>
    <script type="text/javascript">
        $(function () {
            /* Autocomplete - 群組分類(品號) */
            $("#MainContent_tb_ModelNo").catcomplete({
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
                    //自動新增項目
                    Add_Item(ui.item.label);
                    //自動清除目前欄位
                    setTimeout(function () {
                        try {
                            clear();
                        } catch (e) { }
                    }, 300);

                }
            });

        });

        //清除功能
        function clear() {
            $("#MainContent_tb_ModelNo").val('');
            //隱藏驗證控制項警示
            $("#MainContent_rfv_GroupVal_IDs").css("display", "none");
        }
    </script>
    <script type="text/javascript">
        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_Item(ObjVal) {
            //判斷是否已選擇
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            if (ObjVal == "") {
                alert('請輸入正確的品號!');
                return;
            }
            if (ObjVal == '<%=Param_ModelNo %>') {
                alert('品號已存在(主要品號)!');
                return;
            }

            //檢查是否有重覆
            var doInsert = true;
            $("#Item_List ul li .Item_Val").each(
                function (i, elm) {
                    if (ObjVal == $(elm).val()) {
                        alert(ObjVal + ' 重覆新增');
                        doInsert = false;
                        return;
                    }
                }
            );
            if (doInsert == false) {
                return;
            }

            //建立Html
            var NewItem = '<li id="li_' + ObjId + '" class="collection-item">';
            NewItem += '<input type="hidden" class="Item_Val" value="' + ObjVal + '" />';
            NewItem += '<div>' + ObjVal + '<a href="#!" class="secondary-content" onclick="Delete_Item(\'' + ObjId + '\');">刪除</a></div>';
            NewItem += '</li>';
            $("#Item_List ul").append(NewItem);
        }

        /* 刪除項目 */
        function Delete_Item(TarObj) {
            $("#li_" + TarObj).remove();
        }

        function Delete_AllItem() {
            $("#Item_List ul li").each(
               function (i, elm) {
                   $(elm).remove();
               });
        }
        /* 時間function */
        Date.prototype.Format = function (fmt) { //author: meizz
            var o = {
                "M+": this.getMonth() + 1,                 //月份
                "d+": this.getDate(),                    //日
                "h+": this.getHours(),                   //小時
                "m+": this.getMinutes(),                 //分
                "s+": this.getSeconds(),                 //秒
                "q+": Math.floor((this.getMonth() + 3) / 3), //季度
                "S": this.getMilliseconds()             //毫秒
            };
            if (/(y+)/.test(fmt))
                fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
            for (var k in o)
                if (new RegExp("(" + k + ")").test(fmt))
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
            return fmt;
        }

        /* 取得各項目欄位值
        分隔符號 : |
        */
        function Get_Item() {

            //清空欄位值(產品編號)
            var val_ProdID = $("#MainContent_tb_GroupVal_IDs");
            val_ProdID.val("");

            //巡覽物件, 取得值(產品編號)
            $("#Item_List ul li .Item_Val").each(
                function (i, elm) {
                    var OldCont = val_ProdID.val();
                    if (OldCont == '') {
                        val_ProdID.val($(elm).val());
                    } else {
                        val_ProdID.val(OldCont + '|' + $(elm).val());
                    }
                }
            );
        }
        //----- 動態欄位 End -----
    </script>
</asp:Content>

