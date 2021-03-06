﻿(function (UI) {

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

    UI.PropertyCombo.prototype.afterChangeType = function ($element) {
        $element.addClass("custom-select");
    };

    UI.PropertyCombo.prototype.populateValues = function ($element, values) {
        //if (!isRequired()) {
        //    $element.append(new Option("", ""));
        //}

        $.each(values, function (_, value) {
            $element.append(new Option(value, value));
            return true;
        });

        $element.val($element.attr("value"));
    };

    UI.PropertyCombo.prototype.value = function (val) {
        var me = this;
        if (val) {
            me.Element.val(val).change();
        }
        return me.Element.val();
    };

})(window.UI = window.UI || {});