function LoggerController() {

    this.manageSettings = function (applicationId, loggerId) {
        location.href = getApiUrl("applications", applicationId, "loggers", loggerId, "settings");
    };

}

window.LoggerController = new LoggerController();