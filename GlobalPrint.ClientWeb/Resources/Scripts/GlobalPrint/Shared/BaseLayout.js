GlobalPrint.namespace('GlobalPrint.Shared.BaseLayout');
(function (BaseLayout, GlobalPrint) {
    var notificationhub = $.connection.pushNotificationHub;

    notificationhub.client.displayMessage = GlobalPrint.Shared.PushNotifications.notify;
    notificationhub.client.updateIncomingOrdersCount = GlobalPrint.Shared.PushNotifications.updateIncomingOrdersCount;
    notificationhub.client.setIncomingOrdersAlarm = GlobalPrint.Shared.PushNotifications.setIncomingOrdersAlarm;
    $.connection.hub.start();

    //Initialization of fileupload
    GlobalPrint.Shared.FileUpload.initFileUpload();

    // Initialize swithers like in iOS
    GlobalPrint.Utils.CommonUtils.initializeSwitchers();


    GlobalPrint.Utils.CommonUtils.makeProgressBar();
    GlobalPrint.Utils.CommonUtils.disabledCheckboxFix();
    GlobalPrint.Utils.CommonUtils.makeValidation();
    GlobalPrint.Utils.CommonUtils.initializeClearableInputs();

    $(document).ready(function () {
        //usage of tooltip.js
        $('[data-toggle="tooltip"]').tooltip();
        GlobalPrint.Utils.CommonUtils.makeLookups();
        GlobalPrint.Utils.CommonUtils.makeClockPickers();
    });

})(GlobalPrint.Shared.BaseLayout, GlobalPrint);
