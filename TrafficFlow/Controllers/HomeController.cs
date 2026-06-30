// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa utilidades de diagnóstico para rastrear identificadores de solicitudes.
using System.Diagnostics;
// Importa los modelos del proyecto, incluyendo el modelo de error.
using TrafficFlow.Modelos;

namespace TrafficFlow.Controllers
{
    // Clase controlador para la página de inicio predeterminada y manejo de fallos globales.
    public class HomeController : Controller
    {
        // Redirige la petición inicial hacia el controlador de Rutas.
        public IActionResult Index()
        {
            // Redirecciona al usuario a la vista principal de Rutas que muestra el mapa.
            return RedirectToAction("Index", "Rutas");
        }

        // Configura el almacenamiento en caché para que no guarde respuestas de error.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // Renderiza la vista de error con la información detallada del fallo.
        public IActionResult Error()
        {
            // Genera y pasa el modelo de error con el ID de la solicitud actual.
            return View(new ErrorViewModel
            {
                // Asigna el identificador de la actividad actual o el de la traza HTTP.
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}