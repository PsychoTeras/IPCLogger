(function () {

    function getModelId(caller) {
        var modelId = $(caller).parentsUntil("tbody", "#row-application").attr("modelId");
        return modelId;
    };

    function loggerConfigure(sender) {
        var me = this;
        var caller = sender.target;
        var modelId = getModelId(caller);
    };

    function initialize() {
        $("#btn-configure-logger").on("click", loggerConfigure);
    };

    initialize();
})();