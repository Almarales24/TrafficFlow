using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioSimulacion
    {
        private readonly IMongoCollection<SimulacionHorario> _coleccionSimulaciones;
        private readonly IMongoCollection<Ruta> _coleccionRutas;

        public ServicioSimulacion(ServicioMongoDB servicioMongoDB)
        {
            _coleccionSimulaciones = servicioMongoDB.ObtenerColeccionSimulaciones();
            _coleccionRutas = servicioMongoDB.ObtenerColeccionRutas();
        }

        public async Task<List<SimulacionHorario>> ObtenerSimulaciones()
        {
            return await _coleccionSimulaciones
                .Find(_ => true)
                .SortByDescending(s => s.CreadoEn)
                .ToListAsync();
        }

        public async Task<SimulacionHorario> EjecutarSimulacion(SimulacionHorario simulacion)
        {
            var rutas = await _coleccionRutas.Find(_ => true).ToListAsync();
            simulacion.RutasAfectadas = rutas.Select(r => r.Nombre).ToList();
            simulacion.Resultados = new List<ResultadoSimulacion>();

            foreach (var ruta in rutas)
            {
                var velocidadBase = ruta.Segmentos.FirstOrDefault()?.VelocidadKmh ?? 60;
                var velocidadSimulada = velocidadBase / simulacion.FactorCongesion;

                string nivelSimulado;
                if (velocidadSimulada <= 20)
                    nivelSimulado = "alto";
                else if (velocidadSimulada <= 40)
                    nivelSimulado = "medio";
                else
                    nivelSimulado = "bajo";

                simulacion.Resultados.Add(new ResultadoSimulacion
                {
                    NombreRuta = ruta.Nombre,
                    VelocidadSimulada = Math.Round(velocidadSimulada, 1),
                    NivelTransitoSimulado = nivelSimulado
                });
            }

            await _coleccionSimulaciones.InsertOneAsync(simulacion);
            return simulacion;
        }

        public async Task EliminarSimulacion(string id)
        {
            await _coleccionSimulaciones.DeleteOneAsync(s => s.Id == id);
        }
    }
}