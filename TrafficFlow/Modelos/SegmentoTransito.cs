using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class SegmentoTransito
    {
        [BsonElement("desde")]
        public string Desde { get; set; } = string.Empty;

        [BsonElement("hasta")]
        public string Hasta { get; set; } = string.Empty;

        [BsonElement("nivelTransito")]
        public string NivelTransito { get; set; } = "bajo"; // bajo, medio, alto

        [BsonElement("velocidadKmh")]
        public double VelocidadKmh { get; set; }
    }
}