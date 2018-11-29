function LoggerSettingsController() {

    this.validate = function (applicationId, loggerId, propertyObjs, callback) {
        var url = getApiUrl("validate", "properties",
            {
                appid: applicationId,
                lid: loggerId
            });
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
    };

    this.saveChanges = function () {
    };

}

window.LoggerSettingsController = new LoggerSettingsController();