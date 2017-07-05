namespace GetServiceDroid.Models
{
    public class Estado
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}