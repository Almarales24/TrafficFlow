// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo que representa un tramo o segmento que forma parte de una ruta.
    public class SegmentoTransito
    {
        // Mapea el punto de partida físico o coordenada inicial del tramo.
        [BsonElement("desde")]
        public string Desde { get; set; } = string.Empty;

        // Mapea el punto de llegada físico o coordenada final del tramo.
        [BsonElement("hasta")]
        public string Hasta { get; set; } = string.Empty;

        // Mapea el nivel de congestión vial estimado para este tramo.
        [BsonElement("nivelTransito")]
        public string NivelTransito { get; set; } = "bajo";

        // Mapea la velocidad promedio de los vehículos en este tramo en kilómetros por hora.
        [BsonElement("velocidadKmh")]
        public double VelocidadKmh { get; set; }
    }
}