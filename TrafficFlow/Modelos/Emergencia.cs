// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo que define una alerta de emergencia vial de alto impacto.
    public class Emergencia
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea la clasificación del incidente (por ejemplo, terremoto, inundación).
        [BsonElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        // Mapea la descripción textual detallada de la emergencia vial.
        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        // Mapea la severidad de la alerta, que por defecto es alta.
        [BsonElement("nivelImpacto")]
        public string NivelImpacto { get; set; } = "alto";

        // Mapea la lista de los identificadores de rutas viales comprometidas.
        [BsonElement("rutasAfectadas")]
        public List<string> RutasAfectadas { get; set; } = new();

        // Mapea el indicador de vigencia de la emergencia.
        [BsonElement("activo")]
        public bool Activo { get; set; } = true;

        // Mapea la fecha y hora de creación del registro.
        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}