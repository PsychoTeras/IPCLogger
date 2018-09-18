(function () {

    function getModelId(caller) {
        return $(caller).parentsUntil("tbody", "#row-logger").attr("modelId");
    };

    function loggerConfigure(sender) {
        var me = this;
        var caller = sender.target;
        var modelId = getModelId(caller);
        LoggersController.manageLogger(modelId);
    };

    function initialize() {
        $("#btn-configure-logger").on("click", loggerConfigure);
    };

    initialize();
})();