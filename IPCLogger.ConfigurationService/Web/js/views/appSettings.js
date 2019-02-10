(function () {

    //=========== Logger manage methods ===========\\

    function getLoggerRow(caller) {
        return $(caller).parentsUntil("tbody", "#row-logger");
    }

    function getLoggerId(caller) {
        return getLoggerRow(caller).attr("modelId");
    }

    function getLoggerName(caller) {
        return getLoggerRow(caller).children("#cell-logger-type").html();
    }

    function addLogger() {
        var applicationId = getApplicationId();
        PopupController.addLogger(applicationId, function (html) {
            var divAddLogger = $("#popup-add-logger");
            showModal(divAddLogger, html, null, function () {
                var loggerId = $("#popup-add-logger #logger-id").val();
                if (loggerId) {
                    AppSettingsController.addLogger(applicationId, loggerId);
                }
            });
        });
    }

    function loggerSettings(e) {
        var caller = e.target;
        var applicationId = getApplicationId();
        var loggerId = getLoggerId(caller);
        AppSettingsController.loggerSettings(applicationId, loggerId);
    }

    function removeLogger(e) {
        var caller = e.target;
        var loggerId = getLoggerId(caller);
        var loggerName = getLoggerName(caller);
        var applicationId = getApplicationId();

        var msg = "Are you sure you want to remove logger " + loggerName + "?";
        showDialog.confirmation(msg, function (result) {
            if (!result) return;
            AppSettingsController.removeLogger(applicationId, loggerId, function () {
                getLoggerRow(caller).remove();
            });
        });
    }

    //=========== Pattern manage methods ===========\\

    function getPatternRow(caller) {
        return $(caller).parentsUntil("tbody", "#row-pattern");
    }

    function getPatternId(caller) {
        return getPatternRow(caller).attr("modelId");
    }

    function addPattern() {
        var applicationId = getApplicationId();
        AppSettingsController.addPattern(applicationId);
    }

    function patternSettings(e) {
        var caller = e.target;
        var applicationId = getApplicationId();
        var patternId = getPatternId(caller);
        AppSettingsController.patternSettings(applicationId, patternId);
    }

    function removePattern(e) {
        var caller = e.target;
        var applicationId = getApplicationId();
        var patternId = getPatternId(caller);

        var msg = "Are you sure you want to remove selected pattern?";
        showDialog.confirmation(msg, function (result) {
            if (!result) return;
            AppSettingsController.removePattern(applicationId, patternId, function () {
                getPatternRow(caller).remove();
            });
        });
    }

    //=========== Common methods ===========\\

    function getApplicationId() {
        return $("#appSettings #application-id")[0].value;
    }

    function changeTab(tabRef) {
        $(".btn-toolbar a[href='" + tabRef + "']").tab("show");
    }

    function onTabChanged(e) {
        var $target = $(e.target);
        var href = $target.attr("href");
        window.location.hash = href;
    }

    function initialize() {
        initToolBar();

        $("#table-loggers").TableList();
        $("#table-loggers button[id^='btn-logger-settings']").on("click", loggerSettings);
        $("#table-loggers a[id^='btn-logger-remove']").on("click", removeLogger);

        $("#table-patterns").TableList();
        $("#table-patterns button[id^='btn-pattern-settings']").on("click", patternSettings);
        $("#table-patterns a[id^='btn-pattern-remove']").on("click", removePattern);

        $(".btn-toolbar button[id='btn-add-logger']").on("click", addLogger);
        $(".btn-toolbar button[id='btn-add-pattern']").on("click", addPattern);

        changeTab(window.location.hash || ("#" + globalSetting.APPSETTINGS_DEFAULT_TAB));
        $(".btn-toolbar a[data-toggle=\"tab\"]").on("shown.bs.tab", onTabChanged);
    }

    initialize();
})();