using System;
using System.Linq;

namespace dotNetWithMongo.Api.Domain.Enums
{
    public enum ECozinha
    {
        Brasiliera = 1,
        Italiana = 2,
        Arabe = 3,
        Japonesa = 4,
        FastFood = 5
    }

    public static class ECozinhaHelper
    {
        public static ECozinha ConverterDeInteiro(int valor)
        {
            if (Enum.IsDefined(typeof(ECozinha), valor))
                return (ECozinha)valor;

            throw new ArgumentOutOfRangeException(nameof(valor), valor, "Esta cozinha não existe");
        }
    }
}
