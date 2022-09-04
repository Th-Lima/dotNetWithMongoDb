using FluentValidation;
using FluentValidation.Results;

namespace dotNetWithMongo.Api.Domain.ValueObjects
{
    public class Avaliacao : AbstractValidator<Avaliacao>
    {
        public int Estrelas { get; private set; }

        public string Comentario { get; private set; }

        public string NomeRestaurante { get; set; }

        public ValidationResult ValidationResult { get; set; }

        public Avaliacao(int estrelas, string comentario, string nomeRestaurante)
        {
            Estrelas = estrelas;
            Comentario = comentario;
            NomeRestaurante = nomeRestaurante;
        }

        public virtual bool Validar()
        {
            ValidarEstrelas();
            ValidarComentario();

            ValidationResult = Validate(this);

            return ValidationResult.IsValid;
        }

        private void ValidarComentario()
        {
            RuleFor(c => c.Comentario)
               .MaximumLength(100).WithMessage("Comentario pode ter no máximo 100 caractes");
        }

        private void ValidarEstrelas()
        {
            RuleFor(c => c.Estrelas)
                .GreaterThan(0).WithMessage("Número de estrelas dever ser maior que zero.")
                .LessThanOrEqualTo(5).WithMessage("Número de estrelas deve ser menor ou igual a cinco");
        }
    }
}
