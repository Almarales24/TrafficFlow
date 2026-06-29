const conexionFavoritos = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

conexionFavoritos.on("RecibirAlertaFavorito", (alertas) => {
    alertas.forEach(alerta => mostrarAlertaFavorito(alerta));
});

function mostrarAlertaFavorito(alerta) {
    const notificacion = document.createElement("div");
    notificacion.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #161b22;
        border: 1px solid #f0c030;
        border-radius: 8px;
        padding: 16px;
        color: #c9d1d9;
        z-index: 9999;
        min-width: 300px;
        animation: slideIn 0.3s ease;
    `;
    notificacion.innerHTML = `
        <strong style="color:#f0c030;">⭐ Alerta de ruta favorita</strong>
        <p style="margin:6px 0;">${alerta.nombreRuta}</p>
        <p style="color:#8b949e; font-size:0.8rem;">
            Tráfico <span class="nivel-${alerta.nivelAlerta}">${alerta.nivelAlerta}</span> detectado
        </p>
        <button onclick="this.parentElement.remove()"
                style="margin-top:8px; background:transparent; color:#58a6ff; border:none; cursor:pointer;">
            Cerrar
        </button>
    `;
    document.body.appendChild(notificacion);
    setTimeout(() => notificacion.remove(), 10000);
}

function actualizarNombre() {
    const select = document.getElementById("selectRuta");
    const opcion = select.options[select.selectedIndex];
    document.getElementById("nombreRuta").value = opcion.dataset.nombre;
}

async function eliminar(id) {
    await fetch("/Favoritos/Eliminar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    });
    const tarjeta = document.getElementById(`favorito-${id}`);
    tarjeta.style.animation = "none";
    tarjeta.style.opacity = "0";
    tarjeta.style.transform = "translateX(20px)";
    tarjeta.style.transition = "all 0.3s ease";
    setTimeout(() => tarjeta.remove(), 300);
}

async function verificarAlertas() {
    try {
        const respuesta = await fetch("/Favoritos/VerificarAlertas");
        const alertas = await respuesta.json();
        if (alertas.length === 0) {
            const msg = document.createElement("div");
            msg.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                background: #161b22;
                border: 1px solid #39d353;
                border-radius: 8px;
                padding: 16px;
                color: #c9d1d9;
                z-index: 9999;
            `;
            msg.innerHTML = `
                <strong style="color:#39d353;">✅ Sin alertas activas</strong>
                <p style="margin:6px 0; font-size:0.85rem;">Todas tus rutas favoritas están en orden.</p>
            `;
            document.body.appendChild(msg);
            setTimeout(() => msg.remove(), 4000);
        }
    } catch (error) {
        console.error("Error al verificar alertas:", error);
    }
}

async function iniciarFavoritos() {
    try {
        await conexionFavoritos.start();
        setInterval(verificarAlertas, 60000);
    } catch (e) {
        setTimeout(iniciarFavoritos, 5000);
    }
}

document.addEventListener("DOMContentLoaded", iniciarFavoritos);