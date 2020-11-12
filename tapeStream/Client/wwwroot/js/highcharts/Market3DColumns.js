{
    chart: {
        type: 'column',
            zoomType: 'x',
                panning: true,
                    panKey: 'shift',
                        options3d: {
            enabled: true,
                alpha: 15,
                    beta: 15,
                        viewDistance: 25,
                            depth: 200
        }
    },

    plotOptions:
    {
        column:
        {
            stacking: 'normal',
                grouping: true,
                    depth: 100,
                        pointWidth: 30
        },
        series: {
            animation: true,
            //pointWidth: 200
        }
    },

    title: {
        text: 'Chart Title'
    },

    xAxis: {
        categories: ['Price 1', 'Price 2', 'Price 3', 'Price 4', 'Price 5'],
            title: {
            text: 'Price',
                skew3d: true
        },
        labels: {
            skew3d: true,
                style: {
                fontSize: '16px'
            }
        }
    },

    yAxis: {
        allowDecimals: false,
            min: 0,
                title: {
            text: 'Size',
                skew3d: true
        }
    },



    series:
    [{
        name: 'Series 1',
        data: [5, 3, 4, 7, 2],
        color: '#cb6992',
        stack: 'stack 1'
    },

    {
        name: 'Series 2',
        data: [3, 4, 4, 2, 5],
        color: '#0479cc',
        stack: 'stack 1'
    },

    {
        name: 'Series 3',
        data: [3, 0, 4, 4, 3],
        //color: 'crimson',
        stack: 'stack 2'
    },
    {
        name: 'Series 4',
        data: [2, 5, 6, 2, 1],
        //color: 'limegreen',
        stack: 'stack 2'
    }
    ]
}
