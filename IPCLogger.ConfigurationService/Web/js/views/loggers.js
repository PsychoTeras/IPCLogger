(function () {

    function getApplicationId() {
        return $("#loggers #application-id")[0].value;
    }

    function getLoggerId(caller) {
        return $(caller).parentsUntil("tbody", "#row-logger").attr("modelId");
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

    function addPattern() {
        changeTab("#patterns");
    }

    function onTabChanged(e) {
        var $target = $(e.target);
        var href = $target.attr("href");
        window.location.hash = href;
    }

    function initialize() {
        $("#table-loggers").TableList();
        $("#table-loggers button[id^='btn-logger-settings']").on("click", loggerSettings);

        $(".btn-toolbar button[id='btn-add-logger']").on("click", addLogger);
        $(".btn-toolbar button[id='btn-add-pattern']").on("click", addPattern);

        changeTab(window.location.hash || ("#" + globalSetting.LOGGERS_DEFAULT_TAB));
        $(".btn-toolbar a[data-toggle=\"tab\"]").on("shown.bs.tab", onTabChanged);
    }

    initialize();
})();