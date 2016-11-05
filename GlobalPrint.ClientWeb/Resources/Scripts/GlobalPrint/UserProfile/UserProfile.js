GlobalPrint.namespace('GlobalPrint.UserProfile.UserProfile');
(function (UserProfile) {
   
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

    UserProfile.fillDataByBic = function () {
        var bankBic = $('#bankBic');
        var bankName = $('#bankName');
        var bankCorrespondentAccount = $('#bankCorrespondentAccount');

        if (!bankBic.val()) {
            return;
        }

        var bankInfoCallback = function (json) {
            if (json) {
                bankName.val(json.FullName);
                bankCorrespondentAccount.val(json.CorrespondentAccount);
            } else {
                bankName.val(null);
                bankCorrespondentAccount.val(null);
            }
        };

        getDataByBic(bankBic.val(), bankInfoCallback)
    };
    
})(GlobalPrint.UserProfile.UserProfile);

$(document).ready(function () {
    $("#autoFillBankData").click(function (e) {
        e.preventDefault();
        GlobalPrint.UserProfile.UserProfile.fillDataByBic();
    });
});

