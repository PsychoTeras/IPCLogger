(function () {

    function initialize() {
        $("#popup-formattable button.close").on("click", function () {
            var $panelContainer = $(".panel-container[show-formattable]");
            $panelContainer.removeAttr("show-formattable");
            $panelContainer.children(".panel-container-left").width("100%");
        });
    }

    initialize();
})();