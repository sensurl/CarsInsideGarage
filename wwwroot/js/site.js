document.addEventListener("DOMContentLoaded", function () {
    // Find any input field meant for a License Plate
    const plateInputs = document.querySelectorAll('input[name*="LicensePlate"]');

    plateInputs.forEach(input => {
        input.addEventListener('input', function () {
            // Force uppercase and remove spaces while typing
            this.value = this.value.toUpperCase().replace(/\s/g, '');
        });
    });
});

// Function to handle button loading states
document.addEventListener("submit", function (e) {
    const form = e.target;

    // Only show spinner if the HTML5 validation passes
    if (!form.checkValidity()) {
        return;
    }

    const submitBtn = form.querySelector('button[type="submit"]');
    const spinner = form.querySelector('.spinner-btn');

    if (submitBtn && spinner) {
        spinner.style.display = "inline-block";
        submitBtn.classList.add("disabled");
        submitBtn.style.opacity = "0.7";
    }
});

document.getElementById("btnUseLocation").addEventListener("click", () => {
    navigator.geolocation.getCurrentPosition(pos => {
        const lat = pos.coords.latitude;
        const lng = pos.coords.longitude;
        // TODO: Call controller or JS map function
        console.log("User location:", lat, lng);
    });
});

document.getElementById("btnSearchArea").addEventListener("click", () => {
    const query = document.getElementById("searchBox").value;
    // TODO: Geocode and show on map
    console.log("Search for:", query);
});