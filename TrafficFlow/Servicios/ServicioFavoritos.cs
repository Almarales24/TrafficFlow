// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo Ruta y RutaFavorita.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de controlar el flujo de trabajo y persistencia de rutas favoritas.
    public class ServicioFavoritos
    {
        // Define la referencia a la colección de favoritos en MongoDB.
        private readonly IMongoCollection<RutaFavorita> _coleccionFavoritos;
        // Define la referencia a la colección de rutas en MongoDB.
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        // Constructor que inyecta la base de datos MongoDB para inicializar las colecciones.
        public ServicioFavoritos(ServicioMongoDB servicioMongoDB)
        {
            // Inicializa el acceso a la colección de favoritos de la base de datos.
            _coleccionFavoritos = servicioMongoDB.ObtenerColeccionFavoritos();
            // Inicializa el acceso a la colección de rutas de la base de datos.
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        // Obtiene todas las rutas marcadas como favoritas registradas en la base de datos.
        public async Task<List<RutaFavorita>> ObtenerFavoritos()
        {
            // Retorna la lista completa de favoritos sin filtros de manera asíncrona.
            return await _coleccionFavoritos.Find(_ => true).ToListAsync();
        }

        // Agrega una nueva ruta a la colección de favoritos si no está repetida.
        public async Task AgregarFavorito(RutaFavorita favorito)
        {
            // Comprueba si la ruta ya había sido guardada previamente en favoritos.
            var existe = await _coleccionFavoritos
                .Find(f => f.RutaId == favorito.RutaId)
                .FirstOrDefaultAsync();

            // Si la ruta no estaba registrada, la inserta en la colección de favoritos.
            if (existe == null)
                await _coleccionFavoritos.InsertOneAsync(favorito);
        }

        // Elimina permanentemente una ruta de los favoritos mediante su ID.
        public async Task EliminarFavorito(string id)
        {
            // Remueve el documento que coincide con el ID de favorito de forma asíncrona.
            await _coleccionFavoritos.DeleteOneAsync(f => f.Id == id);
        }

        // Revisa las rutas favoritas activas y filtra aquellas que alcancen el nivel de alerta configurado.
        public async Task<List<RutaFavorita>> VerificarAlertas()
        {
            // Recupera todas las rutas favoritas que tienen habilitada la alarma de tránsito.
            var favoritos = await _coleccionFavoritos.Find(f => f.AlertaActiva).ToListAsync();
            // Inicializa una lista vacía para alojar las alertas que se disparen.
            var alertas = new List<RutaFavorita>();

            // Evalúa el estado de tránsito de cada ruta marcada como favorita.
            foreach (var favorito in favoritos)
            {
                // Busca el estado actual en tiempo real de la ruta vinculada.
                var ruta = await _coleccionRutas.Find(r => r.Id == favorito.RutaId).FirstOrDefaultAsync();
                // Si la ruta no se encuentra configurada, omite el análisis para este elemento.
                if (ruta == null) continue;

                // Extrae el nivel de tráfico del primer tramo de la ruta como valor de referencia.
                var nivelActual = ruta.Segmentos.FirstOrDefault()?.NivelTransito ?? "bajo";

                // Dispara la alerta si el nivel de congestión cambió y coincide con la sensibilidad configurada.
                if (nivelActual != favorito.UltimoNivelTransito &&
                    nivelActual == favorito.NivelAlerta)
                {
                    // Actualiza en memoria el último nivel detectado.
                    favorito.UltimoNivelTransito = nivelActual;
                    // Prepara el comando para persistir el último nivel de tráfico medido.
                    var actualizacion = Builders<RutaFavorita>.Update
                        .Set(f => f.UltimoNivelTransito, nivelActual);
                    // Actualiza la ruta favorita en la base de datos.
                    await _coleccionFavoritos.UpdateOneAsync(f => f.Id == favorito.Id, actualizacion);
                    // Añade el favorito modificado al listado de alertas generadas.
                    alertas.Add(favorito);
                }
            }

            // Retorna la colección de alertas activas encontradas.
            return alertas;
        }
    }
}