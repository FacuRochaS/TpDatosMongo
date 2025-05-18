using MongoApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoApi.Services
{
    public class EstadisticasService
    {
        private readonly IMongoCollection<Mensaje> _mensajes;

        public EstadisticasService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]); // Link coneccion
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]); //Nombre BD
            _mensajes = database.GetCollection<Mensaje>("grupoWpp"); //Este es el nombre de la coleccion
        }

        public async Task<List<Mensaje>> ConsultaLibre(string filtroJson)
        {
            var filtroBson = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(filtroJson);
            return await _mensajes.Find(filtroBson).Limit(100).ToListAsync();
        }

        //facuu------------------------------------------------------------------------------------------
        private static string ObtenerHoraTexto(int hora)
        {
            return $"{hora:00}:00";
        }

        public async Task<List<MensajesPorHoraDTO>> Consulta1(DateTime fechaInicio)
        {
            var dia = fechaInicio.Date;
            var diaSiguiente = dia.AddDays(1);

            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$addFields", new BsonDocument("datetime",
            new BsonDocument("$dateFromString", new BsonDocument
            {
                { "dateString", new BsonDocument("$concat", new BsonArray { "$fecha", "T", "$hora" }) }
            })
        )),

        new BsonDocument("$match", new BsonDocument("datetime",
            new BsonDocument {
                { "$gte", dia },
                { "$lt", diaSiguiente }
            }
        )),

        new BsonDocument("$project", new BsonDocument {
            { "hora", new BsonDocument("$hour", "$datetime") }
        }),

        new BsonDocument("$group", new BsonDocument {
            { "_id", "$hora" },
            { "cantidad", new BsonDocument("$sum", 1) }
        }),

        new BsonDocument("$sort", new BsonDocument {
            { "_id", 1 }
        })
            };

            var results = await _mensajes.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // Rellenar con 0s para las horas que no existan
            var horasCompletas = Enumerable.Range(0, 24).ToDictionary(h => h, h => 0);

            foreach (var doc in results)
            {
                int hora = doc["_id"].ToInt32();
                int cantidad = doc["cantidad"].ToInt32();
                horasCompletas[hora] = cantidad;
            }

            var lista = horasCompletas.Select(kvp => new MensajesPorHoraDTO
            {
                Hora = kvp.Key,
                HoraTexto = ObtenerHoraTexto(kvp.Key),
                Cantidad = kvp.Value
            }).OrderBy(x => x.Hora).ToList();

            return lista;
        }






    }
}
