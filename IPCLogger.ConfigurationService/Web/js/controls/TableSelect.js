(function ($) {

    $.fn.TableSelect = function (onSelected) {
        var $table = this;
        $table.on("click", "tr", function () {
            var $tr = $(this);
            $table.find("tr").removeClass("selected");
            $tr.addClass("selected");
            if (onSelected) {
                onSelected($tr);
            }
        });
    };

}(jQuery));