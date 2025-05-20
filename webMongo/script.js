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

async function consultaHistorica() {
    const res = await fetch("https://localhost:7130/api/Consultas/MensajesHora");
    const data = await res.json();

    const labels = data.map(d => d.horaTexto);  // "00:00", ..., "23:00"
    const valores = data.map(d => d.cantidad);

    const ctx = document.getElementById("consultaHistorica").getContext("2d");

    new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Cantidad total de mensajes por hora",
                data: valores,
                backgroundColor: "rgba(34,197,94,0.6)",
                borderColor: "rgba(34,197,94,1)",
                borderWidth: 1
            }]
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
                   // max: 30,
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


async function consultaSemanaFinde() {
    const res = await fetch("https://localhost:7130/api/Consultas/consulta4");
    const data = await res.json();

    const labels = data.map(d => d.horaTexto);
    const valoresSemana = data.map(d => d.cantidadSemana);
    const valoresFinde = data.map(d => d.cantidadFinde);

    const ctx = document.getElementById("actividadSemanaFinde").getContext("2d");

    new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets: [
                {
                    label: "Días de semana",
                    data: valoresSemana,
                    backgroundColor: "rgba(59,130,246,0.6)", // azul
                    borderColor: "rgba(59,130,246,1)",
                    borderWidth: 1
                },
                {
                    label: "Fines de semana",
                    data: valoresFinde,
                    backgroundColor: "rgba(234,88,12,0.6)", // naranja
                    borderColor: "rgba(234,88,12,1)",
                    borderWidth: 1
                }
            ]
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
                    ticks: { color: "white" },
                    grid: { color: "#2d3748" }
                }
            },
            plugins: {
                legend: {
                    labels: { color: "white" }
                }
            }
        }
    });
}


// Llamar al cargar
window.addEventListener("DOMContentLoaded", () => {
    consultaHistorica();
    consultaSemanaFinde(); // 👈 nuevo
});

