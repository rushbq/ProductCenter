<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="SupplierTradeInfo.aspx.cs" Inherits="Product_SupplierHistory" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Top Nav Start -->
    <div class="navbar-fixed">
        <nav class="pkColor">
            <div class="container">
                <div class="nav-wrapper">
                    <ul class="left">
                        <li><a href="<%=Application["WebUrl"] %>Product/SupplierHistory.aspx?DataID=<%=Req_DataID %>"><i class="material-icons left">arrow_back</i>Back</a></li>
                    </ul>
                </div>
                <div class="nav-content">
                    <span class="nav-title right flow-text"><strong><%=Req_DataID %></strong></span>
                </div>
            </div>
        </nav>
    </div>
    <!-- Top Nav End -->
    <!-- Body Start -->
    <div class="row">
        <div class="col s12 m12 l12">
            <div class="card grey">
                <div class="card-content white-text">
                    <h5>供應商交易記錄</h5>
                </div>
                <div class="card-content grey lighten-5">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table class="bordered centered highlight">
                                <thead>
                                    <tr>
                                        <th>幣別</th>
                                        <th>計價單位</th>
                                        <th>初次交易日</th>
                                        <th>廠商品號</th>
                                        <th>核價日</th>
                                        <th>上次進貨日</th>
                                        <th>分量計價</th>
                                        <th>採購單價</th>
                                        <th>備註</th>
                                        <th>含稅</th>
                                        <th>生效日</th>
                                        <th>失效日</th>
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
                                <td><%#Eval("Currency") %></td>
                                <td><%#Eval("Unit") %></td>
                                <td><%#Eval("FirstTradeDate") %></td>
                                <td><%#Eval("SupModelNo") %></td>
                                <td><%#Eval("CheckDate") %></td>
                                <td><%#Eval("LastInvDate") %></td>
                                <td><%#Eval("IsSpQty") %></td>
                                <td><%#Eval("BuyPrice") %></td>
                                <td><%#Eval("Remark") %></td>
                                <td><%#Eval("IsTax") %></td>
                                <td><%#Eval("ValidDate") %></td>
                                <td><%#Eval("InValidDate") %></td>
                                <td>
                                    <a href="#!" target-id="html<%#Server.UrlEncode(Eval("ValidDate").ToString()) %>" data-ValidDate="<%#Eval("ValidDate") %>" class="triggerSpQtyInfo waves-effect waves-green btn-flat" title="顯示分量計價"><i class="material-icons">more_vert</i></a>
                                </td>
                            </tr>
                            <tr id="html<%#Server.UrlEncode(Eval("ValidDate").ToString()) %>"></tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="section">
                                <div class="card-panel grey darken-1">
                                    <i class="material-icons flow-text white-text">error_outline</i>
                                    <span class="flow-text white-text">找不到資料!<i class="material-icons right">arrow_upward</i></span>
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>

                </div>
            </div>

        </div>
    </div>
    <!-- Body End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
      <script>
          $(function () {
              //[列表] - 分量計價 Click 觸發
              $('.triggerSpQtyInfo').on('click', function (event) {
                
                  //取得目標物件
                  var target = $(this);
                  var targetID = target.attr("target-id");

                  //取得參數
                  var modelNo = '<%=Req_DataID%>';
                  var supID = '<%=Req_SupID%>';
                  var company = '<%=Req_Company%>';
                  var validDate = target.attr("data-ValidDate");

                  //取得目標容器
                  var container = $("#" + targetID);

                  //填入Ajax Html
                  container.load("<%=Application["WebUrl"]%>Ajax_Data/GetHTML_SupplierSpQtyInfo.ashx?modelNo=" + modelNo + "&supID=" + supID + "&validDate=" + validDate + "&company=" + company);
                  
            });

        });
    </script>
</asp:Content>
