﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace dotNetWithMongo.Api.Data.Schemas
{
    public class RestauranteAvaliacaoSchema
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id { get; set; }

        public double MediaEstrelas { get; set; }

        public List<RestauranteSchema> Restaurantes { get; set; }

        public List<AvaliacaoSchema> Avaliacoes { get; set; }
    }
}
