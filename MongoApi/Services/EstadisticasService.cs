using MongoApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoApi.Services
{
    public class EstadisticasService
    {
        private readonly IMongoCollection<Mensaje> _mensajes;

        public EstadisticasService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]); // Link coneccion
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]); //Nombre BD
            _mensajes = database.GetCollection<Mensaje>("fashionWorld");
           // _mensajes = database.GetCollection<Mensaje>("grupoWpp"); //Este es el nombre de la coleccion
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

        public List<PalabraFrecuenteDTO> Consulta2(string fecha) /////FEDE
        {
            /*
            db.chat.aggregate([
            { $match: { fechaDelMensaje: "2025-05-19" } }, // Aquí se pasa el día como parámetro desde la API
            { $project: { palabras: { $split: ["$contenido", " "] } } },
            { $unwind: "$palabras" },
            { $group: { _id: "$palabras", count: { $sum: 1 } } },
            { $sort: { count: -1 } },
            { $limit: 10 } // Obtener las 10 palabras más frecuentes
            ]);
             */
            var pipeline = new[]
            {
            new BsonDocument { { "$match", new BsonDocument { { "fechaDelMensaje", fecha } } } },
            new BsonDocument { { "$project", new BsonDocument { { "palabras", new BsonDocument { { "$split", new BsonArray { "$contenido", " " } } } } } } },
            new BsonDocument { { "$unwind", "$palabras" } },
            new BsonDocument { { "$group", new BsonDocument { { "_id", "$palabras" }, { "count", new BsonDocument { { "$sum", 1 } } } } } },
            new BsonDocument { { "$sort", new BsonDocument { { "count", -1 } } } },
            new BsonDocument { { "$limit", 10 } }
            };

            var resultado = _mensajes.Aggregate<BsonDocument>(pipeline).ToList();

            return resultado.Select(doc => new PalabraFrecuenteDTO
            {
                Palabra = doc["_id"].AsString,
                Frecuencia = doc["count"].ToInt32()
            }).ToList();
        }

        //----------------------Nacho----------------------------------------------------------------------

        //--------------------Tincho, martin, furlan, nuca supe como queres que te digan xddd jaja----------

        //----------------------------Santi----------------------------------------------------------------



        //-------------------------------Extrasss-------------------------------------------------
        public async Task<List<MensajesPorHoraDTO>> MensajesHora()
        {
            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$addFields", new BsonDocument("datetime",
            new BsonDocument("$dateFromString", new BsonDocument
            {
                { "dateString", new BsonDocument("$concat", new BsonArray { "$fecha", "T", "$hora" }) }
            })
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

        public async Task<List<MensajesPorAutorDTO>> MensajesPorAutor()
        {
            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$autor" },
            { "cantidad", new BsonDocument("$sum", 1) }
        }),
        new BsonDocument("$sort", new BsonDocument("cantidad", -1))
            };

            var resultados = await _mensajes.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return resultados.Select(doc => new MensajesPorAutorDTO
            {
                Autor = doc["_id"].AsString,
                Cantidad = doc["cantidad"].ToInt32()
            }).ToList();
        }

        public async Task<EstadisticasGeneralesDTO> ObtenerEstadisticasGenerales()
        {
            // Contar todos los mensajes
            var totalMensajes = await _mensajes.CountDocumentsAsync(FilterDefinition<Mensaje>.Empty);

            // Obtener autores únicos
            var totalUsuarios = await _mensajes
                .Distinct<string>("autor", FilterDefinition<Mensaje>.Empty)
                .ToListAsync();

            // Pipeline para obtener fecha de primer y último mensaje
            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$addFields", new BsonDocument("datetime",
            new BsonDocument("$dateFromString", new BsonDocument
            {
                { "dateString", new BsonDocument("$concat", new BsonArray { "$fecha", "T", "$hora" }) }
            })
        )),
        new BsonDocument("$sort", new BsonDocument("datetime", 1)),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", BsonNull.Value },
            { "fechaInicio", new BsonDocument("$first", "$datetime") },
            { "fechaFin", new BsonDocument("$last", "$datetime") }
        })
            };

            var fechasDoc = await _mensajes.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            // Manejar el caso en que no haya fechas (colección vacía)
            DateTime fechaInicio, fechaFin;
            int dias;

            if (fechasDoc != null)
            {
                fechaInicio = fechasDoc["fechaInicio"].ToUniversalTime().Date;
                fechaFin = fechasDoc["fechaFin"].ToUniversalTime().Date;
                dias = (fechaFin - fechaInicio).Days + 1;
            }
            else
            {
                fechaInicio = DateTime.MinValue;
                fechaFin = DateTime.MinValue;
                dias = 0;
            }

            return new EstadisticasGeneralesDTO
            {
                TotalMensajes = (int)totalMensajes,
                TotalUsuarios = totalUsuarios.Count,
                DiasConActividad = dias,
                PromedioPorDia = dias > 0 ? Math.Round(totalMensajes / (double)dias, 1) : 0,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };
        }


























    }
}
