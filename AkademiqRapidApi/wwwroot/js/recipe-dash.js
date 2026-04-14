document.addEventListener("DOMContentLoaded", function () {
    fetchDailyRecipe();
});

function fetchDailyRecipe() {
    fetch('/Recipe/GetDailyRecipe')
        .then(response => response.json())
        .then(payload => {
            if (payload && payload.length > 0) {
                // Rastgele bir tarif seçelim veya ilkini alalım
                const randomIndex = Math.floor(Math.random() * payload.length);
                const recipe = payload[randomIndex];

                const titleEl = document.getElementById("dash-recipe-title");
                const descEl = document.getElementById("dash-recipe-desc");
                const imgEl = document.getElementById("dash-recipe-img");
                const timeEl = document.getElementById("dash-recipe-time");
                const portionEl = document.getElementById("dash-recipe-portion");
                const diffEl = document.getElementById("dash-recipe-diff");

                if (titleEl) titleEl.innerText = recipe.name || "Lezzetli Tarif";
                if (descEl) descEl.innerText = recipe.description || "Günün harika tarifi için hemen tıklayın ve detayları keşfedin.";
                if (imgEl && recipe.thumbnail_url) {
                    imgEl.src = recipe.thumbnail_url;
                }
                
                if (timeEl) {
                    timeEl.innerText = recipe.total_time_minutes ? `${recipe.total_time_minutes} dk` : "30 dk";
                }
                if (portionEl) {
                    portionEl.innerText = recipe.yields ? recipe.yields : "4 kişi";
                }
                if (diffEl) {
                    diffEl.innerText = "Orta"; // API'de zorluk derecesi yok, statik bırakıyoruz
                }
            }
        })
        .catch(err => console.error("Tarif verileri yüklenirken hata:", err));
}
