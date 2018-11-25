﻿(function (UI) {

    UI.PropertyNumeric = function (selector) {
        var me = this;
        UI.PropertyBase.call(me, selector);
    };

    UI.PropertyNumeric.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyNumeric.prototype.constructor = UI.PropertyNumeric;

    UI.PropertyNumeric.prototype.getPropertyType = function () {
        return "ui-property-numeric";
    };

    UI.PropertyNumeric.prototype.getNodeType = function () {
        return "input";
    };

    UI.PropertyNumeric.prototype.getControlType = function () {
        return "number";
    };

})(window.UI = window.UI || {});