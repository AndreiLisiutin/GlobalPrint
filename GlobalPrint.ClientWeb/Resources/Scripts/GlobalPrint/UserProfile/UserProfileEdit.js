GlobalPrint.namespace('GlobalPrint.UserProfile.UserProfileEdit');
(function (UserProfileEdit) {

    UserProfileEdit.defineValidation = function () {
        $("#userProfileEditForm").validate({
            rules: {
                "UserName": {
                    required: true
                },
                "Email": {
                    required: true,
                    email: true
                }
            }
        });
    };
   
    var getDataByBic = function (bic, callback) {
        $.ajax({
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
            data: { bic: bic },
            url: '/UserProfile/GetBankInfo'
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

    UserProfileEdit.fillDataByBic = function () {
        var bankBic = $('#bankBic');
        var bankName = $('#bankName');
        var bankCorrespondentAccount = $('#bankCorrespondentAccount');

        if (!bankBic.val()) {
            return;
        }

        var bankInfoCallback = function (json) {
            if (json) {
                bankName.val(json.ShortName);
                bankCorrespondentAccount.val(json.CorrespondentAccount);
            } else {
                bankName.val(null);
                bankCorrespondentAccount.val(null);
            }
        };

        getDataByBic(bankBic.val(), bankInfoCallback)
    };

    UserProfileEdit.geolocateLatLong = function (cmp) {
        // Call Google geolocation service to find out Latitude, Longtitude 
        // of address string and format the address
        var location = $(cmp).val();

        GlobalPrint.Utils.Services.googleGeolocation(location, function (result) {
            $(cmp).val(result.address);
        });
    };
    
})(GlobalPrint.UserProfile.UserProfileEdit);

$(document).ready(function () {
    GlobalPrint.UserProfile.UserProfileEdit.defineValidation();

    $("#autoFillBankData").click(function (e) {
        e.preventDefault();
        GlobalPrint.UserProfile.UserProfileEdit.fillDataByBic();
    });
    $('#legalGeolocation').click(function () {
        var legalAddress = $('#legalAddress');
        GlobalPrint.UserProfile.UserProfileEdit.geolocateLatLong(legalAddress);
    });
    $('#postGeolocation').click(function () {
        var postAddress = $('#postAddress');
        GlobalPrint.UserProfile.UserProfileEdit.geolocateLatLong(postAddress);
    });
});

