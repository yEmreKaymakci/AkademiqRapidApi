using AkademiqRapidApi.Services;
using AkademiqRapidApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Core servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// DbContext kaydı
builder.Services.AddDbContext<AkademiqRapidApi.Models.Contexts.AkademiqDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// GeminiOptions kaydı
builder.Services.Configure<AkademiqRapidApi.Models.GeminiOptions>(builder.Configuration.GetSection("GeminiOptions"));

// ─────────────────────────────────────────────────────────────────────────────
// Servis Kayıtları — AddHttpClient<Interface, Implementation> pattern'i
// IHttpClientFactory, named/typed client yönetimi, retry politikaları için
// en uygun yöntemdir. Tüm servisler aynı pattern ile kaydedilmiştir.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<IRecipeService, RecipeService>();
builder.Services.AddHttpClient<IGasService, GasService>();
builder.Services.AddHttpClient<IConvertCurrencyService, ConvertCurrencyService>();
builder.Services.AddHttpClient<ICoinService, CoinService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddHttpClient<IFootballService, FootballService>();
builder.Services.AddHttpClient<INewsService, NewsService>();
builder.Services.AddHttpClient<IMovieService, MovieService>();
builder.Services.AddHttpClient<IMusicService, MusicService>();
builder.Services.AddScoped<IMotivationService, MotivationService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddScoped<IArchiveService, ArchiveService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
