namespace MongoApi.Models
{
    public class EstadisticasGeneralesDTO
    {
        public int TotalMensajes { get; set; }
        public int TotalUsuarios { get; set; }
        public int DiasConActividad { get; set; }
        public double PromedioPorDia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

}
