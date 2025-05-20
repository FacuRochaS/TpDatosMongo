 namespace MongoApi.Models
 {
     public class AutorEstadisticasDTO
     {
         public string Autor { get; set; }
         public int TotalMensajes { get; set; }
         public int TotalCaracteres { get; set; }
         public double PromedioCaracteresPorMensaje { get; set; }
     }
 }