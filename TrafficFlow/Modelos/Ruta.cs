// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo que representa una ruta principal de navegación con sus respectivos tramos.
    public class Ruta
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea la denominación descriptiva o nombre de la ruta.
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Mapea el punto geográfico de inicio u origen de la ruta.
        [BsonElement("origen")]
        public string Origen { get; set; } = string.Empty;

        // Mapea el punto geográfico de término o destino de la ruta.
        [BsonElement("destino")]
        public string Destino { get; set; } = string.Empty;

        // Mapea la colección ordenada de segmentos o tramos que componen esta ruta.
        [BsonElement("segmentos")]
        public List<SegmentoTransito> Segmentos { get; set; } = new();

        // Mapea la fecha y hora del último cambio registrado en el estado de la ruta.
        [BsonElement("actualizadoEn")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;
    }
}