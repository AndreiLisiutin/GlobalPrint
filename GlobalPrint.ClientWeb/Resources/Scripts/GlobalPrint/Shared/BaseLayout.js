GlobalPrint.namespace('GlobalPrint.Shared.BaseLayout');
//usage of tooltip.js
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
(function (BaseLayout, GlobalPrint) {
   var notificationhub = $.connection.pushNotificationHub;

    notificationhub.client.displayMessage = GlobalPrint.Shared.PushNotifications.notify;
    notificationhub.client.updateIncomingOrdersCount = GlobalPrint.Shared.PushNotifications.updateIncomingOrdersCount;
    $.connection.hub.start();

    //Initialization of fileupload
    GlobalPrint.Shared.FileUpload.initFileUpload();

})(GlobalPrint.Shared.BaseLayout, GlobalPrint);
