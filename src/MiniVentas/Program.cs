using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using MiniVentas.Data;
using MiniVentas.Models;
using MiniVentas.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<ConexionBD>();
builder.Services.AddScoped<UsuarioDao>();
builder.Services.AddScoped<ProductoDao>();
builder.Services.AddScoped<ClienteDao>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ValidacionCookie>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Acceso/IniciarSesion";
        opciones.AccessDeniedPath = "/Acceso/Denegado";
        opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
        opciones.EventsType = typeof(ValidacionCookie);
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dao = scope.ServiceProvider.GetRequiredService<UsuarioDao>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
    await dao.AsegurarUsuarioAdmin(hasher);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
