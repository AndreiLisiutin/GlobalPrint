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

// Retrieve an instance of Firebase Messaging so that it can handle background messages.
const messaging = firebase.messaging();

var showNotification = function (payload) {	
	var notificationOptions = {
		body: payload.notification.body,
		icon: payload.notification.icon,
		requireInteraction: true,
		data: {
			url: payload.data.url
		}
	};
	return self.registration.showNotification(payload.notification.title,
		notificationOptions);
}

// If you would like to customize notifications that are received in the
// background (Web app is closed or not in browser focus) then you should
// implement this optional method.
messaging.setBackgroundMessageHandler(function (payload) {
	console.log('Received background message ', payload);
		
	self.registration.active.postMessage(payload);
});

self.addEventListener('message', function (event) {
	event.waitUntil(showNotification(event.data));
});

self.addEventListener('notificationclick', function (event) {
	event.notification.close();

	if (event.notification.data && event.notification.data.url ||
		event.notification.data && event.notification.data.click_url ||
		event.notification.click_url ||
		event.notification.url) {

		var url = event.notification.url || event.notification.click_url;
		if (!url && event.notification.data) {
			url = event.notification.data.url || event.notification.data.click_url;
		}

		if (url) {
			// This looks to see if the current is already open and
			// focuses if it is
			event.waitUntil(clients.matchAll({
				type: "window"
			}).then(function (clientList) {
				for (var i = 0; i < clientList.length; i++) {
					var client = clientList[i];
					if (client.url == url && 'focus' in client)
						return client.focus();
				}
				if (clients.openWindow) {
					return clients.openWindow(url);
				}
			}));
		}		
	}
});