GlobalPrint.namespace('GlobalPrint.Account.ResetPassword');

(function (ResetPassword) {

    ResetPassword.defineValidation = function () {
        $("#resetPasswordForm").validate({
            rules: {
                "Email": {
                    required: true,
                    email: true
                },
                "Password": {
                    required: true,
                    minlength: 6
                },
                "ConfirmPassword": {
                    required: true,
                    equalTo: "#userPassword"
                }
            }
        });
    };

})(GlobalPrint.Account.ResetPassword);

$(document).ready(function () {
    GlobalPrint.Account.ResetPassword.defineValidation();
});