// Importa las dependencias necesarias de SignalR para habilitar sockets web.
using Microsoft.AspNetCore.SignalR;
// Importa los modelos del proyecto, como segmentos y rutas.
using TrafficFlow.Modelos;
// Importa los servicios necesarios para la lógica del tránsito.
using TrafficFlow.Servicios;

namespace TrafficFlow.Concentradores
{
    // Clase que actúa como concentrador de SignalR para gestionar la comunicación en tiempo real.
    public class ConcentradorTransito : Hub
    {
        // Almacena la referencia al servicio que consulta y actualiza los datos del tráfico.
        private readonly ServicioTransito _servicioTransito;

        // Constructor que inyecta la instancia del servicio de tránsito.
        public ConcentradorTransito(ServicioTransito servicioTransito)
        {
            // Asigna la instancia inyectada a la variable local de la clase.
            _servicioTransito = servicioTransito;
        }

        // Método invocado por clientes para solicitar la lista actual de rutas.
        public async Task ObtenerRutasActualizadas()
        {
            // Obtiene la colección completa de rutas desde la base de datos de manera asíncrona.
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            // Envía la lista de rutas recuperada únicamente al cliente que realizó la petición.
            await Clients.Caller.SendAsync("RecibirRutas", rutas);
        }

        // Método invocado por clientes para reportar cambios en el tránsito de segmentos específicos.
        public async Task ActualizarTransito(string rutaId, List<SegmentoTransito> segmentos)
        {
            // Actualiza los niveles de tráfico para los segmentos especificados en la ruta correspondiente.
            await _servicioTransito.ActualizarNivelTransito(rutaId, segmentos);
            // Obtiene de nuevo todas las rutas con los datos recientemente actualizados.
            var rutasActualizadas = await _servicioTransito.ObtenerTodasLasRutas();
            // Transmite la lista de rutas actualizada a todos los clientes conectados en tiempo real.
            await Clients.All.SendAsync("RecibirRutas", rutasActualizadas);
        }
    }
}