<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SampleStat.aspx.cs" Inherits="SampleStat" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="../css/layout.css?v=20160622" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>

    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>

    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <script type="text/javascript">
        $(function () {
            /* 日期選擇器 */
            $(".startDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $(".endDate").datepicker("option", "minDate", selectedDate);
                }
            });
            $(".endDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $(".startDate").datepicker("option", "maxDate", selectedDate);
                }
            });

        });
    </script>

    <!-- Google Chart Start -->
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        // Load the Visualization API and the piechart package.
        google.load('visualization', '1', { packages: ['corechart', 'table', 'bar'] });

    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            //填入預設日期(當年開始~今日)
            var thisDate = new Date();
            var defStartDate = thisDate.getFullYear() + '/01/01';
            var defEndDate = thisDate.getFullYear() + '/' + ('0' + (thisDate.getMonth() + 1)).slice(-2) + '/' + ('0' + thisDate.getDate()).slice(-2);
            $(".startDate").val(defStartDate);
            $(".endDate").val(defEndDate);

            /*
              [進入頁面後,載入預設資料] Start
            */

            //載入圖表 - 案件數(類別)
            SearchbyClass(defStartDate, defEndDate);

            /*
              [進入頁面後,載入預設資料] End
            */

            //Click事件 - 查詢
            $("#doSearch").on("click", function () {
                $("#pl_Msg").hide();

                //取得日期參數值
                var startDate = $(".startDate").val();
                var endDate = $(".endDate").val();

                //案件數(類別)
                SearchbyClass(startDate, endDate);
                //案件數(公司別)
                SearchbyComp(startDate, endDate);
                //案件數(負責人)
                SearchCountByWho(startDate, endDate);
            });


            //Tabs切換偵測, 並載入圖表
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                //取得判別編號
                var dataID = $(e.target).attr('data-id');
                //取得日期參數值
                var startDate = $(".startDate").val();
                var endDate = $(".endDate").val();

                //依編號載入圖表
                switch (dataID) {
                    case "1":
                        //案件數(類別)
                        SearchbyClass(startDate, endDate);
                        break;

                    case "2":
                        //案件數(公司別)
                        SearchbyComp(startDate, endDate);
                        break;

                    case "3":
                        //案件數(負責人)
                        SearchCountByWho(startDate, endDate);
                        break;

                }
            })

        })


        //-- 載入圖表 - 案件數(類別) --
        function SearchbyClass(StartDate, EndDate) {
            /* Pie圖 */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetCountbyClass',
                data: '{StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~' + EndDate;
                        //繪製圖表
                        drawChart_Pie(response.d, myTitle, 800, 500, 'Chart_Pie_byClass', '類別', '案件數');
                    }
            });
        }

        //-- 載入圖表 - 案件數(公司別) --
        function SearchbyComp(StartDate, EndDate) {
            /* Pie圖 - by部門(TW) */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetCountbyCompany',
                data: '{StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~ ' + EndDate;
                        //繪製圖表
                        drawChart_Pie(response.d, myTitle, 800, 500, 'Chart_Pie_byComp', '公司別', '案件數');
                    }
            });
        }

        //-- 載入圖表 - 案件數(負責人) --
        function SearchCountByWho(StartDate, EndDate) {
            /* Bar Chart (TW) */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetCountByWho',
                data: '{StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~' + EndDate;
                        //繪製圖表
                        drawChart_Bar(response.d, myTitle, 'BarChart_byWho', '人員', '案件數');
                    }
            });
        }


        //-- 繪製圖表 Start --
        /*
          [Pie Chart]
          dataValues : 資料值
          titleName : 圖表title
          width : 圖表寬度
          height : 圖表高度
          chartID : 圖表ID
          colName : 欄位名 - 類別
          valName : 欄位名 - 值

          ref:https://developers.google.com/chart/interactive/docs/gallery/piechart
        */
        function drawChart_Pie(dataValues, titleName, width, height, chartID, colName, valName) {
            var data = new google.visualization.DataTable();
            //新增欄
            data.addColumn('string', colName);
            data.addColumn('number', valName);

            //新增列值
            var total = 0;
            for (var i = 0; i < dataValues.length; i++) {
                data.addRow([dataValues[i].ColumnName, dataValues[i].Value]);

                //加總
                total = total + dataValues[i].Value;
            }

            //設定圖表選項
            var options = {
                'title': titleName,
                'width': width,
                'height': height,
                is3D: true
            };

            //載入圖表
            new google.visualization.PieChart(document.getElementById(chartID)).
                draw(data, options);

            //載入表格
            if (total > 0) {
                //新增合計列
                data.addRow(['Total', total]);
            }
            var table = new google.visualization.Table(document.getElementById('table_' + chartID));
            table.draw(data, { 'width': '400px' });

        }

        /*
          [Bar Chart]
          dataValues : 資料值
          titleName : 圖表title
          width : 圖表寬度
          height : 圖表高度
          chartID : 圖表ID
          colName : 欄位名 - 類別
          valName : 欄位名 - 值

          ref:https://developers.google.com/chart/interactive/docs/gallery/barchart
        */
        function drawChart_Bar(dataValues, titleName, chartID, colName, valName, valName1) {
            var data = new google.visualization.DataTable();
            //新增欄
            data.addColumn('string', colName);
            data.addColumn('number', valName);  //Value 1

            //新增列值
            var total1 = 0;
            for (var i = 0; i < dataValues.length; i++) {
                data.addRow([dataValues[i].ColumnName, dataValues[i].Value]);

                //加總
                total1 = total1 + dataValues[i].Value;
            }

            //計算高度
            // set inner height to 30 pixels per row
            var chartAreaHeight = data.getNumberOfRows() * 40;
            // add padding to outer height to accomodate title, axis labels, etc
            var chartHeight = chartAreaHeight + 200;

            //設定圖表選項
            //var options = {
            //    height: chartHeight,
            //    legend: { position: 'in' },
            //    chart: {
            //        title: titleName
            //        //,subtitle: 'popularity by percentage'
            //    },
            //    chartArea: { width: '100%', height: chartAreaHeight },
            //    bars: 'horizontal' // Required for Material Bar Charts.
            //};
            var options = {
                title: titleName,
                height: chartHeight,
                bar: { groupWidth: "95%" }
            };

            //載入圖表
            //ref:https://developers.google.com/chart/interactive/docs/gallery/barchart#loading
            //For Material Bar Charts, the visualization's class name is google.charts.Bar.
            //var chart = new google.charts.Bar(document.getElementById(chartID));
            //chart.draw(data, options);

            //一般barchart
            new google.visualization.BarChart(document.getElementById(chartID)).
               draw(data, options);

            if (total1 > 0) {
                //新增合計列
                data.addRow(['Total', total1]);
            }
            var table = new google.visualization.Table(document.getElementById('table_' + chartID));
            table.draw(data, { 'width': '400px' });

        }

        //-- 繪製圖表 End --
    </script>
    <!-- Google Chart End -->
    <script type="text/javascript">
        $(function () {
            $("#doExport").click(function () {
                $("#pl_Msg").hide();

                var sd = $(".startDate").val();
                var ed = $(".endDate").val();

                $("#tb_SDate").val(sd);
                $("#tb_EDate").val(ed);

                if (sd == '' || ed == '') {
                    alert('請填入日期區間');
                    return false;
                }

                //export
                $("#btn_Export").trigger("click");
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>產品檢驗登記</a>&gt;<span>統計</span>
        </div>
        <div class="h2Head">
            <h2>統計</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>日期區間：
                    <input type="text" class="startDate styleBlack" style="text-align: center; width: 90px;" />
                    ~
                    <input type="text" class="endDate styleBlack" style="text-align: center; width: 90px;" />
                    &nbsp;&nbsp;&nbsp;
                    <input type="button" id="doSearch" class="btn btn-default" value="查詢" />
                    <input type="button" id="doExport" class="btn btn-success" value="匯出" />
                </li>
            </ul>
        </div>
        <asp:Panel ID="pl_Msg" runat="server" Visible="false">
            <div class="alert alert-danger">
                <h4>查無資料，請重新選擇區間。</h4>
            </div>
        </asp:Panel>
        <div style="display: none;">
            <asp:TextBox ID="tb_SDate" runat="server"></asp:TextBox>
            <asp:TextBox ID="tb_EDate" runat="server"></asp:TextBox>
            <asp:Button ID="btn_Export" runat="server" Text="export" OnClick="btn_Export_Click" />
        </div>
        <!-- Tabs Start -->
        <div style="margin-top: 10px;">
            <ul id="myTabs" class="nav nav-tabs" role="tablist">
                <li class="active"><a href="#CountByClass" role="tab" data-toggle="tab" data-id="1">類別</a></li>
                <li><a href="#CountByComp" role="tab" data-toggle="tab" data-id="2">公司別</a></li>
                <li><a href="#CountByWho" role="tab" data-toggle="tab" data-id="3">負責人</a></li>
            </ul>
            <div class="tab-content">
                <!-- 案件數(類別) -->
                <div class="tab-pane fade in active" id="CountByClass">
                    <div class="bq-callout blue">
                        <h4>案件數統計(依類別)</h4>
                        <div class="form-group">
                            <div id="Chart_Pie_byClass"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_Chart_Pie_byClass"></div>
                        </div>
                    </div>
                </div>

                <!-- 案件數(公司別) -->
                <div class="tab-pane fade" id="CountByComp">
                    <div class="bq-callout green">
                        <h4>案件數統計(依公司別)</h4>
                        <div class="form-group">
                            <div id="Chart_Pie_byComp"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_Chart_Pie_byComp"></div>
                        </div>
                    </div>
                </div>

                <!-- 案件數統計(依負責人) -->
                <div class="tab-pane fade" id="CountByWho">
                    <div class="bq-callout red">
                        <h4>案件數統計(依負責人)</h4>
                        <div class="form-group">
                            <div id="BarChart_byWho" style="margin: 20px 20px;"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_BarChart_byWho"></div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        <!-- Tabs End -->

    </form>
</body>
</html>
