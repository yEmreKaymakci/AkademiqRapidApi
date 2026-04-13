document.addEventListener("DOMContentLoaded", function () {
    const dataEl = document.getElementById("musicDataJson");
    const container = document.getElementById("musicTrackList");
    const audioPlayer = new Audio();
    const globalPlayer = document.getElementById("musicGlobalPlayer");

    // Player Elements
    const gpCover = document.getElementById("gpCover");
    const gpTitle = document.getElementById("gpTitle");
    const gpArtist = document.getElementById("gpArtist");
    const gpPlayBtn = document.getElementById("gpPlayBtn");
    const gpPlayIcon = document.getElementById("gpPlayIcon");
    const gpCurrTime = document.getElementById("gpCurrTime");
    const gpTotalTime = document.getElementById("gpTotalTime");
    const gpFill = document.getElementById("gpFill");

    let allTracks = [];
    let currentTrackId = null;

    try {
        if (dataEl && dataEl.textContent.trim() !== 'null') {
            allTracks = JSON.parse(dataEl.textContent) || [];
        }
    } catch (e) {
        allTracks = [];
    }

    function formatTime(seconds) {
        if (!seconds) return "0:00";
        const m = Math.floor(seconds / 60);
        const s = Math.floor(seconds % 60);
        return `${m}:${s < 10 ? '0' : ''}${s}`;
    }

    function renderTracks(data) {
        if (!data || data.length === 0) {
            container.innerHTML = `<div class="text-center text-muted py-5">Şarkı bulunamadı.</div>`;
            return;
        }

        let html = '';
        data.forEach((t, idx) => {
            const cover = t.album?.cover_medium || 'https://placehold.co/100x100/282828/fff?text=No+Cover';
            const artistName = t.artist?.name || 'Bilinmeyen Sanatçı';
            const albumName = t.album?.title || 'Bilinmeyen Albüm';
            const isActive = currentTrackId === t.id;
            
            html += `
                <div class="music-track-card ${isActive ? 'playing' : ''}" data-id="${t.id}" data-preview="${t.preview}" data-title="${t.title}" data-artist="${artistName}" data-cover="${cover}">
                    <div class="m-number">${idx + 1}</div>
                    <div class="m-cover-wrapper">
                        <img src="${cover}" class="m-cover" alt="Kapak">
                        <div class="m-play-overlay" onclick="togglePlay(${t.id})">
                            <span class="material-symbols-outlined" style="font-variation-settings: 'FILL' 1;">
                                ${isActive && !audioPlayer.paused ? 'pause' : 'play_arrow'}
                            </span>
                        </div>
                    </div>
                    <div class="m-info">
                        <div class="m-title" title="${t.title}">${t.title}</div>
                        <div class="m-artist" title="${artistName}">${artistName}</div>
                    </div>
                    <div class="m-album-name" title="${albumName}">${albumName}</div>
                    <div class="m-duration">${formatTime(t.duration)}</div>
                </div>
            `;
        });
        container.innerHTML = html;
        attachPlayEvents();
    }

    function attachPlayEvents() {
        document.querySelectorAll('.music-track-card').forEach(card => {
            card.addEventListener('dblclick', () => {
                togglePlay(parseInt(card.dataset.id));
            });
        });
    }

    window.togglePlay = function(id) {
        const track = allTracks.find(t => t.id === id);
        if (!track || !track.preview) {
            alert("Bu şarkı için dinleme URL'si bulunamadı.");
            return;
        }

        // Eğer aynı şarkıya basıldıysa Duraklat/Devam Et
        if (currentTrackId === id) {
            if (audioPlayer.paused) {
                audioPlayer.play();
                gpPlayIcon.textContent = "pause";
            } else {
                audioPlayer.pause();
                gpPlayIcon.textContent = "play_arrow";
            }
            updateGridUI();
            return;
        }

        // Yeni şarkı başlat
        currentTrackId = id;
        audioPlayer.src = track.preview;
        audioPlayer.play();
        
        // UI Güncelle
        globalPlayer.classList.add("active");
        gpCover.src = track.album?.cover_medium || '';
        gpTitle.textContent = track.title;
        gpArtist.textContent = track.artist?.name;
        gpPlayIcon.textContent = "pause";
        
        updateGridUI();
    };

    function updateGridUI() {
        document.querySelectorAll('.music-track-card').forEach(card => {
            const id = parseInt(card.dataset.id);
            const icon = card.querySelector('.m-play-overlay span');
            if (id === currentTrackId) {
                card.classList.add('playing');
                icon.textContent = audioPlayer.paused ? 'play_arrow' : 'pause';
            } else {
                card.classList.remove('playing');
                icon.textContent = 'play_arrow';
            }
        });
    }

    // Global Player Play/Pause Btn
    gpPlayBtn.addEventListener('click', () => {
        if (!currentTrackId) return;
        if (audioPlayer.paused) {
            audioPlayer.play();
            gpPlayIcon.textContent = "pause";
        } else {
            audioPlayer.pause();
            gpPlayIcon.textContent = "play_arrow";
        }
        updateGridUI();
    });

    // Audio Events
    audioPlayer.addEventListener('timeupdate', () => {
        if (audioPlayer.duration) {
            const perc = (audioPlayer.currentTime / audioPlayer.duration) * 100;
            gpFill.style.width = `${perc}%`;
            gpCurrTime.textContent = formatTime(audioPlayer.currentTime);
            gpTotalTime.textContent = formatTime(audioPlayer.duration);
        }
    });

    audioPlayer.addEventListener('ended', () => {
        gpPlayIcon.textContent = "play_arrow";
        updateGridUI();
        gpFill.style.width = "0%";
        gpCurrTime.textContent = "0:00";
    });

    // İlk çalışma
    renderTracks(allTracks);
});
