using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Concentradores;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    public class IncidentesController : Controller
    {
        private readonly ServicioIncidentes _servicioIncidentes;
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        public IncidentesController(ServicioIncidentes servicioIncidentes, IHubContext<ConcentradorTransito> concentrador)
        {
            _servicioIncidentes = servicioIncidentes;
            _concentrador = concentrador;
        }

        public async Task<IActionResult> Index()
        {
            var incidentes = await _servicioIncidentes.ObtenerIncidentesActivos();
            return View(incidentes);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Incidente incidente)
        {
            await _servicioIncidentes.CrearIncidente(incidente);
            await _concentrador.Clients.All.SendAsync("RecibirIncidente", incidente);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Desactivar(string id)
        {
            await _servicioIncidentes.DesactivarIncidente(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerActivos()
        {
            var incidentes = await _servicioIncidentes.ObtenerIncidentesActivos();
            return Json(incidentes);
        }
    }
}