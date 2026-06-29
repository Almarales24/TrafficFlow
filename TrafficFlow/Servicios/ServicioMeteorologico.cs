using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioMeteorologico
    {
        private readonly IMongoCollection<DatoMeteorologico> _coleccionMeteo;
        private readonly HttpClient _httpClient;
        private const string ApiKey = "TU_API_KEY_OPENWEATHER";
        private const string Ciudad = "Bogota,CO";

        public ServicioMeteorologico(ServicioMongoDB servicioMongoDB, IHttpClientFactory httpClientFactory)
        {
            _coleccionMeteo = servicioMongoDB.ObtenerColeccionMeteorologica();
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<DatoMeteorologico> ObtenerClimaActual()
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={Ciudad}&appid={ApiKey}&units=metric&lang=es";
                var respuesta = await _httpClient.GetFromJsonAsync<dynamic>(url);

                var dato = new DatoMeteorologico
                {
                    Temperatura = (double)respuesta!["main"]["temp"],
                    Descripcion = (string)respuesta["weather"][0]["description"],
                    Humedad = (int)respuesta["main"]["humidity"],
                    Viento = (double)respuesta["wind"]["speed"],
                    Condicion = DeterminarCondicion((string)respuesta["weather"][0]["main"]),
                    ImpactoTransito = DeterminarImpacto((string)respuesta["weather"][0]["main"])
                };

                await _coleccionMeteo.InsertOneAsync(dato);
                return dato;
            }
            catch
            {
                return GenerarDatoSimulado();
            }
        }

        public async Task<List<DatoMeteorologico>> ObtenerHistorialMeteo()
        {
            return await _coleccionMeteo
                .Find(_ => true)
                .SortByDescending(d => d.RegistradoEn)
                .Limit(20)
                .ToListAsync();
        }

        private DatoMeteorologico GenerarDatoSimulado()
        {
            var condiciones = new[] { "lluvia", "despejado", "niebla", "nublado" };
            var condicion = condiciones[new Random().Next(condiciones.Length)];

            return new DatoMeteorologico
            {
                Temperatura = Math.Round(new Random().NextDouble() * 10 + 10, 1),
                Descripcion = condicion,
                Humedad = new Random().Next(60, 95),
                Viento = Math.Round(new Random().NextDouble() * 20, 1),
                Condicion = condicion,
                ImpactoTransito = DeterminarImpacto(condicion)
            };
        }

        private string DeterminarCondicion(string condicionApi)
        {
            return condicionApi.ToLower() switch
            {
                "rain" or "drizzle" => "lluvia",
                "thunderstorm" => "tormenta",
                "fog" or "mist" or "haze" => "niebla",
                "clouds" => "nublado",
                _ => "despejado"
            };
        }

        private string DeterminarImpacto(string condicion)
        {
            return condicion.ToLower() switch
            {
                "tormenta" or "thunderstorm" => "alto",
                "lluvia" or "rain" or "niebla" or "fog" => "medio",
                _ => "bajo"
            };
        }
    }
}