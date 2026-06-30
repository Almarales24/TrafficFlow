// Crea la conexión con el hub de SignalR apuntando al endpoint del concentrador de tránsito
const conexion = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect() // Reconecta automáticamente si se pierde la conexión
    .build();

let mapa; // Instancia del mapa Leaflet
let capasRutas = {}; // Diccionario que almacena las líneas dibujadas por ID de ruta

// Colores asociados a cada nivel de tránsito para pintar las rutas en el mapa
const coloresTransito = {
    bajo: "#39d353",
    medio: "#f0c030",
    alto: "#f85149"
};

// Inicializa el mapa Leaflet centrado en Bogotá y agrega las capas de TomTom
function inicializarMapa() {
    mapa = L.map("mapa").setView([4.7110, -74.0721], 13); // Centra el mapa en Bogotá con zoom 13

    // Capa base TomTom
    L.tileLayer(
        `https://api.tomtom.com/map/1/tile/basic/main/{z}/{x}/{y}.png?key=pXvYWN3CLrPH2zJSVRSsogEsUpzTICVA`,
        { attribution: "© TomTom", maxZoom: 19 }
    ).addTo(mapa);

    // Capa de tráfico en tiempo real
    L.tileLayer(
        `https://api.tomtom.com/traffic/map/4/tile/flow/relative/{z}/{x}/{y}.png?key=pXvYWN3CLrPH2zJSVRSsogEsUpzTICVA`,
        { attribution: "© TomTom Traffic", maxZoom: 19, opacity: 0.7 }
    ).addTo(mapa);

    // Capa de incidentes TomTom
    L.tileLayer(
        `https://api.tomtom.com/traffic/map/4/tile/incidents/s3/{z}/{x}/{y}.png?key=pXvYWN3CLrPH2zJSVRSsogEsUpzTICVA`,
        { attribution: "© TomTom Incidents", maxZoom: 19, opacity: 0.9 }
    ).addTo(mapa);
}


// Borra las rutas actuales del mapa y las redibuja con los datos nuevos recibidos
function dibujarRutas(rutas) {
    Object.values(capasRutas).forEach(capa => mapa.removeLayer(capa)); // Elimina las capas anteriores
    capasRutas = {};

    const panelRutas = document.getElementById("listaRutas");
    panelRutas.innerHTML = ""; // Limpia el panel lateral de rutas

    rutas.forEach(ruta => {
        if (!ruta.segmentos || ruta.segmentos.length === 0) return; // Omite rutas sin segmentos

        ruta.segmentos.forEach(segmento => {
            const color = coloresTransito[segmento.nivelTransito] || coloresTransito.bajo; // Color según nivel de tráfico
            const linea = L.polyline(
                generarCoordenadasSimuladas(ruta.nombre), // Coordenadas predefinidas por nombre de ruta
                { color: color, weight: 5, opacity: 0.8 }
            ).addTo(mapa);

            // Ventana emergente con los detalles del segmento al hacer clic
            linea.bindPopup(`
                <b>${ruta.nombre}</b><br>
                ${segmento.desde} → ${segmento.hasta}<br>
                Tránsito: <b>${segmento.nivelTransito}</b><br>
                Velocidad: ${segmento.velocidadKmh} km/h
            `);

            capasRutas[ruta.id] = linea; // Registra la línea en el diccionario por ID de ruta
        });

        const nivelGeneral = ruta.segmentos[0]?.nivelTransito || "bajo"; // Nivel del primer segmento como representativo
        const tarjeta = document.createElement("div");
        tarjeta.className = "tarjeta-ruta";
        tarjeta.innerHTML = `
            <h3>${ruta.nombre}</h3>
            <p>${ruta.origen} → ${ruta.destino}</p>
            <p class="nivel-${nivelGeneral}">
                ● Tránsito ${nivelGeneral} — ${ruta.segmentos[0]?.velocidadKmh} km/h
            </p>
            <p style="font-size:0.75rem; margin-top:4px; color:#555;">
                Actualizado: ${new Date(ruta.actualizadoEn).toLocaleTimeString()}
            </p>
        `;

        // Al hacer clic en la tarjeta, centra el mapa en esa ruta y abre su popup
        tarjeta.addEventListener("click", () => {
            if (capasRutas[ruta.id]) {
                mapa.fitBounds(capasRutas[ruta.id].getBounds());
                capasRutas[ruta.id].openPopup();
            }
        });

        panelRutas.appendChild(tarjeta); // Agrega la tarjeta al panel lateral
    });
}

// Devuelve las coordenadas predefinidas para cada ruta conocida; usa un par por defecto si no se encuentra
function generarCoordenadasSimuladas(nombreRuta) {
    const coordenadas = {
        "Ruta Centro - Norte": [
            [4.5981, -74.0761], [4.6200, -74.0700],
            [4.6500, -74.0580], [4.6800, -74.0500], [4.7110, -74.0300]
        ],
        "Ruta Occidente - Centro": [
            [4.6800, -74.1500], [4.6700, -74.1200],
            [4.6600, -74.1000], [4.6500, -74.0800], [4.6400, -74.0600]
        ],
        "Ruta Sur - Centro": [
            [4.5500, -74.1000], [4.5700, -74.0900],
            [4.5900, -74.0800], [4.6100, -74.0750], [4.6300, -74.0700]
        ]
    };
    return coordenadas[nombreRuta] || [[4.6097, -74.0817], [4.6500, -74.0600]]; // Coordenadas de respaldo
}

// Alterna entre modo día y modo nocturno en el mapa y el tema visual
function toggleModoNocturno() {
    const body = document.body;
    const btn = document.getElementById("btnModoNocturno");
    const esDia = body.classList.toggle("modo-dia"); // Activa o desactiva la clase de modo día

    // Elimina todas las capas de teselas del mapa para reemplazarlas
    mapa.eachLayer(layer => {
        if (layer._url) mapa.removeLayer(layer);
    });

    if (esDia) {
        btn.textContent = "🌙 Modo oscuro"; // Cambia el texto del botón al modo opuesto
        L.tileLayer(
            `https://api.tomtom.com/map/1/tile/basic/main/{z}/{x}/{y}.png?key=pXvYWN3CLrPH2zJSVRSsogEsUpzTICVA`,
            { attribution: "© TomTom", maxZoom: 19 }
        ).addTo(mapa);
    } else {
        btn.textContent = "☀️ Modo claro";
        L.tileLayer(
            `https://api.tomtom.com/map/1/tile/basic/night/{z}/{x}/{y}.png?key=pXvYWN3CLrPH2zJSVRSsogEsUpzTICVA`,
            { attribution: "© TomTom", maxZoom: 19 }
        ).addTo(mapa);
    }

    // Volver a agregar capa de tráfico
    L.tileLayer(
        `https://api.tomtom.com/traffic/map/4/tile/flow/relative/{z}/{x}/{y}.png?key=pXvYWN3CLrPH2zJSVRSsogEsUpzTICVA`,
        { attribution: "© TomTom Traffic", maxZoom: 19, opacity: 0.7 }
    ).addTo(mapa);
}

// Cuando el servidor envía rutas actualizadas, las redibuja en el mapa
conexion.on("RecibirRutas", (rutas) => {
    dibujarRutas(rutas);
});

// Inicia la conexión con SignalR, solicita rutas inmediatamente y repite cada 30 segundos
async function iniciar() {
    try {
        await conexion.start(); // Establece la conexión
        await conexion.invoke("ObtenerRutasActualizadas"); // Solicita las rutas al servidor
        setInterval(async () => {
            await conexion.invoke("ObtenerRutasActualizadas"); // Refresca las rutas cada 30 s
        }, 30000);
    } catch (error) {
        console.error("Error de conexión:", error);
        setTimeout(iniciar, 5000); // Reintenta la conexión tras 5 segundos
    }
}

// Ejecuta la inicialización del mapa y la conexión cuando el DOM está listo
document.addEventListener("DOMContentLoaded", () => {
    inicializarMapa();
    iniciar();
});