using System;

namespace GetServiceDroid.Models
{
    class Mensagem
    {
        public string remetenteUserName { get; set; }

        public string DestinatarioUserName { get; set; }

        public string Texto { get; set; }

        public DateTime Data { get; set; }
    }
}