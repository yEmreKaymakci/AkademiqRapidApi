/**
 * football.js — Skor Merkezi
 * Bugünün maçları sayfa yüklenirken server-side render edilir,
 * lig butonları AJAX ile veri çeker.
 */

document.addEventListener('DOMContentLoaded', () => {

    // ── Elemanlar ────────────────────────────────────────────────
    const dataEl       = document.getElementById('footballDataJson');
    const fixtureList  = document.getElementById('fixtureList');
    const loadingState = document.getElementById('loadingState');
    const emptyState   = document.getElementById('emptyState');
    const matchSearch  = document.getElementById('matchSearch');
    const searchWrapper= document.getElementById('searchWrapper');
    const searchClear  = document.getElementById('searchClear');
    const refreshBtn   = document.getElementById('refreshBtn');

    // Stats
    const statTotal = document.getElementById('statTotal');
    const statLive  = document.getElementById('statLive');
    const statFt    = document.getElementById('statFt');
    const statNs    = document.getElementById('statNs');

    // ── Durum ────────────────────────────────────────────────────
    let allFixtures = [];
    let activeMode  = 'today';
    let activeId    = '';
    let searchQuery = '';

    // ── Sayfa ilk yüklendiğinde server'dan gelen veriyi kullan ──
    try {
        if (dataEl && dataEl.textContent.trim() !== 'null') {
            allFixtures = JSON.parse(dataEl.textContent) || [];
        }
    } catch (e) { allFixtures = []; }

    // ── Yardımcı: fixture key dönüşümü ──────────────────────────
    // Server C# PascalCase -> JSON PascalCase, JS işlemlerde normalize et
    function norm(f) {
        // JSON.NET veya System.Text.Json ile PascalCase serialize edildi;
        // hem büyük hem küçük harfli alanlara karşı dayanıklı erişim sağla
        const fixture = f.Fixture || f.fixture || {};
        const league  = f.League  || f.league  || {};
        const teams   = f.Teams   || f.teams   || {};
        const goals   = f.Goals   || f.goals   || {};
        const score   = f.Score   || f.score   || {};
        const status  = fixture.Status || fixture.status || {};
        const home    = teams.Home || teams.home || {};
        const away    = teams.Away || teams.away || {};
        const venue   = fixture.Venue || fixture.venue || {};
        const ht      = score.Halftime || score.halftime || {};

        return {
            id:          fixture.Id   || fixture.id   || 0,
            date:        fixture.Date || fixture.date || '',
            statusShort: status.Short || status.short || '',
            statusLong:  status.Long  || status.long  || '',
            elapsed:     status.Elapsed || status.elapsed || null,

            leagueId:    league.Id   || league.id   || 0,
            leagueName:  league.Name || league.name || '',
            leagueLogo:  league.Logo || league.logo || '',
            leagueCountry: league.Country || league.country || '',
            leagueFlag:  league.Flag || league.flag || '',

            homeName:    home.Name || home.name || '',
            homeLogo:    home.Logo || home.logo || '',
            homeWinner:  home.Winner ?? home.winner ?? null,
            awayName:    away.Name || away.name || '',
            awayLogo:    away.Logo || away.logo || '',
            awayWinner:  away.Winner ?? away.winner ?? null,

            goalsHome:   goals.Home ?? goals.home ?? null,
            goalsAway:   goals.Away ?? goals.away ?? null,

            htHome:      ht.Home ?? ht.home ?? null,
            htAway:      ht.Away ?? ht.away ?? null,

            venueName:   venue.Name || venue.name || '',
            venueCity:   venue.City || venue.city || '',
        };
    }

    // ── İstatistik güncelleme ────────────────────────────────────
    function updateStats(fixtures) {
        statTotal.textContent = fixtures.length;
        statLive.textContent  = fixtures.filter(f => {
            const m = norm(f);
            return m.statusShort !== 'NS' && m.statusShort !== 'FT' && m.statusShort !== 'TBD';
        }).length;
        statFt.textContent    = fixtures.filter(f => norm(f).statusShort === 'FT').length;
        statNs.textContent    = fixtures.filter(f => norm(f).statusShort === 'NS').length;
    }

    // ── Badge render ─────────────────────────────────────────────
    function renderBadge(m) {
        if (m.statusShort === 'FT')  return `<span class="fb-badge fb-badge--ft">FT</span>`;
        if (m.statusShort === 'NS') {
            const time = m.date ? new Date(m.date).toLocaleTimeString('tr-TR', { hour:'2-digit', minute:'2-digit' }) : '--:--';
            return `<span class="fb-badge fb-badge--ns">${time}</span>`;
        }
        if (m.statusShort === 'HT') return `<span class="fb-badge fb-badge--other">DEVRE ARASI</span>`;
        if (['1H','2H','ET','P'].includes(m.statusShort)) {
            return `<span class="fb-badge fb-badge--live">${m.elapsed ?? ''}&apos;</span>`;
        }
        return `<span class="fb-badge fb-badge--other">${m.statusShort}</span>`;
    }

    // ── Tek maç kartı ─────────────────────────────────────────────
    function renderCard(f) {
        const m = norm(f);
        const isLive   = ['1H','2H','ET','P'].includes(m.statusShort);
        const isFt     = m.statusShort === 'FT';
        const homeWins = m.homeWinner === true;
        const awayWins = m.awayWinner === true;

        const scoreDisplay = (m.goalsHome !== null && m.goalsAway !== null)
            ? `<span class="${isLive ? 'fb-score live-score' : 'fb-score'}">
                   <span class="${homeWins ? 'winner-num' : ''}">${m.goalsHome}</span>
                   <span class="fb-score-sep">–</span>
                   <span class="${awayWins ? 'winner-num' : ''}">${m.goalsAway}</span>
               </span>`
            : `<span class="fb-score"><span class="fb-score-sep" style="font-size:1.1rem">vs</span></span>`;

        const htDisplay = (m.htHome !== null && m.htAway !== null && isFt)
            ? `<span class="fb-score-ht">İY: ${m.htHome}–${m.htAway}</span>` : '';

        const dateObj  = m.date ? new Date(m.date) : null;
        const timeStr  = dateObj ? dateObj.toLocaleTimeString('tr-TR', { hour:'2-digit', minute:'2-digit' }) : '';
        const dateStr  = dateObj ? dateObj.toLocaleDateString('tr-TR', { day:'2-digit', month:'short' }) : '';

        const logoPlaceholder = `onerror="this.src='data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 32 32%22%3E%3Ccircle cx=%2216%22 cy=%2216%22 r=%2215%22 fill=%22%23122%22 stroke=%22%2322c55e%22 stroke-width=%221%22/%3E%3C/svg%3E'"`;

        return `
        <div class="fb-fixture-card animate-in">
            <div class="fb-col-status">
                ${renderBadge(m)}
                ${m.elapsed ? `<span class="fb-fixture-minute">${m.elapsed}&apos;</span>` : ''}
            </div>
            <div class="fb-col-home">
                <span class="fb-team-name ${homeWins ? 'winner' : ''}">${m.homeName}</span>
                <img src="${m.homeLogo}" class="fb-team-logo" alt="${m.homeName}" ${logoPlaceholder} />
            </div>
            <div class="fb-col-score">
                ${scoreDisplay}
                ${htDisplay}
            </div>
            <div class="fb-col-away">
                <img src="${m.awayLogo}" class="fb-team-logo" alt="${m.awayName}" ${logoPlaceholder} />
                <span class="fb-team-name ${awayWins ? 'winner' : ''}">${m.awayName}</span>
            </div>
            <div class="fb-col-info">
                ${m.venueName ? `<span class="fb-venue" title="${m.venueName}, ${m.venueCity}">${m.venueName}</span>` : ''}
                <span class="fb-date-time">${dateStr} ${timeStr}</span>
            </div>
        </div>`;
    }

    // ── Lig gruplaması ile render ─────────────────────────────────
    function renderAll(fixtures) {
        const filtered = searchQuery
            ? fixtures.filter(f => {
                const m = norm(f);
                const q = searchQuery.toLowerCase();
                return m.homeName.toLowerCase().includes(q)
                    || m.awayName.toLowerCase().includes(q)
                    || m.leagueName.toLowerCase().includes(q)
                    || m.leagueCountry.toLowerCase().includes(q);
              })
            : fixtures;

        updateStats(fixtures);

        if (filtered.length === 0) {
            fixtureList.style.display = 'none';
            emptyState.style.display  = 'flex';
            return;
        }

        emptyState.style.display  = 'none';
        fixtureList.style.display = 'block';

        // Lige göre grupla
        const groups = {};
        filtered.forEach(f => {
            const m = norm(f);
            const key = m.leagueId || 0;
            if (!groups[key]) groups[key] = { name: m.leagueName, logo: m.leagueLogo, country: m.leagueCountry, flag: m.leagueFlag, items: [] };
            groups[key].items.push(f);
        });

        fixtureList.innerHTML = Object.values(groups).map(g => `
            <div class="fb-fixture-group">
                <div class="fb-group-header">
                    <img src="${g.logo}" class="fb-group-logo" alt="${g.name}" onerror="this.style.display='none'" />
                    <span class="fb-group-name">${g.name}</span>
                    <span class="fb-group-country">
                        ${g.flag ? `<img src="${g.flag}" class="fb-group-flag" onerror="this.style.display='none'" />` : ''}
                        ${g.country}
                    </span>
                </div>
                ${g.items.map(renderCard).join('')}
            </div>
        `).join('');
    }

    // ── Loading göster/gizle ─────────────────────────────────────
    function showLoading() {
        loadingState.style.display  = 'flex';
        fixtureList.style.display   = 'none';
        emptyState.style.display    = 'none';
    }
    function hideLoading() {
        loadingState.style.display = 'none';
    }

    // ── AJAX ile veri çek ─────────────────────────────────────────
    async function loadFixtures(mode, leagueId) {
        showLoading();
        let apiError = false;
        try {
            let url;
            if (mode === 'live')        url = '/Football/GetLive';
            else if (mode === 'today')  url = '/Football/GetToday';
            else url = `/Football/GetByLeague?leagueId=${leagueId}&season=2024`;

            const res  = await fetch(url);
            if (!res.ok) { apiError = true; allFixtures = []; }
            else {
                const data = await res.json();
                allFixtures = data || [];
            }
        } catch {
            apiError = true;
            allFixtures = [];
        } finally {
            hideLoading();
            renderAll(allFixtures);

            // API abonelik hatası için özel mesaj
            if (apiError || allFixtures.length === 0) {
                const emptyIcon = document.getElementById('emptyIcon');
                const emptyText = document.getElementById('emptyText');
                const emptySub  = document.getElementById('emptySub');
                const emptyLink = document.getElementById('emptyLink');
                if (emptyIcon) emptyIcon.textContent = '🔑';
                if (emptyText) emptyText.textContent = 'football-data.org API\'sine erişilemiyor.';
                if (emptySub)  emptySub.textContent  = 'football-data.org üzerinden ücretsize kayıt olabilirsiniz.';
                if (emptyLink) emptyLink.style.display = 'inline-block';
            }
        }
    }

    // ── Lig butonları ─────────────────────────────────────────────
    document.querySelectorAll('.fb-league-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
            document.querySelectorAll('.fb-league-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            activeMode = btn.dataset.mode;
            activeId   = btn.dataset.id || '';
            searchQuery= '';
            matchSearch.value = '';
            searchWrapper.classList.remove('has-value');

            await loadFixtures(activeMode, activeId);
        });
    });

    // ── Arama ─────────────────────────────────────────────────────
    matchSearch.addEventListener('input', e => {
        searchQuery = e.target.value.trim();
        searchWrapper.classList.toggle('has-value', searchQuery.length > 0);
        renderAll(allFixtures);
    });

    searchClear.addEventListener('click', () => {
        matchSearch.value = '';
        searchQuery = '';
        searchWrapper.classList.remove('has-value');
        renderAll(allFixtures);
    });

    // ── Yenile butonu ─────────────────────────────────────────────
    refreshBtn.addEventListener('click', async () => {
        refreshBtn.classList.add('spinning');
        await loadFixtures(activeMode, activeId);
        setTimeout(() => refreshBtn.classList.remove('spinning'), 600);
    });

    // ── İlk render: server-side veri ─────────────────────────────
    hideLoading();
    if (allFixtures.length > 0) {
        renderAll(allFixtures);
    } else {
        // Server boş döndüyse AJAX ile çek
        loadFixtures('today', '');
    }

});

// ── Dashboard widget'ı güncelle (her sayfada çalışır) ──────────
(async function initDashboardFootball() {
    const widget = document.getElementById('fb-dashboard-widget');
    if (!widget) return;

    try {
        const res  = await fetch('/Football/GetFeatured');
        if (!res.ok) return;
        const data = await res.json();
        if (!data) return;

        const league = data.League || data.league || {};
        const teams  = data.Teams  || data.teams  || {};
        const goals  = data.Goals  || data.goals  || {};
        const status = (data.Fixture || data.fixture || {}).Status || {};
        const home   = teams.Home || teams.home || {};
        const away   = teams.Away || teams.away || {};

        const isLive = ['1H','2H','ET','P'].includes(status.Short || status.short || '');
        const isFt   = (status.Short || status.short) === 'FT';
        const elapsed= status.Elapsed || status.elapsed;

        // Badge
        const badgeEl = widget.querySelector('[data-fb="badge"]');
        if (badgeEl) {
            if (isLive) {
                badgeEl.innerHTML = `<span class="w-1.5 h-1.5 rounded-full bg-green-400 pulse-dot"></span><span class="font-headline text-[8px] text-green-400 uppercase tracking-wider">${elapsed || ''}' Canlı</span>`;
                badgeEl.className = 'flex items-center gap-1.5 px-2 py-1 rounded-full bg-green-400/10 border border-green-400/25';
            } else if (isFt) {
                badgeEl.innerHTML = `<span class="font-headline text-[8px] text-error uppercase tracking-wider">Bitti</span>`;
                badgeEl.className = 'flex items-center gap-1.5 px-2 py-1 rounded-full bg-error/10 border border-error/25';
            }
        }

        // Lig adı
        const leagueEl = widget.querySelector('[data-fb="league"]');
        if (leagueEl && league.Name) leagueEl.textContent = `${league.Name || ''} · ${league.Country || ''}`;

        // Ev sahibi
        const homeName = widget.querySelector('[data-fb="homeName"]');
        const homeLogo = widget.querySelector('[data-fb="homeLogo"]');
        if (homeName) homeName.textContent = home.Name || '';
        if (homeLogo && home.Logo) {
            homeLogo.src = home.Logo;
            homeLogo.alt = home.Name || '';
            homeLogo.style.display = 'block';
        }

        // Deplasman
        const awayName = widget.querySelector('[data-fb="awayName"]');
        const awayLogo = widget.querySelector('[data-fb="awayLogo"]');
        if (awayName) awayName.textContent = away.Name || '';
        if (awayLogo && away.Logo) {
            awayLogo.src = away.Logo;
            awayLogo.alt = away.Name || '';
            awayLogo.style.display = 'block';
        }

        // Skor
        const scoreHome = widget.querySelector('[data-fb="scoreHome"]');
        const scoreAway = widget.querySelector('[data-fb="scoreAway"]');
        if (scoreHome) scoreHome.textContent = goals.Home ?? goals.home ?? '-';
        if (scoreAway) scoreAway.textContent = goals.Away ?? goals.away ?? '-';
    } catch (e) { /* sessizce başarısız ol */ }
})();