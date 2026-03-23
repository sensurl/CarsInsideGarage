document.addEventListener("DOMContentLoaded", function () {

    // =========================
    // License plate formatting
    // =========================
    const plateInputs = document.querySelectorAll('input[name*="CarPlateNumber"]');
    plateInputs.forEach(input => {
        input.addEventListener('input', function () {
            this.value = this.value.toUpperCase().replace(/\s/g, '');
        });
    });

    // =========================
    // Form submit spinner
    // =========================
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

    // =========================
    // MAP VARIABLES
    // =========================
    let map;
    let selectedLat = null;
    let selectedLng = null;
    let marker = null;

    // =========================
    // MAP INIT
    // =========================
    function initMap() {
        const lat = 42.6977;
        const lng = 23.3219;

        map = L.map('map').setView([lat, lng], 13);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        L.marker([lat, lng]).addTo(map)
            .bindPopup('Sofia City Center')
            .openPopup();

        // CLICK EVENT
        map.on('click', function (e) {
            selectedLat = e.latlng.lat;
            selectedLng = e.latlng.lng;

            if (marker) {
                map.removeLayer(marker);
            }

            marker = L.marker([selectedLat, selectedLng])
                .addTo(map)
                .bindPopup("Selected location")
                .openPopup();
        });
    }

    // =========================
    // Use Selected Location
    // =========================
    function useSelectedLocation() {
        if (!selectedLat || !selectedLng) {
            alert("Please select a location on the map.");
            return;
        }

        window.location.href = `/Garages/GetNearbyGarages?lat=${selectedLat}&lng=${selectedLng}`;
    }

    // =========================
    // Search Area
    // =========================
    const btnSearchArea = document.getElementById("btnSearchArea");
    if (btnSearchArea) {
        btnSearchArea.addEventListener("click", () => {
            const query = document.getElementById("searchBox").value;
            console.log("Search for:", query);
        });
    }

    // =========================
    // Bind NEW button
    // =========================
    const btnUseSelected = document.getElementById("btnUseSelected");
    if (btnUseSelected) {
        btnUseSelected.addEventListener("click", useSelectedLocation);
    }

    // =========================
    // INIT MAP ONLY IF PRESENT
    // =========================
    if (document.getElementById("map")) {
        initMap();
    }
});


// =========================
// GLOBAL FUNCTION
// =========================
function findNearest() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;

            window.location.href = `/Garages/GetNearbyGarages?lat=${lat}&lng=${lng}`;
        });
    } else {
        alert("Geolocation is not supported.");
    }
}
