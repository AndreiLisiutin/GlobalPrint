GlobalPrint.namespace('GlobalPrint.Utils.Services');
(function (Services) {

    Services.isUserAuthenticated = function () {
        return $("body").data('isAuthenticated');
    };
})(GlobalPrint.Utils.Services);