using GetServiceDroid.Models.Enums;

namespace GetServiceDroid.Models
{
    class EditarServico
    {
        public int Id { get; set; }

        public string Descricao { get; set; }
        
        public string Sobre { get; set; }

        public bool Ativo { get; set; }

        public int CategoriaId { get; set; }
        
        public int SubCategoriaId { get; set; }

        public TipoValor TipoValor { get; set; }

        public double Valor { get; set; }
    }
}