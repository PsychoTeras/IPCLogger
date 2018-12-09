function ApplicationController() {

    this.manageApplication = function (applicationId) {
        location.href = getApiUrl("applications", applicationId);
    };

}

window.ApplicationController = new ApplicationController();