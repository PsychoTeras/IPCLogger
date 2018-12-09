function PopupController() {

    this.addLogger = function (applicationId, onReady) {
        var url = getApiUrl("applications", applicationId, "popupAddLogger");
        asyncQuery(
            url,
            "GET",
            "html",
            onReady);
    };

}

window.PopupController = new PopupController();