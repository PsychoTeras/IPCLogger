﻿(function () {

    function initialize() {

        var leftScroll = initPerfectScrollBar("#popup-formattable .tab-snippets");

        $.sidebarMenu($("#popup-formattable .tab-snippets")).on("resized", function () {
            leftScroll.update();
        });

        leftScroll.update();
        
        $("#popup-formattable a.close").on("click", function () {
            var $panelContainer = $(".panel-container[show-formattable]");
            $panelContainer.removeAttr("show-formattable");
            $panelContainer.children(".panel-container-left").width("100%");
        });
    }

    initialize();
})();