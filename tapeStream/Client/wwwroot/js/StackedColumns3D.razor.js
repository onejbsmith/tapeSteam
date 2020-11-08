

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
            animation: false,
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
        max: 5000,
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
        },
        {
            name: 'Jane2',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane3',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane4',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane5',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane6',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane7',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane8',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane9',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane12',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane13',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane14',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane15',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane16',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane17',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }, {
            name: 'Jane18',
            data: [2, 5, 6, 2, 1],
            //color: 'limegreen',
            stack: 'female'
        }]
}

window.Initialize = function (dotNetObj) {
    this.dotNetObject = dotNetObj;
};

window.getChartSeriesJson = function (jsObject) {
    dotNetObject.invokeMethodAsync('getChartSeriesJson', JSON.stringify(window.chart3DObject.series));
};

window.getChartJson = function (jsObject) {
    dotNetObject.invokeMethodAsync('getChartJson', JSON.stringify(window.chart3DObject));
};

//window.getChartSeriesObject = function (jsObject) {
//    dotNetObject.invokeMethodAsync('getChartSeriesObject', JSON.stringify(window.chart3DObject));
//};