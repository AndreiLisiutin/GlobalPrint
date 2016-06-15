home = home || {};
home.index = home.index || (function () {

    var _map = null;
    var _markersArray = [];

    var init = function () {
        loadMap();
    };

    var loadMap = function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
                _createMap(location);
            });
        } else {
            var location = new google.maps.LatLng(51.0532, 31.83);
            _createMap(location);
        }
    };

    var _createMap = function (location) {
        var mapOptions = {
            minZoom: 10,
            zoom: 13,
            center: location,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        _map = new google.maps.Map($("#googlemaps")[0], mapOptions);
        _map.addListener('idle', function () {
            loadPrinters();
        });
    };

    var _addMarker = function (latitude, longtitude, info) {
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(latitude, longtitude),
            map: _map,
            title: info
        });

        var contentString = '<div id="content">' + info + '</div>';
        var infowindow = new google.maps.InfoWindow({
            content: contentString
        });
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.open(_map, marker);
        });
        _markersArray.push(marker);
    };

    function deleteAllMarkers() {
        if (_markersArray) {
            for (i in _markersArray) {
                _markersArray[i].setMap(null);
            }
            _markersArray.length = 0;
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
                var url = "/Printer/Details/" + e.PrinterID;
                _addMarker(e.Latitude, e.Longtitude, "<a href=\"" + url + "\">Printer " + e.Name + "</a>");
            });
        }).fail(function () {
            console.log('Error: ajax call failed.');
        });
    };

    return {
        init: init
    };
}());

$(document).ready(function () {
    home.index.init();
});