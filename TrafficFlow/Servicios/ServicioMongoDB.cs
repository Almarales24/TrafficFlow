// Importa funciones del sistema como excepciones y tipos básicos.
using System;
// Importa las utilidades de configuración de .NET para leer cadenas de conexión.
using Microsoft.Extensions.Configuration;
// Importa tipos Bson de MongoDB para comandos ping.
using MongoDB.Bson;
// Importa el controlador oficial de MongoDB para conectividad a base de datos.
using MongoDB.Driver;
// Importa los modelos del proyecto como Ruta, Incidente, Emergencia, etc.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Servicio encargado de inicializar la conexión con MongoDB y exponer las colecciones.
    public class ServicioMongoDB
    {
        // Almacena la referencia a la base de datos de MongoDB conectada.
        private readonly IMongoDatabase _baseDatos;

        // Constructor que inyecta la configuración para establecer la conexión con la base de datos.
        public ServicioMongoDB(IConfiguration configuracion)
        {
            // Obtiene la cadena de conexión de MongoDB desde los archivos de configuración JSON.
            var cadena = configuracion["MongoDB:CadenaConexion"];
            // Obtiene el nombre de la base de datos a utilizar de la configuración.
            var nombre = configuracion["MongoDB:NombreBaseDatos"];

            // Lanza una excepción si la cadena de conexión no está configurada.
            if (string.IsNullOrWhiteSpace(cadena))
                throw new ArgumentException("MongoDB:CadenaConexion no está configurada.");
            // Lanza una excepción si el nombre de la base de datos no está configurado.
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("MongoDB:NombreBaseDatos no está configurada.");

            // Inicializa una variable para registrar excepciones entre reintentos.
            Exception lastEx = null;

            // Intento 1: Establece una conexión directa utilizando la cadena de conexión proporcionada.
            try
            {
                // Crea la instancia del cliente de MongoDB usando la cadena cruda.
                var cliente = new MongoClient(cadena);
                // Obtiene el objeto de base de datos correspondiente.
                _baseDatos = cliente.GetDatabase(nombre);
                // Ejecuta un comando ping a la base de datos para validar si la conexión es funcional.
                _baseDatos.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                // Termina la ejecución exitosa del constructor.
                return;
            }
            // Captura errores específicos de autenticación de credenciales.
            catch (MongoAuthenticationException mex)
            {
                // Guarda la excepción de autenticación para su análisis posterior.
                lastEx = mex;
            }
            // Captura cualquier otro tipo de error al conectar.
            catch (Exception ex)
            {
                // Guarda la excepción genérica de conexión para su análisis.
                lastEx = ex;
            }

            // Intento 2: Construye una conexión reconstruyendo las credenciales explícitas.
            try
            {
                // Parsea la cadena de conexión de MongoDB para extraer sus partes.
                var url = new MongoUrl(cadena);
                // Crea los parámetros de configuración a partir de la URL parseada.
                var settings = MongoClientSettings.FromUrl(url);

                // Si la URL contiene un nombre de usuario, configura las credenciales de autenticación explícitas.
                if (!string.IsNullOrEmpty(url.Username))
                {
                    // Obtiene el origen de autenticación especificado o usa la base de datos por defecto.
                    var authSource = url.AuthenticationSource ?? url.DatabaseName ?? "admin";
                    // Genera la credencial de conexión de MongoDB con usuario y contraseña descifrada.
                    settings.Credential = MongoCredential.CreateCredential(authSource, url.Username, url.Password);
                }

                // Crea una nueva instancia del cliente de base de datos con los parámetros corregidos.
                var cliente2 = new MongoClient(settings);
                // Obtiene la referencia a la base de datos.
                _baseDatos = cliente2.GetDatabase(nombre);
                // Ejecuta un ping para probar la validez de la nueva conexión construida.
                _baseDatos.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                // Termina la ejecución exitosa del constructor.
                return;
            }
            // Maneja el fallo si la autenticación explícita también falló.
            catch (MongoAuthenticationException mex2)
            {
                // Lanza una excepción indicando fallo de credenciales con sugerencias.
                throw new InvalidOperationException("Autenticación a MongoDB fallida. Verifica usuario, contraseña y 'authSource' en la cadena de conexión. Si usas caracteres especiales en la contraseña, considera configurarlos por separado (MongoDB:Usuario y MongoDB:Password).", mex2);
            }
            // Maneja cualquier otro fallo crítico al intentar conectar.
            catch (Exception ex)
            {
                // Lanza una excepción general informando que el servicio no puede iniciar.
                throw new InvalidOperationException($"Error al inicializar conexión a MongoDB: {ex.Message}", ex);
            }
        }

        // Obtiene la colección de MongoDB correspondiente a las rutas de tránsito.
        public IMongoCollection<Ruta> ObtenerColeccionRutas()
        {
            // Retorna la colección mapeada con el nombre de tabla 'rutas'.
            return _baseDatos.GetCollection<Ruta>("rutas");
        }

        // Obtiene la colección de MongoDB correspondiente a los incidentes reportados.
        public IMongoCollection<Incidente> ObtenerColeccionIncidentes()
        {
            // Retorna la colección mapeada con el nombre de tabla 'incidentes'.
            return _baseDatos.GetCollection<Incidente>("incidentes");
        }

        // Obtiene la colección de MongoDB correspondiente a las alertas de emergencia.
        public IMongoCollection<Emergencia> ObtenerColeccionEmergencias()
        {
            // Retorna la colección mapeada con el nombre de tabla 'emergencias'.
            return _baseDatos.GetCollection<Emergencia>("emergencias");
        }

        // Obtiene la colección de MongoDB correspondiente al historial de mediciones.
        public IMongoCollection<RegistroTransito> ObtenerColeccionHistorial()
        {
            // Retorna la colección mapeada con el nombre de tabla 'historial'.
            return _baseDatos.GetCollection<RegistroTransito>("historial");
        }

        // Obtiene la colección de MongoDB correspondiente a los escenarios de simulación.
        public IMongoCollection<SimulacionHorario> ObtenerColeccionSimulaciones()
        {
            // Retorna la colección mapeada con el nombre de tabla 'simulaciones'.
            return _baseDatos.GetCollection<SimulacionHorario>("simulaciones");
        }

        // Obtiene la colección de MongoDB correspondiente a las rutas favoritas de usuarios.
        public IMongoCollection<RutaFavorita> ObtenerColeccionFavoritos()
        {
            // Retorna la colección mapeada con el nombre de tabla 'favoritos'.
            return _baseDatos.GetCollection<RutaFavorita>("favoritos");
        }

        // Obtiene la colección de MongoDB correspondiente a los reportes del clima.
        public IMongoCollection<DatoMeteorologico> ObtenerColeccionMeteorologica()
        {
            // Retorna la colección mapeada con el nombre de tabla 'meteorologia'.
            return _baseDatos.GetCollection<DatoMeteorologico>("meteorologia");
        }
    }
}
