GlobalPrint.namespace('GlobalPrint.Utils.Services');
(function (Services) {

    Services.googleGeolocation = function (location, successCallback, failureCallback) {
        //call Google geolocation service to find out Latitude, Longtitude of a printer by its address string 
        //and format the address
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
                    successCallback({
                        latitude: null,
                        longtitude: null,
                        address: ''
                    });
                }
                var latitude = json.results[0].geometry.location.lat;
                var longtitude = json.results[0].geometry.location.lng;
                var address = json.results[0].formatted_address;

                if (successCallback) {
                    successCallback({
                        latitude: latitude.toFixed(4),
                        longtitude: longtitude.toFixed(4),
                        address: address
                    });
                }
            } catch (err) {
                if (failureCallback) {
                    failureCallback(err);
                }
            }
        }).fail(function () {
            failureCallback({ message: 'Error: ajax call failed.' });
        });
    };



})(GlobalPrint.Utils.Services);