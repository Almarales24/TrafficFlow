using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class Incidente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty; // accidente, viasCerradas, obras

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; } = string.Empty;

        [BsonElement("activo")]
        public bool Activo { get; set; } = true;

        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}