(function (UI) {

    var dictUIControls;

    UI.ControlsFactory = function () {
    };

    UI.ControlsFactory.prototype.initializeControls = function (selector) {
        var me = this;
        me.$Controls = $(selector);

        $.each(me.$Controls,
            function (_, control) {
                $.each(control.classList, function (_, className) {

                    var value = dictUIControls[className];
                    if (!value) return true;

                    var uiControl = Object.create(value.prototype);
                    uiControl.initialize($(control));

                    return true;
                });

                return true;
            });
    };

    function initialize() {
        dictUIControls = {};

        for (var prop in UI) {
            if (!UI.hasOwnProperty(prop)) continue;

            var value = UI[prop];
            if (UI.PropertyBase.prototype.isPrototypeOf(value.prototype)) {
                dictUIControls[value.prototype.getPropertyType()] = value;
            }
        }
    }

    initialize();

})(window.UI = window.UI || {});