namespace GetServiceDroid.Models
{
    public class SubCategoria
    {
        public int Id { get; set; }
        
        public string Descricao { get; set; }
        
        public int CategoriaId { get; set; }

        public override string ToString()
        {
            return Descricao;
        }
    }
}