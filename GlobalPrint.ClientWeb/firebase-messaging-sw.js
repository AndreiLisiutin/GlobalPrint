// [START initialize_firebase_in_sw]
// Give the service worker access to Firebase Messaging.
// Note that you can only use Firebase Messaging here, other Firebase libraries
// are not available in the service worker.
importScripts('https://www.gstatic.com/firebasejs/3.5.2/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/3.5.2/firebase-messaging.js');

// Initialize the Firebase app in the service worker by passing in the
// messagingSenderId.
firebase.initializeApp({
    'messagingSenderId': "130428096836"
});

// Retrieve an instance of Firebase Messaging so that it can handle background
// messages.
const messaging = firebase.messaging();
// [END initialize_firebase_in_sw]

// If you would like to customize notifications that are received in the
// background (Web app is closed or not in browser focus) then you should
// implement this optional method.
// [START background_handler]
messaging.setBackgroundMessageHandler(function (payload) {
    console.log('Received background message ', payload);

    if (!payload || !payload.notification || !payload.data || !payload.data.destinationUserID) {
        return;
    }

    // Get current user ID from server.
    $.ajax({
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        url: '/UserProfile/GetUserID'
    }).done(function (json) {
        if (!json) {
            console.log('Error: ajax call response is empty.');
            return;
        }

        var userID = json.userID;
        if (!userID) {
            return;
        }

        // if current logged in user is reciever of message, show notification
        if (userID == payload.data.destinationUserID) {
            var notificationOptions = {
                body: payload.notification.body,
                icon: payload.notification.icon
            };
            return self.registration.showNotification(payload.notification.title,
                notificationOptions);
        }

    }).fail(function () {
        console.log('Error: ajax call failed.');
    });

});
// [END background_handler]