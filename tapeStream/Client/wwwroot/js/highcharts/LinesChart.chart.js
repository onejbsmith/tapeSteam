{
    chart: {
        zoomType: 'xy',
            animation: false,

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
        categories: [],
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

            { // Tertiary yAxis
            gridLineWidth: 1,
            title: {
                text: 'Mark',
                style: {
                    color: 'forestgreen'
                }
            },
            labels: {
                format: '{value}',
                style: {
                    color: 'forestgreen'
                }
            },
            opposite: true
            }
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
        data: [],
        marker: {
            enabled: false
        },
        tooltip: {
            valueSuffix: ''
        }

    },
    {
        //showInLegend: false,
        name: 'Mark',
        type: 'spline',
        yAxis: 1,
        color: 'forestgreen',
        data: [],
        marker: {
            enabled: false
        },
        dashStyle: 'shortdot',
        tooltip: {
            valueSuffix: ''
        }

    }
        , {
        name: 'Buys',
        type: 'spline',
        data: [],
        marker: {
            enabled: false
        },
        tooltip: {
            valueSuffix: ''
        }
    },
    {
        showInLegend: false,
        name: 'fourth',
        type: 'spline',
        yAxis: 0,
        data: [],
        marker: {
            enabled: false
        },
        dashStyle: 'shortdot',
        tooltip: {
            valueSuffix: ''
        }

        },
        {
            showInLegend: false,
            name: 'fifth',
            type: 'spline',
            yAxis: 0,
            data: [],
            marker: {
                enabled: false
            },
            dashStyle: 'shortdot',
            tooltip: {
                valueSuffix: ''
            }

        },
        {
            showInLegend: false,
            name: 'sixth',
            type: 'spline',
            yAxis: 0,
            data: [],
            marker: {
                enabled: false
            },
            dashStyle: 'shortdot',
            tooltip: {
                valueSuffix: ''
            }
        }
    ],
        plotOptions: {
        series: {
            lineWidth: 4,
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