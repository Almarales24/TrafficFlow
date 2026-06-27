using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioTransito
    {
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        public ServicioTransito(ServicioMongoDB servicioMongoDB)
        {
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        public async Task<List<Ruta>> ObtenerTodasLasRutas()
        {
            return await _coleccionRutas.Find(_ => true).ToListAsync();
        }

        public async Task<Ruta?> ObtenerRutaPorId(string id)
        {
            return await _coleccionRutas.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task CrearRuta(Ruta ruta)
        {
            await _coleccionRutas.InsertOneAsync(ruta);
        }

        public async Task ActualizarNivelTransito(string id, List<SegmentoTransito> segmentos)
        {
            var actualizacion = Builders<Ruta>.Update
                .Set(r => r.Segmentos, segmentos)
                .Set(r => r.ActualizadoEn, DateTime.UtcNow);

            await _coleccionRutas.UpdateOneAsync(r => r.Id == id, actualizacion);
        }

        public async Task EliminarRuta(string id)
        {
            await _coleccionRutas.DeleteOneAsync(r => r.Id == id);
        }
    }
}