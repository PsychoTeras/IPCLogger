(function (UI) {

    UI.PropertySize = function () {
        var me = this;
        UI.PropertyBase.call(me);
    };

    UI.PropertySize.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertySize.prototype.constructor = UI.PropertySize;

    UI.PropertySize.prototype.getPropertyType = function () {
        return "ui-property-size";
    };

    UI.PropertySize.prototype.getNodeType = function () {
        return "input";
    };

    UI.PropertySize.prototype.getControlType = function () {
        return "text";
    };

    UI.PropertySize.prototype.afterChangeControlType = function ($control) {
        return $control.sizePicker().next();
    };

})(window.UI = window.UI || {});