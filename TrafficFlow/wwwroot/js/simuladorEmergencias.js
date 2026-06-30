// Crea la conexión SignalR al concentrador de tránsito con reconexión automática
const conexionEmergencias = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

// Maneja eventos del concentrador de tránsito
// Recibe emergencias del servidor y actualiza el mapa
conexionEmergencias.on("RecibirEmergencia", (emergencia) => {
    mostrarAlertaEmergencia(emergencia);
    agregarEmergenciaAlPanel(emergencia);
});

// Evento que se activa cuando el servidor indica que se deben actualizar los datos del mapa
conexionEmergencias.on("ActualizarMapa", () => {
    if (window.location.pathname === "/Rutas") {
        conexionEmergencias.invoke("ObtenerRutasActualizadas");
    }
});

// Crea una alerta visual roja cuando se recibe una nueva emergencia
function mostrarAlertaEmergencia(emergencia) {
    const alerta = document.createElement("div");
    alerta.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        background: linear-gradient(135deg, #2d0f0f, #1a0a0a);
        border-bottom: 2px solid #f85149;
        padding: 20px;
        color: #c9d1d9;
        z-index: 9999;
        text-align: center;
        animation: slideDown 0.5s ease;
    `;
    // Construye el contenido HTML de la alerta con tipo, descripción y número de rutas afectadas
    alerta.innerHTML = `
        <h2 style="color:#f85149; margin:0;">⚠️ EMERGENCIA ACTIVA: ${emergencia.tipo.toUpperCase()}</h2>
        <p style="margin:8px 0;">${emergencia.descripcion}</p>
        <p style="color:#8b949e;">Rutas afectadas: ${emergencia.rutasAfectadas?.length || 0}</p>
        <button onclick="this.parentElement.remove()" style="
            background: #f85149;
            color: white;
            border: none;
            padding: 8px 20px;
            border-radius: 6px;
            cursor: pointer;
            margin-top: 8px;
        ">Cerrar alerta</button>
    `;
    // Agrega la alerta al inicio del documento y programa su eliminación automática
    document.body.prepend(alerta);
    setTimeout(() => alerta.remove(), 15000);
}

// Agrega la nueva emergencia al panel lateral de emergencias
function agregarEmergenciaAlPanel(emergencia) {
    const lista = document.getElementById("listaEmergencias");
    if (!lista) return;

    // Crea una tarjeta para la nueva emergencia
    const tarjeta = document.createElement("div");
    tarjeta.className = "tarjeta-emergencia";
    tarjeta.id = `emergencia-${emergencia.id}`;
    // Construye el contenido HTML de la tarjeta con tipo, descripción y nivel de impacto
    tarjeta.innerHTML = `
        <h3 style="color:#f85149;">${emergencia.tipo.toUpperCase()}</h3>
        <p>${emergencia.descripcion}</p>
        <p style="color:#8b949e;">Impacto: <strong>${emergencia.nivelImpacto}</strong></p>
        <button class="btn-finalizar" onclick="finalizar('${emergencia.id}')">
            ✓ Finalizar emergencia
        </button>
    `;
    lista.prepend(tarjeta);
}

// Función para finalizar una emergencia
async function finalizar(id) {
    await fetch(`/Emergencias/Finalizar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    });
    document.getElementById(`emergencia-${id}`)?.remove();
}

// Inicia la conexión SignalR con el servidor
async function iniciarSimulador() {
    try {
        await conexionEmergencias.start();
    } catch (e) {
        setTimeout(iniciarSimulador, 5000);
    }
}

// Inicia el simulador cuando el DOM está completamente cargado
document.addEventListener("DOMContentLoaded", iniciarSimulador);