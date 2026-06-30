// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo para representar la información del clima y su repercusión en la movilidad.
    public class DatoMeteorologico
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea y establece el nombre de la ciudad del reporte.
        [BsonElement("ciudad")]
        public string Ciudad { get; set; } = "Bogotá";

        // Mapea la temperatura registrada en grados centígrados.
        [BsonElement("temperatura")]
        public double Temperatura { get; set; }

        // Mapea una breve descripción textual de la condición del clima.
        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        // Mapea el porcentaje de humedad relativa del ambiente.
        [BsonElement("humedad")]
        public int Humedad { get; set; }

        // Mapea la velocidad medida del viento.
        [BsonElement("viento")]
        public double Viento { get; set; }

        // Mapea la etiqueta de la condición climatológica principal (ej. lluvia, niebla, despejado).
        [BsonElement("condicion")]
        public string Condicion { get; set; } = string.Empty;

        // Mapea la severidad estimada del impacto sobre el tránsito (bajo, medio, alto).
        [BsonElement("impactoTransito")]
        public string ImpactoTransito { get; set; } = string.Empty;

        // Mapea la fecha y hora exacta en la que se guardó el reporte.
        [BsonElement("registradoEn")]
        public DateTime RegistradoEn { get; set; } = DateTime.UtcNow;
    }
}