onRegisterUser = function () {
    var name = $(document)[0].getElementById('inputName').value;
    var login = $(document)[0].getElementById('inputEmail').value;
    var password = $(document)[0].getElementById('inputPassword').value;
    var confirmPassword = $(document)[0].getElementById('inputConfirmPassword').value;
    if (!name || !login || !password || !confirmPassword) {
        return;
    }
    if (password != confirmPassword) {
        alert('Different passwords');
    }

    $.ajax({
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        data: {
            login: login,
            password: password
        },
        url: '/Login/RegisterUser'
    }).done(function (json) {
        if (!json) {
            console.log('Error: ajax response is empty.');
            return;
        }
        if (!(json instanceof Array) || json.length == 0) {
            alert('User with login ' + login + ' alerady exists');
            return;
        } else {
            alert('Success');
            return;
        }
    }).fail(function () {
        console.log('Error: ajax call failed.');
    });
};