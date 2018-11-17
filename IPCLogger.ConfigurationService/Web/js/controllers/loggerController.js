function LoggerController() {

    this.manageSettings = function (applicationId, loggerId) {

        var url = getApiUrl("applications", applicationId, "loggers", loggerId, "settings");
        window.location.assign(url);
    };

}

window.LoggerController = new LoggerController();