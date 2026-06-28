const conexionEmergencias = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

conexionEmergencias.on("RecibirEmergencia", (emergencia) => {
    mostrarAlertaEmergencia(emergencia);
    agregarEmergenciaAlPanel(emergencia);
});

conexionEmergencias.on("ActualizarMapa", () => {
    if (window.location.pathname === "/Rutas") {
        conexionEmergencias.invoke("ObtenerRutasActualizadas");
    }
});

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
    document.body.prepend(alerta);
    setTimeout(() => alerta.remove(), 15000);
}

function agregarEmergenciaAlPanel(emergencia) {
    const lista = document.getElementById("listaEmergencias");
    if (!lista) return;

    const tarjeta = document.createElement("div");
    tarjeta.className = "tarjeta-emergencia";
    tarjeta.id = `emergencia-${emergencia.id}`;
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

async function finalizar(id) {
    await fetch(`/Emergencias/Finalizar`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    });
    document.getElementById(`emergencia-${id}`)?.remove();
}

async function iniciarSimulador() {
    try {
        await conexionEmergencias.start();
    } catch (e) {
        setTimeout(iniciarSimulador, 5000);
    }
}

document.addEventListener("DOMContentLoaded", iniciarSimulador);