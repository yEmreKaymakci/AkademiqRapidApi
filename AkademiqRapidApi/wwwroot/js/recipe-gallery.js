/* ================================================
   recipe-gallery.js — Tarif Galerisi Scripti
   wwwroot/js/recipe-gallery.js
   ================================================ */

(function () {
    'use strict';

    /* Bu sayfa için gerekli element yoksa çık */
    const raw = document.getElementById('recipe-data');
    if (!raw) return;

    const ITEMS_PER_PAGE = 16;
    const ALL_RECIPES = JSON.parse(raw.textContent);

    /* ── State ── */
    let filtered = [...ALL_RECIPES];
    let currentPage = 1;
    let searchTimeout;

    /* ── DOM Referansları ── */
    const grid         = document.getElementById('recipe-grid');
    const pagination   = document.getElementById('pagination');
    const searchInput  = document.getElementById('search-input');
    const resultsCount = document.getElementById('results-count');

    /* Modal */
    const modal        = document.getElementById('recipe-modal');
    const modalTitle   = document.getElementById('modal-title');
    const modalImg     = document.getElementById('modal-img');
    const modalVideo   = document.getElementById('modal-video-btn');
    const modalIngr    = document.getElementById('modal-ingredients');
    const modalSteps   = document.getElementById('modal-steps');

    /* ── Grid Render ── */
    function renderGrid() {
        const start = (currentPage - 1) * ITEMS_PER_PAGE;
        const slice = filtered.slice(start, start + ITEMS_PER_PAGE);

        if (slice.length === 0) {
            grid.innerHTML = `
                <div class="empty-state">
                    <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                        <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
                    </svg>
                    <h4>Sonuç bulunamadı</h4>
                    <p>Farklı bir arama terimi deneyin</p>
                </div>`;
            pagination.innerHTML = '';
            return;
        }

        grid.innerHTML = slice.map(r => `
            <article class="recipe-card" tabindex="0" data-id="${r.id}" role="button" aria-label="${escHtml(r.name)} tarifini görüntüle">
                <div class="card-img-wrap">
                    <img src="${escHtml(r.thumbnail)}" alt="${escHtml(r.name)}" loading="lazy" />
                    <span class="card-time-badge">
                        <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/>
                        </svg>
                        ${r.time} dk
                    </span>
                </div>
                <div class="card-body">
                    <p class="card-title">${escHtml(r.name)}</p>
                    <div class="card-action">
                        <button class="btn-detail">İncele →</button>
                    </div>
                </div>
            </article>
        `).join('');

        renderPagination();
    }

    /* ── Sayfalama ── */
    function renderPagination() {
        const total = Math.ceil(filtered.length / ITEMS_PER_PAGE);
        if (total <= 1) { pagination.innerHTML = ''; return; }

        let html = `<button class="pg-btn" id="pg-prev" ${currentPage === 1 ? 'disabled' : ''}>&#8249;</button>`;

        buildPageRange(currentPage, total).forEach(p => {
            if (p === '…') {
                html += `<span class="pg-info">…</span>`;
            } else {
                html += `<button class="pg-btn ${p === currentPage ? 'active' : ''}" data-page="${p}">${p}</button>`;
            }
        });

        html += `<button class="pg-btn" id="pg-next" ${currentPage === total ? 'disabled' : ''}>&#8250;</button>`;
        html += `<span class="pg-info">${(currentPage - 1) * ITEMS_PER_PAGE + 1}–${Math.min(currentPage * ITEMS_PER_PAGE, filtered.length)} / ${filtered.length}</span>`;

        pagination.innerHTML = html;

        pagination.querySelector('#pg-prev')?.addEventListener('click', () => goPage(currentPage - 1));
        pagination.querySelector('#pg-next')?.addEventListener('click', () => goPage(currentPage + 1));
        pagination.querySelectorAll('[data-page]').forEach(btn => {
            btn.addEventListener('click', () => goPage(parseInt(btn.dataset.page, 10)));
        });
    }

    function buildPageRange(cur, total) {
        if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
        const pages = new Set([1, total, cur, cur - 1, cur + 1].filter(p => p >= 1 && p <= total));
        const sorted = [...pages].sort((a, b) => a - b);
        const result = [];
        sorted.forEach((p, i) => {
            if (i > 0 && p - sorted[i - 1] > 1) result.push('…');
            result.push(p);
        });
        return result;
    }

    function goPage(page) {
        const total = Math.ceil(filtered.length / ITEMS_PER_PAGE);
        if (page < 1 || page > total) return;
        currentPage = page;
        renderGrid();
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    /* ── Arama ── */
    if (searchInput) {
        searchInput.addEventListener('input', () => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const q = searchInput.value.trim().toLowerCase();
                filtered = q
                    ? ALL_RECIPES.filter(r => r.name.toLowerCase().includes(q))
                    : [...ALL_RECIPES];
                currentPage = 1;
                renderGrid();
                if (resultsCount) {
                    resultsCount.textContent = q ? `${filtered.length} sonuç` : '';
                }
            }, 220);
        });
    }

    /* ── Kart Tıklama (Event Delegation) ── */
    grid.addEventListener('click', e => {
        const card = e.target.closest('.recipe-card');
        if (card) openModal(parseInt(card.dataset.id, 10));
    });

    grid.addEventListener('keydown', e => {
        if (e.key === 'Enter' || e.key === ' ') {
            const card = e.target.closest('.recipe-card');
            if (card) { e.preventDefault(); openModal(parseInt(card.dataset.id, 10)); }
        }
    });

    /* ── Modal ── */
    function openModal(id) {
        const r = ALL_RECIPES.find(x => x.id === id);
        if (!r) return;

        modalTitle.textContent = r.name;
        modalImg.src = r.thumbnail;
        modalImg.alt = r.name;

        if (r.video) {
            modalVideo.href = r.video;
            modalVideo.style.display = 'flex';
        } else {
            modalVideo.style.display = 'none';
        }

        modalIngr.innerHTML = r.ingredients.length
            ? r.ingredients.map(i => `<li>${escHtml(i)}</li>`).join('')
            : `<li class="no-data">Malzeme bilgisi bulunamadı</li>`;

        modalSteps.innerHTML = r.steps.length
            ? r.steps.map(s => `
                <li class="step-item">
                    <span class="step-num">${s.pos}</span>
                    <span class="step-text">${escHtml(s.text)}</span>
                </li>`).join('')
            : `<li class="no-data">Hazırlanış adımları bulunamadı</li>`;

        modal.classList.add('open');
        document.body.style.overflow = 'hidden';
    }

    function closeModal() {
        modal.classList.remove('open');
        document.body.style.overflow = '';
    }

    document.getElementById('modal-close-btn').addEventListener('click', closeModal);
    modal.addEventListener('click', e => { if (e.target === modal) closeModal(); });
    document.addEventListener('keydown', e => { if (e.key === 'Escape') closeModal(); });

    /* ── Yardımcı: XSS Koruması ── */
    function escHtml(str) {
        return String(str ?? '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    /* ── İlk Render ── */
    renderGrid();

})();
