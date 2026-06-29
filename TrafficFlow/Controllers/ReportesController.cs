using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ServicioPDF _servicioPDF;

        public ReportesController(ServicioPDF servicioPDF)
        {
            _servicioPDF = servicioPDF;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Descargar()
        {
            var bytes = await _servicioPDF.GenerarReporteTrafico();
            var nombreArchivo = $"ReporteTrafico_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(bytes, "application/pdf", nombreArchivo);
        }
    }
}