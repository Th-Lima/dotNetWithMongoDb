using dotNetWithMongo.Api.Data.Schemas;
using dotNetWithMongo.Api.Domain.ValueObjects;

namespace dotNetWithMongo.Api.Extensions
{
    public static class AvaliacaoSchemaExtensao
    {
        public static Avaliacao ConverterParaDomain(this AvaliacaoSchema document)
        {
            return new Avaliacao(document.Estrelas, document.Comentario, document.NomeRestaurante);
        }
    }
}
