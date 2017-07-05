using System;

namespace GetServiceDroid.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public string UserName { get; set; }

        public string NomeCompleto { get; set; }

        public int ServicoId { get; set; }

        public DateTime Data { get; set; }

        public int Avaliacao { get; set; }
    }
}