using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class Ruta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("origen")]
        public string Origen { get; set; } = string.Empty;

        [BsonElement("destino")]
        public string Destino { get; set; } = string.Empty;

        [BsonElement("segmentos")]
        public List<SegmentoTransito> Segmentos { get; set; } = new();

        [BsonElement("actualizadoEn")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;
    }
}