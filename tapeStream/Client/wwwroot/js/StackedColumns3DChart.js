

var dotNetObject = {};


window.chart3DObject = {
    chart: {
        type: 'column',
        zoomType: 'x',
        backgroundColor: 'darkgray',
        panning: true,
        panKey: 'shift',
        options3d: {
            enabled: false,
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
            grouping: true,
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
        },
        plotBands:
            [
                {                               /// mark the weekend
                    color: '#FCFFC5',
                    from: 3,                    /// Date.UTC(2010, 0, 2),
                    to: 5,                      /// Date.UTC(2010, 0, 4),
                    label:
                    {
                        text: '',   // Content of the label. 
                        align: 'left',          // Positioning of the label. 
                        x: +20,                  // Amount of pixels the label will be repositioned according to the alignment. 
                        y: -10,                 // Amount of pixels the label will be repositioned according to the alignment.
                    }
                }
            ],
        plotLines: [{
            color: 'red', // Color value
            dashStyle: 'longdashdot', // Style of the plot line. Default to solid
            value: 3, // Value of where the line will appear
            width: 2 // Width of the line    
        }] 
    },

    yAxis: {
        allowDecimals: false,
        min: 0,
        max: 0,
        title: {
            text: 'Number of fruits',
            skew3d: true
        },
        //plotBands:
        //    [
        //        {                               /// mark the weekend
        //            color: '#FCFFC5',
        //            from: 0,                    /// Date.UTC(2010, 0, 2),
        //            to: 0,                      /// Date.UTC(2010, 0, 4),
        //            label:
        //            {
        //                text: 'I am a label',   // Content of the label. 
        //                align: 'left',          // Positioning of the label. 
        //                x: +10,                  // Amount of pixels the label will be repositioned according to the alignment. 
        //                y: +10,                 // Amount of pixels the label will be repositioned according to the alignment.
        //           }
        //        }
        //    ],
        //plotLines: [{
        //    color: 'red', // Color value
        //    dashStyle: 'longdashdot', // Style of the plot line. Default to solid
        //    value: 3, // Value of where the line will appear
        //    width: 2 // Width of the line    
        //}] 

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
        ]
}


//window.getChartSeriesObject = function (jsObject) {
//    dotNetObject.invokeMethodAsync('getChartSeriesObject', JSON.stringify(window.chart3DObject));
//};