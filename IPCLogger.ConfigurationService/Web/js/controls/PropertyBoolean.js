(function (UI) {

    UI.PropertyBoolean = function () {
        var me = this;
        UI.PropertyBase.call(me);
    };

    UI.PropertyBoolean.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyBoolean.prototype.constructor = UI.PropertyBoolean;

    UI.PropertyBoolean.prototype.getPropertyType = function () {
        return "ui-property-boolean";
    };

    UI.PropertyBoolean.prototype.afterChangeControlType = function ($control) {
        var me = this;

        var cbId = "id-" + Math.random().toString(36).substr(2, 9);
        $control.addClass("custom-control custom-checkbox");
        $control.append('<input type="checkbox" class="custom-control-input" id="' + cbId + '">');
        $control.append('<label class="custom-control-label" for="' + cbId + '">YES/NO</label>');

        me.value($control.attr("value"));
    };

    UI.PropertyBoolean.prototype.value = function (val) {
        var $input = this.Control.find("input");
        if (val !== null && val !== undefined) {
            val = val.toLowerCase();
            if (val === "true") {
                $input.attr("checked", "checked");
            } else {
                $input.removeAttr("checked");
            }
        }
        return $input[0].checked;
    };

})(window.UI = window.UI || {});