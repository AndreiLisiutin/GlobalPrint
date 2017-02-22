GlobalPrint.namespace('GlobalPrint.Shared.FirebaseNotifications');
(function (FirebaseNotifications) {

	// Initialize Firebase
	var config = {
		apiKey: "AIzaSyBN9GmnvUmZh9I5NR9Tl7j-fTgnzOHE33A",
		authDomain: "globalprint-9ade2.firebaseapp.com",
		databaseURL: "https://globalprint-9ade2.firebaseio.com",
		storageBucket: "globalprint-9ade2.appspot.com",
		messagingSenderId: "130428096836"
	};
	firebase.initializeApp(config);
	const messaging = firebase.messaging();

	var sendTokenToServer = function (currentToken) {
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
            	sendTokenToServer(token);
            })
            .catch(function (err) {
            	console.log('Unable to retrieve refreshed token ', err);
            });
	};
	
	var requestPermission = function () {
		messaging.requestPermission()
			.then(function () {
				console.log('Notification permission granted.');
				refreshToken();
			})
			.catch(function (err) {
				console.log('Unable to get permission to notify.', err);
			});
	}

	if ('serviceWorker' in navigator) {
		navigator.serviceWorker.register('./firebase-messaging-sw.js')
		  .then(requestPermission);
	} else {
		console.error('Service workers aren\'t supported in this browser.');
	}

	// fired if Instance ID token is updated.
	messaging.onTokenRefresh(function () {
		refreshToken();
	});

	// Handle incoming messages. Called when:
	// - a message is received while the app has focus
	// - the user clicks on an app notification created by a sevice worker "messaging.setBackgroundMessageHandler" handler.
	messaging.onMessage(function (payload) {
		console.log("Message received. ", payload);
		if (!payload || !payload.notification || !payload.data || !payload.data.destinationUserID) {
			return;
		}

		navigator.serviceWorker.ready
			.then(function (serviceWorkerRegistration) {
				serviceWorkerRegistration.active.postMessage(payload);
			});
	});

}(GlobalPrint.Shared.FirebaseNotifications));