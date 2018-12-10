(function () {

    function addLogger() {
        var loggerId = $("#popup-add-logger #table-loggers tr.selected").attr("modelId");
        if (loggerId) {
            $("#popup-add-logger #logger-id").val(loggerId);
            $("#popup-add-logger").modal("hide");
        }
    }

    function initialize() {
        $("#popup-add-logger #table-loggers").TableSelect(function() {
            $("#popup-add-logger #btn-add").removeClass("disabled");
        });

        $("#popup-add-logger #btn-add").on("click", function() {
            addLogger();
        });

        $("#popup-add-logger").submit(function(e) {
            e.preventDefault();
            addLogger();
        });
    }

    initialize();
})();