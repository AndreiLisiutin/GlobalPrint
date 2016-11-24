GlobalPrint.namespace('GlobalPrint.Printer.MyPrinters');
(function (MyPrinters) {


})(GlobalPrint.Printer.MyPrinters);



$(document).ready(function () {
    $(".deletePrinter").submit(function (event) {
        event.preventDefault();
        GlobalPrint.Utils.CommonUtils.showModalQuestion({
            question: 'Удалить выбранный принтер?',
            answers: [
                {
                    text: 'Да',
                    handler: function () {
                        event.target.submit();
                    }
                },
                {
                    text: 'Нет',
                    handler: function () {
                    }
                }
            ]
        });
    });
});