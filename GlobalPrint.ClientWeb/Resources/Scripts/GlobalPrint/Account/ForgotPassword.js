GlobalPrint.namespace('GlobalPrint.Account.ForgotPassword');

(function (ForgotPassword) {

    ForgotPassword.defineValidation = function () {
        $("#forgotPasswordForm").validate({
            rules: {
                "Email": {
                    required: true,
                    email: true
                }
            }
        });
    };

})(GlobalPrint.Account.ForgotPassword);

$(document).ready(function () {
    GlobalPrint.Account.ForgotPassword.defineValidation();
});