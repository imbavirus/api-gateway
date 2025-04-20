document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.getElementById('apiCallsTableBody');
    const headers = document.querySelectorAll('.sortable-header');
    const initialSort = {
        columnIndex: 2, // Initially sort date descending
        ascending: false
    };
    let currentSort = {
        columnIndex: initialSort.columnIndex,
        ascending: initialSort.ascending
    }; // Track current sort state

    let originalRows = null; // Store original rows for reset if needed

    // Helper Function to Get Cell Value
    function getCellValue(row, columnIndex) {
        const cells = row.getElementsByTagName('td');
        if (cells.length > columnIndex) {
            const dataValue = cells[columnIndex].dataset.sortValue;
            if (dataValue !== undefined) {
                return dataValue;
            }
            return cells[columnIndex].textContent || cells[columnIndex].innerText;
        }
        return '';
    }

    // Sorting Function
    function sortTable(columnIndex) {
        if (!tableBody || columnIndex < 0) {
             console.log("Sort cleared or invalid column index.");
             return;
        }

        const rows = Array.from(tableBody.getElementsByTagName('tr'));

        if (originalRows === null) {
            originalRows = rows;
        }

        rows.sort((rowA, rowB) => {
            const valA = getCellValue(rowA, columnIndex);
            const valB = getCellValue(rowB, columnIndex);

            let comparison = 0;

            switch (columnIndex) {
                case 0: // Status Code
                    comparison = compareNumber(valA, valB);
                    break;
                case 2: // Time
                    comparison = compareDate(valA, valB);
                    break;
                default: // Default: String (Request/Endpoint)
                    comparison = compareString(valA, valB);
            }

            return currentSort.ascending ? comparison : comparison * -1;
        });

        // Update Table
        const fragment = document.createDocumentFragment();
        rows.forEach(row => fragment.appendChild(row));
        tableBody.innerHTML = '';
        tableBody.appendChild(fragment);

        // Update Headers
        updateHeaderIcons();
    }

    function resetTableOrder() {
        if (!tableBody || !originalRows) return;
        console.log("Resetting table to original order.");
        const fragment = document.createDocumentFragment();
        originalRows.forEach(row => fragment.appendChild(row));
        tableBody.innerHTML = '';
        tableBody.appendChild(fragment);
        currentSort.columnIndex = initialSort.columnIndex; // Reset to initial sort column
        currentSort.ascending = initialSort.ascending; // Reset to initial sort direction
        updateHeaderIcons();
    }

    // Update Header Icons
    function updateHeaderIcons() {
         headers.forEach(header => {
            const icon = header.querySelector('.sort-icon');
            if (icon) {
                icon.classList.remove('bi-arrow-up', 'bi-arrow-down', 'text-muted');
                const headerColumnIndex = parseInt(header.getAttribute('data-column-index'), 10);

                if (headerColumnIndex === currentSort.columnIndex) {
                    icon.classList.add(currentSort.ascending ? 'bi-arrow-up' : 'bi-arrow-down');
                }
            }
        });
    }


    // Attach Event Listeners
    headers.forEach(header => {
        header.addEventListener('click', () => {
            const clickedColumnIndex = parseInt(header.getAttribute('data-column-index'), 10);

            if (currentSort.columnIndex === clickedColumnIndex) {
                // Clicking the same column
                if (!currentSort.ascending) {
                    // Was descending, now ascending
                    currentSort.ascending = true;
                    sortTable(clickedColumnIndex); // Re-sort ascending
                } else {
                    // Was ascending, now remove sort
                    currentSort.columnIndex = -1; // Mark sort as inactive
                    currentSort.ascending = false; // Reset direction for next time
                    resetTableOrder();
                    console.log("Sort removed for column", clickedColumnIndex);
                }
            } else {
                // Clicking a new column, sort descending
                currentSort.columnIndex = clickedColumnIndex;
                currentSort.ascending = false;
                sortTable(clickedColumnIndex); // Sort descending
            }
        });
    });

    // Function to Re-sort Table Based on Current State
    function resortTableIfActive() {
        if (currentSort.columnIndex !== -1) {
            console.log(`Re-sorting by column ${currentSort.columnIndex}, ascending: ${currentSort.ascending}`);
            // Call sortTable with the *current* sort column index.
            sortTable(currentSort.columnIndex);
        }
    }

    // Comparison Helpers
    function compareString(a, b) {
        // Ensure values are strings for localeCompare
        const strA = String(a || '');
        const strB = String(b || '');
        return strA.localeCompare(strB);
    }
    function compareNumber(a, b) {
        const numA = parseFloat(a);
        const numB = parseFloat(b);
        if (isNaN(numA) && isNaN(numB)) return 0;
        if (isNaN(numA)) return 1;
        if (isNaN(numB)) return -1;
        return numA - numB;
    }
    function compareDate(a, b) {
        // Attempt to create Date objects, handle invalid dates
        const dateA = new Date(a);
        const dateB = new Date(b);
        const timeA = !isNaN(dateA.getTime()) ? dateA.getTime() : -Infinity;
        const timeB = !isNaN(dateB.getTime()) ? dateB.getTime() : -Infinity;
        return timeA - timeB;
    }

    // Expose the re-sort function globally
    window.tableSorter = {
        resort: resortTableIfActive
    };

    // Initial Setup
    updateHeaderIcons();

});
