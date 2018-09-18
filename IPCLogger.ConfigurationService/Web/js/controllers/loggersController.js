function LoggersController() {

    this.manageLogger = function (loggerId) {

        var url = getApiUrl("loggers", loggerId);
        window.location.href = url;
    };

};

window.LoggersController = new LoggersController();