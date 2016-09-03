$(document).ready(function () {
    $('.clockpicker').clockpicker();

    var disablePriceForNotSupported = function () {
        //disable price controls for print services which are not supported (by checkbox "Is supported") 
        $('#printerServices .printer-service').each(function (index, item) {
            var isServiceSupported = $(item)
                .find('.is-printer-service-supported')
                .is(':checked');

            var prices = $(item)
                .find('.printer-service-price');

            prices.attr('disabled', !isServiceSupported);
            if (!isServiceSupported) {
                prices.val('');
            }

        });
    };
    var disableTimeForClosed = function () {
        //disable time edition for those schedule items where 
        //"Open this day" checkbox is unchecked
        $('#printerSchedules .printer-schedule').each(function (index, item) {
            var isPrinterOpen = $(item)
                .find('.is-printer-schedule-open')
                .is(':checked');

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

    disablePriceForNotSupported();
    disableTimeForClosed();

    $("#printerServices .is-printer-service-supported").change(function (event) {
        disablePriceForNotSupported();
    });
    $("#printerSchedules .is-printer-schedule-open").change(function (event) {
        disableTimeForClosed();
    });

    $('#geolocation').click(function () {
        //call Google geolocation service to find out Latitude, Longtitude of a printer by its address string 
        //and format the address
        var location = $('#location').val();
        var protocol = window.location.protocol;

        $.ajax({
            url: protocol + '//maps.google.com/maps/api/geocode/json',
            type: 'GET',
            data: {
                address: location,
                sensor: false
            },
            async: false
        }).done(function (json) {
            if (!json) {
                console.log('Error: ajax response is empty.');
                return;
            }

            try {
                if (!json.results || !json.results[0]) {
                    return;
                }
                var position = {};
                position.lat = json.results[0].geometry.location.lat;
                position.lng = json.results[0].geometry.location.lng;

                $('#latitude').val(position.lat);
                $('#longtitude').val(position.lng);
                $('#location').val(json.results[0].formatted_address);

            } catch (err) {
                position = null;
            }
        }).fail(function () {
            console.log('Error: ajax call failed.');
        });

    });
});