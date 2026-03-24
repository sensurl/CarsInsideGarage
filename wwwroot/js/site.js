document.addEventListener("DOMContentLoaded", function () {

    // ==========================================
    // 1. LICENSE PLATE FORMATTING
    // ==========================================
    const plateInputs = document.querySelectorAll('input[name*="CarPlateNumber"]');
    plateInputs.forEach(input => {
        input.addEventListener('input', function () {
            this.value = this.value.toUpperCase().replace(/\s/g, '');
        });
    });

    // ==========================================
    // 2. FORM SUBMIT SPINNER
    // ==========================================
    document.addEventListener("submit", function (e) {
        const form = e.target;
        if (!form.checkValidity()) return;

        const submitBtn = form.querySelector('button[type="submit"]');
        const spinner = form.querySelector('.spinner-btn');

        if (submitBtn && spinner) {
            const icon = submitBtn.querySelector('i');
            if (icon) icon.style.display = "none";

            spinner.style.display = "inline-block";
            submitBtn.classList.add("disabled");
            submitBtn.style.opacity = "0.7";
        }
    });

    // ==========================================
    // 3. UNIFIED LEAFLET MAP LOGIC
    // ==========================================
    const mapElement = document.getElementById("map");

    // Only proceed if a map div exists on the current page
    if (mapElement) {
        const mode = mapElement.dataset.mode;

        // Initial setup (Default: Sofia center if no data provided)
        const map = L.map('map').setView([42.6977, 23.3219], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        // --- MODE A: Single Garage Preview (Details View) ---
        if (mode === "preview") {
            const lat = parseFloat(mapElement.dataset.lat);
            const lng = parseFloat(mapElement.dataset.lng);
            const name = mapElement.dataset.name || "Garage Location";

            if (!isNaN(lat) && !isNaN(lng)) {
                map.setView([lat, lng], 15);
                L.marker([lat, lng]).addTo(map)
                    .bindPopup(`<b>${name}</b>`)
                    .openPopup();
            }
        }

        // --- MODE B: Nearby Garages (List View) ---
        else if (mode === "nearby") {
            const garagesData = mapElement.dataset.garages;
            if (garagesData) {
                const garages = JSON.parse(garagesData);
                const markers = [];

                garages.forEach(g => {
                    // Check for both C# casing (Latitude) and JS casing (latitude)
                    const lat = g.Latitude || g.latitude;
                    const lng = g.Longitude || g.longitude;
                    const name = g.Name || g.name;
                    const id = g.Id || g.id;

                    if (lat && lng) {
                        const m = L.marker([lat, lng])
                            .addTo(map)
                            .bindPopup(`<b>${name}</b><br><a href="/Garages/Details/${id}" class="btn btn-sm btn-link">View Details</a>`);
                        markers.push(m);
                    }
                });

                if (markers.length > 0) {
                    const group = new L.featureGroup(markers);
                    map.fitBounds(group.getBounds().pad(0.2));
                }
            }
        }

        // --- MODE C: Manual Location Picker (Home/Search View) ---
        else {
            let pickerMarker = null;

            map.on('click', function (e) {
                const lat = e.latlng.lat;
                const lng = e.latlng.lng;

                // Update hidden data attributes on the map element for the 'Use Selected' button
                mapElement.dataset.selectedLat = lat;
                mapElement.dataset.selectedLng = lng;

                if (pickerMarker) {
                    map.removeLayer(pickerMarker);
                }

                pickerMarker = L.marker([lat, lng])
                    .addTo(map)
                    .bindPopup("Selected location")
                    .openPopup();
            });
        }
    }

    // ==========================================
    // 4. BUTTON EVENT BINDINGS
    // ==========================================
    const btnUseSelected = document.getElementById("btnUseSelected");
    if (btnUseSelected) {
        btnUseSelected.addEventListener("click", function () {
            const mapEl = document.getElementById("map");
            const lat = mapEl.dataset.selectedLat;
            const lng = mapEl.dataset.selectedLng;

            if (!lat || !lng) {
                alert("Please click on the map to select a location first.");
                return;
            }
            window.location.href = `/Garages/GetNearbyGarages?lat=${lat}&lng=${lng}`;
        });
    }

    const btnSearchArea = document.getElementById("btnSearchArea");
    if (btnSearchArea) {
        btnSearchArea.addEventListener("click", () => {
            const query = document.getElementById("searchBox").value;
            console.log("Searching for location name:", query);
            // ToDo: Add Geocoding logic here if needed later
        });
    }
});

// ==========================================
// 5. GLOBAL GEOLOCATION FUNCTION
// ==========================================
function findNearest() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            window.location.href = `/Garages/GetNearbyGarages?lat=${lat}&lng=${lng}`;
        }, function (error) {
            alert("Unable to retrieve your location. Please ensure location services are enabled.");
        });
    } else {
        alert("Geolocation is not supported by your browser.");
    }
}