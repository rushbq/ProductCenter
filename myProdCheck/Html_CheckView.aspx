<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Html_CheckView.aspx.cs" Inherits="myProdCheck_Html_CheckView" EnableViewState="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查檢表</title>
    <style>
        .myTable td {
            border: 1px solid;
        }

        body {
            font-family: Calibri;
            font-size: 12px;
        }

        .setBg {
            background-color: #94ffff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- A4橫印 -->
        <table align="center" border="0" cepllpadding="0" cellspacing="0" style="width: 100%">
            <tr>
                <td valign="top">
                    <!-- 表頭 -->
                    <table border="0" cepllpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td colspan="8" align="center">
                                <h2>寶工實業股份有限公司&nbsp;&nbsp;&nbsp;&nbsp;外驗查檢表</h2>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="8" height="20">&nbsp;</td>
                        </tr>
                        <tr>
                            <td width="60" align="right">採購單號：</td>
                            <td width="110">
                                <asp:Literal ID="lt_ErpID" runat="server"></asp:Literal>
                            </td>
                            <td width="60" align="right">供應商：</td>
                            <td width="220">
                                <asp:Literal ID="lt_Vendor" runat="server"></asp:Literal>
                            </td>
                            <td width="60" align="right">工廠地址：</td>
                            <td>
                                <asp:Literal ID="lt_Address" runat="server"></asp:Literal>
                            </td>
                            <td width="60" align="right">檢驗日期：</td>
                            <td width="80">&nbsp;<!-- 空 -->
                            </td>

                        </tr>
                        <tr>
                            <td align="right">產品料號：</td>
                            <td>
                                <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                            </td>
                            <td align="right">品名規格：</td>
                            <td>
                                <asp:Literal ID="lt_ModelName" runat="server"></asp:Literal>
                            </td>
                            <td align="right">數量：</td>
                            <td>&nbsp;<!-- 空 -->
                            </td>
                            <td align="right">查驗標準：</td>
                            <td>&nbsp;<!-- 空 -->
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <!-- 內容區塊1 -->
                    <table class="myTable" style="width: 100%; text-align: center; border-collapse: collapse;">
                        <tr>
                            <td rowspan="2" class="setBg">項<br />
                                次
                            </td>
                            <td class="setBg">查 驗 內 容</td>
                            <td colspan="20" class="setBg">編&nbsp;&nbsp;&nbsp;&nbsp;號</td>
                        </tr>
                        <tr>
                            <td width="350" class="setBg">內 部 包 材 / 出 貨 成 品 本 體</td>
                            <%=Get_EmptyColumn(20,true) %>
                        </tr>


                        <!-- 檢驗內容 Data -->
                        <asp:Literal ID="lt_ItemContent" runat="server"></asp:Literal>
                        <!-- 檢驗內容 額外多 3 列空白 -->
                        <%for (int idx = 0; idx < 3; idx++)
                          { %>
                        <tr>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <%=Get_EmptyColumn(20, false)%>
                        </tr>
                        <%} %>

                        <!-- 固定內容 1 -->
                        <tr>
                            <td colspan="2" class="setBg">內 盒 查 驗</td>
                            <td colspan="20" class="setBg">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>A</td>
                            <td align="left">成品內盒需註明型號及數量</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                        <tr>
                            <td>B</td>
                            <td align="left">使用透明膠帶封內盒</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                        <tr>
                            <td>C</td>
                            <td align="left">其他：</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>


                        <!-- 固定內容 2 -->
                        <tr>
                            <td colspan="2" class="setBg">外 箱 查 驗</td>
                            <td colspan="20" class="setBg">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>A</td>
                            <td align="left">外箱正、側嘜頭、型號、數量需書寫正確、清楚</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                        <tr>
                            <td>B</td>
                            <td align="left">未裝滿箱需妥善加物固定，不可鬆動</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                        <tr>
                            <td>C</td>
                            <td align="left">膠帶使用透明膠帶封"工"字型</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                        <tr>
                            <td>D</td>
                            <td align="left">外箱需依照規定打"井"字型4條打包帶</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                        <tr>
                            <td>E</td>
                            <td align="left">其他：</td>
                            <%=Get_EmptyColumn(20,false) %>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <table width="100%">
                        <tr>
                            <td height="40" style="vertical-align: top; padding-left: 20px;">標示方式： O - 表示合格&nbsp;&nbsp;&nbsp;&nbsp;X - 表示不合格

                            </td>
                            <td rowspan="2" style="width: 50%; border: 1px solid; vertical-align: top;">備註：</td>
                        </tr>
                        <tr>
                            <td height="40" style="vertical-align: top; padding-left: 20px;">正嘜：&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                側嘜：
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td style="vertical-align: top;">
                                <table border="0" cepllpadding="0" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td style="width: 25%;">最終判定：</td>
                                        <td style="width: 25%;">審核：</td>
                                        <td style="width: 25%;">供應商：</td>
                                        <td style="width: 25%;">填表：</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <table border="0" cepllpadding="0" cellspacing="0" style="width: 100%; line-height: 40px; font-size: 18px; margin-top: 20px;">
                                    <tr>
                                        <td style="width: 100px; text-align: right;">到廠時間：</td>
                                        <td style="width: 80px; text-align: right;">年</td>
                                        <td style="width: 80px; text-align: right;">月</td>
                                        <td style="width: 80px; text-align: right;">日</td>
                                        <td>&nbsp;&nbsp;&nbsp;時間：</td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: right;">離廠時間：</td>
                                        <td style="text-align: right;">年</td>
                                        <td style="text-align: right;">月</td>
                                        <td style="text-align: right;">日</td>
                                        <td>&nbsp;&nbsp;&nbsp;時間：</td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">供應商人員：</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
