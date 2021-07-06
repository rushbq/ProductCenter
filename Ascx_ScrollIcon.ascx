<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ascx_ScrollIcon.ascx.cs"
    Inherits="Ascx_ScrollIcon" %>
<%--捲軸按鈕 Start--%>
<link href="../js/goTop/goTop.css" rel="stylesheet" type="text/css" />
<script src="../js/goTop/goTop.js" type="text/javascript"></script>
<%--捲軸按鈕 End--%>
<!-- ScrollBar 浮動按鈕 start  -->
<div id="abgne_float_Top" style="background-color: #efefef; text-align: center; padding-bottom: 5px;">
    <span class="abgne_close_Top">
        <img src="../images/delete2.png" width="11" height="11" alt="關閉快捷列" title="關閉快捷列" /></span>
    <asp:Panel ID="ph_infoMsg" runat="server" Visible="false">
        <strong>(<%=Lang %>)</strong>
        <br />
        <b style="color: darkred"><%=ModelNo %></b>
        <hr />
    </asp:Panel>

    <asp:Panel ID="pl_Save" runat="server" Style="padding: 2px 10px 5px 5px">
        <a class="Font12 styleReddark doSave" style="cursor: pointer">
            <img src="../images/save.png" width="32" alt="儲存" title="儲存" /><br />
            儲&nbsp;&nbsp;存</a>
    </asp:Panel>
    <asp:Panel ID="pl_List" runat="server" Style="padding: 2px 10px 0px 5px">
        <a href="<%=Session["BackListUrl"] %>" class="Font11 styleBlack">
            <img src="../images/to_list.png" width="30" alt="回列表頁" title="回列表頁" /><br />
            回列表</a>
    </asp:Panel>
    <asp:Panel ID="pl_Top" runat="server" Style="padding: 6px 10px 0px 5px">
        <a class="gotoTop Font11 styleBlack" style="cursor: pointer">
            <img src="../images/go-top.png" width="30" alt="回頁首" title="回頁首" /><br />
            回頁首</a>
    </asp:Panel>
    <asp:Panel ID="pl_Bottom" runat="server" Style="padding: 6px 10px 0px 5px">
        <a class="gotoBottom Font11 styleBlack" style="cursor: pointer">
            <img src="../images/go-bottom.png" width="30" alt="至頁尾" title="至頁尾" /><br />
            至頁尾</a>
    </asp:Panel>
</div>
<!-- ScrollBar 浮動按鈕 End  -->
