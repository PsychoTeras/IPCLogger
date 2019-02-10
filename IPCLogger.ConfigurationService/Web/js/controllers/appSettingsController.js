function AppSettingsController() {

    this.addLogger = function (applicationId, loggerId) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId, "new");
        navigate(url);
    };

    this.loggerSettings = function (applicationId, loggerId) {
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

    this.addPattern = function (applicationId) {
        var url = getApiUrl("applications", applicationId, "patterns", "new");
        navigate(url);
    };

    this.patternSettings = function (applicationId, patternId) {
        var url = getApiUrl("applications", applicationId, "patterns", patternId, "settings");
        navigate(url);
    };

    this.removePattern = function (applicationId, patternId, onReady) {
        var url = getApiUrl("applications", applicationId, "patterns", patternId);
        asyncQuery(
            url,
            "DELETE",
            null,
            onReady);
    };
}

window.AppSettingsController = new AppSettingsController();