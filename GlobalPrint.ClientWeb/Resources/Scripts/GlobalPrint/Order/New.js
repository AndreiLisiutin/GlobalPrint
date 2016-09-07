GlobalPrint.namespace('GlobalPrint.Order.New');

(function (OrderNew) {
    var printerServices = [];

    OrderNew.loadPrinterServices = function (printerID, callback) {
        $.ajax({
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
            data: { printerID: printerID },
            url: '/Printer/GetPrinterServices'
        }).done(function (json) {
            if (!json) {
                console.log('Error: ajax response is empty.');
                return;
            }
            printerServices = json;

            if (callback) {
                callback();
            }
        }).fail(function () {
            console.log('Error: ajax call failed.');
        });
    };

    OrderNew.getPrintTypes = function () {
        var printTypes = [];
        $.each(printerServices, function (index, item) {
            var type = item.PrintService.PrintType;

            var alreadyExists = _.find(printTypes, function (existing) {
                return existing.ID == type.ID;
            });
            if (!alreadyExists) {
                printTypes.push(type);
            }
        });
        return printTypes;
    };

    OrderNew.getTwoSidedTypes = function (printTypeID, printSizeID) {
        var bothTypes = [];
        $.each(printerServices, function (index, item) {
            var type = item.PrintService.PrintService.IsTwoSided;
            
            var alreadyExists = _.find(bothTypes, function (existing) {
                return type == existing;
            });
            if (!alreadyExists && item.PrintService.PrintType.ID == printTypeID && item.PrintService.PrintSize.ID == printSizeID) {
                bothTypes.push(type);
            }
        });
        return bothTypes;
    };

    OrderNew.getColoredTypes = function (printTypeID, printSizeID) {
        var bothTypes = [];
        $.each(printerServices, function (index, item) {
            var type = item.PrintService.PrintService.IsColored;
            
            var alreadyExists = _.find(bothTypes, function (existing) {
                return type == existing;
            });
            if (!alreadyExists && item.PrintService.PrintType.ID == printTypeID && item.PrintService.PrintSize.ID == printSizeID) {
                bothTypes.push(type);
            }
        });
        return bothTypes;
    };

    OrderNew.getPrintSizes = function (printTypeID) {
        var printSizes = [];
        $.each(printerServices, function (index, item) {
            var size = item.PrintService.PrintSize;

            var alreadyExists = _.find(printSizes, function (existing) {
                return existing.ID == size.ID;
            });
            if (!alreadyExists && item.PrintService.PrintType.ID == printTypeID) {
                printSizes.push(size);
            }
        });
        return printSizes;
    };
})(GlobalPrint.Order.New);

$(document).ready(function () {
    var reloadTypes = function () {
        var printTypes = GlobalPrint.Order.New.getPrintTypes();
        $('#printType')
            .find('option')
            .remove()
            .end();
        $.each(printTypes, function (index, item) {
            $('#printType')
                .append($("<option></option>")
                    .attr("value", item.ID)
                    .text(item.Name));
        });
        $('#printType').change();
    };

    var reloadSizes = function () {
        var printTypeID = $('#printType').val();
        $('#printSize')
            .find('option')
            .remove()
            .end();
        if (!printTypeID) {
            return;
        }
        var printSizes = GlobalPrint.Order.New.getPrintSizes(printTypeID);
        $('#printSize').val(null);
        $.each(printSizes, function (index, item) {
            $('#printSize')
                .append($("<option></option>")
                    .attr("value", item.ID)
                    .text(item.Name));
        });
        $('#printSize').change();
    };

    var reloadColored = function () {
        var printTypeID = $('#printType').val();
        var printSizeID = $('#printSize').val();
        $('#IsColored').prop('checked', false);
        if (!printTypeID || !printSizeID) {
            $('#IsColored').attr('disabled', 'disabled');
            return;
        }

        var coloredTypes = GlobalPrint.Order.New.getColoredTypes(printTypeID, printSizeID);
        if (coloredTypes.length == 1) {
            $('#IsColored').prop('checked', coloredTypes[0]); 
            $('#IsColored').attr('disabled', 'disabled');
        } else {
            $('#IsColored').attr('disabled', null);
        }
    };

    var reloadTwoSided = function () {
        var printTypeID = $('#printType').val();
        var printSizeID = $('#printSize').val();
        $('#IsTwoSided').prop('checked', false);
        if (!printTypeID || !printSizeID) {
            $('#IsTwoSided').attr('disabled', 'disabled');
            return;
        }

        var twoSidedTypes = GlobalPrint.Order.New.getTwoSidedTypes(printTypeID, printSizeID);
        if (twoSidedTypes.length == 1) {
            $('#IsTwoSided').prop('checked', twoSidedTypes[0]);
            $('#IsTwoSided').attr('disabled', 'disabled');
        } else {
            $('#IsTwoSided').attr('disabled', null);
        }
    };

    var printerID = $('#PrinterID').val();
    GlobalPrint.Order.New.loadPrinterServices(printerID, function () {
        reloadTypes();
    });

    $('#printType').change(function () {
        reloadSizes();
    });
    $('#printSize').change(function () {
        reloadColored();
        reloadTwoSided();
    });
});