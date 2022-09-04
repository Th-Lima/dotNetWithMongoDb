using dotNetWithMongo.Api.Data.Schemas;
using dotNetWithMongo.Api.Domain.Entities;
using dotNetWithMongo.Api.Domain.ValueObjects;

namespace dotNetWithMongo.Api.Extensions
{
    public static class RestauranteSchemaExtensao
    {
        public static Restaurante ConverterParaDomain(this RestauranteSchema document)
        {
            var restaurante = new Restaurante(document.Id.ToString(), document.Nome, document.Cozinha);
            var endereco = new Endereco(
                document.Endereco.Logradouro, 
                document.Endereco.Numero, 
                document.Endereco.Cidade, 
                document.Endereco.UF, 
                document.Endereco.Cep);

            restaurante.AtribuirEndereco(endereco);

            return restaurante;
        }
    }
}
