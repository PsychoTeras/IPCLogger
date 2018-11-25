(function (UI) {

    UI.PropertyTime = function (selector) {
        var me = this;
        UI.PropertyBase.call(me, selector);
    };

    UI.PropertyTime.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyTime.prototype.constructor = UI.PropertyTime;

    UI.PropertyTime.prototype.getPropertyType = function () {
        return "ui-property-time";
    };

    UI.PropertyTime.prototype.getNodeType = function () {
        return "input";
    };

    UI.PropertyTime.prototype.getControlType = function () {
        return "text";
    };

    UI.PropertyTime.prototype.afterChangeControlType = function ($control) {
        $control.durationPicker();
    };

})(window.UI = window.UI || {});