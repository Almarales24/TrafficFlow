const conexionNotificaciones = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

conexionNotificaciones.on("RecibirIncidente", (incidente) => {
    mostrarNotificacion(incidente);
    agregarIncidenteAlPanel(incidente);
});

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
    setTimeout(() => notificacion.remove(), 8000);
}

function agregarIncidenteAlPanel(incidente) {
    const lista = document.getElementById("listaIncidentes");
    if (!lista) return;

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
    lista.prepend(tarjeta);
}

async function desactivar(id) {
    await fetch(`/Incidentes/Desactivar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    });
    document.getElementById(`incidente-${id}`)?.remove();
}

async function iniciarNotificaciones() {
    try {
        await conexionNotificaciones.start();
    } catch (e) {
        setTimeout(iniciarNotificaciones, 5000);
    }
}

document.addEventListener("DOMContentLoaded", iniciarNotificaciones);