<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Icon_Edit.aspx.cs" Inherits="Icon_Edit" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
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
    <script src="../js/fancybox/helpers/jquery.fancybox-thumbs.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-thumbs.css" rel="stylesheet" type="text/css" />
    <script src="../js/fancybox/helpers/jquery.fancybox-buttons.js" type="text/javascript"></script>
    <link href="../js/fancybox/helpers/jquery.fancybox-buttons.css" rel="stylesheet"
        type="text/css" />
    <%-- fancybox helpers End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- 多筆上傳 Start --%>
    <script src="../js/multiFile/jquery.MultiFile.pack.js" type="text/javascript"></script>
    <%-- 多筆上傳 End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //資料展開/收合
            $("img.DTtoggle").click(function () {
                //取得此物件的rel屬性值 (#xx)
                var _this = $(this).attr("rel");
                //判斷指定元素是否隱藏
                if ($(_this).css("display") == "none") {
                    $(_this).show();
                    $(this).attr("src", "../images/icon_top.png").attr("title", "收合");
                } else {
                    $(_this).hide();
                    $(this).attr("src", "../images/icon_down.png").attr("title", "展開");
                }
                return false;
            });

            //fancybox - 圖片顯示
            $(".PicGroup").fancybox({
                prevEffect: 'none',
                nextEffect: 'none',
                helpers: {
                    title: {
                        type: 'inside'
                    },
                    overlay: {
                        opacity: 0.8,
                        css: {
                            'background-color': '#000'
                        }
                    },
                    thumbs: {
                        width: 50,
                        height: 50
                    },
                    buttons: {}
                }
            });

            //多筆上傳
            $('#fu_Pic').MultiFile({
                STRING: {
                    remove: '<img src="../images/cancel.png" alt="x" border="0" width="10" />' //移除圖示
                },
                accept: 'jpg|png|gif' //副檔名限制
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>符號資料庫</a>&gt;<span>符號資料維護</span>
    </div>
    <div class="h2Head">
        <h2>
            符號資料維護</h2>
    </div>
    <!-- 單頭資料 Start -->
    <asp:Panel ID="pl_Tip1" runat="server" CssClass="ListIllusArea">
        <div class="JQ-ui-state-error">
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span><span class="stylePurple">Step1. 新增基本資料，填入完成後請按<span
                    class="styleBluelight">「儲存基本資料」</span></span></div>
        </div>
    </asp:Panel>
    <div>
        <table class="TableModify">
            <!-- 基本設定 Start -->
            <tr class="ModifyHead">
                <td colspan="4">
                    <img src="../images/icon_top.png" rel="#dt1" class="DTtoggle" title="收合" style="cursor: pointer" />
                    基本資料<em class="TableModifyTitleIcon"></em>&nbsp;&nbsp;&nbsp; <span class="styleEarth Font12">
                        (Step1) 編輯基本資料</span>
                </td>
            </tr>
            <tbody id="dt1">
                <tr>
                    <td class="TableModifyTdHead" style="width: 100px">
                        系統編號
                    </td>
                    <td class="TableModifyTd styleBlue" style="width: 350px">
                        <asp:Literal ID="lt_Icon_ID" runat="server">系統自動編號</asp:Literal>
                    </td>
                    <td class="TableModifyTdHead" style="width: 100px">
                        符號使用者
                    </td>
                    <td class="TableModifyTd">
                        <asp:DropDownList ID="ddl_Icon_Type" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 自訂編號
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_CID" runat="server" Width="100px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_CID" runat="server" ErrorMessage="-&gt; 請輸入「自訂編號」"
                            ControlToValidate="tb_CID" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"
                            Style="text-align: center;"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cv_tb_CID" runat="server" ControlToValidate="tb_CID" Display="Dynamic"
                            ErrorMessage="-&gt; 請輸入數字！" Operator="DataTypeCheck" Type="Integer" ForeColor="Red"
                            ValidationGroup="Add"></asp:CompareValidator>
                    </td>
                    <td class="TableModifyTdHead">
                        是否顯示
                    </td>
                    <td class="TableModifyTd">
                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="Y" Selected="True">顯示</asp:ListItem>
                            <asp:ListItem Value="N">隱藏</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">
                        名稱
                    </td>
                    <td class="TableModifyTd">
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(繁中)</span>&nbsp;
                            <asp:TextBox ID="tb_IconName_zh_TW" runat="server" Width="80%" MaxLength="100"></asp:TextBox>
                        </div>
                        <div style="padding-bottom: 4px">
                            <span class="styleGraylight">(英文)</span>&nbsp;
                            <asp:TextBox ID="tb_IconName_en_US" runat="server" Width="80%" MaxLength="150"></asp:TextBox>
                        </div>
                        <div>
                            <span class="styleGraylight">(簡中)</span>&nbsp;
                            <asp:TextBox ID="tb_IconName_zh_CN" runat="server" Width="80%" MaxLength="100"></asp:TextBox>
                        </div>
                    </td>
                    <td class="TableModifyTdHead">
                        排序
                    </td>
                    <td class="TableModifyTd">
                        <div>
                            <asp:TextBox ID="tb_Sort" runat="server" Width="50px" MaxLength="3" Style="text-align: center;">999</asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="-&gt; 請輸入「排序」"
                                Display="Dynamic" ControlToValidate="tb_Sort" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="-&gt; 請輸入1 ~ 999 的數字"
                                Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                ForeColor="Red" ValidationGroup="Add"></asp:RangeValidator>
                        </div>
                        <div style="text-align: left; padding-top: 20px;">
                            <asp:Button ID="btn_Save" runat="server" Text="儲存基本資料" ValidationGroup="Add" OnClick="btn_Save_Click"
                                CssClass="btnBlock colorBlue" />
                            <input onclick="location.href = '<%=Session["BackListUrl"] %>';" type="button" value="返回列表"
                                style="width: 90px" class="btnBlock colorGray" />
                            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                            <asp:ValidationSummary ID="vs_Add" runat="server" ShowSummary="false" ShowMessageBox="true"
                                ValidationGroup="Add" />
                        </div>
                    </td>
                </tr>
                <!-- 基本設定 End -->
                <!-- 維護資訊 Start -->
                <tr class="ModifyHead" style="display: none">
                    <td colspan="4">
                        維護資訊<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tr style="display: none">
                    <td class="TableModifyTdHead" style="width: 100px">
                        維護資訊
                    </td>
                    <td class="TableModifyTd" colspan="3">
                        <table cellpadding="3" border="0">
                            <tr>
                                <td align="right" width="100px">
                                    建立者：
                                </td>
                                <td class="styleGreen" width="200px">
                                    <asp:Literal ID="lt_Create_Who" runat="server" Text="新增資料中"></asp:Literal>
                                </td>
                                <td align="right" width="100px">
                                    建立時間：
                                </td>
                                <td class="styleGreen" width="250px">
                                    <asp:Literal ID="lt_Create_Time" runat="server" Text="新增資料中"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    最後修改者：
                                </td>
                                <td class="styleGreen">
                                    <asp:Literal ID="lt_Update_Who" runat="server"></asp:Literal>
                                </td>
                                <td align="right">
                                    最後修改時間：
                                </td>
                                <td class="styleGreen">
                                    <asp:Literal ID="lt_Update_Time" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <!-- 維護資訊 End -->
            </tbody>
        </table>
    </div>
    <!-- 單頭資料 End -->
    <!-- 單身資料 Start -->
    <asp:Panel ID="pl_Tip2" runat="server" CssClass="ListIllusArea" Visible="false">
        <div class="JQ-ui-state-error">
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span><span class="stylePurple">Step2. 開始上傳符號圖片</span></div>
            <div>
                <span class="JQ-ui-icon ui-icon-info"></span><span class="stylePurple">(副檔名上傳限制：jpg,
                    png, gif)</span></div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pl_Detail" runat="server" Visible="false">
        <table class="TableModify">
            <tr class="ModifyHead">
                <td colspan="3">
                    圖片資料<em class="TableModifyTitleIcon"></em>&nbsp;&nbsp;&nbsp; <span class="styleEarth Font12">
                        (Step2) 編輯圖片資料</span>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead" style="width: 100px">
                    <em>(*)</em>上傳新圖片
                </td>
                <td class="TableModifyTd" valign="top" style="width: 450px">
                    <table>
                        <tr>
                            <td valign="top" style="width: 300px">
                                <asp:FileUpload ID="fu_Pic" runat="server" Width="250px" />
                                <asp:RequiredFieldValidator ID="rfv_fu_Pic" runat="server" ErrorMessage="-&gt; 請選擇要上傳的圖片"
                                    ForeColor="Red" Display="Dynamic" ControlToValidate="fu_Pic" ValidationGroup="PicAdd"></asp:RequiredFieldValidator>
                            </td>
                            <td valign="top" style="width: 150px" class="SiftLight">
                                * 以 PNG 為優先<br />
                                * 尺寸：150px * 150px
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="TableModifyTd" valign="top">
                    <div>
                        <asp:Button ID="btn_Upload" runat="server" Text="上傳圖片" ValidationGroup="PicAdd" OnClick="btn_Upload_Click"
                            Width="90px" CssClass="btnBlock colorBlue" />&nbsp;
                        <asp:Button ID="btn_SaveSort" runat="server" Text="儲存設定" OnClick="btn_SaveSort_Click"
                            ValidationGroup="List" Width="90px" CssClass="btnBlock colorRed" />&nbsp;
                        <asp:Button ID="btn_DelAll" runat="server" Text="全部刪除" OnClick="btn_DelAll_Click"
                            CausesValidation="false" OnClientClick="return confirm('是否確定刪除所有圖片!?')" Width="90px"
                            CssClass="btnBlock colorRed" />
                        <input onclick="location.href = '<%=Session["BackListUrl"] %>';" type="button" value="返回列表"
                            style="width: 90px" class="btnBlock colorGray" />
                    </div>
                    <div class="SiftLight">
                        * 可一次上傳多張圖片。
                    </div>
                    <asp:ValidationSummary ID="vs_List" runat="server" ShowMessageBox="true" ShowSummary="false"
                        ValidationGroup="List" />
                </td>
            </tr>
            <tr id="PicList">
                <td class="TableModifyTd" colspan="3">
                    <asp:ListView ID="lvDataList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                        GroupItemCount="4" OnItemCommand="lvDataList_ItemCommand">
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
                            <td align="center" width="25%">
                                <table border="0" width="100%">
                                    <tr>
                                        <%#PicUrl(Eval("Pic_File").ToString(), Eval("IconName").ToString())%>
                                        <td align="center">
                                            <table class="TableS1" width="98%">
                                                <tbody>
                                                    <tr>
                                                        <td class="TableS1TdHead" style="width: 35px;">
                                                            排序
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="tb_Sort" runat="server" Width="40px" Style="text-align: center;"
                                                                Text='<%# Eval("Sort")%>'></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="排序不可為空白"
                                                                ControlToValidate="tb_Sort" Display="Dynamic" ForeColor="Red" ValidationGroup="List"></asp:RequiredFieldValidator>
                                                            <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字！"
                                                                Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                                                ForeColor="Red" ValidationGroup="List"></asp:RangeValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="TableS1TdHead">
                                                            置換
                                                        </td>
                                                        <td>
                                                            <asp:FileUpload ID="fu_NewPic" runat="server" Width="120px" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:LinkButton ID="lbtn_NewPic" runat="server" CommandName="NewPic" CssClass="Edit">置換</asp:LinkButton>
                                                            <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                                                                OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                                                            <asp:Literal ID="lt_PicID" runat="server" Text='<%#Eval("Pic_ID") %>' Visible="false"></asp:Literal>
                                                            <asp:HiddenField ID="hf_OldFile" runat="server" Value='<%#Eval("Pic_File") %>' />
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </ItemTemplate>
                        <EmptyItemTemplate>
                            <td>
                            </td>
                        </EmptyItemTemplate>
                    </asp:ListView>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- 單身資料 End -->
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
        ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
