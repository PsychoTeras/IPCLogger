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

    UI.PropertyBoolean.prototype.afterChangeType = function ($element) {
        var me = this;

        var cbId = "id-" + Math.random().toString(36).substr(2, 9);
        $element.addClass("custom-control custom-checkbox");
        $element.append('<input type="checkbox" class="custom-control-input" id="' + cbId + '">');
        $element.append('<label class="custom-control-label" for="' + cbId + '">YES/NO</label>');

        me.value($element.attr("value"));
    };

    UI.PropertyBoolean.prototype.value = function (val) {
        var input = this.Element.find("input")[0];
        if (val !== null && val !== undefined) {
            val = val.toString().toLowerCase();
            input.checked = val === "true";
        }
        return input.checked;
    };

})(window.UI = window.UI || {});