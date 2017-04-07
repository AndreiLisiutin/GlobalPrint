GlobalPrint.namespace('GlobalPrint.UserProfile.FillUpBalance');
(function (FillUpBalance) {
	FillUpBalance.defineValidation = function () {
		$("#fillUpBalanceForm").validate({
            rules: {
                "AmountOfMoney": {
                    required: true,
                    money: true
                }
            },
            messages: {
            }
        });
    };

})(GlobalPrint.UserProfile.FillUpBalance);



$(document).ready(function () {
	GlobalPrint.UserProfile.FillUpBalance.defineValidation();
});