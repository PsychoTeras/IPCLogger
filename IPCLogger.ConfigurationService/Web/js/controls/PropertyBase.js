(function (UI) {

    function ThrowIsAbstractError() {
        throw "PropertyBase is abstract class";
    }

    UI.PropertyBase = function () {
        var me = this;
        if (me.constructor === UI.PropertyBase) {
            ThrowIsAbstractError();
        }
    };

    UI.PropertyBase.prototype.getPropertyType = function () {
        ThrowIsAbstractError();
    };

    UI.PropertyBase.prototype.getNodeType = function() {
        return null;
    };

    UI.PropertyBase.prototype.getControlType = function () {
        return null;
    };

    UI.PropertyBase.prototype.afterChangeControlType = function ($control) {
        return $control;
    };

    UI.PropertyBase.prototype.populateValues = function () {
    };

    UI.PropertyBase.prototype.name = function () {
        var me = this;
        return me.Control.attr("name");
    };

    UI.PropertyBase.prototype.value = function (val) {
        var me = this;
        if (val || val === "" || val === 0) {
            me.Control.val(val).change();
        }
        return me.Control.val();
    };

    UI.PropertyBase.prototype.resetValue = function () {
        var me = this;
        me.value(me.OrigValue);
    };

    UI.PropertyBase.prototype.getPropertyObject = function() {
        var me = this;
        var name = me.name();
        var value = me.value();
        return {
            name: name,
            value: value
        };
    };

    UI.PropertyBase.prototype.setValidity = function (valid, errorMessage) {
        var me = this;
        var $invalidFeedback = me.VisibleControl.next(".invalid-feedback");
        if (valid === true) {
            me.VisibleControl.removeAttr("invalid");
            $invalidFeedback.text("").removeClass("visible");
        } else if (valid === false) {
            me.VisibleControl.attr("invalid", "");
            if (errorMessage) {
                $invalidFeedback.text(errorMessage).addClass("visible");
            }
        } else {
            throw "Valid is not specified";
        }
    };

    UI.PropertyBase.prototype.initialize = function ($control) {

        function changeControlType($control, nodeType, controlType) {
            if (!nodeType) {
                return $control;
            }

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

        me.Control = $control = changeControlType($control, nodeType, controlType);

        me.VisibleControl = me.afterChangeControlType($control) || $control;

        var $values = $control.attr("values");
        if ($values) {
            var values = CSVToArray($values);
            if (values.length) {
                me.populateValues($control, values[0]);
            }
            $control.removeAttr("values");
        }

        me.OrigValue = me.value();
    };

})(window.UI = window.UI || {});