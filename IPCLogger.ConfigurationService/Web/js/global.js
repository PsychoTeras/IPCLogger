(function() {
    var regexStackGetCaller = new RegExp("\\s+at\\s(.*?)\\s");

    $(document).on({
        ajaxStart: function() {
            $("body").addClass("loading");
        },
        ajaxStop: function() {
            $("body").removeClass("loading"); 
        },
        mousemove: function (e) {
            sessionStorage.setItem("mouseX", e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft);
            sessionStorage.setItem("mouseY", e.clientY + document.body.scrollTop + document.documentElement.scrollTop);
        }
    });

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
                    params = (first ? "?" : "&") + key + "=" + value[key];
                    first = false;
                }
            }
            return params;
        }

        var url = globalSetting.APP_URL;
        $.each(arguments,
            function(i, value) {
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

    window.asyncQuery = function(url, method, success, error, data, dataType) {

        success = success || function() {};
        var objToGetStack = new Error();

        error = error || function(xhr, ajaxOptions, thrownError) {
            var callerName = "";
            var stack = objToGetStack.stack;
            if (stack) {
                var callerStack = stack.split("\n");
                callerName = regexStackGetCaller.exec(callerStack[2])[1] + ": ";
            }

            var message = "";
            if (xhr.responseJSON) {
                message = xhr.responseJSON.Message;
            }

            showDialog.error(callerName + thrownError, message);
        };

        dataType = dataType || "json";

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

    window.syncQuery = function (url, method, dataType) {
        method = method || "GET";
        return $.ajax({
            url: url,
            cache: globalSetting.CACHE_AJAX_RESULT,
            type: method,
            dataType: dataType,
            async: false
        });
    };

    window.showModal = function(popupDiv, popupHtmlSourceUrl, options, onClose) {
        if (!popupDiv || !popupHtmlSourceUrl) return;
        options = options || { backdrop: "static" };

        var popupSourceDiv = $(popupDiv).children(".modal-dialog");
        if (!popupSourceDiv.length) return;

        popupSourceDiv.load(popupHtmlSourceUrl);

        popupDiv.modal(options).on("hidden.bs.modal", function(e) {
            if (onClose) {
                onClose(e.target);
            }
            popupSourceDiv.empty();
        });
    };
})();
