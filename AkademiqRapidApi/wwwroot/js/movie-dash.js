document.addEventListener("DOMContentLoaded", async function () {
    const titleEl = document.getElementById("dash-movie-title");
    const descEl = document.getElementById("dash-movie-desc");
    const ratingEl = document.getElementById("dash-movie-rating");
    const dateEl = document.getElementById("dash-movie-date");
    const posterEl = document.getElementById("dash-movie-poster");
    const backdropEl = document.getElementById("dash-movie-backdrop");

    try {
        const response = await fetch('/Movie/GetMoviesByCategory?category=trending');
        if (!response.ok) throw new Error("API hatası");
        const data = await response.json();

        if (data && data.length > 0) {
            // İlk filmi (Günün trendi 1 numara) widget'a bas
            const movie = data[0];
            const baseUrl = "https://image.tmdb.org/t/p/";
            
            // PascalCase vs camelCase güvencesi
            const title = movie.Title || movie.title || movie.Original_title || movie.original_title || "Bilinmeyen Film";
            const overview = movie.Overview || movie.overview || "-";
            const vote = (movie.Vote_average || movie.vote_average || 0).toFixed(1);
            const rDate = movie.Release_date || movie.release_date || "";
            const year = rDate.split('-')[0];
            const posterPath = movie.Poster_path || movie.poster_path;
            const bgPath = movie.Backdrop_path || movie.backdrop_path;

            titleEl.textContent = title;
            descEl.textContent = overview;
            ratingEl.textContent = `${vote}`;
            dateEl.textContent = year;
            
            if (posterPath) posterEl.src = `${baseUrl}w500${posterPath}`;
            if (bgPath) backdropEl.src = `${baseUrl}original${bgPath}`;
        } else {
            throw new Error("Boş veri dizisi");
        }
    } catch (e) {
        console.error("Dashboard TMDB Fetch Hatası: ", e);
        titleEl.textContent = "Bağlantı Hatası";
        descEl.textContent = "Filmler alınamadı. İnternet bağlantınızı kontrol edin.";
    }
});
