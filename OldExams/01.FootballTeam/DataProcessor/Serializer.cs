namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {

            var sb = new StringBuilder();
            XmlRootAttribute root = new XmlRootAttribute("Coaches");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCoachesDto[]), root);

            using StringWriter stringWriter = new StringWriter(sb);


            var a = context.Coaches
                .Where(x => x.Footballers.Any())
                .Select(x => new ExportCoachesDto()
                {
                    FootballersCount = x.Footballers.Count(),
                    CoachName = x.Name,
                    Footballers = x.Footballers.Select(y => new ExportFootballersDto()
                    {
                        Name = y.Name,
                        PositionType = y.PositionType.ToString()
                    })
                    .OrderBy(x => x.Name)
                    .ToArray()

                })
                .OrderByDescending(x => x.Footballers.Length)
                .ThenBy(x => x.CoachName)
                .ToArray();


                 xmlSerializer.Serialize(stringWriter, a, namespaces);

                 return sb.ToString().TrimEnd();

        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {

            var a = context.Teams
                .Where(x => x.TeamsFootballers.Any(x => x.Footballer.ContractStartDate >= date))
                .Select(x => new
                {
                    Name = x.Name,
                    Footballers = x.TeamsFootballers
                    .ToArray()
                    .Where(x => x.Footballer.ContractStartDate >= date)
                    .OrderByDescending(x => x.Footballer.ContractEndDate)
                    .ThenBy(x => x.Footballer.Name)
                    .Select(y => new
                    {
                        FootballerName = y.Footballer.Name,
                        ContractStartDate = y.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                        ContractEndDate = y.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                        BestSkillType = y.Footballer.BestSkillType.ToString(),
                        PositionType = y.Footballer.PositionType.ToString()
                    })
                    .ToArray()

                })
                .OrderByDescending(x => x.Footballers.Length)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToArray();

            var result = JsonConvert.SerializeObject(a, Formatting.Indented);
            return result;
        }
    }
}
