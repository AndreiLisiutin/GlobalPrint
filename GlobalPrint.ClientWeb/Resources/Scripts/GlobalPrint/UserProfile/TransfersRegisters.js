GlobalPrint.namespace('GlobalPrint.UserProfile.TransfersRegisters');
(function (TransfersRegisters) {

    TransfersRegisters.GetNextTransferRegisterPrediction = function (callback) {
        $.ajax({
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
            data: {},
            url: '/UserProfile/GetNextTransferRegisterPrediction'
        }).done(function (json) {
            if (!json) {
                console.log('Error: ajax response is empty.');
                return;
            }
            printerServices = json;

            if (callback) {
                callback(json);
            }
        }).fail(function () {
            console.log('Error: ajax call failed.');
        });
    };

})(GlobalPrint.UserProfile.TransfersRegisters);

$(document).ready(function () {
    $("#createNewRegister").submit(function (e) {
        e.preventDefault();
        GlobalPrint.UserProfile.TransfersRegisters.GetNextTransferRegisterPrediction(function (data) {
            if (!data.CashRequestsTotalCount) {
                GlobalPrint.Utils.CommonUtils.showModalQuestion({
                    question: 'Нет заявок на вывод денег для формирования реестра.',
                    answers: [
                        {
                            text: 'ОК',
                            handler: function () {
                            }
                        }
                    ]
                }, null, 'modal-fullscreen');
            } else {
                GlobalPrint.Utils.CommonUtils.showModalQuestion({
                    question: 'Сформировать реестр перечислений по ' + data.CashRequestsTotalCount + ' заявкам на вывод ' + parseFloat(data.CashRequestsTotalAmountOfMoney).toFixed(2) + 
                        ' рублей? Запрошенная сумма по каждому пользователю будет включена в реестр только при наличии этой суммы на его текущем счету системы.',
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
                }, null, 'modal-fullscreen');
            }
        });
    });
});

