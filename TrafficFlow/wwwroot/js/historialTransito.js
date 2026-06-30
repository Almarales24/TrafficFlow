// Consulta el historial según los filtros seleccionados (ruta o rango de fechas)
async function filtrar() {
    const desde = document.getElementById("fechaDesde").value;
    const hasta = document.getElementById("fechaHasta").value;
    const ruta = document.getElementById("filtroRuta").value;

    let url = "";
    let registros = [];

    try {
        if (ruta) {
            // Filtra solo por nombre de ruta
            const respuesta = await fetch(`/Historial/FiltrarPorRuta?nombreRuta=${encodeURIComponent(ruta)}`);
            registros = await respuesta.json();
        } else if (desde && hasta) {
            // Filtra por rango de fechas
            const respuesta = await fetch(`/Historial/FiltrarPorFecha?desde=${desde}&hasta=${hasta}`);
            registros = await respuesta.json();
        } else {
            // Sin filtros: trae todos los registros con un rango amplio
            const respuesta = await fetch("/Historial/FiltrarPorFecha?desde=2000-01-01&hasta=2099-12-31");
            registros = await respuesta.json();
        }

        actualizarTabla(registros); // Renderiza los resultados en la tabla
    } catch (error) {
        console.error("Error al filtrar:", error);
    }
}

// Reemplaza el contenido de la tabla con los registros filtrados
function actualizarTabla(registros) {
    const cuerpo = document.getElementById("cuerpoTabla");
    cuerpo.innerHTML = ""; // Limpia las filas anteriores

    if (registros.length === 0) {
        // Si no hay resultados, muestra un mensaje en la tabla
        cuerpo.innerHTML = `
            <tr>
                <td colspan="6" style="text-align:center; color:#555; padding:24px;">
                    No hay registros para este filtro.
                </td>
            </tr>`;
        return;
    }

    // Genera una fila HTML por cada registro recibido
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

// Envía una solicitud al servidor para capturar y guardar el estado actual del tráfico
async function guardarEstado() {
    const btn = document.querySelector(".btn-guardar");
    btn.textContent = "Guardando..."; // Indica visualmente que se está procesando
    btn.disabled = true;

    try {
        const respuesta = await fetch("/Historial/GuardarEstadoActual", { method: "POST" });
        const resultado = await respuesta.json();

        btn.textContent = "✅ Guardado"; // Confirmación visual del éxito
        btn.style.background = "#39d353";

        // Restaura el botón y recarga la tabla tras 2 segundos
        setTimeout(() => {
            btn.textContent = "💾 Guardar estado actual";
            btn.style.background = "var(--color-verde)";
            btn.disabled = false;
            location.reload();
        }, 2000);

    } catch (error) {
        console.error("Error al guardar:", error);
        btn.textContent = "❌ Error"; // Indica el fallo visualmente
        btn.disabled = false;
    }
}