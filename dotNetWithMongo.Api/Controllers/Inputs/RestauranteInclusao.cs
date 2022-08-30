namespace dotNetWithMongo.Api.Controllers.Inputs
{
    public class RestauranteInclusao
    {
        public string Nome { get; private set; }

        public int Cozinha { get; private set; }

        public string Logradouro { get; private set; }

        public string Numero { get; private set; }

        public string Cidade { get; private set; }

        public string UF { get; private set; }

        public string Cep { get; private set; }
    }
}
