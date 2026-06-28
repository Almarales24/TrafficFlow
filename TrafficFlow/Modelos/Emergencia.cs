using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class Emergencia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty; // terremoto, cierresMasivos, inundacion

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("nivelImpacto")]
        public string NivelImpacto { get; set; } = "alto"; // bajo, medio, alto

        [BsonElement("rutasAfectadas")]
        public List<string> RutasAfectadas { get; set; } = new();

        [BsonElement("activo")]
        public bool Activo { get; set; } = true;

        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}