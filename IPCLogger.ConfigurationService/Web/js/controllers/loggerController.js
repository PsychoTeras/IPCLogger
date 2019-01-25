function LoggerController() {

    this.addLogger = function (applicationId, loggerId) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId);
        navigate(url);
    };

    this.manageSettings = function (applicationId, loggerId) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId, "settings");
        navigate(url);
    };

    this.removeLogger = function (applicationId, loggerId, onReady) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId);
        asyncQuery(
            url,
            "DELETE",
            null,
            onReady);
    };
}

window.LoggerController = new LoggerController();