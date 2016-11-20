GlobalPrint.namespace('GlobalPrint.Shared.PushNotifications');
(function (PushNotifications) {

    // Initialize Firebase
    var config = {
        apiKey: "AIzaSyBN9GmnvUmZh9I5NR9Tl7j-fTgnzOHE33A",
        authDomain: "globalprint-9ade2.firebaseapp.com",
        databaseURL: "https://globalprint-9ade2.firebaseio.com",
        storageBucket: "globalprint-9ade2.appspot.com",
        messagingSenderId: "130428096836"
    };
    firebase.initializeApp(config);
    // Retrieve Firebase Messaging object.
    const messaging = firebase.messaging();

    var setTokenSentToServer = function (sent) {
        if (sent) {
            window.localStorage.setItem('sentToServer', 1);
        } else {
            window.localStorage.setItem('sentToServer', 0);
        }
    };

    var sendTokenToServer = function (currentToken) {
        if (!isTokenSentToServer()) {
            // Send the current token to server.
            $.ajax({
                type: 'GET',
                contentType: 'application/json',
                dataType: 'json',
                data: { deviceID: currentToken + '' },
                url: '/UserProfile/UpdateDeviceID'
            }).done(function (json) {
                setTokenSentToServer(true);
                console.log('Token updated on server...');

            }).fail(function () {
                setTokenSentToServer(false);
                console.log('Error: ajax call failed.');
            });

        } else {
            console.log('Token already sent to server so won\'t send it again ' +
                'unless it changes');
        }
    };

    var isTokenSentToServer = function () {
        if (window.localStorage.getItem('sentToServer') == 1) {
            return true;
        }
        return false;
    };

    var refreshToken = function () {
        messaging.getToken()
            .then(function (refreshedToken) {
                console.log('Token refreshing...');
                // Indicate that the new Instance ID token has not yet been sent to the app server.
                setTokenSentToServer(false);
                // Send Instance ID token to app server.
                sendTokenToServer(refreshedToken);
            })
            .catch(function (err) {
                console.log('Unable to retrieve refreshed token ', err);
            });
    };

    // Callback fired if Instance ID token is updated.
    messaging.onTokenRefresh(function () {
        refreshToken();
    });

    messaging.requestPermission()
        .then(function () {
            console.log('Notification permission granted.');
            // Retrieve an Instance ID token for use with FCM.
            messaging.getToken()
                .then(function (token) {
                    console.log('Token received: ' + token);

                    // Get current token from server.
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json',
                        dataType: 'json',
                        url: '/UserProfile/GetDeviceID'
                    }).done(function (json) {
                        if (!json) {
                            console.log('Error: ajax call response is empty.');
                            return;
                        }

                        var deviceID = json.deviceID;
                        if (!deviceID || deviceID != token) {
                            refreshToken();
                        }

                    }).fail(function () {
                        setTokenSentToServer(false);
                        console.log('Error: ajax call failed.');
                    });

                })
                .catch(function (err) {
                    console.log('Unable to retrieve token ', err);
                });

        })
        .catch(function (err) {
            console.log('Unable to get permission to notify.', err);
        });

    // Handle incoming messages. Called when:
    // - a message is received while the app has focus
    // - the user clicks on an app notification created by a sevice worker
    //   `messaging.setBackgroundMessageHandler` handler.
    messaging.onMessage(function (payload) {
        console.log("Message received. ", payload);
    });

}(GlobalPrint.Shared.PushNotifications));