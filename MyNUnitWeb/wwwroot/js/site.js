document.getElementById("uploadButton")
    .addEventListener("click", upload);

document.getElementById("runButton")
    .addEventListener("click", runTests);

document.getElementById("historyButton")
    .addEventListener("click", loadHistory);

async function upload() {
    const fileInput = document.getElementById("fileInput");

    if (fileInput.files.length === 0) {
        alert("Select a DLL file first");
        return;
    }

    const formData = new FormData();
    formData.append("file", fileInput.files[0]);

    const response = await fetch("/api/tests/upload", {
        method: "POST",
        body: formData
    });

    if (!response.ok) {
        const text = await response.text();
        alert("Upload failed: " + text);
        return;
    }

    alert("File uploaded successfully");
}

async function runTests() {
    const response = await fetch("http://localhost:5194/api/tests/run", {
        method: "POST"
    });

    if (!response.ok) {
        alert("Failed to run tests");
        return;
    }

    const data = await response.json();

    showResults(data.results);
}

async function loadHistory() {
    const response = await fetch("/api/tests/history");

    if (!response.ok) {
        alert("Failed to load history");
        return;
    }

    const history = await response.json();
    showHistory(history);
}

function showResults(results) {
    const container = document.getElementById("results");
    container.innerHTML = "";

    results.forEach(result => {
        const div = document.createElement("div");

        const duration = result.duration ?? 0;

        div.textContent =
            `[${result.status}] ${result.testName} (${duration} ms)` +
            (result.message ? ` — ${result.message}` : "");

        container.appendChild(div);
    });
}

function showResults(results) {
    const container = document.getElementById("results");
    container.innerHTML = "";

    results.forEach(result => {
        const div = document.createElement("div");
        div.className = result.status.toLowerCase();

        div.innerText =
            `[${result.status}] ${result.testName} ` +
            `(${result.executionTimeMs} ms)` +
            (result.message ? " — " + result.message : "");

        container.appendChild(div);
    });
}

function countStats(results) {
    let passed = 0;
    let failed = 0;
    let ignored = 0;

    results.forEach(r => {
        if (r.status === "Passed") {
            passed++;
        }
        else if (r.status === "Failed") {
            failed++;
        }
        else if (r.status === "Ignored") {
            ignored++;
        }
    });

    return { passed, failed, ignored };
}