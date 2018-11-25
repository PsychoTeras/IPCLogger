(function (UI) {

    function ThrowIsAbstractError() {
        throw "PropertyBase is abstract class";
    }

    UI.PropertyBase = function (selector) {
        var me = this;
        if (me.constructor === UI.PropertyBase) {
            ThrowIsAbstractError();
        }
        me.Control = $(selector);
    };

    UI.PropertyBase.prototype.getPropertyType = function () {
        ThrowIsAbstractError();
    };

    UI.PropertyBase.prototype.getNodeType = function() {
        ThrowIsAbstractError();
    };

    UI.PropertyBase.prototype.getControlType = function () {
        return null;
    };

    UI.PropertyBase.prototype.afterChangeControlType = function ($control) {
        return $control;
    };

    UI.PropertyBase.prototype.initialize = function ($control) {

        function changeControlType($control, nodeType, controlType) {
            var attrs = {};
            $.each($control[0].attributes, function (_, attr) {
                attrs[attr.nodeName] = attr.nodeValue;
            });
            if (controlType) {
                attrs["type"] = controlType;
            }
            var $controlParent = $control.parent();
            $control.replaceWith(function () {
                return $("<" + nodeType + "/>", attrs).append($control.contents());
            });
            return $controlParent.children(nodeType);
        }

        var me = this;
        var nodeType = me.getNodeType();
        var controlType = me.getControlType();
        $control = changeControlType($control, nodeType, controlType);
        $control = me.afterChangeControlType($control);
    };

})(window.UI = window.UI || {});