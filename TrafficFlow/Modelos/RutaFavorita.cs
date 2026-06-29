using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class RutaFavorita
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombreRuta")]
        public string NombreRuta { get; set; } = string.Empty;

        [BsonElement("rutaId")]
        public string RutaId { get; set; } = string.Empty;

        [BsonElement("nivelAlerta")]
        public string NivelAlerta { get; set; } = "alto"; // bajo, medio, alto

        [BsonElement("alertaActiva")]
        public bool AlertaActiva { get; set; } = true;

        [BsonElement("ultimoNivelTransito")]
        public string UltimoNivelTransito { get; set; } = string.Empty;

        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}