function ApplicationController() {

    this.manageApplication = function (applicationId, defaultTab) {
        var url = getApiUrl("applications", applicationId) +
            ("#" + (defaultTab || globalSetting.LOGGERS_DEFAULT_TAB));
        navigate(url);
    };

}

window.ApplicationController = new ApplicationController();