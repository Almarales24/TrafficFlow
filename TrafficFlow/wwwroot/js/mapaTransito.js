// Conexión SignalR
const conexion = new signalR.HubConnectionBuilder()
    .withUrl("/concentradorTransito")
    .withAutomaticReconnect()
    .build();

// Mapa Leaflet
let mapa;
let capasRutas = {};

const coloresTransito = {
    bajo: "#39d353",
    medio: "#f0c030",
    alto: "#f85149"
};

function inicializarMapa() {
    mapa = L.map("mapa").setView([4.7110, -74.0721], 13);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: "© OpenStreetMap",
        maxZoom: 19
    }).addTo(mapa);
}

function dibujarRutas(rutas) {
    // Limpiar capas anteriores
    Object.values(capasRutas).forEach(capa => mapa.removeLayer(capa));
    capasRutas = {};

    const panelRutas = document.getElementById("listaRutas");
    panelRutas.innerHTML = "";

    rutas.forEach(ruta => {
        if (!ruta.segmentos || ruta.segmentos.length === 0) return;

        // Dibujar cada segmento en el mapa
        ruta.segmentos.forEach(segmento => {
            const color = coloresTransito[segmento.nivelTransito] || coloresTransito.bajo;

            const linea = L.polyline(
                generarCoordenadasSimuladas(ruta.nombre),
                {
                    color: color,
                    weight: 5,
                    opacity: 0.8
                }
            ).addTo(mapa);

            linea.bindPopup(`
                <b>${ruta.nombre}</b><br>
                ${segmento.desde} → ${segmento.hasta}<br>
                Tránsito: <b>${segmento.nivelTransito}</b><br>
                Velocidad: ${segmento.velocidadKmh} km/h
            `);

            capasRutas[ruta.id] = linea;
        });

        // Tarjeta en panel lateral
        const nivelGeneral = ruta.segmentos[0]?.nivelTransito || "bajo";
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

        tarjeta.addEventListener("click", () => {
            if (capasRutas[ruta.id]) {
                mapa.fitBounds(capasRutas[ruta.id].getBounds());
                capasRutas[ruta.id].openPopup();
            }
        });

        panelRutas.appendChild(tarjeta);
    });
}

// Coordenadas simuladas por nombre de ruta (Bogotá)
function generarCoordenadasSimuladas(nombreRuta) {
    const coordenadas = {
        "Ruta Centro - Norte": [
            [4.5981, -74.0761],
            [4.6200, -74.0700],
            [4.6500, -74.0580],
            [4.6800, -74.0500],
            [4.7110, -74.0300]
        ],
        "Ruta Occidente - Centro": [
            [4.6800, -74.1500],
            [4.6700, -74.1200],
            [4.6600, -74.1000],
            [4.6500, -74.0800],
            [4.6400, -74.0600]
        ],
        "Ruta Sur - Centro": [
            [4.5500, -74.1000],
            [4.5700, -74.0900],
            [4.5900, -74.0800],
            [4.6100, -74.0750],
            [4.6300, -74.0700]
        ]
    };

    return coordenadas[nombreRuta] || [
        [4.6097, -74.0817],
        [4.6500, -74.0600]
    ];
}

// Eventos SignalR
conexion.on("RecibirRutas", (rutas) => {
    dibujarRutas(rutas);
});

// Iniciar conexión
async function iniciar() {
    try {
        await conexion.start();
        console.log("Conectado al concentrador de tránsito");
        await conexion.invoke("ObtenerRutasActualizadas");

        // Actualizar cada 30 segundos
        setInterval(async () => {
            await conexion.invoke("ObtenerRutasActualizadas");
        }, 30000);

    } catch (error) {
        console.error("Error de conexión:", error);
        setTimeout(iniciar, 5000);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    inicializarMapa();
    iniciar();
});