function LoggerSettingsController() {

    function createOrUpdate(applicationId, loggerId, propertyObjs, callback, create) {
        var url = getApiUrl("applications", applicationId, "loggers", loggerId, "settings");
        asyncQuery(
            url,
            create ? "POST" : "PUT",
            "json",
            function (data) {
                var failedProps = data.length ? data : null;
                callback(failedProps, propertyObjs);
            },
            null,
            JSON.stringify(propertyObjs));
    }

    this.create = function (applicationId, loggerId, propertyObjs, callback) {
        createOrUpdate(applicationId, loggerId, propertyObjs, callback, true);
    };

    this.update = function (applicationId, loggerId, propertyObjs, callback) {
        createOrUpdate(applicationId, loggerId, propertyObjs, callback, false);
    };
}

window.LoggerSettingsController = new LoggerSettingsController();