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
                });
            } else {
                GlobalPrint.Utils.CommonUtils.showModalQuestion({
                    question: 'Сформировать реестр перечислений по ' + data.CashRequestsTotalCount + ' заявкам на вывод денег на предположительную общую сумму ' + parseFloat(data.CashRequestsTotalAmountOfMoney).toFixed(2) + ' руб.?',
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
            }
        });
    });
});

