<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Spec_Frame_SpecClass.aspx.cs"
    Inherits="ProdSpec_Spec_Frame_SpecClass" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>項目關聯 </title>
</head>
<frameset id="frameset1" rows="*" cols="*,250">
    <frame src="Spec_Rel_SpecClass.aspx?SpecClass=<%=Request["SpecClass"] %>" id="leftSpec" name="leftSpec">
    <frame src="Spec_Tree_SpecClass.aspx" id="rightSpec" name="rightSpec">
</frameset>
</html>