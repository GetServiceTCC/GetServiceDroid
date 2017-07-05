namespace GetServiceDroid.Models
{
    public class Profissional
    {
        public string UserName { get; set; }

        public string NomeCompleto { get; set; }

        public string Status { get; set; }

        public int CidadeId { get; set; }

        public string Cidade { get; set; }

        public int EstadoId { get; set; }

        public string Estado { get; set; }

        public string Uf { get; set; }

        public string Endereco { get; set; }

        public int QtdComentarios { get; set; }

        public double? Avaliacao { get; set; }
    }
}