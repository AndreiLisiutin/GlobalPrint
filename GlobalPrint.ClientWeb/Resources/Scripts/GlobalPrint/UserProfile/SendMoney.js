GlobalPrint.namespace('GlobalPrint.UserProfile.SendMoney');
(function (SendMoney) {
    SendMoney.defineValidation = function () {
        var userAmountOfMoney = $('#userAmountOfMoney').val();
        $("#sendModeyForm").validate({
            rules: {
                "AmountOfMoney": {
                    required: true,
                    decimal: true,
                    max: userAmountOfMoney,
                    min: 1,
                },
                "ReceiverUserId": {
                    required: true,
                    notEqual: '#senderUserId',
                },
            },
            messages: {
                "AmountOfMoney": {
                    max: 'Недостаточно средств на счету. Укажите сумму не более {0} руб.',
                },
                "ReceiverUserId": {
                    notEqual: 'Нельзя пересылать деньги со своего счета себе же. Выберите другого пользователя.',
                },
            }
        });
    };

})(GlobalPrint.UserProfile.SendMoney);



$(document).ready(function () {
    GlobalPrint.UserProfile.SendMoney.defineValidation();
});