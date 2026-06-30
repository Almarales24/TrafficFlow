// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo Ruta y SegmentoTransito.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de gestionar las operaciones CRUD y de estado de las rutas de tránsito.
    public class ServicioTransito
    {
        // Define la referencia a la colección de rutas en MongoDB.
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        // Constructor que inyecta la base de datos MongoDB para inicializar la colección de rutas.
        public ServicioTransito(ServicioMongoDB servicioMongoDB)
        {
            // Inicializa el acceso a la colección de rutas de la base de datos.
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        // Obtiene el catálogo completo de rutas de tránsito disponibles.
        public async Task<List<Ruta>> ObtenerTodasLasRutas()
        {
            // Realiza una búsqueda sin filtros de forma asíncrona devolviendo la lista completa.
            return await _coleccionRutas.Find(_ => true).ToListAsync();
        }

        // Busca una única ruta de tránsito a partir de su identificador.
        public async Task<Ruta?> ObtenerRutaPorId(string id)
        {
            // Filtra por ID de ruta y retorna el primer documento encontrado o nulo.
            return await _coleccionRutas.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        // Inserta una nueva ruta de tránsito en la base de datos de manera persistente.
        public async Task CrearRuta(Ruta ruta)
        {
            // Guarda el nuevo objeto de ruta de forma asíncrona.
            await _coleccionRutas.InsertOneAsync(ruta);
        }

        // Actualiza los segmentos y la fecha de última modificación de una ruta específica.
        public async Task ActualizarNivelTransito(string id, List<SegmentoTransito> segmentos)
        {
            // Prepara las instrucciones de modificación para los campos de segmentos y fecha.
            var actualizacion = Builders<Ruta>.Update
                .Set(r => r.Segmentos, segmentos)
                .Set(r => r.ActualizadoEn, DateTime.UtcNow);

            // Ejecuta la actualización sobre la ruta que coincide con el ID provisto.
            await _coleccionRutas.UpdateOneAsync(r => r.Id == id, actualizacion);
        }

        // Remueve permanentemente una ruta de tránsito del sistema por su identificador.
        public async Task EliminarRuta(string id)
        {
            // Borra de manera asíncrona la ruta que coincide con el ID.
            await _coleccionRutas.DeleteOneAsync(r => r.Id == id);
        }
    }
}