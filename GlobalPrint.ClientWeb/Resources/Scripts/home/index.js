home = home || {};
home.index = home.index || (function () {

    var _map = null;
    var _markersArray = [];
    var _markersCurrentArray = [];
    var _currentPrinterID = null;
    var _lastState = null;

    var init = function () {
        loadMap();
    };


    function CenterControl(controlDiv, map) {

        // Set CSS for the control border.
        var controlUI = document.createElement('div');
        controlUI.style.backgroundColor = '#fff';
        controlUI.style.border = '2px solid #fff';
        controlUI.style.borderRadius = '3px';
        controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
        controlUI.style.cursor = 'pointer';
        controlUI.style.marginBottom = '22px';
        controlUI.style.textAlign = 'center';
        controlUI.title = 'Click to recenter the map';
        controlDiv.appendChild(controlUI);

        // Set CSS for the control interior.
        var controlText = document.createElement('div');
        controlText.style.color = 'rgb(25,25,25)';
        controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
        controlText.style.fontSize = '16px';
        controlText.style.lineHeight = '38px';
        controlText.style.paddingLeft = '5px';
        controlText.style.paddingRight = '5px';
        controlText.innerHTML = 'Где я?';
        controlUI.appendChild(controlText);

        // Setup the click event listeners: simply set the map to Chicago.
        controlUI.addEventListener('click', function () {

            if (navigator.geolocation) {

                navigator.geolocation.getCurrentPosition(
                function (position) {

                    deleteAllMarkersCurrent();

                    var location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                    _map.setCenter(location);
                    _map.setZoom(15);

                    var marker = new google.maps.Marker({
                        position: location,
                        map: _map,
			animation: google.maps.Animation.DROP,
			label: "Я",
                        title: "Ваше текущее положение"
                    });

                    _markersCurrentArray.push(marker);
                },
                function (error) {
                    alert("Ошибка определения текущего положения." + error.message);
                }, {
                    enableHighAccuracy: true,
                    timeout: 5000,
                    maximumAge: 0
                });
            } else {
                alert("Отключено определение текущего положения.");
            }

        });

    }

    var loadMap = function () {
        if (navigator.geolocation) {

            navigator.geolocation.getCurrentPosition(
            function (position) {
                var location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                _createMap(location);
            },
            function (error) {
                var location = new google.maps.LatLng(55.828345, 49.125938);
                _createMap(location);
            }, {
                enableHighAccuracy: true,
                timeout: 5000,
                maximumAge: 0
            });
        } else {
            var location = new google.maps.LatLng(55.828345, 49.125938);
            _createMap(location);
        }
    };

    var _createMap = function (location) {
        var mapOptions = {
            //minZoom: 10,
            zoom: 4,
            center: location,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        _map = new google.maps.Map($("#googlemaps")[0], mapOptions);
        google.maps.event.addListenerOnce(_map, 'idle', function () {
            loadPrinters();
        });
        google.maps.event.addListener(_map, 'click', closePrinterInfo);
        $("#googlemaps").on("heightChange", function () {
            google.maps.event.trigger(_map, "resize");
        });

        // Create the DIV to hold the control and call the CenterControl() constructor
        // passing in this DIV.
        var centerControlDiv = document.createElement('div');
        var centerControl = new CenterControl(centerControlDiv, _map);

        centerControlDiv.index = 1;
        _map.controls[google.maps.ControlPosition.TOP_CENTER].push(centerControlDiv);


    };

    var closePrinterInfo = function () {
        if (_currentPrinterID) {
            _currentPrinterID = null;
            $("#wrapper").toggleClass("toggled");
            _map.setZoom(_lastState.zoom);
            _map.setCenter(_lastState.center);
            _lastState = null;
        }
    };



    var image = "/Resources/Images/printer_online.png";

    var _addMarker = function (info) {
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(info.Latitude, info.Longtitude),
            map: _map,
            icon: image,
  
            title: info.Name
        });

        var printerInfo = info;
        google.maps.event.addListener(marker, 'click', function () {
            //infowindow.open(_map, marker);
            $("#printer-info").html(
                printerInfo.Name + "<br>" +
                "Адрес: " + printerInfo.Location + "<br>" +
                "Цена ч/б печати(стр): " + printerInfo.BlackWhitePrintPrice + "руб.<br>");

            $("#printer-print").prop("href", "/Printer/Print/" + printerInfo.PrinterID);
            $("#login-loginandprint").prop("href", "/Account/LoginAndPrint/" + printerInfo.PrinterID);
            if (!_currentPrinterID) {
                _lastState = {
                    zoom: _map.getZoom(),
                    center: _map.getCenter()
                };

                $("#wrapper").toggleClass("toggled");
                setTimeout(function () {
                    _zoomMarker(marker);
                }, 600);
            } else {
                _zoomMarker(marker);
            }
            _currentPrinterID = printerInfo.PrinterID;
        });
        _markersArray.push(marker);
    };

    var _zoomMarker = function (marker) {
        _map.setZoom(16);
        _map.setCenter(marker.getPosition());
    };

    function deleteAllMarkers() {
        if (_markersArray) {
            for (i in _markersArray) {
                _markersArray[i].setMap(null);
            }
            _markersArray.length = 0;
        }
    }

    function deleteAllMarkersCurrent() {
        if (_markersCurrentArray) {
            for (i in _markersCurrentArray) {
                _markersCurrentArray[i].setMap(null);
            }
            _markersCurrentArray.length = 0;
        }
    }

    var loadPrinters = function () {
        var lat0 = _map.getBounds().getNorthEast().lat();
        var lat1 = _map.getBounds().getSouthWest().lat();

        var lng0 = _map.getBounds().getNorthEast().lng();
        var lng1 = _map.getBounds().getSouthWest().lng();

        var boundaries = {
            minLatitude: lat1 >= lat0 ? lat0 : lat1,
            maxLatitude: lat1 >= lat0 ? lat1 : lat0,
            minLongtitude: lng1 >= lng0 ? lng0 : lng1,
            maxLongtitude: lng1 >= lng0 ? lng1 : lng0
        };

        $.ajax({
            type: 'GET',
            contentType: 'application/json',
            dataType: 'json',
            data: boundaries,
            url: '/Home/GetPrinters'
        }).done(function (json) {
            if (!json) {
                console.log('Error: ajax response is empty.');
                return;
            }

            deleteAllMarkers();
            $.each(json, function (index, e) {
                _addMarker(e);
            });
        }).fail(function () {
            console.log('Error: ajax call failed.');
        });
    };

    var setMapFullScreen = function () {
        var winHeight = $(window).height();
        var navheight = $(".main-navbar").height();
        $('#googlemaps').height(winHeight - navheight);
        $('#googlemaps').trigger("heightChange");
        $('#sidebar-wrapper').height(winHeight - navheight);
    };

    return {
        init: init,
        closePrinterInfo: closePrinterInfo,
        setMapFullScreen: setMapFullScreen
    };
}());

$(document).ready(function () {
    home.index.init();
    $("#close-printer-info").click(function () {
        home.index.closePrinterInfo();
    });

    $(window).resize(function () {
        home.index.setMapFullScreen();
    });

    home.index.setMapFullScreen();

    $(document).click(function (event) {
        var clickover = $(event.target);
        var _opened = $("#navbar").hasClass("navbar-collapse collapse in");
        if (_opened === true && !clickover.hasClass("navbar-toggle")) {
            $("button.navbar-toggle").click();
        }
    });
});