using Boardgames.Data.Models.Enums;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Boardgames.Data.Models
{
    public class Boardgame
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Rating { get; set; }

        public int YearPublished { get; set; }

        public CategoryType CategoryType { get; set; }

        public string Mechanics { get; set; }

        public int CreatorId { get; set; }

        public Creator Creator { get; set;}

        public virtual ICollection<BoardgameSeller> BoardgamesSellers { get; set; } = new HashSet<BoardgameSeller>();

    }
}


