(function ($) {

    $.fn.TableList = function () {
        var $table = this;

        var TableClasses = "table-hover";
        var CellActions = "cell-actions";

        $table.addClass(TableClasses);

        $table.find("tbody>tr").mouseover(function () {
            $table.find("tbody>tr>td").removeClass("mouseover");
            $(this).find("#" + CellActions).addClass("mouseover");
        });

        $table.find("tbody>tr").mouseleave(function () {
            $(this).find("#" + CellActions).removeClass("mouseover");
        });

        var mouseX = window.mouseX(), mouseY = window.mouseY();
        if (mouseX !== undefined && mouseY !== undefined) {
            var $el = $(document.elementFromPoint(mouseX, mouseY));
            if ($el[0] && $el[0].id !== CellActions) {
                $el = $el.closest("#" + CellActions);
            }
            if ($el.length) {
                $el.addClass("mouseover");
            }
        }
    };

}(jQuery));