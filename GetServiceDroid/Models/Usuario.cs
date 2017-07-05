namespace GetServiceDroid.Models
{
    public class Usuario
    {
        public string UserId { get; set; }

        public string NomeUsuario { get; set; }

        public string NomeCompleto { get; set; }

        public string Status { get; set; }

        public int CidadeId { get; set; }

        public int EstadoId { get; set; }

        public string Endereco { get; set; }

        public bool Profissional { get; set; }
    }
}