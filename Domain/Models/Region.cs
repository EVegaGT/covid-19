namespace Domain.Models
{
    public class Region
    {
        public string Iso { get; set; }
        public string Name { get; set; }

        public int Confirmed { get; set; }

        public int Deaths { get; set; }
    }
}
