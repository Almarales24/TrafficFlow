// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa el concentrador de SignalR para realizar notificaciones viales dinámicas.
using TrafficFlow.Concentradores;
// Importa los modelos del proyecto, incluyendo la definición de incidente vial.
using TrafficFlow.Modelos;
// Importa los servicios correspondientes a la lógica de negocio.
using TrafficFlow.Servicios;
// Importa la biblioteca de SignalR para el manejo de contextos de comunicación en tiempo real.
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    // Clase controlador para el reporte, consulta y desactivación de incidentes de tráfico.
    public class IncidentesController : Controller
    {
        // Define la referencia al servicio de persistencia y lógica de incidentes.
        private readonly ServicioIncidentes _servicioIncidentes;
        // Define la referencia al hub de SignalR para notificar a los clientes conectados.
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        // Constructor que inyecta los servicios correspondientes mediante dependencias.
        public IncidentesController(ServicioIncidentes servicioIncidentes, IHubContext<ConcentradorTransito> concentrador)
        {
            // Asigna la instancia del servicio de incidentes a la propiedad correspondiente.
            _servicioIncidentes = servicioIncidentes;
            // Asigna el contexto del concentrador de SignalR para emitir eventos.
            _concentrador = concentrador;
        }

        // Carga la página principal de incidentes mostrando la lista activa.
        public async Task<IActionResult> Index()
        {
            // Consulta de forma asíncrona la lista de incidentes que continúan activos.
            var incidentes = await _servicioIncidentes.ObtenerIncidentesActivos();
            // Retorna la vista enviando los incidentes activos recuperados.
            return View(incidentes);
        }

        // Registra un nuevo incidente vial en el sistema y avisa por sockets.
        [HttpPost]
        public async Task<IActionResult> Crear(Incidente incidente)
        {
            // Inserta de forma asíncrona el nuevo reporte de incidente en el almacenamiento.
            await _servicioIncidentes.CrearIncidente(incidente);
            // Envía los datos del nuevo incidente a todos los clientes web en tiempo real.
            await _concentrador.Clients.All.SendAsync("RecibirIncidente", incidente);
            // Redirecciona el navegador al listado principal de incidentes.
            return RedirectToAction(nameof(Index));
        }

        // Desactiva un incidente vial activo mediante su identificador.
        [HttpPost]
        public async Task<IActionResult> Desactivar(string id)
        {
            // Cambia el estado del incidente especificado para marcarlo como inactivo.
            await _servicioIncidentes.DesactivarIncidente(id);
            // Devuelve un código HTTP 200 de éxito al cliente de la petición.
            return Ok();
        }

        // Expone el listado de incidentes activos directamente en formato JSON.
        [HttpGet]
        public async Task<IActionResult> ObtenerActivos()
        {
            // Consulta de forma asíncrona los reportes de incidentes activos.
            var incidentes = await _servicioIncidentes.ObtenerIncidentesActivos();
            // Retorna la lista en formato JSON como respuesta directa del endpoint HTTP.
            return Json(incidentes);
        }
    }
}