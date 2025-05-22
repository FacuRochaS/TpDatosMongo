let chart;

async function consultarAPI(fecha) {
    const res = await fetch(`https://localhost:7130/api/Consultas/consulta1?fechaInicio=${fecha}`);
    return await res.json();
}

async function consultaComparativa() {
    const fecha1 = document.getElementById("fecha1").value;
    const fecha2 = document.getElementById("fecha2").value;

    if (!fecha1 && !fecha2) {
        alert("Seleccioná al menos una fecha.");
        return;
    }

    const fechas = [];
    const datos = [];

    if (fecha1) {
        fechas.push(fecha1);
        datos.push(await consultarAPI(fecha1));
    }

    if (fecha2) {
        fechas.push(fecha2);
        datos.push(await consultarAPI(fecha2));
    }

    const labels = Array.from({ length: 24 }, (_, h) => `${h.toString().padStart(2, "0")}:00`);
    const datasets = [];

    datos.forEach((data, index) => {
        const valores = new Array(24).fill(0);
        data.forEach(d => {
            valores[d.hora] = d.cantidad;
        });

        const colores = [
            ["rgba(59, 130, 246, 1)", "rgba(59, 130, 246, 0.2)"], // azul
            ["rgba(234, 88, 12, 1)", "rgba(234, 88, 12, 0.2)"]    // naranja
        ];

        datasets.push({
            label: `Fecha ${fechas[index]}`,
            data: valores,
            borderColor: colores[index][0],
            backgroundColor: colores[index][1],
            tension: 0.3,
            fill: true
        });
    });

    const ctx = document.getElementById("consulta1").getContext("2d");
    if (chart) chart.destroy();

    chart = new Chart(ctx, {
        type: "line",
        data: {
            labels,
            datasets
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    ticks: { color: "white" },
                    grid: { color: "#2d3748" }
                },
                y: {
                    beginAtZero: true,
                    min: 0,
                    //max: 30, // podés ajustarlo o hacerlo dinámico
                    ticks: { color: "white" },
                    grid: { color: "#2d3748" }
                }
            },
            plugins: {
                legend: { labels: { color: "white" } }
            }
        }
    });
}






let graficoSemanaDia = null;
let graficoMesDia = null;
let graficoHistoricoDia = null;

async function crearGraficoPorDia(canvasId, datos, label, colores, graficoVariable) {
    const ctx = document.getElementById(canvasId).getContext("2d");

    if (graficoVariable && typeof graficoVariable.destroy === "function") {
        graficoVariable.destroy();
    }

    const nuevoGrafico = new Chart(ctx, {
        type: "line",
        data: {
            labels: datos.map(d => d.fecha),
            datasets: [{
                label,
                data: datos.map(d => d.cantidad),
                borderColor: colores[0],
                backgroundColor: colores[1],
                tension: 0.3,
                fill: true
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    ticks: { color: "white", maxRotation: 90, minRotation: 45 },
                    grid: { color: "#2d3748" }
                },
                y: {
                    beginAtZero: true,
                    ticks: { color: "white" },
                    grid: { color: "#2d3748" }
                }
            },
            plugins: {
                legend: { labels: { color: "white" } }
            }
        }
    });

    return nuevoGrafico;
}

async function cargarGraficosPorDia() {
    const colores = {
        semana: ["rgba(59,130,246,1)", "rgba(59,130,246,0.3)"],       // azul
        mes: ["rgba(234,88,12,1)", "rgba(234,88,12,0.3)"],            // naranja
        todo: ["rgba(34,197,94,1)", "rgba(34,197,94,0.3)"]            // verde
    };

    const [semana, mes, todo] = await Promise.all([
        fetch("https://localhost:7130/api/Consultas/mensajesPorDia?rango=semana").then(r => r.json()),
        fetch("https://localhost:7130/api/Consultas/mensajesPorDia?rango=mes").then(r => r.json()),
        fetch("https://localhost:7130/api/Consultas/mensajesPorDia?rango=todo").then(r => r.json())
    ]);

    graficoSemanaDia = await crearGraficoPorDia("graficoSemanaDia", semana, "Última semana", colores.semana, graficoSemanaDia);
    graficoMesDia = await crearGraficoPorDia("graficoMesDia", mes, "Último mes", colores.mes, graficoMesDia);
    graficoHistoricoDia = await crearGraficoPorDia("graficoHistoricoDia", todo, "Histórico", colores.todo, graficoHistoricoDia);
}





let chartCombinado;
let maxY = 0;

document.getElementById("chkDetalle").addEventListener("change", () => {
    const mostrar = document.getElementById("chkDetalle").checked;
    const detalleBox = document.getElementById("checkboxDetalle");

    if (mostrar) {
        detalleBox.classList.remove("max-h-0", "opacity-0");
        detalleBox.classList.add("max-h-32", "opacity-100");
    } else {
        detalleBox.classList.add("max-h-0", "opacity-0");
        detalleBox.classList.remove("max-h-32", "opacity-100");
    }

    actualizarGraficoCombinado();
});

document.getElementById("chkSemana").addEventListener("change", actualizarGraficoCombinado);
document.getElementById("chkFinde").addEventListener("change", actualizarGraficoCombinado);

async function actualizarGraficoCombinado() {
    const verDetalle = document.getElementById("chkDetalle").checked;
    const mostrarSemana = document.getElementById("chkSemana").checked;
    const mostrarFinde = document.getElementById("chkFinde").checked;

    const [resHistorico, resSemanaFinde] = await Promise.all([
        fetch("https://localhost:7130/api/Consultas/MensajesHora").then(r => r.json()),
        fetch("https://localhost:7130/api/Consultas/consulta4").then(r => r.json())
    ]);

    const labels = Array.from({ length: 24 }, (_, h) => `${h.toString().padStart(2, "0")}:00`);
    const datasets = [];

    // Recalcular maxY solo una vez
    if (maxY === 0) {
        maxY = Math.max(
            ...resHistorico.map(d => d.cantidad),
            ...resSemanaFinde.map(d => d.cantidadSemana),
            ...resSemanaFinde.map(d => d.cantidadFinde)
        );
        maxY = Math.ceil(maxY * 1.2); // margen superior
    }

    if (verDetalle) {
        if (mostrarSemana) {
            datasets.push({
                label: "Días de semana",
                data: resSemanaFinde.map(d => d.cantidadSemana),
                backgroundColor: "rgba(59,130,246,0.6)",
                borderColor: "rgba(59,130,246,1)",
                borderWidth: 1,
                stack: "actividad"
            });
        }
        if (mostrarFinde) {
            datasets.push({
                label: "Fines de semana",
                data: resSemanaFinde.map(d => d.cantidadFinde),
                backgroundColor: "rgba(234,88,12,0.6)",
                borderColor: "rgba(234,88,12,1)",
                borderWidth: 1,
                stack: "actividad"
            });
        }
    } else {
        datasets.push({
            label: "Histórico total",
            data: resHistorico.map(d => d.cantidad),
            backgroundColor: "rgba(34,197,94,0.6)",
            borderColor: "rgba(34,197,94,1)",
            borderWidth: 1
        });
    }

    const ctx = document.getElementById("graficoCombinado").getContext("2d");
    if (chartCombinado) chartCombinado.destroy();

    chartCombinado = new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    stacked: verDetalle,
                    ticks: { color: "white" },
                    grid: { color: "#2d3748" }
                },
                y: {
                    beginAtZero: true,
                    max: maxY,
                    stacked: verDetalle,
                    ticks: { color: "white" },
                    grid: { color: "#2d3748" }
                }
            },
            plugins: {
                legend: { labels: { color: "white" } }
            }
        }
    });
}









// ⏯ Eventos
document.getElementById("chkDetalle").addEventListener("change", actualizarGraficoCombinado);


window.addEventListener("DOMContentLoaded", () => {
    actualizarGraficoCombinado();
    cargarGraficosPorDia();
});


