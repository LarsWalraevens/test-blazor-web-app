export function load_map(raw) {
    let map = L.map('map').setView(new L.LatLng(raw[0].lat, raw[0].lng), 16);
    console.log(JSON.stringify(raw));
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);

    if (raw) {
        var polylinePoints = [];

        raw.forEach((element, i) => {
            if (raw[i + 1] === undefined) return
            var pointA = new L.LatLng(element.lat, element.lng);
            var pointB = new L.LatLng(raw[i + 1].lat, raw[i + 1].lng);
            polylinePoints = polylinePoints.concat([pointA, pointB]);

        });
        var polyline = new L.polyline(polylinePoints, {
            color: 'blue',
            weight: 3,
            opacity: 0.5,
            smoothFactor: 1

        });
        polyline.addTo(map);

    }

    return "";
}
