(function () {
    var TimeSpan = window.TimeSpan = function (milliseconds, seconds, minutes, hours, days) {
        var msecPerSecond = 1000,
            msecPerMinute = 60000,
            msecPerHour = 3600000,
            msecPerDay = 86400000,
            msecs = 0;

        var isNumeric = function(input) {
            return !isNaN(parseFloat(input)) && isFinite(input);
        };

        if (milliseconds && !isNumeric(milliseconds)) {
            var obj = milliseconds;
            milliseconds = obj.milliseconds;
            seconds = obj.seconds;
            minutes = obj.minutes;
            hours = obj.hours;
            days = obj.days;
        }

        // Ctor
        if (isNumeric(days)) {
            msecs += days * msecPerDay;
        }
        if (isNumeric(hours)) {
            msecs += hours * msecPerHour;
        }
        if (isNumeric(minutes)) {
            msecs += minutes * msecPerMinute;
        }
        if (isNumeric(seconds)) {
            msecs += seconds * msecPerSecond;
        }
        if (isNumeric(milliseconds)) {
            msecs += milliseconds;
        }

        // Addition
        this.addMilliseconds = function (milliseconds) {
            if (!isNumeric(milliseconds)) {
                return;
            }
            msecs += milliseconds;
        };
        this.addSeconds = function (seconds) {
            if (!isNumeric(seconds)) {
                return;
            }
            msecs += seconds * msecPerSecond;
        };
        this.addMinutes = function (minutes) {
            if (!isNumeric(minutes)) {
                return;
            }
            msecs += (minutes * msecPerMinute);
        };
        this.addHours = function (hours) {
            if (!isNumeric(hours)) {
                return;
            }
            msecs += (hours * msecPerHour);
        };
        this.addDays = function (days) {
            if (!isNumeric(days)) {
                return;
            }
            msecs += (days * msecPerDay);
        };

        // Subtraction
        this.subtractMilliseconds = function (milliseconds) {
            if (!isNumeric(milliseconds)) {
                return;
            }
            msecs -= milliseconds;
        };
        this.subtractSeconds = function (seconds) {
            if (!isNumeric(seconds)) {
                return;
            }
            msecs -= seconds * msecPerSecond;
        };
        this.subtractMinutes = function (minutes) {
            if (!isNumeric(minutes)) {
                return;
            }
            msecs -= minutes * msecPerMinute;
        };
        this.subtractHours = function (hours) {
            if (!isNumeric(hours)) {
                return;
            }
            msecs -= hours * msecPerHour;
        };
        this.subtractDays = function (days) {
            if (!isNumeric(days)) {
                return;
            }
            msecs -= days * msecPerDay;
        };

        // Interact with other instance
        this.isTimeSpan = true;
        this.add = function (otherTimeSpan) {
            if (!otherTimeSpan.isTimeSpan) {
                return;
            }
            msecs += otherTimeSpan.totalMilliseconds();
        };
        this.subtract = function (otherTimeSpan) {
            if (!otherTimeSpan.isTimeSpan) {
                return;
            }
            msecs -= otherTimeSpan.totalMilliseconds();
        };
        this.equals = function (otherTimeSpan) {
            if (!otherTimeSpan.isTimeSpan) {
                return false;
            }
            return msecs === otherTimeSpan.totalMilliseconds();
        };

        // Getters
        this.totalMilliseconds = function (roundDown) {
            var result = msecs;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalSeconds = function (roundDown) {
            var result = msecs / msecPerSecond;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalMinutes = function (roundDown) {
            var result = msecs / msecPerMinute;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalHours = function (roundDown) {
            var result = msecs / msecPerHour;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };
        this.totalDays = function (roundDown) {
            var result = msecs / msecPerDay;
            if (roundDown === true) {
                result = Math.floor(result);
            }
            return result;
        };

        // Return a Fraction of the TimeSpan
        this.milliseconds = function () {
            return msecs % 1000;
        };
        this.seconds = function () {
            return Math.floor(msecs / msecPerSecond) % 60;
        };
        this.minutes = function () {
            return Math.floor(msecs / msecPerMinute) % 60;
        };
        this.hours = function () {
            return Math.floor(msecs / msecPerHour) % 24;
        };
        this.days = function () {
            return Math.floor(msecs / msecPerDay);
        };

        // toString use this format "hh:mm.dd"
        this.toString = function() {

            function appendFormat(value, separator) {
                return (value < 10 ? "0" + value : value) + (separator || "");
            }

            return appendFormat(this.days(), ".") +
                   appendFormat(this.hours(), ":") +
                   appendFormat(this.minutes(), ":") +
                   appendFormat(this.seconds());
        };
    };

    // "Static Constructors"
    TimeSpan.FromSeconds = function (seconds) {
        return new TimeSpan(0, seconds, 0, 0, 0);
    };
    TimeSpan.FromMinutes = function (minutes) {
        return new TimeSpan(0, 0, minutes, 0, 0);
    };
    TimeSpan.FromHours = function (hours) {
        return new TimeSpan(0, 0, 0, hours, 0);
    };
    TimeSpan.FromDays = function (days) {
        return new TimeSpan(0, 0, 0, 0, days);
    };
    TimeSpan.FromDates = function (firstDate, secondDate, forcePositive) {
        var differenceMsecs = secondDate.valueOf() - firstDate.valueOf();
        if (forcePositive === true) {
            differenceMsecs = Math.abs(differenceMsecs);
        }
        return new TimeSpan(differenceMsecs, 0, 0, 0, 0);
    };
    TimeSpan.Parse = function(timespanText) {
        var tokens = timespanText.split(":");
        var days = tokens[0].split(".");
        if (days.length === 2)
            return new TimeSpan(0, tokens[2], tokens[1], days[1], days[0]);
        return new TimeSpan(0, tokens[2], tokens[1], tokens[0], 0);
    };
}());