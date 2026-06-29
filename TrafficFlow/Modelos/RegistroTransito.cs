using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class RegistroTransito
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombreRuta")]
        public string NombreRuta { get; set; } = string.Empty;

        [BsonElement("nivelTransito")]
        public string NivelTransito { get; set; } = string.Empty; // bajo, medio, alto

        [BsonElement("velocidadPromedio")]
        public double VelocidadPromedio { get; set; }

        [BsonElement("totalIncidentes")]
        public int TotalIncidentes { get; set; }

        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [BsonElement("horaPico")]
        public bool HoraPico { get; set; } = false;
    }
}