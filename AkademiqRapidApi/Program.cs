using AkademiqRapidApi.Services;
using AkademiqRapidApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Core servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

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
