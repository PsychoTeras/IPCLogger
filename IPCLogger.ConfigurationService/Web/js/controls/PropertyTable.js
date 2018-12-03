(function (UI) {

    UI.PropertyTable = function () {
        var me = this;
        UI.PropertyBase.call(me);
    };

    UI.PropertyTable.prototype = Object.create(UI.PropertyBase.prototype);
    UI.PropertyTable.prototype.constructor = UI.PropertyTable;

    UI.PropertyTable.prototype.getPropertyType = function () {
        return "ui-property-table";
    };

    UI.PropertyBase.prototype.value = function (val) {
        var me = this;
        if (val || val === "" || val === 0) {
            me.Element.val(val).change();
        }
        return me.Element.attr("value")
    };

    function buildTable($element) {
        var $table = $element.append("<table>").find("table");
        $table.addClass("table table-sm table-hover");
        return $table;
    }

    function buildHeaders($table, jsonValue) {
        var $headRow = $table.append("<thead class='card-header'><tr>").find("tr");

        var colNumber = jsonValue.colNumber;
        for (var colIdx = 1; colIdx <= colNumber; colIdx++) {
            var colKey = "col" + colIdx;
            var colName = jsonValue[colKey];
            $headRow.append($("<td class='td-autosize'>").text(colName));
        }
    }

    function buildRows($table, jsonValue) {
        var $body = $table.append("<tbody>").find("tbody");

        var colNumber = jsonValue.colNumber;
        var rowNumber = jsonValue.values.length;
        for (var rowIdx = 0; rowIdx < rowNumber; rowIdx++) {
            var $bodyRow = $body.append("<tr>").find("tr:last");
            var rowData = jsonValue.values[rowIdx];
            for (var colIdx = 1; colIdx <= colNumber; colIdx++) {
                var colKey = "col" + colIdx;
                var cellValue = rowData[colKey];
                $bodyRow.append($("<td class='td-autosize'>").text(cellValue));
            }
        }
    }

    UI.PropertyTable.prototype.afterChangeType = function ($element) {
        var me = this;

        $element.addClass("settings-table-wrap");

        var value = me.value();
        var jsonValue = JSON.parse(value);   

        var $table = buildTable($element);
        buildHeaders($table, jsonValue);
        buildRows($table, jsonValue);
    };

})(window.UI = window.UI || {});