using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    public class HistorialController : Controller
    {
        private readonly ServicioHistorial _servicioHistorial;
        private readonly ServicioTransito _servicioTransito;

        public HistorialController(ServicioHistorial servicioHistorial, ServicioTransito servicioTransito)
        {
            _servicioHistorial = servicioHistorial;
            _servicioTransito = servicioTransito;
        }

        public async Task<IActionResult> Index()
        {
            var historial = await _servicioHistorial.ObtenerHistorial();
            return View(historial);
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarPorFecha(DateTime desde, DateTime hasta)
        {
            var historial = await _servicioHistorial.ObtenerHistorialPorFecha(desde, hasta);
            return Json(historial);
        }

        [HttpGet]
        public async Task<IActionResult> FiltrarPorRuta(string nombreRuta)
        {
            var historial = await _servicioHistorial.ObtenerHistorialPorRuta(nombreRuta);
            return Json(historial);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarEstadoActual()
        {
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            await _servicioHistorial.GuardarRegistrosDesdRutas(rutas);
            return Ok(new { mensaje = "Estado guardado correctamente" });
        }
    }
}