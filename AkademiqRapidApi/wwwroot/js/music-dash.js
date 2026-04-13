document.addEventListener("DOMContentLoaded", async function () {
    const titleEl = document.getElementById("dash-music-title");
    const artistEl = document.getElementById("dash-music-artist");
    const coverEl = document.getElementById("dash-music-cover");
    const playBtn = document.getElementById("dash-music-play-btn");
    const playIcon = document.getElementById("dash-music-play-icon");
    const progressFill = document.getElementById("dash-music-progress");
    const currTimeEl = document.getElementById("dash-music-curr");
    const totalTimeEl = document.getElementById("dash-music-total");

    let dashAudio = new Audio();
    let currentTrack = null;

    try {
        const response = await fetch('/Music/GetTopTracks');
        if (!response.ok) throw new Error("Müzik API hatası");
        const data = await response.json();

        if (data && data.length > 0) {
            currentTrack = data[0]; // Günün #1 Şarkısı
            
            titleEl.textContent = currentTrack.title;
            artistEl.textContent = currentTrack.artist?.name || "Bilinmeyen Sanatçı";
            if (currentTrack.album?.cover_medium) {
                coverEl.src = currentTrack.album.cover_medium;
            }
            
            dashAudio.src = currentTrack.preview;
            totalTimeEl.textContent = "0:30"; // Deezer preview is usually 30s
        }
    } catch (e) {
        console.error("Dashboard Deezer Fetch Hatası: ", e);
        titleEl.textContent = "Bağlantı Hatası";
    }

    function formatTime(seconds) {
        if (!seconds) return "0:00";
        const m = Math.floor(seconds / 60);
        const s = Math.floor(seconds % 60);
        return `${m}:${s < 10 ? '0' : ''}${s}`;
    }

    if (playBtn) {
        playBtn.addEventListener("click", () => {
            if (!dashAudio.src) return;
            
            if (dashAudio.paused) {
                dashAudio.play();
                playIcon.textContent = "pause";
            } else {
                dashAudio.pause();
                playIcon.textContent = "play_arrow";
            }
        });
    }

    dashAudio.addEventListener('timeupdate', () => {
        if (dashAudio.duration) {
            const perc = (dashAudio.currentTime / dashAudio.duration) * 100;
            progressFill.style.width = `${perc}%`;
            currTimeEl.textContent = formatTime(dashAudio.currentTime);
        }
    });

    dashAudio.addEventListener('ended', () => {
        playIcon.textContent = "play_arrow";
        progressFill.style.width = "0%";
        currTimeEl.textContent = "0:00";
    });
});
