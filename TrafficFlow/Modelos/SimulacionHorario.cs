using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrafficFlow.Modelos
{
    public class SimulacionHorario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("horarioInicio")]
        public TimeSpan HorarioInicio { get; set; }

        [BsonElement("horarioFin")]
        public TimeSpan HorarioFin { get; set; }

        [BsonElement("factorCongesion")]
        public double FactorCongesion { get; set; } = 1.0; // 1.0 = normal, 2.0 = doble congestión

        [BsonElement("rutasAfectadas")]
        public List<string> RutasAfectadas { get; set; } = new();

        [BsonElement("resultados")]
        public List<ResultadoSimulacion> Resultados { get; set; } = new();

        [BsonElement("creadoEn")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }

    public class ResultadoSimulacion
    {
        [BsonElement("nombreRuta")]
        public string NombreRuta { get; set; } = string.Empty;

        [BsonElement("velocidadSimulada")]
        public double VelocidadSimulada { get; set; }

        [BsonElement("nivelTransitoSimulado")]
        public string NivelTransitoSimulado { get; set; } = string.Empty;
    }
}