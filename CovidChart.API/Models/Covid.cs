namespace CovidChart.API.Models
{
    public enum ECity
    {
        Istanbul = 1,
        Ankara = 2,
        Izmir = 3,
        Kocaeli = 4,
        Gumushane = 5
    }
    public class Covid
    {
        public int Id { get; set; }
        public ECity  City{ get; set; }
        public int Count { get; set; }
        public DateTime CovidDate { get; set; }
    }
}
