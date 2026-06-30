// Conexión SignalR para escuchar eventos del hub de tránsito
const conexionNotificaciones = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect() // Reconecta si se pierde la conexión
    .build();

// Cuando el servidor envía un incidente, lo muestra como alerta y lo agrega al panel
conexionNotificaciones.on("RecibirIncidente", (incidente) => {
    mostrarNotificacion(incidente);
    agregarIncidenteAlPanel(incidente);
});

// Muestra una notificación flotante en la esquina superior derecha con los datos del incidente
function mostrarNotificacion(incidente) {
    const notificacion = document.createElement("div");
    notificacion.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #161b22;
        border: 1px solid #f85149;
        border-radius: 8px;
        padding: 16px;
        color: #c9d1d9;
        z-index: 9999;
        min-width: 300px;
        animation: slideIn 0.3s ease;
    `;
    notificacion.innerHTML = `
        <strong style="color:#f85149;">🚨 ${incidente.tipo.toUpperCase()}</strong>
        <p style="margin:6px 0;">${incidente.descripcion}</p>
        <p style="color:#8b949e; font-size:0.8rem;">📍 ${incidente.ubicacion}</p>
        <button onclick="this.parentElement.remove()" 
                style="margin-top:8px; background:transparent; color:#58a6ff; border:none; cursor:pointer;">
            Cerrar
        </button>
    `;
    document.body.appendChild(notificacion);
    setTimeout(() => notificacion.remove(), 8000); // Se elimina automáticamente tras 8 segundos
}

// Agrega una tarjeta del incidente al listado visible en la página
function agregarIncidenteAlPanel(incidente) {
    const lista = document.getElementById("listaIncidentes");
    if (!lista) return; // Sale si el panel no existe en la vista actual

    const tarjeta = document.createElement("div");
    tarjeta.className = "tarjeta-ruta";
    tarjeta.id = `incidente-${incidente.id}`;
    tarjeta.innerHTML = `
        <h3>${incidente.tipo.toUpperCase()}</h3>
        <p>${incidente.descripcion}</p>
        <p style="color:#8b949e;">📍 ${incidente.ubicacion}</p>
        <button onclick="desactivar('${incidente.id}')"
                style="margin-top:8px; background:#f85149; color:white; border:none; padding:4px 10px; border-radius:4px; cursor:pointer;">
            Desactivar
        </button>
    `;
    lista.prepend(tarjeta); // Inserta la tarjeta al inicio de la lista
}

// Llama al servidor para marcar el incidente como inactivo y elimina su tarjeta del DOM
async function desactivar(id) {
    await fetch(`/Incidentes/Desactivar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    });
    document.getElementById(`incidente-${id}`)?.remove(); // Quita la tarjeta del panel
}

// Inicia la conexión SignalR; reintenta cada 5 segundos si falla
async function iniciarNotificaciones() {
    try {
        await conexionNotificaciones.start();
    } catch (e) {
        setTimeout(iniciarNotificaciones, 5000);
    }
}

// Arranca el sistema de notificaciones cuando el DOM está listo
document.addEventListener("DOMContentLoaded", iniciarNotificaciones);