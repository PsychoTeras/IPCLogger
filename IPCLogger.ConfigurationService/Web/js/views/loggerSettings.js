(function () {

    var dictControls;

    function getApplicationId() {
        return $("#logger-settings #application-id").val();
    }

    function getLoggerId() {
        return $("#logger-settings #logger-id").val();
    }

    function isNew() {
        return $("#logger-settings #is-new").val() === "True";
    }

    function isEmbedded() {
        return $("#logger-settings #is-embedded").val() === "True";
    }

    function hasChanges() {
        var result = false;
        $.each(dictControls,
            function(_, item) {
                result = isNew() || item.control.isChanged();
                return !result;
            });
        return result;
    }

    function getPropertyObjs() {
        var propertyObjs = [];
        $.each(dictControls,
            function (_, item) {

                var control = item.control;
                if (control.isChanged() || control.isRequired() || isNew()) {
                    var propertyObj = control.getPropertyObject();
                    if (!propertyObj) {
                        throw "PropertyObject is undefined, control " + control.name();
                    }
                    propertyObj.isChanged |= isNew();
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
            $.each(dictControls,
                function (_, item) {
                    item.control.saveOrigValue();
                });
            if (!cancel(true)) {
                $.alert("Changes successfully saved", { type: "success" });
            }
        }
    }

    function savePropertyObjs(propertyObjs) {
        var applicationId = getApplicationId();
        var loggerId = getLoggerId();
        if (isNew()) {
            LoggerSettingsController.create(applicationId, loggerId, propertyObjs, processSaveResult);
        } else {
            LoggerSettingsController.update(applicationId, loggerId, propertyObjs, processSaveResult);
        }
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
        } else if (!cancel(true)) {
            $.alert("Nothing to save");
        }
    }

    function cancel(force) {
        if (isEmbedded()) {
            return false;
        }

        if (force) {
            window.onbeforeunload = undefined;
        }

        var applicationId = getApplicationId();
        ApplicationController.manageApplication(applicationId, "loggers");
        return true;
    }

    function reset() {
        $.each(dictControls,
            function (_, item) {
                item.control.resetValue();
                item.control.setValidity(true);
            });
    }

    function showFormattablePopup(e, element) {
        var applicationId = getApplicationId();
        PopupController.snippetsInfo(applicationId, function (data) {
        });
    }

    function initialize() {
        initToolBar();

        dictControls = new UI.ControlsFactory("#logger-settings div.form-control");
        $.each(dictControls,
            function (_, item) {
                var $control = $(item.control);
                $control.on("showFormattable", showFormattablePopup);
            });

        $("#logger-settings .div-save-cancel #btn-save").on("click", save);
        $("#logger-settings .div-save-cancel #btn-cancel").on("click", isEmbedded() ? reset : cancel);
        $("#logger-settings .div-save-cancel #btn-reset").on("click", reset);

        window.onbeforeunload = function () {
            return hasChanges() ? "Your changes will be lost. Continue?" : undefined;
        };
    }

    initialize();
})();