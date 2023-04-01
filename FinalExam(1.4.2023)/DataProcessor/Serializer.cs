namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {

            var sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute("Creators");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCreatorDto[]), root);

            using StringWriter stringWriter = new StringWriter(sb);

            var a = context.Creators
                .Where(x => x.Boardgames.Any())
                .Select(x => new ExportCreatorDto
                {
                    BoardgamesCount = x.Boardgames.Count(),
                    CreatorName = x.FirstName + " " + x.LastName,
                    Boardgames = x.Boardgames.Select(x => new ExportBoardGamesDto
                    {
                        Name = x.Name,
                        YearPublished = x.YearPublished,
                    })
                    .OrderBy(x => x.Name)
                    .ToArray()
                })
                .OrderByDescending(x => x.BoardgamesCount)
                .ThenBy(x => x.CreatorName)
                .ToArray();

            xmlSerializer.Serialize(stringWriter, a, namespaces);

            return sb.ToString().TrimEnd();

        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {



            var a = context.Sellers
                .ToArray()
                .Where(x => x.BoardgamesSellers.Any(x => x.Boardgame.YearPublished >= year && x.Boardgame.Rating <= rating))
                .Select(x => new
                {
                    x.Name,
                    x.Website,
                    Boardgames = x.BoardgamesSellers
                    .ToArray()
                    .Where(x => x.Boardgame.YearPublished >= year && x.Boardgame.Rating <= rating)
                    .Select(x => new
                    {
                        Name = x.Boardgame.Name,
                        Rating = x.Boardgame.Rating,
                        Mechanics = x.Boardgame.Mechanics,
                        Category = x.Boardgame.CategoryType.ToString()
                    })
                    .OrderByDescending(x => x.Rating)
                    .ThenBy(x => x.Name)
                    .ToArray()

                })
                .OrderByDescending(x => x.Boardgames.Length)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToArray();



            var result = JsonConvert.SerializeObject(a, Formatting.Indented);
            return result;
        }
    }
}