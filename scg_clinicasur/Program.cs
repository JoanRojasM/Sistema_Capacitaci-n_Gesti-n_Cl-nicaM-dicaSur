using Microsoft.EntityFrameworkCore;
using scg_clinicasur.Models;
using scg_clinicasur.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la cadena de conexi�n con el nombre "DefaultConnection"
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Soporte para sesiones
builder.Services.AddSession();

// Registro IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // A�ade autenticaci�n
app.UseAuthorization();  // A�ade autorizaci�n

app.UseSession(); // Habilitar sesi�n

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
