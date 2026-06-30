// Recoge los datos del formulario, valida y envía la simulación al servidor
async function ejecutarSimulacion() {

    // Obtiene los valores ingresados por el usuario en el formulario
    const nombre = document.getElementById("nombreSim").value;
    const horarioInicio = document.getElementById("horarioInicio").value;
    const horarioFin = document.getElementById("horarioFin").value;
    const factorCongesion = document.getElementById("factorCongesion").value;

    // Valida que el nombre no esté vacío antes de continuar
    if (!nombre) {
        alert("Por favor ingresa un nombre para la simulación.");
        return;
    }

    // Muestra el indicador de carga y desactiva el botón durante el proceso
    const cargando = document.getElementById("cargando");
    const btnEjecutar = document.querySelector(".btn-ejecutar");
    cargando.style.display = "block";
    btnEjecutar.disabled = true;
    btnEjecutar.textContent = "Simulando...";

    try {
        // Construye los parámetros de la URL con los datos del formulario
        const params = new URLSearchParams({
            nombre,
            horarioInicio,
            horarioFin,
            factorCongesion
        });

        // Envía la solicitud POST al servidor con los parámetros de la simulación
        const respuesta = await fetch(`/Simulacion/Ejecutar?${params}`, {
            method: "POST"
        });

        // Convierte la respuesta a JSON y agrega el resultado al panel visual
        const resultado = await respuesta.json();
        agregarSimulacionAlPanel(resultado);

        // Limpia el campo de nombre para una nueva simulación
        document.getElementById("nombreSim").value = "";

    } catch (error) {
        // Registra el error en consola si la solicitud falla
        console.error("Error al ejecutar simulación:", error);

    } finally {
        // Oculta el indicador de carga y restaura el botón en cualquier caso
        cargando.style.display = "none";
        btnEjecutar.disabled = false;
        btnEjecutar.textContent = "▶ Ejecutar simulación";
    }
}

// Crea y agrega una tarjeta con los resultados de la simulación al panel
function agregarSimulacionAlPanel(sim) {

    // Obtiene el contenedor de la lista de simulaciones
    const lista = document.getElementById("listaSimulaciones");

    // Crea la tarjeta y aplica el color de acento azul temporalmente
    const tarjeta = document.createElement("div");
    tarjeta.className = "tarjeta-simulacion";
    tarjeta.style.borderColor = "var(--color-azul)";

    // Genera el HTML de cada ruta afectada con su velocidad y nivel de tránsito
    const resultadosHtml = sim.resultados.map(r => `
        <div class="resultado-ruta">
            <span>${r.nombreRuta}</span>
            <span class="nivel-${r.nivelTransitoSimulado}">
                ${r.velocidadSimulada} km/h — ${r.nivelTransitoSimulado}
            </span>
        </div>
    `).join("");

    // Construye el contenido de la tarjeta con nombre, horario y resultados
    tarjeta.innerHTML = `
        <div style="display:flex; justify-content:space-between; align-items:center;">
            <h3>${sim.nombre}</h3>
            <span class="factor-badge">x${sim.factorCongesion} congestión</span>
        </div>
        <p style="color:#8b949e; font-size:0.8rem; margin-bottom:12px;">
            ${sim.horarioInicio} → ${sim.horarioFin} | Recién ejecutado
        </p>
        ${resultadosHtml}
    `;

    // Inserta la tarjeta al inicio de la lista para mostrar la más reciente primero
    lista.prepend(tarjeta);

    // Restaura el color del borde al valor normal después de 3 segundos
    setTimeout(() => {
        tarjeta.style.borderColor = "var(--color-borde)";
    }, 3000);
}