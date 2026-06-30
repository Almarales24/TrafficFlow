// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa los modelos del proyecto para la gestión de entidades de ruta.
using TrafficFlow.Modelos;
// Importa los servicios requeridos del negocio de tránsito.
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    // Clase controlador para gestionar las rutas de transporte y tránsito urbano.
    public class RutasController : Controller
    {
        // Define la referencia al servicio de tránsito.
        private readonly ServicioTransito _servicioTransito;

        // Constructor que inicializa el servicio inyectando su dependencia correspondiente.
        public RutasController(ServicioTransito servicioTransito)
        {
            // Asigna el servicio de tránsito inyectado a la variable privada.
            _servicioTransito = servicioTransito;
        }

        // Carga la página principal que muestra la lista de rutas registradas.
        public async Task<IActionResult> Index()
        {
            // Obtiene la totalidad de las rutas de tránsito almacenadas de forma asíncrona.
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            // Retorna la vista enviándole el listado completo de rutas.
            return View(rutas);
        }

        // Registra una nueva ruta de tránsito en el sistema si los datos son correctos.
        [HttpPost]
        public async Task<IActionResult> Crear(Ruta ruta)
        {
            // Valida que el modelo de la ruta sea correcto de acuerdo a las reglas declaradas.
            if (ModelState.IsValid)
            {
                // Guarda la nueva ruta en la base de datos a través del servicio de tránsito.
                await _servicioTransito.CrearRuta(ruta);
                // Redirige al usuario al listado de rutas para visualizar el cambio.
                return RedirectToAction(nameof(Index));
            }
            // Retorna la misma vista con el modelo provisto en caso de errores de validación.
            return View(ruta);
        }

        // Remueve de forma definitiva una ruta del sistema por su identificador.
        [HttpDelete]
        public async Task<IActionResult> Eliminar(string id)
        {
            // Llama al servicio de tránsito para borrar la ruta del almacenamiento.
            await _servicioTransito.EliminarRuta(id);
            // Devuelve un código HTTP 200 de éxito.
            return Ok();
        }

        // Expone el listado de todas las rutas directamente en formato JSON.
        [HttpGet]
        public async Task<IActionResult> ObtenerRutas()
        {
            // Consulta de forma asíncrona la lista de todas las rutas de tránsito.
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            // Retorna la lista en formato JSON como respuesta HTTP directa.
            return Json(rutas);
        }
    }
}