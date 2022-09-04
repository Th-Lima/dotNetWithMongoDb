using dotNetWithMongo.Api.Data.Schemas;
using dotNetWithMongo.Api.Domain.Entities;
using dotNetWithMongo.Api.Domain.ValueObjects;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using dotNetWithMongo.Api.Extensions;
using dotNetWithMongo.Api.Domain.Enums;

namespace dotNetWithMongo.Api.Data.Repositories
{
    public class RestauranteRepository
    {
        IMongoCollection<RestauranteSchema> _restaurantes;

        public RestauranteRepository(MongoDb mongoDb)
        {
            _restaurantes = mongoDb.Db.GetCollection<RestauranteSchema>("restaurantes");
        }

        public void Inserir(Restaurante restaurante)
        {
            var document = new RestauranteSchema
            {
                Nome = restaurante.Nome,
                Cozinha = restaurante.Cozinha,
                Endereco = new EnderecoSchema
                {
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    Cidade = restaurante.Endereco.Cidade,
                    Cep = restaurante.Endereco.Cep,
                    UF = restaurante.Endereco.UF
                }
            };

            _restaurantes.InsertOne(document);
        }

        public async Task<IEnumerable<Restaurante>> ObterTodos()
        { 
            var restaurantes = new List<Restaurante>();

            var filter = Builders<RestauranteSchema>.Filter.Empty;
            await _restaurantes.AsQueryable().ForEachAsync(d =>
            {
                var r = new Restaurante(d.Id, d.Nome, d.Cozinha);
                var e = new Endereco(d.Endereco.Logradouro, d.Endereco.Numero, d.Endereco.Cidade, d.Endereco.UF, d.Endereco.Cep);
                r.AtribuirEndereco(e);
                restaurantes.Add(r);
            });

            return restaurantes;
        }

        public Restaurante ObterPorId(string id)
        {
            var document = _restaurantes.AsQueryable().FirstOrDefault(_ => _.Id == id);

            return document?.ConverterParaDomain();
        }

        public IEnumerable<Restaurante> ObterPorNome(string nome)
        {
            var restaurantes = new List<Restaurante>();

            _restaurantes.AsQueryable()
                 .Where(_ => _.Nome.ToLower().Contains(nome.ToLower()))
                 .ToList()
                 .ForEach(d => restaurantes.Add(d.ConverterParaDomain()));

            return restaurantes;
        }

        public bool AlterarCompleto(Restaurante restaurante)
        {
            var documento = new RestauranteSchema
            {
                Id = restaurante.Id,
                Nome = restaurante.Nome,
                Cozinha = restaurante.Cozinha,
                Endereco = new EnderecoSchema
                {
                    Cep = restaurante.Endereco.Cep,
                    Cidade = restaurante.Endereco.Cidade,
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    UF = restaurante.Endereco.UF,
                }
            };

            var resultado = _restaurantes.ReplaceOne(_ => _.Id == documento.Id, documento);

            return resultado.ModifiedCount > 0;
        }

        public bool AlterarCozinha(string id, ECozinha cozinha)
        {
            var atualizacao = Builders<RestauranteSchema>.Update.Set(_ => _.Cozinha, cozinha);

            var resultado = _restaurantes.UpdateOne(_ => _.Id == id, atualizacao);

            return resultado.ModifiedCount > 0;
        }
    }
}
