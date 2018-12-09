(function (UI) {

    var $Table = null;
    var $RowEditing = null;
    var $ColsNumber = null;
    var $ColSettings = [];

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

    function captureKeys(e) {
        if (e.which === 13) {
            saveChanges();
        } else if (e.which === 27) {
            cancelChanges();
        }
    }

    function beginEditing($currentCell) {
        $Table.addClass("editing");
        $RowEditing.addClass("editing");
        $RowEditing.find(".td-actions .btn-edit").removeClass("fa-pencil").addClass("fa-save");
        $RowEditing.find(".td-actions .btn-remove").removeClass("fa-trash").addClass("fa-remove");
        $($currentCell && $currentCell.children(".td-edit")[0] || $RowEditing.find(".td-edit:first")[0]).focus();
    }

    function editRow($tr, $currentCell) {
        $("td[data-field]", $RowEditing = $tr).each(function (colIdx) {
            var me = this;
            var $me = $(this);

            var value = !$me.attr("is-empty") ? $me.text() : "";
            var width = $me.width();
            var colKey = "col" + (colIdx + 1);
            var colSettings = $ColSettings[colKey];

            $me.empty().width(width);

            var input;
            if (!colSettings.values) {
                input = $('<input type="text" />').
                    val(value).
                    data("old-value", value).
                    dblclick(captureEvent);
            } else {
                input = $("<select/>");

                $.each(colSettings.values, function() {
                    $("<option/>").
                        text(this).
                        appendTo(input);
                });

                input.
                    val(value).
                    data("old-value", value).
                    dblclick(captureEvent);
            }

            input.addClass("td-edit");
            input.appendTo(me);
            input.keydown(captureKeys);
        });

        beginEditing($currentCell);
    }

    function endEditing() {
        $Table.removeClass("editing");
        $RowEditing.removeClass("editing");
        $RowEditing.find(".td-actions .btn-edit").removeClass("fa-save").addClass("fa-pencil");
        $RowEditing.find(".td-actions .btn-remove").removeClass("fa-remove").addClass("fa-trash");
        $RowEditing.removeAttr("is-new-row");
        $RowEditing = null;
    }

    function setTdValue($td, value) {
        $td.empty().text(value || "empty");
        if (!value) {
            $td.attr("is-empty", "yes");
        }
        else {
            $td.removeAttr("is-empty");
        }
    }

    function saveChanges() {
        $("td[data-field]", $RowEditing).each(function () {
            var value = $(".td-edit", this).val();
            setTdValue($(this), value);
        });
        endEditing();
    }

    function cancelChanges() {
        $("td[data-field]", $RowEditing).each(function () {
            var value = $(".td-edit", this).data("old-value");
            setTdValue($(this), value);
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
            var $currentCell = $(document.elementFromPoint(e.clientX, e.clientY));
            editRow($tr, $currentCell);
        } else {
            saveChanges();
        }
    }

    function deleteRow($tr) {
        var $body = $tr.parent();
        $tr.remove();
        displayNoRowMessage($body);
    }

    //=========== Build table methods ===========\\

    function displayNoRowMessage($body) {
        if (!$body.children().length) {
            $body.append($("<tr>")).children("tr").
                append($("<td class='no-records'>")).
                append($("<td class='no-records'>")).children("td:last").
                text("No records");
            $Table.children("tbody").sortable("option", "disabled", true);
        }
    }

    function hideNoRowMessage($body) {
        if ($body.find(".no-records").length) {
            $body.empty();
            $Table.children("tbody").sortable("option", "disabled", false);
        }
    }

    function addActionsCell($bodyRow) {
        var $actionsRow = $bodyRow.append($("<td class='td-actions'>")).children("td:last");
        var $buttonsGroup = $actionsRow.append($("<div class='btn-group'>")).children("div");

        function addButton($parent, classes) {
            return $parent.append("<button>").children("button:last").
                addClass("btn btn-sm btn-glyph fa " + classes).
                on("keydown", captureKeys);
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

    function addRow($body, colsNumber, rowData, isNewRow) {
        hideNoRowMessage($body);

        var $bodyRow = $body.append("<tr>").children("tr:last");
        if (isNewRow) {
            $bodyRow.attr("is-new-row", "yes");
        }
        for (var colIdx = 1; colIdx <= colsNumber; colIdx++) {
            var colKey = "col" + colIdx;
            var cellValue = rowData ? rowData[colKey] : "";
            var $td = $("<td>").attr("data-field", colKey);
            $bodyRow.append($td);
            setTdValue($td, cellValue);
        }
        addActionsCell($bodyRow);

        $bodyRow.mouseover(function () {
            $body.find("tr td").removeClass("mouseover");
            $(this).find(".td-actions").addClass("mouseover");
        }).mouseleave(function () {
            $(this).find(".td-actions").removeClass("mouseover");
        });

        return $bodyRow;
    }

    function buildRows($table, jsonData) {
        var $body = $table.append("<tbody>").children("tbody").sortable({
            delay: 100,
            distance: 5,
            cursor: "move"
        });

        var colsNumber = jsonData.colsNumber;
        var rowsNumber = jsonData.values.length;
        for (var rowIdx = 0; rowIdx < rowsNumber; rowIdx++) {
            var rowData = jsonData.values[rowIdx];
            addRow($body, colsNumber, rowData);
        }

        displayNoRowMessage($body);
    }

    function setRows($table, jsonData, colsNumber) {
        var $body = $table.children("tbody");
        $body.empty();

        var rowsNumber = jsonData.length;
        for (var rowIdx = 0; rowIdx < rowsNumber; rowIdx++) {
            var rowData = jsonData[rowIdx];
            addRow($body, colsNumber, rowData);
        }

        displayNoRowMessage($body);
    }

    function buildTable($element) {
        var $table = $element.append("<table>").children("table");
        $table.addClass("table table-sm table-hover");
        return $table;
    }

    function buildHeaders($table, jsonData) {
        var $headRow = $table.append("<thead class='card-header'><tr>").find("tr");

        var colsNumber = jsonData.colsNumber;
        var width = Math.round(100 / colsNumber);
        for (var colIdx = 1; colIdx <= colsNumber; colIdx++) {
            var colKey = "col" + colIdx;
            var colName = jsonData[colKey];
            $headRow.append($("<td>").css("width", width + "%").text(colName));
        }

        $headRow.append($("<td>")).children("td:last").
            addClass("th-actions").append($("<button>")).children("button").
            addClass("btn btn-link").text("Add new record").on("click", function (e) {
                e.preventDefault();

                if ($RowEditing) {
                    if ($RowEditing.attr("is-new-row") &&
                        !$RowEditing.find(".td-edit").filter(function() { return this.value; }).length) {
                        $RowEditing.find(".td-edit:first").focus();
                        return;
                    }
                    saveChanges();
                }

                var $body = $table.children("tbody");
                var $bodyRow = addRow($body, colsNumber, null, true);
                editRow($bodyRow);
            });

        return colsNumber;
    }

    function initTable($table) {
        $table.bind("dblclick", toggle);
    }

    function readColSettings(jsonData) {
        var colSettings = [];

        var colsNumber = jsonData.colsNumber;
        for (var colIdx = 1; colIdx <= colsNumber; colIdx++) {

            var colKey = "col" + colIdx;

            //Expand %col#Values% pattern
            var colValuesKey = colKey + "Values";
            var colValues = jsonData[colValuesKey], regexColValues;
            if (colValues && (regexColValues = /%(col\dValues)%/.exec(colValues))) {
                colValues = jsonData[regexColValues[1]];
            }

            //Store row settings
            colSettings[colKey] = {
                values: CSVToArray(colValues)
            };
        }

        return colSettings;
    }

    //=========== PropertyTable base methods ===========\\

    function getValue() {
        if ($RowEditing) {
            saveChanges();
        }

        var result = [];
        $Table.find("tbody tr").each(function () {
            var $tr = $(this);
            var item = {};
            $tr.find("td:not(.td-actions)").each(function (colIdx) {
                var $me = $(this);
                var cellValue = !$me.attr("is-empty") ? $me.text() : "";
                var colKey = "col" + (colIdx + 1);
                item[colKey] = cellValue;
            });
            result.push(item);
        });

        return JSON.stringify(result);
    }

    function setValue(value) {
        var jsonData = JSON.parse(value);
        if (jsonData) {
            setRows($Table, jsonData, $ColsNumber);
        }
    }

    UI.PropertyTable.prototype.getPropertyType = function () {
        return "ui-property-table";
    };

    UI.PropertyTable.prototype.value = function (val) {
        if (val || val === "") {
            setValue(val);
        }
        return getValue();
    };

    UI.PropertyTable.prototype.afterChangeType = function ($element) {
        var me = this;

        $element.addClass("settings-table-wrap");

        var value = me.Element.attr("value");
        var jsonData = JSON.parse(value);
        me.Element.removeAttr("value");

        $ColSettings = readColSettings(jsonData);
        $Table = buildTable($element);
        $ColsNumber = buildHeaders($Table, jsonData);
        buildRows($Table, jsonData);
        initTable($Table);
    };

})(window.UI = window.UI || {});