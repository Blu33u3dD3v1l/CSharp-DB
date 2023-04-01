namespace Boardgames.Data.Models
{
    public class Creator
    {

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<Boardgame> Boardgames { get; set; } = new HashSet<Boardgame>();
    }
}



