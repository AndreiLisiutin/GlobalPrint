$(function () {
    var notificationhub = $.connection.pushNotificationHub;
    notificationhub.client.displayMessage = window.GlonalPrint.PushNotifications.notify;
    notificationhub.client.updateIncomingOrdersCount = window.GlonalPrint.PushNotifications.updateIncomingOrdersCount;
    $.connection.hub.start();
});