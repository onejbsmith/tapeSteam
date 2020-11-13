﻿

var dotNetObject = {};


window.chart3DObject = {
    chart: {
        type: 'column',
        options3d: {
            enabled: true,
            alpha: 20,
            beta: 30,
            depth: 400,
            panning: {
                enabled: true,
                type: 'xy'
            },
            panKey: 'shift',
            viewDistance: 50,
            frame: {
                bottom: {
                    size: 1,
                    color: 'rgba(0,0,0,0.05)'
                }
            }
        }
    },
    plotOptions: {
        series: {
            groupZPadding: 1,
            depth: 8,
            groupPadding: 0,
            grouping: false,
        }
    },


    title: {
        text: 'Chart Title 2'
    },
    subtitle: {
        text: 'Sub Title'
    },
    yAxis: {
        title: {
            text: 'Size'
        },
        min: 0,
        max: 10
    },
    xAxis: {
        title: {
            text: 'Price',
            skew3d: true
        },
        min: 0,
        max: 10
    },
    zAxis: {
        min: 0,
        max: 10,
        categories: ['A01', 'A02', 'A03', 'A04'],
        labels: {
            y: 5,
            rotation: 18
        }
    },



    series:
        [
            {
                data:
                    [
                        [0, 2], [1, 1], [2, 7], [3, 0]
                    ],
                color: '#FF0000'
            },
            {
                data: [
                    [4, 5], [5, 1],
                ]
            },
            {
                data: [
                    [6, 9]
                ]
            },
            {
                data: [
                    [6, 3], [7, 9], [8, 7], [9, 0]
                ]
            },

            {
                showInLegend: false,
                data: [
                    [0, 2], [1, 1], [2, 7], [3, 0]
                ], color: '#FF0000'
            },
            {
                showInLegend: false,
                data: [
                    [4, 5], [5, 1],
                ]
            },
            {
                showInLegend: false,
                data: [[6, 9]]
            },
            {
                showInLegend: false,
                data: [[6, 3], [7, 9], [8, 7], [9, 0]]
            },

            {
                showInLegend: false,
                data: [[0, 2], [1, 1], [2, 7], [3, 0]], color: '#FF0000'
            },
            {
                showInLegend: false, data: [[4, 5], [5, 1],]
            },
            {
                showInLegend: false, data:
                    [[6, 9]]
            },
            { showInLegend: false, data: [[6, 3], [7, 9], [8, 7], [9, 0]] },

            { showInLegend: false, data: [[0, 2], [1, 1], [2, 7], [3, 0]], color: '#FF0000' },
            { showInLegend: false, data: [[4, 5], [5, 1],] },
            { showInLegend: false, data: [[6, 9]] },
            { showInLegend: false, data: [[6, 3], [7, 9], [8, 7], [9, 0]] },

            { showInLegend: false, data: [[0, 2], [1, 1], [2, 7], [3, 0]], color: '#FF0000' },
            { showInLegend: false, data: [[4, 5], [5, 1],] },
            { showInLegend: false, data: [[6, 9]] },
            { showInLegend: false, data: [[6, 3], [7, 9], [8, 7], [9, 0]] },

            { showInLegend: false, data: [[0, 2], [1, 1], [2, 7], [3, 0]], color: '#FF0000' },
            { showInLegend: false, data: [[4, 5], [5, 1],] },
            { showInLegend: false, data: [[6, 9]] },
            { showInLegend: false, data: [[6, 3], [7, 9], [8, 7], [9, 0]] }
        ]
}




//window.getChartSeriesObject = function (jsObject) {
//    dotNetObject.invokeMethodAsync('getChartSeriesObject', JSON.stringify(window.chart3DObject));
//};