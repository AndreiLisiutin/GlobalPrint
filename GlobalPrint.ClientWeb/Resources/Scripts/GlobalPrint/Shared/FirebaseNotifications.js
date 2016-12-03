﻿GlobalPrint.namespace('GlobalPrint.Shared.PushNotifications');
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

    var sendTokenToServer = function (currentToken) {
        // Send the current token to server.
        $.ajax({
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
            data: { deviceID: currentToken + '' },
            url: '/UserProfile/AddDeviceToGroup'
        }).done(function (json) {
            console.log('Token updated on server...');

        }).fail(function () {
            console.log('Error: ajax call failed.');
        });
    };

    var refreshToken = function () {
        messaging.getToken()
            .then(function (token) {
                console.log('Token received: ' + token);
                console.log('Token refreshing...');
                // Send Instance ID token to app server.
                sendTokenToServer(token);
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
            refreshToken();

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