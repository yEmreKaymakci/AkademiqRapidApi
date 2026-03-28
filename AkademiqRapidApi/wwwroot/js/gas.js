document.addEventListener("DOMContentLoaded", function () {
    const gasDataElement = document.getElementById("gasDataJson");
    if (!gasDataElement) return;

    const gasData = JSON.parse(gasDataElement.textContent);
    let currentPage = 1;
    const rowsPerPage = 10;

    function renderTable(page) {
        const tbody = document.getElementById("gasTableBody");
        const start = (page - 1) * rowsPerPage;
        const end = start + rowsPerPage;
        const currentItems = gasData.slice(start, end);

        tbody.innerHTML = currentItems.map(item => `
            <tr>
                <td class="fw-bold text-white">${item.Country}</td>
                <td class="text-info">${item.Gasoline}</td>
                <td class="text-warning">${item.Diesel}</td>
                <td style="color: #a78bfa">${item.Lpg}</td>
                <td><small class="text-muted">${item.Currency}</small></td>
            </tr>
        `).join('');

        // UI Bilgilendirmesi Türkçe
        document.getElementById("pageInfo").innerText = `Kayıt: ${start + 1}-${Math.min(end, gasData.length)} / Toplam: ${gasData.length}`;
        document.getElementById("btnPrev").disabled = page === 1;
        document.getElementById("btnNext").disabled = end >= gasData.length;
    }

    // ApexCharts Grafiği (Grafik başlıkları Türkçe)
    const chartOptions = {
        series: [
            { name: 'Benzin', data: gasData.slice(0, 12).map(x => parseFloat(x.Gasoline.replace(',', '.'))) },
            { name: 'Motorin', data: gasData.slice(0, 12).map(x => parseFloat(x.Diesel.replace(',', '.'))) }
        ],
        chart: { type: 'bar', height: 350, toolbar: { show: false }, background: 'transparent' },
        colors: ['#22d3ee', '#fbbf24'],
        theme: { mode: 'dark' },
        xaxis: { categories: gasData.slice(0, 12).map(x => x.Country) },
        tooltip: { y: { title: { formatter: (val) => val + ":" } } }
    };

    new ApexCharts(document.querySelector("#gasChart"), chartOptions).render();

    document.getElementById("btnPrev").onclick = () => { if (currentPage > 1) renderTable(--currentPage); };
    document.getElementById("btnNext").onclick = () => { if (currentPage * rowsPerPage < gasData.length) renderTable(++currentPage); };

    renderTable(1);
});