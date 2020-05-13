<%@ Page Title="認證資料維護" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="Cert_View.aspx.cs" Inherits="Certification_Cert_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">產品中心</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">認證資料庫</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section red-text text-darken-2">認證資料檢視
                    </h5>
                </div>
            </div>
            <div class="right menu">
                <a class="anchor" id="top"></a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui grid">
            <div class="row">
                <!-- Left Body Content Start -->
                <div id="myStickyBody" class="thirteen wide column">
                    <div class="ui attached segment grey-bg lighten-5">
                        <!-- Section-基本資料 Start -->
                        <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                            <div class="ui negative message">
                                <div class="header">
                                    Oops!
                                </div>
                                <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>
                        <div class="ui segments">
                            <div class="ui green segment">
                                <h5 class="ui header"><a class="anchor" id="baseData"></a>基本資料</h5>
                            </div>
                            <div id="formBase" class="ui small form segment">
                                <div class="fields">
                                    <div class="four wide field">
                                        <label>系統編號</label>
                                        <div class="ui red basic large label">
                                            <asp:Literal ID="lt_SeqNo" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                    <div class="six wide field">
                                        <label>品號</label>
                                        <div class="ui basic blue large label">
                                            <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                    <div class="three wide field">
                                        <label>類別</label>
                                        <div class="ui basic green large label">
                                            <asp:Literal ID="lt_Class" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                    <div class="three wide field">
                                        <label>主要出貨地</label>
                                        <div class="ui basic brown large label">
                                            <asp:Literal ID="lt_ShipFrom" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </div>
                                <div class="fields">
                                    <div class="four wide field">
                                        <label>歸檔位置</label>
                                        <div class="ui basic large fluid label">
                                            <asp:Literal ID="lt_Doc_Path" runat="server"></asp:Literal>&nbsp;
                                        </div>
                                    </div>
                                    <div class="six wide field">
                                        <label>寶工自有認證</label>
                                        <div class="ui basic large fluid label">
                                            <asp:Literal ID="lt_Self_Cert" runat="server"></asp:Literal>
                                        </div>
                                    </div>

                                    <div class="three wide field">
                                        <label>目錄</label>
                                        <div class="ui basic large fluid label">
                                            <asp:Literal ID="lb_Vol" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                    <div class="three wide field">
                                        <label>頁次</label>
                                        <div class="ui basic large fluid label">
                                            <asp:Literal ID="lb_Page" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </div>
                                <div class="fields">
                                    <div class="four wide field">
                                        <label>廠商</label>
                                        <div class="ui basic large fluid label">
                                            <asp:Literal ID="lt_Supplier" runat="server"></asp:Literal>&nbsp;
                                        </div>
                                    </div>
                                    <div class="six wide field">
                                        <label>廠商料號</label>
                                        <div class="ui basic large fluid label">
                                            <asp:Literal ID="lt_Supplier_ItemNo" runat="server"></asp:Literal>&nbsp;
                                        </div>
                                    </div>

                                    <div class="six wide field">
                                        <label>備註</label>
                                        <div class="ui basic fluid label">
                                            <asp:Literal ID="lt_Remark" runat="server"></asp:Literal>&nbsp;
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Section-基本資料 End -->

                        <asp:PlaceHolder ID="ph_Details" runat="server" Visible="false">
                            <!-- Section-認證明細 Start -->
                            <div class="ui segments">
                                <div class="ui blue segment">
                                    <h5 class="ui header"><a class="anchor" id="section1"></a>認證明細</h5>
                                </div>
                                <div class="ui segment">
                                    <asp:ListView ID="lv_Detail" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lv_Detail_ItemDataBound">
                                        <LayoutTemplate>
                                            <table id="tableList" class="ui celled selectable compact small table nowrap" style="width: 100%;">
                                                <thead>
                                                    <tr>
                                                        <th>證書類別</th>
                                                        <th>證書編號</th>
                                                        <th>測試報告號碼</th>
                                                        <th>官網下載</th>
                                                        <th>認證指令</th>
                                                        <th>認證規範</th>
                                                        <th>測試器/主機/安全等級</th>
                                                        <th>測試棒/安全等級</th>
                                                        <th>發證日期</th>
                                                        <th>有效日期</th>
                                                        <th>證書</th>
                                                        <th>Test Report</th>
                                                        <th>自我宣告</th>
                                                        <th>自我檢測</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                </tbody>
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="left aligned collapsing">
                                                    <div class="red-text text-darken-2">
                                                        <b>
                                                            <asp:Literal ID="lt_CertType" runat="server"></asp:Literal></b>
                                                    </div>
                                                    <div style="padding-top: 5px">
                                                        <asp:Literal ID="lt_Icon" runat="server"></asp:Literal>
                                                    </div>
                                                </td>
                                                <td>
                                                    <%#Eval("Cert_No")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Cert_RptNo")%>
                                                </td>
                                                <td class="center aligned">
                                                    <%#fn_Desc.PubAll.YesNo(Eval("IsWebDW").ToString())%>
                                                </td>
                                                <td>
                                                    <%#Eval("Cert_Cmd")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Cert_Norm").ToString().Replace("\r\n", "<BR/>")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Cert_Desc1").ToString().Replace("\r\n", "<BR/>")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Cert_Desc2").ToString().Replace("\r\n", "<BR/>")%>
                                                </td>
                                                <td class="center aligned collapsing">
                                                    <%# String.Format("{0:yyyy-MM-dd}", Eval("Cert_ApproveDate"))%>
                                                </td>
                                                <td class="center aligned collapsing">
                                                    <%# String.Format("{0:yyyy-MM-dd}", Eval("Cert_ValidDate"))%>
                                                </td>
                                                <td class="center aligned">
                                                    <asp:Literal ID="lt_CertFile" runat="server"></asp:Literal>
                                                </td>
                                                <td class="center aligned">
                                                    <asp:Literal ID="lt_FileTestReport" runat="server"></asp:Literal>
                                                </td>
                                                <td class="center aligned collapsing">
                                                    <div style="padding-bottom: 4px">
                                                        <span class="styleGraylight">(繁中)</span>&nbsp;                                           
                                                        <asp:Literal ID="lt_FileCE" runat="server"></asp:Literal>
                                                    </div>
                                                    <div style="padding-bottom: 4px">
                                                        <span class="styleGraylight">(英文)</span>&nbsp;                                           
                                                        <asp:Literal ID="lt_FileCE_enUS" runat="server"></asp:Literal>
                                                    </div>
                                                    <div>
                                                        <span class="styleGraylight">(簡中)</span>&nbsp;                                           
                                                        <asp:Literal ID="lt_FileCE_zhCN" runat="server"></asp:Literal>
                                                    </div>
                                                </td>
                                                <td class="center aligned collapsing">
                                                    <div style="padding-bottom: 4px">
                                                        <span class="styleGraylight">(繁中)</span>&nbsp;                                           
                                                        <asp:Literal ID="lt_FileCheck" runat="server"></asp:Literal>
                                                    </div>
                                                    <div style="padding-bottom: 4px">
                                                        <span class="styleGraylight">(英文)</span>&nbsp;                                           
                                                        <asp:Literal ID="lt_FileCheck_enUS" runat="server"></asp:Literal>
                                                    </div>
                                                    <div>
                                                        <span class="styleGraylight">(簡中)</span>&nbsp;                                           
                                                        <asp:Literal ID="lt_FileCheck_zhCN" runat="server"></asp:Literal>
                                                    </div>
                                                </td>

                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="ui placeholder segment">
                                                <div class="ui icon header">
                                                    <i class="coffee icon"></i>
                                                    尚未加入資料
                                                </div>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <!-- Section-認證明細 End -->

                            <!-- Section-維護資訊 Start -->
                            <div class="ui segments">
                                <div class="ui grey segment">
                                    <h5 class="ui header"><a class="anchor" id="infoData"></a>維護資訊</h5>
                                </div>
                                <div class="ui segment">
                                    <table class="ui celled small four column table">
                                        <thead>
                                            <tr>
                                                <th colspan="2" class="center aligned">建立</th>
                                                <th colspan="2" class="center aligned">最後更新</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr class="center aligned">
                                                <td>
                                                    <asp:Literal ID="info_Creater" runat="server">資料建立中...</asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="info_CreateTime" runat="server">資料建立中...</asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="info_Updater" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="info_UpdateTime" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <!-- Section-維護資訊 End -->
                        </asp:PlaceHolder>
                    </div>

                </div>
                <!-- Left Body Content End -->

                <!-- Right Navi Menu Start -->
                <div class="three wide column">
                    <div class="ui sticky">
                        <div id="fastjump" class="ui secondary vertical pointing fluid text menu">
                            <div class="header item">快速跳轉<i class="dropdown icon"></i></div>
                            <a href="#baseData" class="item">基本資料</a>
                            <a href="#section1" class="item">認證明細</a>
                            <a href="#top" class="item"><i class="angle double up icon"></i>到頂端</a>
                        </div>

                        <div class="ui vertical text menu">
                            <div class="header item">功能按鈕</div>
                            <div class="item">
                                <a href="<%:Page_SearchUrl %>" class="ui small button"><i class="undo icon"></i>返回列表</a>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Right Navi Menu End -->
            </div>
        </div>

    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">

    <%-- 快速選單 --%>
    <script src="<%=fn_Param.WebUrl %>javascript/sticky.js"></script>

    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script>
        $(function () {
            //使用DataTable
            var table = $('#tableList').DataTable({
                fixedHeader: true,
                searching: true,  //搜尋
                ordering: false,   //排序
                paging: false,     //分頁
                info: false,      //頁數資訊
                language: {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                //捲軸設定
                "scrollY": '60vh',
                "scrollCollapse": true,
                "scrollX": true
            });


        });

    </script>
    <%-- DataTable End --%>
</asp:Content>

