async function filtrar() {
    const desde = document.getElementById("fechaDesde").value;
    const hasta = document.getElementById("fechaHasta").value;
    const ruta = document.getElementById("filtroRuta").value;

    let url = "";
    let registros = [];

    try {
        if (ruta) {
            const respuesta = await fetch(`/Historial/FiltrarPorRuta?nombreRuta=${encodeURIComponent(ruta)}`);
            registros = await respuesta.json();
        } else if (desde && hasta) {
            const respuesta = await fetch(`/Historial/FiltrarPorFecha?desde=${desde}&hasta=${hasta}`);
            registros = await respuesta.json();
        } else {
            const respuesta = await fetch("/Historial/FiltrarPorFecha?desde=2000-01-01&hasta=2099-12-31");
            registros = await respuesta.json();
        }

        actualizarTabla(registros);
    } catch (error) {
        console.error("Error al filtrar:", error);
    }
}

function actualizarTabla(registros) {
    const cuerpo = document.getElementById("cuerpoTabla");
    cuerpo.innerHTML = "";

    if (registros.length === 0) {
        cuerpo.innerHTML = `
            <tr>
                <td colspan="6" style="text-align:center; color:#555; padding:24px;">
                    No hay registros para este filtro.
                </td>
            </tr>`;
        return;
    }

    registros.forEach(r => {
        const fila = document.createElement("tr");
        fila.innerHTML = `
            <td>${r.nombreRuta}</td>
            <td class="nivel-${r.nivelTransito}">● ${r.nivelTransito}</td>
            <td>${r.velocidadPromedio} km/h</td>
            <td>${r.totalIncidentes}</td>
            <td>${r.horaPico
                ? '<span class="badge-hora-pico">⏰ Hora pico</span>'
                : '<span style="color:#555;">Normal</span>'}</td>
            <td>${new Date(r.fechaRegistro).toLocaleString("es-CO")}</td>
        `;
        cuerpo.appendChild(fila);
    });
}

async function guardarEstado() {
    const btn = document.querySelector(".btn-guardar");
    btn.textContent = "Guardando...";
    btn.disabled = true;

    try {
        const respuesta = await fetch("/Historial/GuardarEstadoActual", { method: "POST" });
        const resultado = await respuesta.json();

        btn.textContent = "✅ Guardado";
        btn.style.background = "#39d353";

        setTimeout(() => {
            btn.textContent = "💾 Guardar estado actual";
            btn.style.background = "var(--color-verde)";
            btn.disabled = false;
            location.reload();
        }, 2000);

    } catch (error) {
        console.error("Error al guardar:", error);
        btn.textContent = "❌ Error";
        btn.disabled = false;
    }
}