<%@ Page Title="" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5" style="min-height: 550px;">
            <div class="ui segments">
                <div class="ui grey segment">
                    <h5 class="ui header">產品快查&nbsp;<small class="blue-text text-darken-2">(按「Enter鍵」或「查詢鈕」皆可執行)</small>
                    </h5>
                </div>
                <div class="ui small form attached segment">
                    <div id="searchSection" class="ui action left icon input">
                        <i class="search icon"></i>
                        <input type="text" id="keyword" placeholder="輸入關鍵字, 查詢品號或品名" maxlength="40" autocomplete="off" />
                        <button class="ui button" type="button" id="doSearch">查詢</button>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script type="text/javascript">
        function EnterClick(e) {
            // 偵測enter
            if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;
            if (e.which == 13) {
                // Submit按鈕
                $("#doSearch").trigger("click");
                return false;
            }
        }

        document.onkeypress = EnterClick;
    </script>
    <script>
        //init
        $("#keyword").focus();

        //觸發查詢
        $("#doSearch").click(function () {
            //載入Loading
            var btn = $(this);
            btn.parent().addClass('loading');

            //取得輸入值
            var keyword = $("#keyword").val();

            //導向Url
            location.replace('Product/Prod_Search.aspx?Model_No=' + encodeURIComponent(keyword));
        });

    </script>
</asp:Content>

