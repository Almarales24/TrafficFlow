// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo que define un incidente vial común, como accidentes u obras.
    public class Incidente
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea la clasificación del incidente (por ejemplo, accidente, vía cerrada, obras).
        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        // Mapea la descripción descriptiva del suceso vial.
        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        // Mapea la coordenada o dirección textual donde ocurrió el incidente.
        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; } = string.Empty;

        // Mapea el indicador de vigencia del incidente vial.
        [BsonElement("activo")]
        public bool Activo { get; set; } = true;

        // Mapea la fecha y hora en la que se reportó el incidente en la aplicación.
        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}