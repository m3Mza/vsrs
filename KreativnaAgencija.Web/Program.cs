using Microsoft.EntityFrameworkCore;
using CreativeAgency.DAL;
using CreativeAgency.Repository;
using CreativeAgency.Business;
using CreativeAgency.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using SoapCore;
using System.Text;
using System.ServiceModel.Channels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repository layer
builder.Services.AddScoped<IKorisnikRepozitorijum, KorisnikRepozitorijum>();
builder.Services.AddScoped<IProjekatRepozitorijum, ProjekatRepozitorijum>();
builder.Services.AddScoped<IZadatakRepozitorijum, ZadatakRepozitorijum>();

// Business layer
builder.Services.AddScoped<KorisnikServis>();
builder.Services.AddScoped<ProjekatServis>();
builder.Services.AddScoped<ZadatakServis>();

// SOAP Service
builder.Services.AddScoped<IProjekatSoapServis, ProjekatSoapServis>();
builder.Services.AddSoapCore();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Nalog/Prijava";
        options.LogoutPath = "/Nalog/Odjava";
        options.AccessDeniedPath = "/Nalog/PristupOdbijen";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Pocetna/Greska");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// SOAP endpoint
((IApplicationBuilder)app).UseSoapEndpoint<IProjekatSoapServis>("/ServisZaProjekte.asmx", new SoapEncoderOptions 
{ 
    MessageVersion = MessageVersion.Soap11,
    WriteEncoding = Encoding.UTF8
}, SoapSerializer.XmlSerializer);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pocetna}/{action=Pocetna}/{id?}");

app.Run();
