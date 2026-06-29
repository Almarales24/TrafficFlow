using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Concentradores;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly ServicioFavoritos _servicioFavoritos;
        private readonly ServicioTransito _servicioTransito;
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        public FavoritosController(ServicioFavoritos servicioFavoritos, ServicioTransito servicioTransito, IHubContext<ConcentradorTransito> concentrador)
        {
            _servicioFavoritos = servicioFavoritos;
            _servicioTransito = servicioTransito;
            _concentrador = concentrador;
        }

        public async Task<IActionResult> Index()
        {
            var favoritos = await _servicioFavoritos.ObtenerFavoritos();
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            ViewBag.Rutas = rutas;
            return View(favoritos);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(RutaFavorita favorito)
        {
            await _servicioFavoritos.AgregarFavorito(favorito);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(string id)
        {
            await _servicioFavoritos.EliminarFavorito(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> VerificarAlertas()
        {
            var alertas = await _servicioFavoritos.VerificarAlertas();
            if (alertas.Any())
            {
                await _concentrador.Clients.All.SendAsync("RecibirAlertaFavorito", alertas);
            }
            return Json(alertas);
        }
    }
}