(function () {

    function getApplicationId() {
        return $("#application-id")[0].value;
    }

    function getLoggerId(caller) {
        return $(caller).parentsUntil("tbody", "#row-logger").attr("modelId");
    }

    function loggerSettings(e) {
        var caller = e.target;
        var applicationId = getApplicationId();
        var loggerId = getLoggerId(caller);
        LoggerController.manageSettings(applicationId, loggerId);
    }

    function addLogger() {
        var applicationId = getApplicationId();
        PopupController.addLogger(applicationId, function (html) {
            var divAddLogger = $("#popup-add-logger");
            showModal(divAddLogger, html, null, function (e) {

            });
        });
    }

    function initialize() {
        $("#table-loggers").TableList();

        $("#table-loggers button[id^='btn-logger-settings']").on("click", loggerSettings);
        $("#table-loggers button[id='btn-add-logger']").on("click", addLogger);
    }

    initialize();
})();