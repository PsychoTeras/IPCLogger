function ApplicationController() {

    this.manageApplication = function (applicationId, defaultPage) {
        var url = getApiUrl("applications", applicationId) +
            (defaultPage ? "#" + defaultPage : "#settings");
        navigate(url);
    };

}

window.ApplicationController = new ApplicationController();