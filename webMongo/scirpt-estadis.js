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

async function cargarEstadisticasGenerales() {
    const res = await fetch("https://localhost:7130/api/Consultas/EstadisticasGenerales");
    const data = await res.json();

    document.getElementById("statTotal").innerText = data.totalMensajes;
    document.getElementById("statUsuarios").innerText = data.totalUsuarios;
    document.getElementById("statDias").innerText = data.diasConActividad;
    document.getElementById("statPromedio").innerText = data.promedioPorDia;
    document.getElementById("statRango").innerText = `${formatearFecha(data.fechaInicio)} - ${formatearFecha(data.fechaFin)}`;
}

async function cargarActividadEspecial() {
    const res = await fetch("https://localhost:7130/api/Consultas/actividad-especial");
    const data = await res.json();

    document.getElementById("statVacaciones").innerText = data.mensajesVacaciones;
    document.getElementById("statFindes").innerText = data.mensajesFinde;
}

async function cargarPromedioRespuesta() {
    const res = await fetch("https://localhost:7130/api/Consultas/promedio-respuesta");
    const data = await res.json();

    document.getElementById("statPromedioRespuesta").innerText = `${Math.round(data.promedioSegundos)}s`;
}

async function cargarMiembrosActivos() {
    const res = await fetch("https://localhost:7130/api/Consultas/miembros-activos?minimo=5");
    const data = await res.json();

    const lista = document.getElementById("listaActivos");
    lista.innerHTML = "";

    if (data.length === 0) {
        lista.innerHTML = "<li class='text-gray-400'>No hay miembros activos en el último mes.</li>";
        return;
    }

    data.forEach(m => {
        const li = document.createElement("li");
        li.innerText = `${m.autor}: ${m.cantidad} mensajes`;
        lista.appendChild(li);
    });
}

function formatearFecha(fechaISO) {
    const fecha = new Date(fechaISO);
    return fecha.toLocaleDateString("es-AR", { day: "2-digit", month: "2-digit", year: "numeric" });
}

window.addEventListener("DOMContentLoaded", () => {
    cargarEstadisticasGenerales();
    cargarActividadEspecial();
    cargarPromedioRespuesta();
    cargarMiembrosActivos();
});



