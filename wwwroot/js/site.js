document.addEventListener("DOMContentLoaded", function () {

    // License plate formatting
    const plateInputs = document.querySelectorAll('input[name*="CarPlateNumber"]');
    plateInputs.forEach(input => {
        input.addEventListener('input', function () {
            this.value = this.value.toUpperCase().replace(/\s/g, '');
        });
    });

    // Form submit spinner
    document.addEventListener("submit", function (e) {
        const form = e.target;
        if (!form.checkValidity()) return;

        const submitBtn = form.querySelector('button[type="submit"]');
        const spinner = form.querySelector('.spinner-btn');

        if (submitBtn && spinner) {
            spinner.style.display = "inline-block";
            submitBtn.classList.add("disabled");
            submitBtn.style.opacity = "0.7";
        }
    });

    // Use Location button
    const btnUseLocation = document.getElementById("btnUseLocation");
    if (btnUseLocation) {
        btnUseLocation.addEventListener("click", () => {
            navigator.geolocation.getCurrentPosition(pos => {
                const lat = pos.coords.latitude;
                const lng = pos.coords.longitude;
                console.log("User location:", lat, lng);
            });
        });
    }

    // Search Area button
    const btnSearchArea = document.getElementById("btnSearchArea");
    if (btnSearchArea) {
        btnSearchArea.addEventListener("click", () => {
            const query = document.getElementById("searchBox").value;
            console.log("Search for:", query);
        });
    }
});

// Global function
function findNearest() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;

            window.location.href = `/Garages/Nearest?lat=${lat}&lng=${lng}`;
        });
    } else {
        alert("Geolocation is not supported.");
    }
}
