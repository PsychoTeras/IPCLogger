(function () {

    function initialize() {
        $("#btn-configure-logger").on("click", ruleAdd);

        userId = getParameterByName("userId");
        UsersController.getById(userId, onSetupUserName);

        RuleTypesController.get(true, function (data, success) {
            if (success) {
                window.app.ruleTypes = data;
                AssignedRulesController.get(userId, reloadAssignedRulesTable);
            }
        });
    };

    initialize();
})();