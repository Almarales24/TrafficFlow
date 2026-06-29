using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioHistorial
    {
        private readonly IMongoCollection<RegistroTransito> _coleccionHistorial;

        public ServicioHistorial(ServicioMongoDB servicioMongoDB)
        {
            _coleccionHistorial = servicioMongoDB.ObtenerColeccionHistorial();
        }

        public async Task<List<RegistroTransito>> ObtenerHistorial()
        {
            return await _coleccionHistorial
                .Find(_ => true)
                .SortByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        public async Task<List<RegistroTransito>> ObtenerHistorialPorFecha(DateTime desde, DateTime hasta)
        {
            return await _coleccionHistorial
                .Find(r => r.FechaRegistro >= desde && r.FechaRegistro <= hasta)
                .SortByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        public async Task<List<RegistroTransito>> ObtenerHistorialPorRuta(string nombreRuta)
        {
            return await _coleccionHistorial
                .Find(r => r.NombreRuta == nombreRuta)
                .SortByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        public async Task GuardarRegistro(RegistroTransito registro)
        {
            await _coleccionHistorial.InsertOneAsync(registro);
        }

        public async Task GuardarRegistrosDesdRutas(List<Ruta> rutas)
        {
            var registros = rutas.Select(r => new RegistroTransito
            {
                NombreRuta = r.Nombre,
                NivelTransito = r.Segmentos.FirstOrDefault()?.NivelTransito ?? "bajo",
                VelocidadPromedio = r.Segmentos.FirstOrDefault()?.VelocidadKmh ?? 0,
                TotalIncidentes = 0,
                FechaRegistro = DateTime.UtcNow,
                HoraPico = EsHoraPico()
            }).ToList();

            await _coleccionHistorial.InsertManyAsync(registros);
        }

        private bool EsHoraPico()
        {
            var hora = DateTime.Now.Hour;
            return (hora >= 7 && hora <= 9) || (hora >= 17 && hora <= 19);
        }
    }
}