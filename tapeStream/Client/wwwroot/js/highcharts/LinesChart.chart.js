{
    chart: {
        zoomType: 'xy',
            animation: false

    },
    title: {
        text: null,
            align: 'left'
    },
    subtitle: {
        text: '!',
            align: 'left'
    },
    xAxis: [{
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
            'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        crosshair: true
    }],
        yAxis: [{ // Primary yAxis
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            title: {
                text: 'Size or Ratio',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            opposite: false

        },
            //{ // Secondary yAxis
            //gridLineWidth: 0,
            //title: {
            //    text: 'Sells',
            //    style: {
            //        color: Highcharts.getOptions().colors[0]
            //    }
            //},
            //labels: {
            //    format: '{value}',
            //    style: {
            //        color: Highcharts.getOptions().colors[0]
            //    }
            //}

            //},

            //{ // Tertiary yAxis
            //gridLineWidth: 0,
            //title: {
            //    text: 'Mark',
            //    style: {
            //        color: Highcharts.getOptions().colors[1]
            //    }
            //},
            //labels: {
            //    format: '{value}',
            //    style: {
            //        color: Highcharts.getOptions().colors[1]
            //    }
            //},
            //opposite: true
            //}
        ],
            tooltip: {
        shared: true
    },
    legend: {
        layout: 'vertical',
            align: 'left',
                x: 80,
                    verticalAlign: 'top',
                        y: 55,
                            floating: true,
                                backgroundColor: Highcharts.defaultOptions.legend.backgroundColor || 'rgba(255,255,255,0.25)'
    },
    series: [{
        name: 'Sells',
        type: 'spline',
        yAxis: 0,
        data: [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4],
        marker: {
            enabled: true
        },
        tooltip: {
            valueSuffix: ''
        }

    },
    {
        showInLegend: false,
        name: 'Mark',
        type: 'spline',
        yAxis: 0,
        data: [],
        marker: {
            enabled: true
        },
        dashStyle: 'shortdot',
        tooltip: {
            valueSuffix: ''
        }

    }
        , {
        name: 'Buys',
        type: 'spline',
        data: [7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6],
        marker: {
            enabled: true
        },
        tooltip: {
            valueSuffix: ''
        }
    }],
        plotOptions: {
        series: {
            animation: false,
                line: {
                pointInterval: 10000
            }

        }
    },
    responsive: {
        rules: [{
            condition: {
                maxWidth: 500
            },
            chartOptions: {
                legend: {
                    floating: false,
                    layout: 'horizontal',
                    align: 'center',
                    verticalAlign: 'bottom',
                    x: 0,
                    y: 0
                },
                yAxis: [{
                    labels: {
                        align: 'right',
                        x: 0,
                        y: -6
                    },
                    showLastLabel: false
                }, {
                    labels: {
                        align: 'left',
                        x: 0,
                        y: -6
                    },
                    showLastLabel: false
                }, {
                    visible: false
                }]
            }
        }]
    }
}