(function (UI) {

    var dictControls;

    UI.ControlsFactory = function (selector) {
        var result = [];

        $.each($(selector),
            function (_, element) {
                $.each(element.classList, function (_, className) {

                    var value = dictControls[className];
                    if (!value) return true;

                    var control = Object.create(value.prototype);
                    control.initialize($(element));

                    var controlName = element.getAttribute("name");
                    if (element.getAttribute("common") !== null) {
                        controlName = "#" + controlName;
                    }

                    result.push({ name: controlName, control: control });

                    return true;
                });

                return true;
            });

        return result;
    };

    function initialize() {
        dictControls = {};

        for (var prop in UI) {
            if (!UI.hasOwnProperty(prop)) continue;

            var value = UI[prop];
            if (UI.PropertyBase.prototype.isPrototypeOf(value.prototype)) {
                dictControls[value.prototype.getPropertyType()] = value;
            }
        }
    }

    initialize();

})(window.UI = window.UI || {});