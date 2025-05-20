namespace MongoApi.Models
{
    public class HorarioActividadDto
    {
        public int Hora { get; set; }             // 0 a 23
        public string HoraTexto { get; set; }     // "00:00", "01:00", etc.
        public int CantidadSemana { get; set; }   // Lunes a Viernes
        public int CantidadFinde { get; set; }    // Sábado y Domingo
    }
}
