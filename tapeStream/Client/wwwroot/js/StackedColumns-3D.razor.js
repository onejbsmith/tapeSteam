

var dotNetObject = {};


window.chart3DObject = {
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
            depth: 100
        }
    },

    plotOptions:
    {
        column:
        {
            stacking: 'normal',
            grouping: false,
            depth: 100
        },
        series: {
            animation: true,
            //pointWidth: 200
        }
    },

    title: {
        text: 'Total fruit consumption, grouped by gender'
    },

    xAxis: {
        categories: ['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas'],
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
        max: 10000,
        title: {
            text: 'Number of fruits',
            skew3d: true
        }
    },



    series:
        [{
            name: 'John',
            data: [5, 3, 4, 7, 2],
            color: '#cb6992',
            stack: 'male'
        },

        {
            name: 'Joe',
            data: [3, 4, 4, 2, 5],
            color: '#0479cc',
            stack: 'male'
        },

        {
            name: 'Janet',
            data: [3, 0, 4, 4, 3],
            //color: 'crimson',
            stack: 'female'
        },
        {
            name: 'Jane',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }]
}



//window.getChartSeriesObject = function (jsObject) {
//    dotNetObject.invokeMethodAsync('getChartSeriesObject', JSON.stringify(window.chart3DObject));
//};