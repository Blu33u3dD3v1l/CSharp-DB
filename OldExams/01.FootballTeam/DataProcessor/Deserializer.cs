namespace Footballers.DataProcessor
{
    using Castle.Core.Internal;
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Numerics;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ImportCochesDto[]), new XmlRootAttribute("Coaches"));

            var stringReader = new StringReader(xmlString);

            var coachesDto = (ImportCochesDto[])xmlSerializer.Deserialize(stringReader);

            var allCoches = new List<Coach>();


            foreach (var coach in coachesDto)
            {
                if(!IsValid(coach) || coach.Nationality.IsNullOrEmpty())
                {
                    sb.AppendLine(ErrorMessage); 
                    continue;
                }

                var currentCoach = new Coach
                {
                    Name = coach.Name,
                    Nationality = coach.Nationality,
                };


              

                foreach (var footballer in coach.Footballers)
                {

                    DateTime footballerContractStartDate;
                    bool isFootballerContractStartDateValid = DateTime.TryParseExact(footballer.ContractStartDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out footballerContractStartDate);

                    DateTime footballerContractEndDate;
                    bool isFootballerContractEndDateValid = DateTime.TryParseExact(footballer.ContractEndDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out footballerContractEndDate);



                    if (!IsValid(footballer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var currentFootballer = new Footballer
                    {
                        Name = footballer.Name,
                        ContractStartDate = footballerContractStartDate,
                        ContractEndDate = footballerContractEndDate,
                        BestSkillType = (BestSkillType)Enum.Parse(typeof(BestSkillType), footballer.BestSkillType),
                        PositionType = (PositionType)Enum.Parse(typeof(PositionType), footballer.PositionType)
                    };


                    if(currentFootballer.ContractStartDate > currentFootballer.ContractEndDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    currentCoach.Footballers.Add(currentFootballer);

                }
                allCoches.Add(currentCoach);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, currentCoach.Name, currentCoach.Footballers.Count));
            }
            context.AddRange(allCoches);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {

            
            var teamDtos = JsonConvert.DeserializeObject<IEnumerable<ImportTeamDto>>(jsonString);

            var sb = new StringBuilder();

            var allTeams = new List<Team>();

            foreach (var team in teamDtos)
            {
                if(!IsValid(team) || team.Trophies == 0 || team.Nationality.IsNullOrEmpty())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var currentTeam = new Team
                {
                    Name = team.Name,
                    Nationality = team.Nationality,
                    Trophies = team.Trophies,
                };

                foreach (var footballer in team.Footballers.Distinct())
                {

                    if (!IsValid(footballer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var existingFootballer = context.Footballers.FirstOrDefault(x => x.Id == footballer);
                    if(existingFootballer == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    currentTeam.TeamsFootballers.Add(new TeamFootballer {FootballerId = existingFootballer.Id });
                }
                allTeams.Add(currentTeam);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, currentTeam.Name, currentTeam.TeamsFootballers.Count));
            }
            context.AddRange(allTeams);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
