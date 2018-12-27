(function () {
    var $ = jQuery;

    $.alert = function (message, options) {
        options = $.extend({}, $.alert.default_options, options);
        var $alert = $("<div>");
        $alert.attr("class", "alert");
        if (options.type) {
            $alert.addClass("alert-" + options.type);
        }

        $alert.append(message);
        if (options.top_offset) {
            options.offset = {
                from: "top",
                amount: options.top_offset
            };
        }

        var fadeIn = true;
        var offsetAmount = options.offset.amount;

        if (options.singleton) {
            $(".alert").each(function () {
                $(this).alert("close");
                fadeIn = false;
            });
        } else {
            $(".alert").each(function() {
                return offsetAmount = Math.max(offsetAmount,
                    parseInt($(this).css(options.offset.from)) + $(this).outerHeight() + options.stackup_spacing);
            });
        }
        var css = {
            "position": (options.ele === "body" ? "fixed" : "absolute")
        };
        css[options.offset.from] = offsetAmount + "px";
        $alert.css(css);
        if (options.width !== "auto") {
            $alert.css("width", options.width + "px");
        }
        $(options.ele).append($alert);
        switch (options.align) {
            case "center":
                $alert.css({
                    "left": "50%",
                    "margin-left": "-" + ($alert.outerWidth() / 2) + "px"
                });
                break;
            case "left":
                $alert.css("left", "20px");
                break;
            default:
                $alert.css("right", "20px");
        }

        if (fadeIn) {
            $alert.fadeIn();
        } else {
            $alert.show();
        }

        if (options.delay > 0) {
            $alert.delay(options.delay).fadeOut(function () {
                return $(this).alert("close");
            });
        }

        if (options.allow_dismiss) {
            $alert.on("click", function () {
                $(this).alert("close");
            });
        }

        return $alert;
    };

    $.alert.default_options = {
        ele: "body",
        type: "info",
        offset: {
            from: "bottom",
            amount: 20
        },
        align: "right",
        width: 250,
        delay: 2500,
        allow_dismiss: true,
        stackup_spacing: 10,
        singleton: true
    };

})();