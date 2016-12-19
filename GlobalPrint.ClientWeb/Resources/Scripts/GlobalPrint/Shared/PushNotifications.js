GlobalPrint.namespace('GlobalPrint.Shared.PushNotifications');
(function (PushNotifications) {

    // audio file name to play
    var audioFile = "../Resources/Sounds/notification";

    // Update incoming prders count
    PushNotifications.updateIncomingOrdersCount = function (count) {
        $("#incomingOrdersCountBadge").text(count > 0 ? count : null);
    };

    PushNotifications.notify = function (message, url) {
        // we are using FCM
        return;
        PushNotifications.displayMessage(message, url);
        PushNotifications.playSound();
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

    PushNotifications.playSound = function () {
        $("#sound")[0].innerHTML = '<audio autoplay="autoplay"><source src="' + audioFile + '.mp3" type="audio/mpeg" /><source src="' + audioFile + '.ogg" type="audio/ogg" /><embed hidden="true" autostart="true" loop="false" src="' + audioFile + '.mp3" /></audio>';
    };

}(GlobalPrint.Shared.PushNotifications));