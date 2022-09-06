using System.Collections.Generic;

namespace dotNetWithMongo.Api.Controllers.Outputs
{
    public class RestauranteTopTres
    {
        public string Id { get; set; }

        public string Nome { get; set; }

        public int Cozinha { get; set; }

        public string Cidade { get; set; }

        public double Media_Estrelas_Nota { get; set; }

        public List<string> Comentarios { get; set; }
    }
}
