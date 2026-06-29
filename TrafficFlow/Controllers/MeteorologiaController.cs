using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Concentradores;
using TrafficFlow.Servicios;
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    public class MeteorologiaController : Controller
    {
        private readonly ServicioMeteorologico _servicioMeteorologico;
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        public MeteorologiaController(ServicioMeteorologico servicioMeteorologico, IHubContext<ConcentradorTransito> concentrador)
        {
            _servicioMeteorologico = servicioMeteorologico;
            _concentrador = concentrador;
        }

        public async Task<IActionResult> Index()
        {
            var clima = await _servicioMeteorologico.ObtenerClimaActual();
            var historial = await _servicioMeteorologico.ObtenerHistorialMeteo();
            ViewBag.Historial = historial;
            return View(clima);
        }

        [HttpGet]
        public async Task<IActionResult> ActualizarClima()
        {
            var clima = await _servicioMeteorologico.ObtenerClimaActual();
            await _concentrador.Clients.All.SendAsync("RecibirClima", clima);
            return Json(clima);
        }
    }
}