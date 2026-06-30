// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa el concentrador de SignalR para notificar cambios climáticos en tiempo real.
using TrafficFlow.Concentradores;
// Importa los servicios del negocio, en especial el servicio meteorológico.
using TrafficFlow.Servicios;
// Importa la biblioteca de SignalR para inyectar contextos de concentradores de sockets.
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    // Clase controlador para la visualización y actualización de las condiciones climáticas.
    public class MeteorologiaController : Controller
    {
        // Define la referencia al servicio meteorológico para obtención de datos del clima.
        private readonly ServicioMeteorologico _servicioMeteorologico;
        // Define la referencia al hub de SignalR para envío de eventos de clima.
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        // Constructor que inicializa los servicios mediante inyección de dependencias.
        public MeteorologiaController(ServicioMeteorologico servicioMeteorologico, IHubContext<ConcentradorTransito> concentrador)
        {
            // Asigna la instancia del servicio meteorológico a la propiedad privada.
            _servicioMeteorologico = servicioMeteorologico;
            // Asigna el contexto del concentrador de SignalR recibido a la propiedad privada.
            _concentrador = concentrador;
        }

        // Carga la página principal del clima con la información actual e histórica.
        public async Task<IActionResult> Index()
        {
            // Obtiene de forma asíncrona la información meteorológica más reciente de la API/BD.
            var clima = await _servicioMeteorologico.ObtenerClimaActual();
            // Obtiene los datos climatológicos registrados anteriormente en el sistema.
            var historial = await _servicioMeteorologico.ObtenerHistorialMeteo();
            // Envía la lista del historial del clima a la vista usando ViewBag.
            ViewBag.Historial = historial;
            // Retorna la vista enviando los datos del clima actual como modelo principal.
            return View(clima);
        }

        // Fuerza la recarga de datos meteorológicos y los notifica a los usuarios conectados.
        [HttpGet]
        public async Task<IActionResult> ActualizarClima()
        {
            // Consulta los nuevos datos del clima y actualiza el almacenamiento local.
            var clima = await _servicioMeteorologico.ObtenerClimaActual();
            // Emite la actualización meteorológica a todos los clientes web en tiempo real.
            await _concentrador.Clients.All.SendAsync("RecibirClima", clima);
            // Retorna la respuesta con el nuevo clima en formato JSON.
            return Json(clima);
        }
    }
}