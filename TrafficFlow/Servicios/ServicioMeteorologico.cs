// Importa el driver de MongoDB necesario para operaciones de base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto, incluyendo DatoMeteorologico.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio que administra la consulta, persistencia e impacto vial del clima.
    public class ServicioMeteorologico
    {
        // Define la referencia a la colección de datos meteorológicos en MongoDB.
        private readonly IMongoCollection<DatoMeteorologico> _coleccionMeteo;
        // Define el cliente HTTP para realizar consultas a la API de clima externa.
        private readonly HttpClient _httpClient;
        // Llave de acceso configurada para autenticarse contra el servicio OpenWeatherMap.
        private const string ApiKey = "TU_API_KEY_OPENWEATHER";
        // Define el nombre de la ciudad fija para la consulta de información del clima.
        private const string Ciudad = "Bogota,CO";

        // Constructor que inyecta la conexión a MongoDB y la fábrica de clientes HTTP.
        public ServicioMeteorologico(ServicioMongoDB servicioMongoDB, IHttpClientFactory httpClientFactory)
        {
            // Obtiene el puntero a la colección meteorológica en MongoDB.
            _coleccionMeteo = servicioMongoDB.ObtenerColeccionMeteorologica();
            // Inicializa una instancia reutilizable del cliente de red HTTP.
            _httpClient = httpClientFactory.CreateClient();
        }

        // Consulta el clima actual llamando a OpenWeather y guarda el resultado en base de datos.
        public async Task<DatoMeteorologico> ObtenerClimaActual()
        {
            try
            {
                // Construye la URL de petición a la API con los parámetros correspondientes.
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={Ciudad}&appid={ApiKey}&units=metric&lang=es";
                // Realiza la petición asíncrona de red obteniendo un objeto dinámico deserealizado de la API.
                var respuesta = await _httpClient.GetFromJsonAsync<dynamic>(url);

                // Instancia el modelo DatoMeteorologico a partir de los campos dinámicos de respuesta de la API.
                var dato = new DatoMeteorologico
                {
                    // Obtiene e inyecta la temperatura principal.
                    Temperatura = (double)respuesta!["main"]["temp"],
                    // Obtiene e inyecta el texto descriptivo del clima.
                    Descripcion = (string)respuesta["weather"][0]["description"],
                    // Obtiene e inyecta el porcentaje de humedad relativa.
                    Humedad = (int)respuesta["main"]["humidity"],
                    // Obtiene e inyecta la velocidad del viento medida.
                    Viento = (double)respuesta["wind"]["speed"],
                    // Mapea la condición climática de la API a los tipos locales definidos.
                    Condicion = DeterminarCondicion((string)respuesta["weather"][0]["main"]),
                    // Calcula el impacto vial correspondiente según el tipo de clima.
                    ImpactoTransito = DeterminarImpacto((string)respuesta["weather"][0]["main"])
                };

                // Inserta el dato meteorológico nuevo de manera asíncrona en MongoDB.
                await _coleccionMeteo.InsertOneAsync(dato);
                // Retorna el objeto de clima construido.
                return dato;
            }
            catch
            {
                // Retorna un objeto generado de manera aleatoria local si ocurre algún fallo de API o red.
                return GenerarDatoSimulado();
            }
        }

        // Obtiene las mediciones meteorológicas guardadas en el sistema ordenadas de forma cronológica.
        public async Task<List<DatoMeteorologico>> ObtenerHistorialMeteo()
        {
            // Retorna los últimos 20 registros guardados ordenados de forma descendente.
            return await _coleccionMeteo
                .Find(_ => true)
                .SortByDescending(d => d.RegistradoEn)
                .Limit(20)
                .ToListAsync();
        }

        // Genera un registro simulado con datos aleatorios en caso de que falle la API.
        private DatoMeteorologico GenerarDatoSimulado()
        {
            // Lista de posibles condiciones meteorológicas simuladas.
            var condiciones = new[] { "lluvia", "despejado", "niebla", "nublado" };
            // Selecciona una de las condiciones meteorológicas al azar de la lista.
            var condicion = condiciones[new Random().Next(condiciones.Length)];

            // Retorna el nuevo objeto meteorológico inicializado con valores aleatorios estables.
            return new DatoMeteorologico
            {
                // Genera una temperatura aleatoria realista para Bogotá.
                Temperatura = Math.Round(new Random().NextDouble() * 10 + 10, 1),
                // Asigna la condición seleccionada como descripción.
                Descripcion = condicion,
                // Genera un porcentaje de humedad aleatorio.
                Humedad = new Random().Next(60, 95),
                // Genera una velocidad de viento aleatoria.
                Viento = Math.Round(new Random().NextDouble() * 20, 1),
                // Asigna la condición climática seleccionada.
                Condicion = condicion,
                // Calcula la afectación de tránsito según la condición de clima simulada.
                ImpactoTransito = DeterminarImpacto(condicion)
            };
        }

        // Mapea la etiqueta recibida de la API externa a la nomenclatura de traducción interna.
        private string DeterminarCondicion(string condicionApi)
        {
            // Evalúa el nombre en inglés del clima para retornar su traducción correspondiente.
            return condicionApi.ToLower() switch
            {
                // Traduce lluvias.
                "rain" or "drizzle" => "lluvia",
                // Traduce tormentas eléctricas.
                "thunderstorm" => "tormenta",
                // Traduce niebla o neblina.
                "fog" or "mist" or "haze" => "niebla",
                // Traduce nublado.
                "clouds" => "nublado",
                // Retorna despejado para cualquier otro caso.
                _ => "despejado"
            };
        }

        // Asigna el nivel de afectación al tráfico de acuerdo a la condición climatológica.
        private string DeterminarImpacto(string condicion)
        {
            // Evalúa la severidad de la condición meteorológica para estimar la ralentización.
            return condicion.ToLower() switch
            {
                // Establece afectación severa para tormentas.
                "tormenta" or "thunderstorm" => "alto",
                // Establece afectación moderada para lluvias y nieblas.
                "lluvia" or "rain" or "niebla" or "fog" => "medio",
                // Establece afectación baja para días despejados o nublados.
                _ => "bajo"
            };
        }
    }
}