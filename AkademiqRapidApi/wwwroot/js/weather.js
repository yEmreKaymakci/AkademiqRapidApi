/* ═══════════════════════════════════════════════════════════════════════════
   WEATHER.JS — Atmosfer Terminali Front-End Mantığı
   ─ 15 günlük tahmin render
   ─ 4 şehir kartı (server-side data)
   ─ Arama + dropdown (debounce)
   ─ Türkiye lokasyonları grid + pagination + AJAX arama
   ─ ApexCharts sıcaklık trendi
   ═══════════════════════════════════════════════════════════════════════════ */

/* ─── Hava kodu → emoji + açıklama ──────────────────────────────────────── */
const WX_CODES = {
    0:  { desc: 'Açık',            icon: '☀️' },
    1:  { desc: 'Az Bulutlu',      icon: '🌤️' },
    2:  { desc: 'Parçalı Bulutlu', icon: '⛅' },
    3:  { desc: 'Bulutlu',         icon: '☁️' },
    45: { desc: 'Sisli',           icon: '🌫️' },
    48: { desc: 'Kırağılı Sis',    icon: '🌫️' },
    51: { desc: 'Çisenti',         icon: '🌦️' },
    53: { desc: 'Orta Çisenti',    icon: '🌦️' },
    55: { desc: 'Yoğun Çisenti',   icon: '🌧️' },
    61: { desc: 'Hafif Yağmur',    icon: '🌦️' },
    63: { desc: 'Orta Yağmur',     icon: '🌧️' },
    65: { desc: 'Kuvvetli Yağmur', icon: '🌧️' },
    71: { desc: 'Hafif Kar',       icon: '🌨️' },
    73: { desc: 'Kar',             icon: '❄️' },
    75: { desc: 'Yoğun Kar',       icon: '❄️' },
    80: { desc: 'Sağanak',         icon: '🌦️' },
    81: { desc: 'Kuvvetli Sağanak',icon: '⛈️' },
    95: { desc: 'Fırtına',         icon: '⛈️' },
    96: { desc: 'Dolu Fırtınası',  icon: '⛈️' },
    99: { desc: 'Şiddetli Fırtına',icon: '🌪️' },
};

function wxInfo(code) {
    // Tam eşleşme yoksa en yakın altı
    if (WX_CODES[code]) return WX_CODES[code];
    const keys = Object.keys(WX_CODES).map(Number).sort((a, b) => a - b);
    const closest = keys.filter(k => k <= code).pop();
    return WX_CODES[closest] || { desc: 'Değişken', icon: '🌡️' };
}

/* ─── Yardımcı ───────────────────────────────────────────────────────────── */
function formatDay(dateStr, short = true) {
    const d = new Date(dateStr + 'T00:00:00');
    if (short) return d.toLocaleDateString('tr-TR', { weekday: 'short', day: 'numeric' });
    return d.toLocaleDateString('tr-TR', { weekday: 'long', day: 'numeric', month: 'long' });
}

function isToday(dateStr) {
    const today = new Date().toISOString().slice(0, 10);
    return dateStr === today;
}

function debounce(fn, delay) {
    let t;
    return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), delay); };
}

/* ═══════════════════════════════════════════════════════════════════════════
   1. 15 GÜNLÜK TAHMİN
   ═══════════════════════════════════════════════════════════════════════════ */
function renderForecast(data) {
    const container = document.getElementById('wx-forecast-list');
    if (!container || !data?.daily?.time) return;

    container.innerHTML = data.daily.time.map((time, i) => {
        const info   = wxInfo(data.daily.weathercode[i]);
        const rain   = data.daily.precipitation_probability_max?.[i];
        const today  = isToday(time);
        return `
        <div class="wx-forecast-row ${today ? 'today' : ''}">
            <span class="day-name">${today ? 'Bugün' : formatDay(time)}</span>
            <span class="day-icon">${info.icon}</span>
            <span class="day-max">${Math.round(data.daily.temperature_2m_max[i])}°</span>
            <span class="day-min">${Math.round(data.daily.temperature_2m_min[i])}°</span>
            ${rain != null ? `<span class="day-rain">💧${rain}%</span>` : '<span class="day-rain"></span>'}
        </div>`;
    }).join('');
}

/* ═══════════════════════════════════════════════════════════════════════════
   2. APEXCHARts — SICAKLIK TRENDİ
   (defer yükleme nedeniyle retry mekanizması kullanılıyor)
   ═══════════════════════════════════════════════════════════════════════════ */
let _chartData = null;
let _chartRendered = false;

function renderChart(data) {
    const el = document.getElementById('wx-chart');
    if (!el || !data?.daily?.time) return;

    _chartData = data; // retry için sakla

    if (typeof ApexCharts === 'undefined') {
        // ApexCharts henüz yüklenmemiş — 100ms sonra tekrar dene (max 30 deneme)
        retryChart(0);
        return;
    }

    _doRenderChart(el, data);
}

function retryChart(attempt) {
    if (attempt > 30) return; // 3 saniye timeout
    if (typeof ApexCharts !== 'undefined' && _chartData && !_chartRendered) {
        const el = document.getElementById('wx-chart');
        if (el) _doRenderChart(el, _chartData);
        return;
    }
    setTimeout(() => retryChart(attempt + 1), 100);
}

function _doRenderChart(el, data) {
    if (_chartRendered) {
        // Yeniden çizim: önce temizle
        el.innerHTML = '';
    }
    _chartRendered = false;

    const opts = {
        series: [
            { name: 'Maks °C', data: data.daily.temperature_2m_max.map(v => Math.round(v)) },
            { name: 'Min °C',  data: data.daily.temperature_2m_min.map(v => Math.round(v)) },
        ],
        chart: {
            type: 'area', height: 240,
            toolbar: { show: false },
            background: 'transparent',
            animations: { enabled: true, easing: 'easeinout', speed: 800 },
            fontFamily: "'Space Grotesk', 'Inter', sans-serif"
        },
        stroke: { curve: 'smooth', width: [2.5, 1.5] },
        colors: ['#00c8ff', '#ff24e4'],
        fill: {
            type: 'gradient',
            gradient: {
                shadeIntensity: 1, opacityFrom: 0.2, opacityTo: 0.01, stops: [0, 100]
            }
        },
        theme: { mode: 'dark' },
        xaxis: {
            categories: data.daily.time.map(t => isToday(t) ? 'Bugün' : formatDay(t)),
            labels: { style: { colors: Array(data.daily.time.length).fill('#5a7a9a'), fontSize: '10px' } },
            axisBorder: { show: false },
            axisTicks: { show: false }
        },
        yaxis: {
            labels: {
                style: { colors: '#5a7a9a', fontSize: '10px' },
                formatter: v => `${v}°`
            }
        },
        grid: { borderColor: 'rgba(255,255,255,.05)', strokeDashArray: 3 },
        legend: { labels: { colors: ['#5a7a9a', '#5a7a9a'] }, fontSize: '10px', position: 'top' },
        tooltip: {
            theme: 'dark',
            y: { formatter: v => `${v}°C` }
        },
        markers: { size: [3, 2], colors: ['#00c8ff', '#ff24e4'], strokeWidth: 0, hover: { size: 5 } },
        dataLabels: { enabled: false }
    };

    try {
        new ApexCharts(el, opts).render();
        _chartRendered = true;
    } catch (e) {
        console.error('ApexCharts render hatası:', e);
    }
}

/* ═══════════════════════════════════════════════════════════════════════════
   3. ARAMA + DROPDOWN (server-side geocoding)
   ═══════════════════════════════════════════════════════════════════════════ */
function setupSearch() {
    const input    = document.getElementById('wx-city-search');
    const dropdown = document.getElementById('wx-search-dropdown');
    const clearBtn = document.getElementById('wx-search-clear');
    if (!input || !dropdown) {
        console.warn('Weather: Arama elementleri bulunamadı');
        return;
    }

    let abortCtrl = null;

    function closeDropdown() {
        dropdown.classList.remove('open');
        dropdown.innerHTML = '';
    }

    const doSearch = debounce(async (q) => {
        if (q.length < 2) { closeDropdown(); return; }

        if (abortCtrl) abortCtrl.abort();
        abortCtrl = new AbortController();

        dropdown.innerHTML = '<div class="wx-dropdown-item"><span style="font-size:11px;color:var(--wx-text-dim)">Aranıyor…</span></div>';
        dropdown.classList.add('open');

        try {
            const res  = await fetch(`/Weather/SearchLocation?query=${encodeURIComponent(q)}`,
                                     { signal: abortCtrl.signal });
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const data = await res.json();

            if (!data || !data.length) {
                dropdown.innerHTML = '<div class="wx-dropdown-item" style="color:var(--wx-text-dim);">Sonuç bulunamadı</div>';
                return;
            }

            dropdown.innerHTML = data.map(loc => `
                <div class="wx-dropdown-item"
                     data-lat="${loc.latitude}" data-lon="${loc.longitude}"
                     data-name="${(loc.name || '')}${loc.admin1 ? ', ' + loc.admin1 : ''}${loc.country ? ' — ' + loc.country : ''}">
                    <span style="font-size:1.1rem">📍</span>
                    <div>
                        <div class="loc-name">${loc.name || ''}${loc.admin1 ? ', ' + loc.admin1 : ''}</div>
                        <div class="loc-sub">${loc.country || ''}</div>
                    </div>
                </div>`).join('');

            dropdown.querySelectorAll('.wx-dropdown-item').forEach(item => {
                item.addEventListener('click', () => selectLocation(item, input, dropdown, clearBtn));
            });

        } catch (err) {
            if (err.name !== 'AbortError') {
                console.error('Arama hatası:', err);
                dropdown.innerHTML = '<div class="wx-dropdown-item" style="color:#f87171;">Bağlantı hatası</div>';
            }
        }
    }, 400);

    input.addEventListener('input', e => {
        const q = e.target.value.trim();
        if (clearBtn) clearBtn.style.display = q ? 'block' : 'none';
        doSearch(q);
    });

    // Enter tuşuyla da çalışsın
    input.addEventListener('keydown', e => {
        if (e.key === 'Escape') { input.value = ''; closeDropdown(); if (clearBtn) clearBtn.style.display = 'none'; }
    });

    if (clearBtn) {
        clearBtn.addEventListener('click', () => {
            input.value = '';
            clearBtn.style.display = 'none';
            closeDropdown();
            input.focus();
        });
    }

    document.addEventListener('click', e => {
        if (!e.target.closest('.wx-search-wrap')) closeDropdown();
    });
}

async function selectLocation(item, input, dropdown, clearBtn) {
    const lat  = parseFloat(item.dataset.lat);
    const lon  = parseFloat(item.dataset.lon);
    const name = item.dataset.name;

    input.value = name;
    clearBtn.style.display = 'block';
    dropdown.classList.remove('open');
    dropdown.innerHTML = '';

    // Hero loading göster
    document.getElementById('wx-loc-name-text')?.textContent && 
        (document.getElementById('wx-loc-name-text').textContent = name);
    document.getElementById('wx-hero-temp')?.classList.add('wx-skeleton');
    document.getElementById('wx-hero-icon') && 
        (document.getElementById('wx-hero-icon').innerHTML = '<div class="wx-spinner"></div>');

    try {
        const res  = await fetch(`/Weather/GetWeatherByLocation?lat=${lat}&lon=${lon}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const data = await res.json();

        updateHero(data, name);
        renderForecast(data);

        // Grafik yeniden çiz
        const chartEl = document.getElementById('wx-chart');
        if (chartEl) chartEl.innerHTML = '';
        _chartRendered = false;
        renderChart(data);
    } catch(e) {
        console.error('Lokasyon verisi alınamadı:', e);
        document.getElementById('wx-hero-temp')?.classList.remove('wx-skeleton');
        document.getElementById('wx-hero-icon') && 
            (document.getElementById('wx-hero-icon').innerHTML = '❌');
    } finally {
        document.getElementById('wx-hero-temp')?.classList.remove('wx-skeleton');
    }
}

function updateHero(data, locationName) {
    const cw   = data.current_weather;
    const info = wxInfo(cw?.weathercode ?? 0);

    setEl('wx-hero-temp', cw ? `${Math.round(cw.temperature)}°` : '--');
    setEl('wx-hero-icon', info.icon);
    setEl('wx-hero-desc', info.desc);
    setEl('wx-hero-wind', cw ? `${Math.round(cw.windspeed)} km/s` : '--');

    // Max/min
    const max = data.daily?.temperature_2m_max?.[0];
    const min = data.daily?.temperature_2m_min?.[0];
    setEl('wx-hero-maxmin', max != null ? `↑${Math.round(max)}° ↓${Math.round(min)}°` : '');

    if (locationName) setEl('wx-loc-name-text', locationName);
}

function setEl(id, html) {
    const el = document.getElementById(id);
    if (el) el.innerHTML = html;
}

/* ═══════════════════════════════════════════════════════════════════════════
   4. TÜRKİYE LOKASYONLARI — Grid + Pagination + AJAX Arama
   ═══════════════════════════════════════════════════════════════════════════ */
let _trState = { page: 1, pageSize: 12, total: 0, totalPages: 0, search: '' };

function renderTurkeyCards(items) {
    const grid = document.getElementById('wx-turkey-grid');
    if (!grid) return;

    if (!items.length) {
        grid.innerHTML = `<div style="grid-column:1/-1;text-align:center;color:var(--wx-text-dim);padding:32px;">
            Sonuç bulunamadı.</div>`;
        return;
    }

    grid.innerHTML = items.map(c => {
        if (c.hasError) return `
            <div class="wx-tr-card has-error">
                <div class="tr-city">${c.cityName}</div>
                <div class="tr-meta">Veri yok</div>
            </div>`;

        const info = wxInfo(c.weatherCode ?? 0);
        return `
        <div class="wx-tr-card wx-animate"
             onclick="searchCityFromCard('${c.cityName}')">
            <div style="display:flex;justify-content:space-between;align-items:flex-start;">
                <div>
                    <div class="tr-city">${c.cityName}</div>
                    <div class="tr-meta">${info.desc}</div>
                </div>
                <div class="tr-icon">${info.icon}</div>
            </div>
            <div class="tr-temp">${c.temperature != null ? Math.round(c.temperature) + '°' : '--'}</div>
            <div class="tr-meta" style="display:flex;gap:12px;">
                ${c.windSpeed != null ? `<span>💨 ${Math.round(c.windSpeed)} km/s</span>` : ''}
                ${c.humidity   != null ? `<span>💧 %${c.humidity}</span>` : ''}
                ${c.precipitationProbability != null ? `<span>🌧 %${c.precipitationProbability}</span>` : ''}
            </div>
        </div>`;
    }).join('');
}

function renderPagination() {
    const wrap = document.getElementById('wx-pagination');
    if (!wrap) return;

    const { page, totalPages } = _trState;
    if (totalPages <= 1) { wrap.innerHTML = ''; return; }

    let btns = '';
    // prev
    btns += `<button class="wx-page-btn" onclick="loadTurkeyPage(${page - 1})"
              ${page <= 1 ? 'disabled' : ''}>‹</button>`;

    // numbers
    const range = buildPageRange(page, totalPages);
    range.forEach(p => {
        if (p === '…') {
            btns += `<span style="color:var(--wx-text-dim);padding:0 4px;">…</span>`;
        } else {
            btns += `<button class="wx-page-btn ${p === page ? 'active' : ''}"
                             onclick="loadTurkeyPage(${p})">${p}</button>`;
        }
    });

    // next
    btns += `<button class="wx-page-btn" onclick="loadTurkeyPage(${page + 1})"
              ${page >= totalPages ? 'disabled' : ''}>›</button>`;

    wrap.innerHTML = btns;
}

function buildPageRange(current, total) {
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const pages = [];
    if (current <= 4) {
        for (let i = 1; i <= 5; i++) pages.push(i);
        pages.push('…'); pages.push(total);
    } else if (current >= total - 3) {
        pages.push(1); pages.push('…');
        for (let i = total - 4; i <= total; i++) pages.push(i);
    } else {
        pages.push(1); pages.push('…');
        pages.push(current - 1); pages.push(current); pages.push(current + 1);
        pages.push('…'); pages.push(total);
    }
    return pages;
}

async function loadTurkeyPage(page) {
    const grid = document.getElementById('wx-turkey-grid');
    if (!grid) return;
    _trState.page = page;

    // Skeleton
    grid.innerHTML = Array(6).fill(0).map(() =>
        `<div class="wx-tr-card"><div class="wx-skeleton" style="height:100px;border-radius:8px;"></div></div>`
    ).join('');

    try {
        const url = `/Weather/GetTurkeyLocations?page=${page}&pageSize=${_trState.pageSize}` +
                    (_trState.search ? `&search=${encodeURIComponent(_trState.search)}` : '');
        const res  = await fetch(url);
        const data = await res.json();

        _trState.total      = data.total;
        _trState.totalPages = data.totalPages;

        renderTurkeyCards(data.items);
        renderPagination();

        // Sayfanın turkey bölümüne scroll
        document.getElementById('wx-turkey-section')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    } catch (err) {
        grid.innerHTML = `<div style="grid-column:1/-1;color:#f87171;padding:24px;">
            Veri yüklenemedi: ${err.message}</div>`;
    }
}

// Türkiye arama kutusu
function setupTurkeySearch() {
    const input = document.getElementById('wx-turkey-search');
    if (!input) return;

    const doSearch = debounce(async (q) => {
        _trState.search = q;
        _trState.page = 1;
        await loadTurkeyPage(1);
    }, 500);

    input.addEventListener('input', e => doSearch(e.target.value.trim()));
}

// Türkiye kartına tıklandığında ana arama kutusuna yönlendir + hero'yu güncelle
function searchCityFromCard(cityName) {
    const input = document.getElementById('wx-city-search');
    if (input) {
        input.value = cityName;
        input.dispatchEvent(new Event('input'));
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
}

/* ═══════════════════════════════════════════════════════════════════════════
   5. DASHBOARD WIDGET GÜNCELLE (eğer dashboard sayfasındaysa)
   ═══════════════════════════════════════════════════════════════════════════ */
async function loadDashboardWeather() {
    const widget = document.getElementById('dashboard-weather-widget');
    if (!widget) return;

    try {
        const res  = await fetch('/Weather/GetCurrentLocationWeather');
        if (!res.ok) return;
        const d = await res.json();

        const info = wxInfo(d.weathercode ?? 0);
        const tempEl = widget.querySelector('[data-wx="temp"]');
        const iconEl = widget.querySelector('[data-wx="icon"]');
        const descEl = widget.querySelector('[data-wx="desc"]');
        const cityEl = widget.querySelector('[data-wx="city"]');
        const humEl  = widget.querySelector('[data-wx="humidity"]');
        const windEl = widget.querySelector('[data-wx="wind"]');

        if (tempEl) tempEl.textContent = d.temp != null ? `${Math.round(d.temp)}°C` : '--';
        if (iconEl) iconEl.textContent = info.icon;
        if (descEl) descEl.textContent = info.desc;
        if (cityEl) cityEl.textContent = `${d.city}, ${d.country}`;
        if (humEl)  humEl.textContent  = d.humidity != null ? `%${d.humidity}` : '--';
        if (windEl) windEl.textContent = d.windspeed != null ? `${Math.round(d.windspeed)} km/s` : '--';

        // Loading class'ını kaldır
        widget.classList.remove('wx-loading');
    } catch { /* sessiz hata */ }
}

/* ═══════════════════════════════════════════════════════════════════════════
   INIT
   ═══════════════════════════════════════════════════════════════════════════ */
function initWeatherPage() {
    // Server-side'dan JSON olarak gelen veri
    const rawEl = document.getElementById('wx-data-json');
    if (rawEl) {
        try {
            const pageData = JSON.parse(rawEl.textContent);

            // Hero / anlık (server-side zaten dolduruyor, JS sadece interaktiflik için)
            if (pageData.currentForecast) {
                // Hero zaten SSR ile dolu, sadece chart ve forecast'ı JS ile çiz
                renderForecast(pageData.currentForecast);
                renderChart(pageData.currentForecast); // retry mekanizması ile ApexCharts bekler
            }

            // Türkiye pagination state
            if (pageData.turkeyTotalCount != null) {
                _trState.total      = pageData.turkeyTotalCount;
                _trState.pageSize   = pageData.turkeyPageSize ?? 12;
                _trState.totalPages = Math.ceil(pageData.turkeyTotalCount / _trState.pageSize);
                renderPagination();
            }
        } catch (e) {
            console.error('Weather JSON parse hatası:', e);
        }
    }

    setupSearch();
    setupTurkeySearch();
    loadDashboardWeather();

    // ApexCharts window.load sonrası da bir kez daha kontrol et
    window.addEventListener('load', () => {
        if (!_chartRendered && _chartData) {
            const el = document.getElementById('wx-chart');
            if (el && typeof ApexCharts !== 'undefined') {
                _doRenderChart(el, _chartData);
            }
        }
    });
}

// DOMContentLoaded veya zaten yüklendiyse hemen çalıştır
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initWeatherPage);
} else {
    initWeatherPage();
}