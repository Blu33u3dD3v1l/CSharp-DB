namespace Footballers.Data.Models
{
    public class Coach
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Nationality { get; set; }

        public virtual ICollection<Footballer> Footballers { get; set; } = new HashSet<Footballer>();
    }
}



