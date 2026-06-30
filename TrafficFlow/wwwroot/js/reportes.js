// Muestra el estado de generación del PDF y bloquea el botón mientras se procesa
function mostrarEstado() {

    // Obtiene el elemento de estado y el botón de descarga
    const estado = document.getElementById("estadoGeneracion");
    const btn = document.querySelector(".btn-descargar");

    // Hace visible el mensaje de generación y desactiva el botón visualmente
    estado.style.display = "block";
    btn.style.opacity = "0.6";
    btn.style.pointerEvents = "none";

    // Restaura el botón y oculta el mensaje después de 3 segundos
    setTimeout(() => {
        estado.style.display = "none";
        btn.style.opacity = "1";
        btn.style.pointerEvents = "auto";
    }, 3000);
}