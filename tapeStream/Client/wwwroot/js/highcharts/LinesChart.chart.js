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
        gridLineWidth: 1,
        crosshair: true,
        labels: {
            style: {
                fontSize: "15px"
            }
        },
        type: "datetime",
        
    }],
        yAxis: [

            { // Tertiary yAxis
                gridLineWidth: 1,
                title: {
                    text: 'Mark',
                    style: {
                        color: '#555',
                        fontSize: "15px"
                    }
                },
                labels: {
                    format: '{value}',
                    style: {
                        color: '#555',
                        fontSize: "15px"
                    }
                },
                opposite: true,
                plotLines: [
                    {
                        color: 'red', // Color value
                        dashStyle: 'shortdot', // Style of the plot line. Default to solid
                        value: 0, // Value of where the line will appear
                        width: 0 // Width of the line    
                    }
                ]
            },
            { // Primary yAxis
                labels: {
                    format: '{value}',
                    style: {
                        color: Highcharts.getOptions().colors[0],
                        fontSize: "15px"
                    }
                },
                title: {
                    text: null,
                    style: {
                        color: Highcharts.getOptions().colors[0],
                        fontSize: "15px"
                    }
                },
                opposite: true

            },
            { // Secondary yAxis
            gridLineWidth: 0,
            title: {
                text: null,
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            },
            labels: {
                format: '{value}',
                style: {
                    color: Highcharts.getOptions().colors[1]
                }
            }

            },


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
    series: [

        {
        name: 'Buys',
        type: 'spline',
        yAxis: 1,
        data: [],
        marker: {
            enabled: false
        },
        dashStyle: 'shortdash',
        tooltip: {
            valueSuffix: ''
        }

    },
    {
        //showInLegend: false,
        name: 'Mark',
        type: 'spline',
        yAxis: 0,
        color: '#555',
        data: [],
        marker: {
            enabled: false
        },
        tooltip: {
            valueSuffix: ''
        }

    }
        , {
        name: 'Sells',
        type: 'spline',
        data: [],
        yAxis: 1,
        marker: {
            enabled: false
        },
        dashStyle: 'shortdash',
        tooltip: {
            valueSuffix: ''
        }
    },
    {
        showInLegend: false,
        name: 'high',
        type: 'spline',
        yAxis: 2,
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
        name: 'mid',
        type: 'spline',
        yAxis: 2,
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
        name: 'low',
        type: 'spline',
        yAxis: 2,
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