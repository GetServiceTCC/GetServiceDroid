using GetServiceDroid.Models.Enums;

namespace GetServiceDroid.Models
{
    public class Contato
    {
        public string UsuarioId { get; set; }

        public string UsuarioUserName { get; set; }

        public string UsuarioNomeCompleto { get; set; }

        public string UsuarioStatus { get; set; }

        public string ContatoId { get; set; }

        public string ContatoUserName { get; set; }

        public string ContatoNomeCompleto { get; set; }

        public string ContatoStatus { get; set; }

        public ContatoStatus Status { get; set; }
    }
}