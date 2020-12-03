{

    chart: {
        type: 'gauge',
            backgroundColor: 'transparent',
            plotBackgroundColor: null,
                plotBackgroundImage: null,
                    plotBorderWidth: 0,
                        plotShadow: true,
                            height: '150px'
    },

    credits: {
        enabled: false
    },

    title: {
        text: ''
    },

    pane: {
        background: [{
            // default background
        }, {
            // reflex for supported browsers
            backgroundColor: Highcharts.svg ? {
                radialGradient: {
                    cx: 0.5,
                    cy: -0.4,
                    r: 2.2
                },
                stops: [
                    [0.5, 'rgba(255, 255, 255, 0.2)'],
                    [0.5, 'rgba(200, 200, 200, 0.2)']
                ]
            } : null
        }]
    },

    yAxis: {
        labels: {
            distance: -12
        },
        min: 0,
            max: 12,
                lineWidth: 0,
                    showFirstLabel: false,

                        minorTickInterval: 'auto',
                            minorTickWidth: 1,
                                minorTickLength: 5,
                                    minorTickPosition: 'inside',
                                        minorGridLineWidth: 0,
                                            minorTickColor: '#666',

                                                tickInterval: 1,
                                                    tickWidth: 2,
                                                        tickPosition: 'inside',
                                                            tickLength: 5,
                                                                tickColor: '#666',
                                                                    title: {
            text: '',
                style: {
                color: '#BBB',
                    fontWeight: 'normal',
                        fontSize: '8px',
                            lineHeight: '10px'
            },
            y: 10
        }
    },

    tooltip: {
        formatter: function () {
            return this.series.chart.tooltipText;
        }
    },

    series: [{
        data: [{
            id: 'hour',
            y: ((new Date()).getHours() + (new Date()).getMinutes() / 60),
            dial: {
                radius: '55%',
                baseWidth: 4,
                baseLength: '75%',
                rearLength: 0
            }
        }, {
            id: 'minute',
                y: ((new Date()).getMinutes() * 12 / 60 + (new Date()).getSeconds() * 12 / 3600),
            dial: {
                baseLength: '60%',
                rearLength: 0
            }
        }, {
            id: 'second',
                y:( (new Date()).getSeconds() * 12 / 60),
                color:'red',
            dial: {
                radius: '80%',
                baseWidth: 1,
                rearLength: '20%'
            }
        }],
        animation: false,
        dataLabels: {
            enabled: false
        }
    }]
}