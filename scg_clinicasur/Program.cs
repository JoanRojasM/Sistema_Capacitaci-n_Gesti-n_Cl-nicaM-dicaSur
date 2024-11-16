using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Models;
using scg_clinicasur.Data;
using scg_clinicasur.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<EmailService>();


// Configuraci�n de la cadena de conexi�n con el nombre "DefaultConnection"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Soporte para sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad de la sesi�n
    options.Cookie.HttpOnly = true; // La cookie no puede ser accedida desde JavaScript
    options.Cookie.IsEssential = true; // Hacer que la cookie de sesi�n sea esencial
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Aseg�rate de que la cookie sea segura
});

// Registro IHttpContextAccessor para acceder a la sesi�n en controladores/vistas
builder.Services.AddHttpContextAccessor();

// A�adir servicios de autenticaci�n, si est�s utilizando autenticaci�n basada en cookies
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta de login
        options.LogoutPath = "/Account/Logout"; // Ruta de logout
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Aseg�rate de que la cookie sea segura
    });

// Add services to the container (controladores y vistas)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Configuraci�n HSTS (solo en producci�n)
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Habilita autenticaci�n
app.UseAuthorization();  // Habilita autorizaci�n

app.UseSession(); // Habilitar sesiones

// Configurar las rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();