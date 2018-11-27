(function () {

    function initialize() {
        var me = this;
        me.uiControls = new UI.ControlsFactory().initializeControls("#form-logger-settings div.form-control");
    }

    initialize();

})();