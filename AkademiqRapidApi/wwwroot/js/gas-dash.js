document.addEventListener("DOMContentLoaded", function () {
    fetchGasPrices();
});

function fetchGasPrices() {
    fetch('/Gas/GetDashboardGas')
        .then(response => response.json())
        .then(payload => {
            if (payload && payload.length > 0) {
                const wrapper = document.getElementById("gas-wrapper");
                if (!wrapper) return;

                // Türkiye'yi arayalım, yoksa ilk elemanı alalım
                let trData = payload.find(x => x.country.toLowerCase().includes("turkey"));
                if (!trData) {
                    trData = payload[0];
                }

                const curr = trData.currency || "TL";
                const diesel = parseFloat(trData.diesel) || 0;
                const gasoline = parseFloat(trData.gasoline) || 0;
                const lpg = parseFloat(trData.lpg) || 0;

                // Basit mock değişim yüzdeleri
                const dChange = "+0.12%"; const dColor = "text-green-400"; const dBg = "bg-green-400/10 border-green-400/20";
                const gChange = "-0.05%"; const gColor = "text-error"; const gBg = "bg-error/10 border-error/20";
                const lChange = "+0.44%"; const lColor = "text-green-400"; const lBg = "bg-green-400/10 border-green-400/20";

                let htmlContent = `
                    <div class="flex justify-between items-center p-2.5 rounded bg-surface-container border border-outline-variant/10 hover:border-cyan-500/20 transition-colors">
                        <span class="text-[11px] text-on-surface-variant">Motorin</span>
                        <div class="flex items-center gap-2">
                            <span class="font-headline text-sm font-bold text-primary-container">${diesel.toFixed(2)} ${curr}</span>
                            <span class="text-[8px] font-bold ${dColor} ${dBg} px-1.5 py-0.5 rounded-full">${dChange}</span>
                        </div>
                    </div>
                    <div class="flex justify-between items-center p-2.5 rounded bg-surface-container border border-outline-variant/10 hover:border-cyan-500/20 transition-colors">
                        <span class="text-[11px] text-on-surface-variant">Benzin</span>
                        <div class="flex items-center gap-2">
                            <span class="font-headline text-sm font-bold text-primary-container">${gasoline.toFixed(2)} ${curr}</span>
                            <span class="text-[8px] font-bold ${gColor} ${gBg} px-1.5 py-0.5 rounded-full">${gChange}</span>
                        </div>
                    </div>
                    <div class="flex justify-between items-center p-2.5 rounded bg-surface-container border border-outline-variant/10 hover:border-cyan-500/20 transition-colors">
                        <span class="text-[11px] text-on-surface-variant">LPG</span>
                        <div class="flex items-center gap-2">
                            <span class="font-headline text-sm font-bold text-primary-container">${lpg.toFixed(2)} ${curr}</span>
                            <span class="text-[8px] font-bold ${lColor} ${lBg} px-1.5 py-0.5 rounded-full">${lChange}</span>
                        </div>
                    </div>
                `;

                wrapper.innerHTML = htmlContent;
            }
        })
        .catch(err => console.error("Akaryakıt verileri yüklenirken hata:", err));
}
