<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_View.aspx.cs" Inherits="Prod_View" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<%@ Register Src="Ascx_TabMenu_View.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- fancybox helpers Start --%>
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'none',
                nextEffect: 'none',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                }

            });

            //資料展開/收合
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
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">
                <%=Navi_系統首頁%></a>&gt;<a><%=Navi_產品資料庫%></a>&gt;<span><%=Navi_產品資料%></span>
        </div>
        <div class="h2Head">
            <h2>
                <%=Navi_產品資料%></h2>
        </div>
        <div class="SysTab">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <table class="TableModify">
            <!-- 基本資料 Start -->
            <tr class="ModifyHead DTtoggle" rel="#dt1" imgrel="#img1" title="收合" style="cursor: pointer">
                <td colspan="4">
                    <img src="../images/icon_top.png" id="img1" />
                    基本資料<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt1">
                <asp:PlaceHolder ID="ph_MainNo" runat="server" Visible="false">
                    <tr>
                        <td class="TableModifyTd Font15 styleBlue B" colspan="4">此品號為子件型號，請點此 <a class="styleRed" href="<%=Application["WebUrl"] %>Product/Prod_View.aspx?Model_No=<%=Server.UrlEncode(Param_MainNo) %>">
                            <%=Param_MainNo %></a> 查看主件資料。
                        </td>
                    </tr>
                </asp:PlaceHolder>

                <tr class="Must">
                    <td class="TableModifyTdHead" rowspan="9">產品圖
                    </td>
                    <td class="TableModifyTd" align="center" rowspan="9">
                        <asp:Literal ID="lt_Photo" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">品號
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Model_No" runat="server" CssClass="styleRed B"><%=Param_ModelNo %></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">貨號
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Item_No" runat="server" CssClass="styleGraylight"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">銷售類別
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Class_ID" runat="server" CssClass="styleGreen B"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">倉管類別
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Warehouse_Class_ID" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">主要出貨地
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Ship_From" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">上市日期
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Date_Of_Listing" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">停售日期
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Stop_Offer_Date" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">子件型號
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Part_No" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">品名
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">(繁中)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Model_Name_zh_TW" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(簡中)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Model_Name_zh_CN" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(英文)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Model_Name_en_US" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">規格
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">(繁中)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Model_Desc" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(簡中)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Model_Desc_zh_CN" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(英文)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Model_Desc_en_US" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead" style="width: 100px">條碼
                    </td>
                    <td class="TableModifyTd" style="width: 350px">
                        <asp:Literal ID="lt_BarCode" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead" style="width: 100px">目錄/頁次
                    </td>
                    <td class="TableModifyTd" style="width: 350px">
                        <asp:Literal ID="lt_Catelog_Vol" runat="server"></asp:Literal>/
                    <asp:Literal ID="lt_Page" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">替代品號
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">(TW)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Substitute_Model_No_TW" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(SH)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Substitute_Model_No_SH" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(SZ)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Substitute_Model_No_SZ" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">個案失效日
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Cases_Of_Failure_Date" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">專利號碼
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Patent_No" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead">國家標準</td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Standard1" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">企業標準
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Standard2" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead">行業標準
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Standard3" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">Logo/呈現方式
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lt_Pub_Logo" runat="server" CssClass="styleEarth B"></asp:Label><br />
                        <asp:Literal ID="lt_Pub_Logo_Printing" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead">主/配件</td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Accessories" runat="server"></asp:Literal>
                    </td>
                </tr>

                <tr>
                    <td class="TableModifyTdHead">理想用途
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <div class="SiftItem">
                            <asp:Literal ID="lt_Pub_Use" runat="server"></asp:Literal>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">選購配件型號
                    </td>
                    <td class="TableModifyTd">
                        <div class="SiftItem">
                            <asp:Literal ID="lt_Pub_Select_ModelNo" runat="server"></asp:Literal>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">對應主件型號
                    </td>
                    <td class="TableModifyTd">
                        <div class="SiftItem">
                            <asp:Literal ID="lt_Pub_Compare_ModelNo" runat="server"></asp:Literal>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- 基本資料 End -->
            <!-- 共用欄位 Start -->
            <!-- // 產品單一裸重 Start // -->
            <tr class="ModifyHead DTtoggle" rel="#dt9" imgrel="#img9" title="展開" style="cursor: pointer">
                <td colspan="4">
                    <img src="../images/icon_down.png" id="img9" />
                    產品單一裸重<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt9" style="display: none">
                <tr>
                    <td class="TableModifyTdHead">尺寸
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">長(mm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_PW_L" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">寬(mm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_PW_W" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">高(mm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_PW_H" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">重量
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="70px" class="styleGraylight">裸重(g)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IP_Net_Weight" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">其他
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Pub_PW_Other" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>
            <!-- // 產品單一裸重 End // -->
            <!-- // 包裝 Start // -->
            <tr class="ModifyHead DTtoggle" rel="#dt2" imgrel="#img2" title="展開" style="cursor: pointer">
                <td colspan="4">
                    <img src="../images/icon_down.png" id="img2" />
                    Individual Packing (最小包裝)<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt2" style="display: none">
                <tr>
                    <td class="TableModifyTdHead">包裝方式
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">(中文)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Individual_Packing_zh_TW" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="styleGraylight">(英文)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Individual_Packing_en_US" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">重量
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="70px" class="styleGraylight">單重(Kg)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IP_Weight" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">尺寸
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">長(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IP_L" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">寬(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IP_W" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">高(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IP_H" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">圖片
                    </td>
                    <td class="TableModifyTd" align="left">
                        <asp:Literal ID="lt_Photo_Packing" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>
            <!-- // 包裝 End // -->
            <!-- // 內箱 Start // -->
            <tr class="ModifyHead DTtoggle" rel="#dt3" imgrel="#img3" title="展開" style="cursor: pointer">
                <td colspan="4">
                    <img src="../images/icon_down.png" id="img3" />
                    Inner box (內盒包裝)<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt3" style="display: none">
                <tr>
                    <td class="TableModifyTdHead">內盒產品數量
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_IB_Qty" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead">重量
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="70px" class="styleGraylight">淨重(kg)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IB_NW" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="70px" class="styleGraylight">毛重(Kg)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IB_GW" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">尺寸
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">長(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IB_L" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">寬(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IB_W" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">高(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_IB_H" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">材積(CUFT)
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_IB_CUFT" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>
            <!-- // 內箱 End // -->
            <!-- // 外箱 Start // -->
            <tr class="ModifyHead DTtoggle" rel="#dt4" imgrel="#img4" title="展開" style="cursor: pointer">
                <td colspan="4">
                    <img src="../images/icon_down.png" id="img4" />
                    Carton (整箱數量)<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt4" style="display: none">
                <tr>
                    <td class="TableModifyTdHead">數量
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="60px" class="styleGraylight">整箱數量
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_Qty_CTN" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="120px" class="styleGraylight">外包裝含內盒數
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_Qty" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">重量
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="70px" class="styleGraylight">淨重(kg)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_NW" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="70px" class="styleGraylight">毛重(Kg)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_GW" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">尺寸
                    </td>
                    <td class="TableModifyTd">
                        <table cellpadding="3" border="0" width="98%">
                            <tr>
                                <td align="right" width="50px" class="styleGraylight">長(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_L" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">寬(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_W" runat="server"></asp:Literal>
                                </td>
                                <td align="right" width="50px" class="styleGraylight">高(cm)
                                </td>
                                <td>
                                    <asp:Literal ID="lt_Pub_Carton_H" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="TableModifyTdHead">材積(CUFT)
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Carton_CUFT" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>
            <!-- // 外箱 End // -->
            <!-- // 其他 Start // -->
            <tr class="ModifyHead DTtoggle" rel="#dt5" imgrel="#img5" title="展開" style="cursor: pointer">
                <td colspan="4">
                    <img src="../images/icon_down.png" id="img5" />
                    其他<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody id="dt5" style="display: none">
                <tr>
                    <td class="TableModifyTdHead" style="width: 100px">產品線圖
                    </td>
                    <td class="TableModifyTd" style="width: 350px">
                        <asp:Literal ID="lt_Photo_Outline" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead" style="width: 100px">產品檢驗輔助圖片
                    </td>
                    <td class="TableModifyTd" style="width: 350px">
                        <asp:Literal ID="lt_QC_Pics" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">品管類別
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_QC_Category" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead">主廚推薦
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Recommended" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">品管檢驗項目
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table class="List1" width="100%">
                                    <tr class="tdHead">
                                        <td width="100px">檢驗名稱
                                        </td>
                                        <td width="80px">缺點等級
                                        </td>
                                        <td>檢驗標準說明
                                        </td>
                                        <td width="150px">備註
                                        </td>
                                    </tr>
                                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr id="trItem" runat="server">
                                    <td align="center">
                                        <%#Eval("ME002")%>
                                    </td>
                                    <td align="center">
                                        <%#Eval("MG005")%>
                                    </td>
                                    <td align="left">
                                        <%#Eval("MG006")%>
                                    </td>
                                    <td align="left">
                                        <%#Eval("MG007")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">產品備註
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Pub_Notes" runat="server"></asp:Literal>
                    </td>
                </tr>
                <%--
                    ** Remove at 2017-11-02, by Kelly **
                <tr>
                    <td class="TableModifyTdHead">產品特點
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <a href="http://www.proskit.com.tw/tw/product/product_detail.asp?itemid=<%=HttpUtility.UrlEncode(Param_ModelNo) %>#features"
                            target="_blank">連結</a>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">特點說明
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Pub_Features_Remark" runat="server"></asp:Literal>
                    </td>
                </tr>--%>
                <tr>
                    <td class="TableModifyTdHead">卡片品號
                    </td>
                    <td class="TableModifyTd">
                        <asp:Literal ID="lt_Pub_Card_Model_No" runat="server"></asp:Literal>
                    </td>
                    <td class="TableModifyTdHead"></td>
                    <td class="TableModifyTd"></td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">備用欄位2
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Pub_Alternate2" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">產品訊息
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <asp:Literal ID="lt_Pub_Message" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>
            <!-- // 其他 End // -->
            <!-- 共用欄位 End -->
        </table>
        <div id="view" style="left: 0px; top: 0px; width: 100%; overflow: auto;">
            <table class="TableModify">
                <!-- // 認證資料 Start // -->
                <tr class="ModifyHead DTtoggle" rel="#dt6" imgrel="#img6" title="展開" style="cursor: pointer">
                    <td>
                        <img src="../images/icon_down.png" id="img6" />
                        認證資料<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody id="dt6" style="display: none">
                    <tr>
                        <td class="TableModifyTd">
                            <asp:ListView ID="lvCertList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvCertList_ItemDataBound">
                                <LayoutTemplate>
                                    <table class="List1" width="100%">
                                        <tr class="tdHead" style="white-space: nowrap;">
                                            <td width="130px">證書類別
                                            </td>
                                            <td width="120px">證書編號
                                            </td>
                                            <td width="100px">認證指令
                                            </td>
                                            <td>認證規範
                                            </td>
                                            <td>測試器/主機/安全等級
                                            </td>
                                            <td>測試棒/安全等級
                                            </td>
                                            <td width="90px">發證日期
                                            </td>
                                            <td width="90px">有效日期
                                            </td>
                                            <td width="70px">證書
                                            </td>
                                            <td width="70px">Test<br />
                                                Report
                                            </td>
                                            <td width="80px">自我宣告
                                            </td>
                                            <td width="80px">自我檢測
                                            </td>
                                        </tr>
                                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr id="trItem" runat="server" style="white-space: nowrap;">
                                        <td class="L2MainHead">
                                            <div>
                                                <asp:Literal ID="lt_CertType" runat="server"></asp:Literal>
                                            </div>
                                            <div style="padding-top: 5px">
                                                <asp:Literal ID="lt_Icon" runat="server"></asp:Literal>
                                            </div>
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
                <!-- // 認證資料 End // -->
            </table>
        </div>
        <div>
            <table class="TableModify">
                <!-- // 規格符號 Start // -->
                <tr class="ModifyHead DTtoggle" rel="#dt7" imgrel="#img7" title="展開" style="cursor: pointer">
                    <td>
                        <img src="../images/icon_down.png" id="img7" />
                        規格符號<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody id="dt7" style="display: none">
                    <tr>
                        <td class="TableModifyTd">
                            <asp:ListView ID="lvIconList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                                GroupItemCount="8" OnItemDataBound="lvIconList_ItemDataBound">
                                <LayoutTemplate>
                                    <table class="List1" width="100%">
                                        <asp:PlaceHolder ID="ph_Group" runat="server" />
                                    </table>
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <tr>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tr>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <td align="center" width="12.5%">
                                        <%#PicUrl(Eval("Pic_File").ToString())%>
                                        <div style="padding-top: 5px;">
                                            <asp:Label ID="lb_Disp" runat="server"></asp:Label>
                                        </div>
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td></td>
                                </EmptyItemTemplate>
                            </asp:ListView>
                        </td>
                    </tr>
                </tbody>
                <!-- // 規格符號 End // -->
            </table>
        </div>
        <div>
            <table class="TableModify">
                <!-- // SOP下載 Start // -->
                <tr class="ModifyHead DTtoggle" rel="#dt8" imgrel="#img8" title="收合" style="cursor: pointer">
                    <td>
                        <img src="../images/icon_down.png" id="img8" />
                        SOP下載<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody id="dt8" style="display: none;">
                    <tr>
                        <td class="TableModifyTd">
                            <asp:ListView ID="lv_SOP" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                                GroupItemCount="3">
                                <LayoutTemplate>
                                    <table class="List1" width="100%">
                                        <asp:PlaceHolder ID="ph_Group" runat="server" />
                                    </table>
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <tr>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tr>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <td width="24%">
                                        <a href="<%#Application["WebUrl"] %>FileDownload.ashx?OrgiName=<%#Server.UrlEncode(Eval("SOP_OrgFile").ToString()) %>&FilePath=<%#Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + Eval("SOP_File").ToString())) %>">
                                            <%#Eval("SOP_OrgFile").ToString()%></a>
                                    </td>
                                    <td align="center" width="9%">
                                        <%#Eval("Create_Time").ToString().ToDateString("yyyy-MM-dd")%>
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td width="24%">&nbsp;
                                    </td>
                                    <td width="9%">&nbsp;
                                    </td>
                                </EmptyItemTemplate>
                            </asp:ListView>
                        </td>
                    </tr>
                </tbody>
                <!-- // SOP下載 End // -->
            </table>
        </div>
        <div>
            <table class="TableModify">
                <!-- 維護資訊 Start -->
                <tr class="ModifyHead">
                    <td colspan="4">維護資訊<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead" style="width: 100px">維護資訊
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <table cellpadding="3" border="0">
                            <tr>
                                <td align="right" width="100px">建立者：
                                </td>
                                <td class="styleGreen" width="150px">
                                    <asp:Literal ID="lt_Create_Who" runat="server" Text="新增資料中"></asp:Literal>
                                </td>
                                <td align="right" width="100px">建立時間：
                                </td>
                                <td class="styleGreen" width="250px">
                                    <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">最後修改者：
                                </td>
                                <td class="styleGreen">
                                    <asp:Literal ID="lt_Update_Who" runat="server"></asp:Literal>
                                </td>
                                <td align="right">最後修改時間：
                                </td>
                                <td class="styleGreen">
                                    <asp:Literal ID="lt_Update_Time" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <!-- 維護資訊 End -->
            </table>
        </div>
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
