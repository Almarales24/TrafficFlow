const conexionMeteo = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

conexionMeteo.on("RecibirClima", (clima) => {
    actualizarTarjetaClima(clima);
});

function actualizarTarjetaClima(clima) {
    const iconos = {
        lluvia: "🌧️",
        tormenta: "⛈️",
        niebla: "🌫️",
        nublado: "☁️",
        despejado: "☀️"
    };

    document.getElementById("iconoClima").textContent = iconos[clima.condicion] || "☀️";
    document.getElementById("temperatura").textContent = `${clima.temperatura}°C`;
    document.getElementById("descripcion").textContent = clima.descripcion;
    document.getElementById("humedad").textContent = `${clima.humedad}%`;
    document.getElementById("viento").textContent = `${clima.viento} km/h`;

    const badge = document.getElementById("impactoBadge");
    badge.className = `impacto-badge impacto-${clima.impactoTransito}`;
    badge.textContent = `Impacto en tráfico: ${clima.impactoTransito}`;
}

async function actualizarClima() {
    const btn = document.querySelector(".btn-actualizar");
    btn.textContent = "🔄 Actualizando...";
    btn.disabled = true;

    try {
        const respuesta = await fetch("/Meteorologia/ActualizarClima");
        const clima = await respuesta.json();
        actualizarTarjetaClima(clima);

        btn.textContent = "✅ Actualizado";
        btn.style.background = "#39d353";

        setTimeout(() => {
            btn.textContent = "🔄 Actualizar clima";
            btn.style.background = "var(--color-azul)";
            btn.disabled = false;
        }, 2000);

    } catch (error) {
        console.error("Error al actualizar clima:", error);
        btn.textContent = "❌ Error";
        btn.disabled = false;
    }
}

async function iniciarMeteo() {
    try {
        await conexionMeteo.start();
        setInterval(actualizarClima, 300000); // cada 5 minutos
    } catch (e) {
        setTimeout(iniciarMeteo, 5000);
    }
}

document.addEventListener("DOMContentLoaded", iniciarMeteo);