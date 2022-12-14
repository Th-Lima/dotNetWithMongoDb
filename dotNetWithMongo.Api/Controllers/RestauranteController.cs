using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotNetWithMongo.Api.Controllers.Inputs;
using dotNetWithMongo.Api.Controllers.Outputs;
using dotNetWithMongo.Api.Data.Repositories;
using dotNetWithMongo.Api.Domain.Entities;
using dotNetWithMongo.Api.Domain.Enums;
using dotNetWithMongo.Api.Domain.ValueObjects;
using System;

namespace dotNetWithMongo.Api.Controller
{
    [ApiController]
    public class RestauranteController : ControllerBase
    {
        private readonly RestauranteRepository _restauranteRepository;

        public RestauranteController(RestauranteRepository restauranteRepository)
        {
            _restauranteRepository = restauranteRepository;
        }

        [HttpPost("restaurante")]
        public ActionResult IncluirRestaurante([FromBody] RestauranteInclusao restauranteInclusao)
        {
            var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteInclusao.Cozinha);

            var restaurante = new Restaurante(restauranteInclusao.Nome, cozinha);
            var endereco = new Endereco
                (restauranteInclusao.Logradouro,
                restauranteInclusao.Numero,
                restauranteInclusao.Cidade,
                restauranteInclusao.UF,
                restauranteInclusao.Cep);

            restaurante.AtribuirEndereco(endereco);

            if (!restaurante.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = restaurante.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                    });
            }

            _restauranteRepository.Inserir(restaurante);

            return Ok(new { data = "Restaurante inserido com sucesso" });
        }

        [HttpGet("restaurante/todos")]
        public async Task<ActionResult> ObterRestaurantes()
        {
            var restaurantes = await _restauranteRepository.ObterTodos();

            var listagem = restaurantes.Select(_ => new RestauranteListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Cozinha = (int)_.Cozinha,
                Cidade = _.Endereco.Cidade,
            });

            return Ok(
                new
                {
                    data = listagem
                });
        }

        [HttpGet("restaurante/{id}")]
        public ActionResult ObterPorId(string id)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);

            if (restaurante == null)
                return NotFound("Não encontramos este restaurante");

            var exibicao = new RestauranteExibicao
            {
                Id = restaurante.Id,
                Nome = restaurante.Nome,
                Cozinha = (int)restaurante.Cozinha,
                Endereco = new EnderecoExibicao
                {
                    Cep = restaurante.Endereco.Cep,
                    Cidade = restaurante.Endereco.Cidade,
                    Logradouro = restaurante.Endereco.Logradouro,
                    Numero = restaurante.Endereco.Numero,
                    UF = restaurante.Endereco.UF,
                }
            };

            return Ok(new
            {
                data = exibicao
            });
        }

        [HttpGet("restaurante")]
        public ActionResult ObterPorNome([FromQuery] string nome)
        {
            var restaurantes = _restauranteRepository.ObterPorNome(nome);

            if (restaurantes == null || !restaurantes.Any())
                return NotFound("Não encontramos este restaurante");

            var listagem = restaurantes.Select(_ => new RestauranteListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Cozinha = (int)_.Cozinha,
                Cidade = _.Endereco.Cidade
            });

            return Ok(new
            {
                data = listagem
            });
        }

        [HttpPut("restaurante")]
        public ActionResult AlterarCompleto([FromBody] RestauranteAlteracaoCompleto restauranteAlteracaoCompleto)
        {
            var restaurante = _restauranteRepository.ObterPorId(restauranteAlteracaoCompleto.Id);

            if (restaurante == null)
                return NotFound("Não encontramos este restaurante");

            var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteAlteracaoCompleto.Cozinha);
            restaurante = new Restaurante(restauranteAlteracaoCompleto.Id, restauranteAlteracaoCompleto.Nome, cozinha);
            var endereco = new Endereco(
                restauranteAlteracaoCompleto.Logradouro,
                restauranteAlteracaoCompleto.Numero,
                restauranteAlteracaoCompleto.Cidade,
                restauranteAlteracaoCompleto.UF,
                restauranteAlteracaoCompleto.Cep);

            restaurante.AtribuirEndereco(endereco);

            if (!restaurante.Validar())
            {
                return BadRequest(new
                {
                    errors = restaurante.ValidationResult.Errors.Select(x => x.ErrorMessage)
                });
            }

            if (!_restauranteRepository.AlterarCompleto(restaurante))
            {
                return BadRequest(new
                {
                    errors = "Nenhum documento foi alterado"
                });
            }

            return Ok(new
            {
                data = $"Restaurante alterado com sucesso!!  RESTAURANTE: {restaurante.Nome}"
            });
        }

        [HttpPatch("restaurante/{id}")]
        public ActionResult AlterarCozinha(string id, [FromBody] RestauranteAlteracaoParcial restauranteAlteracaoParcial)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);

            if (restaurante == null)
                return NotFound("Não encontramos este restaurante");

            var cozinha = ECozinhaHelper.ConverterDeInteiro(restauranteAlteracaoParcial.Cozinha);

            if (!_restauranteRepository.AlterarCozinha(id, cozinha))
            {
                return BadRequest(new
                {
                    errors = "Nenhum documento foi alterado"
                });
            }

            return Ok(new
            {
                data = $"Restaurante alterado com sucesso!! RESTAURANTE: {restaurante.Nome}"
            });
        }

        [HttpPatch("restaurante/{id}/avaliar")]
        public ActionResult Avaliar(string id, [FromBody] AvaliacaoInclusao avaliacaoInclusao)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);

            if (restaurante == null)
                return NotFound("Não encontramos este restaurante");

            var avaliacao = new Avaliacao(avaliacaoInclusao.Estrelas, avaliacaoInclusao.Comentario, restaurante.Nome);

            if (!avaliacao.Validar())
            {
                return BadRequest(new
                {
                    errors = avaliacao.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                });
            }

            _restauranteRepository.Avaliar(id, avaliacao);

            return Ok(new
            {
                data = $"Restaurante {restaurante.Nome} avaliado com sucesso!"
            });
        }

        [HttpGet("restaurante/top3")]
        public async Task<ActionResult> ObterTopTresRestaurantes()
        {
            var topTres = await _restauranteRepository.ObterTopTres();

            var listagem = topTres.Select(_ => new RestauranteTopTres
            {
                Id = _.Key.Id,
                Nome = _.Key.Nome,
                Cidade = _.Key.Endereco.Cidade,
                Cozinha = (int)_.Key.Cozinha,
                Media_Estrelas_Nota = Math.Round(_.Value, 1),
                Comentarios = _.Key.Avaliacoes.Select(x => x.Comentario).ToList()
            });

            return Ok(new
            {
                data = listagem
            });
        }

        [HttpGet("restaurante/top3-lookup")]
        public async Task<ActionResult> ObterTop3RestaurantesComLookup()
        {
            var top3 = await _restauranteRepository.ObterTopTresComLookup();

            var listagem = top3.Select(_ => new RestauranteTopTres
            {
                Id = _.Key.Id,
                Nome = _.Key.Nome,
                Cozinha = (int)_.Key.Cozinha,
                Cidade = _.Key.Endereco.Cidade,
                Media_Estrelas_Nota = Math.Round(_.Value, 1),
                Comentarios = _.Key.Avaliacoes.Select(x => x.Comentario).ToList()
            });

            return Ok(
                new
                {
                    data = listagem
                }
            );
        }

        [HttpDelete("restaurante/{id}")]
        public ActionResult Remover(string id)
        {
            var restaurante = _restauranteRepository.ObterPorId(id);

            if (restaurante == null)
                return NotFound("Não encontramos este restaurante");

            (var totalRestauranteRemovido, var totalAvaliacoesRemovidas) = _restauranteRepository.Remover(id);

            return Ok(new
            {
                data = $"Total de exclusões: {totalRestauranteRemovido} restaurantes com {totalAvaliacoesRemovidas} avaliações"
            });
        }

        [HttpGet("restaurante/textual")]
        public async Task<ActionResult> ObterRestaurantePorBuscaTextual([FromQuery] string texto)
        {
            var restaurantes = await _restauranteRepository.ObterBuscaPorTexto(texto);

            if (restaurantes == null || !restaurantes.Any())
                return NotFound("Não encontramos restaurantes com este texto");

            var listagem = restaurantes.ToList().Select(_ => new RestauranteListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Cozinha = (int)_.Cozinha,
                Cidade = _.Endereco.Cidade
            });

            return Ok(new
            {
                data = listagem
            });
        }
    }
}
