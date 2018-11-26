(function (UI) {

    UI.PropertySize = function (selector) {
        var me = this;
        UI.PropertyBase.call(me, selector);
    };

    UI.PropertySize.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertySize.prototype.constructor = UI.PropertySize;

    UI.PropertySize.prototype.getPropertyType = function () {
        return "ui-property-timespan";
    };

    UI.PropertySize.prototype.getNodeType = function () {
        return "input";
    };

    UI.PropertySize.prototype.getControlType = function () {
        return "text";
    };

    UI.PropertySize.prototype.afterChangeControlType = function ($control) {
        $control.sizePicker();
    };

})(window.UI = window.UI || {});