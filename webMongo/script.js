const franjas = Array.from({ length: 12 }, (_, i) =>
    `${(i * 2).toString().padStart(2, '0')}:00–${(i * 2 + 1).toString().padStart(2, '0')}:59`
);

let chart;

async function consultarAPI() {
    const fecha = document.getElementById("fecha").value;
    if (!fecha) return alert("Seleccioná una fecha");

    const res = await fetch(`https://localhost:7130/api/Consultas/consulta1?fechaInicio=${fecha}`);
    const data = await res.json();

    const fechas = [...new Set(data.map(d => d.fecha))].sort();

    const seriesPorFranja = franjas.map((_, franjaIdx) => {
        return fechas.map(fecha => {
            const item = data.find(x => x.fecha === fecha && x.franja === franjaIdx);
            return {
                time: fecha,
                value: item ? item.cantidad : 0
            };
        });
    });

    // Limpiar gráfico anterior
    document.getElementById('chart').innerHTML = '';
    chart = LightweightCharts.createChart(document.getElementById('chart'), {
        layout: {
            background: { color: '#111827' },
            textColor: '#ffffff'
        },
        width: document.getElementById('chart').clientWidth,
        height: 400,
        timeScale: {
            timeVisible: true,
            borderColor: '#333'
        },
        grid: {
            vertLines: { color: '#2d3748' },
            horzLines: { color: '#2d3748' }
        }
    });

    const colores = [
        '#60a5fa', '#3b82f6', '#1d4ed8', '#9333ea', '#e11d48', '#f97316',
        '#84cc16', '#14b8a6', '#06b6d4', '#0ea5e9', '#4ade80', '#c084fc'
    ];

    seriesPorFranja.forEach((serie, idx) => {
        const s = chart.addLineSeries({
            color: colores[idx % colores.length],
            lineWidth: 2,
            title: franjas[idx]
        });
        s.setData(serie);
    });
}
