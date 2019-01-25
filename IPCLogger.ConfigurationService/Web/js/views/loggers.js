(function () {

    function getApplicationId() {
        return $("#loggers #application-id")[0].value;
    }

    function getLoggerRow(caller) {
        return $(caller).parentsUntil("tbody", "#row-logger");
    }

    function getLoggerId(caller) {
        return getLoggerRow(caller).attr("modelId");
    }

    function getLoggerName(caller) {
        return getLoggerRow(caller).children("#cell-logger-type").html();
    }

    function changeTab(tabRef) {
        $(".btn-toolbar a[href='" + tabRef + "']").tab("show");
    }

    function loggerSettings(e) {
        var caller = e.target;
        var applicationId = getApplicationId();
        var loggerId = getLoggerId(caller);
        LoggerController.manageSettings(applicationId, loggerId);
    }

    function addLogger() {
        changeTab("#loggers");

        var applicationId = getApplicationId();
        PopupController.addLogger(applicationId, function (html) {
            var divAddLogger = $("#popup-add-logger");
            showModal(divAddLogger, html, null, function () {
                var loggerId = $("#popup-add-logger #logger-id").val();
                if (loggerId) {
                    LoggerController.addLogger(applicationId, loggerId);
                }
            });
        });
    }

    function removeLogger(e) {
        var caller = e.target;
        var loggerId = getLoggerId(caller);
        var loggerName = getLoggerName(caller);
        var applicationId = getApplicationId();

        var msg = "Are you sure you want to remove logger " + loggerName + "?";
        showDialog.confirmation(msg, function (result) {
            if (!result) return;
            LoggerController.removeLogger(applicationId, loggerId, function () {
                getLoggerRow(caller).remove();
            });
        });
    }

    function addPattern() {
        changeTab("#patterns");
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

        $(".btn-toolbar button[id='btn-add-logger']").on("click", addLogger);
        $(".btn-toolbar button[id='btn-add-pattern']").on("click", addPattern);

        changeTab(window.location.hash || ("#" + globalSetting.LOGGERS_DEFAULT_TAB));
        $(".btn-toolbar a[data-toggle=\"tab\"]").on("shown.bs.tab", onTabChanged);
    }

    initialize();
})();