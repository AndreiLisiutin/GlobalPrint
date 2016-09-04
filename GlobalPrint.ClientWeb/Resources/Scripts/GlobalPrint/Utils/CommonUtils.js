GlobalPrint.namespace('GlobalPrint.Utils.CommonUtils');
(function (CommonUtils) {

    CommonUtils.isUserAuthenticated = function () {
        var isAuthenticated = $("body").data('isAuthenticated');
        return isAuthenticated && isAuthenticated.toLowerCase() == 'true';
    };

    CommonUtils.getUserID = function () {
        var userId = $("body").data('userId');
        return userId && parseInt(userId);
    };

    //geolocate user's position and perform callback with it
    CommonUtils.geolocate = function (successCallback, failureCallback) {
        failureCallback = failureCallback || function (error) {
            console.log("Ошибка определения текущего положения." + error.message);
        };
        successCallback = successCallback || function() {};


        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(successCallback, failureCallback, {
                enableHighAccuracy: true,
                timeout: 5000,
                maximumAge: 0
            });
        } else {
            failureCallback({ message: "Отключено определение текущего положения."});
        }
    };
})(GlobalPrint.Utils.CommonUtils);