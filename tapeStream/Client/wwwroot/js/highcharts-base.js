"use strict";

window.chartInDiv = {};
window.chartObj = {};
window.chart = {};
window.dotNetObject = {};
window.chartDataUrls = {};
window.chartColNames = {};
window.dataRequests = {};

window.tracing = false;



window.loadHighchart = function (chartDivName, chartJson, redrawChart) {
    loadScript("https://code.highcharts.com/highcharts.js").then(
        function () {
            loadScript("https://code.highcharts.com/modules/annotations.js").then(
                function () {
                    loadScript("https://code.highcharts.com/highcharts-more.js").then(
                        function () {
                            loadScript("https://code.highcharts.com/modules/data.js").then(
                                function () {
                                    loadScript("https://code.highcharts.com/highcharts-3d.js").then(
                                        function () {
                                            waitForGlobal("Highcharts", function () {
                                                /// if window.chartObj is empty it's our first time rendering the chart
                                                //window.tracing = chartDivName == "LinesChart";
                                                //if (chartDivName =="LinesChart")
                                                //    debugger;
                                                if (window.tracing) {
                                                    console.warn(`0. ${chartDivName} window.loadHighchart`);
                                                    //if (chartDivName == "ClockGauge")
                                                    //    debugger;
                                                    console.warn(chartJson);
                                                }

                                                /// if window.chartObj is empty it's our first time rendering the chart
                                                var isFirstTime = window.chartObj[chartDivName] == undefined;
                                                //console.debug("window.loadHighchart => " + chartJson);

                                                /// turn json to js object (deserialize)
                                                window.chartObj[chartDivName] = looseJsonParse(chartJson);

                                                if (isFirstTime && window.tracing) {
                                                    console.log(`1. ${chartDivName} window.loadHighchart`);
                                                    console.table(window.chartObj[chartDivName]);
                                                }


                                                if (redrawChart == false || isFirstTime == true) {
                                                    //console.table(window.chart);
                                                    /// render the chart
                                                    window.chart[chartDivName] = Highcharts.chart(chartDivName, window.chartObj[chartDivName]);
                                                    chartInDiv = $("#" + chartDivName).highcharts();

                                                    /// if more than one 3d on page, only first gets this
                                                    $(window.chart[chartDivName].container).on('mousedown.hc touchstart.hc', function (eStart) {
                                                        eStart = chart[chartDivName].pointer.normalize(eStart);

                                                        var posX = eStart.pageX,
                                                            posY = eStart.pageY,
                                                            alpha = chart[chartDivName].options.chart.options3d.alpha,
                                                            beta = chart[chartDivName].options.chart.options3d.beta,
                                                            newAlpha,
                                                            newBeta,
                                                            sensitivity = 5; // lower is more sensitive

                                                        $(document).on({
                                                            'mousemove.hc touchdrag.hc': function (e) {
                                                                // Run beta
                                                                newBeta = beta + (posX - e.pageX) / sensitivity;
                                                                chart[chartDivName].options.chart.options3d.beta = newBeta;

                                                                // Run alpha
                                                                newAlpha = alpha + (e.pageY - posY) / sensitivity;
                                                                chart[chartDivName].options.chart.options3d.alpha = newAlpha;

                                                                chart[chartDivName].redraw(false);
                                                            },
                                                            'mouseup touchend': function () {
                                                                $(document).off('.hc');
                                                            }
                                                        });
                                                    });

                                                    //SetLanguage();

                                                }
                                                else {
                                                    chartInDiv = $("#" + chartDivName).highcharts();


                                                    //if (chartDivName == "Surface3D") {
                                                    //    for (var i = 0; i < chart["Surface3D"].series.length; i++) {
                                                    //        chartInDiv.series[i].data.options = window.chartObj["Surface3D"].series[i].data;
                                                    //    }
                                                    //    chartInDiv.redraw();
                                                    //    //chartInDiv.redraw();
                                                    //    //chart["Surface3D"] = chartInDiv;
                                                    //    //chart["Surface3D"].redraw();

                                                    //}
                                                    //else {
                                                    //debugger;
                                                    //var series = window.chartObj.series;
                                                    //for (var i = 0; i < series.length; i++)
                                                    //{
                                                    //    chartInDiv.series[i].setData(series[i].data, false);
                                                    //};
                                                    //chartInDiv.redraw();
                                                    if (window.tracing) {
                                                        console.log(`2. ${chartDivName} chartInDiv`);
                                                        console.table(chartInDiv);
                                                    }

                                                    //var axes = chartInDiv.axes;

                                                    chartInDiv.update(window.chartObj[chartDivName]);

                                                    if (window.tracing) {
                                                        console.log(`3. ${chartDivName} chartInDiv`);
                                                        console.table(chartInDiv);
                                                    }


                                                    //chartInDiv.axes = axes;

                                                    //chartInDiv.series = window.chartObj.series;

                                                    //chartInDiv.series.update(window.chartObj.series);
                                                    //chart.series = (window.chartObj);
                                                    //chart.series = 
                                                    //window.updateHighchartSeries(Json.stringify(window.chartObj["series"]))
                                                    // debugger;

                                                    //chartInDiv.redraw();
                                                    ////}
                                                    //chartInDiv.reflow();
                                                    //chartInDiv.xAxis[0].setExtremes(0, chartInDiv.xAxis[0].categories.length);


                                                }


                                                /// send the chart object back to the server
                                                /// (this should only be done once to initialize the cs chart object)
                                                //if (isFirstTime)
                                                if (isFirstTime) {
                                                    var json = JSON.stringify(window.chartObj[chartDivName]);
                                                    if (window.tracing)
                                                        console.log(json);
                                                    window.getChartJson(chartDivName, json, window.tracing);
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
//window.loadHighchartRequestData = function (chartDivName, chartJson, chartDataUrl, redrawChart) {

//}

window.loadHighchartRequestData = function (chartDivName, chartJson, chartDataUrl, redrawChart) {
    loadScript("https://code.highcharts.com/highcharts.js").then(
        function () {
            loadScript("https://code.highcharts.com/modules/annotations.js").then(
                function () {
                    loadScript("https://code.highcharts.com/highcharts-more.js").then(
                        function () {
                            loadScript("https://code.highcharts.com/modules/data.js").then(
                                function () {
                                    loadScript("https://code.highcharts.com/highcharts-3d.js").then(
                                        function () {
                                            waitForGlobal("Highcharts", function () {
                                                /// if window.chartObj is empty it's our first time rendering the chart
                                                //window.tracing = chartDivName == "LinesChart";
                                                //if (chartDivName =="LinesChart")
                                                //    debugger;
                                                if (window.tracing) {
                                                    console.warn(`0. ${chartDivName} window.loadHighchart`);
                                                    //if (chartDivName == "ClockGauge")
                                                    //    debugger;
                                                    console.warn(chartJson);
                                                }

                                                /// if window.chartObj is empty it's our first time rendering the chart
                                                var isFirstTime = window.chartObj[chartDivName] == undefined;
                                                //console.debug("window.loadHighchart => " + chartJson);

                                                /// turn json to js object (deserialize)
                                                window.chartObj[chartDivName] = looseJsonParse(chartJson);

                                                if (isFirstTime && window.tracing) {
                                                    console.log(`1. ${chartDivName} window.loadHighchart`);
                                                    console.table(window.chartObj[chartDivName]);
                                                }


                                                if (redrawChart == false || isFirstTime == true) {
                                                    //console.table(window.chart);
                                                    /// render the chart
                                                    window.chart[chartDivName] = Highcharts.chart(chartDivName, window.chartObj[chartDivName]);

                                                    //debugger;
                                                    window.chartDataUrls[chartDivName] = chartDataUrl;

                                                    // get the column names from the Url
                                                    const urlParts = chartDataUrl.split("=");
                                                    const colNamesString = urlParts[urlParts.length - 1];
                                                    const colNames = colNamesString.split(",");
                                                    window.chartColNames[chartDivName] = colNames;

                                                    //SetLanguage();
                                                    window.requestData(chartDivName);
                                                    //window.addEventListener('load', function () {
                                                    //    chartInDiv = $("#" + chartDivName).highcharts();
                                                    //}

                                                    /// if more than one 3d on page, only first gets this
                                                    $(window.chart[chartDivName].container).on('load', function (eStart) {
                                                        eStart = chart[chartDivName].pointer.normalize(eStart);

                                                        var posX = eStart.pageX,
                                                            posY = eStart.pageY,
                                                            alpha = chart[chartDivName].options.chart.options3d.alpha,
                                                            beta = chart[chartDivName].options.chart.options3d.beta,
                                                            newAlpha,
                                                            newBeta,
                                                            sensitivity = 5; // lower is more sensitive

                                                        $(document).on({
                                                            'mousemove.hc touchdrag.hc': function (e) {
                                                                // Run beta
                                                                newBeta = beta + (posX - e.pageX) / sensitivity;
                                                                chart[chartDivName].options.chart.options3d.beta = newBeta;

                                                                // Run alpha
                                                                newAlpha = alpha + (e.pageY - posY) / sensitivity;
                                                                chart[chartDivName].options.chart.options3d.alpha = newAlpha;

                                                                chart[chartDivName].redraw(false);
                                                            },
                                                            'mouseup touchend': function () {
                                                                $(document).off('.hc');
                                                            }
                                                        });
                                                    });

                                                }
                                                else {
                                                    chartInDiv = $("#" + chartDivName).highcharts();


                                                    //if (chartDivName == "Surface3D") {
                                                    //    for (var i = 0; i < chart["Surface3D"].series.length; i++) {
                                                    //        chartInDiv.series[i].data.options = window.chartObj["Surface3D"].series[i].data;
                                                    //    }
                                                    //    chartInDiv.redraw();
                                                    //    //chartInDiv.redraw();
                                                    //    //chart["Surface3D"] = chartInDiv;
                                                    //    //chart["Surface3D"].redraw();

                                                    //}
                                                    //else {
                                                    //debugger;
                                                    //var series = window.chartObj.series;
                                                    //for (var i = 0; i < series.length; i++)
                                                    //{
                                                    //    chartInDiv.series[i].setData(series[i].data, false);
                                                    //};
                                                    //chartInDiv.redraw();
                                                    if (window.tracing) {
                                                        console.log(`2. ${chartDivName} chartInDiv`);
                                                        console.table(chartInDiv);
                                                    }

                                                    //var axes = chartInDiv.axes;

                                                    chartInDiv.update(window.chartObj[chartDivName]);

                                                    if (window.tracing) {
                                                        console.log(`3. ${chartDivName} chartInDiv`);
                                                        console.table(chartInDiv);
                                                    }


                                                    //chartInDiv.axes = axes;

                                                    //chartInDiv.series = window.chartObj.series;

                                                    //chartInDiv.series.update(window.chartObj.series);
                                                    //chart.series = (window.chartObj);
                                                    //chart.series = 
                                                    //window.updateHighchartSeries(Json.stringify(window.chartObj["series"]))
                                                    // debugger;

                                                    //chartInDiv.redraw();
                                                    ////}
                                                    //chartInDiv.reflow();
                                                    //chartInDiv.xAxis[0].setExtremes(0, chartInDiv.xAxis[0].categories.length);


                                                }


                                                /// send the chart object back to the server
                                                /// (this should only be done once to initialize the cs chart object)
                                                //if (isFirstTime)
                                                if (isFirstTime) {
                                                    var json = JSON.stringify(window.chartObj[chartDivName]);
                                                    if (window.tracing)
                                                        console.log(json);
                                                    window.getChartJson(chartDivName, json, window.tracing);
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


window.requestData = async function (chartDivName, chartDataUrl = "") {

    if (chartDataUrl.length > 0)
        window.chartDataUrls[chartDivName] = chartDataUrl;

    const url = window.chartDataUrls[chartDivName];

    const fetchPromise = fetch(url);

    fetchPromise.then(response => {
        return response.json();
    }).then(dataArray => {

        if (dataArray.length > 0)
            AddPoints(chartDivName, dataArray);
        //console.log(dataArray);
        //debugger;
    });
    //console.log(fetchPromise);

    // call it again after one second
    // setTimeout(window.requestData, 1000);
}

function AddPoints(chartDivName, dataArray) {
    //debugger;
    const nFrames = dataArray.length;
    const series = chart[chartDivName].series;
    if (series[0].data.length == 0) {
        SetPoints(chartDivName, dataArray);
    }
    else {
        AddPoint(chartDivName, dataArray);
    }
    //ResetPlotBands(chartDivName, dataArray);
    //ResetYAxisMax(chartDivName, dataArray);
}

function AddPoint(chartDivName, dataArray) {
    const firstCol = 4;
    const colNames = window.chartColNames[chartDivName];
    const nFrames = dataArray.length;
    const series = chart[chartDivName].series;

    var shift = false;
    if (series != undefined && series[0].data != undefined)
        shift = series[0].data.length > 300; // shift if the series is longer than 20

    for (var n = 0; n < nFrames; n++) {

        const data = dataArray[n];
        const date = data["DateTime"]
        const x = new Date(date).getTime();

        // Skip the first 4 column names, they are key data
        // Values start in column 5
        for (var iCol = firstCol; iCol < colNames.length; iCol++) {
            var iSeries = iCol - firstCol;

            // console.log(chartDivName + `.series[${iSeries}].data.length=${series[iSeries].data.length}`);

            const y = Number.parseFloat(data[colNames[iCol]]);
            const point = [x, y];
            //debugger;
            try {
                if (series[iSeries].data.length == 0)
                    series[iSeries].setData(point);
                else
                    series[iSeries].addPoint(point, true, shift);
            }
            catch (err) {
                // debugger;

            }
        }
        //console.log("-----------------------------------------------------------")
        //chart[chartDivName].redraw();
    }

}

function NewAddPoint(chartDivName, dataArray) {
    const firstCol = 4;
    const colNames = window.chartColNames[chartDivName];
    const series = chart[chartDivName].series;
    var redraw = true;
    var shift = false;
    //debugger;
    if (series != undefined && series[0].data != undefined)
        shift = series[0].data.length > 300; // shift if the series is longer than 20


    const nSeries = colNames.length - firstCol;
    var points = [];
    for (var i = 0; i < nSeries; i++)  points[i] = [];

    /// Build array of points for columns for each series

    const data = dataArray[0];
    const date = data["DateTime"];
    const x = new Date(date).getTime();

    for (var iCol = firstCol; iCol < colNames.length; iCol++) {
        var iSeries = iCol - firstCol;
        const y = Number.parseFloat(data[colNames[iCol]]);
        const point = [x, y];
        points[iSeries].push(point);
    }


    /// set all series data
    for (var k = 0; k < nSeries; k++) {
        try {
            chart[chartDivName].series[k].addPoint(points[k], false, shift);
        }
        catch (err) {
            //debugger;
        }
    }
    debugger;
    chart[chartDivName].redraw();
    //chart[chartDivName].update();

}

function SetPoints(chartDivName, dataArray) {


    const firstCol = 4;
    const colNames = window.chartColNames[chartDivName];
    const series = chart[chartDivName].series;


    const nSeries = colNames.length - firstCol;
    var points = [];
    for (var i = 0; i < nSeries; i++)  points[i] = [];

    const nFrames = dataArray.length;

    /// Build array of points for columns for each series
    for (var n = 0; n < nFrames; n++) {
        const data = dataArray[n];
        const date = data["DateTime"];
        const x = new Date(date).getTime();

        for (var iCol = firstCol; iCol < colNames.length; iCol++) {
            var iSeries = iCol - firstCol;
            const y = Number.parseFloat(data[colNames[iCol]]);
            const point = [x, y];
            points[iSeries].push(point);
        }
    }

    /// set all series data
    for (var k = 0; k < nSeries; k++) {
        try {
            chart[chartDivName].series[k].setData(points[k]);
        }
        catch (err) {
            //debugger;
        }
    }

    /// make sure chart uses local time
    Highcharts.setOptions({
        global: {
            useUTC: false
        }
    });

    //UpdateChartDataUrl(chartDivName);
}

function UpdateChartDataUrl(chartDivName) {
    /// Remove time element from dataUrl
    /// This transforms it from an initial pull of all day's records to the second
    /// into a pull of the latest frame, which the setTimeout will perform every second

    const url = window.chartDataUrls[chartDivName];
    //debugger;
    // split the url into sections
    var saUrl = url.split("?").join(";").split("&").join(";").split(";");
    // remove the sections with "Time" in them

    var saNewUrl = [];
    for (var i = 0; i < saUrl.length; i++) {
        if (!(saUrl[i].includes("toDate") || saUrl[i].includes("fromDate"))) {
            saNewUrl.push(saUrl[i]);
        }
    }
    // rejoin the sections
    var sUrl = saNewUrl.join("&");

    // replace the first & with "?".
    sUrl = sUrl.replace('&', '?');

    // update the url
    window.chartDataUrls[chartDivName] = sUrl;
}

/// Note that all the charts get the same plotBands so
/// should only calc once per request
/// BUT charts are not aware of one another!!!
function ResetPlotBands(chartDivName, dataArray) {

    /// Gather arrays of even minute start and end times
    const startTimes = [];
    const endTimes = [];

    const series = chart[chartDivName].series[0];

    const nFrames = series.data.length;

    var prevX = new Date(dataArray[0]["DateTime"]).getTime();
    var prevMinute = 0;
    for (var n = 0; n < nFrames; n++) {
        const data = series.data[n];
        const date = data.x;
        const dateTime = new Date(date);
        const x = dateTime.getTime();

        var currentMinute = dateTime.getMinutes();

        if (currentMinute != prevMinute) {
            if (currentMinute % 2 == 0) {
                // starting an even minute
                startTimes.push(x);
            }
            else { // new odd minute (ends prev even minute)
                endTimes.push(prevX);
            }

        }
        prevMinute = currentMinute;
        prevX = x;
    }
    if (startTimes.length - endTimes.length == 1) // means ended in middle of an even minute
    {
        endTimes.push(prevX);                     // so add last second as end of the even minute
    }

    //    debugger;
    //debugger;
    // Update chart plotbands
    chart[chartDivName].xAxis[0].plotBands = [];
    for (var i = 0; i < startTimes.length; i++) {
        chart[chartDivName].xAxis[0].addPlotBand({
            from: startTimes[i],
            to: endTimes[i],
            color: 'lavender',
            id: 'plot-band ' + i
        });
    }



}

window.syncedCharts = {};
window.yMax = 0;
/// Keep Y-Axis max in sync for PrintsBelow and PrintsAbove
function ResetYAxisMax(chartDivName, dataArray) {
    /// Get the buysField name, (first column name)
    const firstCol = 4;
    const colNames = window.chartColNames[chartDivName];
    const buysFieldName = colNames[firstCol];
    if (buysFieldName == "BuysBelow" || buysFieldName == "BuysAbove") {
        window.syncedCharts[chartDivName] = chart[chartDivName];

        const nFrames = dataArray.length;
        for (var n = 0; n < nFrames; n++) {
            const data = dataArray[n];
            // only test cols 0 and 2 ( i.e. skip Mark)
            for (var iCol = firstCol; iCol < firstCol + 2; iCol += 2) {
                const y = Number.parseFloat(data[colNames[iCol]]);
                window.yMax = Math.max(y, window.yMax);
            }
        }
        // next 1000 after max
        window.Max = 1000 * Math.ceil(yMax / 1000);

        ///// Update both charts
        //debugger;
        //for (const [key, value] of Object.entries(window.syncedCharts)) {

        //    value.yAxis[1].max = window.yMax;
        //}
    }
}


//window.setCsvDataContent = function (pre_Id, text) {
//    var dataPre = window.document.getElementById(pre_Id);
//    dataPre.innerHTML = text;
//}


//const result = await fetch('https://demo-live-data.highcharts.com/time-rows.json');

// 'url' looks like this
//https://localhost:44363/api/Frames/getFramesWholeColumns/QQQ/30?fromDateTime=2020-12-01-1200-00%20&columnNames=Id,DateTime,Symbol,Seconds,BuysTradeSizes,MarkPrice,SellsTradeSizes,BollingerLow,BollingerMid,BollingerHigh


// 'data' looks like this
//[
//    {
//0        Id: 341207,
//1        DateTime: "2020-12-01T12:00:00.223",
//2        Symbol: "QQQ",
//3        Seconds: 30,
//4        BuysTradeSizes: 34836,
//        SellsTradeSizes: 26151,
//        MarkPrice: 303.585,
//        BollingerLow: 308.51486809647747,
//        BollingerMid: 309.0557,
//        BollingerHigh: 309.59653190352253
//    }
//]

window.Initialize = function (dotNetObj, chartDivName) {
    window.dotNetObject[chartDivName] = dotNetObj;
    if (window.tracing)
        console.log(`0. ${chartDivName} window.Initialize`);




    //var darkCss = Array.from(window.document.querySelectorAll('.darkreader')).map((n) => n.textContent).join('\n');
    //console.log("Darkreader css");
    //console.log(darkCss);
};

window.setChartHeight = function (newHeight) {
    chart.chartHeight = newHeight;
    chart.update();
}

//window.getChartSeriesJson = function (jsObject) {
//    dotNetObject.invokeMethodAsync('getChartSeriesJson', JSON.stringify(window.chart3DObject.series));
//};

window.getChartJson = function (chartDivName, json) {
    if (window.tracing) {
        console.log(`3. ${chartDivName} window.getChartJson`);
        console.table(json);
    }
    window.dotNetObject[chartDivName].invokeMethodAsync('getChartJson', json);

};

/// Replace the chart series 
//window.updateHighchartSeries = function (seriesJson) {
//    if (chart.series.length) {
//        var series = looseJsonParse(seriesJson);
//        for (var i = 0; i < series.length; i++) {
//            chart.series.unshift(series[i]);
//            chart.redraw();
//            //debugger;
//            //chart.addSeries(series[i].data);
//            //debugger;

//        }
//        chart.update();
//        //chart.redraw();
//        //chart.reflow();
//    }
//}

///// Append the chart series
//window.appendHighchartSeries = function (chartDivName, newDataJson, isShifted) {
//    var newDataPoints = looseJsonParse(newDataJson);
//    var targetSeries = window.chart[chartDivName].series;
//    //var isShifted = targetSeries[0].data.length >= maxDataLength;
//    if (newDataPoints.length <= targetSeries.length) {

//        /// Put the new point into the first value of the series
//        /// Will not replace the actual value in the series
//        for (var i = 0; i < newDataPoints.length; i++) {
//            /// append a point to the series
//            targetSeries[i].addPoint(newDataPoints[i], false, isShifted);
//        }
//        window.chart[chartDivName].redraw();
//    }
//}

//function addSeries() {
//    if (chart.series.length === 1) {
//        chart.addSeries({
//            data: [194.1, 95.6, 54.4, 29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4]
//        });
//    }
//}

function loadHighchart(n, t) {
    loadScript("https://code.highcharts.com/stock/highstock.js").then(function () {
        waitForGlobal("Highcharts", function () {
            var i = looseJsonParse(t);
            Highcharts.chart(n, i);
            SetLanguage()
        })
    }, function () { })
}

function looseJsonParse(n) {
    try {
        return Function('"use strict";return (' + n + ")")()
    }
    catch (err) {
        //alert(n);
        //debugger;
    }
}

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
                contextButtonTitle: "Chart menu",
                decimalPoint: ".",
                downloadJPEG: "Download JPEG image",
                downloadPDF: "Download PDF document",
                downloadPNG: "Download PNG image",
                downloadSVG: "Download SVG Vector Graphics",
                drillUpText: "Back to {series.name}",
                invalidDate: "",
                loading: "Loading ...",
                months: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                noData: "No data to display",
                numericSymbols: ["k", "M", "G", "T", "P", "E"],
                printChart: "Print Chart",
                resetZoom: "Reset zoom",

                resetZoomTitle: "Reset zoom level 1: 1",
                shortMonths: ["Jan", "Feb", "Mar", "Apr", "Maj", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec"],
                thousandsSep: "",
                weekdays: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
                rangeSelectorZoom: "Zoom",
                rangeSelectorFrom: "Off",
                rangeSelectorTo: "On"
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

var waitForGlobal = function waitForGlobal(n, t) {
    window[n] ? t() : setTimeout(function () { waitForGlobal(n, t) }, 100)
};

window.loadHighchart = loadHighchart;

/// Utils not needed with JsConsole component
window.Dump = function (dumpJson, dumpName) {
    window.dumpObj = looseJsonParse(dumpJson);
    console.groupCollapsed(dumpName);
    console.table(window.dumpObj);
    console.groupEnd();
}

window.Confirm = function (message) {
    return window.confirm(message);
}

window.GroupTable = function (dumpJson, dumpName) {
    window.dumpObj = looseJsonParse(dumpJson);
    console.groupCollapsed(dumpName);
    console.table(window.dumpObj);
    console.groupEnd();
}

window.Table = function (objectJson) {
    window.dumpObj = looseJsonParse(objectJson);
    console.table(window.dumpObj);
}

window.Group = function (groupLabel) {
    console.groupCollapsed(groupLabel);
}

window.EndGroup = function () {
    console.groupEnd();
}

window.Log = function (message) {
    console.log(message);
}

/////============================================
//const container = document.getElementById('container');

//// create some buttons to test the resize logic
//const up = document.createElement('button');
//up.innerText = '+';
//up.addEventListener('click', () => {
//    chartWidth *= 1.1;
//    chartHeight *= 1.1;
//    chart.setSize(chartWidth, chartHeight);
//});
//container.before(up);

//const down = document.createElement('button');
//down.innerText = '-';
//down.addEventListener('click', () => {
//    chartWidth *= 0.9;
//    chartHeight *= 0.9;
//    chart.setSize(chartWidth, chartHeight);
//});
//container.before(down);

//const orig = document.createElement('button');
//orig.innerText = '1:1';
//orig.addEventListener('click', () => {
//    chartWidth = origChartWidth;
//    chartHeight = origChartHeight;
//    chart.setSize(origChartWidth, origChartHeight);
//});
//container.before(orig);

//$('.resizer').resizable({
//    // On resize, set the chart size to that of the
//    // resizer minus padding. If your chart has a lot of data or other
//    // content, the redrawing might be slow. In that case, we recommend
//    // that you use the 'stop' event instead of 'resize'.
//    resize: function () {
//        chart.setSize(
//            this.offsetWidth - 20,
//            this.offsetHeight - 20,
//            false
//        );
//        chart.update();
//    }
//});

//document.getElementById('plain').addEventListener('click', () => {
//    chart.update({
//        chart: {
//            inverted: false,
//            polar: false
//        },
//        subtitle: {
//            text: 'Plain'
//        }
//    });
//});

//document.getElementById('button').addEventListener('click', () => {
//    chart.xAxis[0].setExtremes(0, 5);
//});

//document.getElementById('button').addEventListener('click', () => {
//    chart.xAxis[0].setExtremes(Date.UTC(2010, 0, 2), Date.UTC(2010, 0, 8));
//});

//document.getElementById('button').addEventListener('click', () => {
//    var yAxis = chart.yAxis[0];

//    yAxis.options.startOnTick = false;
//    yAxis.options.endOnTick = false;

//    chart.yAxis[0].setExtremes(40, 210);
//});

//document.getElementById('button').addEventListener('click', () => {
//    chart.xAxis[0].setCategories(['J', 'F', 'M', 'A', 'M', 'J', 'J', 'A', 'S', 'O', 'N', 'D']);
//});

//document.getElementById('button').addEventListener('click', () => {
//    if (chart.series.length) {
//        chart.series[0].remove();
//    }
//});

//document.getElementById('button').addEventListener('click', e => {
//    chart.addSeries({
//        data: [216.4, 194.1, 95.6, 54.4, 29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5]
//    });

//    e.target.disabled = true;
//});


var clickCount = 0;

function renderjQueryComponents() {
    //$("#accordion").accordion();
    $(".jquery-btn button").button();
    $(".jquery-btn button").click(function () {
        console.log('Clicked');
        $('.click-count')[0].innerText = ++clickCount;
    });

    $('.resizer').resizable({
        // On resize, set the chart size to that of the
        // resizer minus padding. If your chart has a lot of data or other
        // content, the redrawing might be slow. In that case, we recommend
        // that you use the 'stop' event instead of 'resize'.
        resize: function () {
            chart.setSize(
                this.offsetWidth - 20,
                this.offsetHeight - 20,
                false
            );
            chart.update();
        }
    });
}