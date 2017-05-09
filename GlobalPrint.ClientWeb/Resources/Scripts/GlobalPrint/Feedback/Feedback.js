GlobalPrint.namespace('GlobalPrint.Feedback.Feedback');

(function (Feedback) {

    Feedback.defineValidation = function () {
        $("#feedbackForm").validate({
            rules: {
                "Email": {
                    required: true,
                    email: true
                },
                "UserName": {
                    required: true
                },
                "Subject": {
                    required: true
                },
                "Message": {
                    required: true
                }
            }
        });
    };

})(GlobalPrint.Feedback.Feedback);

$(document).ready(function () {
    GlobalPrint.Feedback.Feedback.defineValidation();
});