// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo RegistroTransito y Ruta.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de controlar el almacenamiento y consulta del historial de tránsito.
    public class ServicioHistorial
    {
        // Define la referencia a la colección histórica en MongoDB.
        private readonly IMongoCollection<RegistroTransito> _coleccionHistorial;

        // Constructor que inyecta la base de datos MongoDB para inicializar la colección del historial.
        public ServicioHistorial(ServicioMongoDB servicioMongoDB)
        {
            // Inicializa el acceso a la colección de historial de la base de datos.
            _coleccionHistorial = servicioMongoDB.ObtenerColeccionHistorial();
        }

        // Obtiene todos los registros del historial ordenados de manera cronológica descendente.
        public async Task<List<RegistroTransito>> ObtenerHistorial()
        {
            // Busca sin filtros y ordena los registros de más reciente a más antiguo de forma asíncrona.
            return await _coleccionHistorial
                .Find(_ => true)
                .SortByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        // Obtiene los registros históricos comprendidos dentro de un rango de fechas.
        public async Task<List<RegistroTransito>> ObtenerHistorialPorFecha(DateTime desde, DateTime hasta)
        {
            // Filtra los documentos por rango de fechas y los devuelve ordenados cronológicamente descendente.
            return await _coleccionHistorial
                .Find(r => r.FechaRegistro >= desde && r.FechaRegistro <= hasta)
                .SortByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        // Obtiene el historial de registros que corresponden a una ruta en particular.
        public async Task<List<RegistroTransito>> ObtenerHistorialPorRuta(string nombreRuta)
        {
            // Busca registros cuyo nombre coincida con el provisto y los devuelve ordenados.
            return await _coleccionHistorial
                .Find(r => r.NombreRuta == nombreRuta)
                .SortByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        // Inserta un único registro de tránsito en la base de datos de manera directa.
        public async Task GuardarRegistro(RegistroTransito registro)
        {
            // Guarda el objeto de registro de manera asíncrona.
            await _coleccionHistorial.InsertOneAsync(registro);
        }

        // Mapea y guarda múltiples registros históricos a partir de un listado de rutas activas.
        public async Task GuardarRegistrosDesdRutas(List<Ruta> rutas)
        {
            // Convierte el listado de rutas en un lote de modelos de RegistroTransito.
            var registros = rutas.Select(r => new RegistroTransito
            {
                // Asigna el nombre de la ruta para el historial.
                NombreRuta = r.Nombre,
                // Toma la congestión del primer segmento como referencia del tránsito global.
                NivelTransito = r.Segmentos.FirstOrDefault()?.NivelTransito ?? "bajo",
                // Toma la velocidad del primer segmento como referencia del flujo.
                VelocidadPromedio = r.Segmentos.FirstOrDefault()?.VelocidadKmh ?? 0,
                // Inicializa el contador de incidentes en cero por defecto.
                TotalIncidentes = 0,
                // Asigna la fecha y hora de la captura.
                FechaRegistro = DateTime.UtcNow,
                // Llama al método interno para evaluar si es una hora de alto tráfico habitual.
                HoraPico = EsHoraPico()
            }).ToList();

            // Inserta en lote la colección de registros en la base de datos de forma asíncrona.
            await _coleccionHistorial.InsertManyAsync(registros);
        }

        // Evalúa si la hora actual local corresponde a las franjas habituales de mayor afluencia vial.
        private bool EsHoraPico()
        {
            // Obtiene la hora actual del servidor.
            var hora = DateTime.Now.Hour;
            // Retorna verdadero si se encuentra entre 7 y 9 AM, o entre 5 y 7 PM.
            return (hora >= 7 && hora <= 9) || (hora >= 17 && hora <= 19);
        }
    }
}