using dotNetWithMongo.Api.Controllers.Inputs;
using dotNetWithMongo.Api.Domain.Entities;
using dotNetWithMongo.Api.Domain.Enums;
using dotNetWithMongo.Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace dotNetWithMongo.Api.Controller
{
    [ApiController]
    public class RestauranteController : ControllerBase
    {
        private readonly IRestauranteRepository _restauranteRepository;

        public RestauranteController()
        {

        }

        [HttpPost("restaurante")]
        public IActionResult IncluirRestaurante([FromBody] RestauranteInclusao restauranteInclusao)
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
    }
}
