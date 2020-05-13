<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Send.aspx.cs" Inherits="Prod_News_Send" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%>
    </title>
    <style type="text/css">
        /* Button Style Start
----------------------------------*/
        .btnBlock {
            display: inline-block;
            text-decoration: none;
            font-size: 12px;
            color: #fff;
            color: rgba(255,255,255,1);
            padding: 0.3em 1em;
            margin: 0.5em;
            border-style: solid;
            border-width: 1px;
            border-radius: 4px;
            box-shadow: 0 1px 1px rgba(255,255,255,0.5) inset;
            cursor: pointer;
            opacity: 0.85;
        }

            .btnBlock:active {
                box-shadow: 0px 0px 6px 0px rgba(0,0,0,0.4) inset;
                opacity: 1;
                color: rgba(255,255,255,0.5);
            }

            .btnBlock:hover {
                box-shadow: 0 1px 1px rgba(255,255,255,0.5) inset,0 0 2px rgba(0,0,0,0.2);
                opacity: 1;
            }

        .colorRed {
            background: #da4d42;
            background: rgba(218,77,66,1);
            border-color: #a44f36;
            border-color: rgba(164,79,54,1);
        }

        .colorBlue {
            background: #4297da;
            background: rgba(66,151,218,1);
            border-color: #3663a4;
            border-color: rgba(54,99,164,1);
        }

        .colorGreen {
            background: #73B839;
            background: rgba(115,184,57,1);
            border-color: #61a436;
            border-color: rgba(97,164,54,1);
        }

        .colorCoffee {
            background: #D2691E;
            background: rgba(210,105,30,1);
            border-color: #8B4513;
            border-color: rgba(139,69,19,1);
        }

        .colorDark {
            background: #808080;
            background: rgba(128, 128, 128,1);
            border-color: #404040;
            border-color: rgba(64, 64, 64,1);
        }

        .colorSilver {
            font-size: 12px;
            background: #f9f9f9;
            background: -moz-linear-gradient(top,#f9f9f9 0%,#eaeaea 100%);
            background: -webkit-gradient(linear,left top,left bottom,color-stop(0%,#f9f9f9),color-stop(100%,#eaeaea));
            background: -webkit-linear-gradient(top,#f9f9f9 0%,#eaeaea 100%);
            background: -o-linear-gradient(top,#f9f9f9 0%,#eaeaea 100%);
            background: -ms-linear-gradient(top,#f9f9f9 0%,#eaeaea 100%);
            background: linear-gradient(top,#f9f9f9 0%,#eaeaea 100%);
            border: 1px solid #c9c9c9;
            box-shadow: inset 0 1px 0 white,0 1px 2px rgba(0,0,0,0.1);
            color: #444;
            text-shadow: 0 1px 0 #fff;
        }

        .colorGray {
            background: #f2f2f2;
            background: rgba(0,0,0,0.05);
            border-color: #c0c0c0;
            border-color: rgba(192,192,192,0.8);
            color: #222;
        }

            .colorGray:hover {
                box-shadow: 0px 0px 6px 0px rgba(0,0,0,0.1) inset;
                color: #212121;
                opacity: 1;
            }
        /* Button Style End
----------------------------------*/
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 5px 15px 5px 15px; background: #EFEFEF; border-bottom: 1px solid #BEBEBE; height: 30px;">
            <div style="color: red; float: left;"><strong>此為預覽內容，確認後請按下「確認發信」</strong></div>
            <div style="float: right; text-align: right;">
                <asp:Button ID="btn_Send_Top" class="btnBlock colorRed" runat="server" Text="確認發信" OnClick="btn_Send_Top_Click" />
            </div>
        </div>

        <div style="clear: both; padding-top: 5px; padding-bottom: 5px;">
            <asp:Literal ID="lt_Container" runat="server"></asp:Literal>
        </div>

        <div style="padding: 5px 15px 5px 15px; background: #EFEFEF; border-top: 1px solid #BEBEBE; text-align: right;">
                <asp:Button ID="btn_Send_Bottom" class="btnBlock colorRed" runat="server" Text="確認發信" OnClick="btn_Send_Bottom_Click" />
        </div>
    </form>
</body>
</html>
