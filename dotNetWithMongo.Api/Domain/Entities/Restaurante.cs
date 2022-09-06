using dotNetWithMongo.Api.Domain.Enums;
using dotNetWithMongo.Api.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace dotNetWithMongo.Api.Domain.Entities
{
    public class Restaurante : AbstractValidator<Restaurante>
    {
        public string Id { get; private set; }

        public string Nome { get; private set; }

        public ECozinha Cozinha { get; private set; }

        public Endereco Endereco { get; private set; }

        public List<Avaliacao> Avaliacoes { get; private set; }

        public ValidationResult ValidationResult { get; set; }

        public Restaurante(string id, string nome, ECozinha cozinha)
        {
            Id = id;
            Nome = nome;
            Cozinha = cozinha;
            Avaliacoes = new List<Avaliacao>();
        }

        public Restaurante(string nome, ECozinha cozinha)
        {
            Nome = nome;
            Cozinha = cozinha;
            Avaliacoes = new List<Avaliacao>();
        }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public void InserirAvaliacao(Avaliacao avaliacao)
        {
            Avaliacoes.Add(avaliacao);
        }

        public virtual bool Validar()
        {
            ValidarNome();
            ValidationResult = Validate(this);

            ValidarEndereco();

            return ValidationResult.IsValid;
        }

        private void ValidarNome()
        {
            RuleFor(c => c.Nome)
               .NotEmpty().WithMessage("Nome não pode ser vazio")
               .MaximumLength(30).WithMessage("Nome pode ter no máximo 30 caractes");
        }

        private void ValidarEndereco()
        {
            if (Endereco.Validar())
                return;

            foreach (var erro in Endereco.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);
        }
    }
}
