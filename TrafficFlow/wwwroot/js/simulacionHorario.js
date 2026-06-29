async function ejecutarSimulacion() {
    const nombre = document.getElementById("nombreSim").value;
    const horarioInicio = document.getElementById("horarioInicio").value;
    const horarioFin = document.getElementById("horarioFin").value;
    const factorCongesion = document.getElementById("factorCongesion").value;

    if (!nombre) {
        alert("Por favor ingresa un nombre para la simulación.");
        return;
    }

    const cargando = document.getElementById("cargando");
    const btnEjecutar = document.querySelector(".btn-ejecutar");
    cargando.style.display = "block";
    btnEjecutar.disabled = true;
    btnEjecutar.textContent = "Simulando...";

    try {
        const params = new URLSearchParams({
            nombre,
            horarioInicio,
            horarioFin,
            factorCongesion
        });

        const respuesta = await fetch(`/Simulacion/Ejecutar?${params}`, {
            method: "POST"
        });

        const resultado = await respuesta.json();
        agregarSimulacionAlPanel(resultado);

        document.getElementById("nombreSim").value = "";

    } catch (error) {
        console.error("Error al ejecutar simulación:", error);
    } finally {
        cargando.style.display = "none";
        btnEjecutar.disabled = false;
        btnEjecutar.textContent = "▶ Ejecutar simulación";
    }
}

function agregarSimulacionAlPanel(sim) {
    const lista = document.getElementById("listaSimulaciones");

    const tarjeta = document.createElement("div");
    tarjeta.className = "tarjeta-simulacion";
    tarjeta.style.borderColor = "var(--color-azul)";

    const resultadosHtml = sim.resultados.map(r => `
        <div class="resultado-ruta">
            <span>${r.nombreRuta}</span>
            <span class="nivel-${r.nivelTransitoSimulado}">
                ${r.velocidadSimulada} km/h — ${r.nivelTransitoSimulado}
            </span>
        </div>
    `).join("");

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

    lista.prepend(tarjeta);

    setTimeout(() => {
        tarjeta.style.borderColor = "var(--color-borde)";
    }, 3000);
}