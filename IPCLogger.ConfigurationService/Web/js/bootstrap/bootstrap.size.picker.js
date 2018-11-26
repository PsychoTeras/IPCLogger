(function ($) {

    var langs = {
        en: {
            byte: "B",
            kbyte: "KB",
            mbyte: "MB",
            gbyte: "GB"
        }
    };

    $.fn.sizePicker = function (options) {

        var totalBytes = 0;

        var defaults = {
            lang: "en",
            showBytes: false,
            showGBytes: true
        };

        var settings = $.extend({}, defaults, options);

        var $mainInputReplacer;
        var $mainInputContainer;

        this.each(function (_, mainInput) {

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
            $mainInputContainer.append(buildDisplayBlock("bytes", !settings.showBytes));
            $mainInputContainer.append(buildDisplayBlock("kbytes"));
            $mainInputContainer.append(buildDisplayBlock("mbytes"));
            $mainInputContainer.append(buildDisplayBlock("gbytes", !settings.showGBytes));

            $mainInput.after($mainInputReplacer).hide().data("bsp", "1");

            var inputs = [];

            var disabled = false;
            if ($mainInput.hasClass("disabled") || $mainInput.attr("disabled") === "disabled") {
                disabled = true;
                $mainInputReplacer.addClass("disabled");
            }

            function updateMainInput() {
                $mainInput.val(totalBytes.totalSeconds());
                $mainInput.change();
            }

            function updateMainInputReplacer() {
                $mainInputReplacer.find("#bsp-bytes").text(totalBytes.bytes());
                $mainInputReplacer.find("#bsp-kbytes").text(totalBytes.kbytes());
                $mainInputReplacer.find("#bsp-mbytes").text(totalBytes.mbytes());
                $mainInputReplacer.find("#bsp-gbytes").text(totalBytes.gbytes());

                $mainInputReplacer.find("#bytes_label").text(langs[settings.lang]["B"]);
                $mainInputReplacer.find("#kbytes_label").text(langs[settings.lang]["KB"]);
                $mainInputReplacer.find("#mbytes_label").text(langs[settings.lang]["MB"]);
                $mainInputReplacer.find("#gbytes_label").text(langs[settings.lang]["GB"]);
            }

            function updatePicker() {
                if (!disabled) {
                    inputs.bytes.val(totalBytes.bytes());
                    inputs.kbytes.val(totalBytes.kbytes());
                    inputs.mbytes.val(totalBytes.mbytes());
                    inputs.gbytes.val(totalBytes.gbytes());
                }
            }

            function init() {
                if (!$mainInput.val()) {
                    $mainInput.val(0);
                }

                totalBytes = Time.FromBytes(parseInt($mainInput.val(), 10));
                updateMainInputReplacer();
                updatePicker();
            }

            function pickerChanged(e) {

                if (e.keyCode === 13 || e.keyCode === 27) {
                    $mainInputContainer.popover("hide");
                    return;
                }

                function getInputValue(input) {
                    var value = parseInt(input.val());
                    if (input[0].min !== undefined) {
                        value = Math.max(value, parseInt(input[0].min));
                    }
                    if (input[0].max !== undefined) {
                        value = Math.min(value, parseInt(input[0].max));
                    }
                    return value;
                }

                totalBytes = new Size({
                    bytes: getInputValue(inputs.bytes),
                    kbytes: getInputValue(inputs.kbytes),
                    mbytes: getInputValue(inputs.mbytes),
                    gbytes: getInputValue(inputs.gbytes)
                });
                updateMainInputReplacer();
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
                return $ctrl.prepend($input);
            }

            if (!disabled) {
                var $picker = $('<div class="bsp-popover"></div>');
                buildNumericInput("bytes", !settings.showBytes, 1024).appendTo($picker);
                buildNumericInput("kbytes", false, 1000).appendTo($picker);
                buildNumericInput("mbytes", false, 1000).appendTo($picker);
                buildNumericInput("gbytes", !settings.showGBytes, 1000).appendTo($picker);

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