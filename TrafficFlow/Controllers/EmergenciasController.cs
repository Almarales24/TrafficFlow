using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Concentradores;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    public class EmergenciasController : Controller
    {
        private readonly ServicioEmergencias _servicioEmergencias;
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        public EmergenciasController(ServicioEmergencias servicioEmergencias, IHubContext<ConcentradorTransito> concentrador)
        {
            _servicioEmergencias = servicioEmergencias;
            _concentrador = concentrador;
        }

        public async Task<IActionResult> Index()
        {
            var emergencias = await _servicioEmergencias.ObtenerEmergenciasActivas();
            return View(emergencias);
        }

        [HttpPost]
        public async Task<IActionResult> Simular(Emergencia emergencia)
        {
            var resultado = await _servicioEmergencias.SimularEmergencia(emergencia);
            await _concentrador.Clients.All.SendAsync("RecibirEmergencia", resultado);
            await _concentrador.Clients.All.SendAsync("ActualizarMapa");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Finalizar(string id)
        {
            await _servicioEmergencias.FinalizarEmergencia(id);
            return Ok();
        }
    }
}