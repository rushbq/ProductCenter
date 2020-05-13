<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cert_Edit_DTL.aspx.cs" Inherits="Cert_Edit_DTL" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <%-- tooltip Start --%>
    <link href="../js/tooltip/tip-darkgray/tip-darkgray.css" rel="stylesheet" type="text/css" />
    <script src="../js/tooltip/jquery.poshytip.min.js" type="text/javascript"></script>
    <%-- tooltip End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            /* DatePicker */
            $("#tb_Cert_ApproveDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd'
            });
            $("#tb_Cert_ValidDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd'
            });

            /* --- CE相關判斷 Start --- */
<%--            //CE, 宣告相關元素
            var cb_Items = $('input[id*="cb_IsCE"]');  //CE Checkbox
            var tr_CEFile = $('#CEFile');  //自我宣告檔案上傳
            var tr_CheckFile = $('#CheckFile');  //自我測試檔案上傳
            var tr_Icons = $('#Icons');  //符號表
            var Cert_Type = $('select#ddl_Cert_Type');  //證書類別
            var Cert_TypeText = $("#tb_Cert_TypeText"); //證書類別-其他(輸入框)
            var Detail_ID = $('#hf_Detail_ID');  //Detail ID
            Cert_TypeText.hide();

            //(PageLoad)判斷CE是否選取
            if (cb_Items[0].checked) {
                switch (Cert_Type.val()) {
                    case "EMC":
                        tr_CEFile.show();
                        tr_Icons.hide();
                        break;

                    case "LVD":
                        tr_CEFile.show();
                        tr_Icons.hide();
                        break;
                }
            } else {
                //自我檢測, (PageLoad)判斷類別
                switch (Cert_Type.val().toUpperCase()) {
                    case "ROHS":
                    case "EMC":
                    case "LVD":
                    case "PB FREE":
                        tr_CEFile.show();
                        tr_CheckFile.show();
                        break;

                    case "其他":
                        Cert_TypeText.show();
                        break;

                    default:
                        tr_CEFile.hide();
                        tr_CheckFile.hide();
                        break;
                }
                tr_Icons.show();
            }


            //CE, Click事件
            cb_Items.click(function () {
                if (cb_Items[0].checked) {
                    //判斷是否有選擇EMC/LVD
                    switch (Cert_Type.val()) {
                        case "EMC":
                            tr_CEFile.show();
                            tr_Icons.hide();
                            break;

                        case "LVD":
                            tr_CEFile.show();
                            tr_Icons.hide();
                            break;

                        default:
                            tr_CEFile.hide();
                            tr_Icons.show();
                            cb_Items[0].checked = false;
                            alert('「證書類別」須為EMC/LVD');
                            return false;
                    }
                } else {
                    tr_CEFile.hide();
                    tr_Icons.show();
                }
            });

            //Cert_Type, onChange事件
            Cert_Type.change(function () {
                var getVal = $(this).find(':selected').val().toUpperCase();  //目標值
                Cert_TypeText.hide();

                if (cb_Items[0].checked) {
                    //判斷是否有選擇EMC/LVD
                    switch (getVal) {
                        case "EMC":
                            tr_CEFile.show();
                            tr_CheckFile.show();
                            tr_Icons.hide();
                            break;

                        case "LVD":
                            tr_CEFile.show();
                            tr_CheckFile.show();
                            tr_Icons.hide();
                            break;

                        default:
                            //判斷為新增/修改
                            if (Detail_ID.val() != '') {
                                Cert_Type.val('<%=Param_CertType %>');
                            } else {
                                tr_CEFile.hide();
                                tr_CheckFile.hide();
                                tr_Icons.show();
                                cb_Items[0].checked = false;
                            }
                            alert('「證書類別」須為EMC/LVD');
                            return false;
                    }
                } else {
                    //判斷是否有選擇Rohs/PbFree
                    switch (getVal) {
                        case "ROHS":
                        case "EMC":
                        case "LVD":
                        case "PB FREE":
                            tr_CEFile.show();
                            tr_CheckFile.show();
                            break;

                        case "其他":
                            Cert_TypeText.show();
                            break;

                        default:
                            tr_CEFile.hide();
                            tr_CheckFile.hide();
                            break;
                    }
                    tr_Icons.show();
                }
            });--%>
            /* --- CE相關判斷 End --- */

            /* --- 符號表 Start --- */
            //符號表顯示大圖 
            $(".tooltip").poshytip({
                className: 'tip-darkgray',
                bgImageFrameSize: 9,
                offsetX: -10,
                offsetY: 10,
                content: function () {
                    var fileSrc = $(this).attr('src');
                    var iconName = $(this).attr('iconName');
                    var html = '<div>' + iconName + '</div>';
                    html += '<div><img src="' + fileSrc + '" width="150"></div>';
                    return html;
                }
            });
            //Click事件 - 圖片選擇
            $('input[id*="cbl_Icon"]').click(function () {
                //取得上層 - TD
                var this_tID = $(this).parent("td");
                //判斷是否選取
                if ($(this).attr("checked")) {
                    //變更Css
                    this_tID.addClass("input01");
                } else {
                    //變更Css
                    this_tID.removeClass("input01");
                }
            });

            //巡覽Checkbox - 判斷圖片選擇已勾選
            $('input[id*="cbl_Icon"]').each(function () {
                //取得上層 - TD
                var this_tID = $(this).parent("td");
                //判斷是否選取
                if ($(this).attr("checked")) {
                    //變更Css
                    this_tID.addClass("input01");
                } else {
                    //變更Css
                    this_tID.removeClass("input01");
                }
            });
            /* --- 符號表 End --- */
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                明細資料編輯<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead" style="width: 150px">
                <em>(*)</em>證書類別
            </td>
            <td class="TableModifyTd styleBlue">
                <asp:DropDownList ID="ddl_Cert_Type" runat="server" Width="100px" ValidationGroup="GPAdd">
                </asp:DropDownList>
                <asp:TextBox ID="tb_Cert_TypeText" runat="server" MaxLength="50"></asp:TextBox>
                <br />
                <asp:RequiredFieldValidator ID="rfv_ddl_Cert_Type" runat="server" ErrorMessage="-&gt; 請輸入「證書類別」"
                    ControlToValidate="ddl_Cert_Type" Display="Dynamic" ForeColor="Red" ValidationGroup="GPAdd"></asp:RequiredFieldValidator>
            </td>
           <%-- <td class="TableModifyTdHead" style="width: 80px">
                CE
            </td>
            <td class="TableModifyTd" style="width: 100px">
                <asp:CheckBox ID="cb_IsCE" runat="server" />
            </td>--%>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>官網可否下載
            </td>
            <td class="TableModifyTd">
                <asp:RadioButtonList ID="rbl_IsWebDW" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Y">是&nbsp;</asp:ListItem>
                    <asp:ListItem Value="N" Selected="True">否</asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="rfv_rbl_IsWebDW" runat="server" ErrorMessage="-&gt; 請選擇「官網可否下載」"
                    ControlToValidate="rbl_IsWebDW" Display="Dynamic" ForeColor="Red" ValidationGroup="GPAdd"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr id="CEFile" runat="server">
            <td class="TableModifyTdHead">
                自我宣告
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;
                    <asp:Literal ID="lt_Cert_File_CE" runat="server"></asp:Literal>
                    <asp:FileUpload ID="fu_Cert_File_CE" runat="server" />
                    <asp:Button ID="btn_Del_CE" runat="server" Text="刪除" CssClass="styleReddark" OnClientClick="return confirm('是否確定刪除!?')"
                        OnClick="btn_Del_CE_Click" Visible="false" />
                </div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;
                    <asp:Literal ID="lt_Cert_File_CE_en_US" runat="server"></asp:Literal>
                    <asp:FileUpload ID="fu_Cert_File_CE_en_US" runat="server" />
                    <asp:Button ID="btn_Del_CE_en_US" runat="server" Text="刪除" CssClass="styleReddark"
                        OnClientClick="return confirm('是否確定刪除!?')" OnClick="btn_Del_CE_en_US_Click" Visible="false" />
                </div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;
                    <asp:Literal ID="lt_Cert_File_CE_zh_CN" runat="server"></asp:Literal>
                    <asp:FileUpload ID="fu_Cert_File_CE_zh_CN" runat="server" />
                    <asp:Button ID="btn_Del_CE_zh_CN" runat="server" Text="刪除" CssClass="styleReddark"
                        OnClientClick="return confirm('是否確定刪除!?')" OnClick="btn_Del_CE_zh_CN_Click" Visible="false" />
                </div>
            </td>
        </tr>
        <tr id="CheckFile" runat="server" style="display: none">
            <td class="TableModifyTdHead">
                自我檢測
            </td>
            <td class="TableModifyTd">
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(繁中)</span>&nbsp;
                    <asp:Literal ID="lt_Cert_File_Check" runat="server"></asp:Literal>
                    <asp:FileUpload ID="fu_Cert_File_Check" runat="server" />
                    <asp:Button ID="btn_Del_Check" runat="server" Text="刪除" CssClass="styleReddark" OnClientClick="return confirm('是否確定刪除!?')"
                        OnClick="btn_Del_Check_Click" Visible="false" />
                </div>
                <div style="padding-bottom: 4px">
                    <span class="styleGraylight">(英文)</span>&nbsp;
                    <asp:Literal ID="lt_Cert_File_Check_en_US" runat="server"></asp:Literal>
                    <asp:FileUpload ID="fu_Cert_File_Check_en_US" runat="server" />
                    <asp:Button ID="btn_Del_Check_en_US" runat="server" Text="刪除" CssClass="styleReddark"
                        OnClientClick="return confirm('是否確定刪除!?')" OnClick="btn_Del_Check_en_US_Click"
                        Visible="false" />
                </div>
                <div>
                    <span class="styleGraylight">(簡中)</span>&nbsp;
                    <asp:Literal ID="lt_Cert_File_Check_zh_CN" runat="server"></asp:Literal>
                    <asp:FileUpload ID="fu_Cert_File_Check_zh_CN" runat="server" />
                    <asp:Button ID="btn_Del_Check_zh_CN" runat="server" Text="刪除" CssClass="styleReddark"
                        OnClientClick="return confirm('是否確定刪除!?')" OnClick="btn_Del_Check_zh_CN_Click"
                        Visible="false" />
                </div>
            </td>
        </tr>
        <tr id="Icons">
            <td class="TableModifyTdHead">
                符號表<br />
                <span class="styleBlue">(移到圖上可放大)</span>
            </td>
            <td class="TableModifyTd">
                <asp:CheckBoxList ID="cbl_Icon" runat="server" RepeatColumns="6" RepeatLayout="Table"
                    RepeatDirection="Horizontal" CssClass="List2" Width="70%">
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>發證日期
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_ApproveDate" runat="server" Style="text-align: center" Width="100px"
                    ValidationGroup="GPAdd"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="rfv_tb_Cert_ApproveDate" runat="server" Display="Dynamic"
                    ControlToValidate="tb_Cert_ApproveDate" ErrorMessage="-&gt; 請輸入「發證日期」" ForeColor="Red"
                    ValidationGroup="GPAdd"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="rev_tb_Cert_ApproveDate" runat="server" ErrorMessage="-&gt; 「發證日期」格式錯誤"
                    ControlToValidate="tb_Cert_ApproveDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                    Display="Dynamic" ForeColor="Red" ValidationGroup="GPAdd"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr class="Must">
            <td class="TableModifyTdHead">
                <em>(*)</em>證書編號
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_No" runat="server" MaxLength="50" Width="300px" autocomplete="off"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfv_tb_Cert_No" runat="server" Display="Dynamic"
                    ControlToValidate="tb_Cert_No" ErrorMessage="-&gt; 請輸入「證書編號」" ForeColor="Red"
                    ValidationGroup="GPAdd"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                測試報告號碼
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_RptNo" runat="server" MaxLength="20" Width="300px"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td class="TableModifyTdHead">
                認證指令
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_Cmd" runat="server" MaxLength="100" Width="300px" Height="60px"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                認證規範
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_Norm" runat="server" MaxLength="255" Width="300px" Height="60px"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                測試器/主機/安全等級
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_Desc1" runat="server" MaxLength="300" Width="300px" Height="60px"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                測試棒/安全等級
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_Desc2" runat="server" MaxLength="300" Width="300px" Height="60px"
                    TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                有效日期
            </td>
            <td class="TableModifyTd">
                <asp:TextBox ID="tb_Cert_ValidDate" runat="server" Style="text-align: center" Width="100px"
                    ValidationGroup="GPAdd"></asp:TextBox><br />
                <asp:RegularExpressionValidator ID="rev_Cert_ValidDate" runat="server" ErrorMessage="-&gt; 「有效日期」格式錯誤"
                    ControlToValidate="tb_Cert_ValidDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                    Display="Dynamic" ForeColor="Red" ValidationGroup="GPAdd"></asp:RegularExpressionValidator>
                <asp:CompareValidator ID="cv_Date" runat="server" ErrorMessage="-&gt; 「有效日期」必須大於「發證日期」"
                    Type="Date" Operator="GreaterThan" ControlToValidate="tb_Cert_ValidDate" ControlToCompare="tb_Cert_ApproveDate"
                    Display="Dynamic" ForeColor="Red" ValidationGroup="GPAdd"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTitleIcon styleBlue" colspan="4">
                &nbsp; ** 上傳檔案的總大小，一次不可超過 <span class="styleRed">200 MB</span>，超過會有恐佈的事發生 **
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                證書 <em>(檔案上傳)</em>
            </td>
            <td class="TableModifyTd">
                <asp:Literal ID="lt_Cert_File" runat="server"></asp:Literal>
                <asp:FileUpload ID="fu_Cert_File" runat="server" />
                <asp:Button ID="btn_Del_File" runat="server" Text="刪除" CssClass="styleReddark" OnClientClick="return confirm('是否確定刪除,證書!?')"
                    OnClick="btn_Del_File_Click" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead">
                Test Report <em>(檔案上傳)</em>
            </td>
            <td class="TableModifyTd">
                <asp:Literal ID="lt_Cert_File_Report" runat="server"></asp:Literal>
                <asp:FileUpload ID="fu_Cert_File_Report" runat="server" />
                <asp:Button ID="btn_Del_Report" runat="server" Text="刪除" CssClass="styleReddark"
                    OnClientClick="return confirm('是否確定刪除,Test Report!?')" OnClick="btn_Del_Report_Click"
                    Visible="false" />
            </td>
        </tr>
        <!-- 維護資訊 Start -->
        <tr class="ModifyHead">
            <td colspan="4">
                維護資訊<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTd" colspan="4">
                <table cellpadding="3" border="0">
                    <tr>
                        <td align="right" width="100px">
                            建立者：
                        </td>
                        <td class="styleGreen">
                            <asp:Literal ID="lt_Create_Who" runat="server" Text="新增資料中"></asp:Literal>
                        </td>
                        <td align="right" width="100px">
                            建立時間：
                        </td>
                        <td class="styleGreen" width="150px">
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
    </table>
    <div class="SubmitArea">
        <asp:HiddenField ID="hf_Cert_ID" runat="server" />
        <asp:HiddenField ID="hf_Detail_ID" runat="server" />
        <asp:HiddenField ID="hf_ModelNo" runat="server" />
        <asp:Button ID="btn_Edit" runat="server" Text="新增" OnClick="btn_Edit_Click" ValidationGroup="GPAdd"
            Width="90px" CssClass="btnBlock colorBlue" />
        <input onclick="parent.$.fancybox.close();" type="button" value="關閉視窗" style="width: 90px"
            class="btnBlock colorGray" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
            ShowMessageBox="true" ValidationGroup="GPAdd" />
    </div>
    <div class="ListIllus">
        <span>標明<em class="Must">(*)</em>的項目請務必填寫。</span>
    </div>
    </form>
    <script language="javascript" type="text/javascript">
        function EnterClick(e) {
            // 這一行讓 ie 的判斷方式和 Firefox 一樣。
            if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;

            if (e.which == 13) {
                // Submit按鈕
                __doPostBack('btn_Edit', '');
                return false;
            }
        }

        document.onkeypress = EnterClick;
    </script>
</body>
</html>
