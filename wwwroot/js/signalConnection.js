"use strict";

// --- SignalR Connection Setup ---
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/datahub") // Matches the endpoint mapping in Program.cs/Startup.cs
    .configureLogging(signalR.LogLevel.Information)
    .build();

// --- Helper function to get CSS class (JS version) ---
function getStatusColorClass(statusCode) {
    const codeStr = statusCode.toString();
    if (!codeStr) return "table-secondary";

    switch (codeStr[0]) {
        case '1': return "table-info";
        case '2': return "table-success";
        case '3': return "table-warning";
        case '4':
        case '5': return "table-danger";
        default: return "table-secondary";
    }
}

// --- Helper function to format DateTime ---
function formatDateTime(dateString) {
    if (!dateString) return "";
    try {
        const date = new Date(dateString);
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        const seconds = date.getSeconds().toString().padStart(2, '0');
        return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
    } catch (e) {
        console.error("Error formatting date:", dateString, e);
        return dateString; // Return original string on error
    }
}


// --- Function to add a row to the table ---
function addApiCallToTable(item) {
    const tableBody = document.getElementById("apiCallsTableBody");
    if (!tableBody) return; // Should not happen

    const tr = document.createElement("tr");
    tr.className = getStatusColorClass(item.statusCode); // Use item.statusCode (check casing from C# model)

    const thStatus = document.createElement("th");
    thStatus.scope = "row";
    thStatus.colSpan = 1;
    thStatus.textContent = item.statusCode; // Use item.statusCode

    const tdEndpoint = document.createElement("td");
    tdEndpoint.colSpan = 7;
    tdEndpoint.textContent = item.endpoint || item.source || 'N/A'; // Use item.endpoint or item.source

    const tdTime = document.createElement("td");
    tdTime.colSpan = 1;
    // Use item.time or item.requestTimestamp depending on what the hub sends
    tdTime.textContent = formatDateTime(item.time || item.requestTimestamp);

    tr.appendChild(thStatus);
    tr.appendChild(tdEndpoint);
    tr.appendChild(tdTime);

    // Add to the top for newest first, or appendChild for oldest first
    tableBody.prepend(tr);
}

// --- Register Hub Event Handlers ---

// Handler for receiving the initial batch of data
connection.on("ReceiveInitialData", (allData) => {
    console.log("Received initial data:", allData);
    const tableBody = document.getElementById("apiCallsTableBody");
    if (!tableBody) return;

    // Clear existing rows (like the "Connecting..." message)
    tableBody.innerHTML = "";

    if (allData && allData.length > 0) {
        allData.forEach(item => addApiCallToTable(item));
    } else {
        // Show message if no initial data
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center">No API calls recorded yet.</td></tr>';
    }
});

// Handler for receiving a single new data item
connection.on("ReceiveNewData", (newItem) => {
    console.log("Received new data:", newItem);
    const tableBody = document.getElementById("apiCallsTableBody");
    if (!tableBody) return;

    // Remove the "No API calls" message if it's present
    const firstRow = tableBody.querySelector('tr');
    if (firstRow && firstRow.cells.length === 1 && firstRow.cells[0].colSpan === 9) {
        tableBody.innerHTML = "";
    }

    addApiCallToTable(newItem);
});


// --- Start the Connection ---
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        // Note: The initial data fetch is handled by the Hub's OnConnectedAsync
        // which calls ReceiveInitialData on the client.
    } catch (err) {
        console.error("SignalR Connection Error: ", err);
        const tableBody = document.getElementById("apiCallsTableBody");
        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center text-danger">Could not connect to data stream. Please refresh.</td></tr>';
        }
        // Retry connection after a delay
        setTimeout(start, 5000);
    }
}

connection.onclose(async () => {
    console.log("SignalR Connection Closed.");
    // Attempt to reconnect
    await start();
});

// Start the connection when the script loads
start();