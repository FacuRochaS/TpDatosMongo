namespace MongoApi.Models
{
    public class MensajesPorHoraDTO
    {
        public int Hora { get; set; }           // 0 a 23
        public string HoraTexto { get; set; }   // "00:00", "01:00", etc.
        public int Cantidad { get; set; }
    }

}
