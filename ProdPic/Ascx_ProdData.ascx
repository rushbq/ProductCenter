<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ascx_ProdData.ascx.cs"
    Inherits="Ascx_ProdData" %>
<script type="text/javascript">
    $(document).ready(function () {
        $(".DTtoggle").click(function () {
            //取得此物件的rel屬性值 (#xx)
            var _this = $(this).attr("rel");
            var _img = $(this).attr("imgrel");
            //判斷指定元素是否隱藏
            if ($(_this).css("display") == "none") {
                $(this).attr("title", "收合");
                $(_this).show();
                $(_img).attr("src", "../images/icon_top.png");
            } else {
                $(this).attr("title", "展開");
                $(_this).hide();
                $(_img).attr("src", "../images/icon_down.png");
            }
            return false;
        });
    });
</script>
<!-- 基本資料 Start -->
<div>
    <table class="TableModify">
        <tr class="ModifyHead DTtoggle" rel="#dt1" imgrel="#img1" title="收合" style="cursor: pointer">
            <td colspan="4">
                <img src="../images/icon_top.png" id="img1" />
                基本資料<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tbody id="dt1">
            <tr>
                <td class="TableModifyTdHead" style="width: 120px">
                    品號
                </td>
                <td class="TableModifyTd styleRed B" style="width: 350px">
                    <%=Param_ModelNo %>
                </td>
                <td class="TableModifyTdHead" style="width: 100px">
                    主要出貨地
                </td>
                <td class="TableModifyTd">
                    <%=Param_ShipFrom%>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    子件品號
                </td>
                <td class="TableModifyTd">
                    <%=Param_Parts%>
                </td>
                <td class="TableModifyTdHead">
                    目錄/頁次
                </td>
                <td class="TableModifyTd">
                    <%=Param_Vol%>
                    /
                    <%=Param_Page%>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    上市日期
                </td>
                <td class="TableModifyTd styleBlue">
                    <%=Param_onLine%>
                </td>
                <td class="TableModifyTdHead">
                    停售日期
                </td>
                <td class="TableModifyTd">
                    <%=Param_offLine%>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    條碼
                </td>
                <td class="TableModifyTd">
                    <%=Param_BarCode%>
                </td>
                <td class="TableModifyTdHead">
                    卡片品號
                </td>
                <td class="TableModifyTd">
                    <%=Param_Pub_Card_Model_No %>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    替代品號
                </td>
                <td class="TableModifyTd">
                    <%=Param_Substitute_Model_No%>
                </td>
                <td class="TableModifyTdHead">
                    個案失效日
                </td>
                <td class="TableModifyTd">
                    <%=Param_Cases_Of_Failure_Date%>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    主/配件
                </td>
                <td class="TableModifyTd">
                    <%=Param_Pub_Accessories%>
                </td>
                <td class="TableModifyTdHead">
                </td>
                <td class="TableModifyTd">
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    中文品名
                </td>
                <td class="TableModifyTd" colspan="3">
                    <%=Param_Name_zhTW%>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    英文品名
                </td>
                <td class="TableModifyTd" colspan="3">
                    <%=Param_Name_enUS%>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<!-- 基本資料 End -->
<!-- 認證資料 Start -->
<div id="view" style="left: 0px; top: 0px; width: 100%; height: px; overflow: auto;">
    <table class="TableModify">
        <tr class="ModifyHead DTtoggle" rel="#dt2" imgrel="#img2" title="展開" style="cursor: pointer">
            <td colspan="4">
                <img src="../images/icon_down.png" id="img2" />
                認證資料<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tbody id="dt2" style="display: none">
            <tr>
                <td class="TableModifyTd" colspan="4">
                    <asp:ListView ID="lvCertList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvCertList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="List1" width="100%">
                                <tr class="tdHead" style="white-space: nowrap;">
                                    <td width="130px">
                                        證書類別
                                    </td>
                                    <td width="120px">
                                        證書編號
                                    </td>
                                    <td width="100px">
                                        認證指令
                                    </td>
                                    <td>
                                        認證規範
                                    </td>
                                    <td>
                                        測試器/主機/安全等級
                                    </td>
                                    <td>
                                        測試棒/安全等級
                                    </td>
                                    <td width="90px">
                                        發證日期
                                    </td>
                                    <td width="90px">
                                        有效日期
                                    </td>
                                    <td width="70px">
                                        證書
                                    </td>
                                    <td width="70px">
                                        Test<br />
                                        Report
                                    </td>
                                    <td width="80px">
                                        自我宣告
                                    </td>
                                    <td width="80px">
                                        自我檢測
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="trItem" runat="server" style="white-space: nowrap;">
                                <td class="L2MainHead">
                                    <div>
                                        <asp:Literal ID="lt_CertType" runat="server"></asp:Literal></div>
                                    <div style="padding-top: 5px">
                                        <asp:Literal ID="lt_Icon" runat="server"></asp:Literal></div>
                                </td>
                                <td align="center">
                                    <%#Eval("Cert_No")%>
                                </td>
                                <td align="center">
                                    <%#Eval("Cert_Cmd")%>
                                </td>
                                <td align="left">
                                    <%#Eval("Cert_Norm").ToString().Replace("\r\n", "<BR/>")%>
                                </td>
                                <td align="left">
                                    <%#Eval("Cert_Desc1").ToString().Replace("\r\n", "<BR/>")%>
                                </td>
                                <td align="left">
                                    <%#Eval("Cert_Desc2").ToString().Replace("\r\n", "<BR/>")%>
                                </td>
                                <td align="center">
                                    <%# String.Format("{0:yyyy-MM-dd}", Eval("Cert_ApproveDate"))%>
                                </td>
                                <td align="center">
                                    <%# String.Format("{0:yyyy-MM-dd}", Eval("Cert_ValidDate"))%>
                                </td>
                                <td align="center">
                                    <asp:Literal ID="lt_CertFile" runat="server"></asp:Literal>
                                </td>
                                <td align="center">
                                    <asp:Literal ID="lt_FileTestReport" runat="server"></asp:Literal>
                                </td>
                                <td align="left">
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
                                <td align="left">
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
                            <div style="padding: 20px 0px 20px 0px; text-align: center">
                                <span style="color: #FD590B; font-size: 12px">目前尚無認證資料！</span>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<!-- 認證資料 End -->
