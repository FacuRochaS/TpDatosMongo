using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoApi.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("fecha")]
        public string Fecha { get; set; } = "";

        [BsonElement("hora")]
        public string Hora { get; set; } = "";

        [BsonElement("autor")]
        public string Autor { get; set; } = "";

        [BsonElement("mensaje")]
        public string Contenido { get; set; } = "";
    }
}
