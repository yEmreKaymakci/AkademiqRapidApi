document.addEventListener("DOMContentLoaded", function () {
    const dataEl = document.getElementById("movieDataJson");
    const container = document.getElementById("movieMainContainer");
    const loading = document.getElementById("movieLoading");

    let allMovies = [];

    try {
        if (dataEl && dataEl.textContent.trim() !== 'null') {
            allMovies = JSON.parse(dataEl.textContent) || [];
        }
    } catch (e) {
        allMovies = [];
    }

    // JSON Normalize (Pascal -> Camel)
    function norm(m) {
        if (!m) return {};
        // TMDB Data path: https://image.tmdb.org/t/p/original  -- veya w500
        const baseUrl = "https://image.tmdb.org/t/p/";
        const posterObj = m.Poster_path || m.poster_path;
        const backdropObj = m.Backdrop_path || m.backdrop_path;
        
        return {
            id: m.Id || m.id,
            title: m.Title || m.title || m.Original_title || m.original_title || "Bilinmiyor",
            overview: m.Overview || m.overview || "Açıklama bulunmuyor...",
            poster: posterObj ? `${baseUrl}w500${posterObj}` : "/img/movie-placeholder.png",
            backdrop: backdropObj ? `${baseUrl}original${backdropObj}` : "/img/hero-placeholder.jpg",
            release: m.Release_date || m.release_date || "",
            vote: m.Vote_average || m.vote_average || 0,
            adult: m.Adult || m.adult || false
        };
    }

    function renderMovies(data) {
        if (!data || data.length === 0) {
            container.innerHTML = `<div class="alert alert-dark text-center">Gösterilecek film bulunamadı.</div>`;
            return;
        }

        // Hero: İlk film
        const hero = norm(data[0]);
        let html = `
            <div class="movie-hero-section animate-in" style="--animate-delay: 0.1s">
                <img src="${hero.backdrop}" class="movie-hero-backdrop" alt="${hero.title}" onerror="this.src='https://placehold.co/1200x600/18181b/a1a1aa?text=Backdrop'">
                <div class="movie-hero-overlay"></div>
                <div class="movie-hero-content">
                    <div class="movie-hero-info">
                        <span class="movie-hero-rating">
                            <span class="material-symbols-outlined" style="font-size:1.1rem">star</span>
                            ${hero.vote.toFixed(1)}
                        </span>
                        <span>${hero.release.substring(0,4)}</span>
                        ${hero.adult ? '<span class="badge bg-danger">18+</span>' : ''}
                    </div>
                    <h1 class="movie-hero-title">${hero.title}</h1>
                    <p class="movie-hero-desc">${hero.overview}</p>
                    <button class="btn btn-light rounded-pill px-4 py-2 fw-bold d-flex align-items-center gap-2">
                        <span class="material-symbols-outlined">play_arrow</span> Keşfet
                    </button>
                </div>
            </div>
        `;

        // Tüm Filmler Listesi
        html += `
            <div class="movie-carousel-header animate-in" style="--animate-delay: 0.2s">
                <h3 class="movie-carousel-title text-white m-0">Kütüphane Gösterimi</h3>
            </div>
            <div class="movie-scroll-wrapper animate-in" style="--animate-delay: 0.3s">
                <div class="carousel-nav-btn prev" onclick="scrollMovieTrack('left')"><span class="material-symbols-outlined">chevron_left</span></div>
                <div class="movie-track" id="mainMovieTrack">
        `;

        data.forEach(m => {
            const mv = norm(m);
            html += `
                <div class="movie-card">
                    <img src="${mv.poster}" class="movie-card-img" alt="${mv.title}" onerror="this.src='https://placehold.co/500x750/18181b/a1a1aa?text=Poster'">
                    <div class="movie-detail-overlay">
                        <div class="m-title" title="${mv.title}">${mv.title}</div>
                        <div class="m-rating">
                            <span class="material-symbols-outlined" style="font-size: 1rem">star</span>
                            <span>${mv.vote.toFixed(1)}</span>
                        </div>
                    </div>
                </div>
            `;
        });

        html += `
                </div>
                <div class="carousel-nav-btn next" onclick="scrollMovieTrack('right')"><span class="material-symbols-outlined">chevron_right</span></div>
            </div>
        `;

        container.innerHTML = html;
    }

    // Scroll Kontrolü (Track yatay kaydırma)
    window.scrollMovieTrack = function (dir) {
        const track = document.getElementById("mainMovieTrack");
        if (!track) return;
        const scrollAmount = window.innerWidth < 768 ? 320 : 880; // Mobilde az, masüstünde 4 kart kaydır
        track.scrollBy({ left: dir === 'left' ? -scrollAmount : scrollAmount, behavior: 'smooth' });
    };

    window.filterMovieCategory = async function (category, btn) {
        document.querySelectorAll('.btn-category').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');

        loading.style.display = 'flex';
        container.style.display = 'none';

        try {
            const response = await fetch(`/Movie/GetMoviesByCategory?category=${category}`);
            if (!response.ok) throw new Error("API hatası");
            allMovies = await response.json();
        } catch (error) {
            console.error(error);
            allMovies = [];
        } finally {
            loading.style.display = 'none';
            container.style.display = 'block';
            renderMovies(allMovies);
        }
    };

    loading.style.display = 'none';
    renderMovies(allMovies);
});
