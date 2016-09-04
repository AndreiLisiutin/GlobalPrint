GlobalPrint.namespace('GlobalPrint.Home.Index');
(function (HomeIndex) {

    var PRINTER_ONLINE_ICON_URL = "/Resources/Images/printer_online.png";
    var PRINTER_OFFLINE_ICON_URL = "/Resources/Images/printer_offline.png";
    var PRINTER_DEACTIVE_ICON_URL = "/Resources/Images/printer_deactive.png";

    //select printer's icon by its state
    var _getPrinterIcon = function (printerInfo) {
        if (printerInfo.IsAvailableNow) {
            return PRINTER_ONLINE_ICON_URL;
        }
        if (printerInfo.Printer.IsDisabled) {
            return PRINTER_DEACTIVE_ICON_URL;
        }
        return PRINTER_OFFLINE_ICON_URL;
    };

    var _map = null;
    var _markersArray = [];
    var _currentPrinterID = null;
    var _lastState = null;

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
            _map.setCenter(location);
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
            zoom: isUserAuthenticated ? 16 : 4,
            center: location,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        _map = new google.maps.Map($("#googlemaps")[0], mapOptions);
        google.maps.event.addListenerOnce(_map, 'idle', function () {
            loadPrinters();
        });
        google.maps.event.addListener(_map, 'click', HomeIndex.closePrinterInfo);
        $("#googlemaps").on("heightChange", function () {
            //for resize map with browser resize
            google.maps.event.trigger(_map, "resize");
        });
    };

    HomeIndex.closePrinterInfo = function () {
        if (_currentPrinterID) {
            _currentPrinterID = null;
            $("#homeInfoSidebar").addClass("hidden");
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
        loadPrinters();
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
                        console.log('Error: ajax response is empty.');
                        return;
                    }
                    deleteMarkerByPrinterID(json.Printer.ID);
                    var marker = _addMarker(json, true);
                    _zoomMarker(marker);
                }).fail(function () {
                    console.log('Error: ajax call failed.');
                });
            },
            function (error) {
                console.log(error.message);
            }
        );
    };

    var loadPrinters = function () {
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
            $.each(json, function (index, e) {
                _addMarker(e);
            });
        }).fail(function () {
            console.log('Error: ajax call failed.');
        });
    };

    var DayOfWeek = {
        ПН: 1,
        ВТ: 2,
        СР: 3,
        ЧТ: 4,
        ПТ: 5,
        СБ: 6,
        ВС: 0
    };
    var _addMarker = function (printerInfo, animation) {
        //adding new marker to the map.
        //printerInfo is a model for C# PrinterFullInfoModel class.
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(printerInfo.Printer.Latitude, printerInfo.Printer.Longtitude),
            map: _map,
            icon: _getPrinterIcon(printerInfo),
            title: printerInfo.Printer.Name,
            animation: animation ? google.maps.Animation.DROP : null,
            printerID: printerInfo.Printer.ID,
            printerInfo: printerInfo
        });

        google.maps.event.addListener(marker, 'click', function () {
            $("#printerInfoPrinterID").val(printerInfo.Printer.ID);
            $("#printerInfoIsAvailable").val(printerInfo.IsAvailableNow);
            $("#printerInfoName").val(printerInfo.Printer.Name);
            $("#printerInfoLocation").val(printerInfo.Printer.Location);
            function toFixedInt2(n) {
                return n > 9 ? "" + n : "0" + n;
            }
            var name = printerInfo.Printer.Name;
            var location = printerInfo.Printer.Location;
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
            $("#printerInfoOperator").val(name + '\n' + location + '\n' + averallSchedule);

            var averallServices = '';
            $.each(marker.printerInfo.PrinterServices, function (index, item) {
                var service = ''
                service += item.PrintService.PrintType.Name + ' ' +
                    item.PrintService.PrintSize.Name + ' ' +
                    (item.PrintService.IsColored ? 'Цветная' : 'Ч/Б') +
                    (item.PrintService.IsTwoSided ? 'Двусторонняя' : '');

                service += ':.....' + item.PrinterService.PricePerPage + ' руб.';
                averallServices += (averallServices ? '\n' : '') + service;
            });
            $("#printerInfoPrices").val(averallServices);

            if (!_currentPrinterID) {
                _lastState = {
                    zoom: _map.getZoom(),
                    center: _map.getCenter()
                };

                $("#homeInfoSidebar").removeClass("hidden");
                setTimeout(function () {
                    _zoomMarker(marker);
                }, 600);
            } else {
                _zoomMarker(marker);
            }
            _currentPrinterID = marker.printerInfo.Printer.ID;
        });
        _markersArray.push(marker);
        return marker;
    };

    HomeIndex.setMapFullScreen = function () {
        var winHeight = $(window).height();
        var navheight = $(".main-navbar").height();
        var fullHeight = winHeight - navheight;
        $('#googlemaps').height(fullHeight);
        $('#googlemaps').trigger("heightChange");
        $('#homeInfoSidebar').height(fullHeight);
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
            ? ("/Printer/Print/" + printerID)
            : ("/Account/LoginAndPrint/" + printerID);

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
        var clickover = $(event.target);
        var _opened = $("#navbar").hasClass("navbar-collapse collapse in");
        if (_opened === true && !clickover.hasClass("navbar-toggle")) {
            $("button.navbar-toggle").click();
        }
    });
});