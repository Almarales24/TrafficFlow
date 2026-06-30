// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo para guardar instantáneas históricas del tráfico en una ruta determinada.
    public class RegistroTransito
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea la denominación o nombre descriptivo de la ruta evaluada.
        [BsonElement("nombreRuta")]
        public string NombreRuta { get; set; } = string.Empty;

        // Mapea la clasificación del tráfico general detectado (bajo, medio, alto).
        [BsonElement("nivelTransito")]
        public string NivelTransito { get; set; } = string.Empty;

        // Mapea el valor numérico de la velocidad media calculada en kilómetros por hora.
        [BsonElement("velocidadPromedio")]
        public double VelocidadPromedio { get; set; }

        // Mapea el conteo de incidentes activos reportados sobre esta ruta al momento de la captura.
        [BsonElement("totalIncidentes")]
        public int TotalIncidentes { get; set; }

        // Mapea la fecha y hora en que se levantó y guardó la instantánea de datos.
        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Mapea el indicador booleano de si la medición coincide con una franja de hora pico.
        [BsonElement("horaPico")]
        public bool HoraPico { get; set; } = false;
    }
}