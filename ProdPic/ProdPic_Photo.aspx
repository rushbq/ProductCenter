<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProdPic_Photo.aspx.cs" Inherits="ProdPic_Photo" %>

<%@ Register Src="Ascx_ProdPicClass.ascx" TagName="Ascx_ProdPicClass" TagPrefix="uc1" %>
<%@ Register Src="Ascx_ProdData.ascx" TagName="Ascx_ProdData" TagPrefix="uc2" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <%-- fancybox helpers Start --%>
    <script src="../js/fancybox/helpers/jquery.fancybox-thumbs.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-thumbs.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'elastic',
                nextEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    },
                    buttons: {}
                }
            });

            //Click事件 - 一般欄位click後全選
            $("input[type=text]").click(function () {
                $(this).select();
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a href="../Main.aspx">系統首頁</a>&gt;<a>圖片資料庫</a>&gt;<span>圖片資料維護</span>
        </div>
        <div class="h2Head">
            <h2>
                <%=Param_ModelNo %></h2>
        </div>
        <div class="SysTab">
            <uc1:Ascx_ProdPicClass ID="Ascx_ProdPicClass1" runat="server" />
        </div>
        <div>
            <!-- 基本資料 Start -->
            <uc2:Ascx_ProdData ID="Ascx_ProdData1" runat="server" />
            <!-- 基本資料 End -->
        </div>
        <table class="TableModify">
            <!-- 圖片資料 Start -->
            <tr class="ModifyHead" id="PicList">
                <td colspan="4">圖片資料<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTdHead" style="width: 100px">左側
                    </td>
                    <td class="TableModifyTd" style="width: 360px" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic01" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic01") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime01" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName01" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl01" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">正面
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic02" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic02") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime02" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName02" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl02" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">右側
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic03" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic03") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime03" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName03" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl03" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">背面
                    </td>
                    <td class="TableModifyTd" valign="top">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic04" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic04") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime04" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName04" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl04" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">配件包材
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic05" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic05") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime05" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName05" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl05" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">本體線圖
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic06" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic06") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime06" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName06" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl06" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">備用1
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic07" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic07") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime07" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName07" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl07" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">備用2
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic08" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic08") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime08" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName08" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl08" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">實體包裝
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic09" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic09") %>">圖檔維護</a>
                                        &nbsp;
                                     <a href="http://upload.prokits.com.tw/PicUpload/?ModelNo=<%=Server.UrlEncode(Param_ModelNo) %>&PicCls=<%=Server.UrlEncode(Param_Class) %>&PicCol=Pic09" class="btnBlock colorGray" target="_blank">手機上傳</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime09" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName09" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl09" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">電商主圖
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic10" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic10") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime10" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName10" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl10" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">宣傳圖</td>
                    <td class="TableModifyTd">
                        <div style="padding: 5px 0px 5px 0px;">
                            <div>
                                <div style="float: left; width: 180px; text-align: center;">
                                    <asp:Literal ID="lt_Pic11" runat="server"></asp:Literal>
                                </div>
                                <div style="float: left; text-align: left;">
                                    <div>
                                        <a class="btnBlock colorGray" href="<%=Get_MaintainUrl("Pic11") %>">圖檔維護</a>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicUpdTime11" runat="server"></asp:Literal>
                                    </div>
                                    <div class="styleGraylight" style="padding-top: 10px;">
                                        <asp:Literal ID="lt_PicName11" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div style="clear: both; padding-top: 10px;">
                                <asp:Literal ID="lt_PicUrl11" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </td>
                    <td class="TableModifyTdHead"></td>
                    <td class="TableModifyTd"></td>
                </tr>

                <tr>
                    <td class="TableModifyTdHead styleBlue">API圖片路徑<br />
                        (單張)</td>
                    <td class="TableModifyTd" colspan="3">
                        <div>&nbsp;&nbsp;500x500：&nbsp;&nbsp;<input type="text" value="<%=Application["Api_WebUrl"] %>EcImg/500/<%=Server.UrlEncode(Param_ModelNo) %>/" style="width: 500px; cursor: pointer; color: #555" readonly="readonly" /></div>
                        <div>1000x1000：<input type="text" value="<%=Application["Api_WebUrl"] %>EcImg/1000/<%=Server.UrlEncode(Param_ModelNo) %>/" style="width: 500px; cursor: pointer; color: #555" readonly="readonly" /></div>
                    </td>
                </tr>
                <tr style="display:none">
                    <td class="TableModifyTdHead styleCafe">圖片原始檔
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <div>
                            <asp:TextBox ID="tb_PicSource" runat="server" MaxLength="100" Width="70%"></asp:TextBox>
                            <asp:Button ID="btn_Save" runat="server" Text="儲存" OnClick="btn_Save_Click" CssClass="btnBlock colorBlue"
                                ValidationGroup="AddPicSrc" Enabled="false" /><br />
                            <asp:RequiredFieldValidator ID="rfv_tb_PicSource" runat="server" ControlToValidate="tb_PicSource"
                                ForeColor="Red" ErrorMessage="-&gt; 請輸入原始檔路徑" Display="Dynamic" ValidationGroup="AddPicSrc"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_PicSource" runat="server" ControlToValidate="tb_PicSource"
                                ErrorMessage="-&gt; 禁止使用中文字" ValidationExpression="^[^\u4e00-\u9fa5]{0,}$" Display="Dynamic"
                                ForeColor="Red" ValidationGroup="AddPicSrc"></asp:RegularExpressionValidator>
                        </div>
                        <div class="ListIllus SiftLight">
                            <div>
                                註1.&nbsp;請輸入資料夾(含)及以下路徑及檔案名稱 [UNC格式]。
                            </div>
                            <div>
                                註2.&nbsp;請勿使用中文名稱。
                            </div>
                            <div>
                                註3.&nbsp;避免使用空白字元。
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- 圖片資料 End -->
        </table>
        <div style="display: none;">
            <asp:HiddenField ID="hf_PicID" runat="server" />
            <asp:Button ID="btn_DelAll" runat="server" Text="全部刪除" OnClick="btn_DelAll_Click"
                CausesValidation="false" CssClass="btnBlock colorRed" />
            <a href="<%=Session["BackListUrl"] %>" class="btnBlock colorGray">返回列表</a>
        </div>
        <table class="TableModify">
            <!-- 維護資訊 Start -->
            <tr class="ModifyHead">
                <td colspan="2">維護資訊<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead" style="width: 120px">維護資訊
                </td>
                <td class="TableModifyTd">
                    <table cellpadding="3" border="0">
                        <tr>
                            <td align="right" width="100px">建立者：
                            </td>
                            <td class="styleGreen" width="200px">
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
        <!-- Scroll Bar Icon -->
        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
