(function () {

    var controls;

    function getApplicationId() {
        return $("#application-id").val();
    }

    function getLoggerId() {
        return $("#logger-id").val();
    }

    function getPropertyObjs() {
        var propertyObjs = [];
        $.each(controls,
            function (_, control) {

                var uiControl = control.uiControl;
                var propertyObj = uiControl.getPropertyObject();
                if (!propertyObj) {
                    throw "PropertyObject is undefined, control " + uiControl.name();
                }

                propertyObjs.push(propertyObj);
            });
        return propertyObjs;
    }

    function highlightValidationErroredControls(failedProps) {
        $.each(failedProps,
            function(_, p) {
                var uiControl = $.grep(controls, function(c) { return c.name === p.propertyName; })[0].uiControl;
                uiControl.setValidity(false, p.errorMessage);
            });
    }

    function processValidationResult(failedProps, propertyObjs) {
        if (failedProps) {
            highlightValidationErroredControls(failedProps);
        } else {
            savePropertyObjs(propertyObjs);
        }
    }

    function savePropertyObjs(propertyObjs) {
    }

    function validatePropertyObjs(propertyObjs) {
        var applicationId = getApplicationId();
        var loggerId = getLoggerId();
        LoggerSettingsController.validate(applicationId, loggerId, propertyObjs, processValidationResult);
    }

    function prepareForSave() {
        $.each(controls,
            function(_, control) {
                control.uiControl.setValidity(true);
            });
    }

    function save() {
        prepareForSave();
        var propertyObjs = getPropertyObjs();
        validatePropertyObjs(propertyObjs);
    }

    function cancel() {
        var applicationId = getApplicationId();
        ApplicationController.manageApplication(applicationId);
    }

    function reset() {
        $.each(controls,
            function (_, control) {
                control.uiControl.resetValue();
            });
    }

    function initialize() {
        window.controls = controls = new UI.ControlsFactory().initializeControls("#form-logger-settings div.form-control");
        $("#p-save-cancel #btn-save").on("click", save);
        $("#p-save-cancel #btn-cancel").on("click", cancel);
        $("#p-save-cancel #btn-reset").on("click", reset);
    }

    initialize();
})();