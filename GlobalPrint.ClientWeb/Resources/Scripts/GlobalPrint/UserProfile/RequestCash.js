GlobalPrint.namespace('GlobalPrint.UserProfile.RequestCash');
(function (RequestCash) {
    RequestCash.defineValidation = function () {
        var userAmountOfMoney = parseInt($('#userAmountOfMoney').val());
        $("#sendModeyForm").validate({
            rules: {
                "AmountOfMoney": {
                    required: true,
                    money: true,
                    max: userAmountOfMoney,
                }
            },
            messages: {
                "AmountOfMoney": {
                    max: 'Недостаточно средств на счету. Укажите сумму не более {0} руб.',
                }
            }
        });
    };

})(GlobalPrint.UserProfile.RequestCash);



$(document).ready(function () {
    GlobalPrint.UserProfile.RequestCash.defineValidation();
});