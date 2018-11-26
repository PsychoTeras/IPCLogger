(function (UI) {

    UI.PropertyTimeSpan = function (selector) {
        var me = this;
        UI.PropertyBase.call(me, selector);
    };

    UI.PropertyTimeSpan.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyTimeSpan.prototype.constructor = UI.PropertyTimeSpan;

    UI.PropertyTimeSpan.prototype.getPropertyType = function () {
        return "ui-property-timespan";
    };

    UI.PropertyTimeSpan.prototype.getNodeType = function () {
        return "input";
    };

    UI.PropertyTimeSpan.prototype.getControlType = function () {
        return "text";
    };

    UI.PropertyTimeSpan.prototype.afterChangeControlType = function ($control) {
        $control.durationPicker();
    };

})(window.UI = window.UI || {});