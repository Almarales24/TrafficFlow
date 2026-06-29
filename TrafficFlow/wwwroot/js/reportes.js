function mostrarEstado() {
    const estado = document.getElementById("estadoGeneracion");
    const btn = document.querySelector(".btn-descargar");

    estado.style.display = "block";
    btn.style.opacity = "0.6";
    btn.style.pointerEvents = "none";

    setTimeout(() => {
        estado.style.display = "none";
        btn.style.opacity = "1";
        btn.style.pointerEvents = "auto";
    }, 3000);
}