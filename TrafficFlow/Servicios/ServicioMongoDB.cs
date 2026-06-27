using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioMongoDB
    {
        private readonly IMongoDatabase _baseDatos;

        public ServicioMongoDB(IConfiguration configuracion)
        {
            var cadena = configuracion["MongoDB:CadenaConexion"];
            var nombre = configuracion["MongoDB:NombreBaseDatos"];

            if (string.IsNullOrWhiteSpace(cadena))
                throw new ArgumentException("MongoDB:CadenaConexion no está configurada.");
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("MongoDB:NombreBaseDatos no está configurada.");

            Exception lastEx = null;

            // Intento 1: cliente directo desde la cadena de conexión
            try
            {
                var cliente = new MongoClient(cadena);
                _baseDatos = cliente.GetDatabase(nombre);
                _baseDatos.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return;
            }
            catch (MongoAuthenticationException mex)
            {
                lastEx = mex;
                // Se intentará una segunda estrategia abajo
            }
            catch (Exception ex)
            {
                lastEx = ex;
                // Continuar a intento alternativo
            }

            // Intento 2: reconstruir credenciales explícitas usando MongoUrl
            try
            {
                var url = new MongoUrl(cadena);
                var settings = MongoClientSettings.FromUrl(url);

                if (!string.IsNullOrEmpty(url.Username))
                {
                    var authSource = url.AuthenticationSource ?? url.DatabaseName ?? "admin";
                    settings.Credential = MongoCredential.CreateCredential(authSource, url.Username, url.Password);
                }

                var cliente2 = new MongoClient(settings);
                _baseDatos = cliente2.GetDatabase(nombre);
                _baseDatos.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                return;
            }
            catch (MongoAuthenticationException mex2)
            {
                throw new InvalidOperationException("Autenticación a MongoDB fallida. Verifica usuario, contraseña y 'authSource' en la cadena de conexión. Si usas caracteres especiales en la contraseña, considera configurarlos por separado (MongoDB:Usuario y MongoDB:Password).", mex2);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al inicializar conexión a MongoDB: {ex.Message}", ex);
            }
        }

        public IMongoCollection<Ruta> ObtenerColeccionRutas()
        {
            return _baseDatos.GetCollection<Ruta>("rutas");
        }
    }
}
