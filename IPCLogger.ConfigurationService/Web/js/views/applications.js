(function () {

    function getApplicationId(caller) {
        return $(caller).parentsUntil("tbody", "#row-application").attr("modelId");
    }

    function applicationConfigure(sender) {
        var caller = sender.target;
        var applicationId = getApplicationId(caller);
        ApplicationController.manageApplication(applicationId);
    };

    function initialize() {
        this.tableList = new UI.TableList("#table-applications");

        $("#table-applications button[id^='btn-configure-application']").on("click", applicationConfigure);
    }

    initialize();
})();