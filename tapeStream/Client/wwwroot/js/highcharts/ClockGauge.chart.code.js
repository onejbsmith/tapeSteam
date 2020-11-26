/**
 * Get the current time
 */
window.getNow = function () {
    var now = new Date();

    return {
        hours: now.getHours() + now.getMinutes() / 60,
        minutes: now.getMinutes() * 12 / 60 + now.getSeconds() * 12 / 3600,
        seconds: now.getSeconds() * 12 / 60
    };
}

/**
 * Pad numbers
 */
function pad(number, length) {
    // Create an array of the remaining length + 1 and join it with 0's
    return new Array((length || 2) + 1 - String(number).length).join(0) + number;
}

var now = getNow();

window.now = now;

function (chart) {
    setInterval(function () {

        now = getNow();

        if (chart.axes) { // not destroyed
            var hour = chart.get('hour'),
                minute = chart.get('minute'),
                second = chart.get('second'),
                // run animation unless we're wrapping around from 59 to 0
                animation = now.seconds === 0 ?
                    false : {
                        easing: 'easeOutBounce'
                    };

            // Cache the tooltip text
            chart.tooltipText =
                pad(Math.floor(now.hours), 2) + ':' +
                pad(Math.floor(now.minutes * 5), 2) + ':' +
                pad(now.seconds * 5, 2);


            hour.update(now.hours, true, animation);
            minute.update(now.minutes, true, animation);
            second.update(now.seconds, true, animation);
        }

    }, 1000);

};


/**
 * Easing function from https://github.com/danro/easing-js/blob/master/easing.js
 */
Math.easeOutBounce = function (pos) {
    if ((pos) < (1 / 2.75)) {
        return (7.5625 * pos * pos);
    }
    if (pos < (2 / 2.75)) {
        return (7.5625 * (pos -= (1.5 / 2.75)) * pos + 0.75);
    }
    if (pos < (2.5 / 2.75)) {
        return (7.5625 * (pos -= (2.25 / 2.75)) * pos + 0.9375);
    }
    return (7.5625 * (pos -= (2.625 / 2.75)) * pos + 0.984375);
};
