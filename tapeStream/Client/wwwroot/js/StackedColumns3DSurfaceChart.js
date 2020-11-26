
{
    chart: {
        type: 'column',
        //height: 600,
        backgroundColor: 'darkgray',
            animation: false,
       options3d: {
            enabled: true,
            alpha: 15,
            beta: 15,
            depth: 600,
            panning: {
                enabled: true,
                type: 'xy'
            },
            panKey: 'shift',
            viewDistance: 500,
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
            animation: false,
                groupZPadding: 1,
                    depth: 25,
                        groupPadding: 0,
                            grouping: false,
        },
        column: {
            colorByPoint: true,
                //stacking: 'normal',
            //    animation: {  
            //    defer: 0
            //    duration: 0
            //}

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
            max: 10,
                plotBands:
        [
            //{                               /// mark the weekend
            //    color: '#FCFFC5',
            //    from: 3,                    /// Date.UTC(2010, 0, 2),
            //    to: 5,                      /// Date.UTC(2010, 0, 4),
            //    label:
            //    {
            //        text: '',   // Content of the label. 
            //        align: 'left',          // Positioning of the label. 
            //        x: +20,                  // Amount of pixels the label will be repositioned according to the alignment. 
            //        y: -10,                 // Amount of pixels the label will be repositioned according to the alignment.
            //        useHTML: false
            //    }
            //}
        ],
            plotLines: [
                //{
                //color: 'red', // Color value
                //dashStyle: 'longdashdot', // Style of the plot line. Default to solid
                //value: 3, // Value of where the line will appear
                //width: 2 // Width of the line    
                //}
            ]
    },
    xAxis: {
        categories: [],
            title: {
            text: 'Price',
        },
        min: 0,
            max: 10,

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
                    useHTML: false
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
    zAxis: {
        reversed: true,
      min: 0,
            max: 100,

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
        {
            data: [{ x: 0, y: 5, z: 0, color: '#BF0B23' }],
            selected: true
        }

    ],

}
