// Variables para los gráficos
let graficoMensajes;
let graficoCaracteres;
let graficoPromedio;

// Función para cargar los datos al cargar la página
document.addEventListener('DOMContentLoaded', cargarTopAutores);

async function cargarTopAutores() {
    try {
        const response = await fetch('https://localhost:7130/api/Consultas/consulta3');
        const datos = await response.json();

        // Extraer datos para los gráficos
        const autores = datos.map(item => item.autor);
        const totalMensajes = datos.map(item => item.totalMensajes);
        const totalCaracteres = datos.map(item => item.totalCaracteres);
        const promedioCaracteres = datos.map(item => item.promedioCaracteresPorMensaje);

        // Crear gráficos
        crearGraficoMensajes(autores, totalMensajes);
        crearGraficoCaracteres(autores, totalCaracteres);
        crearGraficoPromedio(autores, promedioCaracteres);

        // Llenar la tabla
        llenarTabla(datos);
    } catch (error) {
        console.error('Error al cargar los datos:', error);
        alert('Error al cargar los datos. Consulta la consola para más detalles.');
    }
}

function crearGraficoMensajes(autores, totalMensajes) {
    const ctx = document.getElementById('mensajesPorAutor').getContext('2d');

    // Destruir el gráfico existente si hay uno
    if (graficoMensajes) {
        graficoMensajes.destroy();
    }

    // Crear nuevo gráfico
    graficoMensajes = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: autores,
            datasets: [{
                label: 'Total de Mensajes',
                data: totalMensajes,
                backgroundColor: 'rgba(59, 130, 246, 0.6)', // Azul
                borderColor: 'rgba(59, 130, 246, 1)',
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y', // Gráfico horizontal para mejor visualización
            responsive: true,
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: { color: 'white' },
                    grid: { color: '#2d3748' }
                },
                y: {
                    ticks: { color: 'white' },
                    grid: { color: '#2d3748' }
                }
            },
            plugins: {
                legend: { labels: { color: 'white' } }
            }
        }
    });
}

function crearGraficoCaracteres(autores, totalCaracteres) {
    const ctx = document.getElementById('caracteresPorAutor').getContext('2d');

    // Destruir el gráfico existente si hay uno
    if (graficoCaracteres) {
        graficoCaracteres.destroy();
    }

    // Crear nuevo gráfico
    graficoCaracteres = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: autores,
            datasets: [{
                label: 'Total de Caracteres',
                data: totalCaracteres,
                backgroundColor: 'rgba(234, 88, 12, 0.6)', // Naranja
                borderColor: 'rgba(234, 88, 12, 1)',
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y', // Gráfico horizontal para mejor visualización
            responsive: true,
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: { color: 'white' },
                    grid: { color: '#2d3748' }
                },
                y: {
                    ticks: { color: 'white' },
                    grid: { color: '#2d3748' }
                }
            },
            plugins: {
                legend: { labels: { color: 'white' } }
            }
        }
    });
}

function crearGraficoPromedio(autores, promedioCaracteres) {
    const ctx = document.getElementById('promedioPorAutor').getContext('2d');

    // Destruir el gráfico existente si hay uno
    if (graficoPromedio) {
        graficoPromedio.destroy();
    }

    // Crear nuevo gráfico
    graficoPromedio = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: autores,
            datasets: [{
                label: 'Promedio de Caracteres por Mensaje',
                data: promedioCaracteres,
                backgroundColor: 'rgba(34, 197, 94, 0.6)', // Verde
                borderColor: 'rgba(34, 197, 94, 1)',
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y', // Gráfico horizontal para mejor visualización
            responsive: true,
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: { color: 'white' },
                    grid: { color: '#2d3748' }
                },
                y: {
                    ticks: { color: 'white' },
                    grid: { color: '#2d3748' }
                }
            },
            plugins: {
                legend: { labels: { color: 'white' } }
            }
        }
    });
}

function llenarTabla(datos) {
    const tablaBody = document.getElementById('tablaAutores');

    // Limpiar la tabla
    tablaBody.innerHTML = '';

    // Añadir filas con datos
    datos.forEach(autor => {
        const fila = document.createElement('tr');
        fila.innerHTML = `
            <td class="px-4 py-3 whitespace-nowrap">${autor.autor}</td>
            <td class="px-4 py-3 whitespace-nowrap">${autor.totalMensajes.toLocaleString()}</td>
            <td class="px-4 py-3 whitespace-nowrap">${autor.totalCaracteres.toLocaleString()}</td>
            <td class="px-4 py-3 whitespace-nowrap">${autor.promedioCaracteresPorMensaje.toLocaleString(undefined, {minimumFractionDigits: 2, maximumFractionDigits: 2})}</td>
        `;
        fila.className = 'hover:bg-gray-600';
        tablaBody.appendChild(fila);
    });
}