// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo Ruta y Emergencia.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de controlar el flujo de trabajo y persistencia de emergencias.
    public class ServicioEmergencias
    {
        // Define la referencia a la colección de emergencias en MongoDB.
        private readonly IMongoCollection<Emergencia> _coleccionEmergencias;
        // Define la referencia a la colección de rutas en MongoDB.
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        // Constructor que inyecta la base de datos MongoDB para inicializar las colecciones.
        public ServicioEmergencias(ServicioMongoDB servicioMongoDB)
        {
            // Inicializa el acceso a la colección de emergencias de la base de datos.
            _coleccionEmergencias = servicioMongoDB.ObtenerColeccionEmergencias();
            // Inicializa el acceso a la colección de rutas de la base de datos.
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        // Obtiene las alertas de emergencia que se encuentran activas en el sistema.
        public async Task<List<Emergencia>> ObtenerEmergenciasActivas()
        {
            // Filtra y retorna las emergencias cuyo atributo Activo sea verdadero de forma asíncrona.
            return await _coleccionEmergencias.Find(e => e.Activo).ToListAsync();
        }

        // Registra una emergencia simulada afectando drásticamente el tránsito de todas las rutas.
        public async Task<Emergencia> SimularEmergencia(Emergencia emergencia)
        {
            // Obtiene la lista completa de todas las rutas del sistema.
            var rutas = await _coleccionRutas.Find(_ => true).ToListAsync();
            // Asocia todas las rutas existentes al listado de rutas afectadas de la emergencia.
            emergencia.RutasAfectadas = rutas.Select(r => r.Id!).ToList();

            // Itera a través de cada ruta para colapsar su tránsito a nivel alto.
            foreach (var ruta in rutas)
            {
                // Crea una copia de los segmentos con tránsito alto y velocidad mínima.
                var segmentosActualizados = ruta.Segmentos.Select(s => new SegmentoTransito
                {
                    // Mantiene el punto de inicio del tramo original.
                    Desde = s.Desde,
                    // Mantiene el punto de fin del tramo original.
                    Hasta = s.Hasta,
                    // Establece congestión máxima.
                    NivelTransito = "alto",
                    // Reduce la velocidad a 5 km/h debido a la emergencia.
                    VelocidadKmh = 5
                }).ToList();

                // Define la actualización de segmentos y fecha de modificación de la ruta.
                var actualizacion = Builders<Ruta>.Update
                    .Set(r => r.Segmentos, segmentosActualizados)
                    .Set(r => r.ActualizadoEn, DateTime.UtcNow);

                // Aplica los cambios a la ruta en la base de datos de manera asíncrona.
                await _coleccionRutas.UpdateOneAsync(r => r.Id == ruta.Id, actualizacion);
            }

            // Inserta el nuevo registro de emergencia en la base de datos.
            await _coleccionEmergencias.InsertOneAsync(emergencia);
            // Retorna la emergencia creada con los datos procesados.
            return emergencia;
        }

        // Finaliza una emergencia activa cambiando su estado.
        public async Task FinalizarEmergencia(string id)
        {
            // Define el cambio de estado del campo activo a falso.
            var actualizacion = Builders<Emergencia>.Update.Set(e => e.Activo, false);
            // Actualiza el documento correspondiente en la base de datos usando su ID.
            await _coleccionEmergencias.UpdateOneAsync(e => e.Id == id, actualizacion);
        }
    }
}