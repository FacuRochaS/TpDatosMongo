namespace MongoApi.Models
{
    public class MensajesPorFranjaDTO
    {
        public DateTime Fecha { get; set; }      // 2025-03-04
        public string DiaNombre { get; set; }    // martes
        public int Franja { get; set; }          // 0..11
        public string FranjaTexto { get; set; }  // 08:00–09:59
        public int Cantidad { get; set; }        // cantidad
    }

}
