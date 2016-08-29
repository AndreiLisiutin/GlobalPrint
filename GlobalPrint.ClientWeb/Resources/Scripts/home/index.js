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
            $("#sidebar-wrapper").addClass("hidden");
            _map.setZoom(_lastState.zoom);
            _map.setCenter(_lastState.center);
            _lastState = null;
        }
    };

    var _zoomMarker = function (marker) {
        _map.setZoom(16);
        _map.setCenter(marker.getPosition());
    };

    function deleteAllMarkers() {
        if (_markersArray) {
            $.each(_markersArray, function (index, item) {
                item.setMap(null);
            });
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
        //get google map geographical boundaries
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

    var image = "/Resources/Images/printer_online.png";
    var DayOfWeek = {
        ПН: 1,
        ВТ: 2,
        СР: 3,
        ЧТ: 4,
        ПТ: 5,
        СБ: 6,
        ВС: 0
    };
    var _addMarker = function (printerInfo) {
        //adding new marker to the map.
        //printerInfo is a model for C# PrinterFullInfoModel class.
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(printerInfo.Printer.Latitude, printerInfo.Printer.Longtitude),
            map: _map,
            icon: image,
            title: printerInfo.Printer.Name,
            printerInfo: printerInfo
        });

        google.maps.event.addListener(marker, 'click', function () {
            $("#printerInfoPrinterID").val(printerInfo.Printer.ID);
            $("#printerInfoIsAvailable").val(printerInfo.IsAvailableNow ? 'Доступен' : 'Не доступен');
            $("#printerInfoName").val(printerInfo.Printer.Name);
            $("#printerInfoLocation").val(printerInfo.Printer.Location);
            function toFixedInt2(n) {
                return n > 9 ? "" + n : "0" + n;
            }

            var averallSchedule = '';
            for (var day in DayOfWeek) {
                var daySchedules = $.grep(printerInfo.PrinterSchedule, function (item, index) {
                    return item.DayOfWeek == DayOfWeek[day];
                }).sort(function (x, y) {
                    return x.OpenTime.TotalMilliseconds - y.OpenTime.TotalMilliseconds;
                });

                var scheduleString = '';
                $.each(daySchedules, function (index, item) {
                    scheduleString += (scheduleString ? ' ... ' : '') +
                        toFixedInt2(item.OpenTime.Hours) + ':' + toFixedInt2(item.OpenTime.Minutes) + '-' +
                        toFixedInt2(item.CloseTime.Hours) + ':' + toFixedInt2(item.CloseTime.Minutes);
                });
                scheduleString = scheduleString || 'Не работает';
                averallSchedule += (averallSchedule ? '\n' : '') + day + ':.....' + scheduleString;
            }
            $("#printerInfoSchedule").val(averallSchedule);

            var averallServices = '';
            $.each(marker.printerInfo.PrinterServices, function (index, item) {
                var service = ''
                service += item.PrintService.PrintType.Name + ' ' +
                    item.PrintService.PrintSize.Name + ' ' + 
                    (item.PrintService.IsColored ? 'Цветная' : 'Ч/Б') + 
                    (item.PrintService.IsTwoSided ? 'Двусторонняя' : '');

                service += ':.....' + item.PrinterService.PricePerPage;
                averallServices += (averallServices ? '\n' : '') + service;
            });
            $("#printerInfoPrices").val(averallServices);

            if (!_currentPrinterID) {
                _lastState = {
                    zoom: _map.getZoom(),
                    center: _map.getCenter()
                };

                $("#sidebar-wrapper").removeClass("hidden");
                setTimeout(function () {
                    _zoomMarker(marker);
                }, 600);
            } else {
                _zoomMarker(marker);
            }
            _currentPrinterID = marker.printerInfo.Printer.ID;
        });
        _markersArray.push(marker);
    };

    var setMapFullScreen = function () {
        var winHeight = $(window).height();
        var navheight = $(".main-navbar").height();
        var fullHeight = winHeight - navheight;
        $('#googlemaps').height(fullHeight);
        $('#googlemaps').trigger("heightChange");
        $('#sidebar-wrapper').height(fullHeight);
    };

    return {
        init: init,
        closePrinterInfo: closePrinterInfo,
        setMapFullScreen: setMapFullScreen
    };
}());

$(document).ready(function () {
    home.index.init();
    $("#printerInfoClose").click(function () {
        //Close button in print details sidebar
        home.index.closePrinterInfo();
    });

    $("#printerInfoPrint").click(function (event) {
        //Print button in print details sidebar
        var isUserAuthenticated = $("body").data('isAuthenticated');
        var printerID = $("#printerInfoPrinterID").val();

        if (!printerID) {
            console.log('printerID is not defined for printing.');
            return;
        }

        var printUrl = isUserAuthenticated
            ? ("/Printer/Print/" + printerID)
            : ("/Account/LoginAndPrint/" + printerID);

        window.location.href = printUrl;
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