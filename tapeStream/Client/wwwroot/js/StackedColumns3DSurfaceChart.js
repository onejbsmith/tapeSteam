
{
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
        },
        column: {
            colorByPoint: true,
                //colors: [
                //    '#ff0000',
                //    '#ff0000',
                //    '#ff0000',
                //    '#ff0000',
                //    '#00ff00',

                //    '#00ff00',
                //    '#00ff00',
                //    '#0000ff',
                //    '#0000ff',
                //    '#0000ff',

                //    '#00ff00',
                //    '#00ff00',
                //    '#0000ff',
                //    '#0000ff',
                //    '#0000ff',

                //    '#ff0000',
                //    '#ff0000',
                //    '#ff0000',
                //    '#ff0000',
                //    '#00ff00',
                //]
        }
    },


    title: {
        text: 'Chart Title 2 el'
    },
    subtitle: {
        text: 'Sub Title'
    },
    yAxis: {
        title: {
            text: 'Y-Axis'
        },
        min: 0,
            max: 10
    },
    xAxis: {
        title: {
            text: 'Price',
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
        },
        title: {
            text: 'Time',
        },
    },



    series:

    [
        //{
        //    data: [{ x: 0, y: 5, color: '#BF0B23' }]
        //},
        {
            data:
                [
                    [0, 2], [1, 1], [2, 7], [3, 0], [4, 5],
                    [5, 1], [6, 9], [7, 9], [8, 7], [9, 0]
                ],

        },

        {
            data: [
                [0, 2], [1, 1], [2, 7], [3, 0], [4, 5],
                [5, 1], [6, 3], [7, 1], [8, 2], [9, 3]
            ]
        },

        {
            data: [[0, 2], [1, 1], [2, 7], [3, 0], [4, 5],
            [5, 1], [6, 3], [7, 9], [8, 7], [9, 0]
            ]
        },


        {
            data: [{x:0, y: 5, color: '#BF0B23' }, [1, 1], [2, 7], [3, 0], [4, 5],
            [5, 1], [6, 3], [7, 9], [8, 7], [9, 0]
            ]
        },


        {
            showInLegend: false, data: [[0, 2], [1, 1], [2, 7], [3, 0], [4, 5],
            [5, 1], [6, 9], [7, 9], [8, 7], [9, 0]]
        },

        {
            showInLegend: false, data: [[0, 2], [1, 1], [2, 7], [3, 0], [4, 5],
            [5, 1], [6, 9], [7, 9], [8, 7], [9, 0]]
        },
    ]
}
