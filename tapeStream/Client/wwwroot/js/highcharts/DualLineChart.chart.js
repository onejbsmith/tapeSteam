{

    chart: {
        type: 'spline',
            scrollablePlotArea: {
            minWidth: 100
        }
    },

    title: {
        text: 'Daily sessions at www.highcharts.com'
    },

    subtitle: {
        text: 'Source: Google Analytics'
    },

    xAxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
            'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            gridLineWidth: 1,
                labels: {
            align: 'left',
                x: 3,
                    y: -3
        }
    },

    yAxis: [{ // left y axis
        title: {
            text: null
        },

        labels: {
            align: 'left',
            x: 3,
            y: 16,
            format: '{value:.,0f}'
        },
        showFirstLabel: false
    }, { // right y axis
        gridLineWidth: 0,
        opposite: true,
        title: {
            text: null
        },
        labels: {
            align: 'right',
            x: -3,
            y: 16,
            format: '{value:.,0f}'
        },
        showFirstLabel: false
    }],

        legend: {
        align: 'left',
            verticalAlign: 'top',
                borderWidth: 0
    },

    tooltip: {
        shared: true,
            crosshairs: true
    },

    plotOptions: {
        series: {
            cursor: 'pointer',
        }
    },

    series: [{
        name: 'All sessions',
        data: [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4],

        lineWidth: 4,
        yAxis: 0,
    },
    {
        name: 'New users',
        yAxis: 1,
        data: [1016, 1016, 1015.9, 1015.5, 1012.3, 1009.5, 1009.6, 1010.2, 1013.1, 1016.9, 1018.2, 1016.7],
    }]
}