document.addEventListener("DOMContentLoaded", function () {
    const dataElement = document.getElementById("currencyDataJson");
    if (!dataElement) return;

    const data = JSON.parse(dataElement.textContent);
    const rates = data.rates;
    // Rates nesnesini [ {code: "USD", val: 0.03}, ... ] formatına çeviriyoruz
    const rateList = Object.keys(rates).map(key => ({ code: key, val: rates[key] }));

    let currentPage = 1;
    const rowsPerPage = 10;

    // --- Tablo ---
    function renderTable(page) {
        const tbody = document.getElementById("rateTableBody");
        const start = (page - 1) * rowsPerPage;
        const currentData = rateList.slice(start, start + rowsPerPage);

        tbody.innerHTML = currentData.map(item => `
            <tr>
                <td class="fw-bold" style="color: #00f2ff">${item.code}</td>
                <td>${item.val.toFixed(6)}</td>
                <td class="text-warning">${(1 / item.val).toFixed(2)} ₺</td>
            </tr>
        `).join('');
    }

    // --- Hesaplayıcı ---
    const amountInput = document.getElementById("calcAmount");
    const targetSelect = document.getElementById("calcTarget");
    const resultH3 = document.getElementById("calcResult");

    function calculate() {
        const amount = parseFloat(amountInput.value) || 0;
        const targetRate = rates[targetSelect.value];
        const result = amount * targetRate;
        resultH3.innerText = `${result.toFixed(2)} ${targetSelect.value}`;
    }

    amountInput.oninput = calculate;
    targetSelect.onchange = calculate;
    calculate();

    // --- Grafik ---
    const mainCodes = ['USD', 'EUR', 'GBP', 'AUD', 'CAD', 'CHF'];
    const chartOptions = {
        series: [{ name: '1 Birim Kaç TL?', data: mainCodes.map(c => (1 / rates[c]).toFixed(2)) }],
        chart: { type: 'area', height: 300, toolbar: { show: false }, background: 'transparent' },
        colors: ['#ff24e4'],
        theme: { mode: 'dark' },
        stroke: { curve: 'smooth', width: 3 },
        xaxis: { categories: mainCodes },
        fill: { type: 'gradient', gradient: { shadeIntensity: 1, opacityFrom: 0.4, opacityTo: 0.1 } }
    };
    new ApexCharts(document.querySelector("#currencyChart"), chartOptions).render();

    // --- Sayfalama ---
    document.getElementById("prevBtn").onclick = () => { if (currentPage > 1) renderTable(--currentPage); };
    document.getElementById("nextBtn").onclick = () => { if (currentPage * rowsPerPage < rateList.length) renderTable(++currentPage); };

    renderTable(1);
});