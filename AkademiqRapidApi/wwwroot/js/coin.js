document.addEventListener("DOMContentLoaded", function () {
    const dataEl = document.getElementById("coinDataJson");
    if (!dataEl) return;

    const root = JSON.parse(dataEl.textContent);
    const allCoins = root.data || [];

    // Formatter helpers
    const formatMoney = (val) => {
        let num = parseFloat(val);
        if (isNaN(num)) return "0.00";
        if (num > 100) return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 5 });
    };

    const formatTrend = (val) => {
        let num = parseFloat(val);
        if (isNaN(num)) return { cls: 'text-secondary', text: '0.00%', icon: '' };
        if (num >= 0) return { cls: 'text-trend-up', text: `${num}%`, icon: '&#9650;' };
        return { cls: 'text-trend-down', text: `${num}%`, icon: '&#9660;' };
    };

    // --- 1. Featured Coins ---
    const featuredArea = document.getElementById("featuredCoinsArea");
    const targets = ["Bitcoin", "Ethereum", "Solana", "XRP"];
    const featured = allCoins.filter(c => targets.includes(c.name));

    featuredArea.innerHTML = featured.map(c => {
        let trend = formatTrend(c.percent_change_24h);
        return `
        <div class="col-md-6 col-lg-3">
            <div class="glass-panel p-4 h-100 position-relative overflow-hidden">
                <div class="d-flex align-items-center gap-3 mb-3">
                    <div class="coin-logo-box highlight shadow-sm">${c.symbol.substring(0,3)}</div>
                    <div>
                        <h6 class="mb-0 text-white fw-bold">${c.name}</h6>
                        <small class="text-secondary">${c.symbol}</small>
                    </div>
                </div>
                <div class="crypto-price mb-2">$ ${formatMoney(c.price_usd)}</div>
                <div class="${trend.cls} small d-flex align-items-center gap-1">
                    <span style="font-size: 10px;">${trend.icon}</span> 
                    <span>${trend.text}</span>
                    <span class="text-secondary ms-1">(24s)</span>
                </div>
                
                <!-- Background decorative circle -->
                <div style="position:absolute; right:-20px; bottom:-20px; width:100px; height:100px; border-radius:50%; background:var(--crypto-green); opacity:0.05; filter:blur(20px); pointer-events:none;"></div>
            </div>
        </div>
        `;
    }).join('');

    // --- 2. Table & Pagination ---
    let currentPage = 1;
    const rowsPerPage = 10;
    const totalPages = Math.ceil(allCoins.length / rowsPerPage);

    function renderTable(page) {
        const tbody = document.getElementById("coinTableBody");
        const start = (page - 1) * rowsPerPage;
        const currentData = allCoins.slice(start, start + rowsPerPage);

        tbody.innerHTML = currentData.map((c, index) => {
            let trend = formatTrend(c.percent_change_24h);
            return `
            <tr>
                <td class="text-muted fw-bold">#${c.rank}</td>
                <td>
                    <div class="d-flex align-items-center gap-3">
                        <div class="coin-logo-box">${c.symbol.substring(0,2)}</div>
                        <div>
                            <span class="d-block text-white fw-bold">${c.name}</span>
                            <span class="text-muted small">${c.symbol}</span>
                        </div>
                    </div>
                </td>
                <td class="text-white fw-semibold">$ ${formatMoney(c.price_usd)}</td>
                <td class="text-secondary d-none d-md-table-cell">$ ${parseFloat(c.market_cap_usd).toLocaleString('en-US')}</td>
                <td>
                    <span class="${trend.cls} px-2 py-1 rounded" style="background: rgba(255,255,255,0.03);">
                        <span style="font-size:10px;">${trend.icon}</span> ${trend.text}
                    </span>
                </td>
            </tr>
            `;
        }).join('');

        document.getElementById("coinPageIndicator").innerText = `${page} / ${totalPages}`;
        document.getElementById("coinTotalInfo").innerText = `Anlık ${allCoins.length} varlık sergileniyor`;
        document.getElementById("coinPrevBtn").disabled = page === 1;
        document.getElementById("coinNextBtn").disabled = page === totalPages;
    }

    if (allCoins.length > 0) {
        renderTable(1);
    } else {
        document.getElementById("coinTableBody").innerHTML = `<tr><td colspan="5" class="text-center text-muted py-4">Veri bulunamadı. Lütfen API bağlantınızı kontrol edin.</td></tr>`;
    }

    document.getElementById("coinPrevBtn").onclick = () => { if (currentPage > 1) renderTable(--currentPage); };
    document.getElementById("coinNextBtn").onclick = () => { if (currentPage < totalPages) renderTable(++currentPage); };
});