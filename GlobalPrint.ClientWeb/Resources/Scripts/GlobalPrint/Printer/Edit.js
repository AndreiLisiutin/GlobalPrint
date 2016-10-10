GlobalPrint.namespace('GlobalPrint.Printer.Edit');
(function (Edit) {

    Edit.defineValidation = function () {
        $("#printerEditForm").validate({
            rules: {
                "Printer.Name": {
                    required: true
                },
                "Printer.Location": {
                    required: true
                },
                "Printer.Latitude": {
                    required: true,
                    decimal: true
                },
                "Printer.Longtitude": {
                    required: true,
                    decimal: true
                },
                "Printer.Phone": {
                    phone: true
                },
                "Printer.Email": {
                    email: true
                },
            }
        });
        $('.printer-schedule-time').each(function () {
            $(this).rules('add', {
                time: true,
                required: {
                    depends: function (element) {
                        return $(element)
                            .closest('.printer-schedule-row')
                            .find('.is-printer-schedule-open')
                            .prop('checked');
                    }
                }
            });
        });
        $('.printer-schedule-time-close').each(function () {
            $(this).rules('add', {
                custom: {
                    depends: function (element) {
                        var scheduleIsActive = $(element)
                            .closest('.printer-schedule-row')
                            .find('.is-printer-schedule-open')
                            .prop('checked');

                        if (!scheduleIsActive) {
                            return false;
                        }

                        var openTime = $(element)
                            .closest('.printer-schedule-row')
                            .find('.printer-schedule-time-open')
                            .val();
                        var closeTime = $(element)
                            .closest('.printer-schedule-row')
                            .find('.printer-schedule-time-close')
                            .val();

                        if (!/\d\d:\d\d/.test(openTime) || !/\d\d:\d\d/.test(closeTime)) {
                            return false;
                        }

                        var openHour = parseInt(openTime.substring(0, 2));
                        var closeHour = parseInt(closeTime.substring(0, 2));
                        var openMinute = parseInt(openTime.substring(3, 5));
                        var closeMinute = parseInt(closeTime.substring(3, 5));

                        var isOk = openHour < closeHour || (openHour == closeHour && openMinute < closeMinute);
                        return !isOk;
                    }
                },
                messages: {
                    custom: "Дата завершения работы не может быть раньше даты начала рабты."
                }
            });
        });
        $('.printer-service-price').each(function () {
            $(this).rules('add', {
                required: {
                    depends: function (element) {
                        return $(element)
                            .closest('.printer-schedule-row')
                            .find('.is-printer-schedule-open')
                            .prop('checked');
                    }
                }
            });
        });
    };

    Edit.disablePriceForNotSupported = function () {
        //disable price controls for print services which are not supported (by checkbox "Is supported") 
        $('.printer-service-row').each(function (index, item) {
            var isServiceSupported = $(item)
                .find('.is-printer-service-supported')
                .prop('checked');

            var prices = $(item)
                .find('.printer-service-price');

            prices.attr('disabled', !isServiceSupported);
            if (!isServiceSupported) {
                prices.val('');
            }

        });
    };

    Edit.disableTimeForClosed = function () {
        //disable time edition for those schedule items where 
        //"Open this day" checkbox is unchecked
        $('#printerSchedules .printer-schedule-row').each(function (index, item) {
            var isPrinterOpen = $(item)
                .find('.is-printer-schedule-open')
                .prop('checked');

            var clockpickers = $(item)
                .find('.printer-schedule-time');

            clockpickers.attr('disabled', !isPrinterOpen);
            if (!isPrinterOpen) {
                clockpickers.val('');
            }

            var clockpickerBtn = $(item)
                .find('.clockpicker .input-group-addon');

            if (isPrinterOpen) {
                clockpickerBtn.removeClass('hidden');
            } else {
                clockpickerBtn.addClass('hidden');
            }
        });
    };

    Edit.geolocateLatLong = function () {
        //call Google geolocation service to find out Latitude, Longtitude of a printer by its address string 
        //and format the address
        var location = $('#location').val();
        var protocol = window.location.protocol;

        GlobalPrint.Utils.Services.googleGeolocation(location, function (result) {
            $('#latitude').val(result.latitude);
            $('#longtitude').val(result.longtitude);
            $('#location').val(result.address);
        });
    };

})(GlobalPrint.Printer.Edit);



$(document).ready(function () {
    $('.clockpicker').clockpicker();
    GlobalPrint.Printer.Edit.defineValidation();

    $(".is-printer-service-supported").change(function (event) {
        GlobalPrint.Printer.Edit.disablePriceForNotSupported();
    });
    $(".is-printer-schedule-open").change(function (event) {
        GlobalPrint.Printer.Edit.disableTimeForClosed();
    });
    GlobalPrint.Printer.Edit.disablePriceForNotSupported();
    GlobalPrint.Printer.Edit.disableTimeForClosed();

    $('#geolocation').click(function () {
        GlobalPrint.Printer.Edit.geolocateLatLong();
    });
});