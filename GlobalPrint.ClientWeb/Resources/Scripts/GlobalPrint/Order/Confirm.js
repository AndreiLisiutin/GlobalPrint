GlobalPrint.namespace('GlobalPrint.Order.Confirm');
(function (Confirm) {

    Confirm.defineValidation = function (isOrderAvailable) {
        $("#createOrderForm").validate({
            rules: {
                "PriceInCurrency": {
                    custom: {
                        depends: function (element) {
                            return isOrderAvailable == GlobalPrint.Utils.Enums.PrintOrderAvailabilities.Unavailable;
                        }
                    }
                }
            },
            messages: {
                "PriceInCurrency": {
                    custom: "Недостаточно средств на счету."
                },
            }
        });
    };

})(GlobalPrint.Order.Confirm);

$(document).ready(function () {
    var isOrderAvailable = parseFloat($("#isOrderAvailable").val());
    GlobalPrint.Order.Confirm.defineValidation(isOrderAvailable);

    $("#createOrderForm").submit(
        function (e) {
            e.preventDefault();
            switch (isOrderAvailable)
            {
                case GlobalPrint.Utils.Enums.PrintOrderAvailabilities.Unavailable:
                    break;
                case GlobalPrint.Utils.Enums.PrintOrderAvailabilities.AvailableWithDebt:
                    GlobalPrint.Utils.CommonUtils.showModalQuestion({
                        question: 'У вас недостаточно средств на счете. Оплату данного заказа произведет проект GlobalPrint.Online.  ' +
                            'Ваш баланс после подтверждения заказа станет отрицательным. Вы можете оплатить текущий заказ позднее в удобное время и в удобном месте. ' +
                            'Для этого необходимо пополнить ваш счет в личном кабинете  Globalprint.online любым удобным способом. Отправить в печать?',
                        answers: [
                            {
                                text: 'Да',
                                handler: function () {
                                    e.target.submit();
                                }
                            },
                            {
                                text: 'Нет',
                                handler: function () {
                                }
                            }
                        ]
                    });
                    break;
                case GlobalPrint.Utils.Enums.PrintOrderAvailabilities.Available:
                    e.target.submit();
                    break;
            }
        }
    );
});