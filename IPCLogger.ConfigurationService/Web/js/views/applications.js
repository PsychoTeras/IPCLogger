﻿(function () {

    function getApplicationId(caller) {
        return $(caller).parentsUntil("tbody", "#row-application").attr("modelId");
    }

    function applicationConfigure(e) {
        var caller = e.target;
        var applicationId = getApplicationId(caller);
        ApplicationsController.applicationSettings(applicationId);
    }

    function initialize() {
        initToolBar();

        $("#table-applications").TableList();
        $("#table-applications button[id^='btn-configure-application']").on("click", applicationConfigure);
    }

    initialize();
})();