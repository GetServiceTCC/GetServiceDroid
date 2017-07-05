namespace GetServiceDroid.Models
{
    public class RegistraUsuario
    {
        public string NomeUsuario { get; set; }
        
        public string NomeCompleto { get; set; }

        public string Status { get; set; }

        public int CidadeId { get; set; }

        public string Endereco { get; set; }
        
        public bool Profissional { get; set; }
        
        public string Senha { get; set; }
        
        public string ConfirmaSenha { get; set; }
    }
}