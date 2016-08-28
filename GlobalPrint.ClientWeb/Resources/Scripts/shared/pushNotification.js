window.GlonalPrint = window.GlonalPrint || {};
window.GlonalPrint.PushNotifications = GlonalPrint.PushNotifications || {};

(function (PushNotifications) {
    var audioFile = "../Resources/Sounds/notification";

    // Update incoming prders count
    PushNotifications.updateIncomingOrdersCount = function (count) {
        $("#incomingOrdersCountBadge").text(count > 0 ? count : null);
    };

    PushNotifications.notify = function (message) {
        PushNotifications.displayMessage(message);
        PushNotifications.playSound(audioFile);
    };

    PushNotifications.displayMessage = function (message) {
        $.notify({
            icon: 'glyphicon glyphicon-info-sign',
            message: message
        }, {
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

    PushNotifications.playSound = function (filename) {
        $("#sound")[0].innerHTML = '<audio autoplay="autoplay"><source src="' + filename + '.mp3" type="audio/mpeg" /><source src="' + filename + '.ogg" type="audio/ogg" /><embed hidden="true" autostart="true" loop="false" src="' + filename + '.mp3" /></audio>';
    };

}(window.GlonalPrint.PushNotifications));