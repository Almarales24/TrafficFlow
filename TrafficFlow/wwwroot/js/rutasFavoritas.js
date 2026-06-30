// Crea la conexión SignalR al concentrador de tránsito con reconexión automática
const conexionFavoritos = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

// Escucha alertas del servidor y muestra una notificación por cada ruta afectada
conexionFavoritos.on("RecibirAlertaFavorito", (alertas) => {
    alertas.forEach(alerta => mostrarAlertaFavorito(alerta));
});

// Crea y muestra una notificación flotante con los datos de la alerta recibida
function mostrarAlertaFavorito(alerta) {

    // Crea el contenedor de la notificación y aplica estilos de posicionamiento fijo
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

    // Construye el contenido de la notificación con nombre de ruta, nivel de alerta y botón de cierre
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

    // Agrega la notificación al cuerpo de la página
    document.body.appendChild(notificacion);

    // Elimina la notificación automáticamente después de 10 segundos
    setTimeout(() => notificacion.remove(), 10000);
}

// Sincroniza el campo oculto de nombre de ruta cuando el usuario cambia la selección
function actualizarNombre() {
    const select = document.getElementById("selectRuta");

    // Obtiene la opción seleccionada y extrae su atributo de nombre
    const opcion = select.options[select.selectedIndex];
    document.getElementById("nombreRuta").value = opcion.dataset.nombre;
}

// Envía la solicitud de eliminación al servidor y anima la salida de la tarjeta
async function eliminar(id) {

    // Llama al endpoint de eliminación con el id de la ruta favorita
    await fetch("/Favoritos/Eliminar", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(id)
    });

    // Aplica animación de desvanecimiento y desplazamiento antes de eliminar del DOM
    const tarjeta = document.getElementById(`favorito-${id}`);
    tarjeta.style.animation = "none";
    tarjeta.style.opacity = "0";
    tarjeta.style.transform = "translateX(20px)";
    tarjeta.style.transition = "all 0.3s ease";

    // Elimina la tarjeta del DOM tras completar la animación
    setTimeout(() => tarjeta.remove(), 300);
}

// Consulta al servidor si hay alertas activas y muestra el resultado al usuario
async function verificarAlertas() {
    try {
        // Llama al endpoint de verificación de alertas
        const respuesta = await fetch("/Favoritos/VerificarAlertas");
        const alertas = await respuesta.json();

        // Si no hay alertas, muestra un mensaje de confirmación verde
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

            // Agrega el mensaje al DOM y lo elimina después de 4 segundos
            document.body.appendChild(msg);
            setTimeout(() => msg.remove(), 4000);
        }
    } catch (error) {
        // Registra el error en consola si la verificación falla
        console.error("Error al verificar alertas:", error);
    }
}

// Inicia la conexión SignalR y programa verificaciones automáticas cada minuto
async function iniciarFavoritos() {
    try {
        await conexionFavoritos.start();

        // Verifica alertas cada 60,000 ms (1 minuto)
        setInterval(verificarAlertas, 60000);

    } catch (e) {
        // Reintenta la conexión después de 5 segundos si falla
        setTimeout(iniciarFavoritos, 5000);
    }
}

// Inicia el módulo de favoritos cuando el DOM está completamente cargado
document.addEventListener("DOMContentLoaded", iniciarFavoritos);