(function () {

    var $SelectedRow;

    function initialize() {
        $("#popup-add-logger #table-loggers").TableSelect(function ($tr) {
            $SelectedRow = $tr;
            $("#popup-add-logger #btn-add").removeClass("disabled");
        });
    }

    initialize();
})();