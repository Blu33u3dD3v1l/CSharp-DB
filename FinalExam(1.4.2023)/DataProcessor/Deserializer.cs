namespace Boardgames.DataProcessor
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Numerics;
    using System.Reflection.Metadata;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ImportCreatorsDto[]), new XmlRootAttribute("Creators"));

            var stringReader = new StringReader(xmlString);

            var creatorDtos = (ImportCreatorsDto[])xmlSerializer.Deserialize(stringReader);


            var allCreators = new List<Creator>();

            foreach (var creator in creatorDtos)
            {
                if (!IsValid(creator))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var currentCreator = new Creator
                {
                    FirstName = creator.FirstName,
                    LastName = creator.LastName,
                };

                foreach (var boardgame in creator.Boardgames)
                {
                    if (!IsValid(boardgame))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var currentBoardgame = new Boardgame
                    {
                        Name = boardgame.Name,
                        Rating = boardgame.Rating,
                        YearPublished = boardgame.YearPublished,
                        CategoryType = (CategoryType)Enum.Parse(typeof(CategoryType), boardgame.CategoryType),
                        Mechanics = boardgame.Mechanics,
                    };

                    currentCreator.Boardgames.Add(currentBoardgame);    
                }
                allCreators.Add(currentCreator);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, currentCreator.FirstName, currentCreator.LastName, currentCreator.Boardgames.Count));
            }

            context.AddRange(allCreators);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {

            var sb = new StringBuilder();
            var sellerDtos = JsonConvert.DeserializeObject<IEnumerable<ImportSellersDto>>(jsonString);

            var allSellers = new List<Seller>();

            foreach (var seller in sellerDtos)
            {
                if (!IsValid(seller))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var currentSeller = new Seller
                {
                    Name = seller.Name,
                    Address = seller.Address,
                    Country = seller.Country,
                    Website = seller.Website,
                };

                foreach (var boardgame in seller.Boardgames.Distinct())
                {
                    var existintBoardGame = context.Boardgames.Find(boardgame);
                    if (existintBoardGame == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    currentSeller.BoardgamesSellers.Add(new BoardgameSeller { BoardgameId = existintBoardGame.Id });
                }
                allSellers.Add(currentSeller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, currentSeller.Name, currentSeller.BoardgamesSellers.Count));
            }
            context.AddRange(allSellers);
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
