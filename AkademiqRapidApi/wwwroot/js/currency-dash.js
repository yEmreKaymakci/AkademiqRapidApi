document.addEventListener("DOMContentLoaded", function () {
    fetchCurrencyRates();
    // Opsiyonel olarak 1 dakikada bir güncelle
    setInterval(fetchCurrencyRates, 60000);
});

function fetchCurrencyRates() {
    fetch('/ConvertCurrency/GetDashboardRates')
        .then(response => response.json())
        .then(data => {
            if (data && data.rates) {
                const tryRate = data.rates.try;
                const usdRate = data.rates.usd;
                const gbpRate = data.rates.gbp;

                // EUR -> TRY (Base is likely EUR)
                const eurToTry = tryRate;
                // USD -> TRY
                const usdToTry = tryRate / usdRate;
                // GBP -> TRY
                const gbpToTry = tryRate / gbpRate;

                // DOM öğelerini güncelle
                const usdEl = document.getElementById("usd-val");
                const eurEl = document.getElementById("eur-val");
                const gbpEl = document.getElementById("gbp-val");

                if (usdEl) usdEl.innerText = usdToTry.toFixed(2);
                if (eurEl) eurEl.innerText = eurToTry.toFixed(2);
                if (gbpEl) gbpEl.innerText = gbpToTry.toFixed(2);
            }
        })
        .catch(err => console.error("Döviz kurları yüklenirken hata oluştu:", err));
}
