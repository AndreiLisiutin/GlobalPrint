$(function () {
    var notificationhub = $.connection.pushNotificationHub;
    notificationhub.client.displayMessage = window.GlonalPrint.PushNotifications.notify;
    $.connection.hub.start();
});