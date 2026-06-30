// Importa los servicios de la aplicación para su posterior inyección.
using TrafficFlow.Servicios;
// Importa los concentradores de SignalR para la comunicación en tiempo real.
using TrafficFlow.Concentradores;

// Inicializa el generador de la aplicación web con los argumentos de entrada.
var constructor = WebApplication.CreateBuilder(args);

// Servicios
// Registra el soporte para controladores y vistas del patrón MVC.
constructor.Services.AddControllersWithViews();
// Habilita la funcionalidad de SignalR para comunicación bidireccional.
constructor.Services.AddSignalR();
// Registra la conexión a MongoDB como un servicio único en toda la aplicación.
constructor.Services.AddSingleton<ServicioMongoDB>();
// Registra el servicio de tránsito para gestionar el flujo vehicular actual.
constructor.Services.AddScoped<ServicioTransito>();
// Registra el servicio de incidentes para gestionar bloqueos y desvíos viales.
constructor.Services.AddScoped<ServicioIncidentes>();
// Registra el servicio de emergencias para gestionar avisos y alertas prioritarias.
constructor.Services.AddScoped<ServicioEmergencias>();
// Registra el servicio de historial para almacenar y consultar el tránsito pasado.
constructor.Services.AddScoped<ServicioHistorial>();
// Registra el servicio de simulación para proyectar el tráfico según horarios.
constructor.Services.AddScoped<ServicioSimulacion>();
// Registra el servicio de PDF para posibilitar la generación de reportes físicos.
constructor.Services.AddScoped<ServicioPDF>();
// Registra el servicio de favoritos para gestionar rutas personalizadas del usuario.
constructor.Services.AddScoped<ServicioFavoritos>();
// Registra el cliente HTTP para realizar solicitudes seguras a APIs externas.
constructor.Services.AddHttpClient();
// Registra el servicio meteorológico para consultar alertas y estado del clima.
constructor.Services.AddScoped<ServicioMeteorologico>();


// Construye la aplicación web configurando todos los servicios registrados.
var aplicacion = constructor.Build();

// Configura el comportamiento para entornos de producción.
if (!aplicacion.Environment.IsDevelopment())
{
    // Redirige los errores globales a la vista de error por defecto.
    aplicacion.UseExceptionHandler("/Home/Error");
    // Habilita el protocolo de seguridad estricta HSTS.
    aplicacion.UseHsts();
}

// Redirige automáticamente el tráfico HTTP al protocolo seguro HTTPS.
aplicacion.UseHttpsRedirection();
// Permite servir archivos estáticos como CSS, imágenes y scripts JS.
aplicacion.UseStaticFiles();
// Activa el motor de enrutamiento para procesar las rutas entrantes.
aplicacion.UseRouting();
// Aplica el control de permisos y autorización sobre los recursos expuestos.
aplicacion.UseAuthorization();

// Rutas
// Establece el patrón de rutas por defecto redirigiendo al controlador de Rutas.
aplicacion.MapControllerRoute(
    name: "default",
    pattern: "{controller=Rutas}/{action=Index}/{id?}");

// SignalR
// Mapea la dirección de comunicación web en tiempo real para el tránsito.
aplicacion.MapHub<ConcentradorTransito>("/concentradorTransito");

// Inicia el servidor y comienza a escuchar peticiones entrantes.
aplicacion.Run();