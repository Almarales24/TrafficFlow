using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioEmergencias
    {
        private readonly IMongoCollection<Emergencia> _coleccionEmergencias;
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        public ServicioEmergencias(ServicioMongoDB servicioMongoDB)
        {
            _coleccionEmergencias = servicioMongoDB.ObtenerColeccionEmergencias();
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        public async Task<List<Emergencia>> ObtenerEmergenciasActivas()
        {
            return await _coleccionEmergencias.Find(e => e.Activo).ToListAsync();
        }

        public async Task<Emergencia> SimularEmergencia(Emergencia emergencia)
        {
            var rutas = await _coleccionRutas.Find(_ => true).ToListAsync();
            emergencia.RutasAfectadas = rutas.Select(r => r.Id!).ToList();

            foreach (var ruta in rutas)
            {
                var segmentosActualizados = ruta.Segmentos.Select(s => new SegmentoTransito
                {
                    Desde = s.Desde,
                    Hasta = s.Hasta,
                    NivelTransito = "alto",
                    VelocidadKmh = 5
                }).ToList();

                var actualizacion = Builders<Ruta>.Update
                    .Set(r => r.Segmentos, segmentosActualizados)
                    .Set(r => r.ActualizadoEn, DateTime.UtcNow);

                await _coleccionRutas.UpdateOneAsync(r => r.Id == ruta.Id, actualizacion);
            }

            await _coleccionEmergencias.InsertOneAsync(emergencia);
            return emergencia;
        }

        public async Task FinalizarEmergencia(string id)
        {
            var actualizacion = Builders<Emergencia>.Update.Set(e => e.Activo, false);
            await _coleccionEmergencias.UpdateOneAsync(e => e.Id == id, actualizacion);
        }
    }
}