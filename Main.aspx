<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["Web_Name"]%></title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.js"></script>
    <%-- bootstrap Start --%>
    <script src="js/bootstrap/js/bootstrap.min.js"></script>
    <link href="js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            //tooltip
            $('[data-toggle=tooltip]').tooltip();

            $("#keyword").focus();

            //搜尋鈕
            $("#goSearch").click(function () {
                //載入Loading
                var btn = $(this);
                btn.button('loading');

                //取得輸入值
                var keyword = $("#keyword").val();

                //導向Url
                location.replace('Product/Prod_Search.aspx?Model_No=' + encodeURIComponent(keyword));
            });


        });
    </script>
    <%-- bootstrap End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="SearhIndex">
            <div class="SearhHeadIndex"></div>
            <div class="SearchShift">
                <div class="col-lg-6">
                    產品快查
                    <div class="input-group" data-toggle="tooltip" data-placement="bottom" title="請輸入關鍵字:品號/品名">
                        <input type="text" class="form-control" id="keyword" maxlength="40" placeholder="請輸入關鍵字:品號/品名">
                        <span class="input-group-btn">
                            <button class="btn btn-default" type="button" id="goSearch" data-loading-text="Loading...">Go!</button>
                        </span>
                    </div>
                </div>
            </div>
        </div>

    </form>
    <script language="javascript" type="text/javascript">
        function EnterClick(e) {
            // 這一行讓 ie 的判斷方式和 Firefox 一樣。
            if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;

            if (e.which == 13) {
                // Submit按鈕
                $("#goSearch").trigger("click");
                return false;
            }
        }

        document.onkeypress = EnterClick;
    </script>
</body>
</html>
