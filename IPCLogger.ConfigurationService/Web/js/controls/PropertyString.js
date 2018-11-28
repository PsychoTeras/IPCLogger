(function (UI) {

    UI.PropertyString = function () {
        var me = this;
        UI.PropertyBase.call(me);
    };

    UI.PropertyString.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyString.prototype.constructor = UI.PropertyString;

    UI.PropertyString.prototype.getPropertyType = function () {
        return "ui-property-string";
    };

    UI.PropertyString.prototype.getNodeType = function () {
        return "input";
    };

})(window.UI = window.UI || {});