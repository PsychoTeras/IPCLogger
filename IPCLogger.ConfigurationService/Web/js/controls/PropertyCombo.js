(function (UI) {

    UI.PropertyCombo = function () {
        var me = this;
        UI.PropertyBase.call(me);
    };

    UI.PropertyCombo.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyCombo.prototype.constructor = UI.PropertyCombo;

    UI.PropertyCombo.prototype.getPropertyType = function () {
        return "ui-property-combobox";
    };

    UI.PropertyCombo.prototype.getNodeType = function () {
        return "select";
    };

    UI.PropertyCombo.prototype.afterChangeControlType = function ($control) {
        $control.addClass("custom-select");
    };

    UI.PropertyCombo.prototype.populateValues = function ($control, values) {
        $control.append(new Option("", ""));
        $.each(values, function (_, value) {
            $control.append(new Option(value, value));
            return true;
        });
    };

})(window.UI = window.UI || {});