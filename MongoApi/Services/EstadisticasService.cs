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
        private static readonly string[] Dias = { "domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado" };

        private static string ObtenerNombreDia(DateTime fecha)
        {
           
            return Dias[(int)fecha.DayOfWeek];
        }

        private static string ObtenerFranjaTexto(int franja)
        {
            int inicio = franja * 2;
            int fin = inicio + 1;
            return $"{inicio:00}:00–{fin:00}:59";
        }




        public async Task<List<MensajesPorFranjaDTO>> Consulta1(DateTime fechaInicio)
        {
            var fechaFin = fechaInicio.Date.AddDays(7);

            var pipeline = new BsonDocument[]
{
            // 1. Combinar fecha y hora en un solo campo datetime (sin zona horaria)
            new BsonDocument("$addFields", new BsonDocument("datetime",
                new BsonDocument("$dateFromString", new BsonDocument
                {
                    { "dateString", new BsonDocument("$concat", new BsonArray { "$fecha", "T", "$hora" }) }
                })
            )),

            // 2. Filtrar por rango de fechas (inicio a inicio + 7 días)
            new BsonDocument("$match", new BsonDocument("datetime",
                new BsonDocument {
                    { "$gte", fechaInicio.Date },
                    { "$lt", fechaInicio.Date.AddDays(7) }
                }
            )),

            // 3. Extraer día de la semana y hora de la fecha
            new BsonDocument("$project", new BsonDocument {
                { "dia", new BsonDocument("$dayOfWeek", "$datetime") },
                { "hora", new BsonDocument("$hour", "$datetime") }
            }),

            // 4. Agrupar por día y franja de 2 horas
            new BsonDocument("$project", new BsonDocument {
                { "dia", 1 },
                { "franja", new BsonDocument("$floor", new BsonDocument("$divide", new BsonArray { "$hora", 2 })) }
            }),

            // 5. Agrupación
            new BsonDocument("$group", new BsonDocument {
                { "_id", new BsonDocument {
                    { "dia", "$dia" },
                    { "franja", "$franja" }
                }},
                { "cantidad", new BsonDocument("$sum", 1) }
            }),

            // 6. Ordenar por día y franja
            new BsonDocument("$sort", new BsonDocument {
                { "_id.dia", 1 },
                { "_id.franja", 1 }
            })
            };


            var results = await _mensajes.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var mapped = results.Select(doc =>
            {
                int diaMongo = doc["_id"]["dia"].ToInt32();  // 1=domingo, 2=lunes...
                int franja = doc["_id"]["franja"].ToInt32();
                int cantidad = doc["cantidad"].ToInt32();

                // Convertir díaMongo (1=domingo) → DayOfWeek enum (0=domingo)
                var diaSem = (DayOfWeek)((diaMongo - 1) % 7);

                // Calcular fecha correspondiente partiendo desde fechaInicio
                var fecha = Enumerable.Range(0, 7)
                    .Select(offset => fechaInicio.Date.AddDays(offset))
                    .FirstOrDefault(f => f.DayOfWeek == diaSem);

                return new MensajesPorFranjaDTO
                {
                    Fecha = fecha,
                    DiaNombre = ObtenerNombreDia(fecha),
                    Franja = franja,
                    FranjaTexto = ObtenerFranjaTexto(franja),
                    Cantidad = cantidad
                };
            }).OrderBy(m => m.Fecha).ThenBy(m => m.Franja).ToList();



            return mapped;
        }






    }
}
