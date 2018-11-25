(function ($) {

    var langs = {
        en: {
            day: "day",
            hour: "hour",
            minute: "minute",
            second: "second",
            days: "days",
            hours: "hours",
            minutes: "minutes",
            seconds: "seconds"
        }
    };

    $.fn.durationPicker = function (options) {

        var totalDuration = 0;

        var defaults = {
            lang: "en",
            showSeconds: false,
            showDays: true
        };

        var settings = $.extend({}, defaults, options);

        var $mainInputReplacer;
        var $mainInputContainer;

        this.each(function (_, mainInput) {

            var $mainInput = $(mainInput);

            if ($mainInput.data("bdp") === "1") {
                return;
            }

            function buildDisplayBlock(id, hidden) {
                return '<div class="bdp-block ' + (hidden ? "hidden" : "") + '">' +
                    '<span id="bdp-' + id + '"></span>' +
                    '<span class="bdp-label" id="' + id + '_label"></span>' +
                    "</div>";
            }

            $mainInputReplacer = $('<div class="bdp-input"></div>');
            $mainInputReplacer.append('<div class="bdp-container" data-toggle="popover"></div>');
            $mainInputContainer = $mainInputReplacer.children();
            $mainInputContainer.append(buildDisplayBlock("days", !settings.showDays));
            $mainInputContainer.append(buildDisplayBlock("hours"));
            $mainInputContainer.append(buildDisplayBlock("minutes"));
            $mainInputContainer.append(buildDisplayBlock("seconds", !settings.showSeconds));

            $mainInput.after($mainInputReplacer).hide().data("bdp", "1");

            var inputs = [];

            var disabled = false;
            if ($mainInput.hasClass("disabled") || $mainInput.attr("disabled") === "disabled") {
                disabled = true;
                $mainInputReplacer.addClass("disabled");
            }

            function updateMainInput() {
                $mainInput.val(totalDuration.totalSeconds());
                $mainInput.change();
            }

            function updateMainInputReplacer() {
                $mainInputReplacer.find("#bdp-days").text(totalDuration.days());
                $mainInputReplacer.find("#bdp-hours").text(totalDuration.hours());
                $mainInputReplacer.find("#bdp-minutes").text(totalDuration.minutes());
                $mainInputReplacer.find("#bdp-seconds").text(totalDuration.seconds());

                $mainInputReplacer.find("#days_label").text(langs[settings.lang][totalDuration.days() === 1 ? "day" : "days"]);
                $mainInputReplacer.find("#hours_label").text(langs[settings.lang][totalDuration.hours() === 1 ? "hour" : "hours"]);
                $mainInputReplacer.find("#minutes_label").text(langs[settings.lang][totalDuration.minutes() === 1 ? "minute" : "minutes"]);
                $mainInputReplacer.find("#seconds_label").text(langs[settings.lang][totalDuration.seconds() === 1 ? "second" : "seconds"]);
            }

            function updatePicker() {
                if (!disabled) {
                    inputs.days.val(totalDuration.days());
                    inputs.hours.val(totalDuration.hours());
                    inputs.minutes.val(totalDuration.minutes());
                    inputs.seconds.val(totalDuration.seconds());
                }
            }

            function init() {
                if (!$mainInput.val()) {
                    $mainInput.val(0);
                }

                totalDuration = TimeSpan.FromSeconds(parseInt($mainInput.val(), 10));
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

                totalDuration = new TimeSpan({
                    seconds: getInputValue(inputs.seconds),
                    minutes: getInputValue(inputs.minutes),
                    hours: getInputValue(inputs.hours),
                    days: getInputValue(inputs.days)
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
                var $picker = $('<div class="bdp-popover"></div>');
                buildNumericInput("days", !settings.showDays, 364).appendTo($picker);
                buildNumericInput("hours", false, 23).appendTo($picker);
                buildNumericInput("minutes", false, 59).appendTo($picker);
                buildNumericInput("seconds", !settings.showSeconds, 59).appendTo($picker);

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