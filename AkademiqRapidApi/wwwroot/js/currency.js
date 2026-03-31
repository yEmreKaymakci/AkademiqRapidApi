document.addEventListener("DOMContentLoaded", function () {
    const dataElement = document.getElementById("currencyDataJson");
    if (!dataElement) return;

    const data = JSON.parse(dataElement.textContent);
    const rates = data.rates;
    // Kurları liste haline getir (TRY hariç tutulabilir veya dahil edilebilir)
    const rateList = Object.keys(rates).sort().map(key => ({ code: key, val: rates[key] }));

    let currentPage = 1;
    const rowsPerPage = 10;
    const totalPages = Math.ceil(rateList.length / rowsPerPage);

    // --- 1. HESAPLAYICI MANTIĞI ---
    const sourceSelect = document.getElementById("calcSource");
    const targetSelect = document.getElementById("calcTarget");
    const amountInput = document.getElementById("calcAmount");
    const resultDisplay = document.getElementById("calcResult");

    const selectOptions = rateList.map(item => `<option value="${item.code}">${item.code}</option>`).join('');
    sourceSelect.innerHTML = selectOptions;
    targetSelect.innerHTML = selectOptions;

    sourceSelect.value = "USD";
    targetSelect.value = "TRY";

    function calculateCrossRate() {
        const amount = parseFloat(amountInput.value) || 0;
        const sourceRate = rates[sourceSelect.value];
        const targetRate = rates[targetSelect.value];

        // Formül: Miktar * (HedefKur / KaynakKur)
        const result = amount * (targetRate / sourceRate);

        resultDisplay.innerText = `${result.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 4 })} ${targetSelect.value}`;
    }

    [sourceSelect, targetSelect, amountInput].forEach(el => el.addEventListener('input', calculateCrossRate));

    // --- 2. TABLO VE SAYFALAMA MANTIĞI ---
    function renderTable(page) {
        const tbody = document.getElementById("rateTableBody");
        const start = (page - 1) * rowsPerPage;
        const currentData = rateList.slice(start, start + rowsPerPage);

        tbody.innerHTML = currentData.map(item => `
            <tr class="bento-item">
                <td class="fw-bold" style="color: var(--cur-cyan)">${item.code}</td>
                <td class="text-secondary">${item.val.toFixed(6)}</td>
                <td class="text-warning">${(1 / item.val).toFixed(4)} ₺</td>
                <td class="text-end"><span class="badge bg-success bg-opacity-10 text-success" style="font-size:10px">AKTİF</span></td>
            </tr>
        `).join('');

        document.getElementById("pageIndicator").innerText = `${page} / ${totalPages}`;
        document.getElementById("totalInfo").innerText = `Toplam ${rateList.length} birimden ${start + 1}-${Math.min(start + rowsPerPage, rateList.length)} arası gösteriliyor`;

        document.getElementById("prevBtn").disabled = page === 1;
        document.getElementById("nextBtn").disabled = page === totalPages;
    }

    document.getElementById("prevBtn").onclick = () => { if (currentPage > 1) renderTable(--currentPage); };
    document.getElementById("nextBtn").onclick = () => { if (currentPage < totalPages) renderTable(++currentPage); };

    // --- 3. GRAFİK MANTIĞI ---
    const mainCodes = ['USD', 'EUR', 'GBP', 'CHF', 'CAD', 'JPY'];
    const chartOptions = {
        series: [{ name: 'Değer (TRY)', data: mainCodes.map(c => (1 / rates[c]).toFixed(2)) }],
        chart: { type: 'area', height: 320, toolbar: { show: false }, background: 'transparent' },
        colors: ['#00f2ff'],
        theme: { mode: 'dark' },
        stroke: { curve: 'smooth', width: 3 },
        xaxis: { categories: mainCodes },
        fill: { type: 'gradient', gradient: { shadeIntensity: 1, opacityFrom: 0.4, opacityTo: 0.1 } },
        grid: { borderColor: '#21262d' }
    };
    new ApexCharts(document.querySelector("#currencyChart"), chartOptions).render();

    // Başlat
    renderTable(1);
    calculateCrossRate();
});