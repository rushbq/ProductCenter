<%@ Page Title="外驗查核表|發信" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Mail.aspx.cs" Inherits="myProdCheck_Mail" %>

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
                    <span class="brand-logo center">發送查檢表：<asp:Literal ID="lt_Title" runat="server"></asp:Literal></span>
                    <ul class="right">
                        <asp:PlaceHolder ID="ph_Lock" runat="server" Visible="false">
                            <li class="grey-text text-lighten-1"><i class="material-icons right">lock_outline</i>已結案</li>
                        </asp:PlaceHolder>
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
            <asp:PlaceHolder ID="ph_OK" runat="server" Visible="false">
                <div class="card-panel green darken-1 white-text">
                    <h4><i class="material-icons right">done</i>郵件最近發送時間<small>
                        <asp:Literal ID="lt_MailTime" runat="server"></asp:Literal>
                    </small></h4>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ph_Mail" runat="server">
                <!-- MailList Start -->
                <div id="mail" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <div class="left">
                            <h5>選取發信人員&nbsp;(請自行勾選要核准的主管)</h5>
                        </div>
                        <div class="right">
                            <asp:PlaceHolder ID="ph_MailBtn" runat="server">
                                <a id="doSet" class="btn waves-effect waves-light blue">確認發信</a>
                            </asp:PlaceHolder>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s12">
                                <blockquote class="color-green">
                                    <h6>人員清單</h6>
                                    <div id="userList" class="ztree"></div>
                                </blockquote>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- MailList End -->

                <div style="display: none" class="serversidecontroller">
                    <asp:Button ID="btn_Setting" runat="server" Text="Set" OnClick="btn_Setting_Click" />
                    <asp:TextBox ID="tb_Values_User" runat="server"></asp:TextBox>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_Data" runat="server">
                <!-- ERP採購單資料 Start -->
                <div id="erp" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>ERP採購單資料</h5>
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
                        <h5>查檢表欄位</h5>
                    </div>
                    <div class="card-content grey lighten-5">
                        <div class="row">
                            <div class="col s6">
                                <label>狀態</label>
                                <p>
                                    <asp:Literal ID="lt_Status" runat="server"></asp:Literal>
                                </p>
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
                            <div class="col s6">
                                <label>預計驗貨日</label>
                                <p>
                                    <asp:Literal ID="lt_Date_Est" runat="server"></asp:Literal>
                                </p>
                            </div>
                            <div class="col s6">
                                <label>實際驗貨日</label>
                                <p>
                                    <asp:Literal ID="lt_Date_Act" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col s12">
                                <label>備註</label>
                                <p>
                                    <asp:Literal ID="lt_Remark" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 查檢表欄位 End -->

                <!-- 檢驗報表 Start -->
                <div id="report" class="card grey scrollspy">
                    <div class="card-content white-text">
                        <h5>檢驗報表</h5>
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


            </asp:PlaceHolder>

        </div>
    </div>

    <!-- Body End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
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
                    $("#MainContent_tb_Values_User").val(valAry_User.join(","));

                    //觸發設定 click
                    $("#MainContent_btn_Setting").trigger("click");

                });
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>

