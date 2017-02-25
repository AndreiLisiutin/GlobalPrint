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

	/**
	 * Обновить токен браузера для текущего пользователя.
	 * @param {string} currentToken Токен браузера.
	 */
	var sendTokenToServer = function (currentToken) {
		$.ajax({
			type: 'GET',
			contentType: 'application/json',
			dataType: 'json',
			data: { deviceID: currentToken + '' },
			url: '/UserProfile/AddDeviceToGroup'
		}).done(function (json) {
			console.log('Токен браузера обновлен.');

		}).fail(function () {
			console.log('Error: ajax call failed.');
		});
	};

	/**
	 * Получить и при необходимости обновить токен браузера для пользователя.
	 */
	var refreshToken = function () {
		messaging.getToken()
            .then(function (token) {
            	console.log('Получен токен браузера: ' + token);
            	sendTokenToServer(token);
            })
            .catch(function (err) {
            	console.log('Ошибка обновления токена браузера для пользователя: ', err);
            });
	};
	
	/**
	 * Запросить разрешение на уведомления.
	 */
	var requestPermission = function () {
		messaging.requestPermission()
			.then(function () {
				console.log('Получено разрешение на уведомления.');
				refreshToken();
			})
			.catch(function (err) {
				console.log('Разрешение на уведомление не получено.', err);
			});
	}

	if ('serviceWorker' in navigator) {
		navigator.serviceWorker.register('/firebase-messaging-sw.js')
		  .then(requestPermission);
	} else {
		console.error('Сервис воркеры не поддерживаются браузером.');
	}

	messaging.onTokenRefresh(function () {
		refreshToken();
	});

	// Handle incoming messages. Called when:
	// - a message is received while the app has focus
	// - the user clicks on an app notification created by a sevice worker "messaging.setBackgroundMessageHandler" handler.
	messaging.onMessage(function (payload) {
		navigator.serviceWorker.ready
			.then(function (serviceWorkerRegistration) {
				serviceWorkerRegistration.active.postMessage(payload);
			});
	});

}(GlobalPrint.Shared.FirebaseNotifications));