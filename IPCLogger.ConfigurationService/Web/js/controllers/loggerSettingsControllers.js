function LoggerSettingsController() {

    function createOrUpdate(applicationId, loggerId, settings, propertyObjs, callback) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId, settings);
        asyncQuery(
            url,
            "POST",
            "json",
            function (data) {
                var failedProps = data.length ? data : null;
                callback(failedProps, propertyObjs);
            },
            null,
            JSON.stringify(propertyObjs));
    }

    this.create = function (applicationId, loggerId, propertyObjs, callback) {
        createOrUpdate(applicationId, loggerId, undefined, propertyObjs, callback);
    };

    this.update = function (applicationId, loggerId, propertyObjs, callback) {
        createOrUpdate(applicationId, loggerId, "settings", propertyObjs, callback);
    };
}

window.LoggerSettingsController = new LoggerSettingsController();