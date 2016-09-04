$(document).ready(function () {
    $('.focus :input:first').focus();
    $(".focus :input:first").val($(".focus :input:first").val());

    var applyFilter = function () {
        $('#userPrintOrderListForm').submit();
    };

    // create debounced function
    var dprocess = _.debounce(applyFilter, 1000);

    // bind event handler
    $('#printOrderIdFilter').on('propertychange change keyup paste input', function () {
        dprocess();
    });
});