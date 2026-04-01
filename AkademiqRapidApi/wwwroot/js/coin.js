document.addEventListener("DOMContentLoaded", function () {
    const dataEl = document.getElementById("coinDataJson");
    if (!dataEl) return;

    const root = JSON.parse(dataEl.textContent);
    const allCoins = root.data.coins || [];

    // --- 1. Featured Coins (Öne Çıkanlar) ---
    const featuredArea = document.getElementById("featuredCoinsArea");
    const targets = ["Bitcoin", "Ethereum", "Solana", "XRP"];
    const featured = allCoins.filter(c => targets.includes(c.name));

    featuredArea.innerHTML = featured.map(c => `
        <div class="col-md-3 bento-item">
            <div class="card bg-dark text-white border-secondary widget-card position-relative hud-tl hud-br rounded-3">
                <div class="card-body p-3">
                    <div class="d-flex align-items-center gap-3 mb-2">
                        <img src="${c.iconUrl}" width="32" height="32">
                        <h6 class="mb-0">${c.name}</h6>
                    </div>
                    <h4 class="crypto-price mb-1">$ ${parseFloat(c.price).toLocaleString()}</h4>
                    <span class="${parseFloat(c.change) >= 0 ? 'text-success' : 'text-danger'} small">
                        ${parseFloat(c.change) >= 0 ? '▲' : '▼'} %${c.change}
                    </span>
                </div>
            </div>
        </div>
    `).join('');

    // --- 2. Tablo ve Sayfalama ---
    let currentPage = 1;
    const rowsPerPage = 10;
    const totalPages = Math.ceil(allCoins.length / rowsPerPage);

    function renderTable(page) {
        const tbody = document.getElementById("coinTableBody");
        const start = (page - 1) * rowsPerPage;
        const currentData = allCoins.slice(start, start + rowsPerPage);

        tbody.innerHTML = currentData.map(c => `
            <tr class="bento-item">
                <td class="text-secondary">#${c.rank}</td>
                <td>
                    <div class="d-flex align-items-center gap-2">
                        <img src="${c.iconUrl}" width="24">
                        <span class="fw-bold">${c.name}</span>
                        <small class="text-muted">${c.symbol}</small>
                    </div>
                </td>
                <td class="crypto-price">$ ${parseFloat(c.price).toFixed(2)}</td>
                <td class="text-secondary small">$ ${parseFloat(c.marketCap).toLocaleString()}</td>
                <td class="${parseFloat(c.change) >= 0 ? 'text-success' : 'text-danger'}">
                    %${c.change}
                </td>
            </tr>
        `).join('');

        document.getElementById("coinPageIndicator").innerText = `${page} / ${totalPages}`;
        document.getElementById("coinTotalInfo").innerText = `Toplam ${allCoins.length} varlık içinden listeleniyor`;
        document.getElementById("coinPrevBtn").disabled = page === 1;
        document.getElementById("coinNextBtn").disabled = page === totalPages;
    }

    document.getElementById("coinPrevBtn").onclick = () => { if (currentPage > 1) renderTable(--currentPage); };
    document.getElementById("coinNextBtn").onclick = () => { if (currentPage < totalPages) renderTable(++currentPage); };

    renderTable(1);
});