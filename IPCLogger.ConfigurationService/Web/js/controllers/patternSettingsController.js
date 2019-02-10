function PatternSettingsController() {

    function createOrUpdate(applicationId, patternId, settings, propertyObjs, callback) {
        var url = getApiUrl("applications", applicationId, "patterns", patternId, settings);
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

    this.create = function (applicationId, propertyObjs, callback) {
        createOrUpdate(applicationId, "new", undefined, propertyObjs, callback);
    };

    this.update = function (applicationId, patternId, propertyObjs, callback) {
        createOrUpdate(applicationId, patternId, "settings", propertyObjs, callback);
    };
}

window.PatternSettingsController = new PatternSettingsController();