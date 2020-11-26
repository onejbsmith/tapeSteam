{
    chart: {
        type: 'column'
    },
    title: {
        text: 'Live Data (Rows JSON)'
    },

    subtitle: {
        text: 'Data input from a remote JSON file'
    },

    data: {
        rowsURL: 'https://demo-live-data.highcharts.com/time-rows.json',
            firstRowAsNames: false,
                enablePolling: true
    }
}