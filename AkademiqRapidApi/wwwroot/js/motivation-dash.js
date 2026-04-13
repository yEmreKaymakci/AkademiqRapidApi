document.addEventListener("DOMContentLoaded", async function () {
    const quoteEl = document.getElementById("dash-quote-body");
    const authorEl = document.getElementById("dash-quote-author");

    try {
        const response = await fetch('/Motivation/GetDashboardQuote');
        if (!response.ok) throw new Error("API hatası");
        const quoteObj = await response.json();

        if (quoteObj) {
            // Animasyonlu çıkış için önce opacity 0
            quoteEl.style.opacity = "0";
            authorEl.style.opacity = "0";

            setTimeout(() => {
                // Çevirisi varsa onu kullan, yoksa orjinali kullan
                quoteEl.textContent = `"${quoteObj.translatedBody || quoteObj.body}"`;
                authorEl.textContent = `— ${quoteObj.author || "Anonim"}`;
                
                quoteEl.style.transition = "opacity 0.8s ease";
                authorEl.style.transition = "opacity 1s ease";
                quoteEl.style.opacity = "1";
                authorEl.style.opacity = "1";
            }, 300);
        }
    } catch (e) {
        console.error("Dashboard Quote Fetch Hatası: ", e);
        if(quoteEl) quoteEl.textContent = "Derin düşünceler yüklenirken bir hata oluştu...";
    }
});
