// Importa las utilidades del framework MVC para manejar controladores y vistas.
using Microsoft.AspNetCore.Mvc;
// Importa los modelos del proyecto para instanciar simulaciones de tráfico.
using TrafficFlow.Modelos;
// Importa los servicios del negocio, especialmente el de simulación de tránsito.
using TrafficFlow.Servicios;

namespace TrafficFlow.Controllers
{
    // Clase controlador para configurar, ejecutar y borrar simulaciones horarias del tráfico vial.
    public class SimulacionController : Controller
    {
        // Referencia al servicio de simulación que contiene las fórmulas del flujo de vehículos.
        private readonly ServicioSimulacion _servicioSimulacion;

        // Constructor que inicializa el servicio inyectando la dependencia correspondiente.
        public SimulacionController(ServicioSimulacion servicioSimulacion)
        {
            // Asigna la instancia inyectada a la variable privada correspondiente.
            _servicioSimulacion = servicioSimulacion;
        }

        // Carga la página principal del módulo de simulación con el histórico de simulaciones.
        public async Task<IActionResult> Index()
        {
            // Obtiene la lista completa de configuraciones de simulación guardadas.
            var simulaciones = await _servicioSimulacion.ObtenerSimulaciones();
            // Retorna la vista index junto con las simulaciones obtenidas.
            return View(simulaciones);
        }

        // Ejecuta un nuevo escenario de simulación basado en parámetros horarios y de congestión.
        [HttpPost]
        public async Task<IActionResult> Ejecutar(string nombre, int horarioInicio, int horarioFin, double factorCongesion)
        {
            // Crea un nuevo objeto de modelo simulacion inicializando sus propiedades.
            var simulacion = new SimulacionHorario
            {
                // Asigna el nombre o descripción descriptiva del escenario.
                Nombre = nombre,
                // Convierte la hora entera de inicio a un objeto TimeSpan.
                HorarioInicio = TimeSpan.FromHours(horarioInicio),
                // Convierte la hora entera de finalización a un objeto TimeSpan.
                HorarioFin = TimeSpan.FromHours(horarioFin),
                // Asigna la tasa de afectación o congestión de vehículos.
                FactorCongesion = factorCongesion
            };

            // Llama al servicio para correr y guardar la simulación.
            var resultado = await _servicioSimulacion.EjecutarSimulacion(simulacion);
            // Devuelve el objeto resultante en formato JSON como confirmación.
            return Json(resultado);
        }

        // Borra una simulación archivada usando su clave primaria única.
        [HttpDelete]
        public async Task<IActionResult> Eliminar(string id)
        {
            // Llama al servicio de simulación para eliminar el registro de forma permanente.
            await _servicioSimulacion.EliminarSimulacion(id);
            // Retorna una respuesta HTTP 200 de éxito.
            return Ok();
        }
    }
}