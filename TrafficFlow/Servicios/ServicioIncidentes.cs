// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo el modelo Incidente.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de controlar el ciclo de vida y persistencia de incidentes viales.
    public class ServicioIncidentes
    {
        // Define la referencia a la colección de incidentes en MongoDB.
        private readonly IMongoCollection<Incidente> _coleccionIncidentes;

        // Constructor que inyecta la base de datos MongoDB para inicializar la colección de incidentes.
        public ServicioIncidentes(ServicioMongoDB servicioMongoDB)
        {
            // Inicializa el acceso a la colección de incidentes de la base de datos.
            _coleccionIncidentes = servicioMongoDB.ObtenerColeccionIncidentes();
        }

        // Obtiene los incidentes viales reportados que continúan activos.
        public async Task<List<Incidente>> ObtenerIncidentesActivos()
        {
            // Filtra y retorna los documentos cuyo atributo Activo sea verdadero de forma asíncrona.
            return await _coleccionIncidentes.Find(i => i.Activo).ToListAsync();
        }

        // Inserta un nuevo reporte de incidente vial de forma persistente.
        public async Task CrearIncidente(Incidente incidente)
        {
            // Guarda el nuevo objeto de incidente en la base de datos de manera asíncrona.
            await _coleccionIncidentes.InsertOneAsync(incidente);
        }

        // Desactiva un incidente vial activo mediante su identificador.
        public async Task DesactivarIncidente(string id)
        {
            // Crea la instrucción para modificar el campo activo a falso.
            var actualizacion = Builders<Incidente>.Update.Set(i => i.Activo, false);
            // Actualiza el documento correspondiente en la base de datos usando su ID.
            await _coleccionIncidentes.UpdateOneAsync(i => i.Id == id, actualizacion);
        }
    }
}