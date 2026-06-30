namespace TrafficFlow.Modelos
{
    // Clase modelo para estructurar los detalles de errores arrojados en la interfaz de usuario.
    public class ErrorViewModel
    {
        // Almacena el identificador único asociado a la solicitud HTTP fallida.
        public string? RequestId { get; set; }

        // Evalúa si el identificador de la solicitud es válido para ser mostrado.
        public bool ShowRequestId => RequestId != null;
    }
}