using Microsoft.EntityFrameworkCore;
using web_backend; //AppDbContexti tanýyan namespace
using web_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Servis mimarisini sisteme tanýtýyoruz
builder.Services.AddScoped<IPredictionService, PredictionService>();

builder.Services.AddScoped<IBinService, BinService>();

// 1. Veritabaný Servisini Kaydet
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<IIoTService, SimulatedIoTService>();

builder.Services.AddScoped<ITrustScoreService, TrustScoreService>();

builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddScoped<IRankingService, RankingService>();
builder.Services.AddScoped<IPointTransactionService, PointTransactionService>();

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

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
