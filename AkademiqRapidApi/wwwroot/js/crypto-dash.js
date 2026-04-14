document.addEventListener("DOMContentLoaded", function () {
    fetchCryptoData();
    setInterval(fetchCryptoData, 120000); // Her 2 dakikada bir güncelle
});

function fetchCryptoData() {
    fetch('/Coin/GetDashboardCoins')
        .then(response => response.json())
        .then(payload => {
            if (payload && payload.data && payload.data.length > 0) {
                const listContainer = document.getElementById("crypto-list");
                if (!listContainer) return;

                // Sadece ilk 4 coini alıyoruz
                const coins = payload.data.slice(0, 4);

                let htmlContent = "";

                coins.forEach(coin => {
                    const symbol = coin.symbol.toUpperCase();
                    const name = coin.name;
                    const price = parseFloat(coin.price_usd);
                    const changeStr = coin.percent_change_24h;
                    const change = parseFloat(changeStr);

                    // Değişim yönü
                    const isUp = change >= 0;
                    const borderHover = isUp ? "hover:border-green-400/20" : "hover:border-red-400/20";
                    const priceColor = isUp ? "text-green-400" : "text-error";
                    const changeSign = isUp ? "+" : "";

                    // Renk eşleştirmeleri
                    let iconBg, iconBorder, iconText;
                    if (symbol === "BTC") { iconBg = "bg-yellow-500/10"; iconBorder = "border-yellow-500/25"; iconText = "text-yellow-400"; }
                    else if (symbol === "ETH") { iconBg = "bg-indigo-500/10"; iconBorder = "border-indigo-500/25"; iconText = "text-indigo-400"; }
                    else if (symbol === "SOL") { iconBg = "bg-purple-500/10"; iconBorder = "border-purple-500/25"; iconText = "text-purple-400"; }
                    else if (symbol === "XRP") { iconBg = "bg-red-500/10"; iconBorder = "border-red-500/25"; iconText = "text-red-400"; }
                    else { iconBg = "bg-cyan-500/10"; iconBorder = "border-cyan-500/25"; iconText = "text-cyan-400"; }

                    // Formatlama
                    let formattedPrice = price.toLocaleString('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2, maximumFractionDigits: 4 });

                    htmlContent += `
                        <div class="flex items-center justify-between p-3 rounded bg-surface-container border border-outline-variant/10 ${borderHover} transition-colors">
                            <div class="crypto-icon ${iconBg} border ${iconBorder} ${iconText}">
                                ${symbol}
                            </div>
                            <div class="flex-1 px-3">
                                <div class="font-headline text-xs font-bold truncate max-w-[100px]" title="${name}">${name}</div>
                                <div class="text-[9px] text-outline">${symbol}</div>
                            </div>
                            <div class="text-right">
                                <div class="font-headline text-sm font-bold ${priceColor}">${formattedPrice}</div>
                                <div class="text-[9px] ${priceColor}">${changeSign}${changeStr}%</div>
                            </div>
                        </div>
                    `;
                });

                // Toplam Hacim (Statik veya array üzerinden toplanabilir)
                htmlContent += `
                    <div class="mt-auto">
                        <div class="h-[3px] bg-outline-variant/20 rounded-full overflow-hidden">
                            <div class="h-full w-[72%] progress-fill rounded-full"></div>
                        </div>
                        <div class="flex justify-between mt-2">
                            <span class="text-[9px] text-outline uppercase tracking-wider">Hacim Durumu</span>
                            <span class="text-[9px] text-green-400 font-bold">Aktif</span>
                        </div>
                    </div>
                `;

                listContainer.innerHTML = htmlContent;
            }
        })
        .catch(err => console.error("Kripto verileri yüklenirken hata:", err));
}
