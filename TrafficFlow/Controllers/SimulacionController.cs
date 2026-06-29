using Microsoft.AspNetCore.Mvc;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    public class SimulacionController : Controller
    {
        private readonly ServicioSimulacion _servicioSimulacion;

        public SimulacionController(ServicioSimulacion servicioSimulacion)
        {
            _servicioSimulacion = servicioSimulacion;
        }

        public async Task<IActionResult> Index()
        {
            var simulaciones = await _servicioSimulacion.ObtenerSimulaciones();
            return View(simulaciones);
        }

        [HttpPost]
        public async Task<IActionResult> Ejecutar(string nombre, int horarioInicio, int horarioFin, double factorCongesion)
        {
            var simulacion = new SimulacionHorario
            {
                Nombre = nombre,
                HorarioInicio = TimeSpan.FromHours(horarioInicio),
                HorarioFin = TimeSpan.FromHours(horarioFin),
                FactorCongesion = factorCongesion
            };

            var resultado = await _servicioSimulacion.EjecutarSimulacion(simulacion);
            return Json(resultado);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(string id)
        {
            await _servicioSimulacion.EliminarSimulacion(id);
            return Ok();
        }
    }
}