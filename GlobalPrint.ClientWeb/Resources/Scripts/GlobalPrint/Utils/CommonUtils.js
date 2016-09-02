GlobalPrint.namespace('GlobalPrint.Utils.CommonUtils');
(function (CommonUtils) {

    CommonUtils.isUserAuthenticated = function () {
        var isAuthenticated = $("body").data('isAuthenticated');
        return isAuthenticated && isAuthenticated.toLowerCase() == 'true';
    };
})(GlobalPrint.Utils.CommonUtils);