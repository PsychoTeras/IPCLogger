(function () {

    function getApplicationId() {
        return $("#application-id")[0].value;
    }

    function getLoggerId(caller) {
        return $(caller).parentsUntil("tbody", "#row-logger").attr("modelId");
    }

    function loggerSettings(sender) {
        var caller = sender.target;
        var applicationId = getApplicationId();
        var loggerId = getLoggerId(caller);
        LoggerController.manageSettings(applicationId, loggerId);
    }

    function initialize() {
        this.tableList = new UI.TableList("#table-loggers");

        $(".btn-logger-settings").on("click", loggerSettings);
    }

    initialize();
})();