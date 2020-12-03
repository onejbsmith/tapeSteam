{
    chart: {
        zoomType: 'xy'
    },

    title: {
        text: 'Price Against Buys and Sells Ratios 8',
            align: 'left'
    },
    subtitle: {
        text: '', // This will be a date
            align: 'left'
    },
    xAxis: [{
        categories: [], /// These will be times
        crosshair: true
    }],
        yAxis: [{ // Tertiary yAxis
            gridLineWidth: 0,
            title: {
                text: 'Mark Price',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            },
            labels: {
                format: '${value}',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            },
            opposite: true
        }, { // Primary yAxis
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[2]
                }
            },
            title: {
                text: 'Buys Ratio',
                style: {
                    color: Highcharts.getOptions().colors[2]
                }
            },
            opposite: true

        }, { // Secondary yAxis
            gridLineWidth: 0,
            title: {
                text: 'Sells Ratio',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            }

        }],
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
                                backgroundColor:
        Highcharts.defaultOptions.legend.backgroundColor || // theme
            'rgba(255,255,255,0.25)'
    },
    series: [{
        name: 'Sells Ratio',
        type: 'spline',
        yAxis: 1,
        data: [],
        tooltip: {
            valueSuffix: ' '
        }

    },
    {
        name: 'Mark Price',
        type: 'line',
        data: [],
        marker: {
            enabled: false
        },
        dashStyle: 'shortdot',
        tooltip: {
            valueSuffix: ' '
        }

    },

    {
        name: 'Buys Ratio',
        type: 'spline',
        yAxis: 2,
        data: [],
        tooltip: {
            valueSuffix: ''
        }
    }],
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