document.addEventListener('DOMContentLoaded', function() {
    const roleSelect = document.querySelector('#Role');
    if (roleSelect) {
        // Hide first option (placeholder)
        const firstOption = roleSelect.querySelector('option:first-child');
        if (firstOption) {
            firstOption.disabled = true;
            firstOption.hidden = true;
            firstOption.selected = false;
        }

        // Select second option by default if nothing selected
        if (!roleSelect.value || roleSelect.value === firstOption.value) {
            roleSelect.selectedIndex = 1;
        }

        // Prevent selecting first option
        roleSelect.addEventListener('change', function(e) {
            if (e.target.selectedIndex === 0) {
                e.target.selectedIndex = 1;
            }
        });
    }
});