function LoggerController() {

    this.addLogger = function (applicationId, loggerId) {
        location.href = getApiUrl("applications", applicationId, "loggers", loggerId);
    };

    this.manageSettings = function (applicationId, loggerId) {
        location.href = getApiUrl("applications", applicationId, "loggers", loggerId, "settings");
    };

}

window.LoggerController = new LoggerController();