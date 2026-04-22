using System.Net.Http.Headers;
using HospitalManagement_BvDKAnViet.Data.Context;
using HospitalManagement_BvDKAnViet.WepApp.Services;
using HospitalManagement_BvDKAnViet.WepApp.Services.Handlers;
using HospitalManagement_BvDKAnViet.WepApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add distributed cache required by session
builder.Services.AddDistributedMemoryCache();

// Add session and HttpContext accessor
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Register DI
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7143");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<AuthTokenHandler>();

// Register EF Core DbContext (SQL Server using connection string in appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware (must be between UseRouting and endpoint middleware)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
