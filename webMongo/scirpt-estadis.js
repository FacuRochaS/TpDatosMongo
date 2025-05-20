async function cargarEstadisticasGenerales() {
    try {
        const res = await fetch("https://localhost:7130/api/Consultas/EstadisticasGenerales");
        const data = await res.json();

        document.getElementById("statTotal").innerText = data.totalMensajes;
        document.getElementById("statUsuarios").innerText = data.totalUsuarios;
        document.getElementById("statDias").innerText = data.diasConActividad;
        document.getElementById("statPromedio").innerText = data.promedioPorDia;
        document.getElementById("statRango").innerText = `${formatearFecha(data.fechaInicio)} - ${formatearFecha(data.fechaFin)}`;
    } catch (error) {
        console.error("Error al cargar estadísticas generales:", error);
        alert("No se pudieron cargar las estadísticas.");
    }
}

function formatearFecha(fechaISO) {
    const fecha = new Date(fechaISO);
    return fecha.toLocaleDateString("es-AR", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    });
}

window.addEventListener("DOMContentLoaded", () => {
    cargarEstadisticasGenerales();
});
