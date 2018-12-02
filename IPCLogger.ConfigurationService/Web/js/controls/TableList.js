(function (UI) {

    UI.TableList = function (selector) {
        var me = this;
        me.Table = $(selector);
        me.initialize();
    };

    UI.TableList.TABLE_CLASSES = "table-hover";
    UI.TableList.CELL_ACTIONS = "cell-actions";

    UI.TableList.prototype.initialize = function() {
        var me = this;
        var vars = UI.TableList;

        me.Table.addClass(vars.TABLE_CLASSES);

        me.Table.find("tbody>tr").mouseover(function () {
            $(this).find("#" + vars.CELL_ACTIONS).addClass("mouseover");
        });

        me.Table.find("tbody>tr").mouseleave(function () {
            $(this).find("#" + vars.CELL_ACTIONS).removeClass("mouseover");
        });

        var mouseX = window.mouseX(), mouseY = window.mouseY();
        if (mouseX !== undefined && mouseY !== undefined) {
            var $el = $(document.elementFromPoint(mouseX, mouseY));
            if ($el[0] && $el[0].id !== vars.CELL_ACTIONS) {
                $el = $el.closest("#" + vars.CELL_ACTIONS);
            }
            if ($el.length) {
                $el.addClass("mouseover");
            }
        }
    };

})(window.UI = window.UI || {});