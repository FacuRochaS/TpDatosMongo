using Microsoft.AspNetCore.Mvc;
using MongoApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly EstadisticasService _servicio;

        public ConsultasController(EstadisticasService servicio)
        {
            _servicio = servicio;
        }



        [HttpGet("consulta1")]
        public async Task<IActionResult> GetConsulta1([FromQuery] string fechaInicio)
        {
            if (!DateTime.TryParse(fechaInicio, out var inicio))
                return BadRequest("Formato de fecha inválido");

            var resultado = await _servicio.Consulta1(inicio);
            return Ok(resultado);
        }


        [HttpGet("consulta2")]
        public IActionResult GetPalabrasPorAutor([FromQuery] string autor)
        {
            try
            {
                var datos = _servicio.Consulta2(autor);
                return Ok(datos);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("consulta3")]
        public async Task<IActionResult> GetConsulta3()
        {
            try
            {
                var resultado = await _servicio.Consulta3();
                return Ok(resultado);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("consulta4")]
        public async Task<IActionResult> GetConsulta4()
        {
            try
            {

                var datos = await _servicio.Consulta4();
                return Ok(datos);
            }
            catch (Exception ex)
            {

                return BadRequest("Error al obtener horarios de actividad: " + ex.Message);
            }

        }

        [HttpGet("promedio-respuesta")]
        public async Task<IActionResult> GetPromedioRespuesta()
        {
            var promedio = await _servicio.TiempoPromedioDeRespuesta();
            return Ok(new { promedioSegundos = promedio });
        }

        [HttpGet("actividad-especial")]
        public async Task<IActionResult> GetActividadEspecial()
        {
            var (vacaciones, findes) = await _servicio.ActividadEnVacacionesYFindes();
            return Ok(new { mensajesVacaciones = vacaciones, mensajesFinde = findes });
        }

        [HttpGet("miembros-activos")]
        public async Task<IActionResult> GetMiembrosActivos([FromQuery] int minimo = 5)
        {
            var miembros = await _servicio.MiembrosActivos(minimo);
            return Ok(miembros.Select(m => new { autor = m.Autor, cantidad = m.Cantidad }));
        }




        //----------------------------------Otras consultas--------------------------------
        [HttpGet("MensajesHora")]
        public async Task<IActionResult> GetMensajesHora()
        {
            

            var resultado = await _servicio.MensajesHora();
            return Ok(resultado);
        }

        [HttpGet("MensajesPorAutor")]
        public async Task<IActionResult> GetMensajesPorAutor()
        {
            var resultado = await _servicio.MensajesPorAutor();
            return Ok(resultado);
        }

        [HttpGet("EstadisticasGenerales")]
        public async Task<IActionResult> GetEstadisticasGenerales()
        {
            var resultado = await _servicio.ObtenerEstadisticasGenerales();
            return Ok(resultado);
        }


        [HttpGet("mensajesPorDia")]
        public async Task<IActionResult> GetMensajesPorDia([FromQuery] string rango)
        {
            DateTime? desde = rango switch
            {
                "semana" => DateTime.Today.AddDays(-6),
                "mes" => DateTime.Today.AddDays(-29),
                "todo" or _ => DateTime.Today.AddDays(-365),
            };

            var datos = await _servicio.GetMensajesPorDiaAsync(desde);
            return Ok(datos);
        }

        [HttpGet("tiposMensajes")]
        public async Task<IActionResult> GetTiposMensajes()
        {
            try
            {
                var resultado = await _servicio.ObtenerTiposDeMensajes();
                return Ok(resultado);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
