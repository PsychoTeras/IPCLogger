(function ($) {

    var langs = {
        en: {
            bytes: "B",
            kbytes: "KB",
            mbytes: "MB",
            gbytes: "GB"
        }
    };

    $.fn.sizePicker = function (options) {

        var totalSize = 0;

        var defaults = {
            lang: "en",
            showBytes: false,
            showGBytes: true
        };

        var settings = $.extend({}, defaults, options);

        var $mainInputReplacer;
        var $mainInputContainer;

        return this.each(function (_, mainInput) {

            var $mainInput = $(mainInput);

            if ($mainInput.data("bsp") === "1") {
                return;
            }

            function buildDisplayBlock(id, hidden) {
                return '<div class="bsp-block ' + (hidden ? "hidden" : "") + '">' +
                    '<span id="bsp-' + id + '"></span>' +
                    '<span class="bsp-label" id="' + id + '_label"></span>' +
                    "</div>";
            }

            $mainInputReplacer = $('<div class="bsp-input"></div>');
            $mainInputReplacer.append('<div class="bsp-container" data-toggle="popover"></div>');
            $mainInputContainer = $mainInputReplacer.children();
            $mainInputContainer.append(buildDisplayBlock("gbytes", !settings.showGBytes));
            $mainInputContainer.append(buildDisplayBlock("mbytes"));
            $mainInputContainer.append(buildDisplayBlock("kbytes"));
            $mainInputContainer.append(buildDisplayBlock("bytes", !settings.showBytes));

            $mainInput.after($mainInputReplacer).hide().data("bsp", "1");

            var inputs = [];

            var disabled = false;
            if ($mainInput.hasClass("disabled") || $mainInput.attr("disabled") === "disabled") {
                disabled = true;
                $mainInputReplacer.addClass("disabled");
            }

            function updateMainInput() {
                $mainInput.val(totalSize.totalBytes());
                $mainInput.change();
            }

            function updateMainInputReplacer() {
                $mainInputReplacer.find("#bsp-bytes").text(totalSize.bytes());
                $mainInputReplacer.find("#bsp-kbytes").text(totalSize.kbytes());
                $mainInputReplacer.find("#bsp-mbytes").text(totalSize.mbytes());
                $mainInputReplacer.find("#bsp-gbytes").text(totalSize.gbytes());

                $mainInputReplacer.find("#bytes_label").text(langs[settings.lang]["bytes"]);
                $mainInputReplacer.find("#kbytes_label").text(langs[settings.lang]["kbytes"]);
                $mainInputReplacer.find("#mbytes_label").text(langs[settings.lang]["mbytes"]);
                $mainInputReplacer.find("#gbytes_label").text(langs[settings.lang]["gbytes"]);
            }

            function updatePicker() {
                if (!disabled) {
                    inputs.bytes.val(totalSize.bytes());
                    inputs.kbytes.val(totalSize.kbytes());
                    inputs.mbytes.val(totalSize.mbytes());
                    inputs.gbytes.val(totalSize.gbytes());
                }
            }

            function init() {
                if (!$mainInput.val()) {
                    $mainInput.val(0);
                }

                totalSize = Size.FromBytes(parseInt($mainInput.val(), 10));
                updateMainInputReplacer();
                updatePicker();
            }

            function pickerChanged(e) {

                if (e.keyCode === 13 || e.keyCode === 27) {
                    $mainInputContainer.popover("hide");
                    return;
                }

                function getInputValue($input) {
                    var input = $input[0];
                    var value = parseInt(input.value);
                    value = Math.max(value, parseInt(input.min || 0));
                    value = Math.min(value, parseInt(input.max || value));
                    return value;
                }
                totalSize = new Size({
                    bytes: getInputValue(inputs.bytes),
                    kbytes: getInputValue(inputs.kbytes),
                    mbytes: getInputValue(inputs.mbytes),
                    gbytes: getInputValue(inputs.gbytes)
                });

                updateMainInput();
            }

            function buildNumericInput(label, hidden, max) {
                var $input = $('<input class="form-control" type="number" min="0" value="0">').bind("change click keyup", pickerChanged);
                if (max) {
                    $input.attr("max", max);
                }
                inputs[label] = $input;
                var $ctrl = $("<div> " + langs[settings.lang][label] + "</div>");
                if (hidden) {
                    $ctrl.addClass("hidden");
                }
                bindMouseWheelIncrement($input, pickerChanged);
                return $ctrl.prepend($input);
            }

            if (!disabled) {
                var $picker = $('<div class="bsp-popover"></div>');
                buildNumericInput("gbytes", !settings.showGBytes, 1000).appendTo($picker);
                buildNumericInput("mbytes", false, 1023).appendTo($picker);
                buildNumericInput("kbytes", false, 1023).appendTo($picker);
                buildNumericInput("bytes", !settings.showBytes, 1023).appendTo($picker);

                $mainInputContainer.popover({
                    placement: "auto",
                    trigger: "click",
                    html: true,
                    content: $picker
                });
            }

            init();
            $mainInput.change(init);
        });
    };
}(jQuery));