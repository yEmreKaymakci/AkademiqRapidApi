document.addEventListener("DOMContentLoaded", function () {
    fetchNews();
    setInterval(fetchNews, 300000); // 5 dakikada bir güncelle
});

function fetchNews() {
    const wrapper = document.getElementById("news-wrapper");
    if (!wrapper) return;

    fetch('/News/GetNewsByCategory?category=general')
        .then(response => response.json())
        .then(payload => {
            // API yapısına göre payload veya payload.articles gibi olabilir
            const articles = payload.articles || payload.data || payload; 
            
            if (articles && articles.length > 0) {
                // İlk 4 haberi al
                const topNews = articles.slice(0, 4);
                let html = "";
                
                const colors = ["text-purple-400", "text-cyan-400", "text-green-400", "text-secondary-container"];

                topNews.forEach((news, idx) => {
                    const title = news.title || news.name || "Haber Başlığı";
                    const source = news.source?.name || news.provider?.[0]?.name || "Haber Kaynağı";
                    const publishedAt = news.publishedAt || news.datePublished || new Date().toISOString();
                    const color = colors[idx % colors.length];

                    // Basit saat hesabı
                    const dateObj = new Date(publishedAt);
                    const now = new Date();
                    const diffMs = now - dateObj;
                    const diffHrs = Math.floor(diffMs / (1000 * 60 * 60));
                    const timeStr = diffHrs > 0 ? `${diffHrs} SAAT ÖNCE` : "YENİ";

                    html += `
                    <div class="p-5 hover:bg-surface-bright/20 transition-colors cursor-pointer group" onclick="window.open('${news.url || '#'}', '_blank')">
                        <span class="font-headline text-[8px] ${color} uppercase tracking-wider block mb-1.5">${source.toUpperCase()}</span>
                        <h4 class="text-xs font-semibold leading-snug group-hover:text-cyan-400 transition-colors mb-2 line-clamp-2">
                            ${title}
                        </h4>
                        <div class="flex gap-3 text-[8px] text-outline uppercase tracking-tight">
                            <span>${timeStr}</span>
                        </div>
                    </div>
                    `;
                });

                wrapper.innerHTML = html;
            }
        })
        .catch(err => {
            console.error("Haberler yüklenirken hata oluştu:", err);
            wrapper.innerHTML = '<div class="p-5 text-xs text-outline">Haberler yüklenemedi.</div>';
        });
}
