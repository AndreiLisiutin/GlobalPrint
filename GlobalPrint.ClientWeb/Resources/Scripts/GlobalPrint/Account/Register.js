GlobalPrint.namespace('GlobalPrint.Account.Register');

(function (Register) {

    Register.defineValidation = function () {
        $("#registerForm").validate({
            rules: {
                "Email": {
                    required: true,
                    email: true
                },
                "UserName": {
                    required: true
                },
                "Password": {
                    required: true,
                    minlength: 6
                },
                "ConfirmPassword": {
                    required: true,
                    equalTo: "#userPassword"
                },
                "IsAgreeWithOffer": {
                    required: true
                }
            }
        });
    };

})(GlobalPrint.Account.Register);

$(document).ready(function () {
	GlobalPrint.Account.Register.defineValidation();
	if ($(".setClick").is(":checked")) {
		$(".loginCheckBox").toggleClass("checked");
	}
});