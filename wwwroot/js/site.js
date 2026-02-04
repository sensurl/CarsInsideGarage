document.addEventListener("DOMContentLoaded", function () {
    // Find any input field meant for a License Plate
    const plateInputs = document.querySelectorAll('input[name*="LicensePlate"]');

    plateInputs.forEach(input => {
        input.addEventListener('input', function () {
            // Force uppercase and remove spaces while they type
            this.value = this.value.toUpperCase().replace(/\s/g, '');
        });
    });
});