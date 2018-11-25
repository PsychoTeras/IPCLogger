function ApplicationController() {

    this.manageApplication = function (applicationId) {
        location.href = getApiUrl("applications", applicationId);

        //asyncQuery(
        //    url,
        //    "GET",
        //    "html",
        //    function (data) {
        //        $("#partial-view-area").html(data);
        //    });
    };

}

window.ApplicationController = new ApplicationController();