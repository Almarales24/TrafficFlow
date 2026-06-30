// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo Ruta y SimulacionHorario.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de calcular e instanciar simulaciones del flujo vehicular.
    public class ServicioSimulacion
    {
        // Define la referencia a la colección de simulaciones en MongoDB.
        private readonly IMongoCollection<SimulacionHorario> _coleccionSimulaciones;
        // Define la referencia a la colección de rutas en MongoDB.
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        // Constructor que inyecta la base de datos MongoDB para inicializar las colecciones de simulación.
        public ServicioSimulacion(ServicioMongoDB servicioMongoDB)
        {
            // Inicializa el acceso a la colección de simulaciones de la base de datos.
            _coleccionSimulaciones = servicioMongoDB.ObtenerColeccionSimulaciones();
            // Inicializa el acceso a la colección de rutas de la base de datos.
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        // Obtiene la lista histórica de simulaciones ordenadas de la más reciente a la más antigua.
        public async Task<List<SimulacionHorario>> ObtenerSimulaciones()
        {
            // Realiza la búsqueda y ordenación cronológica descendente de las simulaciones.
            return await _coleccionSimulaciones
                .Find(_ => true)
                .SortByDescending(s => s.CreadoEn)
                .ToListAsync();
        }

        // Ejecuta los cálculos de simulación en base al factor de congestión indicado y guarda los resultados.
        public async Task<SimulacionHorario> EjecutarSimulacion(SimulacionHorario simulacion)
        {
            // Obtiene la totalidad de rutas registradas en el sistema para simular su comportamiento.
            var rutas = await _coleccionRutas.Find(_ => true).ToListAsync();
            // Asigna los nombres de las rutas analizadas al modelo de simulación.
            simulacion.RutasAfectadas = rutas.Select(r => r.Nombre).ToList();
            // Inicializa una lista vacía para almacenar las métricas resultantes de cada ruta.
            simulacion.Resultados = new List<ResultadoSimulacion>();

            // Calcula el impacto simulado para cada ruta de tránsito.
            foreach (var ruta in rutas)
            {
                // Toma la velocidad actual del tramo de inicio o asume 60 km/h como base.
                var velocidadBase = ruta.Segmentos.FirstOrDefault()?.VelocidadKmh ?? 60;
                // Reduce la velocidad de tránsito dividiéndola por el factor de congestión establecido.
                var velocidadSimulada = velocidadBase / simulacion.FactorCongesion;

                // Declara la variable para categorizar el nivel de embotellamiento resultante.
                string nivelSimulado;
                // Asigna nivel alto si la velocidad simulada baja a 20 km/h o menos.
                if (velocidadSimulada <= 20)
                    nivelSimulado = "alto";
                // Asigna nivel medio si la velocidad simulada se encuentra entre 21 y 40 km/h.
                else if (velocidadSimulada <= 40)
                    nivelSimulado = "medio";
                // Asigna nivel bajo para velocidades superiores a 40 km/h.
                else
                    nivelSimulado = "bajo";

                // Inserta el resultado calculado del tramo en el reporte del escenario de simulación.
                simulacion.Resultados.Add(new ResultadoSimulacion
                {
                    // Asigna el nombre de la ruta.
                    NombreRuta = ruta.Nombre,
                    // Redondea el valor de la velocidad simulada a un solo decimal.
                    VelocidadSimulada = Math.Round(velocidadSimulada, 1),
                    // Asigna la clasificación del tráfico.
                    NivelTransitoSimulado = nivelSimulado
                });
            }

            // Inserta el documento de la simulación completada en la base de datos de forma asíncrona.
            await _coleccionSimulaciones.InsertOneAsync(simulacion);
            // Retorna la simulación procesada con sus resultados detallados.
            return simulacion;
        }

        // Remueve permanentemente una simulación archivada usando su ID.
        public async Task EliminarSimulacion(string id)
        {
            // Elimina el registro correspondiente en la colección de forma asíncrona.
            await _coleccionSimulaciones.DeleteOneAsync(s => s.Id == id);
        }
    }
}