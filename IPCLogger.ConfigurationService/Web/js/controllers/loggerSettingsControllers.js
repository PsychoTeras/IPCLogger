function LoggerSettingsController() {

    this.save = function (applicationId, loggerId, propertyObjs, callback) {
        var url = getApiUrl("settings", "save",
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
}

window.LoggerSettingsController = new LoggerSettingsController();