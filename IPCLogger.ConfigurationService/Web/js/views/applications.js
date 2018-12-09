(function () {

    function getApplicationId(caller) {
        return $(caller).parentsUntil("tbody", "#row-application").attr("modelId");
    }

    function applicationConfigure(e) {
        var caller = e.target;
        var applicationId = getApplicationId(caller);
        ApplicationController.manageApplication(applicationId);
    }

    function initialize() {
        $("#table-applications").TableList();

        $("#table-applications button[id^='btn-configure-application']").on("click", applicationConfigure);
    }

    initialize();
})();