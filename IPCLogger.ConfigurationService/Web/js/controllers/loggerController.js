function LoggerController() {

    this.addLogger = function (applicationId, loggerId) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId);
        navigate(url);
    };

    this.manageSettings = function (applicationId, loggerId) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId, "settings");
        navigate(url);
    };

}

window.LoggerController = new LoggerController();