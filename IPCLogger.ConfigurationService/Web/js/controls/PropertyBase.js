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

    UI.PropertyBase.prototype.afterChangeType = function ($element) {
        return $element;
    };

    UI.PropertyBase.prototype.populateValues = function () {
    };

    UI.PropertyBase.prototype.name = function () {
        var me = this;
        var name = me.Element.attr("name");
        if (me.Element.attr("common")) {
            name = "#" + name;
        }
        return name;
    };

    UI.PropertyBase.prototype.value = function (val) {
        var me = this;
        if (val || val === "" || val === 0) {
            me.Element.val(val).change();
        }
        return me.Element.val();
    };

    UI.PropertyBase.prototype.resetValue = function () {
        var me = this;
        me.value(me.OrigValue);
    };

    UI.PropertyBase.prototype.isRequired = function () {
        var me = this;
        return me.Element.attr("required") !== undefined;
    };

    UI.PropertyBase.prototype.isCommon = function () {
        var me = this;
        return me.Element.attr("common") !== undefined;
    };

    UI.PropertyBase.prototype.isChanged = function () {
        var me = this;
        return me.value() !== me.OrigValue;
    };

    UI.PropertyBase.prototype.getPropertyObject = function() {
        var me = this;
        var name = me.Element.attr("name");
        var value = me.value();
        var isCommon = me.isCommon();
        return {
            name: name,
            value: value,
            isCommon: isCommon
        };
    };

    UI.PropertyBase.prototype.setValidity = function (valid, errorMessage) {
        var me = this;
        var $invalidFeedback = me.VisibleElement.next(".invalid-feedback");
        if (valid === true) {
            me.VisibleElement.removeAttr("invalid");
            $invalidFeedback.text("").removeClass("visible");
        } else if (valid === false) {
            me.VisibleElement.attr("invalid", "");
            if (errorMessage) {
                $invalidFeedback.text(errorMessage).addClass("visible");
            }
        } else {
            throw "Valid is not specified";
        }
    };

    UI.PropertyBase.prototype.initialize = function ($element) {

        function changeType($element, nodeType, controlType) {
            if (!nodeType) {
                return $element;
            }

            var attrs = {};
            $.each($element[0].attributes, function (_, attr) {
                attrs[attr.nodeName] = attr.nodeValue;
            });
            if (controlType) {
                attrs["type"] = controlType;
            }
            var $parentElement = $element.parent();
            $element.replaceWith(function () {
                return $("<" + nodeType + "/>", attrs).append($element.contents());
            });

            return $parentElement.children(nodeType);
        }

        var me = this;
        var nodeType = me.getNodeType();
        var controlType = me.getControlType();

        me.Element = $element = changeType($element, nodeType, controlType);

        me.VisibleElement = me.afterChangeType($element) || $element;

        var $values = $element.attr("values");
        if ($values) {
            var values = CSVToArray($values);
            if (values.length) {
                me.populateValues($element, values[0]);
            }
            $element.removeAttr("values");
        }

        me.OrigValue = me.value();
    };

})(window.UI = window.UI || {});