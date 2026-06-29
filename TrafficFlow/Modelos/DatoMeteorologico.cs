using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class DatoMeteorologico
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ciudad")]
        public string Ciudad { get; set; } = "Bogotá";

        [BsonElement("temperatura")]
        public double Temperatura { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("humedad")]
        public int Humedad { get; set; }

        [BsonElement("viento")]
        public double Viento { get; set; }

        [BsonElement("condicion")]
        public string Condicion { get; set; } = string.Empty; // lluvia, niebla, despejado, tormenta

        [BsonElement("impactoTransito")]
        public string ImpactoTransito { get; set; } = string.Empty; // bajo, medio, alto

        [BsonElement("registradoEn")]
        public DateTime RegistradoEn { get; set; } = DateTime.UtcNow;
    }
}