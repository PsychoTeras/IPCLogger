(function (UI) {

    UI.PropertyString = function (selector) {
        var me = this;
        UI.PropertyBase.call(me, selector);
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