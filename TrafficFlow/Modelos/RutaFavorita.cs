// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo para guardar la preferencia y nivel de alerta de una ruta elegida por el usuario.
    public class RutaFavorita
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea la denominación descriptiva de la ruta guardada.
        [BsonElement("nombreRuta")]
        public string NombreRuta { get; set; } = string.Empty;

        // Mapea el identificador de la ruta vinculada en la base de datos.
        [BsonElement("rutaId")]
        public string RutaId { get; set; } = string.Empty;

        // Mapea el nivel de congestión mínimo para disparar alertas (bajo, medio, alto).
        [BsonElement("nivelAlerta")]
        public string NivelAlerta { get; set; } = "alto";

        // Mapea el estado del interruptor de notificaciones de alerta para esta ruta.
        [BsonElement("alertaActiva")]
        public bool AlertaActiva { get; set; } = true;

        // Mapea la última medición del nivel de congestión guardada en el sistema.
        [BsonElement("ultimoNivelTransito")]
        public string UltimoNivelTransito { get; set; } = string.Empty;

        // Mapea la fecha y hora de creación de la ruta favorita.
        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}