<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Prod_SOP_Upload.aspx.cs"
    Inherits="Prod_SOP_Upload" %>

<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- 多筆上傳 Start --%>
    <script src="../js/multiFile/jquery.MultiFile.pack.js" type="text/javascript"></script>
    <%-- 多筆上傳 End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //多筆上傳
            $('#fu_File').MultiFile({
                STRING: {
                    remove: '<img src="../images/cancel.png" alt="x" border="0" width="10" />' //移除圖示
                },
                accept: '<%=FileExtLimit %>' //副檔名限制
            });

        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="h2Head">
        <h2>
            <%=Param_ModelNo%></h2>
    </div>
    <div class="Sift">
        <ul>
            <li>
                <asp:FileUpload ID="fu_File" runat="server" />
                <asp:RequiredFieldValidator ID="rfv_fu_File" runat="server" ErrorMessage="-&gt; 請選擇要上傳的檔案"
                    ForeColor="Red" Display="Dynamic" ControlToValidate="fu_File" ValidationGroup="Add"></asp:RequiredFieldValidator>
            </li>
            <li>
                <asp:Button ID="btn_Upload" runat="server" Text="上傳檔案" ValidationGroup="Add" OnClick="btn_Upload_Click"
                    CssClass="btnBlock colorBlue" />
                <asp:ValidationSummary ID="vs_Add" runat="server" ShowMessageBox="true" ShowSummary="false"
                    ValidationGroup="Add" />
                <input onclick="parent.$.fancybox.close();" type="button" value="關閉視窗" class="btnBlock colorGray" />
                <span class="SiftLight">(副檔名上傳限制：<%=FileExtLimit.Replace("|",", ")%>)</span>
            </li>
        </ul>
    </div>
    <div>
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead">
                        <td>
                            檔案
                        </td>
                        <td width="100px">
                            建立日期
                        </td>
                        <td width="100px">
                            功能選項
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server">
                    <td valign="top">
                        <a href="<%#Application["WebUrl"] %>FileDownload.ashx?OrgiName=<%#Server.UrlEncode(Eval("SOP_OrgFile").ToString()) %>&FilePath=<%#Server.UrlEncode(Cryptograph.Encrypt(Param_FileFolder + Eval("SOP_File").ToString())) %>">
                            <%#Eval("SOP_OrgFile").ToString()%></a>
                    </td>
                    <td align="center">
                        <%#Eval("Create_Time").ToString().ToDateString("yyyy-MM-dd")%>
                    </td>
                    <td align="center">
                        <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                            OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                        <asp:Literal ID="lt_SID" runat="server" Text='<%#Eval("SID") %>' Visible="false"></asp:Literal>
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div style="padding: 60px 0px 60px 0px; text-align: center">
                    <span style="color: #FD590B; font-size: 12px">未新增或無任何符合資料！</span>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>
    </div>
    <div style="height: 50px;">
    </div>
    </form>
</body>
</html>
