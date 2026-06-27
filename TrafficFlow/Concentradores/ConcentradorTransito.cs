using Microsoft.AspNetCore.SignalR;
using TrafficFlow.Modelos;
using TrafficFlow.Servicios;

namespace TrafficFlow.Concentradores
{
    public class ConcentradorTransito : Hub
    {
        private readonly ServicioTransito _servicioTransito;

        public ConcentradorTransito(ServicioTransito servicioTransito)
        {
            _servicioTransito = servicioTransito;
        }

        public async Task ObtenerRutasActualizadas()
        {
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            await Clients.Caller.SendAsync("RecibirRutas", rutas);
        }

        public async Task ActualizarTransito(string rutaId, List<SegmentoTransito> segmentos)
        {
            await _servicioTransito.ActualizarNivelTransito(rutaId, segmentos);
            var rutasActualizadas = await _servicioTransito.ObtenerTodasLasRutas();
            await Clients.All.SendAsync("RecibirRutas", rutasActualizadas);
        }
    }
}