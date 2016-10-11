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

    CommonUtils.lookup = function (id, callback) {
        var url = "/UserProfile/UsersListPartial?lookupType=" + id;
        var id = "modal_" + Math.floor(Math.random() * 100000000000);
        var str =
            '<div class="modal modal-fullscreen fade lookup-wrapper" id="' + id + '" tabindex="-1" role="dialog" aria-hidden="true">      '+
            '    <div class="modal-dialog">                                                                                               ' +
            '        <div class="modal-content">                                                                                          ' +
            '            <div class="modal-body">                                                                                         ' +
            '                <div class="modal-body-content">                                                                             ' +
            '                </div>                                                                                                       ' +
            '            </div>                                                                                                           ' +
            '        </div>                                                                                                               ' +
            '    </div>                                                                                                                   ';

        $('body').append(str);
        $('#' + id).on('show.bs.modal', function () {
            setTimeout(function () {
                $('.modal-backdrop').addClass("modal-backdrop-fullscreen");
                $('#' + id + ' .modal-body-content').load(url);
            }, 0);
        });
        $('#' + id).on('hidden.bs.modal', function () {
            $('#' + id + ' .modal-backdrop').addClass("modal-backdrop-fullscreen");
        });
        $('#' + id).on('setValue', function (event, data) {
            if (callback) {
                callback(event, data);
            }
        });
        return id;
    };
})(GlobalPrint.Utils.CommonUtils);