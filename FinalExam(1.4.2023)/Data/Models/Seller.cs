using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net;

namespace Boardgames.Data.Models
{
    public class Seller
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string Website { get; set; }

        public virtual ICollection<BoardgameSeller> BoardgamesSellers { get; set; } = new HashSet<BoardgameSeller>();
    }
}



