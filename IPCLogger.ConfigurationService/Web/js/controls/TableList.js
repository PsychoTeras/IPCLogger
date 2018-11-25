(function (UI) {

    UI.TableList = function (selector) {
        var me = this;
        me.Table = $(selector);
        me.initialize();
    };

    UI.TableList.CLASS = "ui-table-list table-hover table-striped";
    UI.TableList.DIV_ACTIONS = "div-actions";
    UI.TableList.CELL_ACTIONS = "cell-actions";

    UI.TableList.prototype.initialize = function() {
        var me = this;
        var vars = UI.TableList;

        me.Table.addClass(vars.CLASS);

        $.each(me.Table.find("tbody>tr #" + vars.CELL_ACTIONS), function () {
            var $cell = this;
            var $div = $("<div/>").
                addClass(vars.DIV_ACTIONS).
                attr("id", vars.DIV_ACTIONS).
                prepend($cell.children);
            $cell.append($div[0]);
            return true;
        });

        me.Table.find("tbody>tr").mouseover(function () {
            $(this).find("#" + vars.DIV_ACTIONS).addClass("mouseover");
        });

        me.Table.find("tbody>tr").mouseleave(function () {
            $(this).find("#" + vars.DIV_ACTIONS).removeClass("mouseover");
        });

        var mouseX = window.mouseX(), mouseY = window.mouseY();
        if (mouseX !== undefined && mouseY !== undefined) {
            var $el = $(document.elementFromPoint(mouseX, mouseY));
            if ($el[0] && $el[0].id !== vars.CELL_ACTIONS) {
                $el = $el.closest('#' + vars.CELL_ACTIONS);
            }
            if ($el.length) {
                $el.children("#" + vars.DIV_ACTIONS).addClass("mouseover");
            }
        }
    };

})(window.UI = window.UI || {});