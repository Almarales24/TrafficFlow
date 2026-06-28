using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioIncidentes
    {
        private readonly IMongoCollection<Incidente> _coleccionIncidentes;

        public ServicioIncidentes(ServicioMongoDB servicioMongoDB)
        {
            _coleccionIncidentes = servicioMongoDB.ObtenerColeccionIncidentes();
        }

        public async Task<List<Incidente>> ObtenerIncidentesActivos()
        {
            return await _coleccionIncidentes.Find(i => i.Activo).ToListAsync();
        }

        public async Task CrearIncidente(Incidente incidente)
        {
            await _coleccionIncidentes.InsertOneAsync(incidente);
        }

        public async Task DesactivarIncidente(string id)
        {
            var actualizacion = Builders<Incidente>.Update.Set(i => i.Activo, false);
            await _coleccionIncidentes.UpdateOneAsync(i => i.Id == id, actualizacion);
        }
    }
}