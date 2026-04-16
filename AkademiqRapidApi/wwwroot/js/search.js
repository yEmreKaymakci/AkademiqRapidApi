document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("globalSearchInput");
    if (!searchInput) return;

    const routes = {
        "hava": "/Weather/Index",
        "durum": "/Weather/Index",
        "döviz": "/ConvertCurrency/Index",
        "para": "/ConvertCurrency/Index",
        "kripto": "/Coin/Index",
        "coin": "/Coin/Index",
        "bitcoin": "/Coin/Index",
        "yemek": "/Recipe/Index",
        "tarif": "/Recipe/Index",
        "futbol": "/Football/Index",
        "maç": "/Football/Index",
        "haber": "/News/Index",
        "haberler": "/News/Index",
        "film": "/Movie/Index",
        "sinema": "/Movie/Index",
        "müzik": "/Music/Index",
        "şarkı": "/Music/Index",
        "motivasyon": "/Motivation/Index",
        "söz": "/Motivation/Index",
        "akaryakıt": "/Gas/Index",
        "benzin": "/Gas/Index",
        "dashboard": "/Dashboard/Index",
        "sistem": "/Dashboard/Index",
        "nova": "/ChatBot/Index",
        "bot": "/ChatBot/Index",
        "asistan": "/ChatBot/Index",
        "arayüz": "/Dashboard/Index",
        "arşiv": "/Archive/Index",
        "geçmiş": "/Archive/Index",
        "analiz": "/Analysis/Index",
        "rapor": "/Analysis/Index"
    };

    searchInput.addEventListener("keypress", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            const query = searchInput.value.toLowerCase().trim();
            if (!query) return;

            for (const key in routes) {
                if (query.includes(key) || key.includes(query)) {
                    window.location.href = routes[key];
                    return;
                }
            }

            // Fallback: Eşleşme yoksa bildirim ver
            alert(`Sistemde "${query}" kelimesine uyan modül tespit edilemedi. Örn: "Futbol", "Hava", "Kripto" arayabilirsiniz.`);
        }
    });
});
