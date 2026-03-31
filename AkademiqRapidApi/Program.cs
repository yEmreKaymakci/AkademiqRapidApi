using AkademiqRapidApi.Services;
using AkademiqRapidApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient(); // Varolan factory-based (IHttpClientFactory) denemelerin bozulmaması için yerinde kalsın.

// 1. Dashboard hızlandırması ve RAM ÖnBellek yapılandırması
builder.Services.AddMemoryCache();

// 2. Servis Kaydı (Typed HttpClient Pattern - Service sınıfına otomatik HttpClient Constructor'u ekler)
builder.Services.AddHttpClient<AkademiqRapidApi.Services.Interfaces.IRecipeService, AkademiqRapidApi.Services.RecipeService>();
builder.Services.AddScoped<IGasService, GasService>();
builder.Services.AddScoped<IConvertCurrencyService, ConvertCurrencyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
