// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa el concentrador de SignalR para notificar a los clientes en tiempo real.
using TrafficFlow.Concentradores;
// Importa las clases de modelos, específicamente las de emergencias.
using TrafficFlow.Modelos;
// Importa el espacio de nombres de los servicios del negocio.
using TrafficFlow.Servicios;
// Importa las funcionalidades básicas para el uso del contexto de hubs de SignalR.
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    // Clase controlador para administrar y coordinar las vistas y acciones de emergencia.
    public class EmergenciasController : Controller
    {
        // Define la referencia al servicio de persistencia y lógica de emergencias.
        private readonly ServicioEmergencias _servicioEmergencias;
        // Define la referencia al hub de SignalR para envío de mensajes en tiempo real.
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        // Constructor que inicializa los servicios requeridos mediante inyección de dependencias.
        public EmergenciasController(ServicioEmergencias servicioEmergencias, IHubContext<ConcentradorTransito> concentrador)
        {
            // Asigna el servicio de emergencias recibido a la variable privada.
            _servicioEmergencias = servicioEmergencias;
            // Asigna la interfaz del hub de SignalR recibida a la variable privada.
            _concentrador = concentrador;
        }

        // Recupera la lista de emergencias activas y devuelve la vista correspondiente.
        public async Task<IActionResult> Index()
        {
            // Consulta de forma asíncrona todas las alertas de emergencias que siguen activas.
            var emergencias = await _servicioEmergencias.ObtenerEmergenciasActivas();
            // Envía la lista de emergencias a la vista index para su renderizado.
            return View(emergencias);
        }

        // Procesa la creación y simulación de un nuevo reporte de emergencia vial.
        [HttpPost]
        public async Task<IActionResult> Simular(Emergencia emergencia)
        {
            // Registra y procesa los datos del incidente de emergencia en la base de datos.
            var resultado = await _servicioEmergencias.SimularEmergencia(emergencia);
            // Informa sobre la nueva emergencia a todos los clientes web conectados por sockets.
            await _concentrador.Clients.All.SendAsync("RecibirEmergencia", resultado);
            // Envía una señal de actualización general a los mapas de los clientes en tiempo real.
            await _concentrador.Clients.All.SendAsync("ActualizarMapa");
            // Redirecciona el flujo del navegador a la acción principal para mostrar el listado.
            return RedirectToAction(nameof(Index));
        }

        // Permite marcar una emergencia existente como resuelta o finalizada.
        [HttpPost]
        public async Task<IActionResult> Finalizar(string id)
        {
            // Modifica el estado del registro de la emergencia para marcarla como terminada.
            await _servicioEmergencias.FinalizarEmergencia(id);
            // Retorna un código HTTP 200 de éxito como respuesta al cliente.
            return Ok();
        }
    }
}