// Crea la conexión SignalR al concentrador de tránsito con reconexión automática
const conexionMeteo = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

// Escucha el evento del servidor y actualiza la tarjeta cuando llega nuevo clima
conexionMeteo.on("RecibirClima", (clima) => {
    actualizarTarjetaClima(clima);
});

// Actualiza todos los campos visuales de la tarjeta con los datos del clima recibido
function actualizarTarjetaClima(clima) {

    // Mapa de condiciones climáticas a su ícono correspondiente
    const iconos = {
        lluvia: "🌧️",
        tormenta: "⛈️",
        niebla: "🌫️",
        nublado: "☁️",
        despejado: "☀️"
    };

    // Actualiza el ícono según la condición, usa sol por defecto si no hay coincidencia
    document.getElementById("iconoClima").textContent = iconos[clima.condicion] || "☀️";

    // Actualiza temperatura, descripción, humedad y velocidad del viento
    document.getElementById("temperatura").textContent = `${clima.temperatura}°C`;
    document.getElementById("descripcion").textContent = clima.descripcion;
    document.getElementById("humedad").textContent = `${clima.humedad}%`;
    document.getElementById("viento").textContent = `${clima.viento} km/h`;

    // Actualiza el badge de impacto al tráfico con el color y texto correspondiente
    const badge = document.getElementById("impactoBadge");
    badge.className = `impacto-badge impacto-${clima.impactoTransito}`;
    badge.textContent = `Impacto en tráfico: ${clima.impactoTransito}`;
}

// Solicita al servidor los datos de clima más recientes y actualiza la vista
async function actualizarClima() {

    // Desactiva el botón y muestra estado de carga mientras se procesa
    const btn = document.querySelector(".btn-actualizar");
    btn.textContent = "🔄 Actualizando...";
    btn.disabled = true;

    try {
        // Llama al endpoint del servidor para obtener el clima actualizado
        const respuesta = await fetch("/Meteorologia/ActualizarClima");
        const clima = await respuesta.json();

        // Refleja los nuevos datos en la tarjeta visual
        actualizarTarjetaClima(clima);

        // Muestra confirmación visual de éxito en el botón
        btn.textContent = "✅ Actualizado";
        btn.style.background = "#39d353";

        // Restaura el botón a su estado original después de 2 segundos
        setTimeout(() => {
            btn.textContent = "🔄 Actualizar clima";
            btn.style.background = "var(--color-azul)";
            btn.disabled = false;
        }, 2000);

    } catch (error) {
        // Muestra error en consola y restaura el botón si la solicitud falla
        console.error("Error al actualizar clima:", error);
        btn.textContent = "❌ Error";
        btn.disabled = false;
    }
}

// Inicia la conexión SignalR y programa actualizaciones automáticas cada 5 minutos
async function iniciarMeteo() {
    try {
        await conexionMeteo.start();

        // Ejecuta la actualización del clima cada 300,000 ms (5 minutos)
        setInterval(actualizarClima, 300000);

    } catch (e) {
        // Si la conexión falla, reintenta después de 5 segundos
        setTimeout(iniciarMeteo, 5000);
    }
}

// Inicia el módulo meteorológico cuando el DOM está completamente cargado
document.addEventListener("DOMContentLoaded", iniciarMeteo);