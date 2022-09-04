using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotNetWithMongo.Api.Data.Schemas
{
    public class AvaliacaoSchema
    {
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string RestauranteId { get; set; }

        public int Estrelas { get; set; }

        public string Comentario { get; set; }

        public string NomeRestaurante { get; set; }
    }
}
