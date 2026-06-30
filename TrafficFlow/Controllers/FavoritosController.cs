// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa el concentrador de SignalR para emitir alertas en tiempo real.
using TrafficFlow.Concentradores;
// Importa los modelos del proyecto, incluyendo el de rutas favoritas.
using TrafficFlow.Modelos;
// Importa los servicios del negocio requeridos.
using TrafficFlow.Servicios;
// Importa la biblioteca de SignalR para inyectar contextos de concentradores.
using Microsoft.AspNetCore.SignalR;

namespace TrafficFlow.Controllers
{
    // Clase controlador para gestionar las rutas favoritas personalizadas de los usuarios.
    public class FavoritosController : Controller
    {
        // Define la referencia al servicio que administra las rutas favoritas.
        private readonly ServicioFavoritos _servicioFavoritos;
        // Define la referencia al servicio de obtención del estado de tránsito general.
        private readonly ServicioTransito _servicioTransito;
        // Define la referencia al hub de SignalR para envío de alertas.
        private readonly IHubContext<ConcentradorTransito> _concentrador;

        // Constructor que inicializa los servicios inyectando sus correspondientes dependencias.
        public FavoritosController(ServicioFavoritos servicioFavoritos, ServicioTransito servicioTransito, IHubContext<ConcentradorTransito> concentrador)
        {
            // Asigna la instancia del servicio de favoritos a la propiedad interna.
            _servicioFavoritos = servicioFavoritos;
            // Asigna la instancia del servicio de tránsito a la propiedad interna.
            _servicioTransito = servicioTransito;
            // Asigna el contexto del hub de SignalR para enviar notificaciones inmediatas.
            _concentrador = concentrador;
        }

        // Carga la vista principal de favoritos con los elementos almacenados y las rutas disponibles.
        public async Task<IActionResult> Index()
        {
            // Obtiene la lista completa de las rutas que el usuario marcó como favoritas.
            var favoritos = await _servicioFavoritos.ObtenerFavoritos();
            // Obtiene el catálogo de todas las rutas de tránsito configuradas.
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            // Pasa el catálogo de rutas a la vista mediante el contenedor ViewBag.
            ViewBag.Rutas = rutas;
            // Retorna la vista principal con el listado de favoritos.
            return View(favoritos);
        }

        // Agrega una nueva ruta a la lista de favoritos de manera persistente.
        [HttpPost]
        public async Task<IActionResult> Agregar(RutaFavorita favorito)
        {
            // Registra de manera asíncrona la ruta favorita en la base de datos.
            await _servicioFavoritos.AgregarFavorito(favorito);
            // Redirige al listado de favoritos para reflejar el cambio.
            return RedirectToAction(nameof(Index));
        }

        // Elimina una ruta de la lista de favoritos.
        [HttpPost]
        public async Task<IActionResult> Eliminar(string id)
        {
            // Remueve la ruta favorita del almacenamiento usando su identificador.
            await _servicioFavoritos.EliminarFavorito(id);
            // Retorna un código HTTP 200 para confirmar la eliminación al cliente.
            return Ok();
        }

        // Verifica si existen alertas de tráfico pesado en las rutas marcadas como favoritas.
        [HttpGet]
        public async Task<IActionResult> VerificarAlertas()
        {
            // Consulta los favoritos para detectar congestiones o incidentes activos.
            var alertas = await _servicioFavoritos.VerificarAlertas();
            // Si se detecta alguna alerta en las rutas monitoreadas, notifica por SignalR.
            if (alertas.Any())
            {
                // Envía el paquete de alertas a todos los clientes activos conectados.
                await _concentrador.Clients.All.SendAsync("RecibirAlertaFavorito", alertas);
            }
            // Devuelve las alertas en formato JSON como respuesta HTTP directa.
            return Json(alertas);
        }
    }
}