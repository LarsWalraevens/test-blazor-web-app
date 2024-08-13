export function load_map(raw) {
    let map = L.map('map', { zoomControl: false }).setView(new L.LatLng(raw[0].lat, raw[0].lng), 12);

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
                smoothFactor: 1
            });
            polyline.addTo(map);
        });
    }

    return "";
}

