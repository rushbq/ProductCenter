<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_MallPic.aspx.cs" Inherits="Prod_MallPic"
    ValidateRequest="false" %>

<%@ Register Src="Ascx_TabMenu.ascx" TagName="Ascx_TabMenu" TagPrefix="ucTab" %>
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
    <script src="../js/blockUI/customFunc.js"></script>
    <%-- blockUI End --%>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <%-- 多筆上傳 Start --%>
    <script src="../js/multiFile/jquery.MultiFile.pack.js" type="text/javascript"></script>
    <%-- 多筆上傳 End --%>
    <script>
        $(function () {
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                //block-ui
                blockBox1('Add', '資料處理中...');

                //觸發
                $('#btn_doSave').trigger('click');
            });

            //Click事件 - 一般欄位click後全選
            $(".url").click(function () {
                $(this).select();
            });
        });
    </script>
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

            /* MultiFile 多筆上傳, 當瀏覽器為 ie 時才啟用 */
            if ($.browser.msie) {
                $('#fu_Pic').MultiFile({
                    STRING: {
                        remove: 'X' //移除圖示
                    },
                    accept: '<%=FileExtLimit %>' //副檔名限制
                });
            }
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
            <h2>商城輔圖 -
                <span class="styleRed B"><%=Param_ModelNo %></span>
            </h2>
        </div>
        <div class="SysTab3">
            <ucTab:Ascx_TabMenu ID="Ascx_TabMenu1" runat="server" />
        </div>
        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <tbody>
                    <tr>
                        <td class="TableModifyTdHead" style="width: 120px">圖片上傳<br />
                            (可多筆上傳)
                        </td>
                        <td class="TableModifyTd">
                            <div>
                                <asp:FileUpload ID="fu_Pic" runat="server" AllowMultiple="true" />
                            </div>
                            <div>
                                <asp:RequiredFieldValidator ID="rfv_fu_Pic" runat="server" ErrorMessage="請選擇要上傳的圖片"
                                    Display="Dynamic" ControlToValidate="fu_Pic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                            </div>
                            <div class="help-block">
                                (<code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>)
                            </div>
                        </td>
                        <td class="TableModifyTd">
                            <input type="button" id="triggerSave" class="btn btn-primary" value="開始上傳" />
                            <a href="<%=Session["BackListUrl"] %>" class="btn btn-default">返回列表</a>

                            <asp:Button ID="btn_doSave" runat="server" OnClick="btn_Upload_Click" ValidationGroup="Add" Style="display: none;" />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                <LayoutTemplate>
                    <div class="row">
                        <div class="col-xs-6 text-left">
                            <h4>圖片列表</h4>
                        </div>
                        <div class="col-xs-6 text-right">
                            <asp:Literal ID="lt_DownloadBtn" runat="server"></asp:Literal>
                            <asp:Button ID="btn_SaveList" runat="server" Text="儲存說明及排序" OnClick="btn_SaveList_Click"
                                ValidationGroup="List" CssClass="btn btn-success" />
                        </div>
                    </div>
                    <table class="List1 table" width="100%">
                        <tr class="tdHead">
                            <td width="5%">編號</td>
                            <td width="40%">圖片</td>
                            <td width="40%">說明</td>
                            <td width="10%">排序</td>
                            <td width="10%">&nbsp;</td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server">
                        <td align="center"><%#Eval("PMID") %></td>
                        <td align="center">
                            <a href="<%=Param_WebFolder %><%# Eval("PicFile")%>" class="PicGroup" rel="gallery">
                                <img src="<%=Param_WebFolder %><%# Eval("PicFile")%>" class="img-thumbnail img-responsive" alt="" />
                            </a>
                        </td>
                        <td align="center">
                            <div class="form-group">
                                <div class="col-xs-12">
                                    <asp:TextBox ID="tb_Desc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="7" MaxLength="500" placeholder="可填寫簡易說明,最多500字" Text='<%# Eval("PicDesc")%>'></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-xs-12">
                                    <input type="text" class="form-control url" value="<%=Param_WebFolder %><%# Eval("PicFile")%>" readonly="readonly" />
                                </div>
                            </div>
                        </td>
                        <td align="center">
                            <asp:TextBox ID="tb_Sort" runat="server" Text='<%# Eval("Sort")%>' Width="98%" CssClass="form-control text-center" type="number" min="1" max="999"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("PMID") %>' />
                            <asp:HiddenField ID="hf_FileName" runat="server" Value='<%#Eval("PicFile") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div style="margin: 0 auto; text-align: center" class="styleRed">
                        尚未新增圖片...
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>

    </form>
</body>
</html>
