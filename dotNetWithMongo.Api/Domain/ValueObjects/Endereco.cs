using FluentValidation;
using FluentValidation.Results;

namespace dotNetWithMongo.Api.Domain.ValueObjects
{
    public class Endereco : AbstractValidator<Endereco>
    {
        public string Logradouro { get; private set; }

        public string Numero { get; private set; }

        public string Cidade { get; private set; }

        public string UF { get; private set; }

        public string Cep { get; private set; }

        public ValidationResult ValidationResult { get; set; }

        public Endereco(string logradouro, string numero, string cidade, string uf, string cep)
        {
            Logradouro = logradouro;
            Numero = numero;
            Cidade = cidade;
            UF = uf;
            Cep = cep;
        }

        public virtual bool Validar()
        {
            ValidarLogradouro();
            ValidarCidade();
            ValidarUf();
            ValidarCep();

            ValidationResult = Validate(this);

            return ValidationResult.IsValid;
        }

        private void ValidarCep()
        {
            RuleFor(c => c.Cep)
               .NotEmpty().WithMessage("Cep não pode ser vazio")
               .MaximumLength(8).WithMessage("Cep pode ter no máximo 8 caractes");
        }

        private void ValidarUf()
        {
            RuleFor(c => c.UF)
                .NotEmpty().WithMessage("UF não pode ser vazio")
                .MaximumLength(2).WithMessage("UF pode ter no máximo 2 caractes");
        }

        private void ValidarCidade()
        {
            RuleFor(c => c.Cidade)
                .NotEmpty().WithMessage("Cidade não pode ser vazio")
                .MaximumLength(100).WithMessage("Cidade pode ter no máximo 100 caractes");
        }

        private void ValidarLogradouro()
        {
            RuleFor(c => c.Logradouro)
                .NotEmpty().WithMessage("O Logradouro não pode ser vazio")
                .MaximumLength(50).WithMessage("Logradouro pode ter no máximo 50 caracteres");
        }
    }
}
