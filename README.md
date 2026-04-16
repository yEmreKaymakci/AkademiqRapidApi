<div align="center">
  <h1>🚀 Akademiq API Dashboard</h1>
  <p>
    <b>Modern mimari, zengin API entegrasyonları ve siberpunk tasarımıyla geliştirilmiş akıllı yönetim paneli.</b>
  </p>
  <br>
</div>

<p align="center">
  <a href="#proje-özellikleri">Proje Özellikleri</a> •
  <a href="#kullanılan-teknolojiler">Kullanılan Teknolojiler</a> •
  <a href="#kurulum">Kurulum</a> •
  <a href="#proje-görselleri">Proje Görselleri</a>
</p>

## 📖 Proje Hakkında

**AkademiqRapidApi**, birçok farklı API servisten( [Rapid Api](https://rapidapi.com/), [football-data](https://www.football-data.org/), [favqs.com](https://favqs.com/), [TMDB](https://www.themoviedb.org/), [Deezer](https://www.deezer.com/), [newsapi](https://newsapi.org/), [Open-Meteo](https://open-meteo.com/)) verileri eş zamanlı çekerek kullanıcıya şık ve fütüristik bir arayüzle sunan gelişmiş bir ASP.NET Core MVC projesidir. Günlük hava durumundan kripto paralara, haber bültenlerinden global akaryakıt verilerine kadar çeşitli alanları kapsayan bu proje; aynı zamanda **Google Gemini destekli Asistan Nova** ile kullanıcılara anlık yapay zeka analizleri ve interaktif sohbet deneyimi sunar. Etkileyici "Cyberpunk" arayüzü sayesinde bilgiyi estetik bir görsellikle verir. 

>**Not:** Bu proje, **AkademiQ AI Business School** eğitimi kapsamında geliştirilmiştir.

---

## ✨ Proje Özellikleri

Proje, hem arka uç (backend) gücü hem de zengin görsel donanımıyla öne çıkmaktadır:

- **Çoklu API Entegrasyonu:** Gerçek zamanlı API'ler kullanılarak Hava Durumu, Akaryakıt, Döviz, Kripto (Coin), Futbol, Haberler, Filmler, Müzikler ve Motivasyon verileri çekilir.
- **Asistan Nova (Yapay Zeka):** Google Gemini kullanılarak sisteme entegre edilen Nova; proje içi verileri okuyabilen, analiz edebilen ve kullanıcı diyaloglarını hafızasında tutabilen yeni nesil bir asistandır.
- **Dinamik Veri Arşivleme:** Nova ile olan sohbetleriniz ve sistem analizleri (Analysis & Archive modülleri), Entity Framework Core kullanılarak otomatik olarak SQL veritabanına kaydedilir.
- **Global Arama Motoru:** Arayüz içi arama kutusu sayesinde "Hava", "Döviz", "Kripto" gibi anahtar kelimeler yazılarak ilgili modüllere anında statü geçişleri (Fast Route) yapılır.
- **Siberpunk/Fütüristik Tasarım:** Cam efektleri (glassmorphism), neon parlamalar (glow etkileri) ve koyu mod desteği ile TailwindCSS tabanlı estetik bir yapı kullanılmıştır.
- **Active State Navigasyonu:** İçerisinde bulunduğunuz modül, hem navbar hem de sidebar menü bileşenleri üzerinden dinamik olarak vurgulanır.

---

## 🚀 Kullanılan Teknolojiler

Proje, güncel teknolojiler kullanılarak ölçeklenebilir ve temiz mimari (Clean Architecture) prensiplerine uygun olarak inşa edilmiştir:

### ⚙️ Backend (Arka Uç) ve Mimari
- **.NET 9.0:** ASP.NET Core MVC çatısı ile yüksek performanslı sunucu taraflı işlemler.
- **C# Programlama Dili:** Asenkron programlama (`async/await`) ve akış destekli (`IAsyncEnumerable`) gelişmiş C# özellikleri.
- **Entity Framework Core 9.0:** Veritabanı yönetimi, Code-First yaklaşımı ve Migration işlemleri için modern ORM.
- **N-Tier (Katmanlı) Mantık:** Servis (I***Service), View Component, Controller ve Model katmanlarının birbirinden izole edilerek kullanılması (Repository Pattern/Dependency Injection).

### 🤖 Entegre Edilen Dış API'ler (External Services)
Projeye veri sağlayan tüm köprüler HTTP istemcileri üzerinden asenkron olarak çekilmektedir:
- **Google Gemini Yapay Zeka API:** Asistan Nova'nın dil modeli altyapısı, `Stream` akış yeteneği ve içerik üretimi.
- **RapidAPI Hub Sağlayıcısı:**
  - *Coinranking API:* Kripto para birimlerinin güncel değerleri ve istatistikleri.
  - *Gas Price / Currency API:* Küresel akaryakıt ve günlük anlık döviz piyasası kurları.
  - *API-Football:* Canlı futbol ligleri, skorlar ve takım verileri.
- **FavQs API & Google Translate API:** Motivasyon modülü için günlük rastgele özlü yabancı sözlerin çekilmesi ve bunların anlık olarak sistem üzerinde Türkçeye çevirilmesi.
- **Open-Meteo API:** Haftalık sıcaklık grafikleri ve detaylı hava durumu verileri.
- **The Movie Database (TMDB)** Popüler filmler/dizileri kategorilere göre çekilerek bir film kitaplığı oluşturma.
- **Deezer** En çok dinlenen müziklerin çekilmesi ve kullanıcıya dinleyebileceği şekilde sunulması.
- **News Api** Dikkat çeken manşetleri, yaşanan yeni gelişmeleri kategorilere ayırarak kullanıcıya sunma.

### 🎨 Frontend (Ön Yüz)
- **TailwindCSS:** Geleneksel CSS yazımını azaltarak HTML içerisinde anlık reaksiyon yaratan Utility-First modern stil çerçevesi.
- **JavaScript & AJAX (Fetch API):** Sayfayı yenilemeden Asistan Nova'nın yapay zeka akış verilerini ekrana basmak ve dinamik arama butonları oluşturmak için kullanılmıştır.
- **Razor Pages (`.cshtml`):** C# değişkenlerinin ve döngülerinin HTML ile harmanlanarak projenin dinamik verileri UI'a basabilmesi.
- **CSS3 Animations:** Arayüze "yaşıyor" hissini katan nefes alma (breathing) ve scanline (tarama) mikro animasyon efektleri.

---

## 🛠️ Ön Koşullar
- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/tr-tr/sql-server/sql-server-downloads)

## 📸 Proje Görselleri


<div align="center">
  
  <p><strong>DASHBOARD</strong></p>
  <img width="1917" height="892" alt="Dashboard1" src="https://github.com/user-attachments/assets/fe4c3889-93a4-4f43-b570-70407cf71826" />
  <br><br>

  <p><strong>DASHBOARD</strong></p>
  <img width="1918" height="937" alt="Dashboard2" src="https://github.com/user-attachments/assets/bd513471-7187-4ae1-ba00-e257c53476fa" />
  <br><br>

  <p><strong>ASİSTAN NOVA</strong></p>
  <img width="1912" height="867" alt="NovaChatBot" src="https://github.com/user-attachments/assets/e6f88a35-0d52-47a6-9377-0b5aaabd0e5e" />
  <br><br>

  <p><strong>ANALİZ</strong></p>
  <img width="1912" height="858" alt="Analiz" src="https://github.com/user-attachments/assets/bd5ec38a-c457-4a56-8fda-12285bf25131" />  
  <br><br>

  <p><strong>ARŞİV</strong></p>
  <img width="1915" height="867" alt="Arşiv1" src="https://github.com/user-attachments/assets/e7d5f6ca-bdbb-4402-a2bb-5ae95d62694d" />
  <br><br>

  <p><strong>ARŞİV DETAY</strong></p>
  <img width="1917" height="860" alt="Arşiv2" src="https://github.com/user-attachments/assets/7b2ddd7a-9b46-410e-8d51-5433e6a82b3d" />
  <br><br>

  <p><strong>DÖVİZ</strong></p>
  <img width="1918" height="863" alt="Döviz" src="https://github.com/user-attachments/assets/25f609b7-8ab2-410b-b976-9be42997fca8" />
  <br><br>


  <p><strong>KRİPTO</strong></p>
  <img width="1917" height="872" alt="Kripto" src="https://github.com/user-attachments/assets/f85a5c93-3276-4d1f-b836-d565dd7925c3" />
  <br><br>


  <p><strong>YEMEK</strong></p>
  <img width="1918" height="877" alt="Tarif" src="https://github.com/user-attachments/assets/6f975683-e35e-4b81-a786-4dbbda5085ff" />
  <br><br>


  <p><strong>HAVA</strong></p>
  <img width="1913" height="880" alt="HavaDurumu" src="https://github.com/user-attachments/assets/06b6a480-37c4-4b10-bd17-6e92050a7ca0" />  
  <br><br>


  <p><strong>FUTBOL</strong></p>
  <img width="1918" height="861" alt="Futbol" src="https://github.com/user-attachments/assets/101f3eb9-2a80-47cf-8928-ed4e8324cd79" />
  <br><br>


  <p><strong>HABERLER</strong></p>
  <img width="1918" height="878" alt="Haber" src="https://github.com/user-attachments/assets/5dc71b27-7a91-4f83-aa56-978c7b3e96a4" />
  <br><br>

  <p><strong>FİLMLER</strong></p>
  <img width="1913" height="871" alt="Filmler" src="https://github.com/user-attachments/assets/5192c23b-fb84-4ed6-a300-49bd2d0b76c2" />
  <br><br>


  <p><strong>MÜZİK</strong></p>
  <img width="1885" height="872" alt="Müzik" src="https://github.com/user-attachments/assets/0853c34e-1906-4d0a-b402-4efb6ee62d2a" />
  <br><br>

  <p><strong>MOTİVASYON</strong></p>
  <img width="1907" height="845" alt="Motivasyon" src="https://github.com/user-attachments/assets/6eb26b0c-59aa-41a8-8df3-879efe0db251" />
  <br><br>
