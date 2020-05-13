<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PDFSet_byClass.aspx.cs" Inherits="PDFSet_byClass" %>

<%@ Register Src="Ascx_QuickMenu.ascx" TagName="Ascx_QuickMenu" TagPrefix="uc1" %>
<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $("#TreeView").treeview({
                collapsed: true,
                animated: "fast",
                control: "#sidetreecontrol"
            });

            //Checkbox階層式全選
            $("input[id*='cb_']").click(function () {
                //取得目前元素的ID
                var thisID = $(this).attr("id");

                //判斷目前元素是否勾選
                if ($(this).attr("checked")) {
                    //檢查每個元素
                    $("input[id*='" + thisID + "']").each(function () {
                        $(this).attr("checked", true);  //將其下每一層CheckBox勾選

                        //[取得定義] - 屬性rel (上層編號)
                        var thisLevel = $(this).attr("rel");

                        //[取得定義] - 上層編號
                        var thisUpID = $("#" + thisLevel);

                        //[判斷定義] - 判斷屬性rel是否有定義
                        if (typeof (thisLevel) != "undefined") {
                            //拆解字串
                            var arythisLevel = thisLevel.split("_");
                            for (var i = 0; i < arythisLevel.length - 1; i++) {
                                //判斷上層項目是否勾選
                                if (typeof (thisUpID.attr("checked")) == "undefined") {
                                    thisUpID.attr("checked", true);
                                }
                                //再上層Rel
                                thisLevel = thisUpID.attr("rel");
                                //再上層編號
                                thisUpID = $("#" + thisLevel);
                            }
                        }
                    });
                }
                else {
                    $("input[id*='" + thisID + "']").each(function () {
                        $(this).attr("checked", false);
                    });
                }
            });
        });

        //取得Checkbox已勾選的值
        function GetCbxValue() {
            var tmpID = '';
            $("input[id*='cb_']:checkbox:checked").each(function (i) {
                //排除空值
                if (this.value != '') {
                    if (tmpID != '') {
                        tmpID += ',';
                    }
                    tmpID += this.value;
                }
            });
            //將勾選值填入(分類項目不列入)
            $("#hf_RelID").val(tmpID);

            //觸發click事件
            $('#btn_GetRelID').trigger('click');
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a href="../Main.aspx">系統首頁</a>&gt;<a>產品資料庫</a>&gt;<a>規格設定</a>&gt;<span>PDF匯出設定 (by類別)</span>
    </div>
    <div class="h2Head">
        <h2>
            PDF匯出設定 (by類別)</h2>
    </div>
    <uc1:Ascx_QuickMenu ID="Ascx_QuickMenu1" runat="server" />
    <!-- 備註說明 -->
    <div class="ListIllusArea">
        <div class="JQ-ui-state-error">
            <div class="styleEarth">
                <span class="JQ-ui-icon ui-icon-info"></span>&nbsp;設定以分類為主的 PDF 匯出規格欄位。</div>
            <div class="styleEarth">
                <span class="JQ-ui-icon ui-icon-info"></span>&nbsp;若要設定以品號為主的欄位，請前往各品號編輯頁設定。</div>
        </div>
    </div>
    <div id="sidetreecontrol" class="MenuSecControl" style="padding-top: 5px">
        <a href="?#">收合</a> | <a href="?#">展開</a> | <a href="#" onclick="location.reload();">
            重整</a> |
        <input type="button" onclick="GetCbxValue()" value="儲存勾選" style="width: 90px" class="btnBlock colorRed" />
        <asp:Button ID="btn_GetRelID" runat="server" Text="Button" OnClick="btn_GetRelID_Click"
            ValidationGroup="Save" Style="display: none" />
        <asp:HiddenField ID="hf_RelID" runat="server" />
        | <span class="styleGreen">目前分類：</span>
        <asp:DropDownListGP ID="ddl_SpecClass" runat="server">
        </asp:DropDownListGP>
        <asp:Button ID="btn_SpecClass" runat="server" Text="選擇分類" OnClick="btn_SpecClass_Click"
            ValidationGroup="Select" Style="width: 90px" class="btnBlock colorGray" />
        <asp:RequiredFieldValidator ID="rfv_ddl_SpecClass" runat="server" ErrorMessage="請選擇分類！"
            Display="Dynamic" ForeColor="Red" ControlToValidate="ddl_SpecClass" ValidationGroup="Select"></asp:RequiredFieldValidator>
        <asp:PlaceHolder ID="ph_link" runat="server"><a href="javascript:top.mainFrame.location.href='SpecClass_Search.aspx'">
            設定規格分類</a></asp:PlaceHolder>
    </div>
    <hr class="MenuSecondHr" />
    <div class="MenuSecond">
        <asp:Literal ID="lt_TreeView" runat="server"></asp:Literal>
    </div>
    <!-- Scroll Bar Icon -->
    <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="N"
        ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
