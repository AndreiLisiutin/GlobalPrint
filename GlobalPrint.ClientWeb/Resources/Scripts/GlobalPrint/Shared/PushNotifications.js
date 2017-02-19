GlobalPrint.namespace('GlobalPrint.Shared.PushNotifications');
(function (PushNotifications, CommonUtils) {

    // audio file name to play
    var audioFile = "../Resources/Sounds/notification";
    var alarmAudioFile = "../Resources/Sounds/alarm";

    // Update incoming prders count
    PushNotifications.updateIncomingOrdersCount = function (count) {
        $("#incomingOrdersCountBadge").text(count > 0 ? count : null);
    };

    /**
     * Воспроизвести бесконечный звонок будильника при наличии хотя бы 1 необработанного входящего заказа.
     * @param {!number} ordersCount Количество входящих заказов.
     */
    PushNotifications.setIncomingOrdersAlarm = function (ordersCount) {
        var elementId = "recievedOrdersSound";
        if (ordersCount > 0) {
            CommonUtils.playSound(elementId, alarmAudioFile, true);
        }
    };

    PushNotifications.notify = function (message, url) {
        // we are using FCM
        return;
        PushNotifications.displayMessage(message, url);
        CommonUtils.playSound("sound", audioFile);
    };

    PushNotifications.displayMessage = function (message, url) {
        var obj = {
            icon: 'glyphicon glyphicon-info-sign',
            message: message,            
            target: "_self"
        }
        if (url) {
            obj.url = url;
        }

        $.notify(obj, {
            type: "info",
            allow_dismiss: true,
            newest_on_top: false,
            delay: 10000,
            placement: {
                from: "bottom",
                align: "right"
            }
        });
    };

}(GlobalPrint.Shared.PushNotifications, GlobalPrint.Utils.CommonUtils));