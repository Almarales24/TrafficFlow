// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa las clases de modelos para el manejo de tipos de datos de rutas e historial.
using TrafficFlow.Modelos;
// Importa los servicios del negocio requeridos.
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    // Clase controlador para gestionar las consultas y persistencia del historial de tránsito.
    public class HistorialController : Controller
    {
        // Define la referencia al servicio encargado del historial de tránsito.
        private readonly ServicioHistorial _servicioHistorial;
        // Define la referencia al servicio de tránsito para obtener el estado actual.
        private readonly ServicioTransito _servicioTransito;

        // Constructor que inicializa los servicios mediante inyección de dependencias.
        public HistorialController(ServicioHistorial servicioHistorial, ServicioTransito servicioTransito)
        {
            // Asigna el servicio de historial recibido a la variable privada.
            _servicioHistorial = servicioHistorial;
            // Asigna el servicio de tránsito recibido a la variable privada.
            _servicioTransito = servicioTransito;
        }

        // Carga la página principal del historial con los registros existentes.
        public async Task<IActionResult> Index()
        {
            // Consulta de forma asíncrona la lista de todos los registros de tráfico del historial.
            var historial = await _servicioHistorial.ObtenerHistorial();
            // Retorna la vista correspondiente enviándole los registros históricos obtenidos.
            return View(historial);
        }

        // Filtra los registros históricos basándose en un rango de fechas.
        [HttpGet]
        public async Task<IActionResult> FiltrarPorFecha(DateTime desde, DateTime hasta)
        {
            // Realiza la consulta del historial acotada al intervalo temporal indicado.
            var historial = await _servicioHistorial.ObtenerHistorialPorFecha(desde, hasta);
            // Devuelve el subconjunto de registros filtrados directamente en formato JSON.
            return Json(historial);
        }

        // Filtra los registros históricos basándose en el nombre de una ruta específica.
        [HttpGet]
        public async Task<IActionResult> FiltrarPorRuta(string nombreRuta)
        {
            // Obtiene todos los registros del historial que coincidan con la ruta provista.
            var historial = await _servicioHistorial.ObtenerHistorialPorRuta(nombreRuta);
            // Devuelve el subconjunto de registros filtrados en formato JSON.
            return Json(historial);
        }

        // Almacena una captura del estado actual de las rutas en el historial.
        [HttpPost]
        public async Task<IActionResult> GuardarEstadoActual()
        {
            // Obtiene la información actualizada en tiempo real de todas las rutas.
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            // Registra en el historial los datos actuales recolectados de cada ruta.
            await _servicioHistorial.GuardarRegistrosDesdRutas(rutas);
            // Retorna un código HTTP 200 con un objeto JSON indicando el éxito de la operación.
            return Ok(new { mensaje = "Estado guardado correctamente" });
        }
    }
}