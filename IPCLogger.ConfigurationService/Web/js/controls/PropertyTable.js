(function (UI) {

    var $Table = null;
    var $RowEditing = null;
    var $ColNumber = null;

    UI.PropertyTable = function () {
        var me = this;
        UI.PropertyBase.call(me);
    };

    UI.PropertyTable.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyTable.prototype.constructor = UI.PropertyTable;

    //=========== Row editing methods ===========\\

    function captureEvent(e) {
        e.stopPropagation();
    }

    function captureKey(e) {
        if (e.which === 13) {
            saveChanges();
        } else if (e.which === 27) {
            cancelChanges();
        }
    }

    function beginEditing() {
        $Table.addClass("editing");
        $RowEditing.addClass("editing");
        $RowEditing.find(".td-actions .btn-edit").removeClass("fa-pencil").addClass("fa-save");
        $RowEditing.find(".td-actions .btn-remove").removeClass("fa-trash").addClass("fa-remove");
        $RowEditing.find("input:first").focus();
    }

    function editRow($tr) {
        $("td[data-field]", $RowEditing = $tr).each(function () {
            var me = this;
            var $me = $(this);

            var value = $me.text();
            var width = $me.width();

            $me.empty().width(width);

            var input = $('<input type="text" />')
                .val(value)
                .data("old-value", value)
                .dblclick(captureEvent);

            input.appendTo(me);
            input.keydown(captureKey);
        });

        beginEditing();
    }

    function endEditing() {
        $Table.removeClass("editing");
        $RowEditing.removeClass("editing");
        $RowEditing.find(".td-actions .btn-edit").removeClass("fa-save").addClass("fa-pencil");
        $RowEditing.find(".td-actions .btn-remove").removeClass("fa-remove").addClass("fa-trash");
        $RowEditing.removeAttr("is-new-row");
        $RowEditing = null;
    }

    function saveChanges() {
        $("td[data-field]", $RowEditing).each(function () {
            var value = $(":input", this).val();
            $(this).empty().text(value);
        });
        endEditing();
    }

    function cancelChanges() {
        $("td[data-field]", $RowEditing).each(function () {
            var value = $(":input", this).data("old-value");
            $(this).empty().text(value);
        });
        if ($RowEditing.attr("is-new-row")) {
            deleteRow($RowEditing);
        }
        endEditing();
    }

    function toggle(e) {
        if (e.target.nodeName === "BUTTON" && e.type === "dblclick") {
            return;
        }

        e.preventDefault();

        var $tr = $(e.target).closest("tr");
        if ($RowEditing && $RowEditing[0] !== $tr[0]) {
            saveChanges();
        }

        if (!$RowEditing) {
            editRow($tr);
        } else {
            saveChanges();
        }
    }

    function deleteRow($tr) {
        $tr.remove();
    }

    //=========== Build table methods ===========\\

    function addActionsCell($bodyRow) {
        var $actionsRow = $bodyRow.append($("<td class='td-actions'>")).children("td:last");
        var $buttonsGroup = $actionsRow.append($("<div class='btn-group'>")).children("div");

        function addButton($parent, classes) {
            var $actionButton = $parent.append("<button>").children("button:last");
            return $actionButton.addClass("btn btn-sm btn-glyph fa " + classes);
        }

        addButton($buttonsGroup, "btn-edit btn-outline-secondary fa-pencil").on("click", function (e) {
            toggle(e);
        });
        addButton($buttonsGroup, "btn-remove btn-outline-danger fa-trash").on("click", function (e) {
            e.preventDefault();
            if ($RowEditing) {
                cancelChanges();
            } else {
                var $tr = $(e.target).closest("tr");
                deleteRow($tr);
            }
        });
    }

    function addRow($body, colNumber, rowData, isNewRow) {
        var $bodyRow = $body.append("<tr>").children("tr:last");
        if (isNewRow) {
            $bodyRow.attr("is-new-row", "yes");
        }
        for (var colIdx = 1; colIdx <= colNumber; colIdx++) {
            var colKey = "col" + colIdx;
            var cellValue = rowData ? rowData[colKey] : "";
            $bodyRow.append($("<td>").attr("data-field", colKey).text(cellValue));
        }
        addActionsCell($bodyRow);
        return $bodyRow;
    }

    function buildRows($table, jsonValue) {
        var $body = $table.append("<tbody>").children("tbody").sortable({
            delay: 100,
            distance: 5,
            cursor: "move"
        });

        var colNumber = jsonValue.colNumber;
        var rowNumber = jsonValue.values.length;
        for (var rowIdx = 0; rowIdx < rowNumber; rowIdx++) {
            var rowData = jsonValue.values[rowIdx];
            addRow($body, colNumber, rowData);
        }
    }

    function buildTable($element) {
        var $table = $element.append("<table>").children("table");
        $table.addClass("table table-sm table-hover");
        return $table;
    }

    function buildHeaders($table, jsonValue) {
        var $headRow = $table.append("<thead class='card-header'><tr>").find("tr");

        var colNumber = jsonValue.colNumber;
        for (var colIdx = 1; colIdx <= colNumber; colIdx++) {
            var colKey = "col" + colIdx;
            var colName = jsonValue[colKey];
            $headRow.append($("<td>").text(colName));
        }

        $headRow.append($("<td>")).children("td:last").
            addClass("th-actions").append($("<button>")).children("button").
            addClass("btn btn-link").text("Add new row").on("click", function (e) {
                e.preventDefault();

                if ($RowEditing) {
                    saveChanges();
                }

                var $body = $table.children("tbody");
                var $bodyRow = addRow($body, colNumber, null, true);
                editRow($bodyRow);
            });

        return colNumber;
    }

    function initTable($table) {
        $table.find("tbody>tr").mouseover(function () {
            $table.find("tbody>tr>td").removeClass("mouseover");
            $(this).find(".td-actions").addClass("mouseover");
        });
        $table.find("tbody>tr").mouseleave(function () {
            $(this).find(".td-actions").removeClass("mouseover");
        });
        $table.bind("dblclick", toggle);
    }

    //=========== PropertyTable base methods ===========\\

    UI.PropertyTable.prototype.getPropertyType = function () {
        return "ui-property-table";
    };

    UI.PropertyBase.prototype.value = function (val) {
        var me = this;
        if (val || val === "" || val === 0) {
            me.Element.val(val).change();
        }
        return me.Element.attr("value");
    };

    UI.PropertyTable.prototype.afterChangeType = function ($element) {
        var me = this;

        $element.addClass("settings-table-wrap");

        var value = me.value();
        var jsonValue = JSON.parse(value);   

        $Table = buildTable($element);
        $ColNumber = buildHeaders($Table, jsonValue);
        buildRows($Table, jsonValue);
        initTable($Table);
    };

})(window.UI = window.UI || {});