using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotNetWithMongo.Api.Controllers.Inputs;
using dotNetWithMongo.Api.Controllers.Outputs;
using dotNetWithMongo.Api.Data.Repositories;
using dotNetWithMongo.Api.Domain.Entities;
using dotNetWithMongo.Api.Domain.Enums;
using dotNetWithMongo.Api.Domain.ValueObjects;

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

            if(!_restauranteRepository.AlterarCozinha(id, cozinha))
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

            if(restaurante == null)
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
    }
}
