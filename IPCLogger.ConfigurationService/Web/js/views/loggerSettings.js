(function () {

    var dictControls;

    function getApplicationId() {
        return $("#application-id").val();
    }

    function getLoggerId() {
        return $("#logger-id").val();
    }

    function hasChanges() {
        var result = false;
        $.each(dictControls,
            function(_, item) {
                result = item.control.isChanged();
                return !result;
            });
        return result;
    }

    function getPropertyObjs() {
        var propertyObjs = [];
        $.each(dictControls,
            function (_, item) {

                var control = item.control;
                if (control.isChanged() || control.isRequired()) {
                    var propertyObj = control.getPropertyObject();
                    if (!propertyObj) {
                        throw "PropertyObject is undefined, control " + control.name();
                    }

                    propertyObjs.push(propertyObj);
                }
            });
        return propertyObjs;
    }

    function highlightValidationErroredControls(failedProps) {
        $.each(failedProps,
            function(_, p) {
                var pName = p.isCommon ? "#" + p.name : p.name;
                var control = $.grep(dictControls, function(c) { return c.name === pName; })[0].control;
                control.setValidity(false, p.errorMessage);
            });
    }

    function processSaveResult(failedProps) {
        if (failedProps) {
            highlightValidationErroredControls(failedProps);
        } else {
            cancel(true);
        }
    }

    function savePropertyObjs(propertyObjs) {
        var applicationId = getApplicationId();
        var loggerId = getLoggerId();
        LoggerSettingsController.save(applicationId, loggerId, propertyObjs, processSaveResult);
    }

    function prepareForSave() {
        $.each(dictControls,
            function (_, item) {
                item.control.setValidity(true);
            });
    }

    function save() {
        prepareForSave();
        var propertyObjs = getPropertyObjs();
        if (propertyObjs.length) {
            savePropertyObjs(propertyObjs);
        } else {
            cancel(true);
        }
    }

    function cancel(force) {
        if (force) {
            window.onbeforeunload = undefined;
        }
        var applicationId = getApplicationId();
        ApplicationController.manageApplication(applicationId);
    }

    function reset() {
        $.each(dictControls,
            function (_, item) {
                item.control.resetValue();
                item.control.setValidity(true);
            });
    }

    function initialize() {
        dictControls = new UI.ControlsFactory("#form-logger-settings div.form-control");

        $(".div-save-cancel #btn-save").on("click", save);
        $(".div-save-cancel #btn-cancel").on("click", cancel);
        $(".div-save-cancel #btn-reset").on("click", reset);

        window.onbeforeunload = function () {
            return hasChanges() ? "Your changes will be lost. Continue?" : undefined;
        };
    }

    initialize();
})();