"use strict";

window.loadHighchart = function (chartDivName, chartJson) {
    loadScript("https://code.highcharts.com/highcharts.js").then(
        function () {
            loadScript("https://code.highcharts.com/modules/annotations.js").then(
                function () {
                    loadScript("https://code.highcharts.com/stock/highstock.js").then(
                        function () {
                            loadScript("https://code.highcharts.com/modules/exporting.js").then(
                                function () {
                                    loadScript("https://code.highcharts.com/highcharts-3d.js").then(
                                        function () {
                                            waitForGlobal("Highcharts", function () {
                                                /// if window.chartObj is empty it's our first time rendering the chart

                                                /// if window.chartObj is empty it's our first time rendering the chart
                                                var isFirstTime = window.chartObj == undefined;
                                                //console.debug("window.loadHighchart => " + chartJson);

                                                /// turn json to js object (deserialize)
                                                window.chartObj = looseJsonParse(chartJson);
                                                if (isFirstTime) {
                                                    console.log("window.loadHighchart " + chartDivName);
                                                    console.table(window.chartObj);
                                                }
                                                    //console.table(window.chart);
                                                    /// render the chart
                                                    window.chart = Highcharts.chart(chartDivName, window.chartObj);
                                                    /// if more than one 3d on page, only first gets this
                                                    $(window.chart.container).on('mousedown.hc touchstart.hc', function (eStart) {
                                                        eStart = chart.pointer.normalize(eStart);

                                                        var posX = eStart.pageX,
                                                            posY = eStart.pageY,
                                                            alpha = chart.options.chart.options3d.alpha,
                                                            beta = chart.options.chart.options3d.beta,
                                                            newAlpha,
                                                            newBeta,
                                                            sensitivity = 5; // lower is more sensitive

                                                        //$(document).on({
                                                        //    'mousemove.hc touchdrag.hc': function (e) {
                                                        //        // Run beta
                                                        //        newBeta = beta + (posX - e.pageX) / sensitivity;
                                                        //        chart.options.chart.options3d.beta = newBeta;

                                                        //        // Run alpha
                                                        //        newAlpha = alpha + (e.pageY - posY) / sensitivity;
                                                        //        chart.options.chart.options3d.alpha = newAlpha;

                                                        //        chart.redraw(false);
                                                        //    },
                                                        //    'mouseup touchend': function () {
                                                        //        $(document).off('.hc');
                                                        //    }
                                                        //});
                                                    });
                                                //}
                                                //else
                                                //{
                                                //    chart.update(window.chartObj);
                                                //    //chart.series = (window.chartObj);
                                                //    //chart.series = 
                                                //    //window.updateHighchartSeries(Json.stringify(window.chartObj["series"]))
                                                //    chart.highcharts().redraw();

                                                //}

                                                SetLanguage();

                                                /// send the chart object back to the server
                                                /// (this should only be done once to initialize the cs chart object)
                                                //if (isFirstTime)
                                                if (isFirstTime) {
                                                    var json = JSON.stringify(window.chartObj);
                                                    console.log("window.getChartJson");
                                                    console.log(json);
                                                window.getChartJson(window.chartObj);
                                                    return json;
                                                }

                                            }
                                            )
                                        }, function () { })
                                }, function () { })
                        }, function () { })
                },
                function () { })
        },
        function () { })
}

window.Initialize = function (dotNetObj) {
    this.dotNetObject = dotNetObj;

    //var darkCss = Array.from(window.document.querySelectorAll('.darkreader')).map((n) => n.textContent).join('\n');
    //console.log("Darkreader css");
    //console.log(darkCss);
};

window.getChartSeriesJson = function (jsObject) {
    dotNetObject.invokeMethodAsync('getChartSeriesJson', JSON.stringify(window.chart3DObject.series));
};

window.getChartJson = function (windowObj) {
    dotNetObject.invokeMethodAsync('getChartJson', JSON.stringify(windowObj));
};

/// Replace the chart series 
window.updateHighchartSeries = function (seriesJson) {
    if (chart.series.length) {
        var series = looseJsonParse(seriesJson);
        for (var i = 0; i < series.length; i++) {
            /// replace the series
            chart.series[i].remove();
            chart.addSeries(series[i]);
        }
    }
}

/// Append the chart series
window.appendHighchartSeries = function (seriesJson, isShifted) {
    if (chart.series.length) {
        var series = looseJsonParse(seriesJson);

        /// Put the new point into the first value of the series
        /// Will not replace the actual value in the series
        for (var i = 0; i < series.length; i++) {
            /// append a point to the series
            chart.series[i].addPoint(series[i].data[0], true, isShifted);
        }
    }
}




function loadHighchart(n, t) {
    loadScript("https://code.highcharts.com/stock/highstock.js").then(function () {
        waitForGlobal("Highcharts", function () {
            var i = looseJsonParse(t);
            Highcharts.chart(n, i);
            SetLanguage()
        })
    }, function () { })
}

function looseJsonParse(n) { return Function('"use strict";return (' + n + ")")() }

function loadStockchart(n, t) {
    loadScript("https://code.highcharts.com/stock/highstock.js").then(
        function () {
            loadScript("https://code.highcharts.com/stock/modules/data.js").then(
                function () {
                    waitForGlobal("Highcharts",
                        function () {
                            var i = looseJsonParse(t);
                            Highcharts.stockChart(n, i);
                            SetLanguage()
                        })
                }, function () { })
        },
        function () { })
}

function load3Dchart(n, t) {
    loadScript("https://code.highcharts.com/highcharts.js").then(
        function () {
            loadScript("https://code.highcharts.com/modules/exporting.js").then(
                function () {
                    loadScript("https://code.highcharts.com/highcharts-3d.js").then(
                        function () {
                            waitForGlobal("Highcharts",
                                function () {
                                    var i = looseJsonParse(t);
                                    console.table(i);
                                    //var i = window.chart3DObject;
                                    //var series = looseJsonParse(t);
                                    //i.series = series;
                                    Highcharts.chart(n, i);
                                    SetLanguage()
                                })
                        }, function () { })
                },
                function () { })
        },
        function () { })
}

function setChart3Dseries(seriesJson, xAxisJson) {
    var series = looseJsonParse(seriesJson);
    var xAxis = looseJsonParse(xAxisJson);
    window.chart3DObject.series = series;
    window.chart3DObject.xAxis = xAxis;
}

function SetLanguage() {
    Highcharts.setOptions(
        {
            lang: {
                contextButtonTitle: "Diagram-meny",
                decimalPoint: ",",
                downloadJPEG: "Ladda ned JPEG-bild",
                downloadPDF: "Ladda ned PDF-dokument",
                downloadPNG: "Ladda ned PNG-bild",
                downloadSVG: "Ladda ned SVG-vektorgrafik",
                drillUpText: "Tillbaka till {series.name}",
                invalidDate: "",
                loading: "Laddar...",
                months: ["Januari", "Februari", "Mars", "April", "Maj", "Juni", "Juli", "Augusti", "September", "Oktober", "November", "December"],
                noData: "Ingen data att visa",
                numericSymbols: ["k", "M", "G", "T", "P", "E"],
                printChart: "Skriv ut diagram",
                resetZoom: "Återställ zoom",

                resetZoomTitle: "Återställ zoomnivå 1:1",
                shortMonths: ["Jan", "Feb", "Mar", "Apr", "Maj", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"],
                thousandsSep: " ",
                weekdays: ["Söndag", "Måndag", "Tisdag", "Onsdag", "Torsdag", "Fredag", "Lördag"],
                rangeSelectorZoom: "Zoom",
                rangeSelectorFrom: "Från",
                rangeSelectorTo: "Till"
            }
        })
}

function loadScript(n) {
    return new Promise(
        function (t, i) {
            for (var e, u, o = !1, f = document.getElementsByTagName("script"), r = 0; r < f.length; ++r)
                f[r].getAttribute("src") != null && f[r].getAttribute("src") == n && (o = !0, t());
            if (!o)
                for (e = [n], r = 0; r < e.length; r++)
                    u = document.createElement("script"),
                        u.src = e[r],
                        u.type = "text/javascript",
                        u.onload = t,
                        u.onerror = i,
                        u.async = !1,
                        u.charset = "utf-8",
                        document.getElementsByTagName("head")[0].appendChild(u)
        })
}

window.loadHighchart = loadHighchart;

var waitForGlobal = function waitForGlobal(n, t) {
    window[n] ? t() : setTimeout(function () { waitForGlobal(n, t) }, 100)
};

