GlobalPrint.namespace('GlobalPrint.Account.Login');

(function (Login) {

    Login.defineValidation = function () {
        $("#loginForm").validate({
            rules: {
                "Email": {
                    required: true,
                    email: true
                },
                "Password": {
                    required: true,
                    minlength: 6
                }
            }
        });
    };

})(GlobalPrint.Account.Login);

$(document).ready(function () {
	GlobalPrint.Account.Login.defineValidation();
	if ($(".setClick").is(":checked")) {
		$(".loginCheckBox").toggleClass('checked');
	}
});