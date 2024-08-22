export function load_map(raw) {
    let map = L.map('map', { zoomControl: false }).setView(new L.LatLng(raw[raw.length - 1].lat, raw[raw.length - 1].lng), 12);
    const ele = document.getElementById('map');

    // L.tileLayer('https://{s}.tile-cyclosm.openstreetmap.fr/cyclosm/{z}/{x}/{y}.png', { maxZoom: 22 }).addTo(map);
    L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', { maxZoom: 22 }).addTo(map);
    // L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', { maxZoom: 22 }).addTo(map);
    // L.tileLayer('https://tile.openstreetmap.bzh/br/{z}/{x}/{y}.png', { maxZoom: 22 }).addTo(map);

    if (raw) {
        var polylineSegments = [];
        var currentSegment = [];

        raw.forEach((element, i) => {
            if (raw[i + 1] === undefined) return;

            var pointA = new L.LatLng(element.lat, element.lng);
            var pointB = new L.LatLng(raw[i + 1].lat, raw[i + 1].lng);

            // Add pointA to the current segment
            currentSegment.push(pointA);

            // Check for longitude wrap (crossing the 180-degree meridian)
            if (Math.abs(pointB.lng - pointA.lng) > 180) {
                // Finish the current segment and start a new one
                polylineSegments.push(currentSegment);
                currentSegment = [];
            }

            // Add pointB to the current segment
            currentSegment.push(pointB);
        });

        // Push the last segment
        if (currentSegment.length > 0) {
            polylineSegments.push(currentSegment);
        }

        // Draw each polyline segment separately
        polylineSegments.forEach(segment => {
            var polyline = new L.polyline(segment, {
                color: '#0F99FF',
                weight: 2,
                opacity: 1,
                smoothFactor: 1,
                noClip: true,
                lineCap: 'round'
            });
            polyline.addTo(map);

        });
        // Add a marker at the end of the polyline
        var marker = L.divIcon({
            html: '<img src="/vessel.png" alt="Vessel" style="width:calc(15px*1.5); height:calc(40px*1.5); transform:rotate(45deg);"><span style="position: absolute; top: 215%; left: -43%; transform: rotate(315deg); text-transform: uppercase; text-align: left; font-weight:600;">abc</span>',
            style: "position:relative",
            className: 'my-div-icon',
            //         iconSize: [15, 41],

            iconAnchor: [7.5, 20.5],
            popupAnchor: [1, -34],
            shadowSize: [41, 41],
        });
        L.marker([raw[raw.length - 1].lat, raw[raw.length - 1].lng], { icon: marker }).addTo(map);
        // var marker = L.marker([raw[raw.length - 1].lat, raw[raw.length - 1].lng], {
        //     icon: L.icon({
        //         iconUrl: '/vessel.png',
        //         iconSize: [15, 41],
        //         iconAnchor: [7.5, 20.5],
        //         popupAnchor: [1, -34],
        //         shadowSize: [41, 41],
        //         rotationAngle: 45
        //     })
        // });
        // marker.addTo(map);
    }
    return "";
}
