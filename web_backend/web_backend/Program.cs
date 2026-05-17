using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using web_backend; //AppDbContexti tanýyan namespace
using web_backend.Services;


var builder = WebApplication.CreateBuilder(args);

// Servis mimarisini sisteme tanýtýyoruz
builder.Services.AddSingleton<IPredictionService, PredictionService>();

builder.Services.AddScoped<IBinService, BinService>();

// 1. Veritabaný Servisini Kaydet
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kestrel için dosya yükleme limitini 50MB yapalým (Base64 için önemli)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<IIoTService, SimulatedIoTService>();

builder.Services.AddScoped<ITrustScoreService, TrustScoreService>();

builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IRankingService, RankingService>();
builder.Services.AddScoped<IPointTransactionService, PointTransactionService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login"; // Yetkisiz eriþimde yönlendirilecek sayfa
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Yetkisi yetmediðinde gidilecek sayfa
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Auth/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var provider = new FileExtensionContentTypeProvider();
// .apk uzantýsýný MIME türü olarak kaydet
provider.Mappings[".apk"] = "application/vnd.android.package-archive";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

