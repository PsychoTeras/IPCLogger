function LoggersController() {

    this.manageLogger = function (loggerId) {

        var url = getApiUrl("loggers", loggerId);
        window.location.assign(url);
    };

};

window.LoggersController = new LoggersController();