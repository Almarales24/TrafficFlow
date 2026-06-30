// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa los servicios del negocio, especialmente el generador de PDF.
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    // Clase controlador encargada de la renderización y exportación de reportes de tráfico en formato PDF.
    public class ReportesController : Controller
    {
        // Define la referencia al servicio de creación de archivos PDF.
        private readonly ServicioPDF _servicioPDF;

        // Constructor que inyecta la instancia del servicio PDF mediante dependencias.
        public ReportesController(ServicioPDF servicioPDF)
        {
            // Asigna la instancia inyectada a la propiedad privada correspondiente.
            _servicioPDF = servicioPDF;
        }

        // Carga la página principal para visualizar las opciones de reporte.
        public IActionResult Index()
        {
            // Retorna la vista correspondiente del módulo de reportes.
            return View();
        }

        // Descarga el reporte consolidado de tráfico en formato de archivo PDF.
        [HttpGet]
        public async Task<IActionResult> Descargar()
        {
            // Llama al servicio de PDF para generar el archivo binario del reporte de tráfico.
            var bytes = await _servicioPDF.GenerarReporteTrafico();
            // Construye un nombre dinámico para el archivo usando la fecha y hora actuales.
            var nombreArchivo = $"ReporteTrafico_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            // Retorna el archivo PDF adjunto en la respuesta HTTP para su descarga.
            return File(bytes, "application/pdf", nombreArchivo);
        }
    }
}