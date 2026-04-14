document.addEventListener("DOMContentLoaded", function () {
    // uptime için rastgele bir saniye değerinden başlatalım (örneğin 3 gün 5 saat vs)
    // Ya da projenin başladığı anı referans alalım (basitçe sıfırdan başlasın demo için)
    let uptimeSeconds = 11342; // 3 saat civarı örnek

    function updateClocks() {
        const now = new Date();
        
        // Saat Formatı
        const hrs = String(now.getHours()).padStart(2, '0');
        const mins = String(now.getMinutes()).padStart(2, '0');
        const secs = String(now.getSeconds()).padStart(2, '0');
        const timeStr = `${hrs}:${mins}:${secs}`;

        // Tarih Formatı
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        const dateStr = now.toLocaleDateString('tr-TR', options);

        // Uptime Formatı
        uptimeSeconds++;
        const uHrs = String(Math.floor(uptimeSeconds / 3600)).padStart(2, '0');
        const uMins = String(Math.floor((uptimeSeconds % 3600) / 60)).padStart(2, '0');
        const uSecs = String(uptimeSeconds % 60).padStart(2, '0');
        const uptimeStr = `${uHrs}:${uMins}:${uSecs}`;

        // DOM güncelle
        const heroClock = document.getElementById("hero-clock");
        const pageClock = document.getElementById("page-clock");
        const heroDate = document.getElementById("hero-date");
        const uptimeEl = document.getElementById("uptime");

        if (heroClock) heroClock.innerText = timeStr;
        if (pageClock) pageClock.innerText = timeStr;
        if (heroDate) heroDate.innerText = dateStr;
        if (uptimeEl) uptimeEl.innerText = uptimeStr;
    }

    // İlk çağrı ve 1sn interval
    updateClocks();
    setInterval(updateClocks, 1000);
});
