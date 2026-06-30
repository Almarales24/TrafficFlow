// Importa las utilidades básicas de serialización BSON para MongoDB.
using MongoDB.Bson;
// Importa los atributos personalizados para el mapeo de colecciones y campos en MongoDB.
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    // Clase modelo para guardar escenarios y parámetros de simulación horaria de tránsito.
    public class SimulacionHorario
    {
        // Define el identificador único del registro mapeado como ObjectId de MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea la etiqueta descriptiva o nombre del escenario de simulación.
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Mapea el momento inicial o de arranque del periodo simulado.
        [BsonElement("horarioInicio")]
        public TimeSpan HorarioInicio { get; set; }

        // Mapea el momento de término o finalización del periodo simulado.
        [BsonElement("horarioFin")]
        public TimeSpan HorarioFin { get; set; }

        // Mapea el multiplicador que incrementa o reduce la congestión de autos (por defecto 1.0).
        [BsonElement("factorCongesion")]
        public double FactorCongesion { get; set; } = 1.0;

        // Mapea la lista de los identificadores de rutas viales simuladas.
        [BsonElement("rutasAfectadas")]
        public List<string> RutasAfectadas { get; set; } = new();

        // Mapea los resultados calculados resultantes de correr la simulación.
        [BsonElement("resultados")]
        public List<ResultadoSimulacion> Resultados { get; set; } = new();

        // Mapea la fecha y hora de creación y registro del escenario.
        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }

    // Clase auxiliar que almacena los resultados calculados por ruta dentro del escenario.
    public class ResultadoSimulacion
    {
        // Mapea el nombre de la ruta donde se calculó el impacto.
        [BsonElement("nombreRuta")]
        public string NombreRuta { get; set; } = string.Empty;

        // Mapea el valor de la velocidad simulada resultante en kilómetros por hora.
        [BsonElement("velocidadSimulada")]
        public double VelocidadSimulada { get; set; }

        // Mapea el nivel de congestión estimado resultante.
        [BsonElement("nivelTransitoSimulado")]
        public string NivelTransitoSimulado { get; set; } = string.Empty;
    }
}