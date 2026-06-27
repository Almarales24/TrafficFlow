using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    public class RutasController : Controller
    {
        private readonly ServicioTransito _servicioTransito;

        public RutasController(ServicioTransito servicioTransito)
        {
            _servicioTransito = servicioTransito;
        }

        public async Task<IActionResult> Index()
        {
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            return View(rutas);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Ruta ruta)
        {
            if (ModelState.IsValid)
            {
                await _servicioTransito.CrearRuta(ruta);
                return RedirectToAction(nameof(Index));
            }
            return View(ruta);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(string id)
        {
            await _servicioTransito.EliminarRuta(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRutas()
        {
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            return Json(rutas);
        }
    }
}