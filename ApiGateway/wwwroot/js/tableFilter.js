document.addEventListener('DOMContentLoaded', function () {
    const filterInput = document.getElementById('tableFilterInput');
    const tableBody = document.getElementById('apiCallsTableBody');

    if (filterInput && tableBody) {
        filterInput.addEventListener('keyup', function () {
            const filterValue = filterInput.value.toLowerCase(); // Get filter text and make it lowercase for case-insensitive search
            const rows = tableBody.getElementsByTagName('tr');

            // Loop through all table body rows
            for (let i = 0; i < rows.length; i++) {
                const row = rows[i];
                const cells = row.getElementsByTagName('td');
                let rowText = '';

                // Concatenate text from all cells in the row
                // Adjust this if you only want to filter specific columns
                for (let j = 0; j < cells.length; j++) {
                    rowText += cells[j].textContent || cells[j].innerText;
                }

                // Check if row text contains the filter value
                if (rowText.toLowerCase().indexOf(filterValue) > -1) {
                    row.style.display = ''; // Show row
                } else {
                    row.style.display = 'none'; // Hide row
                }
            }
        });
    } else {
        console.error("Filter input or table body element not found.");
    }

    // MutationObserver to observe changes in the table body
    const observer = new MutationObserver(function(mutations) {
        // Re-apply filter whenever the table body changes
        if (filterInput.value) { // Only re-filter if there's a filter value
            filterInput.dispatchEvent(new Event('keyup')); // Simulate keyup to trigger filter
        }
    });

    observer.observe(tableBody, { childList: true }); // Observe for added/removed child nodes (rows)
});
