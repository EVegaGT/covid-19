namespace Domain.Models
{
    public class Province
    {
        public string Iso { get; set; }
        public string Name { get; set; }
        public string ProvinceName { get; set; }

        public int Confirmed { get; set; }
        
        public int Deaths { get; set; }
    }
}
