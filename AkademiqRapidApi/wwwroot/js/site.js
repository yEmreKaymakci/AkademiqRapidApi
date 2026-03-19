// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/* ─── DASHBOARD SCRIPTS ──────────────────────────────────────────────────── */

(function () {
    'use strict';

    /* ── Helpers ── */
    function pad(n) {
        return String(n).padStart(2, '0');
    }

    function $(id) {
        return document.getElementById(id);
    }

    /* ════════════════════════════════════════
       CLOCK  —  hero widget + navbar clock
    ════════════════════════════════════════ */
    const DAYS = ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'];
    const MONTHS = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
        'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];

    const startAt = Date.now();

    function tickClock() {
        const now = new Date();
        const h = pad(now.getHours());
        const m = pad(now.getMinutes());
        const s = pad(now.getSeconds());
        const ts = `${h}:${m}:${s}`;

        const heroClock = $('hero-clock');
        const pageClock = $('page-clock');
        if (heroClock) heroClock.textContent = ts;
        if (pageClock) pageClock.textContent = ts;

        const heroDate = $('hero-date');
        if (heroDate) {
            heroDate.textContent =
                `${DAYS[now.getDay()]}, ${now.getDate()} ${MONTHS[now.getMonth()]} ${now.getFullYear()}`;
        }

        const uptimeEl = $('uptime');
        if (uptimeEl) {
            const elapsed = Math.floor((Date.now() - startAt) / 1000);
            uptimeEl.textContent =
                `${pad(Math.floor(elapsed / 3600))}:${pad(Math.floor((elapsed % 3600) / 60))}:${pad(elapsed % 60)}`;
        }
    }

    setInterval(tickClock, 1000);
    tickClock();


    /* ════════════════════════════════════════
       MUSIC PLAYER  —  viz bars + progress
    ════════════════════════════════════════ */
    const vizContainer = $('music-viz');
    let isPlaying = true;
    let vizBars = [];

    if (vizContainer) {
        vizBars = Array.from({ length: 16 }, () => {
            const bar = document.createElement('div');
            bar.className = 'viz-bar';
            const h = Math.random() * 36 + 6;
            bar.style.cssText =
                `--h:${h}px;` +
                `height:${h}px;` +
                `animation-delay:${(Math.random() * .8).toFixed(2)}s;` +
                `animation-duration:${(.55 + Math.random() * .55).toFixed(2)}s`;
            vizContainer.appendChild(bar);
            return bar;
        });
    }

    /* Play / Pause toggle */
    const playBtn = $('play-btn');
    const playIcon = $('play-icon');

    if (playBtn) {
        playBtn.addEventListener('click', function () {
            isPlaying = !isPlaying;
            if (playIcon) playIcon.textContent = isPlaying ? 'pause' : 'play_arrow';
            vizBars.forEach(b => {
                b.style.animationPlayState = isPlaying ? 'running' : 'paused';
            });
        });
    }

    /* Music progress ticker */
    let musicSecs = 102;
    const musicTotal = 235;

    setInterval(function () {
        if (!isPlaying) return;

        musicSecs = (musicSecs + 1) % musicTotal;
        const pct = (musicSecs / musicTotal) * 100;

        const progressBar = $('music-progress');
        const currentTime = $('music-curr');

        if (progressBar) progressBar.style.width = pct + '%';
        if (currentTime) currentTime.textContent =
            `${Math.floor(musicSecs / 60)}:${pad(musicSecs % 60)}`;

        /* Randomise bar heights while playing */
        vizBars.forEach(b => {
            const h = Math.random() * 36 + 6;
            b.style.setProperty('--h', h + 'px');
        });
    }, 1000);


    /* ════════════════════════════════════════
       CURRENCY TICKER  —  live rate wobble
    ════════════════════════════════════════ */
    const rates = {
        'usd-val': 38.42,
        'eur-val': 41.76,
        'gbp-val': 49.11,
    };

    setInterval(function () {
        Object.keys(rates).forEach(function (id) {
            const delta = (Math.random() - .5) * .06;
            rates[id] += delta;

            const el = $(id);
            if (!el) return;

            el.textContent = rates[id].toFixed(2);
            el.style.color = delta > 0 ? '#22c55e' : '#f43f5e';
            el.style.transition = 'color .3s';
            setTimeout(() => el.style.color = '', 600);
        });
    }, 3500);


    /* ════════════════════════════════════════
       AI ASSISTANT  —  Nova chat widget
    ════════════════════════════════════════ */
    const AI_RESPONSES = [
        'Anlıyorum! Daha fazla detay vermemi ister misiniz?',
        'Mevcut verilere göre bu durum ilginç bir gelişme. Araştırabilirim.',
        'Sistemler normal çalışıyor. Herhangi bir anormallik tespit etmedim. 🟢',
        'BTC bu hafta %12.4 yükseldi, piyasalar oldukça hareketli.',
        'İzmir\'de yarın 16°C güneşli bir hava bekleniyor. ☀️',
        'BIST100 bugün %0.8 artışla kapandı.',
    ];

    const aiMessages = $('ai-messages');
    const aiInput = $('ai-input');
    const aiSend = $('ai-send');

    function appendMessage(text, type) {
        if (!aiMessages) return;

        const el = document.createElement('div');
        const baseClass = type === 'user' ? 'ai-msg-user' : 'ai-msg-bot';
        el.className = `${baseClass} rounded p-2.5 text-[11px] leading-relaxed ` +
            (type === 'user' ? 'self-end' : 'self-start') + ' max-w-[85%]';
        el.textContent = text;
        aiMessages.appendChild(el);
        aiMessages.scrollTop = aiMessages.scrollHeight;

        /* Keep conversation window trimmed */
        while (aiMessages.children.length > 8) {
            aiMessages.removeChild(aiMessages.firstChild);
        }
    }

    function showTypingIndicator() {
        if (!aiMessages) return null;

        const el = document.createElement('div');
        el.className = 'flex gap-1.5 p-2.5 self-start';
        el.innerHTML = '<div class="ai-dot"></div><div class="ai-dot"></div><div class="ai-dot"></div>';
        aiMessages.appendChild(el);
        aiMessages.scrollTop = aiMessages.scrollHeight;
        return el;
    }

    function sendAiMessage() {
        const text = (aiInput ? aiInput.value : '').trim();
        if (!text) return;

        if (aiInput) aiInput.value = '';
        appendMessage(text, 'user');

        const indicator = showTypingIndicator();

        setTimeout(function () {
            if (indicator) indicator.remove();
            const reply = AI_RESPONSES[Math.floor(Math.random() * AI_RESPONSES.length)];
            appendMessage(reply, 'bot');
        }, 1300);
    }

    if (aiSend) aiSend.addEventListener('click', sendAiMessage);
    if (aiInput) aiInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') sendAiMessage();
    });

})();

