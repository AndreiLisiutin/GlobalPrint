﻿$(document).ready(function () {
    $('.focus :input:first').focus();
    $(".focus :input:first").val($(".focus :input:first").val());

    var applyFilter = function () {
        $('#userRecievedPrintOrderListForm').submit();
    };

    // create debounced function
    var dprocess = _.debounce(applyFilter, 1000);

    // bind event handler
    $('#recievedPrintOrderIdFilter').on('propertychange change keyup paste input', function () {
        dprocess();
    });

    $(".rejectOrder").submit(function (event) {
        event.preventDefault();
        GlobalPrint.Utils.CommonUtils.showModalQuestion({
            question: 'Отменить выбранный заказ?',
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