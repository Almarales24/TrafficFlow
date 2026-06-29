using TrafficFlow.Servicios;
using TrafficFlow.Concentradores;

var constructor = WebApplication.CreateBuilder(args);

// Servicios
constructor.Services.AddControllersWithViews();
constructor.Services.AddSignalR();
constructor.Services.AddSingleton<ServicioMongoDB>();
constructor.Services.AddScoped<ServicioTransito>();
constructor.Services.AddScoped<ServicioIncidentes>();
constructor.Services.AddScoped<ServicioEmergencias>();
constructor.Services.AddScoped<ServicioHistorial>();
constructor.Services.AddScoped<ServicioSimulacion>();
constructor.Services.AddScoped<ServicioPDF>();
constructor.Services.AddScoped<ServicioFavoritos>();
constructor.Services.AddHttpClient();
constructor.Services.AddScoped<ServicioMeteorologico>();


var aplicacion = constructor.Build();

// Middleware
if (!aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseExceptionHandler("/Home/Error");
    aplicacion.UseHsts();
}

aplicacion.UseHttpsRedirection();
aplicacion.UseStaticFiles();
aplicacion.UseRouting();
aplicacion.UseAuthorization();

// Rutas
aplicacion.MapControllerRoute(
    name: "default",
    pattern: "{controller=Rutas}/{action=Index}/{id?}");

// SignalR
aplicacion.MapHub<ConcentradorTransito>("/concentradorTransito");

aplicacion.Run();