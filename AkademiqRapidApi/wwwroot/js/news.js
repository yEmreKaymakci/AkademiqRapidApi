/**
 * news.js — Haber Merkezi Logiği
 * API verilerinin normalize edilmesi (camelCase vs PascalCase)
 * ve UI etkileşimlerini yönetir. (Carousel versiyonu)
 */

document.addEventListener("DOMContentLoaded", function () {
    const dataEl = document.getElementById("newsDataJson");
    const heroArea = document.getElementById("heroNewsArea");
    const gridArea = document.getElementById("newsGridArea");
    const carouselLayout = document.getElementById("newsCarouselLayout");
    const loadingState = document.getElementById("newsLoading");

    let allArticles = [];

    // Sayfa yüklenince server-side çekilen JSON verisini al
    try {
        if (dataEl && dataEl.textContent.trim() !== 'null') {
            allArticles = JSON.parse(dataEl.textContent) || [];
        }
    } catch (e) {
        allArticles = [];
    }

    // ── Gelen Object'i Normalize Et ──
    function norm(a) {
        if (!a) return {};
        const source = a.Source || a.source || {};
        return {
            title: a.Title || a.title || "İsimsiz Haber",
            desc: a.Description || a.description || "",
            url: a.Url || a.url || "#",
            img: a.UrlToImage || a.urlToImage || "/img/news-placeholder.jpg",
            publishedAt: a.PublishedAt || a.publishedAt || null,
            sourceName: source.Name || source.name || "Bilinmeyen Kaynak"
        };
    }

    const formatDate = (dateStr) => {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        return d.toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' });
    };

    function showLoading() {
        loadingState.style.display = 'flex';
        heroArea.style.display = 'none';
        carouselLayout.style.display = 'none';
    }

    function hideLoading() {
        loadingState.style.display = 'none';
        heroArea.style.display = 'block';
        carouselLayout.style.display = 'block';
    }

    function renderNews(data) {
        if (!data || data.length === 0) {
            heroArea.innerHTML = `<div class="col-12"><div class="news-header-box text-center justify-content-center flex-column"><span class="fs-1 mb-2">📰</span><h4 class="text-white">Şu an gösterilecek haber bulunamadı.</h4><p class="text-secondary">Lütfen farklı bir kategori seçin veya daha sonra tekrar deneyin.</p></div></div>`;
            carouselLayout.style.display = 'none';
            document.getElementById("newsFooterNav").style.display = 'none';
            return;
        }

        document.getElementById("newsFooterNav").style.display = 'flex';
        carouselLayout.style.display = 'block';

        // 1. Hero Haber (İlk sıradaki)
        const heroObj = norm(data[0]);
        heroArea.innerHTML = `
            <div class="col-12 animate-in" style="--animate-delay: 0.1s">
                <a href="${heroObj.url}" target="_blank" class="news-card news-hero-card">
                    <div class="news-img-box">
                        <img src="${heroObj.img}" class="news-img" alt="Hero" onerror="this.src='https://placehold.co/800x400/18181b/a1a1aa?text=Görsel+Yok'">
                        <div class="news-img-overlay"></div>
                    </div>
                    <div class="news-content">
                        <span class="news-source"><span class="material-symbols-outlined me-1" style="font-size: 1.1rem">bolt</span> MANŞET | ${heroObj.sourceName}</span>
                        <h2 class="news-title">${heroObj.title}</h2>
                        <p class="news-desc">${heroObj.desc}</p>
                        <div class="news-meta">
                            <span class="material-symbols-outlined" style="font-size: 1.1rem">schedule</span> ${formatDate(heroObj.publishedAt)}
                        </div>
                    </div>
                </a>
            </div>
        `;

        // 2. Grid (Carousel İçin Kalan Haberler)
        const gridData = data.slice(1);
        
        gridArea.innerHTML = gridData.map((a, idx) => {
            const n = norm(a);
            const delay = 0.2 + (idx * 0.05); // Hafif staggered animasyon
            return `
            <div class="news-card-wrapper animate-in" style="--animate-delay: ${delay}s">
                <a href="${n.url}" target="_blank" class="news-card">
                    <div class="news-img-box">
                        <img src="${n.img}" class="news-img" alt="Haber" onerror="this.src='https://placehold.co/600x300/18181b/a1a1aa?text=Görsel+Yok'">
                        <div class="news-img-overlay"></div>
                    </div>
                    <div class="news-content">
                        <span class="news-source">${n.sourceName}</span>
                        <h5 class="news-title" title="${n.title}">${n.title}</h5>
                        <p class="news-desc">${n.desc}</p>
                        <div class="news-meta">
                            <span class="material-symbols-outlined" style="font-size: 1.1rem">schedule</span> ${formatDate(n.publishedAt)}
                        </div>
                    </div>
                </a>
            </div>
            `;
        }).join('');

        document.getElementById("newsTotalInfo").innerHTML = `<b>${data.length}</b> sonuç bulundu`;
        
        // Crousell'i başa sar
        gridArea.scrollLeft = 0;
        updateCarouselButtons();
    }

    // ── Kategori Filtreleme (AJAX) ──────────────────────────────
    window.filterCategory = async function (category, btn) {
        document.querySelectorAll('.btn-category').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');

        showLoading();
        try {
            const response = await fetch(`/News/GetNewsByCategory?category=${category}`);
            if (!response.ok) throw new Error("API hatası");
            allArticles = await response.json();
            
        } catch (error) {
            console.error("News fetch error:", error);
            allArticles = [];
        } finally {
            hideLoading();
            renderNews(allArticles);
        }
    };

    // ── Carousel Scroll Mantığı ─────────────────────────────────────
    
    // Her kaydırmada yaklaşık 3 veya 4 kart kaydır (960px)
    const scrollAmount = 960; 

    document.getElementById("newsPrevBtn").addEventListener("click", () => {
        gridArea.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
    });

    document.getElementById("newsNextBtn").addEventListener("click", () => {
        gridArea.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    });

    function updateCarouselButtons() {
        const canScrollLeft = gridArea.scrollLeft > 0;
        const canScrollRight = gridArea.scrollLeft + gridArea.clientWidth < gridArea.scrollWidth - 5;
        
        document.getElementById("newsPrevBtn").disabled = !canScrollLeft;
        document.getElementById("newsNextBtn").disabled = !canScrollRight;
        
        // Sayfa/Durum Indicator
        if (!canScrollLeft) {
            document.getElementById("newsPageIndicator").innerText = "BAŞLANGIÇ";
        } else if (!canScrollRight) {
            document.getElementById("newsPageIndicator").innerText = "SON";
        } else {
            document.getElementById("newsPageIndicator").innerText = "DAHA FAZLA";
        }
    }

    // Scroll durduğunda butonları güncelle
    let scrollTimeout;
    gridArea.addEventListener('scroll', () => {
        clearTimeout(scrollTimeout);
        scrollTimeout = setTimeout(updateCarouselButtons, 150);
    });

    // ── İlk Çalıştırma ───────────────────────────────────────────
    hideLoading();
    renderNews(allArticles);
});