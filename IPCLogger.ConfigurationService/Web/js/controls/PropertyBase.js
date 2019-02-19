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

    UI.PropertyBase.prototype.isFormattable = function () {
        var me = this;
        return me.Element.attr("formattable") !== undefined;
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
        var isChanged = me.isChanged();
        return {
            name: name,
            value: value,
            isCommon: isCommon,
            isChanged: isChanged
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

    UI.PropertyBase.prototype.saveOrigValue = function (dontTriggerChange) {
        var me = this;
        me.OrigValue = me.value();
        if (!dontTriggerChange) {
            $(me.Element).trigger("change");
        }
    };

    UI.PropertyBase.prototype.onChangeEvent = function () {
        var me = this;
        if (me.isChanged()) {
            me.VisibleElement.attr("changed", "");
        } else {
            me.VisibleElement.removeAttr("changed");
        }
    };

    UI.PropertyBase.prototype.initialize = function ($element) {

        var me = this;

        function changeType($element, nodeType, controlType, isFormattable) {
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

            var $target = $parentElement.children(nodeType);

            if (isFormattable) {
                var $divToInsertAfter = $parentElement.children("div[class='group-param-info']");
                var $div = $("<div class='div-formattable'>").insertAfter($divToInsertAfter);
                $target.appendTo($div);
                var $btnFmtShow = $("<button class='btn btn-sm btn-glyph btn-light fa fa-book'>");
                $btnFmtShow.click(function () {
                    $(me).trigger("showFormattable", $target);
                });
                $div.append($btnFmtShow);
            }

            return $target;
        }

        var nodeType = me.getNodeType();
        var controlType = me.getControlType();
        var isFormattable = $element.attr("formattable") !== undefined;

        me.Element = $element = changeType($element, nodeType, controlType, isFormattable);

        me.VisibleElement = me.afterChangeType($element) || $element;

        var $values = $element.attr("values");
        if ($values) {
            var values = CSVToArray($values);
            if (values) {
                me.populateValues($element, values);
            }
            $element.removeAttr("values");
        }

        me.saveOrigValue(true);
        me.Element.on("change", $.proxy(me.onChangeEvent, me));
    };

})(window.UI = window.UI || {});