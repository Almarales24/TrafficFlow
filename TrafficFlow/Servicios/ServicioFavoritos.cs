using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioFavoritos
    {
        private readonly IMongoCollection<RutaFavorita> _coleccionFavoritos;
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        public ServicioFavoritos(ServicioMongoDB servicioMongoDB)
        {
            _coleccionFavoritos = servicioMongoDB.ObtenerColeccionFavoritos();
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        public async Task<List<RutaFavorita>> ObtenerFavoritos()
        {
            return await _coleccionFavoritos.Find(_ => true).ToListAsync();
        }

        public async Task AgregarFavorito(RutaFavorita favorito)
        {
            var existe = await _coleccionFavoritos
                .Find(f => f.RutaId == favorito.RutaId)
                .FirstOrDefaultAsync();

            if (existe == null)
                await _coleccionFavoritos.InsertOneAsync(favorito);
        }

        public async Task EliminarFavorito(string id)
        {
            await _coleccionFavoritos.DeleteOneAsync(f => f.Id == id);
        }

        public async Task<List<RutaFavorita>> VerificarAlertas()
        {
            var favoritos = await _coleccionFavoritos.Find(f => f.AlertaActiva).ToListAsync();
            var alertas = new List<RutaFavorita>();

            foreach (var favorito in favoritos)
            {
                var ruta = await _coleccionRutas.Find(r => r.Id == favorito.RutaId).FirstOrDefaultAsync();
                if (ruta == null) continue;

                var nivelActual = ruta.Segmentos.FirstOrDefault()?.NivelTransito ?? "bajo";

                if (nivelActual != favorito.UltimoNivelTransito &&
                    nivelActual == favorito.NivelAlerta)
                {
                    favorito.UltimoNivelTransito = nivelActual;
                    var actualizacion = Builders<RutaFavorita>.Update
                        .Set(f => f.UltimoNivelTransito, nivelActual);
                    await _coleccionFavoritos.UpdateOneAsync(f => f.Id == favorito.Id, actualizacion);
                    alertas.Add(favorito);
                }
            }

            return alertas;
        }
    }
}