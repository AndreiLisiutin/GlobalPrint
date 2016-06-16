onCheckLogin = function () {
    var login = $(document)[0].getElementById('inputEmail').value;
    var password = $(document)[0].getElementById('inputPassword').value;
    if (!login || !password) {
        return;
    }

    $.ajax({
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        data: {
            login: login,
            password: password
        },
        url: '/Login/CheckLogin'
    }).done(function (json) {
        if (!json) {
            console.log('Error: ajax response is empty.');
            return;
        }
        if (!(json instanceof Array) || json.length == 0) {
            alert('Failure');
            return;
        } else {
            alert('Success');
            return;
        }
    }).fail(function () {
        console.log('Error: ajax call failed.');
    });
};