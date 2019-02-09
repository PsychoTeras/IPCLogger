function ApplicationsController() {

    this.applicationSettings = function (applicationId, defaultTab) {
        var url = getApiUrl("applications", applicationId) +
            ("#" + (defaultTab || globalSetting.APPSETTINGS_DEFAULT_TAB));
        navigate(url);
    };

}

window.ApplicationsController = new ApplicationsController();