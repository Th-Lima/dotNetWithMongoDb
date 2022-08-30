﻿using dotNetWithMongo.Api.Domain.Enums;
using dotNetWithMongo.Api.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using System.Drawing;

namespace dotNetWithMongo.Api.Domain.Entities
{
    public class Restaurante : AbstractValidator<Restaurante>
    {
        public string Id { get; private set; }

        public string Nome { get; private set; }

        public ECozinha Cozinha { get; private set; }

        public Endereco Endereco { get; private set; }

        public ValidationResult ValidationResult { get; set; }

        public Restaurante(string id, string nome, ECozinha cozinha)
        {
            Id = id;
            Nome = nome;
            Cozinha = cozinha;
        }

        public Restaurante(string nome, ECozinha cozinha)
        {
            Nome = nome;
            Cozinha = cozinha;
        }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
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
