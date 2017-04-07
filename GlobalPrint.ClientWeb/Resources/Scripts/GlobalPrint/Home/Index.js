GlobalPrint.namespace('GlobalPrint.Home.Index');
(function (HomeIndex) {
	var PRINTER_ONLINE_ICON_URL = "/Content/img/marker.png";
	var PRINTER_OFFLINE_ICON_URL = "/Content/img/marker.png";
	var PRINTER_DEACTIVE_ICON_URL = "/Content/img/marker.png";
	var PRINTER_ONLINE_Z_INDEX = 10000;
	var PRINTER_OFFLINE_Z_INDEX = 5000;
	var PRINTER_DEACTIVE_Z_INDEX = 1000;

	//select printer's icon by its state
	var _getPrinterIcon = function (printerInfo) {
		if (printerInfo.IsAvailableNow && printerInfo.IsOperatorAlive) {
			return PRINTER_ONLINE_ICON_URL;
		}
		if (printerInfo.Printer.IsDisabled) {
			return PRINTER_DEACTIVE_ICON_URL;
		}
		return PRINTER_OFFLINE_ICON_URL;
	};
	var _getPrinterZIndex = function (printerInfo) {
		if (printerInfo.IsAvailableNow && printerInfo.IsOperatorAlive) {
			return PRINTER_ONLINE_Z_INDEX;
		}
		if (printerInfo.Printer.IsDisabled) {
			return PRINTER_DEACTIVE_Z_INDEX;
		}
		return PRINTER_OFFLINE_Z_INDEX;
	};

	var _map = null;
	var _markersArray = [];
	var _currentPrinterID = null;
	var _lastState = null;


	var _zoomMarker = function (position) {
		_map.setZoom(15);
		_map.panTo(position);
	};

	HomeIndex.init = function () {
		loadMap();
	};

	//markerwith current user's geoposition
	var _meMarker = null;
	//find user's geoposition and print it into map as a marker
	HomeIndex.findMe = function (controlDiv, map) {
		GlobalPrint.Utils.CommonUtils.geolocate(function (position) {
			_meMarker && _meMarker.setMap(null);
			var location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
			_map.panTo(location);
			_map.setZoom(15);
			_meMarker = new google.maps.Marker({
				position: location,
				map: _map,
				animation: google.maps.Animation.DROP,
				label: "Я",
				title: "Ваше текущее положение"
			});
		});
	};

	//load google map and try to open current user's geolocation
	var loadMap = function () {
		GlobalPrint.Utils.CommonUtils.geolocate(
            function (position) {
            	var location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            	_createMap(location);
            },
            function (error) {
            	console.log(error.message);
            	var location = new google.maps.LatLng(55.828345, 49.125938);
            	_createMap(location);
            }
        );
	};

	var _createMap = function (location) {
		var isUserAuthenticated = GlobalPrint.Utils.CommonUtils.isUserAuthenticated();
		var mapOptions = {
			zoom: 4,
			center: location,
			mapTypeId: google.maps.MapTypeId.ROADMAP
		};
		_map = new google.maps.Map($("#googlemaps")[0], mapOptions);
		google.maps.event.addListenerOnce(_map, 'idle', function () {
			loadPrinters();
		});
		//google maps api bug-hack
		google.maps.event.addListener(_map, 'idle', function () {
			_map.panBy(0, 0);
		});
		google.maps.event.addListener(_map, 'click', HomeIndex.closePrinterInfo);
		$("#googlemaps").on("heightChange", function () {
			//for resize map with browser resize
			google.maps.event.trigger(_map, "resize");
		});
		if (isUserAuthenticated) {
			_zoomMarker(location);
		}
	};

	HomeIndex.closePrinterInfo = function () {
		if (_currentPrinterID) {
			_currentPrinterID = null;
			$("#homeInfoSidebar").removeClass("active");
			_map.setZoom(_lastState.zoom);
			_map.panTo(_lastState.center);
			_lastState = null;
		}
	};

	function deleteAllMarkers() {
		if (_markersArray) {
			$.each(_markersArray, function (index, item) {
				item.setMap(null);
			});
			_markersArray.length = 0;
		}
	};

	function deleteMarkerByPrinterID(printerID) {
		var marker = findMarkerByPrinterID(printerID);
		if (marker) {
			marker.setMap(null);
		}
		_markersArray = _.filter(_markersArray, function (item) {
			return item.printerID != printerID;
		});
	};

	function findMarkerByPrinterID(printerID) {
		return _markersArray && _.find(_markersArray, function (marker) {
			return marker.printerID == printerID;
		});
	};

	var _onlyMyPrinters = false;
	HomeIndex.loadOnlyMyPrinters = function () {
		_onlyMyPrinters = !_onlyMyPrinters;
		if (_onlyMyPrinters) {
			$('#homeInfoControlMyPrinters').addClass('active');
		} else {
			$('#homeInfoControlMyPrinters').removeClass('active');
		}
		loadPrinters(_onlyMyPrinters);
	};

	HomeIndex.loadClosestPrinter = function () {
		GlobalPrint.Utils.CommonUtils.geolocate(
            function (position) {
            	$.ajax({
            		type: 'GET',
            		contentType: 'application/json',
            		dataType: 'json',
            		data: { latitude: position.coords.latitude, longtitude: position.coords.longitude },
            		url: '/Home/GetClosestPrinter'
            	}).done(function (json) {
            		if (!json) {
            			GlobalPrint.Shared.PushNotifications.displayMessage("Не найдено ни одного активного принтера.");
            			return;
            		}
            		deleteMarkerByPrinterID(json.Printer.ID);
            		var marker = _addMarker(json, true);
            		_zoomMarker(marker.getPosition());
            	}).fail(function () {
            		console.log('Error: ajax call failed.');
            	});
            },
            function (error) {
            	console.log(error.message);
            }
        );
	};

	var loadPrinters = function (fitBounds) {
		fitBounds = fitBounds || false;
		//get google map geographical boundaries
		var lat0 = _map.getBounds().getNorthEast().lat();
		var lat1 = _map.getBounds().getSouthWest().lat();

		var lng0 = _map.getBounds().getNorthEast().lng();
		var lng1 = _map.getBounds().getSouthWest().lng();

		var jsonData = {
			minLatitude: lat1 >= lat0 ? lat0 : lat1,
			maxLatitude: lat1 >= lat0 ? lat1 : lat0,
			minLongtitude: lng1 >= lng0 ? lng0 : lng1,
			maxLongtitude: lng1 >= lng0 ? lng1 : lng0
		};
		jsonData.userID = _onlyMyPrinters && GlobalPrint.Utils.CommonUtils.getUserID();
		$.ajax({
			type: 'GET',
			contentType: 'application/json',
			dataType: 'json',
			data: jsonData,
			url: '/Home/GetPrinters'
		}).done(function (json) {
			if (!json) {
				console.log('Error: ajax response is empty.');
				return;
			}

			deleteAllMarkers();
			var bounds = new google.maps.LatLngBounds();
			$.each(json, function (index, printerInfo) {
				_addMarker(printerInfo);
				bounds.extend(new google.maps.LatLng(printerInfo.Printer.Latitude, printerInfo.Printer.Longtitude));
			});
			if (fitBounds) {
				_map.fitBounds(bounds);
			}

		}).fail(function () {
			console.log('Error: ajax call failed.');
		});
	};

	var _addMarker = function (printerInfo, animation) {
		//adding new marker to the map.
		//printerInfo is a model for C# PrinterFullInfoModel class.
		var marker = new google.maps.Marker({
			position: new google.maps.LatLng(printerInfo.Printer.Latitude, printerInfo.Printer.Longtitude),
			map: _map,
			icon: _getPrinterIcon(printerInfo),
			optimized: false,
			zIndex: _getPrinterZIndex(printerInfo),
			title: printerInfo.Printer.Name,
			animation: animation ? google.maps.Animation.DROP : null,
			printerID: printerInfo.Printer.ID,
			printerInfo: printerInfo,
			class: 'printer-marker'
		});

		google.maps.event.addListener(marker, 'click', function () {
			$("#printerInfoPrinterID").val(printerInfo.Printer.ID);
			$("#printerInfoIsAvailable").prop('checked', printerInfo.IsAvailableNow);
			if (!printerInfo.IsOperatorAlive) {
				$("#printerInfoIsAvailable").addClass('operator-sleeping');
			} else {
				$("#printerInfoIsAvailable").removeClass('operator-sleeping');
			}
			if (!printerInfo.IsAvailableNow || !printerInfo.IsOperatorAlive) {
				$("#printerInfoPrint").addClass('hidden');
			} else {
				$("#printerInfoPrint").removeClass('hidden');
			}
			function toFixedInt2(n) {
				return n > 9 ? "" + n : "0" + n;
			}
			var name = printerInfo.Printer.Name;
			var location = printerInfo.Printer.Location;
			$("#printerInfoLocation").html(name + '<br>' + location);

			var averallServices = '';
			$.each(marker.printerInfo.PrinterServices, function (index, item) {
				var service = item.PrintService.FullName + ': ' + item.PrinterService.PricePerPage + ' руб.';
				averallServices += (averallServices ? '<br>' : '') + service;
			});
			$("#printerInfoPrices").html(averallServices);

			if (!_currentPrinterID) {
				_lastState = {
					zoom: _map.getZoom(),
					center: _map.getCenter()
				};

				$("#homeInfoSidebar").addClass("active");
				setTimeout(function () {
					_zoomMarker(marker.getPosition());
				}, 600);
			} else {
				_zoomMarker(marker.getPosition());
			}
			_currentPrinterID = marker.printerInfo.Printer.ID;
		});
		_markersArray.push(marker);
		return marker;
	};

	HomeIndex.setMapFullScreen = function () {
		//сделать карту в полный размер окна
		var winHeight = $(window).height();
		var navheight = $(".top-visible-item").toArray()
			.map(function (t) { return $(t).height(); })
			.reduce(function (a, b) { return a + b; }, 0);
		var fullHeight = winHeight - navheight;
		$('#googlemaps').height(fullHeight);
		$('#googlemaps').trigger("heightChange");
		$('#homeInfoSidebar').height(fullHeight - 30);
	};

})(GlobalPrint.Home.Index);

$(document).ready(function () {
	GlobalPrint.Home.Index.init();
	$("#printerInfoClose").click(function () {
		//Close button in print details sidebar
		GlobalPrint.Home.Index.closePrinterInfo();
	});

	$("#printerInfoPrint").click(function (event) {
		//Print button in print details sidebar
		var isUserAuthenticated = GlobalPrint.Utils.CommonUtils.isUserAuthenticated();
		var printerID = $("#printerInfoPrinterID").val();

		if (!printerID) {
			console.log('printerID is not defined for printing.');
			return;
		}

		var printUrl = isUserAuthenticated
            ? ("/Order/New/?printerID=" + printerID)
            : ("/Account/LoginAndPrint/?printerID=" + printerID);

		window.location.href = printUrl;
	});

	$(window).resize(function () {
		GlobalPrint.Home.Index.setMapFullScreen();
	});

	var isUserAuthenticated = GlobalPrint.Utils.CommonUtils.isUserAuthenticated();
	if (!isUserAuthenticated) {
		$("#homeInfoControlMyPrinters").addClass('hidden');
	}
	$("#homeIndoControlFindMe").click(function (event) {
		GlobalPrint.Home.Index.findMe();
	});
	$("#homeInfoControlMyPrinters").click(function (event) {
		GlobalPrint.Home.Index.loadOnlyMyPrinters();
	});
	$("#homeInfoControlClosestPrinter").click(function (event) {
		GlobalPrint.Home.Index.loadClosestPrinter();
	});

	GlobalPrint.Home.Index.setMapFullScreen();

	$(document).click(function (event) {
		//закрыть информацию о принтере, если ткнули на карту
		var mark = $(".printer-marker");
		var opened = $("#homeInfoSidebar").hasClass("active");
		if (opened === true && !$(event.target).hasClass("infoBox") && !mark.is(event.target)) {
			Home.Index.closePrinterInfo();
		}
	});
});