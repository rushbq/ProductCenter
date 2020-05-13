﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <link rel="shortcut icon" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/ProductCenter-icon.ico" />
    <link rel="apple-touch-icon" sizes="57x57" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-57.png" />
    <link rel="apple-touch-icon" sizes="72x72" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-72.png" />
    <link rel="apple-touch-icon" sizes="76x76" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-76.png" />
    <link rel="apple-touch-icon" sizes="114x114" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-114.png" />
    <link rel="apple-touch-icon" sizes="120x120" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-120.png" />
    <link rel="apple-touch-icon" sizes="144x144" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-144.png" />
    <link rel="apple-touch-icon" sizes="152x152" href="<%=Application["RefUrl"] %>WebIcon/ProductCenter/Icon-152.png" />
</head>
<frameset rows="80,*">
	<frame id="TopFrame" name="TopFrame" scrolling="no" noresize="noresize" target="contents" src="Top.aspx">
	<frameset id="DownFrame" name="DownFrame" cols="176,*">
		<frame id="LeftMenuFirst" name="LeftMenuFirst" src="Left.aspx" target="main">
		<frame id="mainFrame" name="mainFrame" src="Main.aspx?t=<%=Request.QueryString["t"] %>&ModelNo=<%=Request.QueryString["ModelNo"] %>&dataID=<%=Request.QueryString["dataID"] %>">
	</frameset>
	<noframes>
	<body>

	<p>此網頁使用框架，但是您的瀏覽器不支援框架。</p>

	</body>
	</noframes>
</frameset>
</html>
