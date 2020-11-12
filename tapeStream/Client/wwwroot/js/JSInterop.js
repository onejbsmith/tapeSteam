﻿"use strict";
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
                                    Highcharts.chart(n, i);
                                    SetLanguage()
                                })
                        }, function () { })
                },
                function () { })
        },
        function () { })
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
window.loadStockchart = loadStockchart;
window.load3Dchart = load3Dchart;

var waitForGlobal = function waitForGlobal(n, t) {
    window[n] ? t() : setTimeout(function () { waitForGlobal(n, t) }, 100)
};