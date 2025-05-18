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

        [HttpGet("consulta")]
        public async Task<IActionResult> ConsultaLibre([FromQuery] string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return BadRequest("Falta el filtro");

            try
            {
                var datos = await _servicio.ConsultaLibre(filter);
                return Ok(datos);
            }
            catch (Exception e)
            {
                return BadRequest("Error en filtro: " + e.Message);
            }
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
        public string GetConsulta2()
        {
            return "value1";
        }

        [HttpGet("consulta3")]
        public string GetConsulta3()
        {
            return "value1";
        }


        [HttpGet("consulta4")]
        public string GetConsulta4()
        {
            return "value1";
        }

        [HttpGet("consulta5")]
        public string GetConsulta5()
        {
            return "value1";
        }



    }
}
