(function () {

    var dictControls;

    function getApplicationId() {
        return $("#pattern-settings #application-id").val();
    }

    function getPatternId() {
        return $("#pattern-settings #pattern-id").val();
    }

    function isNew() {
        return $("#pattern-settings #is-new").val() === "True";
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
                var control = $.grep(dictControls, function (c) { return c.name === pName; })[0].control;
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
        if (isNew()) {
            PatternSettingsController.create(applicationId, propertyObjs, processSaveResult);
        } else {
            var patternId = getPatternId();
            PatternSettingsController.update(applicationId, patternId, propertyObjs, processSaveResult);
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
        } else {
            cancel(true);
        }
    }

    function cancel(force) {
        if (force) {
            window.onbeforeunload = undefined;
        }

        var applicationId = getApplicationId();
        ApplicationsController.applicationSettings(applicationId, "patterns");
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

        dictControls = new UI.ControlsFactory("#pattern-settings div.form-control");
        $.each(dictControls,
            function (_, item) {
                var $control = $(item.control);
                $control.on("showFormattable", showFormattablePopup);
            });

        $("#pattern-settings .div-save-cancel #btn-save").on("click", save);
        $("#pattern-settings .div-save-cancel #btn-cancel").on("click", cancel);
        $("#pattern-settings .div-save-cancel #btn-reset").on("click", reset);

        window.onbeforeunload = function () {
            return hasChanges() ? "Your changes will be lost. Continue?" : undefined;
        };
    }

    initialize();
})();