function ApplicationController() {

    this.manageApplication = function (applicationId) {

        var url = getApiUrl("applications", applicationId);
        window.location.assign(url);
    };

}

window.ApplicationController = new ApplicationController();