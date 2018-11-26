(function (UI) {

    UI.PropertyBoolean = function (selector) {
        var me = this;
        UI.PropertyBase.call(me, selector);
    };

    UI.PropertyBoolean.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyBoolean.prototype.constructor = UI.PropertyBoolean;

    UI.PropertyBoolean.prototype.getPropertyType = function () {
        return "ui-property-boolean";
    };

    UI.PropertyBoolean.prototype.afterChangeControlType = function ($control) {
        var cbId = "id-" + Math.random().toString(36).substr(2, 9);

        $control.addClass("custom-control custom-checkbox").removeClass("");
        $control.append('<input type="checkbox" class="custom-control-input" id="' + cbId + '">');
        $control.append('<label class="custom-control-label" for="' + cbId + '">YES/NO</label>');
    };

})(window.UI = window.UI || {});