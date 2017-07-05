namespace GetServiceDroid.Models
{
    public class Cidade
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public int EstadoId { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}