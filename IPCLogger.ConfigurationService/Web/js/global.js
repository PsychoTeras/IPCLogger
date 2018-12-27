(function() {
    var regexStackGetCaller = new RegExp("\\s+at\\s(.*?)\\s");

    //JQuery patches
    $.fn.hasAttr = function (name) {
        var attr = $(this).attr(name);
        return typeof attr !== typeof undefined && attr !== false;
    };

    //Window patches
    $(function () {
        $(window).on("mousedown", function (e) {
            var $target = $(e.target);
            var inPopover = $target.closest(".popover").length > 0;
            if (!inPopover) {
                $(".popover").popover("hide");
            }
        });

        bindMouseWheelIncrement($("input[type='number']"));
    });

    $(document).on({
        ajaxStart: function () {
            lockPage();
        },
        ajaxStop: function () {
            unlockPage();
        },
        mousemove: function (e) {
            if (document.body) {
                sessionStorage.setItem("mouseX",
                    e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft);
                sessionStorage.setItem("mouseY",
                    e.clientY + document.body.scrollTop + document.documentElement.scrollTop);
            }
        }
    });

    window.initToolBar = function () {
        var $toolbarLeft = $(".btn-toolbar-user-left");
        if ($toolbarLeft.length) {
            $(".btn-toolbar-left").append($toolbarLeft.children());
            $toolbarLeft.remove();
        }
        var $toolbarCenter = $(".btn-toolbar-user-center");
        if ($toolbarCenter.length) {
            $(".btn-toolbar-center").append($toolbarCenter.children());
            $toolbarCenter.remove();
        }
        var $toolbarRight = $(".btn-toolbar-user-right");
        if ($toolbarCenter.length) {
            $(".btn-toolbar-right").append($toolbarRight.children());
            $toolbarRight.remove();
        }
        $(".btn-toolbar #btn-go-back").on("click", function () {
            var url = getApiUrl("backurl");
            asyncQuery(url,
                "GET",
                "text",
                function (backUrl) {
                    if (backUrl) {
                        navigate(backUrl);
                    }
                });
        });
    };

    window.bindMouseWheelIncrement = function($input, onChange) {
        $input.bind("mousewheel", function (e) {
            if (!$(this).is(":focus")) {
                return true;
            }

            function validateInputValue(input, value) {
                value = Math.max(value, parseInt(input.min || 0));
                value = Math.min(value, parseInt(input.max || value));
                return value;
            }

            var delta = e.originalEvent.wheelDelta, value;
            if (delta > 0) {
                value = parseInt(this.value) + 1;
            } else {
                value = parseInt(this.value) - 1;
            }

            value = validateInputValue(this, value);

            if (value !== this.value) {
                this.value = validateInputValue(this, value);
                if (onChange) {
                    onChange($input);
                }
            }

            return false;
        });
    };

    window.mouseX = function () {
        return sessionStorage.getItem("mouseX");
    };

    window.mouseY = function () {
        return sessionStorage.getItem("mouseY");
    };

    window.lockPage = function () {
        $("body").addClass("loading locked");
    };

    window.unlockPage = function () {
        $("body").removeClass("loading locked");
    };

    window.onbeforeunload = function() {
        lockPage();

        var apiTrackUrl = getApiUrl("trackurl");
        var trackedUrl = location.href.replace(location.origin, "");
        syncQuery(apiTrackUrl, "POST", "text", trackedUrl);
    };

    window.getParameterByName = function(name, url) {
        if (!url) {
            url = window.location.href;
        }
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"), results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return "";
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    };

    window.showDialog = {
        confirmation: function(message, onClose) {
            bootbox.confirm({
                message: message,
                buttons: {
                    cancel: {
                        label: "No",
                        className: "btn-default"
                    },
                    confirm: {
                        label: "Yes",
                        className: "btn-success"
                    }
                },
                callback: onClose
            });
        },
        error: function(header, message) {
            bootbox.dialog({
                    message: "<div class=\"alert alert-danger\" role=\"alert\"><p><strong>" + header + "</strong></p>" + message + "</div>",
                    closeButton: false
                }).addClass("dialog-alert").
                on("click keydown", function() {
                    bootbox.hideAll();
                });
        }
    };

    window.getApiUrl = function() {

        function valueToGetParams(value) {
            var params = "", first = true;
            for (var key in value) {
                if (value.hasOwnProperty(key)) {
                    params += (first ? "?" : "&") + key + "=" + value[key];
                    first = false;
                }
            }
            return params;
        }

        var url = globalSetting.APP_URL;
        $.each(arguments,
            function (_, value) {
                if (value == null) {
                    return true;
                }
                if (value instanceof Object) {
                    url = url.concat(valueToGetParams(value));
                    return false;
                } else {
                    url = url.concat("/" + value);
                }
                return true;
            });
        return url;
    };

    window.asyncQuery = function (url, method, dataType, success, error, data) {

        success = success || function() {};
        var objToGetStack = new Error();

        error = error || function(xhr, _, thrownError) {
            var callerName = "";
            var stack = objToGetStack.stack;
            if (stack) {
                var callerStack = stack.split("\n");
                callerName = regexStackGetCaller.exec(callerStack[2])[1] + ": ";
            }

            var message = "";
            if (xhr.responseText) {
                message = xhr.responseText;
            } else if (xhr.responseJSON) {
                message = JSON.parse(xhr.responseJSON.Message);
            }

            showDialog.error(callerName + thrownError, message);
        };

        return $.ajax({
            url: url,
            cache: globalSetting.CACHE_AJAX_RESULT,
            dataType: dataType,
            type: method,
            success: success,
            error: error,
            data: data
        });
    };

    window.syncQuery = function (url, method, dataType, data) {
        method = method || "GET";
        return $.ajax({
            url: url,
            cache: globalSetting.CACHE_AJAX_RESULT,
            type: method,
            dataType: dataType,
            data: data,
            async: false
        });
    };

    window.showModal = function(popupDiv, html, options, onClose) {
        options = options || { backdrop: "static" };

        var popupSourceDiv = $(popupDiv).children(".modal-dialog");
        if (!popupSourceDiv.length) return;

        popupSourceDiv.html(html);

        popupDiv.modal(options).on("hidden.bs.modal", function(e) {
            if (onClose) {
                onClose(e.target);
            }
            popupSourceDiv.empty();
        });
    };

    window.CSVToArray = function (strData) {
        if (strData === null || strData === undefined) {
            return null;
        }

        var strDelimiter = ",";
        var objPattern = new RegExp(
            "(\\" + strDelimiter + "|^)" +
            "(?:\"([^\"]*(?:\"\"[^\"]*)*)\"|" +
            "([^\"\\" + strDelimiter + "]*))",
            "gi"
        );

        var arrData = [];

        var arrMatches;
        while ((arrMatches = objPattern.exec(strData)) !== null) {
            var strMatchedValue;
            if (arrMatches[2]) {
                strMatchedValue = arrMatches[2].replace(new RegExp("\"\"", "g"), "\"");
            } else {
                strMatchedValue = arrMatches[3];
            }
            arrData.push(strMatchedValue.trim());
        }

        return arrData;
    };

    window.navigate = function(url) {
        location.href = url;
    };
})();